#region Using Directives
using System.Threading.Tasks;
using WebAPI.DTO.Request;
using WebAPI.DTO.Response;
#endregion

namespace WebAPI.DAO
{
    public interface IUserDAO
    {
        Task<LoginResponse> LoginUserAsync(LoginRequest req);

        Task<TotalPriceResponse> CalculateTotalPriceAsync(double price, double weight, double? discount);
    }
}
