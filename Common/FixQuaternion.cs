using System;

namespace Common
{
    [Serializable]
    public struct FixQuaternion
    {
        #region Fields

        public Fix64 x;
        public Fix64 y;
        public Fix64 z;
        public Fix64 w;

        #endregion

        #region Constructors

        public FixQuaternion(Fix64 x, Fix64 y, Fix64 z, Fix64 w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public FixQuaternion(FixQuaternion v)
        {
            this.x = v.x;
            this.y = v.y;
            this.z = v.z;
            this.w = v.w;
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
                else if (index == 2)
                    return z;
                else
                    return w;
            }
            set
            {
                if (index == 0)
                    x = value;
                else if (index == 1)
                    y = value;
                else if (index == 2)
                    z = value;
                else
                    w = value;
            }
        }

        #endregion

        #region Static Properties

        public static FixQuaternion Identity
        {
            get
            {
                return new FixQuaternion((Fix64)0, (Fix64)0, (Fix64)0, (Fix64)1);
            }
        }

        #endregion

        #region Option Explicit Statement

#if UNITY
        public static explicit operator FixQuaternion(UnityEngine.Quaternion quaternion)
        {
            return new FixQuaternion((Fix64)quaternion.x, (Fix64)quaternion.y, (Fix64)quaternion.z, (Fix64)quaternion.w);
        }

        public static explicit operator UnityEngine.Quaternion(FixQuaternion quaternion)
        {
            return new UnityEngine.Quaternion((float)quaternion.x, (float)quaternion.y, (float)quaternion.z, (float)quaternion.w);
        }
#endif

        #endregion

        #region Operator Overloading

        public static FixQuaternion operator -(FixQuaternion q)
        {
            Negate(q, out var result);
            return result;
        }

        public static FixQuaternion operator *(FixQuaternion a, FixQuaternion b)
        {
            FixQuaternion toReturn;
            Multiply(a, b, out toReturn);
            return toReturn;
        }

        public static bool operator ==(FixQuaternion lhs, FixQuaternion rhs)
        {
            return lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z && lhs.w == rhs.w;
        }

        public static bool operator !=(FixQuaternion lhs, FixQuaternion rhs)
        {
            return lhs.x != rhs.x || lhs.y != rhs.y || lhs.z != rhs.z || lhs.w != rhs.w;
        }

        #endregion

        #region Methods

        public void Normalize()
        {
            Fix64 n = x * x + y * y + z * z + w * w;
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
            w *= n;
        }

        public override string ToString()
        {
            return string.Format("x:{0} y:{1} z:{2} w:{3}", x, y, z, w);
        }

        public override bool Equals(object obj)
        {
            return obj is FixQuaternion && ((FixQuaternion)obj) == this;
        }

        public override int GetHashCode()
        {
            return this.x.GetHashCode() + this.y.GetHashCode() + this.z.GetHashCode() + this.w.GetHashCode();
        }

        #endregion

        #region Static Methods

        public static void Multiply(FixQuaternion a, FixQuaternion b, out FixQuaternion result)
        {
            Fix64 aX = a.x;
            Fix64 aY = a.y;
            Fix64 aZ = a.z;
            Fix64 aW = a.w;
            Fix64 bX = b.x;
            Fix64 bY = b.y;
            Fix64 bZ = b.z;
            Fix64 bW = b.w;
            result.x = aX * bW + bX * aW + aY * bZ - aZ * bY;
            result.y = aY * bW + bY * aW + aZ * bX - aX * bZ;
            result.z = aZ * bW + bZ * aW + aX * bY - aY * bX;
            result.w = aW * bW - aX * bX - aY * bY - aZ * bZ;
        }

        public static void Multiply(FixQuaternion q, Fix64 scale, out FixQuaternion result)
        {
            result.x = q.x * scale;
            result.y = q.y * scale;
            result.z = q.z * scale;
            result.w = q.w * scale;
        }

        public static void Negate(FixQuaternion a, out FixQuaternion b)
        {
            b.x = -a.x;
            b.y = -a.y;
            b.z = -a.z;
            b.w = -a.w;
        }

        public static FixQuaternion Negate(FixQuaternion q)
        {
            Negate(q, out var result);
            return result;
        }

        #endregion
    }
}
