using System;
using System.Collections.Generic;
using Lockstep.Logging;
using Lockstep.Math;
using Lockstep.UnsafeCollision2D;
using Debug = Lockstep.Logging.Debug;
#if UNITY_EDITOR
using UnityEngine;
#endif
// A Dynamic, Loose Octree for storing any objects that can be described with AABB bounds
// See also: PointOctree, where objects are stored as single points and some code can be simplified
// Octree:	An octree is a tree data structure which divides 3D space into smaller partitions (nodes)
//			and places objects into the appropriate nodes. This allows fast access to objects
//			in an area of interest without having to check every object.
// Dynamic: The octree grows or shrinks as required when objects as added or removed
//			It also splits and merges nodes as appropriate. There is no maximum depth.
//			Nodes have a constant - numObjectsAllowed - which sets the amount of items allowed in a node before it splits.
// Loose:	The octree's nodes can be larger than 1/2 their parent's length and width, so they overlap to some extent.
//			This can alleviate the problem of even tiny objects ending up in large nodes if they're near boundaries.
//			A looseness value of 1.0 will make it a "normal" octree.
// T:		The content of the octree can be anything, since the bounds data is supplied separately.

// Originally written for my game Scraps (http://www.scrapsgame.com) but intended to be general-purpose.
// Copyright 2014 Nition, BSD licence (see LICENCE file). www.momentstudio.co.nz
// Unity-based, but could be adapted to work in pure C#

// Note: For loops are often used here since in some cases (e.g. the IsColliding method)
// they actually give much better performance than using Foreach, even in the compiled build.
// Using a LINQ expression is worse again than Foreach.
namespace Lockstep.Collision2D {
    public static partial class LRectExt {
        public static LVector2 ToLVector2XZ(this LVector3 vec){
            return new LVector2(vec.x, vec.z);
        }

        public static LVector3 ToLVector3(this LVector2 vec, int y = 1){
            return new LVector3(vec.x, y.ToLFloat(), vec.y);
        }
        public static LVector3 ToLVector3XZ(this LVector2 vec,LFloat y){
            return new LVector3(vec.x, y,vec.y);
        }
    }

    public partial class BoundsQuadTree  {
        // The total amount of objects currently in the tree
        public int Count { get; private set; }

        // Root node of the octree
        BoundsQuadTreeNode rootNode;

        public delegate bool FuncCheckCanCollide(ColliderProxy a, ColliderProxy b);
        public delegate void FuncOnCollide(ColliderProxy a, ColliderProxy b);

        public static FuncCheckCanCollide FuncCanCollide = (a, b) => true;
        public static FuncOnCollide funcOnCollide = (a, b) => { };

        // Should be a value between 1 and 2. A multiplier for the base size of a node.
        // 1.0 is a "normal" octree, while values > 1 have overlap
        readonly LFloat looseness;

        // Size that the octree was on creation
        readonly LFloat initialSize;

        // Minimum side length that a node can be - essentially an alternative to having a max depth
        readonly LFloat minSize;
        // For collision visualisation. Automatically removed in builds.
#if UNITY_EDITOR
        const int numCollisionsToSave = 4;
        readonly Queue<LRect> lastBoundsCollisionChecks = new Queue<LRect>();
        readonly Queue<Ray> lastRayCollisionChecks = new Queue<Ray>();
#endif

        /// <summary>
        /// Constructor for the bounds octree.
        /// </summary>
        /// <param name="initialWorldSize">Size of the sides of the initial node, in metres. The octree will never shrink smaller than this.</param>
        /// <param name="initialWorldPos">Position of the centre of the initial node.</param>
        /// <param name="minNodeSize">Nodes will stop splitting if the new nodes would be smaller than this (metres).</param>
        /// <param name="loosenessVal">Clamped between 1 and 2. Values > 1 let nodes overlap.</param>
        public BoundsQuadTree(LFloat initialWorldSize, LVector2 initialWorldPos, LFloat minNodeSize, LFloat loosenessVal){
            if (minNodeSize > initialWorldSize) {
                Debug.Log("Minimum node size must be at least as big as the initial world size. Was: " +
                                 minNodeSize + " Adjusted to: " + initialWorldSize);
                minNodeSize = initialWorldSize;
            }

            Count = 0;
            initialSize = initialWorldSize;
            minSize = minNodeSize;
            looseness = LMath.Clamp(loosenessVal, 1.ToLFloat(), 2.ToLFloat());
            rootNode = new BoundsQuadTreeNode(null, initialSize, minSize, looseness, initialWorldPos);
        }

        public void UpdateObj(ColliderProxy obj, LRect bound){
            Debug.Trace($"ColliderProxy UpdateObj { obj.Id} objBounds  {bound}");
            var node = GetNode(obj);
            if (node == null) {
                Add(obj, bound);
            }
            else {
                if (!node.ContainBound(bound)) {
                    Remove(obj);
                    Add(obj, bound);
                }
                else {
                    node.UpdateObj(obj, bound);
                }
            }
        }
        // #### PUBLIC METHODS ####


        // #### PUBLIC METHODS ####
        public BoundsQuadTreeNode GetNode(ColliderProxy obj){
            if (BoundsQuadTreeNode.obj2Node.TryGetValue(obj, out var val)) {
                return val;
            }

            return null;
        }

        /// <summary>
        /// Add an object.
        /// </summary>
        /// <param name="obj">Object to add.</param>
        /// <param name="objBounds">3D bounding box around the object.</param>
        public void Add(ColliderProxy obj, LRect objBounds){
            Debug.Trace($"ColliderProxy Add { obj.Id} objBounds  {objBounds}");
            // Add object or expand the octree until it can be added
            int count = 0; // Safety check against infinite/excessive growth
            while (!rootNode.Add(obj, objBounds)) {
                Debug.LogError("Grow");
                Grow(objBounds.center - rootNode.Center);
                if (++count > 20) {
                    Debug.LogError("Aborted Add operation as it seemed to be going on forever (" + (count - 1) +
                                   ") attempts at growing the octree.");
                    return;
                }
            }

            Count++;
        }

        /// <summary>
        /// Remove an object. Makes the assumption that the object only exists once in the tree.
        /// </summary>
        /// <param name="obj">Object to remove.</param>
        /// <returns>True if the object was removed successfully.</returns>
        public bool Remove(ColliderProxy obj){
            Debug.Trace($"ColliderProxy Remove { obj.Id} ");
            bool removed = rootNode.Remove(obj);

            // See if we can shrink the octree down now that we've removed the item
            if (removed) {
                Count--;
                Shrink();
            }

            return removed;
        }

        /// <summary>
        /// Removes the specified object at the given position. Makes the assumption that the object only exists once in the tree.
        /// </summary>
        /// <param name="obj">Object to remove.</param>
        /// <param name="objBounds">3D bounding box around the object.</param>
        /// <returns>True if the object was removed successfully.</returns>
        public bool Remove(ColliderProxy obj, LRect objBounds){
            Debug.Trace($"ColliderProxy Add { obj.Id} objBounds  {objBounds}");
            bool removed = rootNode.Remove(obj, objBounds);

            // See if we can shrink the octree down now that we've removed the item
            if (removed) {
                Count--;
                Shrink();
            }

            return removed;
        }

        /// <summary>
        /// Check if the specified bounds intersect with anything in the tree. See also: GetColliding.
        /// </summary>
        /// <param name="checkBounds">bounds to check.</param>
        /// <returns>True if there was a collision.</returns>
        public bool IsColliding(ColliderProxy obj, LRect checkBounds){
            //#if UNITY_EDITOR
            // For debugging
            //AddCollisionCheck(checkBounds);
            //#endif
            return rootNode.IsColliding(obj, ref checkBounds);
        }

        public void CheckCollision(ColliderProxy obj, LRect checkBounds){
            rootNode.CheckCollision(obj, ref checkBounds);
        }
        public void CheckCollision(ref LRect checkBounds, FuncCollision callback){
            rootNode.CheckCollision(ref checkBounds, callback);
        }
        public void CheckCollision( LVector2 pos, LFloat radius, FuncCollision callback){
            var rect = LRect.CreateRect(pos,new LVector2(radius,radius));
            rootNode.CheckCollision(rect, callback);
        }

       
        public bool Raycast(Ray2D checkRay, LFloat maxDistance,out LFloat t,out int id) {
            return rootNode.CheckCollision(ref checkRay, maxDistance,out t,out id);
        }
        /// <summary>
        /// Returns an array of objects that intersect with the specified bounds, if any. Otherwise returns an empty array. See also: IsColliding.
        /// </summary>
        /// <param name="collidingWith">list to store intersections.</param>
        /// <param name="checkBounds">bounds to check.</param>
        /// <returns>Objects that intersect with the specified bounds.</returns>
        public void GetColliding(List<ColliderProxy> collidingWith, LRect checkBounds){
            //#if UNITY_EDITOR
            // For debugging
            //AddCollisionCheck(checkBounds);
            //#endif
            rootNode.GetColliding(ref checkBounds, collidingWith);
        }


        public LRect GetMaxBounds(){
            return rootNode.GetBounds();
        }
#if UNITY_EDITOR
        /// <summary>
        /// Draws node boundaries visually for debugging.
        /// Must be called from OnDrawGizmos externally. See also: DrawAllObjects.
        /// </summary>
        public void DrawAllBounds(){
            rootNode.DrawBoundQuadTreeNode(0);
        }

        /// <summary>
        /// Draws the bounds of all objects in the tree visually for debugging.
        /// Must be called from OnDrawGizmos externally. See also: DrawAllBounds.
        /// </summary>
        public void DrawAllObjects(){
            rootNode.DrawAllObjects();
        }
#endif

        public const int NUM_CHILDREN = 4;

        /// <summary>
        /// Grow the octree to fit in all objects.
        /// </summary>
        /// <param name="direction">Direction to grow.</param>
        void Grow(LVector2 direction){
            Debug.Trace("Grow");
            int xDirection = direction.x >= 0 ? 1 : -1;
            int yDirection = direction.y >= 0 ? 1 : -1;
            BoundsQuadTreeNode oldRoot = rootNode;
            LFloat half = rootNode.BaseLength / 2;
            LFloat newLength = rootNode.BaseLength * 2;
            LVector2 newCenter = rootNode.Center + new LVector2(xDirection * half, yDirection * half);

            // Create a new, bigger octree root node
            rootNode = new BoundsQuadTreeNode(null, newLength, minSize, looseness, newCenter);

            if (oldRoot.HasAnyObjects()) {
                // Create 7 new octree children to go with the old root as children of the new root
                int rootPos = rootNode.BestFitChild(oldRoot.Center);
                BoundsQuadTreeNode[] children = new BoundsQuadTreeNode[NUM_CHILDREN];
                for (int i = 0; i < NUM_CHILDREN; i++) {
                    if (i == rootPos) {
                        children[i] = oldRoot;
                    }
                    else {
                        xDirection = i % 2 == 0 ? -1 : 1;
                        yDirection = i > 1 ? -1 : 1;
                        children[i] = new BoundsQuadTreeNode(rootNode, oldRoot.BaseLength, minSize, looseness,
                            newCenter + new LVector2(xDirection * half, yDirection * half));
                    }
                }

                // Attach the new children to the new root node
                rootNode.SetChildren(children);
            }
        }

        /// <summary>
        /// Shrink the octree if possible, else leave it the same.
        /// </summary>
        void Shrink(){
            Debug.Trace("Shrink");
            rootNode = rootNode.ShrinkIfPossible(initialSize);
        }
    }
}