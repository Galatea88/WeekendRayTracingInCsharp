using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTrace
{
    public class Metal : Material
    {

        public Vec3 albedo;
        public float fuzz;


        public Metal(Vec3 a, float f)
        {

            if (a[0] > 1.0f || a[1] > 1.0f || a[2] > 1.0f)
            {
                a.make_unit_vector();
            }
            albedo = a;

            if (f < 1.0f)
                fuzz = f;
            else
                fuzz = 1.0f;
        }



        public override bool scatter(Ray r_in, Hit_Record rec, ref Vec3 attenuation, ref Ray scattered)
        {
            Vec3 reflected = Vec3.reflect(Vec3.unit_vector(r_in.direction()), rec.normal);
            scattered = new Ray(rec.p, reflected + fuzz * Vec3.random_in_unit_sphere());
            attenuation = albedo;
            return (Vec3.dot(scattered.direction(), rec.normal) > 0.0f);
        }

        
    }
}
