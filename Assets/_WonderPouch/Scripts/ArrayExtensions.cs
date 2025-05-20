public static class ArrayExtensions
{
    public static bool IsNullOrEmpty<T>(this T[] array, int minLength = 0)
    {
        return array == null || array.Length <= minLength;
    }

    public static bool CheckIsValidIndex<T>(this T[] array, int index)
    {
        return index >= 0 && index < array.Length;
    }
}
