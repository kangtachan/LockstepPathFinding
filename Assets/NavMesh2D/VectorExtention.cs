using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;


public static class VectorExtention {
    public static Vector3 Add(this Vector3 vec, Vector3 val){
        return vec + val;
    }

    public static Vector3 sub(this Vector3 vec, Vector3 val){
        return vec - val;
    }

    public static Vector3 scl(this Vector3 vec, float val){
        return vec * val;
    }

    public static float dot(this Vector3 vec, Vector3 val){
        return Vector3.Dot(vec, val);
    }

    public static float dot(this Vector3 vec, float x, float y, float z){
        return Vector3.Dot(vec, new Vector3(x, y, z));
    }

    public static Vector3 set(this Vector3 vec, Vector3 val){
        vec = val;
        return vec;
    }

    public static Vector3 mulAdd(this Vector3 _this, Vector3 vec, float scalar){
        _this.x += vec.x * scalar;
        _this.y += vec.y * scalar;
        _this.z += vec.z * scalar;
        return _this;
    }

    public static Vector3 set(this Vector3 vec, float x, float y, float z){
        vec.x = x;
        vec.y = y;
        vec.z = z;
        return vec;
    }

    public static Vector3 cross(this Vector3 vec, Vector3 vector){
        return vec.set(vec.y * vector.z - vec.z * vector.y, vec.z * vector.x - vec.x * vector.z,
            vec.x * vector.y - vec.y * vector.x);
    }

    public static Vector3 cross(this Vector3 vec, float x, float y, float z){
        return vec.set(vec.y * z - vec.z * y, vec.z * x - vec.x * z, vec.x * y - vec.y * x);
    }

    public static Vector3 nor(this Vector3 vec){
        return vec.normalized;
    }

    public static float len(this Vector3 vec){
        return vec.magnitude;
    }

    public static float dst2(this Vector3 vec, Vector3 p){
        return dst2(vec.x, vec.z, p.x, p.z);
    }

    public static float dst2(float x1, float z1, float x2, float z2){
        x1 -= x2;
        z1 -= z2;
        return (x1 * x1 + z1 * z1);
    }
    public static T get<T>(this List<T> lst, int idx){
        return lst[idx];
    }
    public static Tval get<Tkey,Tval>(this Dictionary<Tkey,Tval> lst, Tkey idx){
        return lst[idx];
    }
}

