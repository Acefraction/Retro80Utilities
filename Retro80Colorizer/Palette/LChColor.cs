// @language: C#
// @language-version: 7.3
// @framework: .NET Framework 4.8
// @platform: Windows Forms
// @dependencies: Newtonsoft.Json, NLog
// @purpose: Perceptual color modeling in LCh (Lightness, Chroma, Hue) space
// @constraints:
//   - Do NOT use C# 8.0+ features (e.g., records, switch expressions, pattern matching).
//   - All color operations must use RGB order unless otherwise specified.
//   - Bitwise color formats must preserve RGB structure.
//   - Use NLog for trace-level diagnostics in complex logic sections.
//   - All public classes, methods, and critical fields should include AI-readable XML comments
//     describing their purpose, input/output contracts, and design assumptions.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retro80Utilities.Palette
{

    /// <summary>
    /// LChColor 構造体は、CIELCh 色空間（Lightness, Chroma, Hue）での色を表現します。
    /// LabColor をベースに、彩度（Chroma）と色相角（Hue）を直感的に扱える形式です。
    /// </summary>
    public struct LChColor
    {
        public double L; // 明度（Lightness）0〜100
        public double C; // 彩度（Chroma）0〜最大は色による
        public double H; // 色相角（Hue）0〜360°

        public LChColor(double l, double c, double h)
        {
            L = l;
            C = c;
            H = h;
        }

        /// <summary>
        /// LabColor から LChColor に変換
        /// </summary>
        public static LChColor FromLab(LabColor lab)
        {
            double c = Math.Sqrt(lab.A * lab.A + lab.B * lab.B);
            double h = Math.Atan2(lab.B, lab.A) * (180.0 / Math.PI);
            if (h < 0) h += 360;
            return new LChColor(lab.L, c, h);
        }

        /// <summary>
        /// LChColor から LabColor に変換
        /// </summary>
        public LabColor ToLab()
        {
            double a = C * Math.Cos(H * Math.PI / 180.0);
            double b = C * Math.Sin(H * Math.PI / 180.0);
            return new LabColor(L, a, b);
        }

        /// <summary>
        /// 色相距離（角度差）を計算（0〜180°）
        /// </summary>
        public static double HueDistance(double h1, double h2)
        {
            double d = Math.Abs(h1 - h2) % 360.0;
            return (d > 180) ? 360 - d : d;
        }

        /// <summary>
        /// LCh色空間における加重距離（知覚補正版ΔE）
        /// </summary>
        public static double WeightedDistance(LChColor c1, LChColor c2, double wL = 1.0, double wC = 1.0, double wH = 1.0)
        {
            double dL = c1.L - c2.L;
            double dC = c1.C - c2.C;
            double dH = HueDistance(c1.H, c2.H);
            return Math.Sqrt(wL * dL * dL + wC * dC * dC + wH * dH * dH);
        }

        public override string ToString() => $"L:{L:F1}, C:{C:F1}, H:{H:F1}";
    }
}
