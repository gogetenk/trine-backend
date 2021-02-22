using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assistance.Operational.Bll.Impl.Errors;
using Assistance.Operational.Bll.Impl.Services;
using Assistance.Operational.Dal.Repositories;
using Assistance.Operational.Dto;
using AutoFixture;
using Dto;
using FluentAssertions;
using Moq;
using Sogetrel.Sinapse.Framework.Exceptions;
using Xunit;

namespace Assistance.Operational.Bll.Tests.Services.OrganizationServiceTests
{
    public class OrganizationServiceTest : TestBase
    {
        public OrganizationServiceTest() : base()
        {
        }

        [Fact]
        public async Task GetById_NominalCase()
        {
            // Arrange
            var orga = new Fixture().Create<OrganizationDto>();

            var userRepositoryMock = new Mock<IUserRepository>();
            var orgaRepoMock = new Mock<IOrganizationRepository>();
            orgaRepoMock.Setup(x => x.ApiOrganizationsByIdGetAsync(orga.Id, It.IsAny<string>())).ReturnsAsync(orga);

            var service = new OrganizationService(_loggerMock.Object, _mapper, orgaRepoMock.Object, userRepositoryMock.Object);

            // Act
            var result = await service.GetById(orga.Id);

            // Assert
            result.Should().BeEquivalentTo(_mapper.Map<OrganizationDto>(orga));
        }


        [Fact]
        public async Task Create_NominalCase()
        {
            // Arrange
            var orga = new Fixture().Create<OrganizationDto>();

            var userRepositoryMock = new Mock<IUserRepository>();
            var orgaRepoMock = new Mock<IOrganizationRepository>();
            orgaRepoMock.Setup(x => x.ApiOrganizationsPostAsync(orga, It.IsAny<string>())).ReturnsAsync(orga);

            var service = new OrganizationService(_loggerMock.Object, _mapper, orgaRepoMock.Object, userRepositoryMock.Object);

            // Act
            var result = await service.Insert(_mapper.Map<OrganizationDto>(orga));

            // Assert
            result.Should().BeEquivalentTo(_mapper.Map<OrganizationDto>(orga));
        }

        [Fact]
        public async Task Create_WithNullName_ExpectBusinessException()
        {
            // Arrange
            var orga = new Fixture().Create<OrganizationDto>();
            orga.Name = null;

            var userRepositoryMock = new Mock<IUserRepository>();
            var orgaRepoMock = new Mock<IOrganizationRepository>();
            orgaRepoMock.Setup(x => x.ApiOrganizationsPostAsync(orga, It.IsAny<string>())).ReturnsAsync(orga);

            var service = new OrganizationService(_loggerMock.Object, _mapper, orgaRepoMock.Object, userRepositoryMock.Object);

            // Act
            var exc = await Assert.ThrowsAsync<BusinessException>(() => service.Insert(_mapper.Map<OrganizationDto>(orga)));

            // Assert
            exc.Should().NotBeNull();
            exc.Message.Should().Be(ErrorMessages.createOrgaNoNameError);
        }

        [Fact]
        public async Task GetAllMembers_NominalCase()
        {
            // Arrange
            var orgaId = "1337";
            var members = new Fixture().CreateMany<OrganizationMemberDto>().ToList();
            var users = members.Select(x => new UserDto()
            {
                Id = x.UserId
            }).ToList();

            var orgaRepoMock = new Mock<IOrganizationRepository>();
            orgaRepoMock.Setup(x => x.ApiOrganizationsByOrganizationIdMembersGetAsync(orgaId, It.IsAny<string>())).ReturnsAsync(members);
            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(x => x.ApiUsersIdsGetAsync(It.IsAny<List<string>>(), It.IsAny<string>())).ReturnsAsync(users);

            var service = new OrganizationService(_loggerMock.Object, _mapper, orgaRepoMock.Object, userRepositoryMock.Object);

            // Act
            var result = await service.GetAllMembers(orgaId);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(members.Count);
            foreach (var user in result)
            {
                user.Role.Should().Be(Enum.GetName(typeof(OrganizationMemberDto.RoleEnum), members.FirstOrDefault(y => y.UserId == user.Id).Role));
            }
        }

        [Fact]
        public async Task GetAllMembers_WhenNoMember_ExpectNull()
        {
            // Arrange
            var orgaId = "1337";

            var orgaRepoMock = new Mock<IOrganizationRepository>();
            orgaRepoMock.Setup(x => x.ApiOrganizationsByOrganizationIdMembersGetAsync(orgaId, It.IsAny<string>())).ReturnsAsync(value: null);
            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(x => x.ApiUsersIdsGetAsync(It.IsAny<List<string>>(), It.IsAny<string>())).ReturnsAsync(value: null);

            var service = new OrganizationService(_loggerMock.Object, _mapper, orgaRepoMock.Object, userRepositoryMock.Object);

            // Act
            var result = await service.GetAllMembers(orgaId);

            // Assert
            result.Should().BeNull();
        }
    }
}
