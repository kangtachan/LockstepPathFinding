
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TriangleData : NavMeshData{
	private static long serialVersionUID = 1L;
}


public class GeometryUtil {
	
	/**点到线段距离的平方<br>
	 * Projects a point to a line segment. This implementation is thread-safe.
	 *
	 * @param nearest Output for the nearest vector to the segment.
	 * @param start Segment start point
	 * @param end Segment end point
	 * @param point Point to project from
	 * @return Squared distance between point and nearest.
	 */
	public static float nearestSegmentPointSquareDistance
	(Vector3 nearest, Vector3 start, Vector3 end, Vector3 point) {
		nearest.set(start);
		float abX = end.x - start.x;
		float abY = end.y - start.y;
		float abZ = end.z - start.z;
		float abLen2 = abX * abX + abY * abY + abZ * abZ;
		if (abLen2 > 0) { // Avoid NaN due to the indeterminate form 0/0
			float t = ((point.x - start.x) * abX + (point.y - start.y) * abY + (point.z - start.z) * abZ) / abLen2;
			float s = Mathf.Clamp(t, 0, 1);
			nearest.x += abX * s;
			nearest.y += abY * s;
			nearest.z += abZ * s;
		}
		return nearest.dst2(point);
	}


	private static  Vector3 TMP_VEC_1 = new Vector3();
	private static  Vector3 TMP_VEC_2 = new Vector3();
	private static  Vector3 TMP_VEC_3 = new Vector3();
	
	/**获取离三角形最近的点
	 * Find the closest point on the triangle, given a measure point.
	 * This is the optimized algorithm taken from the book "Real-Time Collision Detection".
	 * <p>
	 * This implementation is NOT thread-safe.
	 * 
	 * @param a   First vertex of the triangle
	 * @param b   Second vertex of the triangle
	 * @param c   Third vertex of the triangle
	 * @param p   The measure point
	 * @param out Output for the closest point; can be {@code null}
	 * @return The closest distance squared, between the triangle's vertices and the point.
	 */
	public static float getClosestPointOnTriangle(Vector3 a, Vector3 b, Vector3 c, Vector3 p,ref Vector3 _out) {
		// Check if P in vertex region outside A
		Vector3 ab = TMP_VEC_1.set(b).sub(a);
		Vector3 ac = TMP_VEC_2.set(c).sub(a);
		Vector3 ap = TMP_VEC_3.set(p).sub(a);
		float d1 = ab.dot(ap);
		float d2 = ac.dot(ap);
		if (d1 <= 0.0f && d2 <= 0.0f) {
			_out = a;
			return p.dst2(a);
		}
		
		// Check if P in vertex region outside B
		Vector3 bp = TMP_VEC_3.set(p).sub(b);
		float d3 = ab.dot(bp);
		float d4 = ac.dot(bp);
		if (d3 >= 0.0f && d4 <= d3) {
			_out = b;
			return p.dst2(b);
		}
		
		// Check if P in edge region of AB, if so return projection of P onto AB
		float vc = d1 * d4 - d3 * d2;
		if (vc <= 0.0f && d1 >= 0.0f && d3 <= 0.0f) {
			float v = d1 / (d1 - d3);
			_out.set(a).mulAdd(ab, v); // barycentric coordinates (1-v,v,0)
			return p.dst2(_out);
		}

		// Check if P in vertex region outside C
		Vector3 cp = TMP_VEC_3.set(p).sub(c);
		float d5 = ab.dot(cp);
		float d6 = ac.dot(cp);
		if (d6 >= 0.0f && d5 <= d6) {
			_out = c;
			return p.dst2(c);
		}

		// Check if P in edge region of AC, if so return projection of P onto AC
		float vb = d5 * d2 - d1 * d6;
		if (vb <= 0.0f && d2 >= 0.0f && d6 <= 0.0f) {
			float w = d2 / (d2 - d6);
			_out.set(a).mulAdd(ac, w); // barycentric coordinates (1-w,0,w)
			return _out.dst2(p); 
		}

		// Check if P in edge region of BC, if so return projection of P onto BC
		float va = d3 * d6 - d5 * d4;
		if (va <= 0.0f && (d4 - d3) >= 0.0f && (d5 - d6) >= 0.0f) {
			float w = (d4 - d3) / ((d4 - d3) + (d5 - d6));
			_out.set(b).mulAdd(TMP_VEC_1.set(c).sub(b), w); // barycentric coordinates (0,1-w,w)
			return _out.dst2(p); 
		}
		
		// P inside face region. Compute Q through its barycentric coordinates (u,v,w)
		float denom = 1.0f / (va + vb + vc);
		{
			float v = vb * denom;
			float w = vc * denom;
			_out.set(a).mulAdd(ab, v).mulAdd(ac, w);
		}
		return _out.dst2(p); 
	}

}