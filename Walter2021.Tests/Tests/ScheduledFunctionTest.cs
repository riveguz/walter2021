using System;
using Walter2021.Function.Funtions;
using Walter2021.Tests.Helpers;
using Xunit;

namespace Walter2021.Tests.Tests
{
    public class ScheduledFunctionTest
    {
        [Fact]
        public void ScheduledFunction_Should_Log_Message()
        {
            // Arrange
            MockCloudTableWalters mockWalters = new MockCloudTableWalters(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            ListLogger logger = (ListLogger)TestFactory.CreateLogger(LoggerTypes.List);

            // Act
            ScheduledFunction.Run(null, mockWalters, logger);
            string message = logger.Logs[0];

            // Asert
            Assert.Contains("Deleting completed", message);
        }
    }
}
