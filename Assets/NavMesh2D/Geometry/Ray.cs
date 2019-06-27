using System;
using UnityEngine;

namespace NoLockstep.AI.Navmesh2D {
    public class Ray {
        private static long serialVersionUID = -620692054835390878L;
        public Vector3 origin = new Vector3(); // 
        public Vector3 direction = new Vector3(); // 

        public Ray(){ }

        public Ray(Vector3 origin, Vector3 direction){
            this.origin.set(origin);
            this.direction.set(direction).nor();
        }

        /** @return a copy of this ray. */
        public Ray cpy(){
            return new Ray(this.origin, this.direction);
        }


        public Vector3 getEndPoint(Vector3 _out, float distance){
            return _out.set(direction).scl(distance).Add(origin);
        }

        static Vector3 tmp = new Vector3();


        /** {@inheritDoc} */
        public String toString(){
            return "ray [" + origin + ":" + direction + "]";
        }


        public Ray set(Vector3 origin, Vector3 direction){
            this.origin.set(origin);
            this.direction.set(direction);
            return this;
        }

        public Ray set(float x, float y, float z, float dx, float dy, float dz){
            this.origin.set(x, y, z);
            this.direction.set(dx, dy, dz);
            return this;
        }


        public Ray set(Ray ray){
            this.origin.set(ray.origin);
            this.direction.set(ray.direction);
            return this;
        }
    }
}