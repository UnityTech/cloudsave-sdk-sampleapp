using System;

namespace UnitySocial
{
namespace Tools
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    internal class JsonPropertyAttribute : Attribute
    {
        public string fieldName { get; private set; }

        public JsonPropertyAttribute(string fieldName)
        {
            this.fieldName = fieldName;
        }
    }
}
}
