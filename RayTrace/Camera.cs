using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace RayTrace
{

    public class Camera
    {

        public static Vec3 origin;
        public static Vec3 lower_left_corner;
        public static Vec3 horizontal;
        public static Vec3 vertical;
        public static Vec3 u, v, w;
        public static float lens_radius;

        public Camera(Vec3 lookfrom, Vec3 lookat, Vec3 vup, float vfov, float aspect, float aperture, float focus_dist)
        {
            lens_radius = aperture / 2.0f;
            float theta = vfov * (float)Math.PI / 180.0f;
            float half_height = (float)Math.Tan(theta / 2.0f);
            float half_width = aspect * half_height;
            origin = lookfrom;
            w = Vec3.unit_vector(lookfrom - lookat);
            u = Vec3.unit_vector(Vec3.cross(vup, w));
            v = Vec3.cross(w, u);
            lower_left_corner = origin - half_width * focus_dist * u - half_height * focus_dist * v - focus_dist * w;
            horizontal = 2.0f * half_width * focus_dist * u;
            vertical = 2.0f * half_height * focus_dist * v;
        }


        public static Ray get_ray(float s, float t)
        {
            Vec3 rd = lens_radius * Vec3.random_in_unit_disk();
            Vec3 offset = u * rd.x() + v * rd.y();

            return new Ray(origin + offset, lower_left_corner + s * horizontal + t * vertical - origin - offset);
        }

    }

}
