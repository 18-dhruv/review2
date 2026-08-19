namespace SubDomainAnalyser2;

using System.Text.RegularExpressions;

public static class DomainValidator
{
    private const int MaxLabelLength = 63;

    private static readonly Regex LabelRegex =
        new Regex(
            @"^[a-zA-Z0-9]([a-zA-Z0-9-]{0,62}[a-zA-Z0-9])?$",
            RegexOptions.Compiled
        );

    public static bool Validate(
        string rawDomain,
        out List<string> labels)
    {
        labels = new List<string>();
       

        if (string.IsNullOrWhiteSpace(rawDomain))
        {
            return false;
        }

        var parts = rawDomain.Split('.');

        if (parts.Length < 2)
        {
            return false;
        }

        if (parts.Any(string.IsNullOrEmpty))
        {
            return false;
        }

        foreach (var part in parts)
        {
            if (part.Length > MaxLabelLength || !LabelRegex.IsMatch(part))
            {
                return false;
            }
        }

        labels = parts.ToList();
        return true;
    }
}