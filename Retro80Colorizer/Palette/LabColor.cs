using System;
using System.Drawing;

/// <summary>
/// LabColor 構造体は、CIELAB（Lab）色空間での色を表現します。
/// 減色・クラスタリング・色差評価など、人間の視覚に近い色操作が必要な用途向け。
/// </summary>
public struct LabColor
{
    public double L; // 明度（Lightness）：0〜100
    public double A; // 緑〜赤の成分（-128〜+127程度）
    public double B; // 青〜黄の成分（-128〜+127程度）

    public LabColor(double l, double a, double b)
    {
        L = l;
        A = a;
        B = b;
    }

    /// <summary>
    /// sRGBからLabに変換する（Color構造体→LabColor）
    /// </summary>
    public static LabColor FromRgb(Color color)
    {
        // RGB [0,255] → 正規化 [0,1] → 線形RGB（ガンマ補正解除）
        double r = PivotRgb(color.R / 255.0);
        double g = PivotRgb(color.G / 255.0);
        double b = PivotRgb(color.B / 255.0);

        // 線形RGB → XYZ空間（sRGB D65基準）
        double x = r * 0.4124564 + g * 0.3575761 + b * 0.1804375;
        double y = r * 0.2126729 + g * 0.7151522 + b * 0.0721750;
        double z = r * 0.0193339 + g * 0.1191920 + b * 0.9503041;

        // XYZ正規化（白色点：D65） → Lab
        return FromXyz(x / 0.95047, y / 1.00000, z / 1.08883);
    }

    private static LabColor FromXyz(double x, double y, double z)
    {
        // XYZ → Lab変換に必要な中間関数
        double fx = PivotXyz(x);
        double fy = PivotXyz(y);
        double fz = PivotXyz(z);

        // Lab値を計算
        double l = Math.Max(0, 116 * fy - 16);
        double a = 500 * (fx - fy);
        double b = 200 * (fy - fz);

        return new LabColor(l, a, b);
    }

    /// <summary>
    /// LabからRGB（Color構造体）に変換する
    /// </summary>
    public static Color ToRgb(LabColor lab)
    {
        // Lab → XYZ
        double fy = (lab.L + 16) / 116.0;
        double fx = lab.A / 500.0 + fy;
        double fz = fy - lab.B / 200.0;

        double x = (Math.Pow(fx, 3) > 0.008856) ? Math.Pow(fx, 3) : (fx - 16.0 / 116.0) / 7.787;
        double y = (Math.Pow(fy, 3) > 0.008856) ? Math.Pow(fy, 3) : (fy - 16.0 / 116.0) / 7.787;
        double z = (Math.Pow(fz, 3) > 0.008856) ? Math.Pow(fz, 3) : (fz - 16.0 / 116.0) / 7.787;

        // 白色点を戻す
        x *= 0.95047;
        y *= 1.00000;
        z *= 1.08883;

        // XYZ → 線形RGB
        double r = x * 3.2406 + y * -1.5372 + z * -0.4986;
        double g = x * -0.9689 + y * 1.8758 + z * 0.0415;
        double b = x * 0.0557 + y * -0.2040 + z * 1.0570;

        // 線形RGB → sRGB（ガンマ補正して [0,255] に戻す）
        return Color.FromArgb(
            ClampToByte(PivotRgbInverse(r)),
            ClampToByte(PivotRgbInverse(g)),
            ClampToByte(PivotRgbInverse(b))
        );
    }

    /// <summary>
    /// 2つのLab色のユークリッド距離（ΔE）を計算する
    /// </summary>
    public static double DeltaE(LabColor c1, LabColor c2)
    {
        return Math.Sqrt(
            Math.Pow(c1.L - c2.L, 2) +
            Math.Pow(c1.A - c2.A, 2) +
            Math.Pow(c1.B - c2.B, 2)
        );
    }

    // sRGB → 線形RGBの変換（ガンマ補正解除）
    private static double PivotRgb(double n) =>
        (n > 0.04045) ? Math.Pow((n + 0.055) / 1.055, 2.4) : n / 12.92;

    // 線形RGB → sRGB（ガンマ補正）
    private static double PivotRgbInverse(double n) =>
        (n <= 0.0031308) ? n * 12.92 : 1.055 * Math.Pow(n, 1.0 / 2.4) - 0.055;

    // XYZ → Labで使う関数（非線形変換）
    private static double PivotXyz(double n) =>
        (n > 0.008856) ? Math.Pow(n, 1.0 / 3.0) : (7.787 * n) + 16.0 / 116.0;

    // 範囲を0〜255にクランプしてbyteへ
    private static int ClampToByte(double val) =>
        (int)Math.Max(0, Math.Min(255, Math.Round(val * 255)));
}
