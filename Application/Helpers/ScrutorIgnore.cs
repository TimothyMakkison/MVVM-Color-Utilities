using System;

namespace Application.Helpers
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ScrutorIgnoreAttribute : Attribute
    {
    }
}