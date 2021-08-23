using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using Walter2021.Common.Models;
using Walter2021.Function.Funtions;
using Walter2021.Tests.Helpers;
using Xunit;

namespace Walter2021.Tests.Tests
{
    public class WalterApiTest
    {
        private readonly ILogger logger = TestFactory.CreateLogger();

        [Fact]
        public async void CreateWalter_Should_Return_200()
        {
            //Arrenge
            MockCloudTableWalters mockWalters = new MockCloudTableWalters(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            Walter walterRequest = TestFactory.GetWalterRequest();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(walterRequest);
            //Act
            IActionResult response = await WalterApi.CreateWalter(request, mockWalters, logger);

            //Assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);

        }

        [Fact]
        public async void UpdateWalter_Should_Return_200()
        {
            //Arrenge
            MockCloudTableWalters mockWalters = new MockCloudTableWalters(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            Walter walterRequest = TestFactory.GetWalterRequest();
            Guid walterId = Guid.NewGuid();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(walterId, walterRequest);
            //Act
            IActionResult response = await WalterApi.UpdateWalter(request, mockWalters, walterId.ToString(), logger);

            //Assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);

        }
    }
}
