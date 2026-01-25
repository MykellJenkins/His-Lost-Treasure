using UnityEngine;
using System;

public class DebugCommands 
{
    private string PcommandId;
    private string PcommandDescription;
    private string PcommandFormat;

    public string commandId { get { return PcommandId;  } }
    public string commandDescription { get { return PcommandDescription; } }
    public string commandFormat { get { return PcommandFormat; } }

    public DebugCommands(string id, string description, string format)
    {
        PcommandId = id;
        PcommandDescription = description;
        PcommandFormat = format;
    }
}

public class DebugCommand : DebugCommands
{
    private Action command;
    public DebugCommand(string id, string description, string format, Action command) : base (id, description, format)
    {
        this.command = command;
    }

    public void Invoke()
    {
        command.Invoke();
    }
}

public class DebugCommand<T1> : DebugCommands
{
    private Action<T1> command;

    public DebugCommand(string id, string description, string format, Action<T1> command) : base(id, description, format)
    {
        this.command = command;
    }

    public void Invoke(T1 value)
    {
        command.Invoke(value);
    }
}

