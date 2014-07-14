using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AccidentalNoisePrototype;
using AccidentalNoise;

namespace AccidentalNoisePrototype
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            Bitmap bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);


            Gradient ground_gradient = new Gradient(0, 0, 0, 1);

            #region lowlands
            Fractal lowland_shape_fractal = new Fractal(FractalType.BILLOW, BasisTypes.GRADIENT, InterpTypes.QUINTIC, 2, 0.25, null);
            AutoCorrect lowland_autocorrect = new AutoCorrect(lowland_shape_fractal, 0, 1);
            ScaleOffset lowland_scale = new ScaleOffset(0.125, -0.45, lowland_autocorrect);
            ScaleDomain lowland_y_scale = new ScaleDomain(lowland_scale, null, 0);
            TranslatedDomain lowland_terrain = new TranslatedDomain(ground_gradient, null, lowland_y_scale);
            #endregion

            #region highlands
            Fractal highland_shape_fractal = new Fractal(FractalType.FBM, BasisTypes.GRADIENT, InterpTypes.QUINTIC, 4, 2, null);
            AutoCorrect highland_autocorrect = new AutoCorrect(highland_shape_fractal, -1, 1);
            ScaleOffset highland_scale = new ScaleOffset(0.25, 0, highland_autocorrect);
            ScaleDomain highland_y_scale = new ScaleDomain(highland_scale, null, 0);
            TranslatedDomain highland_terrain = new TranslatedDomain(ground_gradient, null, highland_y_scale);
            #endregion

            #region mountains
            Fractal mountain_shape_fractal = new Fractal(FractalType.RIDGEDMULTI, BasisTypes.GRADIENT, InterpTypes.QUINTIC, 8, 1, null);
            AutoCorrect mountain_autocorrect = new AutoCorrect(mountain_shape_fractal, -1, 1);
            ScaleOffset mountain_scale = new ScaleOffset(0.3, 0.15, mountain_autocorrect);
            ScaleDomain mountain_y_scale = new ScaleDomain(mountain_scale, null, 0.15);
            TranslatedDomain mountain_terrain = new TranslatedDomain(ground_gradient, null, mountain_y_scale);
            #endregion

            #region terrain
            Fractal terrain_type_fractal = new Fractal(FractalType.FBM, BasisTypes.GRADIENT, InterpTypes.QUINTIC, 3, 0.125, null);
            AutoCorrect terrain_autocorrect = new AutoCorrect(terrain_type_fractal, 0, 1);
            ScaleDomain terrain_type_y_scale = new ScaleDomain(terrain_autocorrect, null, 0);
            Cache terrain_type_cache = new Cache(terrain_type_y_scale);
            Select highland_mountain_select = new Select(terrain_type_cache, highland_terrain, mountain_terrain, 0.55, 0.2);
            Select highland_lowland_select = new Select(terrain_type_cache, lowland_terrain, highland_mountain_select, 0.25, 0.15);
            Cache highland_lowland_select_cache = new Cache(highland_lowland_select);
            Select ground_select = new Select(highland_lowland_select_cache, 0, 1, 0.5, null);
            #endregion

            #region caves
            Fractal cave_shape = new Fractal(FractalType.RIDGEDMULTI, BasisTypes.GRADIENT, InterpTypes.QUINTIC, 1, 4, null);
            Bias cave_attenuate_bias = new Bias(highland_lowland_select_cache, 0.65);
            Combiner cave_shape_attenuate = new Combiner(CombinerTypes.MULT, cave_shape, cave_attenuate_bias);
            Fractal cave_perturb_fractal = new Fractal(FractalType.FBM, BasisTypes.GRADIENT, InterpTypes.QUINTIC, 6, 3, null);
            ScaleOffset cave_perturb_scale = new ScaleOffset(0.5, 0, cave_perturb_fractal);
            TranslatedDomain cave_perturb = new TranslatedDomain(cave_shape_attenuate, cave_perturb_scale, null);
            Select cave_select = new Select(cave_perturb, 1, 0, 0.75, 0);
            #endregion

            Combiner gound_cave_multiply = new Combiner(CombinerTypes.MULT, cave_select, ground_select);

            //EXAMPLE 1
            //Gradient ground_gradient = new Gradient(0, 0, 0, 1);


            //Fractal ground_shape_fractal = new Fractal(FractalType.FBM,
            //                                           BasisTypes.GRADIENT,
            //                                           InterpTypes.QUINTIC,
            //                                           6, 2, null);

            //ScaleOffset ground_scale = new Accidental.ScaleOffset(0.5, 0, ground_shape_fractal);
            //ScaleDomain ground_scale_y = new ScaleDomain(null, 0, ground_scale);
            //TranslatedDomain ground_perturb = new TranslatedDomain(ground_gradient, null, ground_scale_y);


            //Fractal ground_overhang_fractal = new Fractal(FractalType.FBM,
            //                                              BasisTypes.GRADIENT,
            //                                              InterpTypes.QUINTIC,
            //                                              6, 2, 23434);
            //ScaleOffset ground_overhang_scale = new ScaleOffset(0.2, 0, ground_overhang_fractal);
            //TranslatedDomain ground_overhang_perturb = new TranslatedDomain(ground_perturb, ground_overhang_scale, null);

            //Select ground_select = new Select(ground_overhang_perturb, 0, 1, 0.5, null);


            //EXAMPLE 2
            //Fractal cave_shape = new Accidental.Fractal(FractalType.RIDGEDMULTI, BasisTypes.GRADIENT, InterpTypes.QUINTIC, 1, 2, 4533);
            //Select cave_select = new Accidental.Select(cave_shape, 1, 0, 0.6, 0);

            //Fractal cave_perturb_fractal = new Fractal(FractalType.FBM, BasisTypes.GRADIENT, InterpTypes.QUINTIC, 6, 3, null);
            //ScaleOffset cave_perturb_scale = new ScaleOffset(0.25, 0, cave_perturb_fractal);
            //TranslatedDomain cave_perturb = new TranslatedDomain(cave_select, cave_perturb_scale, null);


            SMappingRanges ranges = new SMappingRanges();

            //finally update our image
            for (int x = 0; x < bitmap.Width; x++)
            {
                for(int y = 0; y < bitmap.Height; y++)
                {
                    double p = (double)x / (double)bitmap.Width;
                    double q = (double)y / (double)bitmap.Height;
                    double nx, ny = 0.0;

                    nx = ranges.mapx0 + p * (ranges.mapx1 - ranges.mapx0);
                    ny = ranges.mapy0 + q * (ranges.mapy1 - ranges.mapy0);

                    double val = gound_cave_multiply.Get(nx * 6, ny * 3);

                    bitmap.SetPixel(x, y, Color.Black.Lerp(Color.White, val));
                }
            }

            pictureBox1.Image = bitmap;
        }
    }

    public class SMappingRanges
    {
        public double mapx0,mapy0,mapz0, mapx1,mapy1,mapz1;
        public double loopx0,loopy0,loopz0, loopx1,loopy1,loopz1;

        public SMappingRanges()
        {
            mapx0=mapy0=mapz0=loopx0=loopy0=loopz0=-1;
            mapx1=mapy1=mapz1=loopx1=loopy1=loopz1=1;
        }
    };

    public static class ExtensionMethods
    {
        public static double Lerp(this double start, double end, double amount)
        {
            double difference = end - start;
            double adjusted = difference * amount;
            return start + adjusted;
        }

        public static Color Lerp(this Color colour, Color to, double amount)
        {
            // start colours as lerp-able floats
            double sr = colour.R, sg = colour.G, sb = colour.B;

            // end colours as lerp-able floats
            double er = to.R, eg = to.G, eb = to.B;

            // lerp the colours to get the difference
            byte r = (byte)sr.Lerp(er, amount),
                 g = (byte)sg.Lerp(eg, amount),
                 b = (byte)sb.Lerp(eb, amount);

            // return the new colour
            return Color.FromArgb(r, g, b);
        }
    }
}
