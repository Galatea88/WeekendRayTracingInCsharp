using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RayTrace
{
    public class RayCanvas
    {
        public float[,] r_col;
        public float[,] g_col;
        public float[,] b_col;
        public int num_x_pixels, num_y_pixels, num_samples;



        public RayCanvas(int nx, int ny, int ns)
        {
            num_x_pixels = nx;
            num_y_pixels = ny;
            num_samples = ns;

            r_col = new float[nx, ny];
            g_col = new float[nx, ny];
            b_col = new float[nx, ny];
        }




        public void addR(int x, int y, float val)
        {
            r_col[x, y] += val;
        }

        public void addG(int x, int y, float val)
        {
            g_col[x, y] += val;
        }

        public void addB(int x, int y, float val)
        {
            b_col[x, y] += val;
        }

        public void finalize()
        {
            for (int x = 0; x < num_x_pixels; x++)
            {
                for (int y = 0; y < num_y_pixels; y++)
                {
                    // normalize based on how many samples (photons) we used
                    r_col[x, y] /= num_samples;
                    g_col[x, y] /= num_samples;
                    b_col[x, y] /= num_samples;

                    // apply a gamma correction by simply doing a Sqrt
                    r_col[x, y] = (float)Math.Sqrt(r_col[x, y]);
                    g_col[x, y] = (float)Math.Sqrt(g_col[x, y]);
                    b_col[x, y] = (float)Math.Sqrt(b_col[x, y]);
                }
            }
        }

        public float getR(int x, int y)
        {
            return r_col[x, y];
        }

        public float getG(int x, int y)
        {
            return g_col[x, y];
        }

        public float getB(int x, int y)
        {
            return b_col[x, y];
        }

        public int getNumXPixels()
        {
            return num_x_pixels;
        }

        public int getNumYPixels()
        {
            return num_y_pixels;
        }

        public int getNumSamples()
        {
            return num_samples;
        }


    }
}
