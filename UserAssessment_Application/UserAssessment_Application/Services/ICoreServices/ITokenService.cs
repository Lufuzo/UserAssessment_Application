using UserAssessment_Application.Entities;

namespace UserAssessment_Application.Services.ICoreServices
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}
