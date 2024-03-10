using EcommerceApplication.API.Commons;
using EcommerceApplication.API.ViewModels;
using EcommerceApplication_DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApplication.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<AuthController> _logger;

        public AuthController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _signInManager = signInManager;
            _logger = logger; // Injected ILogger instance
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterVM registerVM)
        {
            try
            {
                // Log registration attempt
                _logger.LogInformation($"Registration attempt for user: {registerVM.Email}");

                var isExist = await _userManager.FindByNameAsync(registerVM.Name);
                if (isExist != null) return StatusCode(StatusCodes.Status500InternalServerError, new Response
                {
                    Status = "Error",
                    Message = "User already exists!"
                });

                AppUser appUser = new AppUser
                {
                    UserName = registerVM.Name,
                    AccountType = registerVM.AccountType,
                    Email = registerVM.Email,
                    PhoneNumber = registerVM.PhoneNo,
                    Password = registerVM.Password,
                    ShopName = registerVM.ShopName,
                    BusinessType = registerVM.BusinessType,
                    UserRole = registerVM.UserRole,
                    IsDeleted = registerVM.IsDeleted
                };

                var result = await _userManager.CreateAsync(appUser, registerVM.Password);
                if (!result.Succeeded) return StatusCode(StatusCodes.Status500InternalServerError, new Response
                {
                    Status = "Error",
                    Message = "User creation failed! Please check user details and try again."
                });

                if (!await _roleManager.RoleExistsAsync(registerVM.UserRole))
                    await _roleManager.CreateAsync(new IdentityRole(registerVM.UserRole));

                if (await _roleManager.RoleExistsAsync(registerVM.UserRole))
                    await _userManager.AddToRoleAsync(appUser, registerVM.UserRole);

                // Log successful registration
                _logger.LogInformation($"User registered successfully: {registerVM.Email}");

                return Ok(new Response
                {
                    Status = "Success",
                    Message = "User created successfully!"
                });
            }
            catch (Exception ex)
            {
                // Log registration failure
                _logger.LogError($"Failed to register user: {registerVM.Email}. Error: {ex.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, new Response
                {
                    Status = "Error",
                    Message = "User creation failed! Please check user details and try again."
                });
            }
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginVM loginVM)
        {
            try
            {
                // Log login attempt
                _logger.LogInformation($"Login attempt for user: {loginVM.UserName}");

                var user = await _userManager.FindByNameAsync(loginVM.UserName);
                if (user != null && await _userManager.CheckPasswordAsync(user, loginVM.Password))
                {
                    var userRoles = await _userManager.GetRolesAsync(user);
                    var authClaims = new List<Claim> {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(ClaimTypes.MobilePhone, user.PhoneNumber),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    };
                    foreach (var userRole in userRoles)
                    {
                        authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                    }
                    var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JsonWebTokenKeys:IssuerSigningKey"]));
                    var token = new JwtSecurityToken(expires: DateTime.Now.AddHours(3), claims: authClaims, signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));

                    // Log successful login
                    _logger.LogInformation($"User logged in successfully: {loginVM.UserName}");

                    return Ok(new
                    {
                        api_key = new JwtSecurityTokenHandler().WriteToken(token),
                        expiration = token.ValidTo,
                        user = user,
                        Role = userRoles,
                        status = "User Login Successfully"
                    });
                }
                else
                {
                    // Log login failure
                    _logger.LogWarning($"Failed login attempt for user: {loginVM.UserName}");

                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log login failure
                _logger.LogError($"Failed to login user: {loginVM.UserName}. Error: {ex.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, new Response
                {
                    Status = "Error",
                    Message = "Login failed! Please try again later."
                });
            }
        }
    }
}
