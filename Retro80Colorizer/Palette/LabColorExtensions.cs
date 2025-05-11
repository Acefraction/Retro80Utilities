using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV.Structure;
using ZedGraph;

namespace Retro80Utilities.Palette
{

    public static class LabColorExtensions
    {
        /// <summary>
        /// LabColor構造体をSystem.Drawing.Color(RGB)に変換します。
        /// </summary>
        public static Color ToColor(this LabColor lab)
        {
            // Lab → XYZ
            double y = (lab.L + 16.0) / 116.0;
            double x = lab.A / 500.0 + y;
            double z = y - lab.B / 200.0;

            double X = 95.047 * LabToXyzHelper(x);
            double Y = 100.000 * LabToXyzHelper(y);
            double Z = 108.883 * LabToXyzHelper(z);

            X /= 100.0; Y /= 100.0; Z /= 100.0;

            double r = X * 3.2406 + Y * -1.5372 + Z * -0.4986;
            double g = X * -0.9689 + Y * 1.8758 + Z * 0.0415;
            double b = X * 0.0557 + Y * -0.2040 + Z * 1.0570;

            r = GammaCorrect(r);
            g = GammaCorrect(g);
            b = GammaCorrect(b);

            return Color.FromArgb(
                ClampToByte(r * 255.0),
                ClampToByte(g * 255.0),
                ClampToByte(b * 255.0)
            );
        }

        private static double LabToXyzHelper(double c)
        {
            double c3 = c * c * c;
            return c3 > 0.008856 ? c3 : (c - 16.0 / 116.0) / 7.787;
        }

        private static double GammaCorrect(double c)
        {
            return c <= 0.0031308 ? 12.92 * c : 1.055 * Math.Pow(c, 1 / 2.4) - 0.055;
        }

        private static int ClampToByte(double val)
        {
            return Math.Min(255, Math.Max(0, (int)Math.Round(val)));
        }
    }
}