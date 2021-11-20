using Amazon.DynamoDBv2.Model;
using Asaph.Infrastructure.SongDirectorRepository;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Asaph.Infrastructure
{
    /// <summary>
    /// Provides extensions for Dynamo DB types.
    /// </summary>
    public static class DynamoDBExtensions
    {
        /// <summary>
        /// Converts an object into a dictionary of <see cref="AttributeValue"/>s.
        /// </summary>
        /// <param name="source">Source object.</param>
        /// <param name="bindingAttr">Binding attributes.</param>
        /// <returns>The conversion result.</returns>
        public static Dictionary<string, AttributeValue> ToAttributeValueDictionary(
            this object source,
            BindingFlags bindingAttr =
                BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
        {
            return source.GetType().GetProperties(bindingAttr).ToDictionary(
                propInfo => propInfo.Name,
                propInfo =>
                {
                    object? value = propInfo.GetValue(source, null);
                    return value switch
                    {
                        bool @bool => new AttributeValue { BOOL = @bool },
                        string @string => new AttributeValue { S = @string },
                        _ => throw PropertyTypeNotSupportedException(propInfo.Name),
                    };
                });
        }

        /// <summary>
        /// Converts an <see cref="AttributeValue"/> dictionary to a
        /// <see cref="SongDirectorDataModel"/>.
        /// </summary>
        /// <param name="attributeValueDictionary">The dictionary to convert.</param>
        /// <returns>The converted value.</returns>
        public static SongDirectorDataModel ToSongDirectorDataModel(
            this Dictionary<string, AttributeValue> attributeValueDictionary)
        {
            attributeValueDictionary.TryGetValue(
                nameof(SongDirectorDataModel.Id), out AttributeValue? idAttributeValue);

            attributeValueDictionary.TryGetValue(
                nameof(SongDirectorDataModel.IsActive), out AttributeValue? isActiveAttributeValue);

            bool? isActive = null;

            if (isActiveAttributeValue != null)
                isActive = isActiveAttributeValue.N == "1";

            return new(
                idAttributeValue?.S ?? null,
                null,
                null,
                null,
                null,
                isActive);
        }

        /// <summary>
        /// Tries to get a value of the provided type from the <see cref="AttributeValue"/>.
        /// </summary>
        /// <typeparam name="TValue">Value type.</typeparam>
        /// <param name="attributeValue">Attribute value.</param>
        /// <returns>The result of the attempt.</returns>
        public static Result<TValue> TryGetValue<TValue>(this AttributeValue attributeValue)
        {
            if (attributeValue.S is TValue stringValue)
                return Result.Ok(stringValue);

            if (attributeValue.BOOL is TValue boolValue)
                return Result.Ok(boolValue);

            return Result.Fail($"{typeof(TValue)} either doesn't have an AttributeValue " +
                "conversion implemented or is unsupported by DynamoDB.");
        }

        private static ArgumentException PropertyTypeNotSupportedException(string propertyName) =>
            new($"The type of property {propertyName} either doesn't have an AttributeValue " +
                "conversion implemented or is unsupported by DynamoDB.");
    }
}
