using System;
using UnityEngine;

public class Plane {
    private static long serialVersionUID = -1240652082930747866L;

    public Vector3 normal = new Vector3(); // 单位长度
    public float d = 0; // 距离

    public Plane(){ }

    public Plane(Vector3 normal, float d){
        this.normal.set(normal).nor();
        this.d = d;
    }

    public Plane(Vector3 normal, Vector3 point){
        this.normal.set(normal).nor();
        this.d = -this.normal.dot(point);
    }

    public Plane(Vector3 point1, Vector3 point2, Vector3 point3){
        set(point1, point2, point3);
    }

    public void set(Vector3 point1, Vector3 point2, Vector3 point3){
        normal = (point1).sub(point2).cross(point2.x - point3.x, point2.y - point3.y, point2.z - point3.z).nor();
        d = -point1.dot(normal);
    }



    public float distance(Vector3 point){
        return normal.dot(point) + d;
    }

    public PlaneSide testPoint(Vector3 point){
        float dist = normal.dot(point) + d;

        if (dist == 0)
            return PlaneSide.OnPlane;
        else if (dist < 0)
            return PlaneSide.Back;
        else
            return PlaneSide.Front;
    }


    public PlaneSide testPoint(float x, float y, float z){
        float dist = normal.dot(x, y, z) + d;

        if (dist == 0)
            return PlaneSide.OnPlane;
        else if (dist < 0)
            return PlaneSide.Back;
        else
            return PlaneSide.Front;
    }


    public bool isFrontFacing(Vector3 direction){
        float dot = normal.dot(direction);
        return dot <= 0;
    }

    /** @return The normal */
    public Vector3 getNormal(){
        return normal;
    }

    /** @return The distance to the origin */
    public float getD(){
        return d;
    }


    public void set(Vector3 point, Vector3 normal){
        this.normal.set(normal);
        d = -point.dot(normal);
    }

    public void set(float pointX, float pointY, float pointZ, float norX, float norY, float norZ){
        this.normal.set(norX, norY, norZ);
        d = -(pointX * norX + pointY * norY + pointZ * norZ);
    }


    public void set(Plane plane){
        this.normal.set(plane.normal);
        this.d = plane.d;
    }

    public override String ToString(){
        return normal.ToString() + ", " + d;
    }
}