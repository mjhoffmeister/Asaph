using FluentResults;

namespace Asaph.Core.Domain;

/// <summary>
/// Used to indicate that a property value change failed because its value didn't change.
/// </summary>
public class UnchangedPropertyValueError : Error
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnchangedPropertyValueError"/> class.
    /// </summary>
    /// <param name="propertyName">Property name.</param>
    public UnchangedPropertyValueError(string propertyName)
        : base($"{propertyName} is unchanged")
    {
    }
}
