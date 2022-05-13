using System;
using System.Collections.Generic;
using System.Text;

namespace WPF_DeckHouseLoad
{
    public enum Panel
    {
        UnprotectedFront,
        ProtectedFront,
        Sides,
        AftEnd
    }
    public enum Tier
    {
        LowestTier,
        SecondTier,
        ThirdTierAndAbove,
        ForthTierAndAbove,
    }
    public enum Location
    {
        AbaftMidships,
        ForwardOfAmidships
    }
    public class ScantlingsCalc : Observable
    {
        public string Description { get; set; }
        public static double L { get; set; } = 283.143;
        public static double B { get; set; } = 45;
        public static double T { get; set; } = 18.15;
        public static double Δ { get; set; } = 207176;
        public static double C
        {
            get
            {
                if (90 <= L && L < 300)
                {
                    return 10.75 - Math.Pow((300 - L) / 100, 1.5);
                }
                if (300 <= L && L < 350)
                {
                    return 10.75;
                }
                return -1;
            }
        }
        public static double Cb
        {
            get
            {
                return Δ / (L * B * T * 1.025);
            }
        }
        private double L2
        {
            get
            {
                if (L >= 300) return 300;
                return L;
            }
        }
        public Tier Tier { get; set; }
        public Panel Panel { get; set; }
        public Location Location { get; set; }
        public bool IsExposedMachineryCasing { get; set; }
        public double b1 { get; set; }
        public double B1 { get; set; }
        public double l { get; set; }
        public double s { get; set; }
        public double k { get; set; } = 1;
        public double x { get; set; }
        public double y { get; set; }
        public double z { get; set; }
        public double cF
        {
            get
            {
                double a = x / L;
                if (a <= 0.1) a = 0.1;

                if (0 < (x / L) && (x / L) < 0.2)
                {
                    return 1 + (5 / Cb) * (0.2 - a);
                }
                if ((x / L) >= 0.2)
                {
                    return 1;
                }
                return -1;
            }
        }
        public double n
        {
            get
            {
                if (Panel == Panel.UnprotectedFront)
                {
                    if (Tier == Tier.LowestTier) return 20 + (L2 / 12);
                    if (Tier == Tier.SecondTier) return 10 + (L2 / 12);
                    if (Tier == Tier.ThirdTierAndAbove || Tier == Tier.ForthTierAndAbove) return 5 + (L2 / 15);
                }

                if (Panel == Panel.ProtectedFront)
                {
                    return 5 + (L2 / 15);
                }

                if (Panel == Panel.Sides)
                {
                    return 5 + (L2 / 15);
                }

                if (Panel == Panel.AftEnd)
                {
                    if (Location == Location.AbaftMidships) return 7 + (L2 / 100) - 8 * (x / L2);
                    if (Location == Location.ForwardOfAmidships) return 5 + (L2 / 100) - 4 * (x / L2);
                }
                return -1;
            }
        }
        public double c
        {
            get
            {
                if (IsExposedMachineryCasing == true)
                {
                    return 1;
                }
                else
                {
                    double br = b1 / B1;
                    if (br <= 0.25) br = 0.25;
                    return 0.3 + 0.7 * (br);
                }
            }
        }
        public double b
        {
            get
            {
                double cb = Cb;
                if (Cb < 0.6) cb = 0.6;
                if (Cb > 0.8) cb = 0.8;

                if (Location == Location.ForwardOfAmidships && Cb <= 0.8) cb = 0.8;

                if ((x / L) < 0.45)
                {
                    return 1 + Math.Pow((((x / L) - 0.45) / (cb + 0.2)), 2);
                }
                if ((x / L) >= 0.45)
                {
                    return 1 + 1.5 * Math.Pow((((x / L) - 0.45) / (cb + 0.2)), 2);
                }
                return -1;
            }
        }
        public double PSIp
        {
            get
            {
                return 2.1 * C * 1 * cF * (Cb + 0.7) * (20 / (10 + z - T));
            }
        }
        public double PSIs
        {
            get
            {
                return 2.1 * C * 0.75 * cF * (Cb + 0.7) * (20 / (10 + z - T));
            }
        }
        public double PAmin
        {
            get
            {
                if (90 < L)
                {
                    if (Tier == Tier.ForthTierAndAbove)
                    {
                        return 12.5;
                    }
                }

                if (90 < L && L <= 250)
                {
                    if (Tier == Tier.LowestTier && Panel == Panel.UnprotectedFront)
                    {
                        return 25 + (L / 10);
                    }
                    else
                    {
                        return 12.5 + (L / 20);
                    }
                }

                if (L > 250)
                {
                    if (Tier == Tier.LowestTier && Panel == Panel.UnprotectedFront)
                    {
                        return 50;
                    }
                    else
                    {
                        return 25;
                    }
                }

                return -1;
            }
        }
        public double PA
        {
            get
            {
                double pA = n * c * (b * C - (z - T));
                return Math.Max(pA, PAmin);
            }
        }
        public double ma
        {
            get
            {
                return 0.204 * (s / l) * (4 - Math.Pow((s / l), 2));
            }
        }
        public double tSideMin
        {
            get
            {
                return Math.Max(0.8 * Math.Sqrt(k * L), 1.21 * s * Math.Sqrt(k * PSIp) + 1.5);
            }
        }
        public double tSide
        {
            get
            {
                return 1.21 * s * Math.Sqrt(k * PSIp) + 1.5;
            }
        }
        public double wSide
        {
            get
            {
                return 0.55 * k * PSIs * s * l * l;
            }
        }
        public double AshSide
        {
            get
            {
                return 0.05 * (1 - 0.817 * ma) * k * PSIs * s * l;
            }
        }
        public double tEnd
        {
            get
            {
                double tmin;
                tmin = (4 + (L2 / 100) * Math.Sqrt(k));
                if (Tier == Tier.LowestTier) tmin = (5 + (L2 / 100) * Math.Sqrt(k));
                if (tmin < 5) tmin = 5;
                return Math.Max(tmin, 0.9 * s * Math.Sqrt(k * PA) + 1.5);
            }
        }
        public double wEnd
        {
            get
            {
                return 0.35 * k * PA * s * l * l;
            }
        }
    }
}