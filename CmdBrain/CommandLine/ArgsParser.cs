namespace No8.CmdBrain.CommandLine;

/// <summary>
/// Parses the metadata held in <see cref="T"/>
/// Each implementation will have a usage like:
///    CommandName [options] [extra arguments] 
///
/// example:
///   PLAN --verbose -language=pirate sea battle
/// what this means is:
///   PLAN               is the command verb
///   --verbose          will set the `verbose` boolean parameter to true
///   -language=pirate   will set the language parameter to `pirate`
///   sea battle         `sea`, `battle` are the extras
/// 
/// example:
///   BATTLE --intensity=insane
/// what this means is:
///   BATTLE              BATTLE is the command verb with no extras
///   --intensity=insane  Set the intensity parameter to `insane`
/// </summary>
public class ArgsParser
{
    public static ArgsParser  Builder => new();

    private readonly List<ArgsCommandMeta> _commands = new();
    private          ArgsCommandMeta?      _defaultCommand;
    private          bool                  _includeEnvironmentVariables;
    private          StringComparison      _stringComparison = StringComparison.Ordinal;

    public ArgsParser AddCommand<T>(bool isDefault = false)
        where T : IArgsCommand
    {
        var type = typeof(T);
        var meta = new ArgsCommandMeta(typeof(T));
        _commands.Add( meta );
        if (isDefault)
            _defaultCommand = meta;

        var properties = type.GetProperties();
        foreach (PropertyInfo propertyInfo in properties)
        {
            var paramAttr = propertyInfo.GetCustomAttribute<ArgsParameterAttribute>();
            if (paramAttr == null)
                continue;

            if (meta.Parameters.Any(p => p.Attr.Name?.Equals(paramAttr.Name, _stringComparison) == true))
                throw new ArgumentException($"Duplicate parameter name of {paramAttr.Name}");

            var metaParameter = new ArgsParameterMeta(paramAttr, propertyInfo);
            meta.Parameters.Add( metaParameter );
        }

        return this;
    }

    public ArgsParser SetIncludeEnvironmentVariables(bool include = true)
    {
        _includeEnvironmentVariables = include;
        return this;
    }

    public ArgsParser SetCaseSensitive(bool caseSensitive = true)
    {
        _stringComparison = caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        return this;
    }

    public IArgsCommand? Parse(string              commandLine)                           => Parse(commandLine.ParseArguments()!, out _);
    public IArgsCommand? Parse(string              commandLine, out List<string>? extras) => Parse(commandLine.ParseArguments()!, out extras);
    public IArgsCommand? Parse(IEnumerable<string> args) => Parse(args, out _);
    public IArgsCommand? Parse(IEnumerable<string> args, out List<string>? extras)
    {
        extras = null;
        var list = args.ToList();

        if (list.Count == 0)
            return null;

        if (_commands.Count == 1)
            _defaultCommand = _commands[0];

        var cmdName = list[0];
        var cmdMeta = _commands.FirstOrDefault(meta => meta.Name.Equals(cmdName, _stringComparison));
        if (cmdMeta == null && _defaultCommand != null)
            return Parse(list, out extras, _defaultCommand);
        if (cmdMeta != null)
            return Parse(list.Skip(1).ToArray(), out extras, cmdMeta);

        return new HelpCommand( Help() );
    }


    private IArgsCommand Parse(IEnumerable<string> args, out List<string>? extras, ArgsCommandMeta meta)
    {
        var parser = new ArgsCommandParser(meta, _includeEnvironmentVariables, _stringComparison);
        parser.Process(args);

        extras = parser.Extras;
        if (parser.AskedForHelp)
            return new HelpCommand( Help(meta), meta.CommandType );

        return parser.Result;
    }

    internal string Help(ArgsCommandMeta meta)
    {
        var sb = new StringBuilder();
        string names = string.Join('|', meta.CommandAttr.Names);

        sb.AppendLine($"{meta.Name}  -  {meta.CommandAttr.Description}");
        sb.AppendLine();
        sb.AppendLine($"Usage: {names} [options] [arguments]");

        var biggest = 0;
        if (meta.Parameters.Count > 0)
        {
            foreach (var parameterMeta in meta.Parameters)
            {
                string key = Type.GetTypeCode(parameterMeta.Info?.PropertyType) switch
                {
                    TypeCode.Boolean => string.Join('|', parameterMeta.Names),
                    _                => string.Join('|', parameterMeta.Names) + "=value"
                };
                biggest = Math.Max(key.Length, biggest);
            }
        }

        if (meta.Parameters.Count > 0)
        {
            sb.AppendLine();
            sb.AppendLine("Options");
            foreach (var property in meta.Parameters)
            {
                switch (Type.GetTypeCode(property.Info?.PropertyType))
                {
                case TypeCode.Boolean:
                    sb.AppendLine(
                        $"   --{string.Join('|', property.Names).PadRight(biggest)}  {property.Attr.Description ?? ""}");

                    break;
                default:
                    sb.AppendLine(
                        $"   --{(string.Join('|', property.Names) + "=value").PadRight(biggest)}  {property.Attr.Description ?? ""}");

                    break;
                }
            }
        }
        return sb.ToString();
    }

    public string Help()
    {
        if (_commands.Count == 1)
            return Help(_commands[0]);

        var sb      = new StringBuilder();
        var biggest = 0;

        foreach (var command in _commands)
        {
            var cmdName = string.Join('|', command.CommandAttr.Names);
            biggest = Math.Max(cmdName.Length, biggest);
        }

        sb.AppendLine("Usage: [command] [command-options] [arguments]");
        sb.AppendLine();
        sb.AppendLine("Commands");
        foreach (var command in _commands)
        {
            var cmdName = string.Join('|', command.CommandAttr.Names);
            var flag    = (command == _defaultCommand) ? "*" : " ";

            sb.AppendLine($"  {flag}{cmdName.PadRight(biggest)}  {(command.CommandAttr.Description ?? "")}");
        }

        return sb.ToString();
    }
}
