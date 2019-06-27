using UnityEngine;

public class Funnel {

    public Plane leftPlane = new Plane(); // 左平面，高度为y轴
    public Plane rightPlane = new Plane();
    public Vector3 leftPortal = new Vector3(); // 路径左顶点，
    public Vector3 rightPortal = new Vector3(); // 路径右顶点
    public Vector3 pivot = new Vector3(); // 漏斗点，路径的起点或拐点

    public void setLeftPlane(Vector3 pivot, Vector3 leftEdgeVertex) {
        leftPlane.set(pivot, pivot.Add(Vector3.up), leftEdgeVertex);
        leftPortal.set(leftEdgeVertex);
    }

    public void setRightPlane(Vector3 pivot, Vector3 rightEdgeVertex) {
        rightPlane.set(pivot, pivot.Add(Vector3.up), rightEdgeVertex); // 高度
        rightPlane.normal.scl(-1); // 平面方向取反
        rightPlane.d = -rightPlane.d;
        rightPortal.set(rightEdgeVertex);
    }
    public void setPlanes(Vector3 pivot, TriangleEdge edge) {
        setLeftPlane(pivot, edge.leftVertex);
        setRightPlane(pivot, edge.rightVertex);
    }

    public PlaneSide sideLeftPlane(Vector3 point) {
        return leftPlane.testPoint(point);
    }

    public PlaneSide sideRightPlane(Vector3 point) {
        return rightPlane.testPoint(point);
    }
}