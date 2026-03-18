using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UserAssessment_Application.DTOs.Request;
using UserAssessment_Application.DTOs.Responce;
using UserAssessment_Application.Repositories.IRepo;

namespace UserAssessment_Application.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        // Injecting repository to access user data
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet("details")]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetUserDetails()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized(new ApiResponse<UserDto> { Success = false, Message = "Invalid token" });

            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null)
                return NotFound(new ApiResponse<UserDto> { Success = false, Message = "User not found" });

            var userDto = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            };

            return Ok(new ApiResponse<UserDto>
            {
                Success = true,
                Message = "User details retrieved successfully",
                Data = userDto
            });
        }
    }
}
