
using System;
using System.Collections.Generic;
using Lockstep.Math;

namespace Lockstep.PathFinding {
	public abstract class NavMesh {

		/** 地图宽x轴 */
		protected LFloat width;

		/** 地图高y轴 */
		protected LFloat height;

		/** 配置id */
		protected int mapId;

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

		public int getMapId(){
			return mapId;
		}

		public void setMapId(int mapId){
			this.mapId = mapId;
		}

	}
}