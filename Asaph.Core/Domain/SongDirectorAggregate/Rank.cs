using FluentResults;
using System.Collections.Generic;
using System.Linq;

namespace Asaph.Core.Domain.SongDirectorAggregate
{
    public sealed record Rank
    {
        public static Rank Apprentice => new("Apprentice", 1);

        public static Rank Journeyer => new("Journeyer", 2);

        public static Rank Master => new("Master", 3);

        public static Rank Grandmaster => new("Grandmaster", 4);

        private Rank(string name, int number) => (Name, Number) = (name, number);

        /// <summary>
        /// Name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Number.
        /// </summary>
        public int Number { get; }

        /// <summary>
        /// Enumberates ranks.
        /// </summary>
        /// <returns>A collection of all ranks.</returns>
        public static IEnumerable<Rank> Enumerate()
        {
            yield return Apprentice;
            yield return Journeyer;
            yield return Master;
            yield return Grandmaster;
        }

        /// <summary>
        /// Tries to get a rank by name.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <returns>The result of the attempt.</returns>
        public static Result<Rank?> TryGetByName(string? name)
        {
            if (name == null)
                return Result.Ok();

            Rank? rank = Enumerate().SingleOrDefault(r => r.Name == name);

            if (rank == null)
                return Result.Fail("Invalid rank.");

            return Result.Ok<Rank?>(rank);
        }

        /// <summary>
        /// Tries to get the rank that comes after this one.
        /// </summary>
        /// <returns>The result of the attempt.</returns>
        public Result<Rank> TryGetNext()
        {
            Rank? nextRank = Enumerate().SingleOrDefault(r => r.Number == Number + 1);

            if (nextRank == null)
                return Result.Fail($"There is no rank after {Name}.");

            return Result.Ok(nextRank);
        }

        /// <summary>
        /// Tries to get the rank that comes before this one.
        /// </summary>
        /// <returns>The result of the attempt.</returns>
        public Result<Rank> TryGetPrevious()
        {
            Rank? previousRank = Enumerate().SingleOrDefault(r => r.Number == Number - 1);

            if (previousRank == null)
                return Result.Fail($"There is no rank before {Name}.");

            return Result.Ok(previousRank);
        }
    }
}
