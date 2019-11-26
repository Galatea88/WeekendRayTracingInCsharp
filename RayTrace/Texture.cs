using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTrace
{
    public abstract class Texture
    {
        public Texture() { }

        public abstract Vec3 value(float u, float v, Vec3 p);
    }



    public class ConstantTexture : Texture
    {
        Vec3 color;


        public ConstantTexture() { }
        public ConstantTexture(Vec3 c)
        {
            color = c;
        }
        public ConstantTexture(Vec3 c, float f)
        {
            color = f*c;
        }
        public override Vec3 value(float u, float v, Vec3 p)
        {
            return color;
        }
    }



    public class CheckerTexture : Texture
    {
        Texture even, odd;

        public CheckerTexture() { }
        public CheckerTexture(Texture t0, Texture t1)
        {
            even = t0;
            odd = t1;
        }
        public override Vec3 value(float u, float v, Vec3 p)
        {
            float sines = (float)(Math.Sin(10.0 * p.x()) * Math.Sin(10.0 * p.y()) * Math.Sin(10.0 * p.z()));
            if (sines < 0.0f)
            {
                return odd.value(u, v, p);
            }
            else
            {
                return even.value(u, v, p);
            }
        }
    }






    public static class TextureLib
    {
        public static Texture white = new ConstantTexture(new Vec3(1.0f, 1.0f, 1.0f));
        public static Texture gray = new ConstantTexture(new Vec3(0.5f, 0.5f, 0.5f));
        public static Texture red = new ConstantTexture(new Vec3(1.0f, 0.0f, 0.0f));
        public static Texture green = new ConstantTexture(new Vec3(0.0f, 1.0f, 0.0f));
        public static Texture blue = new ConstantTexture(new Vec3(0.0f, 0.0f, 1.0f));
        public static Texture burnt_sienna = new ConstantTexture(new Vec3(1.0f, 0.3f, 0.1f));
        public static Texture green_white_checker = new CheckerTexture(
                                        new ConstantTexture(new Vec3(0.2f, 0.3f, 0.1f)),
                                        new ConstantTexture(new Vec3(0.9f, 0.9f, 0.9f)));
    }


}



