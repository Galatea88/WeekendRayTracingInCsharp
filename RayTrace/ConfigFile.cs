using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTrace
{

    public static class ConfigParams
    {
        public static List<Hitable> list = new List<Hitable>(10000 + 1);
        public static bool sky_on = true;
        public static int num_samples = 10;
        public static int X_pixels = 200, Y_pixels = 100;
        public static bool aliasing_on = true;
        public static float aperture_size = 0.0f;
        public static float field_of_view = 20.0f;
        public static Vec3 look_at = new Vec3(0.0f, 0.0f, 0.0f);
        public static Vec3 look_from = new Vec3(10.0f, 10.0f, 10.0f);
        public static int max_bounces = 50;


        public static Hitable getScene()
        {
            //int i = list.Count();
            return new Hitable_List(list, list.Count());
        }


    }


    public static class ConfigFile
    {

        public static List<List<string>> configList = new List<List<string>>();





        public static void readFile(string inFileName)
        {
            char[] seperators = { ' ', ',', '='};


            string[] lines = File.ReadAllLines(inFileName);
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                int comment_start_location = line.IndexOf(';');
                if (comment_start_location >= 0)
                {
                    line = line.Remove(comment_start_location);
                }


                List<string> items = line.Split(seperators, StringSplitOptions.RemoveEmptyEntries).ToList<string>();

                if (items.Count() >= 2)
                {
                    configList.Add(items);
                }


            }


        }




        public static bool workOnConfig()
        {
            List<string> items;

            for (int i = 0; i < configList.Count(); i++)
            {
                items = configList[i];


                bool bflag;
                int iN, iX, iY;
                float fX1, fY1, fZ1;
                float fR1, fG1, fB1, fR2, fG2, fB2;
                float fRadius, fRough, fIntensity;
                string sMtr;

                if (verifySky(items, out bflag))
                {
                    ConfigParams.sky_on = bflag;
                }
                else if (verifyNumSamples(items, out iN))
                {
                    ConfigParams.num_samples = iN;
                }
                else if (verifyMaxBounces(items, out iN))
                {
                    ConfigParams.max_bounces = iN;
                }
                else if (verifyXYPixels(items, out iX, out iY))
                {
                    ConfigParams.X_pixels = iX;
                    ConfigParams.Y_pixels = iY;
                }
                else if (verifyAliasingOn(items, out bflag))
                {
                    ConfigParams.aliasing_on = bflag;
                }
                else if (verifyApertureSize(items, out fX1))
                {
                    ConfigParams.aperture_size = fX1;
                }
                else if (verifyFieldOfView(items, out fX1))
                {
                    ConfigParams.field_of_view = fX1;
                }
                else if (verifyLookAt(items, out fX1, out fY1, out fZ1))
                {
                    ConfigParams.look_at[0] = fX1;
                    ConfigParams.look_at[1] = fZ1; // swapped
                    ConfigParams.look_at[2] = -fY1; // swapped, neg
                }
                else if (verifyLookFrom(items, out fX1, out fY1, out fZ1))
                {
                    ConfigParams.look_from[0] = fX1;
                    ConfigParams.look_from[1] = fZ1; // swapped
                    ConfigParams.look_from[2] = -fY1; // swapped, neg
                }
                else if (verifyObject(items, out fX1, out fY1, out fZ1, out fRadius, out sMtr))
                {
                    if (items.Count() >= 7)
                    {


                        List<string> mtr_param_list = items.GetRange(6, items.Count() - 6);


                        if (sMtr.Equals("matte") && verifyMatte(mtr_param_list, out fR1, out fG1, out fB1))
                        {
                            if (items[1].ToLower().Equals("sphere"))
                                ConfigParams.list.Add(new Sphere(new Vec3(fX1, fZ1, -fY1), fRadius, new Lambertian(new ConstantTexture(new Vec3(fR1, fG1, fB1)))));
                            else if (items[1].ToLower().Equals("cylinder"))
                                ConfigParams.list.Add(new Cylinder(new Vec3(fX1, fZ1, -fY1), fRadius, new Lambertian(new ConstantTexture(new Vec3(fR1, fG1, fB1)))));
                        }
                        else if (sMtr.Equals("checker") && verifyChecker(mtr_param_list,
                                    out fR1, out fG1, out fB1,
                                    out fR2, out fG2, out fB2))
                        {
                            if (items[1].ToLower().Equals("sphere"))
                                ConfigParams.list.Add(new Sphere(new Vec3(fX1, fZ1, -fY1), fRadius, new Lambertian(
                                                new CheckerTexture(
                                                    new ConstantTexture(new Vec3(fR1, fG1, fB1)),
                                                    new ConstantTexture(new Vec3(fR2, fG2, fB2))
                                                    )
                                                )));
                            else if (items[1].ToLower().Equals("cylinder"))
                                ConfigParams.list.Add(new Cylinder(new Vec3(fX1, fZ1, -fY1), fRadius, new Lambertian(
                                                new CheckerTexture(
                                                    new ConstantTexture(new Vec3(fR1, fG1, fB1)),
                                                    new ConstantTexture(new Vec3(fR2, fG2, fB2))
                                                    )
                                                )));
                        }
                        else if (sMtr.Equals("metal") && verifyMetal(mtr_param_list,
                                    out fR1, out fG1, out fB1,
                                    out fRough))
                        {
                            if (items[1].ToLower().Equals("sphere"))
                            {
                                ConfigParams.list.Add(new Sphere(new Vec3(fX1, fZ1, -fY1), fRadius, new Metal(new Vec3(fR1, fG1, fB1), fRough)));
                            }
                            else if (items[1].ToLower().Equals("cylinder"))
                            {
                                ConfigParams.list.Add(new Cylinder(new Vec3(fX1, fZ1, -fY1), fRadius, new Metal(new Vec3(fR1, fG1, fB1), fRough)));
                            }
                            
                        }
                        else if (sMtr.Equals("glass") && verifyGlass(mtr_param_list,
                                    out fR1, out fG1, out fB1))
                        {
                            if (items[1].ToLower().Equals("sphere"))
                                ConfigParams.list.Add(new Sphere(new Vec3(fX1, fZ1, -fY1), fRadius, new Dielectric(1.5f, new Vec3(fR1, fG1, fB1))));
                            else if (items[1].ToLower().Equals("cylinder"))
                                ConfigParams.list.Add(new Cylinder(new Vec3(fX1, fZ1, -fY1), fRadius, new Dielectric(1.5f, new Vec3(fR1, fG1, fB1))));
                        }
                        else if (sMtr.Equals("light") && verifyLight(mtr_param_list,
                                    out fR1, out fG1, out fB1,
                                    out fIntensity))
                        {
                            if (items[1].ToLower().Equals("sphere"))
                                ConfigParams.list.Add(new Sphere(new Vec3(fX1, fZ1, -fY1), fRadius, new DiffuseLight(new ConstantTexture(new Vec3(fR1, fG1, fB1), fIntensity))));
                            else if (items[1].ToLower().Equals("cylinder"))
                                ConfigParams.list.Add(new Cylinder(new Vec3(fX1, fZ1, -fY1), fRadius, new DiffuseLight(new ConstantTexture(new Vec3(fR1, fG1, fB1), fIntensity))));
                        }
                        else
                        {
                            // no good match on KNOWN material types
                            // ignore it
                        }



                    }
                    else
                    {
                        // no material type even listed
                        // ignore it
                    }



                }
                else
                {
                    // unknown config command encountered
                    // ignore it
                }




            } // end loop through each parsed config line

            return true;
        }








        private static bool verifySky(List<string> v, out bool f)
        {
            f = true;

            if (v.Count() >= 2 &&
                v[0].ToLower().Equals("sky_on") &&
                (
                    v[1].ToLower().Equals("on") ||
                    v[1].ToLower().Equals("true") ||
                    v[1].Equals("1")
                ))
            {

                f = true;
                return true;
            }




            if (v.Count() >= 2 &&
                v[0].ToLower().Equals("sky_on") &&
                (
                    v[1].ToLower().Equals("off") ||
                    v[1].ToLower().Equals("false") ||
                    v[1].Equals("0")
                ))
            {

                f = false;
                return true;
            }



            return false;
        }




        

        private static bool verifyNumSamples(List<string> v, out int n)
        {
            n = 0;

            if (v.Count() >= 2)
            {

                try
                {
                    n = Int32.Parse(v[1]);
                }
                catch (FormatException)
                {
                    // unable to extract a valid number
                    return false;
                }

                if (v[0].ToLower().Equals("num_samples") &&
                    n >= 1)
                {

                    return true;
                }

            }


            return false;
        }





        private static bool verifyMaxBounces(List<string> v, out int n)
        {
            n = 0;

            if (v.Count() >= 2)
            {

                try
                {
                    n = Int32.Parse(v[1]);
                }
                catch (FormatException)
                {
                    // unable to extract a valid number
                    return false;
                }

                if (v[0].ToLower().Equals("max_bounces") &&
                    n >= 1)
                {

                    return true;
                }

            }


            return false;
        }






        private static bool verifyXYPixels(List<string> v, out int X, out int Y)
        {
            X = Y = 0;

            if (v.Count() >= 3)
            {
                

                try
                {
                    X = Int32.Parse(v[1]);
                    Y = Int32.Parse(v[2]);
                }
                catch (FormatException)
                {
                    // unable to extract a valid number
                    return false;
                }

                if (v[0].ToLower().Equals("xy_pixels") &&
                    X >= 1 &&
                    Y >= 1)
                {

                    return true;
                }

            }


            return false;
        }




        private static bool verifyAliasingOn(List<string> v, out bool f)
        {
            f = true;

            if (v.Count() >= 2 &&
                v[0].ToLower().Equals("aliasing_on") &&
                (
                    v[1].ToLower().Equals("on") ||
                    v[1].ToLower().Equals("true") ||
                    v[1].Equals("1")
                ))
            {

                f = true;
                return true;
            }



            if (v.Count() >= 2 &&
                v[0].ToLower().Equals("aliasing_on") &&
                (
                    v[1].ToLower().Equals("off") ||
                    v[1].ToLower().Equals("false") ||
                    v[1].Equals("0")
                ))
            {

                f = false;
                return true;
            }




            return false;
        }


        private static bool verifyApertureSize(List<string> v, out float n)
        {
            n = 0.0f;

            if (v.Count() >= 2)
            {

                try
                {
                    n = float.Parse(v[1]);
                }
                catch (FormatException)
                {
                    // unable to extract a valid number
                    return false;
                }

                if (v[0].ToLower().Equals("aperture_size") &&
                    n >= 0.0f)
                {

                    return true;
                }

            }


            return false;
        }




        private static bool verifyFieldOfView(List<string> v, out float n)
        {
            n = 0.0f;

            if (v.Count() >= 2)
            {

                try
                {
                    n = float.Parse(v[1]);
                }
                catch (FormatException)
                {
                    // unable to extract a valid number
                    return false;
                }

                if (v[0].ToLower().Equals("field_of_view") &&
                    n >= 0.0f)
                {

                    return true;
                }

            }


            return false;
        }



        private static bool verifyLookAt(List<string> v, out float X, out float Y, out float Z)
        {
            X = 0.0f; Y = 0.0f; Z = 0.0f;

            if (v.Count() >= 4)
            {
                try
                {
                    X = float.Parse(v[1]);
                    Y = float.Parse(v[2]);
                    Z = float.Parse(v[3]);
                }
                catch (FormatException)
                {
                    // unable to extract a valid number
                    return false;
                }


                if (v[0].ToLower().Equals("look_at"))
                {

                    return true;
                }

            }


            return false;
        }


        private static bool verifyLookFrom(List<string> v, out float X, out float Y, out float Z)
        {
            X = Y = Z = 0.0f;

            if (v.Count() >= 4)
            {
                try
                {
                    X = float.Parse(v[1]);
                    Y = float.Parse(v[2]);
                    Z = float.Parse(v[3]);
                }
                catch (FormatException)
                {
                    // unable to extract a valid number
                    return false;
                }


                if (v[0].ToLower().Equals("look_from"))
                {

                    return true;
                }

            }

            return false;
        }

        //object sphere position radius material
        private static bool verifyObject(List<string> v, out float X, out float Y, out float Z, out float R, out string material)
        {
            X = Y = Z = R = 0.0f;
            material = "";

            if (v.Count() >= 7)
            {
                try
                {
                    X = float.Parse(v[2]);
                    Y = float.Parse(v[3]);
                    Z = float.Parse(v[4]);
                    R = float.Parse(v[5]);
                }
                catch (FormatException)
                {
                    // unable to extract a valid number
                    return false;
                }

                material = v[6].ToLower();


                if (    v[0].ToLower().Equals("object") &&
                        (v[1].ToLower().Equals("sphere") ||
                         v[1].ToLower().Equals("cylinder"))
                   
                    )
                {

                    return true;
                }

            }

            return false;
        }



        //    material:
        //        matte color
        //        matte X Y Z
        private static bool verifyMatte(List<string> v, out float R, out float G, out float B)
        {
            R = G = B = 0.0f;

            if (v.Count() == 4)
            {
                try
                {
                    R = float.Parse(v[1]);
                    G = float.Parse(v[2]);
                    B = float.Parse(v[3]);
                }
                catch (FormatException)
                {
                    // unable to extract a valid number
                    return false;
                }


                if (v[0].ToLower().Equals("matte"))
                {

                    return true;
                }

            }



            if (v.Count() == 2)
            {
                bool isGood = getColorRGBValues(v[1].ToLower(), out R, out G, out B);


                if (v[0].ToLower().Equals("matte") && isGood)
                {

                    return true;
                }

            }


            return false;
        }





        //        checker color_1 color_2  (3)
        //        checker X Y Z color_2    (5)
        //        checker color_1 X Y Z    (5)
        //        checker X Y Z X Y Z      (7)
        private static bool verifyChecker(List<string> v,
                                    out float R1, out float G1, out float B1,
                                    out float R2, out float G2, out float B2)
        {
            R1 = G1 = B1 = 0.0f;
            R2 = G2 = B2 = 0.0f;

            if (v.Count() == 3)
            {
                bool isGood1 = getColorRGBValues(v[1].ToLower(), out R1, out G1, out B1);
                bool isGood2 = getColorRGBValues(v[2].ToLower(), out R2, out G2, out B2);


                if (v[0].ToLower().Equals("checker") && isGood1 && isGood2)
                {

                    return true;
                }

            }



            if (v.Count() == 7)
            {
                try
                {
                    R1 = float.Parse(v[1]);
                    G1 = float.Parse(v[2]);
                    B1 = float.Parse(v[3]);
                    R2 = float.Parse(v[4]);
                    G2 = float.Parse(v[5]);
                    B2 = float.Parse(v[6]);
                }
                catch (FormatException)
                {
                    // unable to extract a valid number
                    return false;
                }


                if (v[0].ToLower().Equals("checker"))
                {

                    return true;
                }

            }

            return false;
        }




        //        metal color roughness
        //        metal X Y Z roughness
        private static bool verifyMetal(List<string> v,
                                    out float R, out float G, out float B,
                                    out float roughness)
        {
            R = G = B = roughness = 0.0f;

            if (v.Count() == 3)
            {
                bool isGood = getColorRGBValues(v[1].ToLower(), out R, out G, out B);


                try
                {
                    roughness = float.Parse(v[2]);
                }
                catch (FormatException)
                {
                    // unable to extract a valid number
                    return false;
                }




                if (v[0].ToLower().Equals("metal") && isGood)
                {
                    return true;
                }

            }







            if (v.Count() == 5)
            {
                try
                {
                    R = float.Parse(v[1]);
                    G = float.Parse(v[2]);
                    B = float.Parse(v[3]);
                    roughness = float.Parse(v[4]);
                }
                catch (FormatException)
                {
                    // unable to extract a valid number
                    return false;
                }


                if (v[0].ToLower().Equals("metal"))
                {

                    return true;
                }

            }

            return false;
        }






        //        glass color
        //        glass X Y Z
        private static bool verifyGlass(List<string> v,
                                    out float R, out float G, out float B)
        {
            R = G = B = 0.0f;

            if (v.Count() == 2)
            {
                bool isGood = getColorRGBValues(v[1].ToLower(), out R, out G, out B);

                if (v[0].ToLower().Equals("glass") && isGood)
                {
                    return true;
                }

            }







            if (v.Count() == 4)
            {
                try
                {
                    R = float.Parse(v[1]);
                    G = float.Parse(v[2]);
                    B = float.Parse(v[3]);
                }
                catch (FormatException)
                {
                    // unable to extract a valid number
                    return false;
                }


                if (v[0].ToLower().Equals("glass"))
                {
                    return true;
                }

            }

            return false;
        }





        //        light color intensity
        //        light X Y Z intensity
        private static bool verifyLight(List<string> v,
                                    out float R, out float G, out float B,
                                    out float intensity)
        {
            R = G = B = intensity = 0.0f;

            if (v.Count() == 3)
            {
                bool isGood = getColorRGBValues(v[1].ToLower(), out R, out G, out B);


                try
                {
                    intensity = float.Parse(v[2]);
                }
                catch (FormatException)
                {
                    // unable to extract a valid number
                    return false;
                }




                if (v[0].ToLower().Equals("light") && isGood)
                {
                    return true;
                }

            }







            if (v.Count() == 5)
            {
                try
                {
                    R = float.Parse(v[1]);
                    G = float.Parse(v[2]);
                    B = float.Parse(v[3]);
                    intensity = float.Parse(v[4]);
                }
                catch (FormatException)
                {
                    // unable to extract a valid number
                    return false;
                }


                if (v[0].ToLower().Equals("light"))
                {

                    return true;
                }

            }

            return false;
        }






        private static bool getColorRGBValues(string inStr, out float R, out float G, out float B)
        {
            R = G = B = 0.0f;

            if (inStr.Equals("red"))    { R = 1.0f; G = 0.0f; B = 0.0f; return true; }
            if (inStr.Equals("green"))  { R = 0.0f; G = 1.0f; B = 0.0f; return true; }
            if (inStr.Equals("blue"))   { R = 0.0f; G = 0.0f; B = 1.0f; return true; }
            if (inStr.Equals("white"))  { R = 1.0f; G = 1.0f; B = 1.0f; return true; }
            if (inStr.Equals("black"))  { R = 0.0f; G = 0.0f; B = 0.0f; return true; }
            if (inStr.Equals("gray"))   { R = 0.5f; G = 0.5f; B = 0.5f; return true; }
            if (inStr.Equals("grey"))   { R = 0.5f; G = 0.5f; B = 0.5f; return true; }
            if (inStr.Equals("clear"))  { R = 1.0f; G = 1.0f; B = 1.0f; return true; }
            if (inStr.Equals("orange")) { R = 1.0f; G = 165.0f/255.0f; B = 0.0f; return true; }
            if (inStr.Equals("brown")) { R = 160.0f / 255.0f; G = 82.0f / 255.0f; B = 45.0f / 255.0f; return true; }
            if (inStr.Equals("gold")) { R = 232.0f / 255.0f; G = 185.0f / 255.0f; B = 55.0f / 255.0f; return true; }
            if (inStr.Equals("purple")) { R = 147.0f / 255.0f; G = 112.0f / 255.0f; B = 219.0f / 255.0f; return true; }
            if (inStr.Equals("violet")) { R = 147.0f / 255.0f; G = 112.0f / 255.0f; B = 219.0f / 255.0f; return true; }
            if (inStr.Equals("yellow")) { R = 1.0f; G = 1.0f; B = 0.0f; return true; }


            return false;
        }


    }
}
