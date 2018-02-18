using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using System.Windows.Media.Animation;
using KG_Lab1.Annotations;

namespace KG_Lab1.ColorModels
{
    public class ColorModel : INotifyPropertyChanged
    {
        private double TOLERANCE = 1e-7;

        private bool _updatable = true;

        private int _rgb_R;
        private int _rgb_G;
        private int _rgb_B;

        public Color RgbColor
        {
            get => Color.FromRgb((byte) _rgb_R, (byte) _rgb_G, (byte) _rgb_B);
            set
            {
                _rgb_R = value.R;
                _rgb_G = value.G;
                _rgb_B = value.B;
                UpdateValues();
            }
        }

        public int Rgb_R
        {
            get => _rgb_R;
            set
            {
                _rgb_R = value > 255 ? 256 : value < 0 ? -1 : value;
                UpdateValues();
            } 
        }

        public int Rgb_G
        {
            get => _rgb_G;
            set
            {
                _rgb_G = value > 255 ? 256 : value < 0 ? -1 : value;
                UpdateValues();
            }
        }

        public int Rgb_B
        {
            get => _rgb_B;
            set
            {
                _rgb_B = value > 255 ? 256 : value < 0 ? -1 : value;
                UpdateValues();
            }
        }

        public double Cmyk_C
        {
            get => Math.Round(Math.Abs(Cmyk_K - 100) < TOLERANCE ? 0
                : (1 - Rgb_R / 255.0 - Cmyk_K / 100.0) / (1 - Cmyk_K / 100.0) * 100, 2);
            set => SetValuesFromCmyk(value, Cmyk_M, Cmyk_Y, Cmyk_K);
        }

        public double Cmyk_M
        {
            get => Math.Round(Math.Abs(Cmyk_K - 100) < TOLERANCE ? 0 
                : (1 - Rgb_G / 255.0 - Cmyk_K / 100.0) / (1 - Cmyk_K / 100.0) * 100, 2);
            set => SetValuesFromCmyk(Cmyk_C, value, Cmyk_Y, Cmyk_K);
        }

        public double Cmyk_Y
        {
            get => Math.Round(Math.Abs(Cmyk_K - 100) < TOLERANCE ? 0 
                : (1 - Rgb_B / 255.0 - Cmyk_K / 100.0) / (1 - Cmyk_K / 100.0) * 100, 2);
            set => SetValuesFromCmyk(Cmyk_C, Cmyk_M, value, Cmyk_K);
        }

        public double Cmyk_K
        {
            get => Math.Round((1 - Math.Max(Math.Max(Rgb_R / 255.0, Rgb_G / 255.0), Rgb_B / 255.0)) * 100, 2);
            set => SetValuesFromCmyk(Cmyk_C, Cmyk_M, Cmyk_Y, value);
        }

        private double Hsl_Cmax => Math.Max(Math.Max(Rgb_R / 255.0, Rgb_G / 255.0), Rgb_B / 255.0);

        private double Hsl_Cmin => Math.Min(Math.Min(Rgb_R / 255.0, Rgb_G / 255.0), Rgb_B / 255.0);

        private double Hsl_Delta => Hsl_Cmax - Hsl_Cmin;

        public int Hsl_H
        {
            get
            {
                double r = Rgb_R / 255.0;
                double g = Rgb_G / 255.0;
                double b = Rgb_B / 255.0;
                if (Math.Abs(Hsl_Delta) < TOLERANCE) return 0;
                if (Hsl_Cmax == r)
                {
                    double result = 60 * (g - b) / Hsl_Delta;
                    if (g < b)
                        result += 360;
                    return (int) Math.Round(result, 0);
                }
                if (Hsl_Cmax == g)
                    return (int)Math.Round(60 * (b - r) / Hsl_Delta, 0) + 120;
                return (int)Math.Round(60 * (r - g) / Hsl_Delta, 0) + 240;
            }
            set => SetValuesFromHsl(value, Hsl_S, Hsl_L);
        }

        public double Hsl_S
        {
            get => Math.Abs(Hsl_Delta) < TOLERANCE ? 0 : Math.Round(Hsl_Delta / (1 - Math.Abs(0.02 * Hsl_L - 1)) * 100.0, 1);
            set => SetValuesFromHsl(Hsl_H, value, Hsl_L);
        }

        public double Hsl_L
        {
            get => Math.Round((Hsl_Cmax + Hsl_Cmin) * 50, 1);
            set => SetValuesFromHsl(Hsl_H, Hsl_S, value);
        }

        public double Lab_L
        {
            get => Math.Round(116 * XyzToLabF(XyzValues.y / XYZ_YN) - 16, 4);
            set => SetValuesFromLab(value, Lab_A, Lab_B);
        }

        public double Lab_A
        {
            get => Math.Round(500 * (XyzToLabF(XyzValues.x / XYZ_XN) - XyzToLabF(XyzValues.y / XYZ_YN)), 4);
            set => SetValuesFromLab(Lab_L, value, Lab_B);
        }

        public double Lab_B
        {
            get => Math.Round(200 * (XyzToLabF(XyzValues.y / XYZ_YN) - XyzToLabF(XyzValues.z / XYZ_ZN)), 4);
            set => SetValuesFromLab(Lab_L, Lab_A, value);
        }

        private const double XYZ_XN = 95.047;
        private const double XYZ_YN = 100.0;
        private const double XYZ_ZN = 108.883;
        private const double XYZ_DELTA = 6.0 / 29.0;

        private double XyzToLabF(double t)
        {
            if (t > XYZ_DELTA * XYZ_DELTA * XYZ_DELTA)
                return Math.Pow(t, 1.0 / 3.0);
            return t / 3 / XYZ_DELTA / XYZ_DELTA + 4.0 / 29.0;
        }

        private double XyzToLabFReverse(double t)
        {
            if (t > XYZ_DELTA)
                return Math.Pow(t, 3.0);
            return 3 * XYZ_DELTA / XYZ_DELTA * (t - 4.0 / 29.0);
        }

        private class Xyz
        {
            public double x;
            public double y;
            public double z;
        }

        private Xyz XyzValues => GetXyzFromRgb();

        private Xyz GetXyzFromRgb()
        {
            Xyz xyz = new Xyz();
            double r = Rgb_R / 255.0;
            double g = Rgb_G / 255.0;
            double b = Rgb_B / 255.0;
            if (r > 0.04045)
                r = Math.Pow((r + 0.055) / 1.055, 2.4);
            else r = r / 12.92;
            if (g > 0.04045)
                g = Math.Pow((g + 0.055) / 1.055, 2.4);
            else
                g = g / 12.92;
            if (b > 0.04045)
                b = Math.Pow((b + 0.055) / 1.055, 2.4);
            else
                b = b / 12.92;
            r = r * 100;
            g = g * 100;
            b = b * 100;
            xyz.x = r * 0.4124 + g * 0.3576 + b * 0.1805;
            xyz.y = r * 0.2126 + g * 0.7152 + b * 0.0722;
            xyz.z = r * 0.0193 + g * 0.1192 + b * 0.9505;
            return xyz;
        }

        private Xyz GetXyzFromLab(double l, double a, double b)
        {
            Xyz xyz = new Xyz();
            xyz.x = XYZ_XN * XyzToLabFReverse((l + 16) / 116 + a / 500);
            xyz.y = XYZ_YN * XyzToLabFReverse((l + 16) / 116);
            xyz.z = XYZ_ZN * XyzToLabFReverse((l + 16) / 116 - b / 200);
            return xyz;
        }

        private void SetValuesFromCmyk(double c, double m, double y, double k)
        {
            _updatable = false;
            Rgb_R = (int)Math.Round(255 * (1 - c / 100.0) * (1 - k / 100.0), 0);
            Rgb_G = (int)Math.Round(255 * (1 - m / 100.0) * (1 - k / 100.0), 0);
            Rgb_B = (int)Math.Round(255 * (1 - y / 100.0) * (1 - k / 100.0), 0);
            _updatable = true;
            UpdateValues();
        }

        private void SetValuesFromHsl(int h, double s, double l)
        {
            double c = (1 - Math.Abs(2 * l / 100.0 - 1)) * s / 100.0;
            double x = c * (1 - Math.Abs(h / 60.0 % 2.0 - 1.0));
            double m = l / 100.0 - c / 2.0;
            double r, g, b;
            if (h >= 0 && h < 60)
            {
                r = c;
                g = x;
                b = 0;
            }
            else if (h < 120)
            {
                r = x;
                g = c;
                b = 0;
            }
            else if (h < 180)
            {
                r = 0;
                g = c;
                b = x;
            }
            else if (h < 240)
            {
                r = 0;
                g = x;
                b = c;
            }
            else if (h < 300)
            {
                r = x;
                g = 0;
                b = c;
            }
            else
            {
                r = c;
                g = 0;
                b = x;
            }
            _updatable = false;
            Rgb_R = (int) Math.Round((r + m) * 255, 0);
            Rgb_G = (int)Math.Round((g + m) * 255, 0);
            Rgb_B = (int)Math.Round((b + m) * 255, 0);
            _updatable = true;
            UpdateValues();
        }

        private void SetValuesFromLab(double l, double a, double b)
        {
            _updatable = false;
            Xyz xyz = GetXyzFromLab(l, a, b);
            double x = xyz.x / 100;
            double y = xyz.y / 100;
            double z = xyz.z / 100;
            double rgbR = x * 3.2406 + y * -1.5372 + z * -0.4986;
            double rgbG = x * -0.9689 + y * 1.8758 + z * 0.0415;
            double rgbB = x * 0.0557 + y * -0.2040 + z * 1.0570;
            if (rgbR > 0.0031308)
                rgbR = 1.055 * Math.Pow(rgbR, 1 / 2.4) - 0.055;
            else
                rgbR = 12.92 * rgbR;
            if (rgbG > 0.0031308)
                rgbG = 1.055 * Math.Pow(rgbG, 1 / 2.4) - 0.055;
            else
                rgbG = 12.92 * rgbG;
            if (rgbB > 0.0031308)
                rgbB = 1.055 * Math.Pow(rgbB, 1 / 2.4) - 0.055;
            else
                rgbB = 12.92 * rgbB;
            Rgb_R = (int)Math.Round(rgbR * 255, 0);
            Rgb_G = (int)Math.Round(rgbG * 255, 0);
            Rgb_B = (int)Math.Round(rgbB * 255, 0);
            _updatable = true;
            UpdateValues();
        }

        public void UpdateValues()
        {
            if (_updatable)
            {
                OnPropertyChanged(nameof(Rgb_R));
                OnPropertyChanged(nameof(Rgb_G));
                OnPropertyChanged(nameof(Rgb_B));
                OnPropertyChanged(nameof(Cmyk_C));
                OnPropertyChanged(nameof(Cmyk_M));
                OnPropertyChanged(nameof(Cmyk_Y));
                OnPropertyChanged(nameof(Cmyk_K));
                OnPropertyChanged(nameof(Hsl_H));
                OnPropertyChanged(nameof(Hsl_L));
                OnPropertyChanged(nameof(Hsl_S));
                OnPropertyChanged(nameof(Lab_L));
                OnPropertyChanged(nameof(Lab_A));
                OnPropertyChanged(nameof(Lab_B));
                OnPropertyChanged(nameof(RgbColor));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
