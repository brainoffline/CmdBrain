namespace No8.CmdBrain.Tests.Helpers;

[TestClass]
public class TextTests
{
    static List<string> EmptyStringList = new();


    [Fact]
    public void SimpleStrings_SplitInfoParts()
    {
        Assert.Equal(
            "".SplitIntoParts(),
            EmptyStringList);

        Assert.Equal(
            "One".SplitIntoParts(),
            new[] { "One" });

        Assert.Equal(
            "One Two".SplitIntoParts(),
            new[] { "One", " ", "Two" }
            );
    }

    [Fact]
    public void TrailingWhiteSpace_SplitIntoParts()
    {
        Assert.Equal(
            " ".SplitIntoParts(),
            EmptyStringList
            );
        Assert.Equal(
            "\n ".SplitIntoParts(),
            new[] { "\n" }
            );
    }

    [Fact]
    public void MultipleWhiteSpace_SplitIntoParts()
    {
        Assert.Equal(
            "One  Two".SplitIntoParts(),
            new[] { "One", "  ", "Two" }
            );

        Assert.Equal(
            "One \t\f\r\n Two".SplitIntoParts(),
            new[] { "One", " \t\f", "\n", " ", "Two" }
            );

        Assert.Equal(
            "One\n \t\fTwo".SplitIntoParts(),
            new[] { "One", "\n", " \t\f", "Two" }
            );
    }

    [Fact]
    public void SimpleStringLessThanMaxLength_SplitIntoLines()
    {
        Assert.Equal(
            "123 567".SplitIntoLines(10),
            new[] { "123 567" }
            );

        Assert.Equal(
            "123 567890".SplitIntoLines(10),
            new[] { "123 567890" }
            );

        Assert.Equal(
            "1234567890".SplitIntoLines(10),
            new[] { "1234567890" }
            );
    }

    [Fact]
    public void SimpleStringMoreThanMaxLength_SplitIntoLines()
    {
        Assert.Equal(
            "123 5678901".SplitIntoLines(10),
            new[] { "123", "5678901" }
            );

        Assert.Equal(
            "12345678901".SplitIntoLines(10),
            new[] { "1234567890", "1" }
            );
    }

    [Fact]
    public void MultiLine_SplitIntoLines()
    {
        // Trailing space on first line
        Assert.Equal(
            "123 56789 123".SplitIntoLines(10),
            new[] { "123 56789", "123" }
            );

        // Leading space on second line
        Assert.Equal(
            "123 567890 23".SplitIntoLines(10),
            new[] { "123 567890", "23" }
            );

        // First word at max length
        Assert.Equal(
            "1234567890 23".SplitIntoLines(10),
            new[] { "1234567890", "23" }
            );

        // First line more than max length
        Assert.Equal(
            "123 5678901 34".SplitIntoLines(10),
            new[] { "123", "5678901 34" }
            );

        // First word more than max length
        Assert.Equal(
            "12345678901 34".SplitIntoLines(10),
            new[] { "1234567890", "1 34" }
            );

        // Second word at max length
        Assert.Equal(
            "123 5678901234 67".SplitIntoLines(10),
            new[] { "123", "5678901234", "67" }
            );

        // Second word more than max length
        Assert.Equal(
            "123 56789012345 78".SplitIntoLines(10),
            new[] { "123", "5678901234", "5 78" }
            );
    }

    [Fact]
    public void MultiLineNewLine_SplitIntoLines()
    {
        Assert.Equal(
            "123\n56789 123".SplitIntoLines(10),
            new[] { "123", "56789 123" }
            );
        Assert.Equal(
            "123 56789\n123".SplitIntoLines(10),
            new[] { "123 56789", "123" }
            );
        Assert.Equal(
            "123 56789 123\n".SplitIntoLines(10),
            new[] { "123 56789", "123" }
            );

        // Leading space on second line
        Assert.Equal(
            "123 567890\n23".SplitIntoLines(10),
            new[] { "123 567890", "23" }
            );

        // First word at max length
        Assert.Equal(
            "1234567890\n23".SplitIntoLines(10),
            new[] { "1234567890", "23" }
            );

        // First line more than max length
        Assert.Equal(
            "123 5678901\n34".SplitIntoLines(10),
            new[] { "123", "5678901", "34" }
            );

        // First word more than max length
        Assert.Equal(
            "12345678901\n34".SplitIntoLines(10),
            new[] { "1234567890", "1", "34" }
            );

        // Second word at max length
        Assert.Equal(
            "123 5678901234\n67".SplitIntoLines(10),
            new[] { "123", "5678901234", "67" }
            );

        // Second word more than max length
        Assert.Equal(
            "123 56789012345\n78".SplitIntoLines(10),
            new[] { "123", "5678901234", "5", "78" }
            );
    }

    [Fact]
    public void Puncuation_SplitIntoLines()
    {
        Assert.Equal(
            "123, 567".SplitIntoLines(10),
            new[] { "123, 567" }
            );

        Assert.Equal(
            "123, 67890".SplitIntoLines(10),
            new[] { "123, 67890" }
            );

        Assert.Equal(
            "123456789,".SplitIntoLines(10),
            new[] { "123456789," }
            );

        // Trailing space on first line
        Assert.Equal(
            "123 5678. 123".SplitIntoLines(10),
            new[] { "123 5678.", "123" }
            );

        // Leading space on second line
        Assert.Equal(
            "123 56789. 23".SplitIntoLines(10),
            new[] { "123 56789.", "23" }
            );

        // First word at max length
        Assert.Equal(
            "123456789. 23".SplitIntoLines(10),
            new[] { "123456789.", "23" }
            );

        // First line more than max length
        Assert.Equal(
            "123 567890, 34".SplitIntoLines(10),
            new[] { "123", "567890, 34" }
            );

        // First word more than max length
        Assert.Equal(
            "1234567890, 34".SplitIntoLines(10),
            new[] { "1234567890", ", 34" }
            );

        // Second word at max length
        Assert.Equal(
            "123 567890123, 67".SplitIntoLines(10),
            new[] { "123", "567890123,", "67" }
            );

        // Second word more than max length
        Assert.Equal(
            "123 5678901234, 78".SplitIntoLines(10),
            new[] { "123", "5678901234", ", 78" }
            );
    }


    [Fact]
    public void SimpleStringLessThanMaxLength_SplitIntoLines_MaxHeight()
    {
        Assert.Equal(
            "123 567".SplitIntoLines(10, 1),
            new[] { "123 567" }
            );

        Assert.Equal(
            "123 567890".SplitIntoLines(10, 1),
            new[] { "123 567890" }
            );

        Assert.Equal(
            "1234567890".SplitIntoLines(10, 1),
            new[] { "1234567890" }
            );

        Assert.Equal(
            "12345678901".SplitIntoLines(10, 1),
            new[] { "12345678.." }
            );
    }



    // Single line scenarios
    // ----*----*
    // abc.dev                 less than max width
    // acbdef.ghi              max width
    // abcdefghij              max width one word
    // abc.defghij             more than max width
    // abcdefghijk             more than max width in one word

    // Multi line scenarios
    // ----*----*----*----*
    // acbdef.ghi.abc          max width
    // abcdefghij.abc          max width one word
    // abc.defghij.abc         more than max width
    // abcdefghijk.abc         more than max width in one word

    // abcdef.ghij.abc         second line less than max width
    // acbde.ghi.acbdef.ghi    second line max width
    // abcdefghij.abcdefghij   second line max width one word
    // abc.defgh.abc.defghijk  second line more than max width
    // abcdefghijk.abcdefghijk second line more than max width in one word

    // Can add new line (maxHeight)

    // if truncate
    // $"{Text.Substring(start, (int)max - 2)}..";

    // White Space = Backspace (8 \b), Horz tab (9 \t), Line feed (10 \n), Vert Tab (11 \v), Form feed (12, \f), Carriage return (13, \r), Space(20 ' '), 

    // what about 
    // - punculation (,.-)
    // - justification of words
    // - lots of white space
    // - trim word,
    // - empty line,
    // - Truncate big words, or split at max width
    // - trim start, trim end
    // - indent subsequent lines


}
