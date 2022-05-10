using Alura.ListaLeitura.Seguranca;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Alura.ListaLeitura.Services
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly SignInManager<Usuario> _singInManager;

        public LoginController(SignInManager<Usuario> signInManager)
        {
            _singInManager = signInManager;
        }

        [HttpPost]
        public async Task<IActionResult> Token([FromBody] LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _singInManager.PasswordSignInAsync(model.Login, model.Password, true, true);

                if (result.Succeeded)
                {
                    //criar token (header + payload >> claims(direitos) + signature >> key)

                    //claims
                    var claims = new[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, model.Login),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                    };

                    //signature
                    var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("alura-webapi-authentication-valid"));
                    var credenciais = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                    var token = new JwtSecurityToken(
                        issuer: "Alura.WebApp",
                        audience: "Postman",
                        claims: claims,
                        signingCredentials: credenciais,
                        expires: DateTime.Now.AddMinutes(30)
                    );

                    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
                    return Ok(tokenString);
                }
                return Unauthorized(); //401
            }

            return BadRequest(); //400
        }
    }
}
