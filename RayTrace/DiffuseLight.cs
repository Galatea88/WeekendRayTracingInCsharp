using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTrace
{
    public class DiffuseLight : Material
    {
        Texture emit;

        public DiffuseLight(Texture a)
        {
            emit = a;
        }

        public override bool scatter(Ray r_in, Hit_Record rec, ref Vec3 attenuation, ref Ray scattered)
        {
            return false;
        }

        public override Vec3 emitted(float u, float v, Vec3 p)
        {
            return emit.value(u, v, p);
        }
    }
}
