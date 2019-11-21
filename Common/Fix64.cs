using System;

namespace Common
{
    [Serializable]
    public struct Fix64 : IEquatable<Fix64>, IComparable<Fix64>
    {
        #region Fields

        public long rawValue;

        #endregion

        #region Static Fields

        public static readonly decimal Precision = (decimal)(new Fix64(1L));
        public static readonly Fix64 One = new Fix64(ONE);
        public static readonly Fix64 Zero = new Fix64();
        public static readonly Fix64 PI = new Fix64(Pi);
        public static readonly Fix64 PITimes2 = new Fix64(PiTimes2);
        public static readonly Fix64 PIOver180 = new Fix64((long)72);
        public static readonly Fix64 Rad2Deg = (Fix64)360 / (Fix64.Pi * (Fix64)2);
        public static readonly Fix64 Deg2Rad = Fix64.Pi * (Fix64)2 / (Fix64)360;
        public static readonly Fix64 I360 = new Fix64(360);
        public static readonly Fix64 I180 = new Fix64(180);
        public static readonly Fix64 IM180 = new Fix64(-180);

        private const int FRACTIONAL_PLACES = 12;
        private const long ONE = 1L << FRACTIONAL_PLACES;
        private const long Pi = 12868;
        private const long PiTimes2 = 25736;

        #endregion

        #region Constructors

        public Fix64(long rawValue)
        {
            this.rawValue = rawValue;
        }

        public Fix64(int rawValue)
        {
            this.rawValue = rawValue;
        }

        #endregion

        #region Option Explicit Statement

        public static explicit operator Fix64(long value)
        {
            return new Fix64(value * ONE);
        }

        public static explicit operator long(Fix64 value)
        {
            return value.rawValue >> FRACTIONAL_PLACES;
        }

        public static explicit operator Fix64(float value)
        {
            return new Fix64((long)(value * ONE));
        }

        public static explicit operator float(Fix64 value)
        {
            return (float)value.rawValue / ONE;
        }

        public static explicit operator int(Fix64 value)
        {
            return (int)((float)value);
        }

        public static explicit operator Fix64(double value)
        {
            return new Fix64((long)(value * ONE));
        }

        public static explicit operator double(Fix64 value)
        {
            return (double)value.rawValue / ONE;
        }

        public static explicit operator Fix64(decimal value)
        {
            return new Fix64((long)(value * ONE));
        }

        public static explicit operator decimal(Fix64 value)
        {
            return (decimal)value.rawValue / ONE;
        }

        #endregion

        #region Operator Overloading

        public static Fix64 operator +(Fix64 x, Fix64 y)
        {
            return new Fix64(x.rawValue + y.rawValue);
        }

        public static Fix64 operator +(Fix64 x, int y)
        {
            return x + (Fix64)y;
        }

        public static Fix64 operator +(int x, Fix64 y)
        {
            return (Fix64)x + y;
        }

        public static Fix64 operator +(Fix64 x, float y)
        {
            return x + (Fix64)y;
        }

        public static Fix64 operator +(float x, Fix64 y)
        {
            return (Fix64)x + y;
        }

        public static Fix64 operator +(Fix64 x, double y)
        {
            return x + (Fix64)y;
        }

        public static Fix64 operator +(double x, Fix64 y)
        {
            return (Fix64)x + y;
        }

        public static Fix64 operator -(Fix64 x, Fix64 y)
        {
            return new Fix64(x.rawValue - y.rawValue);
        }

        public static Fix64 operator -(Fix64 x, int y)
        {
            return x - (Fix64)y;
        }

        public static Fix64 operator -(int x, Fix64 y)
        {
            return (Fix64)x - y;
        }

        public static Fix64 operator -(Fix64 x, float y)
        {
            return x - (Fix64)y;
        }

        public static Fix64 operator -(float x, Fix64 y)
        {
            return (Fix64)x + y;
        }

        public static Fix64 operator -(Fix64 x, double y)
        {
            return x - (Fix64)y;
        }

        public static Fix64 operator -(double x, Fix64 y)
        {
            return (Fix64)x - y;
        }

        public static Fix64 operator *(Fix64 x, Fix64 y)
        {
            return new Fix64((x.rawValue * y.rawValue) >> FRACTIONAL_PLACES);
        }

        public static Fix64 operator *(Fix64 x, int y)
        {
            return x * (Fix64)y;
        }

        public static Fix64 operator *(int x, Fix64 y)
        {
            return (Fix64)x * y;
        }

        public static Fix64 operator *(Fix64 x, float y)
        {
            return x * (Fix64)y;
        }

        public static Fix64 operator *(float x, Fix64 y)
        {
            return (Fix64)x * y;
        }

        public static Fix64 operator *(Fix64 x, double y)
        {
            return x * (Fix64)y;
        }

        public static Fix64 operator *(double x, Fix64 y)
        {
            return (Fix64)x * y;
        }

        public static Fix64 operator /(Fix64 x, Fix64 y)
        {
            return new Fix64((x.rawValue << FRACTIONAL_PLACES) / y.rawValue);
        }

        public static Fix64 operator /(Fix64 x, int y)
        {
            return x / (Fix64)y;
        }

        public static Fix64 operator /(int x, Fix64 y)
        {
            return (Fix64)x / y;
        }

        public static Fix64 operator /(Fix64 x, float y)
        {
            return x / (Fix64)y;
        }

        public static Fix64 operator /(float x, Fix64 y)
        {
            return (Fix64)x / y;
        }

        public static Fix64 operator /(double x, Fix64 y)
        {
            return (Fix64)x / y;
        }

        public static Fix64 operator /(Fix64 x, double y)
        {
            return x / (Fix64)y;
        }

        public static Fix64 operator %(Fix64 x, Fix64 y)
        {
            return new Fix64(x.rawValue % y.rawValue);
        }

        public static Fix64 operator -(Fix64 x)
        {
            return new Fix64(-x.rawValue);
        }

        public static bool operator ==(Fix64 x, Fix64 y)
        {
            return x.rawValue == y.rawValue;
        }

        public static bool operator !=(Fix64 x, Fix64 y)
        {
            return x.rawValue != y.rawValue;
        }

        public static bool operator >(Fix64 x, Fix64 y)
        {
            return x.rawValue > y.rawValue;
        }

        public static bool operator >(Fix64 x, int y)
        {
            return x > (Fix64)y;
        }

        public static bool operator <(Fix64 x, int y)
        {
            return x < (Fix64)y;
        }

        public static bool operator >(Fix64 x, float y)
        {
            return x > (Fix64)y;
        }

        public static bool operator <(Fix64 x, float y)
        {
            return x < (Fix64)y;
        }

        public static bool operator <(Fix64 x, Fix64 y)
        {
            return x.rawValue < y.rawValue;
        }

        public static bool operator >=(Fix64 x, Fix64 y)
        {
            return x.rawValue >= y.rawValue;
        }

        public static bool operator <=(Fix64 x, Fix64 y)
        {
            return x.rawValue <= y.rawValue;
        }

        public static Fix64 operator >>(Fix64 x, int amount)
        {
            return new Fix64(x.rawValue >> amount);
        }

        public static Fix64 operator <<(Fix64 x, int amount)
        {
            return new Fix64(x.rawValue << amount);
        }

        #endregion

        #region Methods

        public override bool Equals(object obj)
        {
            return obj is Fix64 && ((Fix64)obj).rawValue == rawValue;
        }

        public override int GetHashCode()
        {
            return rawValue.GetHashCode();
        }

        public bool Equals(Fix64 other)
        {
            return rawValue == other.rawValue;
        }

        public int CompareTo(Fix64 other)
        {
            return rawValue.CompareTo(other.rawValue);
        }

        public override string ToString()
        {
            return ((decimal)this).ToString();
        }

        public string ToStringRound(int round = 2)
        {
            return (float)System.Math.Round((float)this, round) + "";
        }

        #endregion

        #region Static Methods

        public static int Sign(Fix64 value)
        {
            return
                value.rawValue < 0 ? -1 :
                value.rawValue > 0 ? 1 :
                0;
        }

        public static Fix64 Abs(Fix64 value)
        {
            return new Fix64(value.rawValue > 0 ? value.rawValue : -value.rawValue);
        }

        public static Fix64 Floor(Fix64 value)
        {
            return new Fix64((long)((ulong)value.rawValue & 0xFFFFFFFFFFFFF000));
        }

        public static Fix64 Ceiling(Fix64 value)
        {
            var hasFractionalPart = (value.rawValue & 0x0000000000000FFF) != 0;
            return hasFractionalPart ? Floor(value) + One : value;
        }

        public static Fix64 FromRaw(long rawValue)
        {
            return new Fix64(rawValue);
        }

        public static Fix64 Pow(Fix64 x, int y)
        {
            if (y == 1) return x;
            Fix64 result = Fix64.Zero;
            Fix64 tmp = Pow(x, y / 2);
            if ((y & 1) != 0)
            {
                result = x * tmp * tmp;
            }
            else
            {
                result = tmp * tmp;
            }

            return result;
        }

        public static Fix64 Sqrt(Fix64 f, int numberIterations)
        {
            if (f.rawValue < 0)
            {
                throw new ArithmeticException("sqrt error");
            }

            if (f.rawValue == 0)
                return Fix64.Zero;

            Fix64 k = f + Fix64.One >> 1;
            for (int i = 0; i < numberIterations; i++)
                k = (k + (f / k)) >> 1;

            if (k.rawValue < 0)
                throw new ArithmeticException("Overflow");
            else
                return k;
        }

        public static Fix64 Sqrt(Fix64 f)
        {
            byte numberOfIterations = 8;
            if (f.rawValue > 0x64000)
                numberOfIterations = 12;
            if (f.rawValue > 0x3e8000)
                numberOfIterations = 16;
            return Sqrt(f, numberOfIterations);
        }

        public static Fix64 Min(Fix64 a, Fix64 b)
        {
            if (a < b)
                return a;
            else
                return b;
        }

        #endregion

        #region Sin, Cos, Tan, Asin, ATan, ATan2

        public static Fix64 Sin(Fix64 i)
        {
            Fix64 j = (Fix64)0;
            for (; i < Fix64.Zero; i += Fix64.FromRaw(25736)) ;
            if (i > Fix64.FromRaw(25736))
                i %= Fix64.FromRaw(25736);
            Fix64 k = (i * Fix64.FromRaw(10)) / Fix64.FromRaw(714);
            if (i != Fix64.Zero && i != Fix64.FromRaw(6434) && i != Fix64.FromRaw(12868) &&
                i != Fix64.FromRaw(19302) && i != Fix64.FromRaw(25736))
                j = (i * Fix64.FromRaw(100)) / Fix64.FromRaw(714) - k * Fix64.FromRaw(10);
            if (k <= Fix64.FromRaw(90))
                return SinLookup(k, j);
            if (k <= Fix64.FromRaw(180))
                return SinLookup(Fix64.FromRaw(180) - k, j);
            if (k <= Fix64.FromRaw(270))
                return -SinLookup(k - Fix64.FromRaw(180), j);
            else
                return -SinLookup(Fix64.FromRaw(360) - k, j);
        }

        private static Fix64 SinLookup(Fix64 i, Fix64 j)
        {
            if (j > 0 && j < Fix64.FromRaw(10) && i < Fix64.FromRaw(90))
                return Fix64.FromRaw(SIN_TABLE[i.rawValue]) +
                    ((Fix64.FromRaw(SIN_TABLE[i.rawValue + 1]) - Fix64.FromRaw(SIN_TABLE[i.rawValue])) /
                    Fix64.FromRaw(10)) * j;
            else
                return Fix64.FromRaw(SIN_TABLE[i.rawValue]);
        }

        private static int[] SIN_TABLE =
        {
            0, 71, 142, 214, 285, 357, 428, 499, 570, 641,
            711, 781, 851, 921, 990, 1060, 1128, 1197, 1265, 1333,
            1400, 1468, 1534, 1600, 1665, 1730, 1795, 1859, 1922, 1985,
            2048, 2109, 2170, 2230, 2290, 2349, 2407, 2464, 2521, 2577,
            2632, 2686, 2740, 2793, 2845, 2896, 2946, 2995, 3043, 3091,
            3137, 3183, 3227, 3271, 3313, 3355, 3395, 3434, 3473, 3510,
            3547, 3582, 3616, 3649, 3681, 3712, 3741, 3770, 3797, 3823,
            3849, 3872, 3895, 3917, 3937, 3956, 3974, 3991, 4006, 4020,
            4033, 4045, 4056, 4065, 4073, 4080, 4086, 4090, 4093, 4095,
            4096
        };

        public static Fix64 Cos(Fix64 i)
        {
            return Sin(i + Fix64.FromRaw(6435));
        }

        public static Fix64 Tan(Fix64 i)
        {
            return Sin(i) / Cos(i);
        }

        public static Fix64 Asin(Fix64 F)
        {
            bool isNegative = F < 0;
            F = Abs(F);

            if (F > Fix64.One)
                throw new ArithmeticException("Bad Asin Input:" + (double)F);

            Fix64 f1 = ((((Fix64.FromRaw(145103 >> FRACTIONAL_PLACES) * F) -
                Fix64.FromRaw(599880 >> FRACTIONAL_PLACES) * F) +
                Fix64.FromRaw(1420468 >> FRACTIONAL_PLACES) * F) -
                Fix64.FromRaw(3592413 >> FRACTIONAL_PLACES) * F) +
                Fix64.FromRaw(26353447 >> FRACTIONAL_PLACES);
            Fix64 f2 = PI / (Fix64)2 - (Sqrt(Fix64.One - F) * f1);

            return isNegative ? -f2 : f2;
        }

        public static Fix64 Atan(Fix64 F)
        {
            return Asin(F / Sqrt(Fix64.One + (F * F)));
        }

        public static Fix64 Atan2(Fix64 F1, Fix64 F2)
        {
            if (F2.rawValue == 0 && F1.rawValue == 0)
                return (Fix64)0;

            Fix64 result = (Fix64)0;
            if (F2 > 0)
                result = Atan(F1 / F2);
            else if (F2 < 0)
            {
                if (F1 >= (Fix64)0)
                    result = (PI - Atan(Abs(F1 / F2)));
                else
                    result = -(PI - Atan(Abs(F1 / F2)));
            }
            else
                result = (F1 >= (Fix64)0 ? PI : -PI) / (Fix64)2;

            return result;
        }

        #endregion
    }
}
