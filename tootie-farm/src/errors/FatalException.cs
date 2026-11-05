using System;
using System.Diagnostics;
using Godot;
public class FatalException : Exception
{
    public FatalException(string message, Node context) : base(message)
    {
        Crash(message, context);
    }

    public static FatalException fromException(Exception innerException, Node context)
    {
        return new FatalException(innerException.Message, context);
    }

    private static void Crash(string message, Node context)
    {
        GD.PrintErr($"[FATAL CRASH]: {message}");
        // If an IDE debugger is attached, pause execution right here!
        if (Debugger.IsAttached)
        {
            Debugger.Break();
        }
        else if (OS.IsDebugBuild())
        {
            SceneTree tree = context?.GetTree() ?? (Engine.GetMainLoop() as SceneTree);
            tree.Quit(1);
        }
    }

    public static void ThrowIfNull(object? obj, string paramName, Node context)
    {
        if (obj == null)
        {
            throw new FatalException($"Required node/property '{paramName}' is missing on '{context.Name}' ({context.GetPath()})!", context);
        }
    }
}
