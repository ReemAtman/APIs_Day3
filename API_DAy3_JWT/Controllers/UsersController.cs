using BLL_Project.Dtos;
using DAL_Poject.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API_DAy3_JWT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<Student> userManager;
        private readonly IConfiguration configuration;
        public UsersController(UserManager<Student> _userManager, IConfiguration configuration)
        {
            this.userManager = _userManager;
            this.configuration = configuration;
        }
        
        #region Admin Register 
        [HttpPost]
        [Route("AdminRegister")]
        public async Task<ActionResult> AdminRegister(RegisterDto registerDto)
        {
            var student = new Student
            {
                UserName = registerDto.UserName,
                Email = registerDto.Email,
                Age = registerDto.Age
            };
            var created = await userManager.CreateAsync(student, registerDto.Password);

            if (!created.Succeeded)
            {
                return BadRequest(created.Errors);
            }

            var claims = new List<Claim>
            {
            new Claim("Age",$"{student.Age}"),
            new Claim(ClaimTypes.NameIdentifier, student.Id),
            new Claim(ClaimTypes.Name, student.UserName),
            new Claim(ClaimTypes.Role, "Admin")            
            };

            await userManager.AddClaimsAsync(student, claims);

            return Ok();
        }
        #endregion
        #region User Register 
        [HttpPost]
        [Route("UserRegister")]
        public async Task<ActionResult> UserRegister(RegisterDto registerDto)
        {
            var student = new Student
            {
                UserName = registerDto.UserName,
                Email = registerDto.Email,
                Age = registerDto.Age
            };
            var created = await userManager.CreateAsync(student, registerDto.Password);

            if (!created.Succeeded)
            {
                return BadRequest(created.Errors);
            }

            var claims = new List<Claim>
            {
            new Claim("Age",$"{student.Age}"),
            new Claim(ClaimTypes.NameIdentifier, student.Id),
            new Claim(ClaimTypes.Name, student.UserName),
            new Claim(ClaimTypes.Role, "User")
            };

            await userManager.AddClaimsAsync(student, claims);

            return Ok();
        }
        #endregion

        #region Login
        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult<TokenDto>> Login(LoginDto loginData)
        {
            var user = await userManager.FindByNameAsync(loginData.UserName);

            if (user == null)
            {
                return Unauthorized();
            }

            var isAuthenticated = await userManager.CheckPasswordAsync(user, loginData.Password);

            if (isAuthenticated == false)
            {
                return Unauthorized();
            }

            var claims = await userManager.GetClaimsAsync(user);

            // Secret Key
            var secretKeyString = configuration.GetValue<string>("SecretKey");
            var secretyKeyInBytes = Encoding.ASCII.GetBytes(secretKeyString ?? string.Empty);
            var secretKey = new SymmetricSecurityKey(secretyKeyInBytes);
         

           // Create secretKey, Algorithm 
            var signingCredentials = new SigningCredentials(secretKey,
                SecurityAlgorithms.HmacSha256Signature);
          
         
            var expireDate = DateTime.Now.AddDays(1);
            var token = new JwtSecurityToken(claims: claims,expires: expireDate, signingCredentials: signingCredentials);           

            // Casting Token 
            var tokenHandler = new JwtSecurityTokenHandler();
           
            return new TokenDto(tokenHandler.WriteToken(token), expireDate);
        }

        #endregion

        #region Get User Info
        [HttpGet]
        [Route("GetUserInfo")]
        [Authorize]
        public async Task<ActionResult> GetUserInfo()
        {
             var user = await userManager.GetUserAsync(User); 
            if (user == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                UserName = user?.UserName,
                Email = user?.Email,
                Age = user?.Age
            });
        }
        #endregion

        #region Get Info For Admins And Users
        [HttpGet]
        [Authorize(Policy = "AdminsAndUsers")]
        [Route("AdminsAndUsers")]
        public ActionResult GetInfoForAdminOrUser()
        {
            return Ok(new string[] { "Admin", "User" });
        }
        #endregion

        #region Get Info For Mangers Only
        [HttpGet]
        [Authorize(Policy = "ManagersOnly")]
        [Route("ManagersOnly")]
        public ActionResult GetInfoForManagerOnly()
        {
            return Ok(new string[] { "Manger", "Only" });
        }
        #endregion
    }
}
