using System;

namespace Common
{
    class SRandom
    {
        private ulong randSeed = 1;
        private static int count = 0;

        public SRandom(uint seed)
        {
            randSeed = seed;
        }

        public uint Next()
        {
            randSeed = randSeed * 1103515245 + 12345;
            return (uint)(randSeed / 65536);
        }

        // 0 ~ (max-1)
        public uint Next(uint max)
        {
            return Next() % max;
        }

        // min ~ (max-1)
        public uint Range(uint min, uint max)
        {
            if (min > max)
                throw new ArgumentOutOfRangeException("minValue", string.Format("'{0}' cannot be greater than {1}.", min, max));

            uint num = max - min;
            return Next(num) + min;
        }

        public int Next(int max)
        {
            return (int)(Next() % max);
        }

        public int Range(int min, int max)
        {
            count++;

            if (min > max)
                throw new ArgumentOutOfRangeException("minValue", string.Format("'{0}' cannot be greater than {1}.", min, max));

            int num = max - min;

            return Next(num) + min;
        }

        public Fix64 Range(Fix64 min, Fix64 max)
        {
            if (min > max)
                throw new ArgumentOutOfRangeException("minValue", string.Format("'{0}' cannot be greater than {1}.", min, max));

            uint num = (uint)(max.rawValue - min.rawValue);
            return Fix64.FromRaw(Next(num) + min.rawValue);
        }
    }
}
