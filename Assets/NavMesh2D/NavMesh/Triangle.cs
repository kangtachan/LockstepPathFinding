using System;
using System.Collections.Generic;
using UnityEngine;


/**
 * 三角形
 * 
 * @author JiangZhiYong
 * @QQ 359135103 2017年11月7日 下午4:41:27
 */
public class Triangle {
    /** 三角形序号 */
    public int index;
    public Vector3 a;
    public Vector3 b;
    public Vector3 c;

    public float y; //三角形高度，三个顶点的平均高度

    /** 中点 */
    public Vector3 center;

    /** 三角形和其他三角形的共享边 */
    public List<Connection<Triangle>> connections;

    /**三角形顶点序号*/
    public int[] vectorIndex;

    public Triangle(Vector3 a, Vector3 b, Vector3 c, int index, params int[] vectorIndex){
        this.a = a;
        this.b = b;
        this.c = c;
        this.y = (a.y + b.y + c.y) / 3;
        this.index = index;
        this.center = a.Add(b).Add(c).scl(1f / 3f);
        this.connections = new List<Connection<Triangle>>();
        this.vectorIndex = vectorIndex;
    }

    public override String ToString(){
        return "Triangle [index=" + index + ", a=" + a + ", b=" + b + ", c=" + c + ", center=" + center + "]";
    }

    public int getIndex(){
        return this.index;
    }

    public List<Connection<Triangle>> getConnections(){
        return connections;
    }

    /**
     * Calculates the angle in radians between a reference vector and the (plane)
     * normal of the triangle.
     *
     * @param reference
     * @return
     */
    public float getAngle(Vector3 reference){
        float x = reference.x;
        float y = reference.y;
        float z = reference.z;
        Vector3 normal = reference;
        normal.set(a).sub(b).cross(b.x - c.x, b.y - c.y, b.z - c.z).nor();
        float angle = (float) Math.Acos(normal.dot(x, y, z) / (normal.len() * Math.Sqrt(x * x + y * y + z * z)));
        reference.set(x, y, z);
        return angle;
    }


    /**
     * Calculates the area of the triangle.
     *
     * @return
     */
    public float area(){
        float abx = b.x - a.x;
        float aby = b.y - a.y;
        float abz = b.z - a.z;
        float acx = c.x - a.x;
        float acy = c.y - a.y;
        float acz = c.z - a.z;
        float r = aby * acz - abz * acy;
        float s = abz * acx - abx * acz;
        float t = abx * acy - aby * acx;
        return 0.5f * (float) Math.Sqrt(r * r + s * s + t * t);
    }


    /**
     * 判断一个点是否在三角形内,二维判断
     * <br> http://www.yalewoo.com/in_triangle_test.html
     * @param vector3
     */
    public bool isInnerPoint(Vector3 point){
        bool res = pointInLineLeft(a, b, point);
        if (res != pointInLineLeft(b, c, point)) {
            return false;
        }

        if (res != pointInLineLeft(c, a, point)) {
            return false;
        }

        if (cross2D(a, b, c) == 0) { //三点共线
            return false;
        }

        return true;
    }

    public static float cross2D(Vector3 fromPoint, Vector3 toPoint, Vector3 p){
        return (toPoint.x - fromPoint.x) * (p.z - fromPoint.z) - (toPoint.z - fromPoint.z) * (p.x - fromPoint.x);
    }

    public static bool pointInLineLeft(Vector3 fromPoint, Vector3 toPoint, Vector3 p){
        return cross2D(fromPoint, toPoint, p) > 0;
    }


    public override int GetHashCode(){
        int prime = 31;
        int result = 1;
        result = prime * result + index;
        return result;
    }

    public override bool Equals(object obj){
        if (this == obj)
            return true;
        if (obj == null)
            return false;
        if (GetType() != obj.GetType())
            return false;
        Triangle other = (Triangle) obj;
        if (index != other.index)
            return false;
        return true;
    }
}