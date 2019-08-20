
using System;
using System.Collections.Generic;
using Lockstep.Math;





/**
 * 相连接三角形的共享边
 * 
 * @author JiangZhiYong
 * @QQ 359135103 2017年11月7日 下午4:50:11
 */

using System;
using System.Text;

namespace Lockstep.PathFinding {
	public class TriangleEdge : Connection<Triangle> {
		/** 右顶点 */
		public LVector3 rightVertex;
		public LVector3 leftVertex;

		/** 源三角形 */
		public Triangle fromNode;

		/** 指向的三角形 */
		public Triangle toNode;

		public TriangleEdge(LVector3 rightVertex, LVector3 leftVertex) : this(null, null, rightVertex, leftVertex){ }

		public TriangleEdge(Triangle fromNode, Triangle toNode, LVector3 rightVertex, LVector3 leftVertex){
			this.fromNode = fromNode;
			this.toNode = toNode;
			this.rightVertex = rightVertex;
			this.leftVertex = leftVertex;
		}

		public LFloat GetCost(){
			return LFloat.one;
		}

		public Triangle GetFromNode(){
			return fromNode;
		}

		public Triangle GetToNode(){
			return toNode;
		}

		public override String ToString(){
			StringBuilder sb = new StringBuilder("Edge{");
			sb.Append("fromNode=").Append(fromNode.index);
			//sb.Append(", toNode=").Append(toNode == null ? "null" : toNode.index);
			sb.Append(", rightVertex=").Append(rightVertex);
			sb.Append(", leftVertex=").Append(leftVertex);
			sb.Append('}');
			return sb.ToString();
		}

	}
}