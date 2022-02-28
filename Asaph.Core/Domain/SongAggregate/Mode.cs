using FluentResults;

namespace Asaph.Core.Domain.SongAggregate;

/// <summary>
/// Mode value object.
/// </summary>
public record Mode
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Mode"/> class.
    /// </summary>
    /// <param name="name">Name.</param>
    /// <param name="number">Number.</param>
    private Mode(string name, int number) => (Name, Number) = (name, number);

    /// <summary>
    /// Major.
    /// </summary>
    public static Mode Major => new("major", 1);

    /// <summary>
    /// Minor.
    /// </summary>
    public static Mode Minor => new("minor", 6);

    /// <summary>
    /// Name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Number.
    /// </summary>
    public int Number { get; }

    /// <summary>
    /// Tries to get a mode by its name.
    /// </summary>
    /// <param name="name">Name.</param>
    /// <returns>The result of the attempt.</returns>
    public static Result<Mode> TryGetByName(string name)
    {
        if (name == Major.Name)
            return Result.Ok(Major);

        if (name == Minor.Name)
            return Result.Ok(Minor);

        return Result.Fail($"{name} is not a valid mode.");
    }
}
