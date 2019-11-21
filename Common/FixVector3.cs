using System;

namespace Common
{
    [Serializable]
    public struct FixVector3
    {
        #region Fields

        public Fix64 x;
        public Fix64 y;
        public Fix64 z;

        #endregion

        #region Constructors

        public FixVector3(Fix64 x, Fix64 y, Fix64 z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public FixVector3(FixVector3 v)
        {
            this.x = v.x;
            this.y = v.y;
            this.z = v.z;
        }

        #endregion

        #region Using indexers

        public Fix64 this[int index]
        {
            get
            {
                if (index == 0)
                    return x;
                else if (index == 1)
                    return y;
                else
                    return z;
            }
            set
            {
                if (index == 0)
                    x = value;
                else if (index == 1)
                    y = value;
                else
                    z = value;
            }
        }

        #endregion

        #region Static Properties

        public static FixVector3 Zero
        {
            get { return new FixVector3(Fix64.Zero, Fix64.Zero, Fix64.Zero); }
        }

        #endregion

        #region Option Explicit Statement

#if UNITY
        public static explicit operator FixVector3(UnityEngine.Vector3 vector3)
        {
            return new FixVector3((Fix64)vector3.x, (Fix64)vector3.y, (Fix64)vector3.z);
        }

        public static explicit operator UnityEngine.Vector3(FixVector3 vector3)
        {
            return new UnityEngine.Vector3((float)vector3.x, (float)vector3.y, (float)vector3.z);
        }
#endif

        #endregion

        #region Operator Overloading

        public static FixVector3 operator +(FixVector3 a, FixVector3 b)
        {
            Fix64 x = a.x + b.x;
            Fix64 y = a.y + b.y;
            Fix64 z = a.z + b.z;
            return new FixVector3(x, y, z);
        }

        public static FixVector3 operator -(FixVector3 a, FixVector3 b)
        {
            Fix64 x = a.x - b.x;
            Fix64 y = a.y - b.y;
            Fix64 z = a.z - b.z;
            return new FixVector3(x, y, z);
        }

        public static FixVector3 operator *(Fix64 d, FixVector3 a)
        {
            Fix64 x = a.x * d;
            Fix64 y = a.y * d;
            Fix64 z = a.z * d;
            return new FixVector3(x, y, z);
        }

        public static FixVector3 operator *(FixVector3 a, Fix64 d)
        {
            Fix64 x = a.x * d;
            Fix64 y = a.y * d;
            Fix64 z = a.z * d;
            return new FixVector3(x, y, z);
        }

        public static FixVector3 operator /(FixVector3 a, Fix64 d)
        {
            Fix64 x = a.x / d;
            Fix64 y = a.y / d;
            Fix64 z = a.z / d;
            return new FixVector3(x, y, z);
        }

        public static bool operator ==(FixVector3 lhs, FixVector3 rhs)
        {
            return lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z;
        }

        public static bool operator !=(FixVector3 lhs, FixVector3 rhs)
        {
            return lhs.x != rhs.x || lhs.y != rhs.y || lhs.z != rhs.z;
        }

        #endregion

        #region Methods

        public void Normalize()
        {
            Fix64 n = x * x + y * y + z * z;
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
            z *= n;
        }

        public override string ToString()
        {
            return string.Format("x:{0} y:{1} z:{2}", x, y, z);
        }

        public override bool Equals(object obj)
        {
            return obj is FixVector3 && ((FixVector3)obj) == this;
        }

        public override int GetHashCode()
        {
            return this.x.GetHashCode() + this.y.GetHashCode() + this.z.GetHashCode();
        }

        #endregion

        #region Static Methods

        public static Fix64 SqrMagnitude(FixVector3 a)
        {
            return a.x * a.x + a.y * a.y + a.z * a.z;
        }

        public static Fix64 Distance(FixVector3 a, FixVector3 b)
        {
            return Magnitude(a - b);
        }

        public static Fix64 Magnitude(FixVector3 a)
        {
            return Fix64.Sqrt(FixVector3.SqrMagnitude(a));
        }

        public static FixVector3 Lerp(FixVector3 from, FixVector3 to, Fix64 factor)
        {
            return from * (1 - factor) + to * factor;
        }

        #endregion
    }
}
