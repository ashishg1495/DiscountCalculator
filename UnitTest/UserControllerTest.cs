using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using WebAPI.Controllers;
using WebAPI.DAO;
using WebAPI.DTO.Request;
using WebAPI.DTO.Response;
using Xunit;

namespace UnitTest
{
    public class UserControllerTest
    {
        private readonly UserController _controller;
        private readonly IUserDAO _userDAO;
        public UserControllerTest()
        {
            _userDAO = new UserDAO();
            _controller = new UserController(_userDAO);
        }

        [Fact]
        public void LoginUser_ValidUserPassed_ReturnsOkResult()
        {
            //Arrange
            var loginRequest = new LoginRequest()
            {
                Username = "ashish",
                Password = "abc@123"
            };

            //Act
            var result = _controller.LoginUser(loginRequest);

            //Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public void LoginUser_IncompleteUserPassed_ReturnsBadRequestResult()
        {
            //Arrange
            var loginRequest = new LoginRequest()
            {
                Username = "ashish"
            };

            //Act
            var result = _controller.LoginUser(loginRequest);

            //Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }
        [Fact]
        public void LoginUser_InvalidUserPassed_ReturnsUnauthorizedResult()
        {
            //Arrange
            var loginRequest = new LoginRequest()
            {
                Username = "ashish",
                Password = "123"
            };

            //Act
            var result = _controller.LoginUser(loginRequest);

            //Assert
            Assert.IsType<UnauthorizedObjectResult>(result.Result);
        }

        [Fact]
        public void CalculatePrice_Analysis_HasAuthorizeAttribute()
        {
            // Act
            var type = _controller.GetType();
            var actualAttribute = _controller.GetType().GetMethod("CalculatePrice").GetCustomAttributes(typeof(AuthorizeAttribute), true);

            // Assert
            Assert.Equal(typeof(AuthorizeAttribute), actualAttribute[0].GetType());
        }

        //Adding a mock valid Claim identity to bypass the authorization
        [Theory]
        [InlineData(1000, 10, 10)]
        public void CalculatePrice_ValidUserAccess_ReturnsOkResult(double goldPrice, double goldWeight, double? discount)
        {
            //Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
               {
                   new Claim(ClaimTypes.NameIdentifier, "1"),
               }));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            //Act
            var result = _controller.CalculatePrice(goldPrice, goldWeight, discount);

            //Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }

        //Adding a invalid mock Claim identity to throw unauthorized result
        [Theory]
        [InlineData(1000, 10, 10)]
        public void CalculatePrice_InvalidUserAccess_ReturnsUnauthorisedResult(double goldPrice, double goldWeight, double? discount)
        {
            //Arrange
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = null }
            };

            //Act
            var result = _controller.CalculatePrice(goldPrice, goldWeight, discount);

            //Assert
            Assert.IsType<UnauthorizedObjectResult>(result.Result);
        }

        //Adding a mock valid Claim identity to bypass the authorization
        [Theory]
        [InlineData(1000, 10, 0, 10000)]
        [InlineData(1000, 10, 5, 9500)]
        [InlineData(1000, 10, null, 10000)]
        public async Task CalculatePrice_ValidValuesPassed_ReturnExpectedValueAsync(double goldPrice, double goldWeight, double? discount, double expectedResult)
        {
            //Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
               {
                   new Claim(ClaimTypes.NameIdentifier, "1"),
               }));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            //Act
            var result = _controller.CalculatePrice(goldPrice, goldWeight, discount);
            var objectResult = await result as OkObjectResult ;
            var response = objectResult.Value as TotalPriceResponse;

            //Assert
            Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expectedResult, response.TotalPrice);
        }
    }
}




