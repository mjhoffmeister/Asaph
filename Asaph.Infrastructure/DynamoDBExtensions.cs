using Amazon.DynamoDBv2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Asaph.Infrastructure
{
    public static class DynamoDBExtensions
    {
        public static Dictionary<string, AttributeValue> ToAttributeValueDictionary(
            this object source,
            BindingFlags bindingAttr = 
                BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
        {
            return source.GetType().GetProperties(bindingAttr).ToDictionary
            (
                propInfo => propInfo.Name,
                propInfo => 
                {
                    object? value = propInfo.GetValue(source, null);
                    return value switch
                    {
                        bool @bool => new AttributeValue { BOOL = @bool },
                        string @string => new AttributeValue { S = @string },
                        _ => throw PropertyTypeNotSupportedException(propInfo.Name)
                    };
                }
            );
        }

        private static ArgumentException PropertyTypeNotSupportedException(string propertyName) =>
            new($"The type of property {propertyName} either doesn't have an AttributeValue " +
                "conversion implemented or is unsupported by DynamoDB.");
    }
}
