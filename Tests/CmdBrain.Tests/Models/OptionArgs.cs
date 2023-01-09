using No8.CmdBrain.CommandLine;

namespace No8.CmdBrain.Tests;

public enum OptionArgsMode
{
    NotSet,
    First,
    Second,
    Last
}

[ArgsCommand("ArgsOptions", Description = "Test class for testing different parameters")]
public class OptionArgs : IArgsCommand
{
    [ArgsParameter("flag|f", "Set a flag value")]
    public bool Flag { get; set; }

    [ArgsParameter("int|i", "Set an integer value")]
    public int IntValue { get; set; }

    [ArgsParameter("int16", "Set an integer value")]
    public Int16 Int16Value { get; set; }

    [ArgsParameter("int32", "Set an integer value")]
    public Int32 Int32Value { get; set; }

    [ArgsParameter("int64", "Set an integer value")]
    public Int64 Int64Value { get; set; }

    [ArgsParameter("float|single", "Set a float (single) value")]
    public float FloatValue { get; set; }

    [ArgsParameter("double|d", "Set a double value")]
    public double DoubleValue { get; set; }

    [ArgsParameter("decimal", "Set a decimal value")]
    public decimal DecimalValue { get; set; }

    [ArgsParameter("string|s", "Set a single string value")]
    public string? StringValue { get; set; }

    [ArgsParameter("date", "Set a date time value")]
    public DateTime DateValue { get; set; }

    [ArgsParameter("enum|e", "Set an enum value")]
    public OptionArgsMode EnumValue { get; set; }

    [ArgsParameter("list|l", "Add value to List")]
    public List<string>? ListValue { get; set; }
}
