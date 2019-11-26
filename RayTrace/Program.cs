using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;






namespace RayTrace
{


    class Program
    {

        private static int numThreads = Environment.ProcessorCount;

        private static long threadCounter = 0;
        private static List<int> progressList;

        private static RayCanvas rc; // global canvas to write to
        private static Hitable world;
        private static bool sky_active_flag;











        public static Vec3 color(Ray r, int depth)
        {
            Hit_Record rec;
            rec.t = 0.0f;
            rec.u = rec.v = 0.0f;
            rec.p = null;
            rec.normal = null;
            rec.mat_type = null;
            if (world.hit(r, 0.001f, float.MaxValue, ref rec))
            {
                Ray scattered = new Ray();
                Vec3 attenuation = new Vec3();
                Vec3 emitted = rec.mat_type.emitted(rec.u, rec.v, rec.p);
                if (depth < ConfigParams.max_bounces && rec.mat_type.scatter(r, rec, ref attenuation, ref scattered))
                {
                    // recursive call, finding color of each object hit
                    return emitted + attenuation * color(scattered, depth + 1);
                }
                else
                {
                    // too many bounces, or did not scatter (it absorbed the photon)
                    //return new Vec3(0.0f, 0.0f, 0.0f); // return black
                    return emitted;
                }
            }
            else
            {
                // we hit nothing, so now hit the outer sky

                if (sky_active_flag)
                {
                    Vec3 unit_direction = Vec3.unit_vector(r.direction());

                    // Return a linear interpolation of the sky, based on Y (vertical)
                    // Higher is more white, lower is more blue-gray.
                    float t = 0.5f * (unit_direction.y() + 1.0f);
                    return (1.0f - t) * new Vec3(1.0f, 1.0f, 1.0f) + t * new Vec3(0.5f, 0.7f, 1.0f);
                }

                return new Vec3(0.0f, 0.0f, 0.0f); // black sky
            }


        }




        public static void childTracer(object data)
        {

            int offset = (int)data;


            int nx = rc.getNumXPixels();
            int ny = rc.getNumYPixels();
            int ns = rc.getNumSamples();

            int coreNum = offset + 1;
            Console.WriteLine("starting thread " + coreNum + " of " + numThreads);



            Vec3 col = new Vec3(0.0f, 0.0f, 0.0f);


            for (int j = ny - 1; j >= 0; j--)
            {
                for (int i = offset; i < nx; i += numThreads)
                {
                    for (int s = 0; s < ns; s++)
                    {
                        float u = (float)((float)i + (ConfigParams.aliasing_on ? Rng.f() : 0.0f)) / (float)nx;
                        float v = (float)((float)j + (ConfigParams.aliasing_on ? Rng.f() : 0.0f)) / (float)ny;

                        col = color(Camera.get_ray(u, v), 0);

                        rc.addR(i, j, col.r());
                        rc.addG(i, j, col.g());
                        rc.addB(i, j, col.b());

                    }

                }

                progressList[offset] += 1;
            }


            Interlocked.Increment(ref threadCounter); // signal this thread has finished

            return;

        } // end childTracer






        static void Main(string[] args)
        {





            Stopwatch sw = Stopwatch.StartNew();

            string input_config_filename = @"input.txt";  // default output file name
            string output_filename = @"outfile.png";  // default output file name
            if (args.Length > 0)
            {
                input_config_filename = args[0];
            }



            if (args.Length > 1)
            {
                output_filename = args[1];
            }



            ConfigFile.readFile(input_config_filename);
            ConfigFile.workOnConfig();





            Vec3 lookfrom = ConfigParams.look_from;
            Vec3 lookat = ConfigParams.look_at;
            float dist_to_focus = (lookfrom - lookat).length();
            float aperture = ConfigParams.aperture_size;
            float field_of_view = ConfigParams.field_of_view;


            int nx = ConfigParams.X_pixels;  // width
            int ny = ConfigParams.Y_pixels;  // height
            int ns = ConfigParams.num_samples; // number of photon samples


            new Camera(lookfrom, lookat, new Vec3(0.0f, 1.0f, 0.0f), field_of_view, (float)nx / (float)ny, aperture, dist_to_focus);

            world = ConfigParams.getScene();
            sky_active_flag = ConfigParams.sky_on;
            //world = Scenery.my_light_scene(); sky_active_flag = false;  // alternative way

            rc = new RayCanvas(nx, ny, ns);


            // call threads here.

            if (numThreads < 1) numThreads = 1;

            progressList = new List<int>();

            for (int cc = 0; cc < numThreads; cc++)
            {
                int a = 0;
                progressList.Add(a);
            }



            for (int cc = 0; cc < numThreads; cc++)
            {

                Thread thread1 = new Thread(
                    new ParameterizedThreadStart(childTracer)
                    );
                thread1.Start(cc);

            }



            // wait here until finished threads
            while (Interlocked.Read(ref threadCounter) < numThreads)
            {
                int pi = 0;
                for (int i = 0; i < progressList.Count; i++)
                {
                    pi += progressList[i];
                }
                float pp = (((float)pi / (float)progressList.Count) / (float)ny) * 100.0f;
                if (pp > 100.0f) pp = 100.0f;

                Console.WriteLine(pp.ToString("F2") + "%");
                Thread.Sleep(1000);
            }




            // now when finished, finalize the values
            rc.finalize();


            // write the PNG file
            Helper.writeToPng(output_filename, rc);


            sw.Stop();
            Console.WriteLine(String.Format("Elapsed time: {0:N1} sec", sw.Elapsed.TotalSeconds));
            Console.WriteLine("Finished.");


        } // end Main



    }
}
