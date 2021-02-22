using System.Threading.Tasks;
using Assistance.Event.Dal.Repositories;
using Assistance.Operational.Bll.Impl.Services;
using Assistance.Operational.Dal.Repositories;
using AutoFixture;
using Dto;
using Microsoft.Extensions.Configuration;
using Moq;
using Sogetrel.Sinapse.Framework.Exceptions;
using Xunit;

namespace Assistance.Operational.Bll.Tests.Services.ActivityServiceTests
{
    public class ExportActivityTest : TestBase
    {
        //[Fact]
        //public async Task ExportActivity_NominalCase()
        //{
        //    var activityRepoMock = new Mock<IActivityRepository>();
        //    var missionRepositoryMock = new Mock<IMissionRepository>();
        //    var userRepoMock = new Mock<IUserRepository>();
        //    var eventRepoMock = new Mock<IEventRepository>();
        //    var orgaRepoMock = new Mock<IOrganizationRepository>();
        //    var mailRepoMock = new Mock<IMailRepository>();
        //    var notifRepoMock = new Mock<INotificationRepository>();
        //    var configurationMock = new Mock<IConfiguration>();
        //    var azureStorageRepoMock = new Mock<IAzureStorageRepository>();
        //    var internalNotificationRepository = new Mock<IInternalNotificationRepository>();

        //    var service = new ActivityService(_loggerMock.Object, _mapper, activityRepoMock.Object, userRepoMock.Object, eventRepoMock.Object,
        //         missionRepositoryMock.Object, orgaRepoMock.Object, mailRepoMock.Object, notifRepoMock.Object, configurationMock.Object,
        //         azureStorageRepoMock.Object, internalNotificationRepository.Object);

        //    var activity = new Fixture().Create<ActivityDto>();
        //    activity.Customer.SignatureUri = null;

        //    activityRepoMock.Setup(x => x.ApiActivitiesByIdGetAsync(activity.Id, It.IsAny<string>())).ReturnsAsync(_mapper.Map<ActivityDto>(activity));

        //    var result = await service.ExportActivity(activity.Id);
        //    Assert.NotNull(result);
        //}

        [Fact]
        public async Task ExportActivity_NullCase_ThrowBusinessException()
        {
            var activityRepoMock = new Mock<IActivityRepository>();
            var missionRepositoryMock = new Mock<IMissionRepository>();
            var userRepoMock = new Mock<IUserRepository>();
            var eventRepoMock = new Mock<IEventRepository>();
            var orgaRepoMock = new Mock<IOrganizationRepository>();
            var mailRepoMock = new Mock<IMailRepository>();
            var notifRepoMock = new Mock<INotificationRepository>();
            var configurationMock = new Mock<IConfiguration>();
            var azureStorageRepoMock = new Mock<IAzureStorageRepository>();
            var internalNotificationRepository = new Mock<IInternalNotificationRepository>();

            var service = new ActivityService(_loggerMock.Object, _mapper, activityRepoMock.Object, userRepoMock.Object, eventRepoMock.Object,
                missionRepositoryMock.Object, orgaRepoMock.Object, mailRepoMock.Object, notifRepoMock.Object, configurationMock.Object,
                azureStorageRepoMock.Object, internalNotificationRepository.Object);

            var activity = new Fixture().Create<ActivityDto>();
            activity.Customer = null;
            activity.Consultant = null;
            activity.Commercial = null;
            activity.Days = null;

            var mission = new Fixture().Create<MissionDto>();
            mission.Id = activity.MissionId;

            activityRepoMock.Setup(x => x.ApiActivitiesByIdGetAsync(activity.Id, It.IsAny<string>())).ReturnsAsync(value: null);
            missionRepositoryMock.Setup(m => m.ApiMissionsByIdGetAsync(activity.MissionId, It.IsAny<string>())).ReturnsAsync(value: null);

            await Assert.ThrowsAsync<BusinessException>(() => service.ExportActivity(activity.Id));
        }
    }
}