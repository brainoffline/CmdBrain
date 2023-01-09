namespace No8.CmdBrain.CommandLine;

[AttributeUsage(AttributeTargets.Property)]
public class ArgsParameterAttribute : Attribute
{
    public string?      Name        { get; set; }
    public List<string> Names       { get; } = new();
    public string?      Description { get; set; }
    public bool         IsDefault   { get; set; }

    public ArgsParameterAttribute() { }

    public ArgsParameterAttribute(string name, string description)
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

    public ArgsParameterAttribute(string[] names, string description)
    {
        Names       = names.ToList();
        Name        = Names.FirstOrDefault();
        Description = description;
    }
}