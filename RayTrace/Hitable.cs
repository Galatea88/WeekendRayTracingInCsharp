using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTrace
{

    public struct Hit_Record
    {
        public float t;
        public float u, v;
        public Vec3 p;
        public Vec3 normal;
        public Material mat_type;
    };

    public abstract class Hitable
    {
        public abstract bool hit(Ray r, float t_min, float t_max, ref Hit_Record rec);
    }



    public class Hitable_List : Hitable
    {
        public List<Hitable> list;
        public int list_size;

        public Hitable_List() { }

        public Hitable_List(List<Hitable> l, int n)
        {
            list = l;
            list_size = n;
        }

        public override bool hit(Ray r, float t_min, float t_max, ref Hit_Record rec)
        {
            Hit_Record temp_rec;
            temp_rec.t = 0.0f;
            temp_rec.u = temp_rec.v = 0.0f;
            temp_rec.p = null; // new Vec3();
            temp_rec.normal = null; // new Vec3();
            temp_rec.mat_type = null; // new Lambertian(new Vec3());

            bool hit_anything = false;
            float closest_so_far = t_max;
            for (int i = 0; i < list_size; i++)
            {
                if (list[i].hit(r, t_min, closest_so_far, ref temp_rec))
                {
                    hit_anything = true;
                    closest_so_far = temp_rec.t;
                    rec = temp_rec;
                }
            }


            return hit_anything;
        }
    }

}
