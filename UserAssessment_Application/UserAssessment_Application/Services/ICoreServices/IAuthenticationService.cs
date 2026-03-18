using UserAssessment_Application.DTOs.Request;
using UserAssessment_Application.DTOs.Responce;

namespace UserAssessment_Application.Services.ICoreServices
{
    public interface IAuthenticationService
    {
        Task<ApiResponse<UserDto>> RegisterAsync(RegisterRequest request);
        Task<LoginResponse> LoginAsync(LoginRequest request);
    }
}
