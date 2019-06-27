using System;
using System.Collections.Generic;
using UnityEngine;

namespace NoLockstep.AI.Navmesh2D {
	public class TriangleGraphPath : DefaultGraphPath<Connection<Triangle>> {
		/**
		 * The start point when generating a point path for this triangle path
		 */
		public Vector3 start;

		/**
		 * The end point when generating a point path for this triangle path
		 */
		public Vector3 end;

		/**
		 * If the triangle path is empty, the point path will span this triangle
		 */
		public Triangle startTri;

		/**
		 * @return Last triangle in the path.
		 */
		public Triangle GetEndTriangle(){
			return (GetCount() > 0) ? Get(GetCount() - 1).GetToNode() : startTri;
		}
	}
}