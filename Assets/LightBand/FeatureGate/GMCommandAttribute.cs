using System;

[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public sealed class GMCommandAttribute : Attribute
{
    public string Command { get; }
    public string Group { get; }
    public string Description { get; }

    public GMCommandAttribute(string command, string group = "", string description = "")
    {
        this.Command = command;
        this.Group = group;
        this.Description = description;
    }
}
