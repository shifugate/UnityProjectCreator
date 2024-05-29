namespace Assets._Scripts.Util
{
    public static class MathUtil
    {
        public static float WrapAngle(float angle)
        {
            angle %= 360;
            if (angle > 180)
                return angle - 360;

            return angle;
        }
    }
}
