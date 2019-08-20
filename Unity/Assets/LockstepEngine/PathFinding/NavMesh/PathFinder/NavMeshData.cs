using System;
using System.Collections.Generic;
using Lockstep.Math;

namespace Lockstep.PathFinding {
    [Serializable]
    public class NavMeshData {
        public LFloat agentRadius = LFloat.half;
        private static long serialVersionUID = 1L;

        /** 行走区顶点序号 */
        public int[] pathTriangles;

        /** 行走区坐标 */
        public LVector3[] pathVertices;

        /** 开始坐标 */
        public LFloat startX;

        public LFloat startZ;

        /** 结束坐标 */
        public LFloat endX;

        public LFloat endZ;

        /** navmesh地图id */
        public int mapID;

        public LFloat width; // 宽
        public LFloat height; // 高

        /**
         * 数据检测，客户端的顶点坐标和三角形数据有可能是重复的ç∂
         * TODO 小三角形合并成大三角形或多边形；判断顶点是否在寻路层中，寻路层中的顶点不能作为路径点；两点所连线段是否穿过阻挡区，不穿过，直接获取坐标点
         */
        public void check(int scale){
            amendmentSameVector(pathTriangles, pathVertices);
            scaleVector(pathVertices, scale);

            this.width = LMath.Abs(this.getEndX() - this.getStartX());
            this.height = LMath.Abs(this.getEndZ() - this.getStartZ());
        }

        /**
         * 缩放向量
         */
        protected void scaleVector(LVector3[] vertices, int scale){
            if (vertices == null || scale == 1) {
                return;
            }

            var lscale = scale.ToLFloat();
            for (int i = 0; i < vertices.Length; i++) {
                vertices[i].x += (-this.startX); // 缩放移动
                vertices[i].z += (-this.startZ);
                vertices[i] = vertices[i] * lscale;
            }
        }

        /**
         * 修正重复坐标，使坐标相同的下标修改为一致
         * <p>
         * unity的NavMeshData有一些共边的三角形，共边的三角形其实不是连通关系，共边的三角形只是他们共同构成一个凸多边形，并且这种共边的三角形，全部都是扇形排列。
         * </p>
         */
        public void amendmentSameVector(int[] indexs, LVector3[] vertices){
            if (indexs == null || vertices == null) {
                return;
            }

            Dictionary<LVector3, int> map = new Dictionary<LVector3, int>();
            // 检测路径重复点
            for (int i = 0; i < vertices.Length; i++) {
                // 重复出现的坐标
                if (map.ContainsKey(vertices[i])) {
                    for (int j = 0; j < indexs.Length; j++) {
                        if (indexs[j] == i) { // 修正重复的坐标
                            // System.out.println(String.format("坐标重复为%s",
                            // indexs[j],i,vertices[i].ToString()));
                            indexs[j] = map.get(vertices[i]);
                        }
                    }

                    // vertices[i] = null;
                }
                else {
                    map.Add(vertices[i], i);
                }
            }
        }


        public int[] GetPathTriangles(){
            return pathTriangles;
        }

        public void setPathTriangles(int[] pathTriangles){
            this.pathTriangles = pathTriangles;
        }

        public LVector3[] GetPathVertices(){
            return pathVertices;
        }

        public void setPathVertices(LVector3[] pathVertices){
            this.pathVertices = pathVertices;
        }

        public LFloat getStartX(){
            return startX;
        }

        public void setStartX(LFloat startX){
            this.startX = startX;
        }

        public LFloat getStartZ(){
            return startZ;
        }

        public void setStartZ(LFloat startZ){
            this.startZ = startZ;
        }

        public LFloat getEndX(){
            return endX;
        }

        public void setEndX(LFloat endX){
            this.endX = endX;
        }

        public LFloat getEndZ(){
            return endZ;
        }

        public void setEndZ(LFloat endZ){
            this.endZ = endZ;
        }

        public int getMapID(){
            return mapID;
        }

        public void setMapID(int mapID){
            this.mapID = mapID;
        }


        public LFloat getWidth(){
            return width;
        }

        public void setWidth(LFloat width){
            this.width = width;
        }

        public LFloat getHeight(){
            return height;
        }

        public void setHeight(LFloat height){
            this.height = height;
        }
    }
}