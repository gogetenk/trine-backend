using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Assistance.Operational.Bll.Impl.Builders;
using Assistance.Operational.Dal.MongoImpl.Entities;
using Assistance.Operational.WebApi.IntegrationTests.ApplicationFactory;
using AutoFixture;
using Dto;
using Dto.Requests;
using FluentAssertions;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;
using ActivityStatusEnum = Dto.ActivityStatusEnum;
using DayPartEnum = Assistance.Operational.Dal.MongoImpl.Entities.DayPartEnum;

namespace Assistance.Operational.WebApi.IntegrationTests.Resources
{
    public class ActivityResourceTests : ResourceTestBase<TrineFactory, Startup>
    {
        public const string ActivityEndpoint = "activities";
        private const string _TestUserId = "5ed4ca38d3568c799c82db16";
        private static MongoClient _mongoClient;
        private User _testUser;

        public ActivityResourceTests(TrineFactory factory, ITestOutputHelper helper) : base(factory, x => x.Output = helper)
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
        public async Task Generate_NominalCase_ShouldHaveUtcDates()
        {
            // Arrange
            var uri = $"api/{ActivityEndpoint}/generate";

            // Act
            var response = await Client.GetAsync(uri);

            // Assert
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var json = await response.Content.ReadAsStringAsync();
            var respObject = JsonConvert.DeserializeObject<ActivityDto>(json);
            respObject.Should().NotBeNull();
            respObject.CreationDate.Kind.Should().Be(DateTimeKind.Utc);
            respObject.StartDate.Kind.Should().Be(DateTimeKind.Utc);
            respObject.EndDate.Kind.Should().Be(DateTimeKind.Utc);
        }

        [Fact]
        public async Task OnboardingFirstSaveConsultant_NominalCase_Expect201()
        {
            // Arrange
            //Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");
            var uri = $"api/{ActivityEndpoint}/first";
            var activity = new Fixture().Create<ActivityDto>();
            var payload = new StringContent(JsonConvert.SerializeObject(activity), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PostAsync(uri, payload);

            // Assert
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var json = await response.Content.ReadAsStringAsync();
            var respObject = JsonConvert.DeserializeObject<ActivityDto>(json);
            respObject.Should().NotBeNull();
            respObject.CreationDate.Kind.Should().Be(DateTimeKind.Utc);
            respObject.StartDate.Kind.Should().Be(DateTimeKind.Utc);
            respObject.EndDate.Kind.Should().Be(DateTimeKind.Utc);
        }

        [Fact]
        public async Task Share_NominalCase_Expect200()
        {
            // Arrange
            var activity = new Fixture().Build<Activity>().With(x => x.Id, ObjectId.GenerateNewId()).Create();
            activity.Customer = null;
            activity.Commercial = null;
            activity.Consultant.Id = _TestUserId; // Have to be the consultant to share
            var uri = $"api/{ActivityEndpoint}/{activity.Id}/share";
            var db = _mongoClient.GetDatabase(Factory.Configuration["DatabaseConfiguration:DatabaseName"]);
            var collection = db.GetCollection<Activity>(Factory.ActivitiesCollectionName);
            await collection.InsertOneAsync(activity);

            var req = new ShareActivityRequestDto()
            {
                CustomerEmail = "customer@mail.com"
            };
            var payload = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PatchAsync(uri, payload);

            // Assert
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var json = await response.Content.ReadAsStringAsync();
            var respObject = JsonConvert.DeserializeObject<ActivityDto>(json);
            respObject.Should().NotBeNull();
            respObject.CreationDate.Kind.Should().Be(DateTimeKind.Utc);
            respObject.StartDate.Kind.Should().Be(DateTimeKind.Utc);
            respObject.EndDate.Kind.Should().Be(DateTimeKind.Utc);
            respObject.Customer.CanSign = true;
            respObject.Consultant.CanSign = false;
        }

        [Fact]
        public async Task GetByToken_NominalCase_Expect200()
        {
            // Arrange
            var activity = new Fixture().Build<Activity>().With(x => x.Id, ObjectId.GenerateNewId()).Create();

            // Create token
            long.TryParse(Factory.Configuration["AuthenticationSettings:TokenExpirationInSeconds"], out long exp);
            var token = new TokenBuilder()
                .WithExpiration(exp)
                .WithKey(Factory.Configuration["AuthenticationSettings:Key"])
                .WithEmailClaim(activity.Customer.Email)
                .WithActivityIdClaim(activity.Id.ToString())
                .GetToken();

            var uri = $"api/{ActivityEndpoint}/token/{token}";
            var db = _mongoClient.GetDatabase(Factory.Configuration["DatabaseConfiguration:DatabaseName"]);
            var collection = db.GetCollection<Activity>(Factory.ActivitiesCollectionName);
            await collection.InsertOneAsync(activity);

            // Act
            var response = await Client.GetAsync(uri);

            // Assert
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var json = await response.Content.ReadAsStringAsync();
            var respObject = JsonConvert.DeserializeObject<ActivityDto>(json);
            respObject.Should().NotBeNull();
        }

        [Fact]
        public async Task GetByToken_WhenEmailMismatch_Expect400()
        {
            // Arrange
            var activity = new Fixture().Build<Activity>().With(x => x.Id, ObjectId.GenerateNewId()).Create();

            // Create token
            long.TryParse(Factory.Configuration["AuthenticationSettings:TokenExpirationInSeconds"], out long exp);
            var token = new TokenBuilder()
                .WithExpiration(exp)
                .WithKey(Factory.Configuration["AuthenticationSettings:Key"])
                .WithEmailClaim("notsameemail@mail.com")
                .WithActivityIdClaim(activity.Id.ToString())
                .GetToken();

            var uri = $"api/{ActivityEndpoint}/token/{token}";
            var db = _mongoClient.GetDatabase(Factory.Configuration["DatabaseConfiguration:DatabaseName"]);
            var collection = db.GetCollection<Activity>(Factory.ActivitiesCollectionName);
            await collection.InsertOneAsync(activity);

            // Act
            var response = await Client.GetAsync(uri);

            // Assert
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Sign_AsConsultant_Expect200()
        {
            // Arrange
            var db = _mongoClient.GetDatabase(Factory.Configuration["DatabaseConfiguration:DatabaseName"]);
            var activity = new Fixture()
                .Build<Activity>()
                .With(x => x.Id, ObjectId.GenerateNewId())
                .With(x => x.Status, Dal.MongoImpl.Entities.ActivityStatusEnum.Generated)
                .Create();

            // Inserting customer user
            var customer = new Fixture()
                .Build<User>()
                .With(x => x.Id, ObjectId.GenerateNewId())
                .With(x => x.Email, "toto@titi.com")
                .Create();

            var userCollection = db.GetCollection<User>(Factory.UsersCollectionName);
            await userCollection.InsertOneAsync(customer);

            activity.Consultant.Id = _TestUserId;
            activity.Customer.Email = "toto@titi.com";
            activity.Customer.Id = customer.Id.ToString();

            var uri = $"api/{ActivityEndpoint}/{activity.Id}/sign";

            var activityCollection = db.GetCollection<Activity>(Factory.ActivitiesCollectionName);
            await activityCollection.InsertOneAsync(activity);

            // Act
            var response = await Client.PostAsync(uri, null);

            // Assert
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var json = await response.Content.ReadAsStringAsync();
            var respObject = JsonConvert.DeserializeObject<ActivityDto>(json);
            respObject.Should().NotBeNull();
            respObject.Status.Should().Be(ActivityStatusEnum.ConsultantSigned);
            respObject.Consultant.SignatureDate.Should().BeCloseTo(DateTime.UtcNow, 10000);
            respObject.Consultant.SignatureDate.Value.Kind.Should().Be(DateTimeKind.Utc);
        }

        [Fact]
        public async Task Sign_AsCustomer_Expect200()
        {
            // Arrange
            var db = _mongoClient.GetDatabase(Factory.Configuration["DatabaseConfiguration:DatabaseName"]);
            var activity = new Fixture()
                .Build<Activity>()
                .With(x => x.Id, ObjectId.GenerateNewId())
                .With(x => x.Status, Dal.MongoImpl.Entities.ActivityStatusEnum.ConsultantSigned)
                .Create();

            // Inserting consultant user
            var consultant = new Fixture()
                .Build<User>()
                .With(x => x.Id, ObjectId.GenerateNewId())
                .With(x => x.Email, "toto@titi.com")
                .Create();

            var userCollection = db.GetCollection<User>(Factory.UsersCollectionName);
            await userCollection.InsertOneAsync(consultant);

            activity.Consultant.Id = consultant.Id.ToString();
            activity.Customer.Email = "toto@titi.com";
            activity.Customer.Id = _TestUserId;

            var uri = $"api/{ActivityEndpoint}/{activity.Id}/sign";

            var activityCollection = db.GetCollection<Activity>(Factory.ActivitiesCollectionName);
            await activityCollection.InsertOneAsync(activity);

            // Act
            var response = await Client.PostAsync(uri, null);

            // Assert
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var json = await response.Content.ReadAsStringAsync();
            var respObject = JsonConvert.DeserializeObject<ActivityDto>(json);
            respObject.Should().NotBeNull();
            respObject.Status.Should().Be(ActivityStatusEnum.CustomerSigned);
            respObject.Customer.SignatureDate.Should().BeCloseTo(DateTime.UtcNow, 10000);
            respObject.Customer.SignatureDate.Value.Kind.Should().Be(DateTimeKind.Utc);
        }

        [Fact]
        public async Task Export_NominalCase()
        {
            // Arrange
            var json = "{\r\n  \"daysNumber\": 0,\r\n  \"creationDate\": \"2020-06-17T08:28:41.8380547Z\",\r\n  \"startDate\": \"2020-06-01T00:00:00Z\",\r\n  \"endDate\": \"2020-06-30T00:00:00Z\",\r\n   \"customer\": {\r\n      \"firstname\": \"toto\",\r\n      \"lastname\": \"customer\",\r\n      \"signatureDate\": \"2020-07-01T10:28:21.497Z\",\r\n      \"email\": \"string\"\r\n    },\r\n    \"consultant\": {\r\n       \"firstname\": \"titi\",\r\n      \"lastname\": \"consultant\",\r\n      \"signatureDate\": \"2020-07-01T10:28:21.497Z\",\r\n      \"email\": \"string\"\r\n    },\r\n  \"status\": 1,\r\n  \"days\": [\r\n    {\r\n      \"day\": \"2020-06-01T00:00:00Z\",\r\n      \"isOpen\": true,\r\n      \"workedPart\": 0\r\n    },\r\n    {\r\n      \"day\": \"2020-06-02T00:00:00Z\",\r\n      \"isOpen\": true,\r\n      \"workedPart\": 0\r\n    },\r\n    {\r\n      \"day\": \"2020-06-03T00:00:00Z\",\r\n      \"isOpen\": true,\r\n      \"workedPart\": 0\r\n    },\r\n    {\r\n      \"day\": \"2020-06-04T00:00:00Z\",\r\n      \"isOpen\": true,\r\n      \"workedPart\": 0\r\n    },\r\n    {\r\n      \"day\": \"2020-06-05T00:00:00Z\",\r\n      \"isOpen\": true,\r\n      \"workedPart\": 0\r\n    },\r\n    {\r\n      \"day\": \"2020-06-06T00:00:00Z\",\r\n      \"isOpen\": false,\r\n      \"workedPart\": 0\r\n    },\r\n    {\r\n      \"day\": \"2020-06-07T00:00:00Z\",\r\n      \"isOpen\": false,\r\n      \"workedPart\": 0\r\n    },\r\n    {\r\n      \"day\": \"2020-06-08T00:00:00Z\",\r\n      \"isOpen\": true,\r\n      \"workedPart\": 0\r\n    },\r\n    {\r\n      \"day\": \"2020-06-09T00:00:00Z\",\r\n      \"isOpen\": true,\r\n      \"workedPart\": 0\r\n    },\r\n    {\r\n      \"day\": \"2020-06-10T00:00:00Z\",\r\n      \"isOpen\": true,\r\n      \"workedPart\": 0\r\n    },\r\n    {\r\n      \"day\": \"2020-06-11T00:00:00Z\",\r\n      \"isOpen\": true,\r\n      \"workedPart\": 0\r\n    },\r\n    {\r\n      \"day\": \"2020-06-12T00:00:00Z\",\r\n      \"isOpen\": true,\r\n      \"workedPart\": 0\r\n    },\r\n    {\r\n      \"day\": \"2020-06-13T00:00:00Z\",\r\n      \"isOpen\": false,\r\n      \"workedPart\": 0\r\n    },\r\n    {\r\n      \"day\": \"2020-06-14T00:00:00Z\",\r\n      \"isOpen\": false,\r\n      \"workedPart\": 0\r\n    },\r\n    {\r\n      \"day\": \"2020-06-15T00:00:00Z\",\r\n      \"isOpen\": true,\r\n      \"workedPart\": 0\r\n    },\r\n    {\r\n      \"day\": \"2020-06-16T00:00:00Z\",\r\n      \"isOpen\": true,\r\n      \"workedPart\": 0\r\n    },\r\n    {\r\n      \"day\": \"2020-06-17T00:00:00Z\",\r\n      \"isOpen\": true,\r\n      \"workedPart\": 0\r\n    },\r\n    {\r\n      \"day\": \"2020-06-18T00:00:00Z\",\r\n      \"isOpen\": true,\r\n      \"workedPart\": 0\r\n    },\r\n    {\r\n      \"day\": \"2020-06-19T00:00:00Z\",\r\n      \"isOpen\": true,\r\n      \"workedPart\": 0\r\n    },\r\n    {\r\n      \"day\": \"2020-06-20T00:00:00Z\",\r\n      \"isOpen\": false,\r\n      \"workedPart\": 0\r\n    },\r\n    {\r\n      \"day\": \"2020-06-21T00:00:00Z\",\r\n      \"isOpen\": false,\r\n      \"workedPart\": 0\r\n    },\r\n    {\r\n      \"day\": \"2020-06-22T00:00:00Z\",\r\n      \"isOpen\": true,\r\n      \"workedPart\": 0\r\n    },\r\n    {\r\n      \"day\": \"2020-06-23T00:00:00Z\",\r\n      \"isOpen\": true,\r\n      \"workedPart\": 0\r\n    },\r\n    {\r\n      \"day\": \"2020-06-24T00:00:00Z\",\r\n      \"isOpen\": true,\r\n      \"workedPart\": 0\r\n    },\r\n    {\r\n      \"day\": \"2020-06-25T00:00:00Z\",\r\n      \"isOpen\": true,\r\n      \"workedPart\": 0\r\n    },\r\n    {\r\n      \"day\": \"2020-06-26T00:00:00Z\",\r\n      \"isOpen\": true,\r\n      \"workedPart\": 0\r\n    },\r\n    {\r\n      \"day\": \"2020-06-27T00:00:00Z\",\r\n      \"isOpen\": false,\r\n      \"workedPart\": 0\r\n    },\r\n    {\r\n      \"day\": \"2020-06-28T00:00:00Z\",\r\n      \"isOpen\": false,\r\n      \"workedPart\": 0\r\n    },\r\n    {\r\n      \"day\": \"2020-06-29T00:00:00Z\",\r\n      \"isOpen\": true,\r\n      \"workedPart\": 0\r\n    },\r\n    {\r\n      \"day\": \"2020-06-30T00:00:00Z\",\r\n      \"isOpen\": true,\r\n      \"workedPart\": 0\r\n    }\r\n  ]\r\n}";

            var uri = $"api/{ActivityEndpoint}/first";
            var payload = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await Client.PostAsync(uri, payload);
            var createdActivity = JsonConvert.DeserializeObject<ActivityDto>(await response.Content.ReadAsStringAsync());
            uri = $"api/{ActivityEndpoint}/{createdActivity.Id}/export";

            // Act
            response = await Client.GetAsync(uri);

            // Assert
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var xls = await response.Content.ReadAsStringAsync();
            xls.Should().NotBeNull();
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
