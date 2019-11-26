using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace RayTrace
{

    public static class Helper
    {
        public static void get_sphere_uv(Vec3 p, ref float u, ref float v)
        {
            float phi = (float)Math.Atan2(p.z(), p.x());
            float theta = (float)Math.Asin(p.y());
            u = 1.0f - (float)((phi + Math.PI) / (2 * Math.PI));
            v = (float)((theta + Math.PI / 2.0f) / Math.PI);
        }






        public static void writeToPng(string fileName, RayCanvas rc)
        {

            // X := width  :: col
            // Y := height :: row

            int width = rc.getNumXPixels();
            int height = rc.getNumYPixels();


            WriteableBitmap wbitmap = new WriteableBitmap(
                width, height, 96, 96, PixelFormats.Bgra32, null);

            byte[, ,] pixels = new byte[height, width, 4];


            for (int col = 0; col < width; col++)
            {
                for (int row = 0; row < height; row++)
                {
                    //for (int i = 0; i < 3; i++)
                    //{
                    pixels[row, col, 3] = 255; // alpha?
                    //}

                    int ir = (int)(256 * rc.getR(col, height - row - 1));
                    int ig = (int)(256 * rc.getG(col, height - row - 1));
                    int ib = (int)(256 * rc.getB(col, height - row - 1));

                    if (ir > 255) ir = 255;
                    if (ig > 255) ig = 255;
                    if (ib > 255) ib = 255;

                    pixels[row, col, 0] = (byte)ib;  // Blue first,   PixelFormats.Bgra32
                    pixels[row, col, 1] = (byte)ig;  // Green
                    pixels[row, col, 2] = (byte)ir;  // Red last
                }
            }




            // X := width  :: col
            // Y := height :: row

            byte[] pixels1d = new byte[height * width * 4];
            int index = 0;
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        pixels1d[index++] = pixels[row, col, i];
                    }
                }
            }



            Int32Rect rect = new Int32Rect(0, 0, width, height);
            int stride = 4 * width;
            wbitmap.WritePixels(rect, pixels1d, stride, 0);


            string filePath = fileName;

            var fileStream = new FileStream(filePath, FileMode.Create);

            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(wbitmap));
            encoder.Save(fileStream);

            fileStream.Close();
        }





    }  // end Helper






    public static class Rng
    {

        private static Random _global = new Random();
        [ThreadStatic]
        private static Random _local;



        public static void seedInit(int seed)
        {
            _global = new Random(seed);
        }





        public static int Next()
        {
            Random inst = _local;
            if (inst == null)
            {
                int seed;
                lock (_global) seed = _global.Next();
                _local = inst = new Random(seed);
            }
            return inst.Next();
        }



        public static float f()
        {
            
            Random inst = _local;
            if (inst == null)
            {
                int seed;
                lock (_global) seed = _global.Next();
                _local = inst = new Random(seed);
            }
            return (float)inst.NextDouble();



        }
    } // end class Rng



    public static class Scenery
    {

        // This class holds static functions that programatically generate a list of hitables.
        // You may use functions like these instead of the input config files.
        // Remember to enable/disable the sky to accomodate the lighting type.

        public static Hitable my_light_scene()
        {
            int max_items = 500;
            List<Hitable> list = new List<Hitable>(max_items + 1);


            Material white_light_material = new DiffuseLight(new ConstantTexture(new Vec3(4.5f, 4.5f, 4.5f)));



            list.Add(new Sphere(new Vec3(0.0f, -1000.0f, 0.0f), 1000.0f, new Lambertian(TextureLib.green_white_checker)));


            list.Add(new Sphere(new Vec3(0.0f, 1.0f, 0.0f), 1.0f, new Dielectric(1.5f)));
            //list.Add(new Sphere(new Vec3(-4.0f, 1.0f, 0.0f), 1.0f, new Lambertian(burnt_sienna_texture)));
            list.Add(new Sphere(new Vec3(-4.0f, 1.0f, 0.0f), 1.0f, white_light_material));
            list.Add(new Sphere(new Vec3(4.0f, 1.0f, 0.0f), 1.0f, new Metal(new Vec3(0.7f, 0.6f, 0.5f), 0.0f)));



            int i = list.Count();

            return new Hitable_List(list, i);
        }






        public static Hitable my_random_scene()
        {
            int max_items = 500;
            List<Hitable> list = new List<Hitable>(max_items + 1);

            list.Add(new Sphere(new Vec3(0.0f, -1000.0f, 0.0f), 1000.0f, new Lambertian(TextureLib.green_white_checker)));

            if (true)
                for (int a = -11; a < 11; a++)
                {
                    for (int b = -11; b < 11; b++)
                    {
                        float choose_mat = Rng.f();
                        Vec3 center = new Vec3((float)a + 0.8f * Rng.f(), 0.2f, (float)b + 0.8f * Rng.f());
                        if ((center - new Vec3(4.0f, 0.2f, 0.0f)).length() > 1.5f &&
                            (center - new Vec3(0.0f, 0.2f, 0.0f)).length() > 1.5f &&
                            (center - new Vec3(-4.0f, 0.2f, 0.0f)).length() > 1.5f)
                        {
                            if (choose_mat < 0.8f)
                            {
                                list.Add(new Sphere(center, 0.2f, new Lambertian(new ConstantTexture(new Vec3(Rng.f(), Rng.f(), Rng.f())))));
                            }
                            else if (choose_mat < 0.95f)
                            {
                                list.Add(new Sphere(center, 0.2f,
                                    new Metal(new Vec3(0.5f * (1.0f + Rng.f()), 0.5f * (1.0f + Rng.f()), 0.5f * (1.0f + Rng.f())),
                                        0.5f * Rng.f())));
                            }
                            else
                            {
                                list.Add(new Sphere(center, 0.2f, new Dielectric(1.5f)));
                            }
                        }
                    }
                }




            list.Add(new Sphere(new Vec3(0.0f, 1.0f, 0.0f), 1.0f, new Dielectric(1.5f, new Vec3(1.0f, 1.0f, 1.0f))));
            list.Add(new Sphere(new Vec3(-4.0f, 1.0f, 0.0f), 1.0f, new Lambertian(TextureLib.burnt_sienna)));
            list.Add(new Sphere(new Vec3(4.0f, 1.0f, 0.0f), 1.0f, new Metal(new Vec3(0.7f, 0.6f, 0.5f), 0.0f)));

            int i = list.Count();

            return new Hitable_List(list, i);
        }
    }


}