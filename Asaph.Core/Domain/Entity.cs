namespace Asaph.Core.Domain
{
    public abstract class Entity
    {
        public string? Id { get; private set; }

        public void UpdateId(string? id) => Id = id;
    }
}
