using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTrace
{



    public class Ray
    {
        public Vec3 A;
        public Vec3 B;

        public Ray() { }
        public Ray(Vec3 a, Vec3 b)
        {
            A = a;
            B = b;
        }
        public Vec3 origin() { return A; }
        public Vec3 direction() { return B; }
        public Vec3 point_at_parameter(float t)
        {
            return A + t * B;
        }
    }
}
