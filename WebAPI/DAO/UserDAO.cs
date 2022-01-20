#region Using Directives
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Helpers;
using WebAPI.Models;
using WebAPI.DTO.Request;
using WebAPI.DTO.Response;
#endregion

namespace WebAPI.DAO
{
    public class UserDAO : IUserDAO
    {
        private readonly List<User> userList;

        public UserDAO()
        {
            userList = new List<User>
               {
                   new User { Id = 1, Username = "ashish", Password = "abc@123", Blocked = false },
                   new User { Id = 2, Username = "rohan", Password = "xyz@321", Blocked = false }
               };
        }

        public async Task<LoginResponse> LoginUserAsync(LoginRequest req)
        {

            var user = userList.SingleOrDefault(x => x.Username == req.Username);

            // check if username exists
            if (user == null)
                return null;

            if (user.Password != req.Password)
                return null;

            var token = await Task.Run(() => TokenHelper.GenerateToken(user));

            return new LoginResponse { Username = user.Username, Token = token };
        }

        public async Task<TotalPriceResponse> CalculateTotalPriceAsync(double price, double weight, double? discount)
        {
            double discountAmount = ((discount != null) ? (double)discount : 0 )* price * weight / 100;
            double total = (price * weight) - discountAmount;

            return await Task.FromResult(new TotalPriceResponse { TotalPrice = total });
        }
    }
}
