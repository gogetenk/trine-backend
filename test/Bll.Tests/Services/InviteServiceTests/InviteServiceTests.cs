
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assistance.Operational.Bll.Impl.Services;
using Assistance.Operational.Dal.Repositories;
using Assistance.Operational.Dto;
using AutoFixture;
using Dto;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Sogetrel.Sinapse.Framework.Exceptions;
using Xunit;

namespace Assistance.Operational.Bll.Tests.Services.InviteServiceTests
{
    public class InviteServiceTests : TestBase
    {
        [Fact]
        public async Task AcceptInvite_NominalCase_ExpectMailSend()
        {
            _configuration.SetupGet(m => m[It.IsAny<string>()]).Returns("MailTemplate");

            var inviter = new Fixture().Create<UserDto>();
            var user = new Fixture().Create<UserDto>();
            var organization = new Fixture().Create<OrganizationDto>();
            var OrganizationMemberDto = new Fixture().Create<OrganizationMemberDto>();
            OrganizationMemberDto.UserId = user.Id;
            var invite = new Fixture().Create<InviteDto>();
            invite.GuestEmail = user.Email;
            invite.OrganizationId = organization.Id;
            invite.InviterId = inviter.Id;
            invite.Expires = DateTime.Now.AddMonths(1);

            var configurationMock = new Mock<IConfiguration>();
            configurationMock.SetReturnsDefault<string>(string.Empty);

            var inviteRepoMock = new Mock<IInviteRepository>();
            inviteRepoMock.Setup(x => x.ApiInvitesByCodeGetAsync(invite.Code, It.IsAny<string>())).ReturnsAsync(invite);

            var userRepoMock = new Mock<IUserRepository>();

            userRepoMock.Setup(x => x.ApiUsersSearchGetAsync(user.Email, null, null, It.IsAny<string>())).ReturnsAsync(new List<UserDto>() { user });
            userRepoMock.Setup(x => x.ApiUsersByIdGetAsync(inviter.Id, It.IsAny<string>())).ReturnsAsync(inviter);

            var orgaRepoMock = new Mock<IOrganizationRepository>();
            orgaRepoMock.Setup(x => x.ApiOrganizationsByOrganizationIdMembersPutAsync(It.IsAny<string>(),
                It.IsAny<OrganizationMemberDto>(), It.IsAny<string>()))
                .ReturnsAsync(OrganizationMemberDto);

            var mailRepoMock = new Mock<IMailRepository>();

            var service = new InviteService(_loggerMock.Object, _mapper, orgaRepoMock.Object, inviteRepoMock.Object,
                _configuration.Object, mailRepoMock.Object, userRepoMock.Object);

            var code = invite.Code.ToString();
            try
            {
                await service.AcceptInvite(code);
            }
            catch (Exception exc)
            {
                throw exc;
            }
            Assert.True(mailRepoMock.Invocations.Any());
        }

        //[Fact]
        //public async Task CreateInvite_WithGuestUser_ExpectNotAuthorizedException()
        //{
        //    var member = new Fixture().Create<UserDto>();
        //    var inviter = new Fixture().Create<UserDto>();
        //    var organizationDto = new Fixture().Create<OrganizationDto>();

        //    organizationDto.Members.Add(new OrganizationMemberDto()
        //    {
        //        Firstname = inviter.Firstname,
        //        Lastname = inviter.LastName,
        //        JoinedAt = new DateTime(2019, 1, 1),
        //        Role = OrganizationMemberDto.RoleEnum.GUEST,
        //        UserId = inviter.Id
        //    });
        //    var organization = _mapper.Map<OrganizationDto>(organizationDto);

        //    var orgaRepoMock = new Mock<IOrganizationApi>();
        //    var inviteRepoMock = new Mock<IInviteRepository();
        //    var userRepoMock = new Mock<IUserRepository>();
        //    var mailRepoMock = new Mock<IMailRepository>();

        //    orgaRepoMock.Setup(o => o.ApiOrganizationsByIdGetAsync(organizationDto.Id, It.IsAny<string>())).ReturnsAsync(organization);

        //    var service = new InviteService(_loggerMock.Object, _mapper, orgaRepoMock.Object, inviteRepoMock.Object,
        //        _configuration.Object, mailRepoMock.Object, userRepoMock.Object);

        //    var exeption = await Assert.ThrowsAsync<BusinessException>(async () => await service.CreateInvite(member.Mail, inviter.Id, organizationDto.Id));
        //    Assert.Equal("The user that created this invite has no longer the sufficient rights to invite you.", exeption.Message);
        //}

        [Fact]
        public async Task CreateInvite_WithoutMail_ExpectThrowException()
        {
            var orgaRepoMock = new Mock<IOrganizationRepository>();
            var inviteRepoMock = new Mock<IInviteRepository>();
            var userRepoMock = new Mock<IUserRepository>();
            var mailRepoMock = new Mock<IMailRepository>();

            var service = new InviteService(_loggerMock.Object, _mapper, orgaRepoMock.Object, inviteRepoMock.Object,
            _configuration.Object, mailRepoMock.Object, userRepoMock.Object);

            var exception = await Assert.ThrowsAsync<BusinessException>(async () => await service.CreateInvite(string.Empty, string.Empty, string.Empty));
            Assert.Equal("Required mail", exception.Message);
        }

        [Fact]
        public async Task CreateInvite_NominalCase_ExpectCreatedInvitation()
        {
            _configuration.SetupGet(m => m[It.IsAny<string>()]).Returns("MailTemplate");

            var member = new Fixture().Create<UserDto>();
            var inviterDto = new Fixture().Create<UserDto>();
            var inviter = _mapper.Map<UserDto>(inviterDto);

            var organizationDto = new Fixture().Create<OrganizationDto>();

            organizationDto.Members.Add(new OrganizationMemberDto()
            {
                Firstname = inviter.Firstname,
                Lastname = inviter.Lastname,
                JoinedAt = new DateTime(2019, 1, 1),
                Role = OrganizationMemberDto.RoleEnum.ADMIN,
                UserId = inviter.Id
            });
            var organization = _mapper.Map<OrganizationDto>(organizationDto);

            var inviteDto = new Fixture().Create<InviteDto>();
            inviteDto.InviterId = inviter.Id;
            inviteDto.GuestEmail = member.Email;

            var orgaRepoMock = new Mock<IOrganizationRepository>();
            var inviteRepoMock = new Mock<IInviteRepository>();
            var userRepoMock = new Mock<IUserRepository>();
            var mailRepoMock = new Mock<IMailRepository>();

            orgaRepoMock.Setup(o => o.ApiOrganizationsByIdGetAsync(organizationDto.Id, It.IsAny<string>())).ReturnsAsync(organization);
            inviteRepoMock.Setup(o => o.ApiInvitesPostAsync(It.IsAny<InviteDto>(), It.IsAny<string>())).ReturnsAsync(inviteDto);
            userRepoMock.Setup(u => u.ApiUsersByIdGetAsync(inviter.Id, It.IsAny<string>())).ReturnsAsync(inviter);

            var service = new InviteService(_loggerMock.Object, _mapper, orgaRepoMock.Object, inviteRepoMock.Object,
                _configuration.Object, mailRepoMock.Object, userRepoMock.Object);

            var invitation = await service.CreateInvite(member.Email, inviter.Id, organizationDto.Id);
            invitation.GuestEmail.Should().Equals(member.Email);
            invitation.CurrentStatus.Should().Equals(InviteDto.Status.Pending);
            invitation.InviterId.Should().Equals(inviter.Id);
            invitation.OrganizationId.Should().Equals(organization.Id);
            invitation.OrganizationName.Should().Equals(organization.Name);

            Assert.True(mailRepoMock.Invocations.Any());
        }
    }
}
