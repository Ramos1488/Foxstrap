namespace Foxstrap.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string? value) =>
            string.IsNullOrEmpty(value);

        public static string OrDefault(this string? value, string defaultValue) =>
            string.IsNullOrEmpty(value) ? defaultValue : value!;
    }
}
