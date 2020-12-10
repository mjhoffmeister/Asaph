using Asaph.Core.Domain.SongDirectorAggregate;
using Asaph.Core.Interfaces;
using Asaph.Core.UseCases.AddSongDirector;
using Moq;
using System.Linq;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow.Assist.ValueRetrievers;
using Xunit;

namespace Asaph.Core.Specs.Steps
{
    [Binding]
    public class AddSongDirectorStepDefinitions
    {
        private string? _adderRank;
        private string? _emailAddress;
        private string? _fullName;
        private string? _message;
        private string? _phoneNumber;
        private string? _rankName;

        private readonly ScenarioContext _scenarioContext;

        public AddSongDirectorStepDefinitions(ScenarioContext scenarioContext) =>
            _scenarioContext = scenarioContext;

        [Given(@"the person trying to add a song director is a (.*)")]
        public void GivenThePersonTryingToAddASongDirectorIsA(string adderRank)
        {
            _adderRank = adderRank;
        }

        [Given(@"the new song director email address is (.*)")]
        public void GivenTheNewSongDirectorEmailAddressIs(string? emailAddress)
        {
            _emailAddress = emailAddress.TranslateNull();
        }

        [Given(@"the new song director full name is (.*)")]
        public void GivenTheNewSongDirectorFullNameIs(string? fullName)
        {
            _fullName = fullName.TranslateNull();
        }
        
        [Given(@"the new song director phone number is (.*)")]
        public void GivenTheNewSongDirectorPhoneNumberIs(string? phoneNumber)
        {
            _phoneNumber = phoneNumber.TranslateNull();
        }
        
        [Given(@"the new song director rank is (.*)")]
        public void GivenTheNewSongDirectorRankIs(string? rankName)
        {
            _rankName = rankName.TranslateNull();
        }
        
        [When(@"the song director add is handled")]
        public async Task WhenTheSongDirectorAddIsHandled()
        {
            // Create an add song director request
            // Note: the requester username is mapped to "adderRank" in the mocked song director repository
            AddSongDirectorRequest addSongDirectorRequest = new("john.doe@email.com", _fullName, _emailAddress, _phoneNumber, _rankName);

            // Get an interactor
            AddSongDirectorInteractor<AddSongDirectorResponse> addSongDirectorInteractor = GetSongDirectorInteractor();

            // Handle the request
            var addSongDirectorResponse = await addSongDirectorInteractor.HandleAsync(addSongDirectorRequest);

            // Set the message
            _message = addSongDirectorResponse.Messages.Single();
        }
        
        [Then(@"the result message should be (.*)")]
        public void ThenTheResultMessageShouldBe(string message) => Assert.Equal(message, _message);

        private AddSongDirectorInteractor<AddSongDirectorResponse> GetSongDirectorInteractor()
        {
            // Get a song director repository mock
            IAsyncSongDirectorRepository songDirectorRepositoryMock = GetSongDirectorRepositoryMock();

            // Create a default boundary
            AddSongDirectorDefaultBoundary addSongDirectorDefaultBoundary = new();

            // Create the interactor
            AddSongDirectorInteractor<AddSongDirectorResponse> addSongDirectorInteractor = new(
                songDirectorRepositoryMock, addSongDirectorDefaultBoundary);

            // Return the interactor
            return addSongDirectorInteractor;
        }

        /// <summary>
        /// Gets a song director repository mock.
        /// </summary>
        /// <returns>The mocked repository.</returns>
        private IAsyncSongDirectorRepository GetSongDirectorRepositoryMock()
        {
            // Get a Rank object from the adder rank name
            var adderRank = Rank.TryGetByName(_adderRank).Value;

            // Set up the FindRankAsync method
            var songDirectorRepositoryMock = new Mock<IAsyncSongDirectorRepository>();
            songDirectorRepositoryMock
                .Setup(r => r.FindRankAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(adderRank));

            // Return the mocked object
            return songDirectorRepositoryMock.Object;
        }
        
    }
}
