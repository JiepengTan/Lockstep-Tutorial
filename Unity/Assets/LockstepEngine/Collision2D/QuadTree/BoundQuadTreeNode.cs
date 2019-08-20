//#define SHOW_TREE_NODES //层级显示所有节点
#if SHOW_TREE_NODES && UNITY_EDITOR
#define SHOW_NODES
#endif
#if UNITY_EDITOR
using UnityEngine;
#endif
using Debug = Lockstep.Logging.Debug;

using System;
using System.Collections.Generic;
using Lockstep.Logging;
using Lockstep.Math;
using Lockstep.UnsafeCollision2D;
namespace Lockstep.Collision2D {
// A node in a BoundsOctree
// Copyright 2014 Nition, BSD licence (see LICENCE file). www.momentstudio.co.nz
    public partial class BoundsQuadTreeNode {
        public BoundsQuadTreeNode parent;
#if SHOW_NODES
        public Transform monoTrans;
#endif
        // Centre of this node
        public LVector2 Center { get; private set; }

        // Length of this node if it has a looseness of 1.0
        public LFloat BaseLength { get; private set; }

        // Looseness value for this node
        LFloat looseness;

        // Minimum size for a node in this octree
        LFloat minSize;

        // Actual length of sides, taking the looseness value into account
        LFloat adjLength;

        // Bounding box that represents this node
        LRect bounds = default(LRect);

        // Objects in this node
        readonly List<OctreeObject> objects = new List<OctreeObject>();

        // Child nodes, if any
        BoundsQuadTreeNode[] children = null;

        bool HasChildren {
            get { return children != null; }
        }

        // Bounds of potential children to this node. These are actual size (with looseness taken into account), not base size
        LRect[] childBounds;

        // If there are already NUM_OBJECTS_ALLOWED in a node, we split it into children
        // A generally good number seems to be something around 8-15
        const int NUM_OBJECTS_ALLOWED = 6;
        const int NUM_CHILDREN = BoundsQuadTree.NUM_CHILDREN;

        // An object in the octree
        struct OctreeObject {
            public ColliderProxy Obj;
            public LRect Bounds;
        }

        public static int MonoID = 0;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="baseLengthVal">Length of this node, not taking looseness into account.</param>
        /// <param name="minSizeVal">Minimum size of nodes in this octree.</param>
        /// <param name="loosenessVal">Multiplier for baseLengthVal to get the actual size.</param>
        /// <param name="centerVal">Centre position of this node.</param>
        public BoundsQuadTreeNode(BoundsQuadTreeNode parent, LFloat baseLengthVal, LFloat minSizeVal,
            LFloat loosenessVal, LVector2 centerVal){
#if SHOW_NODES
            monoTrans = new GameObject(MonoID++.ToString()).transform;
            if (parent != null) {
                monoTrans.SetParent(parent.monoTrans, false);
            }

            monoTrans.position = centerVal.ToLVector3().ToVector3();
#endif
            this.parent = parent;
            SetValues(baseLengthVal, minSizeVal, loosenessVal, centerVal);
        }


        /// <summary>
        /// Add an object.
        /// </summary>
        /// <param name="obj">Object to add.</param>
        /// <param name="objBounds">3D bounding box around the object.</param>
        /// <returns>True if the object fits entirely within this node.</returns>
        public bool Add(ColliderProxy obj, LRect objBounds){
            if (!Encapsulates(bounds, objBounds)) {
                return false;
            }

            SubAdd(obj, objBounds);
            return true;
        }

        /// <summary>
        /// Remove an object. Makes the assumption that the object only exists once in the tree.
        /// </summary>
        /// <param name="obj">Object to remove.</param>
        /// <returns>True if the object was removed successfully.</returns>
        public bool Remove(ColliderProxy obj){
            if (obj2Node.TryGetValue(obj, out var val)) {
                obj2Node.Remove(obj);
                bool removed = false;
                for (int i = 0; i < val.objects.Count; i++) {
                    if (ReferenceEquals(val.objects[i].Obj, obj)) {
                        val.objects.RemoveAt(i);
                        removed = true;
                        break;
                    }
                }

                if (removed) {
                    val.OnRemoved();
                }

                return removed;
            }

            return false;
        }


        public bool ContainBound(LRect bound){
            return Encapsulates(bounds, bound);
        }

        public void UpdateObj(ColliderProxy obj, LRect bound){
            for (int i = 0; i < objects.Count; i++) {
                if (ReferenceEquals(objects[i].Obj, obj)) {
                    objects[i] = new OctreeObject() {Obj = obj, Bounds = bound};
                }
            }
        }

        public void OnRemoved(){
            if (ShouldMerge()) {
                Merge();
            }

            parent?.OnRemoved();
        }

        /// <summary>
        /// Removes the specified object at the given position. Makes the assumption that the object only exists once in the tree.
        /// </summary>
        /// <param name="obj">Object to remove.</param>
        /// <param name="objBounds">3D bounding box around the object.</param>
        /// <returns>True if the object was removed successfully.</returns>
        public bool Remove(ColliderProxy obj, LRect objBounds){
            if (!Encapsulates(bounds, objBounds)) {
                return false;
            }

            return false;
        }

        /// <summary>
        /// Check if the specified bounds intersect with anything in the tree. See also: GetColliding.
        /// </summary>
        /// <param name="checkBounds">Bounds to check.</param>
        /// <returns>True if there was a collision.</returns>
        public bool IsColliding(ColliderProxy obj, ref LRect checkBounds){
            // Are the input bounds at least partially in this node?
            if (!bounds.Overlaps(checkBounds)) {
                return false;
            }

            // Check against any objects in this node
            for (int i = 0; i < objects.Count; i++) {
                var o = objects[i];
                if (!ReferenceEquals(o.Obj, obj) && o.Bounds.Overlaps(checkBounds)) {
                    return true;
                }
            }

            // Check children
            if (children != null) {
                for (int i = 0; i < NUM_CHILDREN; i++) {
                    if (children[i].IsColliding(obj, ref checkBounds)) {
                        return true;
                    }
                }
            }

            return false;
        }


        public bool CheckCollision(LRect checkBounds, FuncCollision callback){
            return CheckCollision(ref checkBounds, callback);
        }

        public bool CheckCollision(ref LRect checkBounds, FuncCollision callback){
            // Are the input bounds at least partially in this node?
            if (!bounds.Overlaps(checkBounds)) {
                return false;
            }

            // Check against any objects in this node
            for (int i = 0; i < objects.Count; i++) {
                var o = objects[i];
                if (o.Bounds.Overlaps(checkBounds)) {
                    callback(o.Obj);
                }
            }
            // Check children
            if (children != null) {
                for (int i = 0; i < NUM_CHILDREN; i++) {
                    if (children[i].CheckCollision(ref checkBounds, callback)) {
                        return true;
                    }
                }
            }

            return false;
        }

        public void CheckCollision(ColliderProxy obj, ref LRect checkBounds){
            // Are the input bounds at least partially in this node?
            if (!bounds.Overlaps(checkBounds)) {
                return;
            }

            // Check against any objects in this node
            for (int i = 0; i < objects.Count; i++) {
                var o = objects[i];
                if (!ReferenceEquals(o.Obj, obj)
                    && BoundsQuadTree.FuncCanCollide(o.Obj, obj)
                    && o.Bounds.Overlaps(checkBounds)
                ) {
                    BoundsQuadTree.funcOnCollide(obj, o.Obj);
                }
            }

            // Check children
            if (children != null) {
                for (int i = 0; i < NUM_CHILDREN; i++) {
                    children[i].CheckCollision(obj, ref checkBounds);
                }
            }
        }

        public bool CheckCollision(ref Ray2D checkRay, LFloat maxDistance, out LFloat t, out int id){
            t = maxDistance;
            id = int.MaxValue;
            LFloat distance;
            bool hasCollider = false;
            if (!bounds.IntersectRay(checkRay, out distance) || distance > maxDistance) {
                return false;
            }

            // Check against any objects in this node
            for (int i = 0; i < objects.Count; i++) {
                if (objects[i].Bounds.IntersectRay(checkRay, out distance)) {
                    if (distance < t) {
                        t = distance;
                        id = objects[i].Obj.Id;
                        hasCollider = true;
                        var vv = objects[i].Bounds.IntersectRay(checkRay, out distance);
                    }
                }
            }

            // Check children
            if (children != null) {
                for (int i = 0; i < NUM_CHILDREN; i++) {
                    if (children[i].CheckCollision(ref checkRay, t, out distance, out var tid)) {
                        if (distance < t) {
                            t = distance;
                            id = tid;
                            hasCollider = true;
                        }
                    }
                }
            }

            return hasCollider;
        }


        /// <summary>
        /// Returns an array of objects that intersect with the specified bounds, if any. Otherwise returns an empty array. See also: IsColliding.
        /// </summary>
        /// <param name="checkBounds">Bounds to check. Passing by ref as it improves performance with structs.</param>
        /// <param name="result">List result.</param>
        /// <returns>Objects that intersect with the specified bounds.</returns>
        public void GetColliding(ref LRect checkBounds, List<ColliderProxy> result){
            // Are the input bounds at least partially in this node?
            if (!bounds.Overlaps(checkBounds)) {
                return;
            }

            // Check against any objects in this node
            for (int i = 0; i < objects.Count; i++) {
                if (objects[i].Bounds.Overlaps(checkBounds)) {
                    result.Add(objects[i].Obj);
                }
            }

            // Check children
            if (children != null) {
                for (int i = 0; i < NUM_CHILDREN; i++) {
                    children[i].GetColliding(ref checkBounds, result);
                }
            }
        }


        /// <summary>
        /// Set the 8 children of this octree.
        /// </summary>
        /// <param name="childQuadTrees">The 8 new child nodes.</param>
        public void SetChildren(BoundsQuadTreeNode[] childQuadTrees){
            if (childQuadTrees.Length != NUM_CHILDREN) {
                Debug.LogError("Child octree array must be length 8. Was length: " + childQuadTrees.Length);
                return;
            }

            children = childQuadTrees;
#if SHOW_NODES
            foreach (var child in childQuadTrees) {
                child.monoTrans.SetParent(monoTrans, false);
            }
#endif
        }

        public LRect GetBounds(){
            return bounds;
        }


        /// <summary>
        /// We can shrink the octree if:
        /// - This node is >= double minLength in length
        /// - All objects in the root node are within one octant
        /// - This node doesn't have children, or does but 7/8 children are empty
        /// We can also shrink it if there are no objects left at all!
        /// </summary>
        /// <param name="minLength">Minimum dimensions of a node in this octree.</param>
        /// <returns>The new root, or the existing one if we didn't shrink.</returns>
        public BoundsQuadTreeNode ShrinkIfPossible(LFloat minLength){
            if (BaseLength < (2 * minLength)) {
                return this;
            }

            if (objects.Count == 0 && (children == null || children.Length == 0)) {
                return this;
            }

            // Check objects in root
            int bestFit = -1;
            for (int i = 0; i < objects.Count; i++) {
                OctreeObject curObj = objects[i];
                int newBestFit = BestFitChild(curObj.Bounds.center);
                if (i == 0 || newBestFit == bestFit) {
                    // In same octant as the other(s). Does it fit completely inside that octant?
                    if (Encapsulates(childBounds[newBestFit], curObj.Bounds)) {
                        if (bestFit < 0) {
                            bestFit = newBestFit;
                        }
                    }
                    else {
                        // Nope, so we can't reduce. Otherwise we continue
                        return this;
                    }
                }
                else {
                    return this; // Can't reduce - objects fit in different octants
                }
            }

            // Check objects in children if there are any
            if (children != null) {
                bool childHadContent = false;
                for (int i = 0; i < children.Length; i++) {
                    if (children[i].HasAnyObjects()) {
                        if (childHadContent) {
                            return this; // Can't shrink - another child had content already
                        }

                        if (bestFit >= 0 && bestFit != i) {
                            return this; // Can't reduce - objects in root are in a different octant to objects in child
                        }

                        childHadContent = true;
                        bestFit = i;
                    }
                }
            }

            // Can reduce
            if (children == null) {
                // We don't have any children, so just shrink this node to the new size
                // We already know that everything will still fit in it
                SetValues(BaseLength / 2, minSize, looseness, childBounds[bestFit].center);
                return this;
            }

            // No objects in entire octree
            if (bestFit == -1) {
                return this;
            }

            // We have children. Use the appropriate child as the new root node
            return children[bestFit];
        }

        /// <summary>
        /// Find which child node this object would be most likely to fit in.
        /// </summary>
        /// <param name="objBounds">The object's bounds.</param>
        /// <returns>One of the eight child octants.</returns>
        public int BestFitChild(LVector2 objBoundsCenter){
            return (objBoundsCenter.x <= Center.x ? 0 : 1) + (objBoundsCenter.y <= Center.y ? 0 : 2);
        }

        /// <summary>
        /// Checks if this node or anything below it has something in it.
        /// </summary>
        /// <returns>True if this node or any of its children, grandchildren etc have something in them</returns>
        public bool HasAnyObjects(){
            if (objects.Count > 0) return true;

            if (children != null) {
                for (int i = 0; i < NUM_CHILDREN; i++) {
                    if (children[i].HasAnyObjects()) return true;
                }
            }

            return false;
        }

        /*
        /// <summary>
        /// Get the total amount of objects in this node and all its children, grandchildren etc. Useful for debugging.
        /// </summary>
        /// <param name="startingNum">Used by recursive calls to add to the previous total.</param>
        /// <returns>Total objects in this node and its children, grandchildren etc.</returns>
        public int GetTotalObjects(int startingNum = 0) {
            int totalObjects = startingNum + objects.Count;
            if (children != null) {
                for (int i = 0; i < 8; i++) {
                    totalObjects += children[i].GetTotalObjects();
                }
            }
            return totalObjects;
        }
        */

        // #### PRIVATE METHODS ####

        /// <summary>
        /// Set values for this node. 
        /// </summary>
        /// <param name="baseLengthVal">Length of this node, not taking looseness into account.</param>
        /// <param name="minSizeVal">Minimum size of nodes in this octree.</param>
        /// <param name="loosenessVal">Multiplier for baseLengthVal to get the actual size.</param>
        /// <param name="centerVal">Centre position of this node.</param>
        void SetValues(LFloat baseLengthVal, LFloat minSizeVal, LFloat loosenessVal, LVector2 centerVal){
            BaseLength = baseLengthVal;
            minSize = minSizeVal;
            looseness = loosenessVal;
            Center = centerVal;
            adjLength = looseness * baseLengthVal;

            // Create the bounding box.
            LVector2 size = new LVector2(adjLength, adjLength);
            bounds = CreateLRect(Center, size);

            LFloat quarter = BaseLength / 4;
            LFloat childActualLength = (BaseLength / 2) * looseness;
            LVector2 childActualSize = new LVector2(childActualLength, childActualLength);
            childBounds = new LRect[NUM_CHILDREN];
            childBounds[0] = CreateLRect(Center + new LVector2(-quarter, -quarter), childActualSize);
            childBounds[1] = CreateLRect(Center + new LVector2(quarter, -quarter), childActualSize);
            childBounds[2] = CreateLRect(Center + new LVector2(-quarter, quarter), childActualSize);
            childBounds[3] = CreateLRect(Center + new LVector2(quarter, quarter), childActualSize);
        }

        LRect CreateLRect(LVector2 center, LVector2 size){
            return new LRect(center - size / 2, size);
        }


        public static Dictionary<ColliderProxy, BoundsQuadTreeNode> obj2Node =
            new Dictionary<ColliderProxy, BoundsQuadTreeNode>();

        /// <summary>
        /// Private counterpart to the public Add method.
        /// </summary>
        /// <param name="obj">Object to add.</param>
        /// <param name="objBounds">3D bounding box around the object.</param>
        void SubAdd(ColliderProxy obj, LRect objBounds){
            // We know it fits at this level if we've got this far

            // We always put things in the deepest possible child
            // So we can skip some checks if there are children aleady
            if (!HasChildren) {
                // Just add if few objects are here, or children would be below min size
                if (objects.Count < NUM_OBJECTS_ALLOWED || (BaseLength / 2) < minSize) {
                    OctreeObject newObj = new OctreeObject {Obj = obj, Bounds = objBounds};
                    objects.Add(newObj);
#if SHOW_NODES
                    obj.UnityTransform?.SetParent(monoTrans, true);
#endif

                    obj2Node[obj] = this;
                    return; // We're done. No children yet
                }

                // Fits at this level, but we can go deeper. Would it fit there?
                // Create the 8 children
                int bestFitChild;
                if (children == null) {
                    Split();
                    if (children == null) {
                        Debug.LogError("Child creation failed for an unknown reason. Early exit.");
                        return;
                    }

                    // Now that we have the new children, see if this node's existing objects would fit there
                    for (int i = objects.Count - 1; i >= 0; i--) {
                        OctreeObject existingObj = objects[i];
                        // Find which child the object is closest to based on where the
                        // object's center is located in relation to the octree's center
                        bestFitChild = BestFitChild(existingObj.Bounds.center);
                        // Does it fit?
                        if (Encapsulates(children[bestFitChild].bounds, existingObj.Bounds)) {
                            children[bestFitChild]
                                .SubAdd(existingObj.Obj, existingObj.Bounds); // Go a level deeper					
                            objects.Remove(existingObj); // Remove from here
                        }
                    }
                }
            }

            // Handle the new object we're adding now
            int bestFit = BestFitChild(objBounds.center);
            if (Encapsulates(children[bestFit].bounds, objBounds)) {
                children[bestFit].SubAdd(obj, objBounds);
            }
            else {
                // Didn't fit in a child. We'll have to it to this node instead
                OctreeObject newObj = new OctreeObject {Obj = obj, Bounds = objBounds};
                objects.Add(newObj);
                obj2Node[obj] = this;
            }
        }

        /// <summary>
        /// Private counterpart to the public <see cref="Remove(ColliderProxy, LRect)"/> method.
        /// </summary>
        /// <param name="obj">Object to remove.</param>
        /// <param name="objBounds">3D bounding box around the object.</param>
        /// <returns>True if the object was removed successfully.</returns>
        bool SubRemove(ColliderProxy obj, LRect objBounds){
            bool removed = false;

            for (int i = 0; i < objects.Count; i++) {
                if (ReferenceEquals(objects[i].Obj, obj)) {
                    removed = objects.Remove(objects[i]);
                    break;
                }
            }

            if (!removed && children != null) {
                int bestFitChild = BestFitChild(objBounds.center);
                removed = children[bestFitChild].SubRemove(obj, objBounds);
            }

            if (removed && children != null) {
                // Check if we should merge nodes now that we've removed an item
                if (ShouldMerge()) {
                    Merge();
                }
            }

            return removed;
        }

        /// <summary>
        /// Splits the octree into eight children.
        /// </summary>
        void Split(){
            LFloat quarter = BaseLength / 4;
            LFloat newLength = BaseLength / 2;
            children = new BoundsQuadTreeNode[NUM_CHILDREN];
            children[0] = new BoundsQuadTreeNode(this, newLength, minSize, looseness,
                Center + new LVector2(-quarter, -quarter));
            children[1] = new BoundsQuadTreeNode(this, newLength, minSize, looseness,
                Center + new LVector2(quarter, -quarter));
            children[2] = new BoundsQuadTreeNode(this, newLength, minSize, looseness,
                Center + new LVector2(-quarter, quarter));
            children[3] = new BoundsQuadTreeNode(this, newLength, minSize, looseness,
                Center + new LVector2(quarter, quarter));
        }

        /// <summary>
        /// Merge all children into this node - the opposite of Split.
        /// Note: We only have to check one level down since a merge will never happen if the children already have children,
        /// since THAT won't happen unless there are already too many objects to merge.
        /// </summary>
        void Merge(){
            // Note: We know children != null or we wouldn't be merging
            if (children == null) return;
            for (int i = 0; i < NUM_CHILDREN; i++) {
                BoundsQuadTreeNode curChild = children[i];
                int numObjects = curChild.objects.Count;
                for (int j = numObjects - 1; j >= 0; j--) {
                    OctreeObject curObj = curChild.objects[j];
                    objects.Add(curObj);
                    obj2Node[curObj.Obj] = this;
                }
#if SHOW_NODES
                var childCount = curChild.monoTrans.childCount;
                for (int j = childCount - 1; j >= 0; j--) {
                    var trans = curChild.monoTrans.GetChild(j);
                    trans.SetParent(monoTrans, true);
                }

                Debug.Assert(curChild.monoTrans.childCount == 0);
                UnityEngine.Object.Destroy(curChild.monoTrans.gameObject);
#endif
            }

            // Remove the child nodes (and the objects in them - they've been added elsewhere now)
            children = null;
        }

        /// <summary>
        /// Checks if outerBounds encapsulates innerBounds.
        /// </summary>
        /// <param name="outerBounds">Outer bounds.</param>
        /// <param name="innerBounds">Inner bounds.</param>
        /// <returns>True if innerBounds is fully encapsulated by outerBounds.</returns>
        static bool Encapsulates(LRect outerBounds, LRect innerBounds){
            return outerBounds.Contains(innerBounds.min) && outerBounds.Contains(innerBounds.max);
        }

        /// <summary>
        /// Checks if there are few enough objects in this node and its children that the children should all be merged into this.
        /// </summary>
        /// <returns>True there are less or the same abount of objects in this and its children than numObjectsAllowed.</returns>
        bool ShouldMerge(){
            int totalObjects = objects.Count;
            if (children != null) {
                foreach (BoundsQuadTreeNode child in children) {
                    if (child.children != null) {
                        // If any of the *children* have children, there are definitely too many to merge,
                        // or the child woudl have been merged already
                        return false;
                    }

                    totalObjects += child.objects.Count;
                }
            }

            return totalObjects <= NUM_OBJECTS_ALLOWED;
        }
    }
}