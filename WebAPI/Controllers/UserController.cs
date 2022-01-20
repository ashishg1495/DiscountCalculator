using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using WebAPI.DAO;
using WebAPI.Models;
using WebAPI.DTO.Request;
using WebAPI.DTO.Response;

namespace WebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserDAO _userDAO;

        public UserController(IUserDAO userDAO)
        {
            _userDAO = userDAO;
        }


        // POST: User/authenticate
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Login User", Description = "This method is used to authenticate User Login")]
        [SwaggerResponse(statusCode: 200, type: typeof(LoginResponse), description: "Ok.")]
        [SwaggerResponse(statusCode: 400, type: typeof(ErrorResponse), description: "Bad request.")]
        [SwaggerResponse(statusCode: 401, type: typeof(ErrorResponse), description: "Unauthorized.")]
        [HttpPost("authenticate")]
        public async Task<ActionResult> LoginUser(LoginRequest req)
        {
            if (req == null || string.IsNullOrEmpty(req.Username) || string.IsNullOrEmpty(req.Password))
            {
                return BadRequest(new ErrorResponse { Message = "Missing Login Details" });
            }

            var result = await _userDAO.LoginUserAsync(req);

            if (result == null)
            {
                return Unauthorized(new ErrorResponse { Message = "Invalid Credentials" });
            }

            return Ok(result);
        }

        // GET: User/CalculatePrice
        [Authorize(Policy = "OnlyNonBlockedUser")]
        [SwaggerOperation(Summary = "Calculate Total Price", Description = "This method is used to calculate the Total Price")]
        [SwaggerResponse(statusCode: 200, type: typeof(TotalPriceResponse), description: "Ok.")]
        [SwaggerResponse(statusCode: 401, type: typeof(ErrorResponse), description: "Unauthorized.")]
        [HttpGet("calculate")]
        public async Task<ActionResult> CalculatePrice([Required] double goldPrice, [Required] double goldWeight, double? discount)
        {
            var claimsIdentity = HttpContext.User.Identity as ClaimsIdentity;

            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim == null)
            {
                return Unauthorized(new ErrorResponse { Message = "Invalid User" });
            }

            var result = await _userDAO.CalculateTotalPriceAsync(goldPrice, goldWeight, discount);

            return Ok(result); ;
        }
    }
}
