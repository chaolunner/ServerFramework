using System;

namespace Common
{
    [Serializable]
    public struct FixVector2
    {
        #region Fields

        public Fix64 x;
        public Fix64 y;

        #endregion

        #region Constructors

        public FixVector2(Fix64 x, Fix64 y)
        {
            this.x = x;
            this.y = y;
        }

        public FixVector2(FixVector2 v)
        {
            this.x = v.x;
            this.y = v.y;
        }

        #endregion

        #region Using indexers

        public Fix64 this[int index]
        {
            get
            {
                if (index == 0)
                    return x;
                else
                    return y;
            }
            set
            {
                if (index == 0)
                    x = value;
                else
                    y = value;
            }
        }

        #endregion

        #region Static Properties

        public static FixVector2 Zero
        {
            get { return new FixVector2(Fix64.Zero, Fix64.Zero); }
        }

        #endregion

        #region Option Explicit Statement

#if UNITY
        public static explicit operator FixVector2(UnityEngine.Vector2 vector2)
        {
            return new FixVector2((Fix64)vector2.x, (Fix64)vector2.y);
        }

        public static explicit operator UnityEngine.Vector2(FixVector2 vector2)
        {
            return new UnityEngine.Vector2((float)vector2.x, (float)vector2.y);
        }
#endif

        #endregion

        #region Operator Overloading

        public static FixVector2 operator +(FixVector2 a, FixVector2 b)
        {
            Fix64 x = a.x + b.x;
            Fix64 y = a.y + b.y;
            return new FixVector2(x, y);
        }

        public static FixVector2 operator -(FixVector2 a, FixVector2 b)
        {
            Fix64 x = a.x - b.x;
            Fix64 y = a.y - b.y;
            return new FixVector2(x, y);
        }

        public static FixVector2 operator *(Fix64 d, FixVector2 a)
        {
            Fix64 x = a.x * d;
            Fix64 y = a.y * d;
            return new FixVector2(x, y);
        }

        public static FixVector2 operator *(FixVector2 a, Fix64 d)
        {
            Fix64 x = a.x * d;
            Fix64 y = a.y * d;
            return new FixVector2(x, y);
        }

        public static FixVector2 operator /(FixVector2 a, Fix64 d)
        {
            Fix64 x = a.x / d;
            Fix64 y = a.y / d;
            return new FixVector2(x, y);
        }

        public static bool operator ==(FixVector2 lhs, FixVector2 rhs)
        {
            return lhs.x == rhs.x && lhs.y == rhs.y;
        }

        public static bool operator !=(FixVector2 lhs, FixVector2 rhs)
        {
            return lhs.x != rhs.x || lhs.y != rhs.y;
        }

        #endregion

        #region Methods

        public void Normalize()
        {
            Fix64 n = x * x + y * y;
            if (n == Fix64.Zero)
                return;

            n = Fix64.Sqrt(n);

            if (n < (Fix64)0.0001)
            {
                return;
            }

            n = 1 / n;
            x *= n;
            y *= n;
        }

        public override string ToString()
        {
            return string.Format("x:{0} y:{1}", x, y);
        }

        public override bool Equals(object obj)
        {
            return obj is FixVector2 && ((FixVector2)obj) == this;
        }

        public override int GetHashCode()
        {
            return this.x.GetHashCode() + this.y.GetHashCode();
        }

        #endregion

        #region Static Methods

        public static Fix64 SqrMagnitude(FixVector2 a)
        {
            return a.x * a.x + a.y * a.y;
        }

        public static Fix64 Distance(FixVector2 a, FixVector2 b)
        {
            return Magnitude(a - b);
        }

        public static Fix64 Magnitude(FixVector2 a)
        {
            return Fix64.Sqrt(FixVector2.SqrMagnitude(a));
        }

        public static FixVector2 Lerp(FixVector2 from, FixVector2 to, Fix64 factor)
        {
            return from * (1 - factor) + to * factor;
        }

        #endregion
    }
}
