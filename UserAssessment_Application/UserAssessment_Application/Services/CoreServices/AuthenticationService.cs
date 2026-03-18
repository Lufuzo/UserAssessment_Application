using UserAssessment_Application.DTOs.Request;
using UserAssessment_Application.DTOs.Responce;
using UserAssessment_Application.Entities;
using UserAssessment_Application.Repositories.IRepo;
using UserAssessment_Application.Services.ICoreServices;


namespace UserAssessment_Application.Services.CoreServices
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;

        public AuthenticationService(IUserRepository userRepository, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(request.Email))
                return new LoginResponse { Success = false, Message = "Email is required" };

            if (string.IsNullOrWhiteSpace(request.Password))
                return new LoginResponse { Success = false, Message = "Password is required" };

            // Find user by email
            var user = await _userRepository.GetUserByEmailAsync(request.Email.Trim().ToLower());
            if (user == null)
                return new LoginResponse { Success = false, Message = "Invalid email or password" };

            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return new LoginResponse { Success = false, Message = "Invalid email or password" };

            // Generate token
            var token = _tokenService.GenerateToken(user);

            var userDto = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            };

            return new LoginResponse
            {
                Success = true,
                Message = "Login successful",
                Token = token,
                User = userDto
            };
        }

        public async Task<ApiResponse<UserDto>> RegisterAsync(RegisterRequest request)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(request.FirstName))
                return new ApiResponse<UserDto> { Success = false, Message = "First name is required" };

            if (string.IsNullOrWhiteSpace(request.LastName))
                return new ApiResponse<UserDto> { Success = false, Message = "Last name is required" };

            if (string.IsNullOrWhiteSpace(request.Email))
                return new ApiResponse<UserDto> { Success = false, Message = "Email is required" };

            if (string.IsNullOrWhiteSpace(request.Password))
                return new ApiResponse<UserDto> { Success = false, Message = "Password is required" };

            if (request.Password.Length < 6)
                return new ApiResponse<UserDto> { Success = false, Message = "Password must be at least 6 characters" };

            // Check if user already exists
            var existingUser = await _userRepository.GetUserByEmailAsync(request.Email);
            if (existingUser != null)
                return new ApiResponse<UserDto> { Success = false, Message = "Email is already registered" };

            // Create new user
            var user = new User
            {
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                Email = request.Email.Trim().ToLower(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                CreatedAt = DateTime.UtcNow
            };

            var createdUser = await _userRepository.CreateUserAsync(user);

            var userDto = new UserDto
            {
                Id = createdUser.Id,
                FirstName = createdUser.FirstName,
                LastName = createdUser.LastName,
                Email = createdUser.Email
            };

            return new ApiResponse<UserDto>
            {
                Success = true,
                Message = "User registered successfully",
                Data = userDto
            };
        }
    }
}
