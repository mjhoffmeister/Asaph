namespace Asaph.Core.Domain;

/// <summary>
/// Entity base class.
/// </summary>
public abstract class Entity
{
    /// <summary>
    /// Id.
    /// </summary>
    public string? Id { get; private set; }

    /// <summary>
    /// Updates the identity's id.
    /// </summary>
    /// <param name="id">Id.</param>
    public void UpdateId(string? id) => Id = id;
}
