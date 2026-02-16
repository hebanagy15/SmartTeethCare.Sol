using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartTeethCare.API.DTOs;
using SmartTeethCare.Core.Entities;
using SmartTeethCare.Core.Interfaces.Services;
using SmartTeethCare.Repository.Data;

namespace SmartTeethCare.API.Controllers.SecurityModule
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
            // 1) Set default role if not provided
            var role = string.IsNullOrEmpty(model.Role) ? "Patient" : model.Role;

            // 2) Check if email already exists
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
                return BadRequest("Email is already registered.");

            // 3) Validate role
            if (!await _roleManager.RoleExistsAsync(role))
                return BadRequest("Invalid role");

            // 4) Create user object
            var user = new User
            {
                UserName = model.UserName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Address = model.Address,
                DateOfBirth = model.DateOfBirth,
                Gender = model.Gender
            };

            // 5) Create user in Identity
            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // 6) Assign role
            await _userManager.AddToRoleAsync(user, role);

            // 7) If Doctor → add to Doctors table
            if (role == "Doctor")
            {
                var doctor = new Doctor
                {
                    UserId = user.Id,
                    Salary = 8000,
                    WorkingHours = 8,
                    HiringDate = DateTime.UtcNow
                };

                _context.Doctors.Add(doctor);
                await _context.SaveChangesAsync();
            }

            // 8) If Patient → add to Patients table
            if (role == "Patient")
            {
                var patient = new Patient
                {
                    UserId = user.Id,
                    MedicalHistory = "No prior conditions"
                };

                _context.Patients.Add(patient);
                await _context.SaveChangesAsync();
            }

            // 9) Return response with JWT
            return Ok(new UserDTO
            {
                UserName = user.UserName,
                Email = user.Email,
                Role = role,
                Token = await _authService.CreateTokenAsync(user, _userManager)
            });
        }
    }
}