using System.Collections.Generic;
using System.Linq;
using FluentResults;

namespace Asaph.Core.Domain.SongAggregate
{
    public record Note
    {
        public static Note AFlat => new("A♭", 8);

        public static Note A => new("A", 9);

        public static Note ASharp => new("A♯", 10);

        public static Note BFlat => new("B♭", 11);

        public static Note B => new("B", 12);

        public static Note C => new("C", 0);

        public static Note CSharp => new("C♯", 1);

        public static Note DFlat => new("D♭", 1);

        public static Note D => new("D", 2);

        public static Note DSharp => new("D♯", 3);

        public static Note EFlat => new("E♭", 3);

        public static Note E => new("E", 4);

        public static Note F => new("F", 5);

        public static Note FSharp => new("F♯", 6);

        public static Note GFlat => new("G♭", 6);

        public static Note G => new("G", 7);

        public static Note GSharp => new("G♯", 8);

        /// <summary>
        /// Creates a new note.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="number">Number.</param>
        private Note(string name, int number) => (Name, Number) = (name, number);

        public string Name { get; }

        public int Number { get; }

        public static IEnumerable<Note> Enumerate()
        {
            yield return C;
            yield return CSharp;
            yield return DFlat;
            yield return D;
            yield return DSharp;
            yield return EFlat;
            yield return E;
            yield return F;
            yield return FSharp;
            yield return GFlat;
            yield return G;
            yield return GSharp;
            yield return AFlat;
            yield return A;
            yield return ASharp;
            yield return B;
        }

        public override string ToString() => Name;

        public static Result<Note> TryGetByName(string name)
        {
            Note? note = Enumerate().SingleOrDefault(n => n.Name == name);

            if (note == null)
                return Result.Fail("Invalid note name.");

            return note.ToResult();
        }
    }
}
