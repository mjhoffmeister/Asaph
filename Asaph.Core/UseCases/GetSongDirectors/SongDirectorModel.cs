using Asaph.Core.Domain.SongDirectorAggregate;

namespace Asaph.Core.UseCases.GetSongDirectors
{
    public class SongDirectorModel
    {
        public SongDirectorModel(
            string name,
            string emailAddress,
            string? phoneNumber,
            bool isActive,
            string? rank,
            bool isDeletable,
            bool isEditable)
        {
            EmailAddress = emailAddress;
            IsActive = isActive;
            IsDeletable = isDeletable;
            IsEditable = isEditable;
            Name = name;
            PhoneNumber = phoneNumber;
            Rank = rank;
        }

        public string EmailAddress { get; }

        public bool IsActive { get; }

        public bool IsDeletable { get; }

        public bool IsEditable { get; }

        public string Name { get; }

        public string? PhoneNumber { get; }

        public string? Rank { get; }

        public static SongDirectorModel ReadOnly(SongDirector songDirector)
        {
            return new(
                songDirector.FullName,
                songDirector.EmailAddress,
                songDirector.PhoneNumber,
                songDirector.IsActive,
                rank: null,
                isDeletable: false,
                isEditable: false);
        }

        public static SongDirectorModel Self(SongDirector songDirector, Rank? rank)
        {
            bool isGrandmaster = rank == Domain.SongDirectorAggregate.Rank.Grandmaster;

            return new(
                songDirector.FullName,
                songDirector.EmailAddress,
                songDirector.PhoneNumber,
                songDirector.IsActive,
                rank: isGrandmaster ? songDirector.Rank?.Name : null,
                isDeletable: false,
                isEditable: true);
        }

        public static SongDirectorModel GrandmasterView(SongDirector songDirector)
        {
            return new(
                songDirector.FullName,
                songDirector.EmailAddress,
                songDirector.PhoneNumber,
                songDirector.IsActive,
                songDirector.Rank?.Name,
                isDeletable: true,
                isEditable: true);
        }
    }
}
