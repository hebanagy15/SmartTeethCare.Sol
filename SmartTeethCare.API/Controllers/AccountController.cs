using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SmartTeethCare.API.DTOs;
using SmartTeethCare.Core.Entities;
using SmartTeethCare.Core.Interfaces.Services;
using SmartTeethCare.Repository.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SmartTeethCare.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly IAuthService _authService;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context, IAuthService authService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
            _authService = authService;

        }

        public IAuthService AuthService { get; }

        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
                return Unauthorized("Invalid Email or Password");

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

            if (!result.Succeeded)
                return Unauthorized("Invalid Email or Password");


            return Ok(new UserDTO
            {
                UserName = user.UserName,
                Email = user.Email,
                Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault(),
                Token = await _authService.CreateTokenAsync(user, _userManager)    // Generate JWT Token
            });
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register(RegisterDTO model)
        {
            // 1) Check if user already exists
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
                return BadRequest("Email is already registered.");

            // Check if role exists
            if (!await _roleManager.RoleExistsAsync(model.Role))
                return BadRequest("Invalid role");
            // 2) Create new user object
            var user = new User
            {
                UserName = model.UserName,
                Email = model.Email,
                Password = model.Password,
                PhoneNumber = model.PhoneNumber,
                Address = model.Address,
                DateOfBirth = model.DateOfBirth,
                Gender = model.Gender
            };

            // 3) Create user using Identity
            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            // Assign selected role
            await _userManager.AddToRoleAsync(user, model.Role);


            if (model.Role == "Doctor")
            {
                var doctor = new Doctor
                {
                    UserId = user.Id,
                    Salary = 8000,
                    WorkingHours = 8,
                    HiringDate = DateTime.Now
                };

                _context.Doctors.Add(doctor);
                await _context.SaveChangesAsync();
            }
            if (model.Role == "Patient")
            {
                var patient = new Patient
                {
                    UserId = user.Id,
                    MedicalHistory = "No prior conditions",
                };

                _context.Patients.Add(patient);
                await _context.SaveChangesAsync();
            }
            // 6) Return response
            return Ok(new UserDTO()
            {
                UserName = user.UserName,
                Email = user.Email,
                Role = model.Role,
                Token = await _authService.CreateTokenAsync(user, _userManager) // Generate JWT Token
            });



        }
    }
}
