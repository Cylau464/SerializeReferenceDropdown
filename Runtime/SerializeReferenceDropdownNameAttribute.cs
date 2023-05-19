using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Class)]
public class SerializeReferenceDropdownNameAttribute : PropertyAttribute
{
    public readonly string Name;
    public readonly string Path;

    //public SerializeReferenceDropdownNameAttribute(string name)
    //{
    //    Name = name;
    //}
    public SerializeReferenceDropdownNameAttribute(string name, string path = "")
    {
        Name = name;
        Path = path;
    }
}

