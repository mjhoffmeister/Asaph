namespace Asaph.Core.Specs.Steps
{
    public static class NullValueTranslator
    {
        public static string? TranslateNull(this string? value) => value == "<null>" ? null : value;
    }
}
