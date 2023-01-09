using System.ComponentModel;

namespace No8.CmdBrain.CommandLine;

class ArgsCommandMeta
{
    public Type                    CommandType { get; }
    public ArgsCommandAttribute    CommandAttr { get; }
    public List<ArgsParameterMeta> Parameters  { get; } = new();

    public ArgsCommandMeta(Type type)
    {
        CommandType = type;
        var attr = CommandType.GetCustomAttribute<ArgsCommandAttribute>() 
                   ?? new ArgsCommandAttribute();
        CommandAttr             =   attr;
        CommandAttr.Name        ??= CommandType.Name;
        CommandAttr.Description ??= CommandType.GetCustomAttribute<DescriptionAttribute>()?.Description;
    }

    public string Name => CommandAttr.Name!;
}