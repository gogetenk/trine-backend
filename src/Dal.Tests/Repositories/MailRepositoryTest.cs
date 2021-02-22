namespace Dal.Tests
{
    public class MailRepositoryTest
    {
        //[Fact]
        //public async Task SendAsync_WithoutWhitelistCase_ExpectOriginalMail()
        //{
        //    var configurationMock = new Mock<IConfiguration>();
        //    var loggerMock = new Mock<ILogger<MailRepository>>();
        //    var sendGridMock = new Mock<ISendGridClient>();
        //    var response = new Mock<Response>();

        //    var mailRepo = new MailRepository(configurationMock.Object, loggerMock.Object, sendGridMock.Object);

        //    string mail = "remiroycourt@gmail.com";
        //    string templateId = "template1";

        //    configurationMock.Setup(c => c["Mail:DefaultAddress"]).Returns("sender@hellotrine.com");
        //    configurationMock.Setup(c => c["Mail:DefaultName"]).Returns("Trine administrator");
        //    configurationMock.Setup(c => c["Mail:DisableWhitelist"]).Returns("true");

        //    sendGridMock
        //        .Setup(s => s.SendEmailAsync(It.IsAny<SendGridMessage>(), It.IsAny<CancellationToken>()))
        //        .ReturnsAsync(It.Is<Response>(x => x.StatusCode == HttpStatusCode.Accepted));

        //    var success = await mailRepo.SendAsync(mail, templateId);
        //    Assert.True(success);
        //}
    }
}
