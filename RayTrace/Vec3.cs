using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTrace
{
    public class Vec3
    {
        private float[] e = new float[3];

        public Vec3() { }

        public Vec3(double e0, double e1, double e2)
        {
            e[0] = (float)e0;
            e[1] = (float)e1;
            e[2] = (float)e2;
        }

        public Vec3(float e0, float e1, float e2)
        {
            e[0] = e0;
            e[1] = e1;
            e[2] = e2;
        }

        public float x() { return e[0]; }
        public float y() { return e[1]; }
        public float z() { return e[2]; }
        public float r() { return e[0]; }
        public float g() { return e[1]; }
        public float b() { return e[2]; }

        public static Vec3 operator +(Vec3 lhs, Vec3 rhs) { return new Vec3(lhs.e[0] + rhs.e[0], lhs.e[1] + rhs.e[1], lhs.e[2] + rhs.e[2]); }
        public static Vec3 operator -(Vec3 lhs, Vec3 rhs) { return new Vec3(lhs.e[0] - rhs.e[0], lhs.e[1] - rhs.e[1], lhs.e[2] - rhs.e[2]); }
        public static Vec3 operator -(Vec3 u) { return new Vec3(-u.e[0], -u.e[1], -u.e[2]); }
        //public static vec3 operator [](int i) { return e[i]; }
        public float this[int i]
        {
            get { return e[i]; }
            set { e[i] = value; }
        }

        public static Vec3 operator *(Vec3 lhs, Vec3 rhs) { return new Vec3(lhs.e[0] * rhs.e[0], lhs.e[1] * rhs.e[1], lhs.e[2] * rhs.e[2]); }
        public static Vec3 operator *(Vec3 lhs, float rhs) { return new Vec3(lhs.e[0] * rhs, lhs.e[1] * rhs, lhs.e[2] * rhs); }
        public static Vec3 operator *(float lhs, Vec3 rhs) { return new Vec3(lhs * rhs.e[0], lhs * rhs.e[1], lhs * rhs.e[2]); }
        public static Vec3 operator /(Vec3 lhs, float rhs) { return new Vec3(lhs.e[0] / rhs, lhs.e[1] / rhs, lhs.e[2] / rhs); }

        public float length()
        {
            return (float)Math.Sqrt(e[0] * e[0] + e[1] * e[1] + e[2] * e[2]);
        }
        public float squared_length()
        {
            return (e[0] * e[0] + e[1] * e[1] + e[2] * e[2]);
        }
        public void make_unit_vector()
        {
            float k = (float)(1.0 / Math.Sqrt(e[0] * e[0] + e[1] * e[1] + e[2] * e[2]));
            e[0] *= k; e[1] *= k; e[2] *= k;
        }
        public static float dot(Vec3 v1)
        {
            return (v1.e[0] * v1.e[0] + v1.e[1] * v1.e[1] + v1.e[2] * v1.e[2]);
        }
        public static float dot(Vec3 v1, Vec3 v2)
        {
            return (v1.e[0] * v2.e[0] + v1.e[1] * v2.e[1] + v1.e[2] * v2.e[2]);
        }
        public static Vec3 cross(Vec3 v1, Vec3 v2)
        {
            return new Vec3((v1.e[1] * v2.e[2] - v1.e[2] * v2.e[1]),
                        (-(v1.e[0] * v2.e[2] - v1.e[2] * v2.e[0])),
                        (v1.e[0] * v2.e[1] - v1.e[1] * v2.e[0]));
        }
        public static Vec3 unit_vector(Vec3 v)
        {
            return (v / v.length());
        }

        public static Vec3 reflect(Vec3 v, Vec3 n)
        {
            return v - 2.0f * dot(v, n) * n;
        }

        public static Vec3 random_in_unit_disk()
        {
            Vec3 p = new Vec3();
            p[2] = 0.0f;
            do
            {
                //p = (float)2.0 * new Vec3(Rng.f(), Rng.f(), 0.0f) - new Vec3(1.0f, 1.0f, 0.0f);
                //p = new Vec3(2.0f * Rng.f() - 1.0f, 2.0f * Rng.f() - 1.0f, 0.0f);
                p[0] = 2.0f * Rng.f() - 1.0f;
                p[1] = 2.0f * Rng.f() - 1.0f;
                
            } while (dot(p) >= 1.0f);
            return p;
        }

        public static Vec3 random_in_unit_sphere()
        {
            Vec3 p = new Vec3();
            do
            {
                //p = (float)2.0 * new Vec3(Rng.f(), Rng.f(), Rng.f()) - new Vec3(1.0f, 1.0f, 1.0f);
                //p = new Vec3(2.0f * Rng.f() - 1.0f, 2.0f * Rng.f() - 1.0f, 2.0f * Rng.f() - 1.0f);
                p[0] = 2.0f * Rng.f() - 1.0f;
                p[1] = 2.0f * Rng.f() - 1.0f;
                p[2] = 2.0f * Rng.f() - 1.0f;
            } while (p.squared_length() >= 1.0f);
            return p;
        }




    }
}
