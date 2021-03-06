using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using src.Api.Domain.Dtos;
using src.Api.Domain.Entities;
using src.Api.Domain.Interfaces.Services.User;
using src.Api.Domain.Repository;
using src.Api.Domain.Security;

namespace src.Api.Service.Services
{
    public class LoginService : ILoginService
    {
        private IUserRepository _repository;
        private SigningConfigurations _signingConfigs;
        private TokenConfigurations _tokenConfigs;
        private IConfiguration _configuration { get; }
        public LoginService(
            IUserRepository repository,
            SigningConfigurations signingConfigs,
            TokenConfigurations tokenConfigs,
            IConfiguration configuration
        )
        {
            _repository = repository;
            _signingConfigs = signingConfigs;
            _tokenConfigs = tokenConfigs;
            _configuration = configuration;
        }

        public async Task<object> FindByLogin(LoginDto user)
        {
            var baseUser = new UserEntity();
            if (user != null && !string.IsNullOrWhiteSpace(user.Email))
            {
                baseUser = await _repository.FindByLogin(user.Email);
                if (baseUser == null)
                {
                    return new
                    {
                        authenticated = false,
                        message = "Falhou ao tentar logar!"
                    };

                }
                else
                {
                    var identity = new ClaimsIdentity(
                        new GenericIdentity(baseUser.Email),
                        new[]
                        {
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                            new Claim(JwtRegisteredClaimNames.UniqueName, user.Email),
                        }
                    );

                    var createDate = DateTime.Now;
                    var expirationDate = createDate + TimeSpan.FromSeconds(_tokenConfigs.Seconds);
                    var handler = new JwtSecurityTokenHandler();
                    var token = CreateToken(identity, createDate, expirationDate, handler);
                    return SuccessObject(createDate, expirationDate, token, baseUser);
                }
            }
            else
            {
                return new
                {
                    authenticated = false,
                    message = "Falhou ao tentar logar!"
                };
            }
        }
        private string CreateToken(ClaimsIdentity identity, DateTime createDate, DateTime expirationDate, JwtSecurityTokenHandler handler)
        {
            var securityToken = handler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = _tokenConfigs.Issuer,
                Audience = _tokenConfigs.Audience,
                SigningCredentials = _signingConfigs.SigningCredentials,
                Subject = identity,
                NotBefore = createDate,
                Expires = expirationDate,
            });

            var token = handler.WriteToken(securityToken);
            return token;
        }

        private object SuccessObject(DateTime createDate, DateTime expirationDate, string token, UserEntity user)
        {
            return new
            {
                authenticated = true,
                created = createDate.ToString("yyyy-MM-dd HH:mm:ss"),
                expiration = expirationDate.ToString("yyyy-MM-dd HH:mm:ss"),
                accessToken = token,
                userName = user.Name,
                userEmail = user.Email,
                message = "Usuário Logado com sucesso"
            };
        }
    }
}
