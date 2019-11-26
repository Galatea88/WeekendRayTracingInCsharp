using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTrace
{
    public class Lambertian : Material
    {
        public Texture albedo;

        public Lambertian(Texture a)
        {
            albedo = a;
        }

        public override bool scatter(Ray r_in, Hit_Record rec, ref Vec3 attenuation, ref Ray scattered)
        {
            Vec3 target = rec.p + rec.normal + Vec3.random_in_unit_sphere();
            scattered = new Ray(rec.p, target - rec.p);
            attenuation = albedo.value(0.0f, 0.0f, rec.p);
            return true;
        }

    }
}
