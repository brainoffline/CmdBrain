namespace No8.CmdBrain.CommandLine;

[AttributeUsage(AttributeTargets.Class)]
public class ArgsCommandAttribute : Attribute
{
    public string?      Name        { get; set; }
    public List<string> Names       { get; } = new();
    public string?      Description { get; set; }

    public ArgsCommandAttribute() { }

    public ArgsCommandAttribute(string name, string? description = null)
    {
        if (name.Contains('|'))
        {
            Names = name
                   .Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                   .ToList();
            Name = Names.FirstOrDefault();
        }
        else
        {
            Name = name;
            Names.Add(Name);
        }

        Description = description;
    }

    public ArgsCommandAttribute(string[] names, string? description = null)
    {
        Names       = names.ToList();
        Name        = Names.FirstOrDefault();
        Description = description;
    }
}