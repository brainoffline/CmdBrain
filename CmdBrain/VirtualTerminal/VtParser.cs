namespace No8.CmdBrain.VirtualTerminal;

public record VtCommand
( VtAction     Action,
  char         Ch,
  List< char >? IntermediateChars,
  List< int >?  Parameters )
{
    public string Pattern
    {
        get
        {
            var sb = new StringBuilder();
            switch (Action)
            {
            case VtAction.CsiDispatch:
                sb.Append( "\\E[" );
                break;
            case VtAction.EscDispatch:
                sb.Append("\\E");
                break;

            default:
                break;
            }
            if (IntermediateChars?.Count > 0)
            {
                foreach (var ch in IntermediateChars)
                {
                    if (char.IsControl(ch))
                        sb.Append($"\\x{(int)ch:2X}");
                    else
                        sb.Append(ch);
                }
            }
            if (Parameters?.Count > 0)
            {
                for (int i = 0; i < Parameters.Count - 1; i++)
                    sb.Append( "%n;" );
                sb.Append("%n");
            }
            sb.Append( Ch );

            return sb.ToString();
        }
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append( $"{VtConst.ActionNames[ Action ]} " );

        if (IntermediateChars?.Count > 0)
        {
            sb.Append( ",\"" );
            foreach (var ch in IntermediateChars)
            {
                if (char.IsControl( ch ))
                    sb.Append( $"\\x{(int)ch:2X}" );
                else
                    sb.Append( ch );
            }
            sb.Append( '\"' );
        }
        if (Parameters?.Count > 0)
            sb.Append( $"\t" + string.Join( ',', Parameters ) );
        if (char.IsControl(Ch))
            sb.Append($" \\x{(int)Ch:2X}");
        else
            sb.Append($" '{Ch}'");

        return sb.ToString();
    }
}

public class VtParser
{
    public VtState      State;
    public List< char > IntermediateChars = new();
    public List< int >  Parameters        = new();

    public event EventHandler< VtCommand >? VtCommand;

    public VtParser() { State = VtState.Ground; }

    private void DoAction( VtAction action, char ch = (char)0 )
    {
        switch (action)
        {
        case VtAction.Print:
        case VtAction.Execute:
        case VtAction.Hook:
        case VtAction.Put:
        case VtAction.OscStart:
        case VtAction.OscPut:
        case VtAction.OscEnd:
        case VtAction.Unhook:
        case VtAction.CsiDispatch:
        case VtAction.EscDispatch:
            RaiseVtCommand( new VtCommand( action, ch, IntermediateChars.ToList(), Parameters.ToList() ) );
            Clear();

            break;

        case VtAction.Ignore:
            break;

        case VtAction.Collect:
        {
            IntermediateChars.Add( ch );

            break;
        }

        case VtAction.Param:
        {
            // process the parameter character
            if (ch == ';')
            {
                Parameters.Add( 0 );
            }
            else
            {
                // the character is a digit
                if (Parameters.Count == 0)
                    Parameters.Add( 0 );

                var currentParam = Parameters.Count - 1;
                Parameters[ currentParam ] *= 10;
                Parameters[ currentParam ] += ( ch - '0' );
            }

            break;
        }

        case VtAction.Clear:
            Clear();

            break;

        default:
            RaiseVtCommand( new VtCommand( VtAction.Error, (char)0, default, default ) );
            Clear();

            break;
        }
    }

    private void Clear()
    {
        IntermediateChars.Clear();
        Parameters.Clear();
    }

    private void DoStateChange( StateCell change, char ch = (char)0 )
    {
        // A state change is an action and/or a new state to transition to
        VtState  newState = change.State;
        VtAction action   = change.Action;

        if (newState != 0)
        {
            //Perform up to three actions:
            //1. the exit action of the old state
            //2. the action associated with the transition
            //3. the entry action of the new state


            VtAction exitAction  = VtConst.ExitActions[ (int)State     - 1 ];
            VtAction entryAction = VtConst.EntryActions[ (int)newState - 1 ];

            if (exitAction != 0)
                DoAction( exitAction );

            if (action != 0)
                DoAction( action, ch );

            if (entryAction != 0)
                DoAction( entryAction );

            State = newState;
        }
        else
            DoAction( action, ch );
    }

    public void Parse( char ch )
    {
        var change = VtConst.StateTable[(int)State - 1].FirstOrDefault(t => t.Ch == ch);
        if (change != null)
            DoStateChange(change, ch);
    }

    public void Parse( IEnumerable< char > data )
    {
        int i;
        var array = data.ToArray();
        for (i = 0; i < array.Length; i++)
        {
            Parse(array[ i ]);
        }
    }

    protected virtual void RaiseVtCommand( VtCommand cmd ) { VtCommand?.Invoke( this, cmd ); }
}
