using FluentResults;
using System.Collections.Generic;
using System.Linq;

namespace Asaph.Core.Domain.SongAggregate
{
    /// <summary>
    /// Note value object.
    /// </summary>
    public record Note
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Note"/> class.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="number">Number.</param>
        private Note(string name, int number) => (Name, Number) = (name, number);

        /// <summary>
        /// A♭.
        /// </summary>
        public static Note AFlat => new("A♭", 8);

        /// <summary>
        /// A.
        /// </summary>
        public static Note A => new("A", 9);

        /// <summary>
        /// A♯.
        /// </summary>
        public static Note ASharp => new("A♯", 10);

        /// <summary>
        /// B♭.
        /// </summary>
        public static Note BFlat => new("B♭", 11);

        /// <summary>
        /// B.
        /// </summary>
        public static Note B => new("B", 12);

        /// <summary>
        /// C.
        /// </summary>
        public static Note C => new("C", 0);

        /// <summary>
        /// C♯.
        /// </summary>
        public static Note CSharp => new("C♯", 1);

        /// <summary>
        /// D♭.
        /// </summary>
        public static Note DFlat => new("D♭", 1);

        /// <summary>
        /// D.
        /// </summary>
        public static Note D => new("D", 2);

        /// <summary>
        /// D♯.
        /// </summary>
        public static Note DSharp => new("D♯", 3);

        /// <summary>
        /// E♭.
        /// </summary>
        public static Note EFlat => new("E♭", 3);

        /// <summary>
        /// E.
        /// </summary>
        public static Note E => new("E", 4);

        /// <summary>
        /// F.
        /// </summary>
        public static Note F => new("F", 5);

        /// <summary>
        /// F♯.
        /// </summary>
        public static Note FSharp => new("F♯", 6);

        /// <summary>
        /// G♭.
        /// </summary>
        public static Note GFlat => new("G♭", 6);

        /// <summary>
        /// G.
        /// </summary>
        public static Note G => new("G", 7);

        /// <summary>
        /// G♯.
        /// </summary>
        public static Note GSharp => new("G♯", 8);

        /// <summary>
        /// Name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Number.
        /// </summary>
        public int Number { get; }

        /// <summary>
        /// Enumerates notes.
        /// </summary>
        /// <returns>Notes.</returns>
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

        /// <inheritdoc />
        public override string ToString() => Name;

        /// <summary>
        /// Tries to get a note by name.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <returns>The result of the attempt.</returns>
        public static Result<Note> TryGetByName(string name)
        {
            Note? note = Enumerate().SingleOrDefault(n => n.Name == name);

            if (note == null)
                return Result.Fail("Invalid note name.");

            return note.ToResult();
        }
    }
}
