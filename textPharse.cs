

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
public static class textPharse
{
    //all variantions of saying no there where no changes
    public static bool checkForNoChanges(string machineText)
    {
        if (machineText.Contains("nothing"))
        {
            return true;
        }
        if (machineText.Contains("no"))
        {
            return true;
        }
        if (machineText.Contains("no changes"))
        {
            return true;
        }
        if (machineText.Contains("no change"))
        {
            return true;
        }
        if (machineText.Contains("nothings changed"))
        {
            return true;
        }
        if (machineText.Contains("nothings change"))
        {
            return true;
        }
        if (machineText.Contains("Nein"))
        {
            return true;
        }
        return false;

    }
    public static List<(string, string)> ParseDoubleEntryList(string list)
    {
        List<(string, string)> result = new List<(string, string)>();
        try
        {
            result = textPharse.ParseMixedFormat(list);
            return result;
        }

        catch (Exception e)
        {
        }
        try
        {
            result = textPharse.ParseChildrenListFormat7(list);
            return result;
        }

        catch (Exception e)
        {
        }
        try
        {
            result = textPharse.ParseChildrenListFormat1(list);
            return result;
        }
        catch (Exception e)
        {
        }
        try
        {
            result = textPharse.ParseChildrenListFormat2(list);
            return result;
        }
        catch (Exception e)
        {
        }
        try
        {
            result = textPharse.ParseChildrenListFormat3(list);
            return result;
        }
        catch (Exception e)
        {
        }
        try
        {
            result = textPharse.ParseChildrenListFormat4(list);
            return result;
        }
        catch (Exception e)
        {
        }
        try
        {
            result = textPharse.ParseChildrenListFormat5(list);
            return result;
        }
        catch (Exception e)
        {
        }

        return result;


    }
    public static List<string> ParseList(string list)
    {
        List<string> result = new List<string>();
        try
        {
            result = textPharse.ParseNewlineSeparatedList(list);
            return result;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
        try
        {
            result = textPharse.ParseCommaSeparatedList(list);
            return result;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
        return result;


    }
    public static List<(string, string)> ParseChildrenListFormat1(string childListText)
    {
        var children = new List<(string, string)>();
        string[] childEntries = childListText.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var entry in childEntries)
        {
            var parts = entry.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2)
            {
                string childName = parts[0].Trim();
                string childDescription = parts[1].Trim();
                children.Add((childName, childDescription));
            }
            else
            {
                throw new FormatException("The child entry format is incorrect.");
            }
        }
        return children;
    }

    // Method for parsing the '1. Name: Name1, Description: Desc1; ...' format
    public static List<(string, string)> ParseChildrenListFormat2(string childListText)
    {
        var children = new List<(string, string)>();
        string[] childEntries = childListText.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var entry in childEntries)
        {
            var parts = entry.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2)
            {
                string childName = parts[0].Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries)[1].Trim();
                string childDescription = parts[1].Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries)[1].Trim();
                children.Add((childName, childDescription));
            }
            else
            {
                throw new FormatException("The child entry format is incorrect.");
            }
        }
        return children;
    }
    public static List<(string, string)> ParseChildrenListFormat3(string childListText)
    {
        var children = new List<(string, string)>();
        string[] childEntries = childListText.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var entry in childEntries)
        {
            var parts = entry.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2)
            {
                // Remove any leading number and period (e.g. '1. ')
                string childName = System.Text.RegularExpressions.Regex.Replace(parts[0].Trim(), @"^\d+\.\s*", "");
                string childDescription = parts[1].Trim();
                children.Add((childName, childDescription));
            }
            else
            {
                throw new FormatException("The child entry format is incorrect.");
            }
        }
        return children;
    }

    // Method for parsing the '1. Name: Name1\nDescription: Desc1\n...' format
    public static List<(string, string)> ParseChildrenListFormat4(string childListText)
    {
        var children = new List<(string, string)>();
        string[] childEntries = childListText.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < childEntries.Length; i += 2)
        {
            if (i + 1 < childEntries.Length)
            {
                var nameParts = childEntries[i].Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                var descParts = childEntries[i + 1].Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                if (nameParts.Length >= 2 && descParts.Length >= 2)
                {
                    string childName = nameParts[1].Trim();
                    string childDescription = descParts[1].Trim();
                    children.Add((childName, childDescription));
                }
                else
                {
                    throw new FormatException("The child entry format is incorrect.");
                }
            }
            else
            {
                throw new FormatException("The child entry format is incorrect.");
            }
        }
        return children;
    }
    // Method for parsing the '1. Frostfall Village - ... 2. Stormchaser's Cove - ...' format
    public static List<(string, string)> ParseChildrenListFormat5(string childListText)
    {
        var children = new List<(string, string)>();

        // Check for an unnecessary phrase at the beginning and remove it
        if (!childListText.TrimStart().StartsWith("1."))
        {
            var index = childListText.IndexOf("1.");
            if (index != -1)
            {
                childListText = childListText.Substring(index);
            }
            else
            {
                throw new FormatException("The child entry format is incorrect or incomplete.");
            }
        }

        string[] childEntries = childListText.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var entry in childEntries)
        {
            var parts = entry.Split(new[] { " - " }, StringSplitOptions.None);
            if (parts.Length >= 2)
            {
                // Remove any leading number and period (e.g. '1.')
                string childName = System.Text.RegularExpressions.Regex.Replace(
                    parts[0].Trim(), @"^\d+\.", "").Trim();

                string childDescription = parts[1].Trim();

                children.Add((childName, childDescription));
            }
            else
            {
                throw new FormatException("The child entry format is incorrect.");
            }
        }

        return children;
    }
    public static List<(string, string)> ParseChildrenListFormat6(string childListText)
    {
        var children = new List<(string, string)>();

        // Split the text by asterisks to separate the entries
        string[] childEntries = childListText.Split(new[] { '*' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var entry in childEntries)
        {
            // Split the entry by the first colon to separate the name from the description
            var parts = entry.Split(new[] { ':' }, 2);  // The 2 parameter ensures we only split on the first colon
            if (parts.Length >= 2)
            {
                string childName = parts[0].Trim();
                string childDescription = parts[1].Trim();

                children.Add((childName, childDescription));
            }
            else
            {
                // Optional: Error handling if the parsing fails
                throw new FormatException("The child entry format is incorrect.");
            }
        }

        return children;
    }
    public static List<(string, string)> ParseMixedFormat(string text)
    {
        var regionsAndAreas = new List<(string, string)>();
        var lines = text.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            if (line.StartsWith("Name: "))
            {
                var name = line.Substring("Name: ".Length);
                if (i + 1 < lines.Length && lines[i + 1].StartsWith("Description: "))
                {
                    var description = lines[i + 1].Substring("Description: ".Length);
                    regionsAndAreas.Add((name, description));
                    i++;  // Skip the next line as it's part of the current entry
                }
                else
                {
                    throw new FormatException($"Missing description for name: {name}");
                }
            }
            else
            {
                throw new FormatException($"Unexpected line format: {line}");
            }
        }
        return regionsAndAreas;
    }

    public static List<(string, string)> ParseChildrenListFormat7(string childListText)
    {
        var children = new List<(string, string)>();
        // Split the text into entries based on two newline characters
        string[] childEntries = childListText.Split(new[] { "\n\n" }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var entry in childEntries)
        {
            // Split each entry into lines based on a single newline character
            string[] lines = entry.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length >= 2)
            {
                // Split the name and description lines based on the colon character
                var nameParts = lines[0].Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                var descParts = lines[1].Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                if (nameParts.Length >= 2 && descParts.Length >= 2)
                {
                    string childName = nameParts[1].Trim();
                    string childDescription = descParts[1].Trim();
                    children.Add((childName, childDescription));
                }
                else
                {
                    throw new FormatException("The child entry format is incorrect.");
                }
            }
            else
            {
                throw new FormatException("The child entry format is incorrect.");
            }
        }
        return children;
    }

    public static List<string> ParseCommaSeparatedList(string listText)
    {
        var items = new List<string>();
        string[] entries = listText.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var entry in entries)
        {
            items.Add(entry.Trim());
        }
        return items;
    }
    public static List<string> ParseNewlineSeparatedList(string listText)
    {
        var items = new List<string>();
        string[] entries = listText.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var entry in entries)
        {
            items.Add(entry.Trim());
        }
        return items;
    }
    public static string ParseRemoveNumber(string listText)
    {
        var lines = listText.Split('\n');
        var cleanedLines = lines.Select(line =>
        {
            // Find the position of the first dot in the line
            int dotIndex = line.IndexOf('.');
            if (dotIndex != -1)
            {
                var substringBeforeDot = line.Substring(0, dotIndex).Trim();
                if (substringBeforeDot.All(char.IsDigit))
                {
                    // Remove the numbering and the dot, and trim any leading/trailing spaces
                    return line.Substring(dotIndex + 1).Trim();
                }
            }
            return line;
        });
        return string.Join(" ", cleanedLines);
    }

    public static ParsedPhrase ParsePhrase(string phrase)
    {
        ParsedPhrase result = new ParsedPhrase();

        var subjectMatch = Regex.Match(phrase, @"Subject:\s*(.*)");
        var verbMatch = Regex.Match(phrase, @"Verb:\s*(.*)");
        var objectMatch = Regex.Match(phrase, @"Object:\s*(.*)");

        if (subjectMatch.Success)
        {
            result.Subject = subjectMatch.Groups[1].Value.Trim();
        }
        if (verbMatch.Success)
        {
            result.Verb = verbMatch.Groups[1].Value.Trim();
        }
        if (objectMatch.Success)
        {
            result.Object = objectMatch.Groups[1].Value.Trim();
        }

        return result;

        return result;
    }

}
public class ParsedPhrase
{
    public string Subject { get; set; }
    public string Verb { get; set; }
    public string Object { get; set; }
}
