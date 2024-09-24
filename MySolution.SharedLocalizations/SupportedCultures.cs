using System.Globalization;

namespace MySolution.SharedLocalizations;

public static class SupportedCultures
{
    public static List<Culture> Cultures =>
    [
        new(new CultureInfo("pt"), "Português", true),
        new(new CultureInfo("en"), "English"),
        new(new CultureInfo("es"), "español")
    ];

    public static List<CultureInfo> CultureInfos => Cultures
        .Select(x => x.CultureInfo)
        .ToList();
}