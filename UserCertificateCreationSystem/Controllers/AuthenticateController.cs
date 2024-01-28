using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserCertificateCreationSystem.Common;
using UserCertificateCreationSystem.Model;
using UserCertificateCreationSystem.Model.DBEntity;
using UserCertificateCreationSystem.Services;

namespace UserCertificateCreationSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IBaseService<User> _userService;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public AuthenticateController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IBaseService<User> userService, IEmailService emailService , SignInManager<IdentityUser> signInManager, IConfiguration configuration)
        {
            _configuration = configuration;
            _userManager = userManager;
            _roleManager = roleManager;
            _userService = userService;
            _emailService = emailService;
            _signInManager = signInManager;
        }

        [HttpPost]
        [ProducesResponseType(typeof(BadRequestObjectResult), 400)]
        [ProducesResponseType(typeof(CreatedResult), 201)]
        [Route("registration")]
        public async Task<IActionResult> UserRegistration([FromBody] User model, string role)
        {
            var response = new PayloadResponse<User>
            {
                success = false,
                message = new List<string>() { "" },
                payload = null,
                operation_type = PayloadType.Save
            };

            IdentityUser user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username,
                TwoFactorEnabled = true
            };

            if (await _roleManager.RoleExistsAsync(role))
            {
                var result = await _userManager.CreateAsync(user, model.Password);

                if (!result.Succeeded)
                { 
                    return BadRequest("Failed to register user.");
                }

                await _userManager.AddToRoleAsync(user, role);

                var newUser = new User()
                {
                    Username = model.Username,
                    Email = model.Email,
                    Password = model.Password
                };

                OperationResult createResult = await _userService.Add(newUser);

                if(createResult.Success)
                {
                    response.success = true;
                    response.message = new List<string>() { "User Registered Successfully" };
                    response.payload = createResult.Result;
                }

                return Ok(response);
            }

            return StatusCode(500, "This role isn't exist");
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel info)
        {
            var user = await _userManager.FindByNameAsync(info.Username);

            if (user.TwoFactorEnabled)
            {
                await _signInManager.SignOutAsync();
                await _signInManager.PasswordSignInAsync(user, info.Password, false, true);
                var token = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");

                var message = new Message(new string[] { user.Email! }, "OTP Verification", token);
                _emailService.SendEmail(message);

                return Ok($"A OTP has sent to your email {user.Email}");
            }

            return Unauthorized();
        }

        [HttpPost]
        [Route("login-2FA")]
        public async Task<IActionResult> LoginWithOTP(string otp, string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            var login = await _signInManager.TwoFactorSignInAsync("Email", otp, false, false);

            if(login.Succeeded)
            {
                if (user != null)
                {
                    var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                    var userRole = await _userManager.GetRolesAsync(user);

                    foreach (var role in userRole)
                    {
                        authClaims.Add(new Claim(ClaimTypes.Role, role));
                    }

                    var authicateToken = GetToken(authClaims);

                    return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(authicateToken)
                    });
                }
            }

            return NotFound("Invalid OTP");
        }

        private JwtSecurityToken GetToken(List<Claim> authClaims) 
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT: ValidIssuer"],
                audience: _configuration["JWT: ValidAudience"],
                expires: DateTime.Now.AddHours(1),
                claims: authClaims,
                signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256));

            return token;
        }
    }
}
