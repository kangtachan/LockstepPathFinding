









using System;
using System.Collections.Generic;
using UnityEngine;

public enum PlaneSide {
	OnPlane, Back, Front
}

/**
 * 平面
 * <br>
 * A plane defined via a unit length normal and the distance from the origin, as
 * you learned in your math class.
 * 
 * @author badlogicgames@gmail.com
 * @fix JiangZhiYong
 */
public class Plane  {
	private static long serialVersionUID = -1240652082930747866L;

	/**
	 * 点在平面的方向枚举 <br>
	 * Enum specifying on which side a point lies respective to the plane and it's
	 * normal. {@link PlaneSide#Front} is the side to which the normal points.
	 * 
	 * @author mzechner
	 */


	public Vector3 normal = new Vector3(); // 单位长度
	public float d = 0; // 距离

	/**
	 * Constructs a new plane with all values set to 0
	 */
	public Plane() {

	}

	/**
	 * Constructs a new plane based on the normal and distance to the origin.
	 * 
	 * @param normal
	 *            The plane normal
	 * @param d
	 *            The distance to the origin
	 */
	public Plane(Vector3 normal, float d) {
		this.normal.set(normal).nor();
		this.d = d;
	}

	/**
	 * Constructs a new plane based on the normal and a point on the plane.
	 * 
	 * @param normal
	 *            The normal
	 * @param point
	 *            The point on the plane
	 */
	public Plane(Vector3 normal, Vector3 point) {
		this.normal.set(normal).nor();
		this.d = -this.normal.dot(point);
	}

	/**
	 * Constructs a new plane out of the three given points that are considered to
	 * be on the plane. The normal is calculated via a cross product between
	 * (point1-point2)x(point2-point3)
	 * 
	 * @param point1
	 *            The first point
	 * @param point2
	 *            The second point
	 * @param point3
	 *            The third point
	 */
	public Plane(Vector3 point1, Vector3 point2, Vector3 point3) {
		set(point1, point2, point3);
	}

	/**设置在平面属性<br>
	 * Sets the plane normal and distance to the origin based on the three given
	 * points which are considered to be on the plane. The normal is calculated via
	 * a cross product between (point1-point2)x(point2-point3)
	 * 
	 * @param point1
	 * @param point2
	 * @param point3
	 */
	public void set(Vector3 point1, Vector3 point2, Vector3 point3) {
		normal.set(point1).sub(point2).cross(point2.x - point3.x, point2.y - point3.y, point2.z - point3.z).nor();
		d = -point1.dot(normal);
	}

	/**
	 * Sets the plane normal and distance
	 * 
	 * @param nx
	 *            normal x-component
	 * @param ny
	 *            normal y-component
	 * @param nz
	 *            normal z-component
	 * @param d
	 *            distance to origin
	 */
	public void set(float nx, float ny, float nz, float d) {
		normal.set(nx, ny, nz);
		this.d = d;
	}

	/**
	 * Calculates the shortest signed distance between the plane and the given
	 * point.
	 * 
	 * @param point
	 *            The point
	 * @return the shortest signed distance between the plane and the point
	 */
	public float distance(Vector3 point) {
		return normal.dot(point) + d;
	}

	/**
	 * Returns on which side the given point lies relative to the plane and its
	 * normal. PlaneSide.Front refers to the side the plane normal points to.
	 * 
	 * @param point
	 *            The point
	 * @return The side the point lies relative to the plane
	 */
	public PlaneSide testPoint(Vector3 point) {
		float dist = normal.dot(point) + d;

		if (dist == 0)
			return PlaneSide.OnPlane;
		else if (dist < 0)
			return PlaneSide.Back;
		else
			return PlaneSide.Front;
	}

	/**
	 * Returns on which side the given point lies relative to the plane and its
	 * normal. PlaneSide.Front refers to the side the plane normal points to.
	 * 
	 * @param x
	 * @param y
	 * @param z
	 * @return The side the point lies relative to the plane
	 */
	public PlaneSide testPoint(float x, float y, float z) {
		float dist = normal.dot(x, y, z) + d;

		if (dist == 0)
			return PlaneSide.OnPlane;
		else if (dist < 0)
			return PlaneSide.Back;
		else
			return PlaneSide.Front;
	}

	/**
	 * Returns whether the plane is facing the direction vector. Think of the
	 * direction vector as the direction a camera looks in. This method will return
	 * true if the front side of the plane determined by its normal faces the
	 * camera.
	 * 
	 * @param direction
	 *            the direction
	 * @return whether the plane is front facing
	 */
	public bool isFrontFacing(Vector3 direction) {
		float dot = normal.dot(direction);
		return dot <= 0;
	}

	/** @return The normal */
	public Vector3 getNormal() {
		return normal;
	}

	/** @return The distance to the origin */
	public float getD() {
		return d;
	}

	/**
	 * Sets the plane to the given point and normal.
	 * 
	 * @param point
	 *            the point on the plane
	 * @param normal
	 *            the normal of the plane
	 */
	public void set(Vector3 point, Vector3 normal) {
		this.normal.set(normal);
		d = -point.dot(normal);
	}

	public void set(float pointX, float pointY, float pointZ, float norX, float norY, float norZ) {
		this.normal.set(norX, norY, norZ);
		d = -(pointX * norX + pointY * norY + pointZ * norZ);
	}

	/**
	 * Sets this plane from the given plane
	 * 
	 * @param plane
	 *            the plane
	 */
	public void set(Plane plane) {
		this.normal.set(plane.normal);
		this.d = plane.d;
	}

	public override String ToString() {
		return normal.ToString() + ", " + d;
	}
}



/**
 * NavMesh 生成坐标路径点 
 * 
 * @author jsjolund
 * @fix JiangZhiYong
 */
public class TrianglePointPath  {
	public static Vector3 V3_UP = Vector3.up;
	public static Vector3 V3_DOWN = Vector3.down;
	
	/**
	 * 在三角形边上的交叉点 <br>
	 * A point where an edge on the navmesh is crossed.
	 */
	private class EdgePoint {
		/**
		 * Triangle which must be crossed to reach the next path point.
		 */
		public Triangle toNode;
		/**
		 * Triangle which was crossed to reach this point.
		 */
		public Triangle fromNode;
		/**
		 * Path edges connected to this point. Can be used for spline generation at some
		 * point perhaps...
		 */
		public List<TriangleEdge> connectingEdges = new List<TriangleEdge>();
		/**
		 * The point where the path crosses an edge.
		 */
		public Vector3 point;

		public EdgePoint(Vector3 point, Triangle toNode) {
			this.point = point;
			this.toNode = toNode;
		}
	}

	/**
	 * @ 漏斗算法 <br>
	 * http://blog.csdn.net/yxriyin/article/details/39207709 <br>
	 * Plane funnel for the Simple Stupid Funnel Algorithm
	 */
	private class Funnel {

		public Plane leftPlane = new Plane(); // 左平面，高度为y轴
		public Plane rightPlane = new Plane();
		public Vector3 leftPortal = new Vector3(); // 路径左顶点，
		public Vector3 rightPortal = new Vector3(); // 路径右顶点
		public Vector3 pivot = new Vector3(); // 漏斗点，路径的起点或拐点

		/**
		 * 设置左平面
		 * 
		 * @param pivot
		 * @param leftEdgeVertex
		 *            边左顶点
		 */
		public void setLeftPlane(Vector3 pivot, Vector3 leftEdgeVertex) {
			leftPlane.set(pivot, tmp1.set(pivot).Add(V3_UP), leftEdgeVertex);
			leftPortal.set(leftEdgeVertex);
		}

		/**
		 * 设置右平面
		 * 
		 * @param pivot
		 * @param rightEdgeVertex
		 */
		public void setRightPlane(Vector3 pivot, Vector3 rightEdgeVertex) {
			rightPlane.set(pivot, tmp1.set(pivot).Add(V3_UP), rightEdgeVertex); // 高度
			rightPlane.normal.scl(-1); // 平面方向取反
			rightPlane.d = -rightPlane.d;
			rightPortal.set(rightEdgeVertex);
		}

		/**
		 * 设置平面
		 * 
		 * @param pivot
		 * @param edge
		 *            边
		 */
		public void setPlanes(Vector3 pivot, TriangleEdge edge) {
			setLeftPlane(pivot, edge.leftVertex);
			setRightPlane(pivot, edge.rightVertex);
		}

		/**
		 * 测试点在左平面内侧还是外侧（前或后）
		 * 
		 * @param point
		 * @return
		 */
		public PlaneSide sideLeftPlane(Vector3 point) {
			return leftPlane.testPoint(point);
		}

		/**
		 * 测试点在右平面内侧还是外侧
		 * 
		 * @param point
		 * @return
		 */
		public PlaneSide sideRightPlane(Vector3 point) {
			return rightPlane.testPoint(point);
		}
	}

	private Plane crossingPlane = new Plane(); // 横跨平面
	private static Vector3 tmp1 = new Vector3();
	private static Vector3 tmp2 = new Vector3();
	private List<Connection<Triangle>> nodes; // 路径连接点
	private Vector3 start; // 起点
	private Vector3 end; // 终点
	private Triangle startTri; // 起始三角形
	private EdgePoint lastPointAdded; // 最后一个边点
	private List<Vector3> vectors = new List<Vector3>(); // 路径坐标点
	private List<EdgePoint> pathPoints = new List<EdgePoint>();
	private TriangleEdge lastEdge; // 最后一个边

	

	private TriangleEdge getEdge(int index) {
		return (TriangleEdge) ((index == nodes.Count) ? lastEdge : nodes[index]);
	}

	private int numEdges() {
		return nodes.Count + 1;
	}

	/**
	 * 计算路径点 <br>
	 * Calculate the shortest path through the navigation mesh triangles.
	 *
	 * @param trianglePath
	 * @param calculateCrossPoint true 计算三角形的交叉点，false 只计算拐点
	 */
	public void calculateForGraphPath(TriangleGraphPath trianglePath,bool calculateCrossPoint) {
		clear();
		nodes = trianglePath.nodes;
		this.start = trianglePath.start;
		this.end = trianglePath.end;
		this.startTri = trianglePath.startTri;

		// 矫正开始坐标
		// Check that the start point is actually inside the start triangle, if not,
		// project it to the closest
		// triangle edge. Otherwise the funnel calculation might generate spurious path
		// segments.
		Ray ray = new Ray(tmp1.set(V3_UP).scl(1000).Add(start), tmp2.set(V3_DOWN)); // 起始坐标从上向下的射线
		if (!intersectRayTriangle(ray, startTri.a, startTri.b, startTri.c, out var ss)) {
			float minDst = float.MaxValue;
			Vector3 projection = new Vector3(); // 规划坐标
			Vector3 newStart = new Vector3(); // 新坐标
			float dst;
			// A-B
			if ((dst = GeometryUtil.nearestSegmentPointSquareDistance(projection, startTri.a, startTri.b,
					start)) < minDst) {
				minDst = dst;
				newStart.set(projection);
			}
			// B-C
			if ((dst = GeometryUtil.nearestSegmentPointSquareDistance(projection, startTri.b, startTri.c,
					start)) < minDst) {
				minDst = dst;
				newStart.set(projection);
			}
			// C-A
			if ((dst = GeometryUtil.nearestSegmentPointSquareDistance(projection, startTri.c, startTri.a,
					start)) < minDst) {
				minDst = dst;
				newStart.set(projection);
			}
			start.set(newStart);
		}
		if (nodes.Count == 0) { // 起点终点在同一三角形中
			addPoint(start, startTri);
			addPoint(end, startTri);
		} else {
			lastEdge = new TriangleEdge(nodes.get(nodes.Count - 1).GetToNode(), nodes.get(nodes.Count - 1).GetToNode(), end,
					end);
			calculateEdgePoints(calculateCrossPoint);
		}
	}

	/**
	 * 清理数据 <br>
	 * Clear the stored path data.
	 */
	public void clear() {
		vectors.Clear();
		pathPoints.Clear();
		startTri = null;
		lastPointAdded = null;
		lastEdge = null;
	}

	/**
	 * @ 获取寻路向量点 A path point which crosses one or more edges in the navigation
	 * mesh.
	 *
	 * @param index
	 * @return
	 */
	public Vector3 getVector(int index) {
		return vectors.get(index);
	}

	/**
	 * The number of path points.
	 *
	 * @return
	 */
	public int getSize() {
		return vectors.Count;
	}

	/**
	 * All vectors in the path.
	 *
	 * @return
	 */
	public List<Vector3> getVectors() {
		return vectors;
	}

	/**
	 * The triangle which must be crossed to reach the next path point.
	 *
	 * @param index
	 * @return
	 */
	public Triangle getToTriangle(int index) {
		return pathPoints.get(index).toNode;
	}

	/**
	 * The triangle from which must be crossed to reach this point.
	 *
	 * @param index
	 * @return
	 */
	public Triangle getFromTriangle(int index) {
		return pathPoints.get(index).fromNode;
	}

	/**
	 * The navmesh edge(s) crossed at this path point.
	 *
	 * @param index
	 * @return
	 */
	public List<TriangleEdge> getCrossedEdges(int index) {
		return pathPoints.get(index).connectingEdges;
	}

	/**
	 * 添加坐标点
	 * 
	 * @param point
	 * @param toNode
	 */
	private void addPoint(Vector3 point, Triangle toNode) {
		addPoint(new EdgePoint(point, toNode));
	}
	
	/**
	 * 添加坐标点
	 * 
	 * @param edgePoint
	 */
	private void addPoint(EdgePoint edgePoint) {
		vectors.Add(edgePoint.point);
		pathPoints.Add(edgePoint);
		lastPointAdded = edgePoint;
	}

	/**
	 * 根据三角形路径计算最短路径坐标点 <br>
	 * http://blog.csdn.net/yxriyin/article/details/39207709 Calculate the shortest
	 * point path through the path triangles, using the Simple Stupid Funnel
	 * Algorithm.
	 *
	 * @return
	 */
	private void calculateEdgePoints(bool calculateCrossPoint) {
		TriangleEdge edge = getEdge(0);
		addPoint(start, edge.fromNode);
		lastPointAdded.fromNode = edge.fromNode;

		Funnel funnel = new Funnel();
		funnel.pivot.set(start); // 起点为漏斗点
		funnel.setPlanes(funnel.pivot, edge); // 设置第一对平面

		int leftIndex = 0; // 左顶点索引
		int rightIndex = 0; // 右顶点索引
		int lastRestart = 0;

		for (int i = 1; i < numEdges(); ++i) {
			edge = getEdge(i); // 下一条边

			PlaneSide leftPlaneLeftDP = funnel.sideLeftPlane(edge.leftVertex);
			PlaneSide leftPlaneRightDP = funnel.sideLeftPlane(edge.rightVertex);
			PlaneSide rightPlaneLeftDP = funnel.sideRightPlane(edge.leftVertex);
			PlaneSide rightPlaneRightDP = funnel.sideRightPlane(edge.rightVertex);

			// 右顶点在右平面里面
			if (rightPlaneRightDP != PlaneSide.Front) {
				// 右顶点在左平面里面
				if (leftPlaneRightDP != PlaneSide.Front) {
					// Tighten the funnel. 缩小漏斗
					funnel.setRightPlane(funnel.pivot, edge.rightVertex);
					rightIndex = i;
				} else {
					// Right over left, insert left to path and restart scan from portal left point.
					// 右顶点在左平面外面，设置左顶点为漏斗顶点和路径点，从新已该漏斗开始扫描
					if(calculateCrossPoint) {
						calculateEdgeCrossings(lastRestart, leftIndex, funnel.pivot, funnel.leftPortal);
					}else {
						vectors.Add(funnel.leftPortal);
					}
					funnel.pivot.set(funnel.leftPortal);
					i = leftIndex;
					rightIndex = i;
					if (i < numEdges() - 1) {
						lastRestart = i;
						funnel.setPlanes(funnel.pivot, getEdge(i + 1));
						continue;
					}
					break;
				}
			}
			// 左顶点在左平面里面
			if (leftPlaneLeftDP != PlaneSide.Front) {
				// 左顶点在右平面里面
				if (rightPlaneLeftDP != PlaneSide.Front) {
					// Tighten the funnel.
					funnel.setLeftPlane(funnel.pivot, edge.leftVertex);
					leftIndex = i;
				} else {
					// Left over right, insert right to path and restart scan from portal right
					// point.
					if(calculateCrossPoint) {
						calculateEdgeCrossings(lastRestart, rightIndex, funnel.pivot, funnel.rightPortal);
					}else {
						vectors.Add(funnel.rightPortal);
					}
					funnel.pivot.set(funnel.rightPortal);
					i = rightIndex;
					leftIndex = i;
					if (i < numEdges() - 1) {
						lastRestart = i;
						funnel.setPlanes(funnel.pivot, getEdge(i + 1));
						continue;
					}
					break;
				}
			}
		}
		if(calculateCrossPoint) {
			calculateEdgeCrossings(lastRestart, numEdges() - 1, funnel.pivot, end);
		}else {
			vectors.Add(end);
		}

		for (int i = 1; i < pathPoints.Count; i++) {
			EdgePoint p = pathPoints.get(i);
			p.fromNode = pathPoints.get(i - 1).toNode;
		}
		return;
	}

	/**
	 * 计算存储和边的交叉点<br>
	 * Store all edge crossing points between the start and end indices. If the path
	 * crosses exactly the start or end points (which is quite likely), store the
	 * edges in order of crossing in the EdgePoint data structure.
	 * <p/>
	 * Edge crossings are calculated as intersections with the plane from the start,
	 * end and up vectors.
	 *
	 * @param startIndex
	 * @param endIndex
	 * @param startPoint 
	 * @param endPoint
	 */
	private void calculateEdgeCrossings(int startIndex, int endIndex, Vector3 startPoint, Vector3 endPoint) {

		if (startIndex >= numEdges() || endIndex >= numEdges()) {
			return;
		}
		crossingPlane.set(startPoint, tmp1.set(startPoint).Add(V3_UP), endPoint);

		EdgePoint previousLast = lastPointAdded;

		var edge = getEdge(endIndex);
		EdgePoint end = new EdgePoint(endPoint, edge.toNode);

		for (int i = startIndex; i < endIndex; i++) {
			edge = getEdge(i);

			if (edge.rightVertex.Equals(startPoint) || edge.leftVertex.Equals(startPoint)) {
				previousLast.toNode = edge.toNode;
				if (!previousLast.connectingEdges.Contains(edge)) {
					previousLast.connectingEdges.Add(edge);
				}

			} else if (edge.leftVertex.Equals(endPoint) || edge.rightVertex.Equals(endPoint)) {
				if (!end.connectingEdges.Contains(edge)) {
					end.connectingEdges.Add(edge);
				}

			} else if (intersectSegmentPlane(edge.leftVertex, edge.rightVertex, crossingPlane, tmp1)
					&& !float.IsNaN(tmp1.x + tmp1.y + tmp1.z)) {
				if (i != startIndex || i == 0) {
					lastPointAdded.toNode = edge.fromNode;
					EdgePoint crossing = new EdgePoint(tmp1,edge.toNode);
					crossing.connectingEdges.Add(edge);
					addPoint(crossing);
				}
			}
		}
		if (endIndex < numEdges() - 1) {
			end.connectingEdges.Add(getEdge(endIndex));
		}
		if (!lastPointAdded.Equals(end)) {
			addPoint(end);
		}
	}

	public static bool intersectSegmentPlane(Vector3 start, Vector3 end, Plane plane, Vector3 intersection) {
		Vector3 dir = end.sub(start);
		float denom = dir.dot(plane.getNormal());
		float t = -(start.dot(plane.getNormal()) + plane.getD()) / denom;
		if (t < 0 || t > 1)
			return false;

		intersection.set(start).Add(dir.scl(t));
		return true;
	}
	
	static public bool isZero(float value) {
		return Math.Abs(value) <=FLOAT_ROUNDING_ERROR;
	}

	private static float FLOAT_ROUNDING_ERROR = 0.000001f;
	
	private static  Plane p = new Plane(new Vector3(), 0);
	private static  Vector3 i = new Vector3();

	public static bool intersectRayTriangle(Ray ray, Vector3 t1, Vector3 t2, Vector3 t3,out Vector3 intersection) {
		intersection = Vector3.zero;
		Vector3 edge1 = t2.sub(t1);
		Vector3 edge2 = t3.sub(t1);

		Vector3 pvec = ray.direction.cross(edge2);
		float det = edge1.dot(pvec);
		if (isZero(det)) {
			p.set(t1, t2, t3);
			if (p.testPoint(ray.origin) == PlaneSide.OnPlane && isPointInTriangle(ray.origin, t1, t2, t3)) {
				if (intersection != null)
					intersection.set(ray.origin);
				return true;
			}
			return false;
		}

		det = 1.0f / det;

		Vector3 tvec = i.set(ray.origin).sub(t1);
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
		} else {
			ray.getEndPoint(intersection, t);
		}

		return true;
	}
	public static bool isPointInTriangle(Vector3 point, Vector3 t1, Vector3 t2, Vector3 t3) {
		var v0 = (t1).sub(point);
		var v1 = (t2).sub(point);
		var v2 = (t3).sub(point);

		float ab = v0.dot(v1);
		float ac = v0.dot(v2);
		float bc = v1.dot(v2);
		float cc = v2.dot(v2);

		if (bc * ac - cc * ab < 0)
			return false;
		float bb = v1.dot(v1);
		if (ab * bc - ac * bb < 0)
			return false;
		return true;
	}
}


/**
 * 射线<br>
 * Encapsulates a ray having a starting position and a unit length direction.
 * 
 * @author badlogicgames@gmail.com
 */
public class Ray  {
	private static long serialVersionUID = -620692054835390878L;
	public Vector3 origin = new Vector3(); // 起点
	public Vector3 direction = new Vector3(); // 方向

	public Ray() {
	}

	/**
	 * Constructor, sets the starting position of the ray and the direction.
	 * 
	 * @param origin
	 *            The starting position
	 * @param direction
	 *            The direction
	 */
	public Ray(Vector3 origin, Vector3 direction) {
		this.origin.set(origin);
		this.direction.set(direction).nor();
	}

	/** @return a copy of this ray. */
	public Ray cpy() {
		return new Ray(this.origin, this.direction);
	}

	/**
	 * Returns the endpoint given the distance. This is calculated as startpoint +
	 * distance * direction.
	 * 
	 * @param out
	 *            The vector to set to the result
	 * @param distance
	 *            The distance from the end point to the start point.
	 * @return The out param
	 */
	public Vector3 getEndPoint(Vector3 _out, float distance) {
		return _out.set(direction).scl(distance).Add(origin);
	}

	static Vector3 tmp = new Vector3();


	/** {@inheritDoc} */
	public String toString() {
		return "ray [" + origin + ":" + direction + "]";
	}

	/**
	 * Sets the starting position and the direction of this ray.
	 * 
	 * @param origin
	 *            The starting position
	 * @param direction
	 *            The direction
	 * @return this ray for chaining
	 */
	public Ray set(Vector3 origin, Vector3 direction) {
		this.origin.set(origin);
		this.direction.set(direction);
		return this;
	}

	/**
	 * Sets this ray from the given starting position and direction.
	 * 
	 * @param x
	 *            The x-component of the starting position
	 * @param y
	 *            The y-component of the starting position
	 * @param z
	 *            The z-component of the starting position
	 * @param dx
	 *            The x-component of the direction
	 * @param dy
	 *            The y-component of the direction
	 * @param dz
	 *            The z-component of the direction
	 * @return this ray for chaining
	 */
	public Ray set(float x, float y, float z, float dx, float dy, float dz) {
		this.origin.set(x, y, z);
		this.direction.set(dx, dy, dz);
		return this;
	}

	/**
	 * Sets the starting position and direction from the given ray
	 * 
	 * @param ray
	 *            The ray
	 * @return This ray for chaining
	 */
	public Ray set(Ray ray) {
		this.origin.set(ray.origin);
		this.direction.set(ray.direction);
		return this;
	}

}

