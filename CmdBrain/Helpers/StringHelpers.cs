using System.Collections;

namespace No8.CmdBrain;

public static class StringHelpers
{
    public static bool IsNullOrWhiteSpace(this IEnumerable<char>? value)
    {
        if (value == null) return true;

        foreach (var c in value)
            if (!char.IsWhiteSpace(c))
                return false;

        return true;
    }

    public static bool IsNullOrWhiteSpace(this string? value) { return string.IsNullOrWhiteSpace(value); }

    public static bool IsNullOrEmpty(this string? value) { return string.IsNullOrEmpty(value); }

    public static bool HasValue(this string? value) { return !string.IsNullOrWhiteSpace(value); }

    /// <summary>
    /// Parse a string into multiple arguments (similar to command-line args[]).
    /// Can be a multi-line string
    /// </summary>
    public static List<string>? ParseArguments(this string lines)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        if (lines == null)
            return default;

        var list = new List<string>();
        var span = lines.Trim().AsSpan();

        var start = -1;
        for (var i = 0; i < span.Length; i++)
        {
            var ch = span[i];

            if (ch == '"' || ch == '\'')
            {
                if (start >= 0)
                    AddArg(span, ref start, i);

                var end = ch;
                start = i + 1;

                for (i++; i < span.Length; i++)
                {
                    ch = span[i];

                    if (ch == end)
                        break;
                }

                AddArg(span, ref start, i);
            }
            // Any word separating character
            else if (char.IsWhiteSpace(ch) || ch == '\r' || ch == '\n' || char.IsSeparator(ch))
            {
                if (start >= 0)
                    AddArg(span, ref start, i);
            }
            // everything else is part of the word
            else
            {
                if (start < 0)
                    start = i;
            }
        }

        if (start >= 0)
            AddArg(span, ref start, span.Length);

        return list;

        void AddArg(ReadOnlySpan<char> spanReadOnly, ref int argStart, int end)
        {
            var arg = spanReadOnly.Slice(argStart, end - argStart).ToString().Trim();
            if (arg.HasValue())
                list.Add(arg);
            argStart = -1;
        }
    }

    public static IEnumerable<string> EnumerateLines(this TextReader reader)
    {
        string? line;

        while ((line = reader.ReadLine()) != null)
            yield return line;
    }

    public static IEnumerable<string> EnumerateLines(this string str)
    {
        using var reader = new StringReader(str);
        return EnumerateLines(reader);
    }

    public static char TryGet(this StringBuilder sb, int index)
    {
        return sb.Length > index
            ? sb[index]
            : '\0';
    }

    public static char TryGet(this string str, int index)
    {
        return str.Length > index
            ? str[index]
            : '\0';
    }

    public static bool ContainsWithComparison(this IEnumerable<string> list, string value, StringComparison comparisonType = StringComparison.Ordinal)
    {
        return list.Any(str => str.Equals(value, comparisonType));
    }

    public static bool ContainsWithComparison(this IDictionary dict, string value, StringComparison comparisonType = StringComparison.Ordinal)
    {
        return dict.Keys
                   .Cast<string>()
                   .Any(str => str.Equals(value, comparisonType));
    }

    public static List<string> SplitIntoLines(this string str, int maxWidth, int maxHeight = -1)
    {
        str = str.Replace("\r\n", "\n")
                 .TrimEnd(MyWhiteSpace);

        if (string.IsNullOrEmpty(str))
            return EmptyStringList;

        if (maxWidth <= 3)
            maxWidth = 3;

        if (maxHeight == 1)
        {
            var text = str.Length > maxWidth ? $"{str.Substring(0, maxWidth - 2)}.." : str;
            return new List<string>() { text };
        }

        var lines = new List<string>();
        var index = 0;
        int start = 0;
        int count = 0;
        bool lastPartMaxedLine = false;

        var parts = SplitIntoParts(str);

        while (index < parts.Count)
        {
            var lastPart = index > 0 ? parts[index - 1] : null;
            var part = parts[index++];
            var nextPart = index >= parts.Count ? null : parts[index];

            var lastPartIsNewLine = lastPart == "\n";

            if (part == "\n")
            {
                if (count > 0)
                    lines.Add(str.Substring(start, count));
                else if (!lastPartMaxedLine)
                    lines.Add("");
                start += count + part.Length;
                count = 0;

                lastPartMaxedLine = false;
            }
            else if (part[0].IsWhiteSpace())
            {
                lastPartMaxedLine = false;

                if (count == 0 && !lastPartIsNewLine)
                {
                    // new line but previous line was split by length or spaces
                    start += part.Length;
                    continue;
                }

                if (count + part.Length + nextPart?.Length > maxWidth)
                {
                    // spaces + word is too much
                    if (count > 0)
                        lines.Add(str.Substring(start, count));

                    // ignore trailing spaces on line and carry onto next word
                    start += count + part.Length;
                    count = 0;
                    continue;
                }

                // include spaces + word
                count += part.Length + (nextPart?.Length ?? 0);
                index++;
            }
            else if (count + part.Length == maxWidth)
            {
                count += part.Length;
                lines.Add(str.Substring(start, count));

                start += count;
                count = 0;
                lastPartMaxedLine = true;
            }
            else if (count + part.Length > maxWidth)
            {
                lines.Add(str.Substring(start, maxWidth));
                start += maxWidth;

                index--;
                parts[index] = part.Substring(maxWidth - count);
                count = 0;
                lastPartMaxedLine = false;
            }
            else
                count += part.Length;
        }

        if (count > 0)
            lines.Add(str.Substring(start, count));

        return lines;
    }

    internal static List<string> SplitIntoParts(this string str)
    {
        str = str.Replace("\r\n", "\n")
                 .TrimEnd(MyWhiteSpace);

        if (string.IsNullOrEmpty(str))
            return EmptyStringList;

        var result = new List<string>();

        int index = 0;
        while (true)
        {
            if (index >= str.Length)
                break;

            var startIndex = index;
            var firstCh = str[startIndex];

            if (firstCh.IsNewLine())
            {
                result.Add("\n");
                index++;
                continue;
            }

            if (firstCh.IsWhiteSpace())
            {
                // find end of white space
                index++;
                while (index < str.Length)
                {
                    var nextCh = str[index];
                    if (nextCh.IsNewLine() || !nextCh.IsWhiteSpace())
                        break;
                    index++;
                }

                result.Add(str.Substring(startIndex, index - startIndex));
                continue;
            }

            // find end of word
            index++;
            while (index < str.Length)
            {
                var nextCh = str[index];
                if (nextCh.IsNewLine() || nextCh.IsWhiteSpace())
                    break;

                index++;
            }
            result.Add(str.Substring(startIndex, index - startIndex));
        }

        return result;
    }

    private static bool IsWhiteSpace(this char c)
    {
        if (c < 0xFF)
            return MyWhiteSpace.Contains(c);

        // Unicode
        return char.IsWhiteSpace(c);
    }

    private static bool IsNewLine(this char c)
    {
        switch (c)
        {
            case '\r':
            case '\n':
            case '\x0085':
                return true;
        }
        return false;
    }

    static List<string> EmptyStringList = new();

    static char[] MyWhiteSpace = new[] { '\b', '\f', '\t', '\v', ' ', '\x00a0' };
    // Exclude \r \n from White Space definition

    // U+0009 = <control> HORIZONTAL TAB
    // U+000a = <control> LINE FEED         << NO
    // U+000b = <control> VERTICAL TAB
    // U+000c = <contorl> FORM FEED
    // U+000d = <control> CARRIAGE RETURN   << NO
    // U+0085 = <control> NEXT LINE         << NO
    // U+00a0 = NO-BREAK SPACE


    public static List<string> WrapText(this string text, int maxLineLength)
    {
        List<string> result = new();

        if (string.IsNullOrWhiteSpace(text) || maxLineLength <= 0)
            return result;

        var lines = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

        foreach (var one in lines)
        {
            var line = one.TrimEnd();

            if (line.Length < maxLineLength)
            {
                result.Add(line);
                continue;
            }

            int currentIndex;
            var lastWrap = 0;
            var whitespace = new[] { ' ', '\t' };
            do
            {
                currentIndex = lastWrap + maxLineLength >= line.Length
                                   ? line.Length
                                   : line.LastIndexOfAny(new[] { ' ', '\n', '\r' },
                                                          Math.Min(line.Length - 1, lastWrap + maxLineLength)) + 1;
                if (currentIndex <= lastWrap)
                    currentIndex = Math.Min(lastWrap + maxLineLength, line.Length);
                result.Add(line.Substring(lastWrap, currentIndex - lastWrap).Trim(whitespace));
                lastWrap = currentIndex;
            } while (currentIndex < line.Length);
        }

        // remove any trailing empty lines
        while (result.Count > 0)
        {
            if (result[^1].Length > 0)
                break;
            result.RemoveAt(result.Count - 1);
        }
        return result;
    }

    public static string TruncateWithEllipses(this string text, int maxLineLength, bool atWord = false)
    {
        if (maxLineLength < 3)
            return text;

        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        if (text.Length <= maxLineLength)
            return text;

        if (atWord)
        {
            int lastSpace = text.LastIndexOf(" ", maxLineLength - 2, StringComparison.Ordinal);

            if (lastSpace > 0)
                return text.Substring(0, lastSpace) + "..";
        }

        return text.Substring(0, maxLineLength - 2) + "..";
    }
}
