public static class ArrayExtensions
{
    public static bool IsNullOrEmpty<T>(this T[] array, int minLength = 0)
    {
        return array == null || array.Length <= minLength;
    }
}
