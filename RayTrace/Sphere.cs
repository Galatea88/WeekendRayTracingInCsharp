using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTrace
{
    public class Sphere : Hitable
    {
        public Vec3 center;
        public float radius;
        public Material mat_type;


        public Sphere() {  }

        public Sphere(Vec3 cen, float r, Material m)
        {
            center = cen;
            radius = r;
            mat_type = m;
        }

        public override bool hit(Ray r, float t_min, float t_max, ref Hit_Record rec)
        {
            // Perform a simplified Pythagorean in 3D to determine if
            // the ray vector intersects with the sphere.
            Vec3 oc = r.origin() - center;
            float a = Vec3.dot(r.direction());
            float b = Vec3.dot(oc, r.direction());
            float c = Vec3.dot(oc) - radius * radius;
            float discriminant = b * b - a * c;
            if (discriminant > 0.0f)
            {
                float temp = (-b - (float)Math.Sqrt(b * b - a * c)) / a;
                if (temp < t_max && temp > t_min)
                {
                    rec.t = temp;
                    rec.p = r.point_at_parameter(rec.t);
                    rec.normal = (rec.p - center) / radius;
                    rec.mat_type = mat_type;
                    Helper.get_sphere_uv(rec.p, ref rec.u, ref rec.v);
                    return true;
                }
                temp = (-b + (float)Math.Sqrt(b * b - a * c)) / a;
                if (temp < t_max && temp > t_min)
                {
                    rec.t = temp;
                    rec.p = r.point_at_parameter(rec.t);
                    rec.normal = (rec.p - center) / radius;
                    rec.mat_type = mat_type;
                    Helper.get_sphere_uv(rec.p, ref rec.u, ref rec.v);
                    return true;
                }
            }

            return false;
        }
    }










    public class Cylinder : Hitable
    {
        // A cylinder has similar algebraic properties to a sphere, so I included it here.
        // Note: this only creates a vertical cylinder.

        public Vec3 center;
        public float radius;
        public Material mat_type;


        public Cylinder() { }

        public Cylinder(Vec3 cen, float r, Material m)
        {
            center = cen;
            radius = r;
            mat_type = m;
        }

        public override bool hit(Ray r, float t_min, float t_max, ref Hit_Record rec)
        {
            Vec3 oc = r.origin() - center;
            float a = r.direction().x() * r.direction().x() + r.direction().z() * r.direction().z();
            float b = 2.0f * (oc.x() * r.direction().x() + oc.z() * r.direction().z());
            float c = oc.x()*oc.x() + oc.z()*oc.z() - radius * radius;
            float discriminant = b * b - 4* a * c;
            if (discriminant > 0.0f)
            {
                float temp = (-b - (float)Math.Sqrt(b * b - 4*a * c)) / (2*a);
                if (temp < t_max && temp > t_min)
                {
                    rec.t = temp;
                    rec.p = r.point_at_parameter(rec.t);
                    rec.normal = (rec.p - center) / radius;
                    rec.normal[1] = 0.0f;
                    rec.mat_type = mat_type;
                    Helper.get_sphere_uv(rec.p, ref rec.u, ref rec.v);
                    return true;
                }
                temp = (-b + (float)Math.Sqrt(b * b - 4 * a * c)) / (2 * a);
                if (temp < t_max && temp > t_min)
                {
                    rec.t = temp;
                    rec.p = r.point_at_parameter(rec.t);
                    rec.normal = (rec.p - center) / radius;
                    rec.normal[1] = 0.0f;
                    rec.mat_type = mat_type;
                    Helper.get_sphere_uv(rec.p, ref rec.u, ref rec.v);
                    return true;
                }
            }

            return false;
        }
    }



}
