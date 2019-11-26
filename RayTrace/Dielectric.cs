using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTrace
{
    public class Dielectric : Material
    {

        public float ref_idx; // refraction index
        public Vec3 color;

        public Dielectric(float ri)
        {
            ref_idx = ri;
            color = new Vec3(1.0f, 1.0f, 1.0f); // clear glass
        }

        public Dielectric(float ri, Vec3 c)
        {
            ref_idx = ri;

            if (c[0] > 1.0f || c[1] > 1.0f || c[2] > 1.0f)
            {
                c.make_unit_vector();
            }
            color = c;
        }


        private float schlick(float cosine, float ref_idx)
        {
            float r0 = (1.0f - ref_idx) / (1 + ref_idx);
            r0 = r0 * r0;
            return r0 + (1.0f - r0) * (float)Math.Pow((1.0f - cosine), 5.0);
        }


        private bool refract(Vec3 v, Vec3 n, float ni_over_nt, ref Vec3 refracted)
        {
            Vec3 uv = Vec3.unit_vector(v);
            float dt = Vec3.dot(uv, n);
            float discriminant = 1.0f - ni_over_nt * ni_over_nt * (1.0f - dt * dt);
            if (discriminant > 0.0f)
            {
                refracted = ni_over_nt * (uv - n * dt) - n * (float)Math.Sqrt(discriminant);
                return true;
            }
            else
                //refracted = new vec3(0.0f, 0.0f, 0.0f);
                return false;
        }



        public override bool scatter(Ray r_in, Hit_Record rec, ref Vec3 attenuation, ref Ray scattered)
        {
            Vec3 outward_normal;
            Vec3 reflected = Vec3.reflect(r_in.direction(), rec.normal);
            float ni_over_nt;
            attenuation = color;
            Vec3 refracted = new Vec3(1.0f, 0.0f, 0.0f);
            float reflect_prob;
            float cosine;

            if (Vec3.dot(r_in.direction(), rec.normal) > 0.0f)
            {
                outward_normal = -rec.normal;
                ni_over_nt = ref_idx;
                cosine = ref_idx * Vec3.dot(r_in.direction(), rec.normal) / r_in.direction().length();
            }
            else
            {
                outward_normal = rec.normal;
                ni_over_nt = 1.0f / ref_idx;
                cosine = -Vec3.dot(r_in.direction(), rec.normal) / r_in.direction().length();
            }

            if (refract(r_in.direction(), outward_normal, ni_over_nt, ref refracted))
            {
                reflect_prob = schlick(cosine, ref_idx);
            }
            else
            {
                scattered = new Ray(rec.p, reflected);
                reflect_prob = 1.0f;
            }

            if (Rng.f() < reflect_prob)
            {
                scattered = new Ray(rec.p, reflected); // reFLEcted
            }
            else
            {
                scattered = new Ray(rec.p, refracted); // reFRActed
            }


            return true;

        }
    }
}
