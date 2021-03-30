using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TwetterBook.Domain;
using TwetterBook.Options;

namespace TwetterBook.Services
{
    public class IdentityService : IIdentityService
    {

        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtSettings _jwtSettings;

        public IdentityService(UserManager<IdentityUser> userManager, JwtSettings jwtSettings)
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings;
        }



        public async Task<AuthinticationResult> CreateAsync(string email, string passWord)
        {
            var User = await _userManager.FindByNameAsync(email);

            if (User != null)
            {
                return new AuthinticationResult
                {
                    Errors = new[] { "User With This Email Adress Already exist" }
                };
            }

            var newUser = new IdentityUser
            {
                Email = email,
                UserName = email
            };

            var createdUser = await _userManager.CreateAsync(newUser, passWord);

            if (!createdUser.Succeeded)
            {
                return new AuthinticationResult
                {
                    Errors = createdUser.Errors.Select(x => x.Description)

                };
            }

            return GenerateAuthnticationResultForResult(newUser);
        }

       

        public async Task<AuthinticationResult> LoginAsync(string email, string PassWord)
        {

            var user =await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return new AuthinticationResult
                {
                    Errors = new[] { "User is Not Found"}
                };
            }

            var userHasValidPassword =await _userManager.CheckPasswordAsync(user, PassWord);

            if (!userHasValidPassword)
            {
                return new AuthinticationResult
                {
                    Errors = new[] { "Email/PassWord Are incorrect"}
                };
            }

            return GenerateAuthnticationResultForResult(user);

        }








        private AuthinticationResult GenerateAuthnticationResultForResult(IdentityUser newUser)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var Key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims: new[]
                {
                    new Claim(type: JwtRegisteredClaimNames.Sub, value: newUser.Email),
                    new Claim(type: JwtRegisteredClaimNames.Jti, value: Guid.NewGuid().ToString()),
                    new Claim(type: JwtRegisteredClaimNames.Email, value: newUser.Email),
                    new Claim(type: "id", value: newUser.Id),
                }),

                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Key), SecurityAlgorithms.HmacSha256Signature),

            };
            var Token = tokenHandler.CreateToken(tokenDescriptor);

            return new AuthinticationResult
            {
                Success = true,
                Token = tokenHandler.WriteToken(Token),
            };
        }

    }

}
