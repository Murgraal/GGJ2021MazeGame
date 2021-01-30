public static class FloatExtensions
{
    public static bool IsInBetween(this float a, float min,float max)
    {
        return a > min && a < max;
    }
}
