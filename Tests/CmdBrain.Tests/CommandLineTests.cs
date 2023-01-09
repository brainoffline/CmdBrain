using No8.CmdBrain.CommandLine;
using No8.CmdBrain.Tests.Helpers;
using static No8.CmdBrain.Tests.Helpers.TestHelper;

namespace No8.CmdBrain.Tests;

[TestClass]
public class ArgsParserTests
{
    private readonly ArgsParser _parser = new ArgsParser()
                                         .SetIncludeEnvironmentVariables()
                                         .AddCommand<OptionArgs>(isDefault: true);

    [Fact]
    public void TestCaseInsensitive()
    {
        var parser = new ArgsParser()
                    .SetIncludeEnvironmentVariables()
                    .SetCaseSensitive(false)
                    .AddCommand<OptionArgs>(isDefault: true);

        // Case insensitive
        var obj = (OptionArgs)parser.Parse("--Flag=true --Int=0 --Float=1 --String=what --Enum=first --List=yes")!;

        AssertMultiple(
            () => Assert.True(obj.Flag), 
            () => Assert.Equal(0, obj.IntValue), 
            () => Assert.Equal(1f, obj.FloatValue), 
            () => Assert.Equal("what", obj.StringValue), 
            () => Assert.Equal(OptionArgsMode.First, obj.EnumValue), 
            () => Assert.Equal("yes", obj.ListValue![0])
            );
    }

    [Fact]
    public void TestNegatives()
    {
        // Case sensitive
        AssertException(typeof(ArgumentException), "Parameter [Flag] has not been defined", () => _parser.Parse("--Flag=true"));
        AssertException(typeof(ArgumentException), "Parameter [Int] has not been defined", () => _parser.Parse("--Int=0"));
        AssertException(typeof(ArgumentException), "Parameter [Float] has not been defined", () => _parser.Parse("--Float=1"));
        AssertException(typeof(ArgumentException), "Parameter [String] has not been defined", () => _parser.Parse("--String=what"));
        AssertException(typeof(ArgumentException), "Parameter [Enum] has not been defined", () => _parser.Parse("--Enum=First"));
        AssertException(typeof(ArgumentException), "Requested value 'first' was not found.", () => _parser.Parse("--enum=first"));
        AssertException(typeof(ArgumentException), "Parameter [List] has not been defined", () => _parser.Parse("--List=no"));
    }

    [Fact]
    public void TestBoolean()
    {
        var obj = (OptionArgs)_parser.Parse($"--flag=true")!;
        Assert.True(obj.Flag);

        obj = (OptionArgs)_parser.Parse($"--f=False")!;
        Assert.False(obj.Flag);
    }

    [Fact]
    public void TestInt()
    {
        var obj = (OptionArgs)_parser.Parse("--int=1")!;
        Assert.Equal(1, obj.IntValue);

        obj      = (OptionArgs)_parser.Parse("--i=2")!;
        Assert.Equal(2, obj.IntValue);

        obj      = (OptionArgs)_parser.Parse($"--int16={short.MinValue}")!;
        Assert.Equal(short.MinValue, obj.Int16Value);

        obj      = (OptionArgs)_parser.Parse($"--int32={Int32.MaxValue}")!;
        Assert.Equal(Int32.MaxValue, obj.Int32Value);

        obj      = (OptionArgs)_parser.Parse($"--int64={Int64.MaxValue}")!;
        Assert.Equal(Int64.MaxValue, obj.Int64Value);
    }

    [Fact]
    public void TestNumbers()
    {
        var obj = (OptionArgs)_parser.Parse($"--float={1.0f}")!;
        Assert.Equal(1.0f, obj.FloatValue);

        obj      = (OptionArgs)_parser.Parse($"--double={1.1d}")!;
        Assert.Equal(1.1d, obj.DoubleValue);

        obj      = (OptionArgs)_parser.Parse($"--decimal={1.2m}")!;
        Assert.Equal(1.2m, obj.DecimalValue);
    }


    [Fact]
    public void TestString()
    {
        var obj = (OptionArgs)_parser.Parse($"--string=1")!;
        Assert.Equal("1", obj.StringValue);

        obj      = (OptionArgs)_parser.Parse($"--s=2")!;
        Assert.Equal("2", obj.StringValue);
    }

    [Fact]
    public void TestDateTime()
    {
        var dt = DateTime.Now;

        var obj = (OptionArgs)_parser.Parse($"--date={dt:O}")!;
        Assert.Equal(dt, obj.DateValue);
    }

    [Fact]
    public void TestEnum()
    {
        var obj = (OptionArgs)_parser.Parse($"--enum={OptionArgsMode.First}")!;
        Assert.Equal(OptionArgsMode.First, obj.EnumValue);

        obj      = (OptionArgs)_parser.Parse($"--e=Second")!;
        Assert.Equal(OptionArgsMode.Second, obj.EnumValue);
    }

    [Fact]
    public void TestList()
    {
        var obj = (OptionArgs)_parser.Parse($"--list=First")!;
        Assert.Equal("First", obj.ListValue![0]);

        obj      = (OptionArgs)_parser.Parse($"--list=First --l=Second")!;
        Assert.Equal("Second", obj.ListValue![1]);
    }

    [Fact]
    public void TestCommandLines()
    {
        var obj = (OptionArgs)_parser.Parse(
            new List<string>
            {
                "-flag",
                "--int",
                "1",
                "/int16",
                "2",
                "-int32=3",
                "--int64:4",
                "/enum=Second"
            }, out _)!;

        Assert.True(obj.Flag);
        Assert.Equal(1, obj.IntValue);
        Assert.Equal(2, obj.Int16Value);
        Assert.Equal(3, obj.Int32Value);
        Assert.Equal(4, obj.Int64Value);
        Assert.Equal(OptionArgsMode.Second, obj.EnumValue);
    }

    [Fact]
    public void TestEmptyCommandLine()
    {
        _parser.Parse("");
    }

    [Fact]
    public void TextExtras()
    {
        var obj = (OptionArgs)_parser.Parse($"cmd")!;
        Assert.Null(obj.ListValue);

        _ = (OptionArgs)_parser.Parse($"ArgsOptions Extra Text", out var extras)!;
        Assert.Equal(2, extras!.Count);

        _ = (OptionArgs)_parser.Parse($"doit Extra Text", out extras)!;
        Assert.Equal(3, extras!.Count);
    }

    [Fact]
    public void TextHelp()
    {
        var obj = _parser.Parse($"--?")!;
        if (obj is not HelpCommand)
            Assert.True(false, "No help");
    }

}