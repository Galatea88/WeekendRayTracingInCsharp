using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTrace
{
    public abstract class Material
    {
        public abstract bool scatter(Ray r_in, Hit_Record rec, ref Vec3 attenuation, ref Ray scattered);
        public virtual Vec3 emitted( float u, float v, Vec3 p ) { return new Vec3(0.0f, 0.0f, 0.0f); }
    }
}
