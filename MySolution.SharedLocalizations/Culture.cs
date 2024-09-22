using System.Globalization;

namespace MySolution.SharedLocalizations;

public class Culture(
    CultureInfo cultureInfo, 
    string displayName,
    bool isDefault = false)
{
    public CultureInfo CultureInfo { get; set; } = cultureInfo;
    public string DisplayName { get; set; } = displayName;
    public bool IsDefault { get; set; } = isDefault;
}