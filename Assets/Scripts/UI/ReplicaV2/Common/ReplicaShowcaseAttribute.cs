using System;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class ReplicaShowcaseAttribute : Attribute
{
    public string Group { get; }
    public string Name { get; }

    public ReplicaShowcaseAttribute(string group, string name)
    {
        Group = group;
        Name = name;
    }
}
