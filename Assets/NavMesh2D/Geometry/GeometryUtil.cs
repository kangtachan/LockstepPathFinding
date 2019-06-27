using System;
using UnityEngine;

public class GeometryUtil {

    public static float FLOAT_ROUNDING_ERROR = 0.000001f;
    
    private static Vector3 TMP_VEC_1 = new Vector3();
    private static Vector3 TMP_VEC_2 = new Vector3();
    private static Vector3 TMP_VEC_3 = new Vector3();
    
    /** Projects a point to a line segment. This implementation is thread-safe.	 */
    public static float nearestSegmentPointSquareDistance
        (Vector3 nearest, Vector3 start, Vector3 end, Vector3 point){
        nearest.set(start);
        var abX = end.x - start.x;
        var abY = end.y - start.y;
        var abZ = end.z - start.z;
        var abLen2 = abX * abX + abY * abY + abZ * abZ;
        if (abLen2 > 0) { // Avoid NaN due to the indeterminate form 0/0
            var t = ((point.x - start.x) * abX + (point.y - start.y) * abY + (point.z - start.z) * abZ) / abLen2;
            var s = Mathf.Clamp(t, 0, 1);
            nearest.x += abX * s;
            nearest.y += abY * s;
            nearest.z += abZ * s;
        }

        return nearest.dst2(point);
    }


    /*
	 * Find the closest point on the triangle, given a measure point.
	 * This is the optimized algorithm taken from the book "Real-Time Collision Detection".
	 * <p>
	 * This implementation is NOT thread-safe.
	 */
    public static float getClosestPointOnTriangle(Vector3 a, Vector3 b, Vector3 c, Vector3 p, ref Vector3 _out){
        // Check if P in vertex region outside A
        var ab = b.sub(a);
        var ac = c.sub(a);
        var ap = p.sub(a);
        var d1 = ab.dot(ap);
        var d2 = ac.dot(ap);
        if (d1 <= 0.0f && d2 <= 0.0f) {
            _out = a;
            return p.dst2(a);
        }

        // Check if P in vertex region outside B
        var bp = p.sub(b);
        var d3 = ab.dot(bp);
        var d4 = ac.dot(bp);
        if (d3 >= 0.0f && d4 <= d3) {
            _out = b;
            return p.dst2(b);
        }

        // Check if P in edge region of AB, if so return projection of P onto AB
        var vc = d1 * d4 - d3 * d2;
        if (vc <= 0.0f && d1 >= 0.0f && d3 <= 0.0f) {
            var v = d1 / (d1 - d3);
            _out.set(a).mulAdd(ab, v); // barycentric coordinates (1-v,v,0)
            return p.dst2(_out);
        }

        // Check if P in vertex region outside C
        var cp = p.sub(c);
        var d5 = ab.dot(cp);
        var d6 = ac.dot(cp);
        if (d6 >= 0.0f && d5 <= d6) {
            _out = c;
            return p.dst2(c);
        }

        // Check if P in edge region of AC, if so return projection of P onto AC
        var vb = d5 * d2 - d1 * d6;
        if (vb <= 0.0f && d2 >= 0.0f && d6 <= 0.0f) {
            var w = d2 / (d2 - d6);
            _out.set(a).mulAdd(ac, w); // barycentric coordinates (1-w,0,w)
            return _out.dst2(p);
        }

        // Check if P in edge region of BC, if so return projection of P onto BC
        var va = d3 * d6 - d5 * d4;
        if (va <= 0.0f && (d4 - d3) >= 0.0f && (d5 - d6) >= 0.0f) {
            var w = (d4 - d3) / ((d4 - d3) + (d5 - d6));
            _out.set(b).mulAdd(c.sub(b), w); // barycentric coordinates (0,1-w,w)
            return _out.dst2(p);
        }

        // P inside face region. Compute Q through its barycentric coordinates (u,v,w)
        var denom = 1.0f / (va + vb + vc);
        {
            float v = vb * denom;
            float w = vc * denom;
            _out.set(a).mulAdd(ab, v).mulAdd(ac, w);
        }
        return _out.dst2(p);
    }
    
    public static bool IntersectRayTriangle(Ray ray, Vector3 t1, Vector3 t2, Vector3 t3, out Vector3 intersection){
        intersection = Vector3.zero;
        Vector3 edge1 = t2.sub(t1);
        Vector3 edge2 = t3.sub(t1);

        Vector3 pvec = ray.direction.cross(edge2);
        float det = edge1.dot(pvec);
        if (IsZero(det)) {
            var p = new Plane(t1, t2, t3);
            if (p.testPoint(ray.origin) == PlaneSide.OnPlane && IsPointInTriangle(ray.origin, t1, t2, t3)) {
                intersection.set(ray.origin);
                return true;
            }

            return false;
        }

        det = 1.0f / det;

        Vector3 tvec = ray.origin.sub(t1);
        float u = tvec.dot(pvec) * det;
        if (u < 0.0f || u > 1.0f)
            return false;

        Vector3 qvec = tvec.cross(edge1);
        float v = ray.direction.dot(qvec) * det;
        if (v < 0.0f || u + v > 1.0f)
            return false;

        float t = edge2.dot(qvec) * det;
        if (t < 0)
            return false;

        if (t <= FLOAT_ROUNDING_ERROR) {
            intersection.set(ray.origin);
        }
        else {
            ray.getEndPoint(intersection, t);
        }

        return true;
    }

    public static bool IsPointInTriangle(Vector3 point, Vector3 t1, Vector3 t2, Vector3 t3){
        var v0 = (t1).sub(point);
        var v1 = (t2).sub(point);
        var v2 = (t3).sub(point);

        var ab = v0.dot(v1);
        var ac = v0.dot(v2);
        var bc = v1.dot(v2);
        var cc = v2.dot(v2);

        if (bc * ac - cc * ab < 0)
            return false;
        var bb = v1.dot(v1);
        if (ab * bc - ac * bb < 0)
            return false;
        return true;
    }
    public static bool IsZero(float value){
        return Math.Abs(value) <= GeometryUtil.FLOAT_ROUNDING_ERROR;
    }

}