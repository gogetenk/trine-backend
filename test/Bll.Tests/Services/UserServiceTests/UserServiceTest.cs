using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assistance.Operational.Bll.Impl.Services;
using Assistance.Operational.Dal.Repositories;
using AutoFixture;
using Dto;
using FluentAssertions;
using Moq;
using Xunit;

namespace Assistance.Operational.Bll.Tests.Services.UserServiceTests
{
    public class UserServiceTest : TestBase
    {
        private const string _userApiVersion = "1.0";

        public UserServiceTest() : base()
        {
        }
        public class Test
        {
            public Dictionary<string, string> Days { get; set; }
        }


        [Theory]
        [InlineData("email@mail.com", "toto", "titi")]
        [InlineData("email@mail.com", "", "titi")]
        [InlineData("", "toto", "titi")]
        [InlineData("email@mail.com", "toto", "")]
        [InlineData("", "toto", "")]
        [InlineData(null, null, null)]
        public async Task SearchUser_NominalCase_ExpectUser(string email, string firstname, string lastname)
        {
            // Arrange
            var matchingUser = new Fixture().Build<UserDto>()
                .With(x => x.Email, email)
                .With(x => x.Firstname, firstname)
                .With(x => x.Lastname, lastname)
                .CreateMany()
                .ToList();

            var userRepoMock = new Mock<IUserRepository>();
            var mailRepoMock = new Mock<IMailRepository>();
            var missionRepoMock = new Mock<IMissionRepository>();
            var activityRepoMock = new Mock<IActivityRepository>();

            userRepoMock
                .Setup(x => x.ApiUsersSearchGetAsync(email, firstname, lastname, It.IsAny<string>()))
                .ReturnsAsync(matchingUser);

            var service = new UserService(_loggerMock.Object, _mapper, userRepoMock.Object, _configurationMock.Object, mailRepoMock.Object, missionRepoMock.Object, activityRepoMock.Object);

            // Act
            var results = await service.SearchUsers(email, firstname, lastname, null);

            // Assert
            results.Should().NotBeNull();
            results.Should().BeOfType<List<UserDto>>();
        }

        [Fact]
        public async Task SearchUser_WhenNoResult_ExpectEmptyList()
        {
            // Arrange
            var userRepoMock = new Mock<IUserRepository>();
            var mailRepoMock = new Mock<IMailRepository>();
            var missionRepoMock = new Mock<IMissionRepository>();
            var activityRepoMock = new Mock<IActivityRepository>();

            userRepoMock
                .Setup(x => x.ApiUsersSearchGetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(value: null);

            var service = new UserService(_loggerMock.Object, _mapper, userRepoMock.Object, _configurationMock.Object, mailRepoMock.Object, missionRepoMock.Object, activityRepoMock.Object);

            // Act
            var results = await service.SearchUsers(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null);

            // Assert
            results.Should().NotBeNull();
            results.Should().BeOfType<List<UserDto>>();
            results.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllUsers_NominalCase_ExpectUsers()
        {
            // Arrange
            var matchingUser = new Fixture().Build<UserDto>()
                .CreateMany()
                .ToList();

            var userRepoMock = new Mock<IUserRepository>();
            var mailRepoMock = new Mock<IMailRepository>();
            var missionRepoMock = new Mock<IMissionRepository>();
            var activityRepoMock = new Mock<IActivityRepository>();

            userRepoMock
                .Setup(x => x.ApiUsersGetAsync(It.IsAny<string>()))
                .ReturnsAsync(matchingUser);

            var service = new UserService(_loggerMock.Object, _mapper, userRepoMock.Object, _configurationMock.Object, mailRepoMock.Object, missionRepoMock.Object, activityRepoMock.Object);

            // Act
            var results = await service.GetAllUsers();

            // Assert
            results.Should().NotBeNull();
            results.Should().BeOfType<List<UserDto>>();
        }

        [Fact]
        public async Task GetAllUsers_WhenNoResult_ExpectEmptyList()
        {
            // Arrange
            var userRepoMock = new Mock<IUserRepository>();
            var mailRepoMock = new Mock<IMailRepository>();
            var missionRepoMock = new Mock<IMissionRepository>();
            var activityRepoMock = new Mock<IActivityRepository>();

            userRepoMock
                .Setup(x => x.ApiUsersGetAsync(It.IsAny<string>()))
                .ReturnsAsync(value: null);

            var service = new UserService(_loggerMock.Object, _mapper, userRepoMock.Object, _configurationMock.Object, mailRepoMock.Object, missionRepoMock.Object, activityRepoMock.Object);

            // Act
            var results = await service.GetAllUsers();

            // Assert
            results.Should().NotBeNull();
            results.Should().BeOfType<List<UserDto>>();
            results.Should().BeEmpty();
        }
    }
}
