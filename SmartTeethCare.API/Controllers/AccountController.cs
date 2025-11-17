using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SmartTeethCare.API.DTOs;
using SmartTeethCare.Core.Entities;
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

        public AccountController(UserManager<User> userManager,SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
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

            // Generate JWT Token
            //var token = GenerateToken(user);

            return Ok(new UserDTO
            {
                UserName = user.UserName,
                Email = user.Email,
                Token = "This will be token"
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

            // 5) Generate token (optional)
            //var token = GenerateToken(user);
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
            return Ok(new
            {
                UserName = user.UserName,
                Email = user.Email,
                roleAssigned = model.Role,
                Token = "This will be token"
            });
        }


        //private string GenerateToken(User user)
        //{
        //    var claims = new[]
        //    {
        //        new Claim(JwtRegisteredClaimNames.Sub, user.Id),
        //        new Claim(JwtRegisteredClaimNames.Email, user.Email),
        //        new Claim("uid", user.Id),
        //    };

        //    var key = new SymmetricSecurityKey(
        //        Encoding.UTF8.GetBytes(_config["JWT:Key"]));

        //    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        //    var token = new JwtSecurityToken(
        //        issuer: _config["JWT:Issuer"],
        //        audience: _config["JWT:Audience"],
        //        claims: claims,
        //        expires: DateTime.UtcNow.AddHours(3),
        //        signingCredentials: creds
        //    );
        //    return new JwtSecurityTokenHandler().WriteToken(token);
        //}
    }
}
