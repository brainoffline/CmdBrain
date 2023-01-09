using System.Text.RegularExpressions;

namespace No8.CmdBrain.CommandLine;

internal class ArgsCommandParser
{
    private readonly ArgsCommandMeta  _meta;
    private          string?          _parameterName;
    private          string?          _parameterValue;
    private readonly bool             _includeEnvironmentVariables;
    private readonly StringComparison _stringComparison;

    public IArgsCommand  Result       { get; private set; }
    public bool          AskedForHelp { get; private set; }
    public List<string>? Extras       { get; private set; }

    /// <summary>
    /// [flag][name][separator][value]
    /// flag      = --, -, /
    /// name      = name of parameter
    /// separator = :, = (optional)
    /// value     = value of parameter (optional) 
    /// </summary>
    private static readonly Regex FlagRegex = new(
        @"^(?<flag>--|-|/)(?<name>[^:=]+)((?<separator>[:=])(?<value>.*))?$");

    public ArgsCommandParser(ArgsCommandMeta meta, bool includeEnvironmentVariables, StringComparison stringComparison)
    {
        _meta                        = meta;
        _includeEnvironmentVariables = includeEnvironmentVariables;
        _stringComparison            = stringComparison;
        Result                       = (IArgsCommand)Activator.CreateInstance(meta.CommandType)!;
    }

    public void Process(IEnumerable<string> args)
    {
        Result = (IArgsCommand)Activator.CreateInstance(_meta.CommandType)!;
        AskedForHelp = false;
        Extras       = null;

        foreach (var arg in args)
        {
            Parse(arg);
            if (AskedForHelp)
                return;
        }

        if (_includeEnvironmentVariables)
            ParseEnvironmentVariables();
    }

    private void Parse(string arg)
    {
        // have we already named the parameter name
        if (_parameterName?.HasValue() ?? false)
        {
            if (!SetParam(_parameterName, arg))
                throw new ArgumentException(
                    $"Unable to set parameter [{_parameterName}] with value [{arg}]");

            _parameterName = null;
            return;
        }

        _parameterName  = null;
        _parameterValue = null;

        var match = FlagRegex.Match(arg);
        if (!match.Success)
        {
            var defaultParam = _meta.Parameters.FirstOrDefault(p => p.IsDefault);

            if (defaultParam != null)
            {
                if (!SetParam(defaultParam.Name, arg))
                    throw new ArgumentException(
                        $"Unable to set parameter [{defaultParam.Name}] with value [{arg}]");
                return;
            }

            Extras ??= new();
            Extras.Add(arg);
            return;
        }

        _parameterName = match.Groups["name"].Value;

        var separatorGroup = match.Groups["separator"];
        var valueGroup = match.Groups["value"];
        if (separatorGroup.Success && valueGroup.Success)
        {
            //context.ParameterSeparator = separatorGroup.Value;
            _parameterValue = valueGroup.Value;

            if (!SetParam(_parameterName, _parameterValue))
                throw new ArgumentException(
                    $"Unable to set parameter [{_parameterName}] with value [{_parameterValue}]");
            _parameterName = null;
        }
        else if (_parameterName.Equals("help", _stringComparison) ||
                 _parameterName.Equals("?"))
        {
            AskedForHelp = true;
        }
        else if (IsBooleanParameter(_parameterName))
        {
            SetParam(_parameterName, "true");
            _parameterName  = null;
            _parameterValue = null;
        }
    }

    private bool IsBooleanParameter(string parameterName)
    {
        var propertyMetadata = _meta.Parameters.FirstOrDefault(p => p.Names.Contains(parameterName));

        if (propertyMetadata == null)
            throw new ArgumentException($"Parameter [{parameterName}] has not been defined");

        var property = propertyMetadata.Info;
        var tt = property?.PropertyType;

        // Get the type code so we can switch
        var typeCode = Type.GetTypeCode(tt);

        return typeCode == TypeCode.Boolean;
    }
    private bool SetParam(string parameterName, string value)
    {
        var propertyMetadata = _meta.Parameters.FirstOrDefault(p => p.Names.ContainsWithComparison(parameterName, _stringComparison));
        if (propertyMetadata == null)
            throw new ArgumentException($"Parameter [{parameterName}] has not been defined");

        var property = propertyMetadata.Info;
        if (property == null)
            throw new ArgumentException($"Unknown parameter [{parameterName}]");

        var tt = property.PropertyType;
        if (tt.IsEnum)
        {
            var e = Enum.Parse(tt, value, _stringComparison == StringComparison.OrdinalIgnoreCase);
            property.SetValue(Result, e);

            return true;
        }

        // Get the type code so we can switch
        var typeCode = Type.GetTypeCode(tt);
        switch (typeCode)
        {
            case TypeCode.Boolean:
            case TypeCode.Char:
            case TypeCode.SByte:
            case TypeCode.Byte:
            case TypeCode.Int16:
            case TypeCode.UInt16:
            case TypeCode.Int32:
            case TypeCode.UInt32:
            case TypeCode.Int64:
            case TypeCode.UInt64:
            case TypeCode.Single:
            case TypeCode.Double:
            case TypeCode.Decimal:
            case TypeCode.DateTime:
            case TypeCode.String:
                property.SetValue(Result, Convert.ChangeType(value, typeCode));

                return true;
        }

        if (tt.IsGenericType)
        {
            // This currently works for List<T>

            var currentValue = property.GetValue(Result);
            if (currentValue == null)
            {
                currentValue = Activator.CreateInstance(tt);
                property.SetValue(Result, currentValue);
            }

            var itemType = tt.GetGenericArguments()[0];
            var itemValue = Convert.ChangeType(value, itemType);

            tt.GetMethod("Add")
             ?.Invoke(currentValue, new[] { itemValue });

            return true;
        }

        throw new ArgumentException($"Don't know how to set parameter [{parameterName}] with value [{value}]");
    }

    private void ParseEnvironmentVariables()
    {
        var vars = Environment.GetEnvironmentVariables();

        foreach (var property in _meta.Parameters)
        {
            foreach (var name in property.Names)
            {
                if (vars.ContainsWithComparison(name, _stringComparison))
                {
                    var value = vars[name]?.ToString() ?? "";
                    SetParam(name, value);
                }
            }
        }
    }
}