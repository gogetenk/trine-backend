using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Assistance.Operational.Dal.MongoImpl.Entities;
using Assistance.Operational.WebApi.IntegrationTests.ApplicationFactory;
using AutoFixture;
using Dto.Responses;
using FluentAssertions;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace Assistance.Operational.WebApi.IntegrationTests.Resources
{
    public class DashboardResourceTests : ResourceTestBase<TrineFactory, Startup>
    {
        public const string DashboardEndpoint = "dashboards";
        private const string _TestUserId = "5ed4ca38d3568c799c82db16";
        private static MongoClient _mongoClient;
        private User _testUser;

        public DashboardResourceTests(TrineFactory factory, ITestOutputHelper helper) : base(factory, x => x.Output = helper)
        {
            // We instantiate the mongoclient only once for all the tests
            if (_mongoClient is null)
                _mongoClient = new MongoClient(Factory.Configuration["DatabaseConfiguration:ConnectionString"]);
        }

        public override async Task InitializeAsync()
        {
            // Generating a guid for collection names so it will be easier to clean testing data
            Factory.ActivitiesCollectionName = Guid.NewGuid().ToString();
            Factory.UsersCollectionName = Guid.NewGuid().ToString();

            // Creating the test user
            await CreateTestUser();
        }

        public override async Task DisposeAsync()
        {
            // Deleting test collection to keep data consistency
            var db = _mongoClient.GetDatabase(Factory.Configuration["DatabaseConfiguration:DatabaseName"]);
            await db.DropCollectionAsync(Factory.ActivitiesCollectionName);
            await db.DropCollectionAsync(Factory.UsersCollectionName);
        }

        #region Tests

        [Fact]
        public async Task GetDashboardData_NoActivities_NominalCase()
        {
            // Arrange
            var uri = $"api/{DashboardEndpoint}";

            // Act
            var response = await Client.GetAsync(uri);

            // Assert
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var json = await response.Content.ReadAsStringAsync();
            var respObject = JsonConvert.DeserializeObject<DashboardResponseDto>(json);
            respObject.Should().NotBeNull();
            respObject.Activities.Should().BeNullOrEmpty();
        }

        [Fact]
        public async Task GetDashboardData_WithActivities_NominalCase()
        {
            // Arrange
            var uri = $"api/{DashboardEndpoint}";
            var activity = new Fixture().Build<Activity>().With(x => x.Id, ObjectId.GenerateNewId()).Create();
            activity.Customer = null;
            activity.Commercial = null;
            activity.Consultant.Id = _TestUserId; // Have to be the consultant to share
            activity.CreationDate = DateTime.UtcNow;
            activity.EndDate = DateTime.UtcNow;
            activity.StartDate = DateTime.UtcNow;

            var db = _mongoClient.GetDatabase(Factory.Configuration["DatabaseConfiguration:DatabaseName"]);
            var collection = db.GetCollection<Activity>(Factory.ActivitiesCollectionName);
            await collection.InsertOneAsync(activity);

            // Act
            var response = await Client.GetAsync(uri);

            // Assert
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var json = await response.Content.ReadAsStringAsync();
            var respObject = JsonConvert.DeserializeObject<DashboardResponseDto>(json);
            respObject.Should().NotBeNull();
            respObject.Activities.First().Id.Should().BeEquivalentTo(activity.Id.ToString());
        }

        #endregion

        #region Private

        private async Task CreateTestUser()
        {
            _testUser = new Fixture().Build<User>().With(x => x.Id, new ObjectId(_TestUserId)).Create();
            var db = _mongoClient.GetDatabase(Factory.Configuration["DatabaseConfiguration:DatabaseName"]);
            var collection = db.GetCollection<User>(Factory.UsersCollectionName);
            await collection.InsertOneAsync(_testUser);
        }

        #endregion
    }
}
