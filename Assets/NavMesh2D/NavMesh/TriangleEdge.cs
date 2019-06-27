
using System;
using System.Collections.Generic;
using UnityEngine;





/**
 * 相连接三角形的共享边
 * 
 * @author JiangZhiYong
 * @QQ 359135103 2017年11月7日 下午4:50:11
 */

using System;
using System.Text;

namespace NoLockstep.AI.Navmesh2D {
	public class TriangleEdge : Connection<Triangle> {
		/** 右顶点 */
		public Vector3 rightVertex;
		public Vector3 leftVertex;

		/** 源三角形 */
		public Triangle fromNode;

		/** 指向的三角形 */
		public Triangle toNode;

		public TriangleEdge(Vector3 rightVertex, Vector3 leftVertex) : this(null, null, rightVertex, leftVertex){ }

		public TriangleEdge(Triangle fromNode, Triangle toNode, Vector3 rightVertex, Vector3 leftVertex){
			this.fromNode = fromNode;
			this.toNode = toNode;
			this.rightVertex = rightVertex;
			this.leftVertex = leftVertex;
		}

		public float GetCost(){
			return 1;
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