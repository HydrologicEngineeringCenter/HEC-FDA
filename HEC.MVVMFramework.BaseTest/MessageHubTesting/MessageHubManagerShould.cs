using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Interfaces;
using HEC.MVVMFramework.BaseTest.MockData;
using Xunit;

namespace HEC.MVVMFramework.BaseTest.MessageHubTesting
{
    public class MessageHubManagerShould
    {
        [Fact]
        public void RegisterAndUnRegisterRecursively()
        {
            //Arrange
            MockTieredRegisteredClass mock = new MockTieredRegisteredClass();
            mock.StructProperty = 1;
            IReportMessage[] listOfReporter = new IReportMessage[] { new MockTieredRegisteredClass(), new MockTieredRegisteredClass(), new MockTieredRegisteredClass() };
            mock.ArrayOfMessageReporterProperty = listOfReporter;
            mock.ObjectProperty = new Message("poop");
            mock.MessageReporterProperty = new MockTieredRegisteredClass();
            mock.IReportMessageSavedUnderDifferentInterfaceType = new MockTieredRegisteredClass();

            //Act
            MessageHubManager.RegisterSelfAndProps(mock);

            //Assert
            Assert.Contains(mock, MessageHub.Reporters);
            Assert.Contains(mock.MessageReporterProperty, MessageHub.Reporters);
            Assert.Contains(mock.ArrayOfMessageReporterProperty[0], MessageHub.Reporters);
            Assert.DoesNotContain((object)mock.ObjectProperty, MessageHub.Reporters);
            Assert.Contains((object)mock.IReportMessageSavedUnderDifferentInterfaceType, MessageHub.Reporters);
            Assert.Equal(6,MessageHub.Reporters.Count);

            //Act
            MessageHubManager.UnRegisterSelfAndProps(mock);

            //Assert
            Assert.Empty(MessageHub.Reporters);
        }
    }
}
