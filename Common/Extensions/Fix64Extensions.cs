namespace Common
{
    public static class Fix64Extensions
    {
        public static Fix64 ClampAngle(this Fix64 i)
        {
            for (; i < Fix64.Zero; i += Fix64.I360) ;
            if (i > Fix64.I360)
                i %= Fix64.I360;
            return i;
        }
    }
}
