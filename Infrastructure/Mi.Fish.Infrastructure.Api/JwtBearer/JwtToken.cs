using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Mi.Fish.Infrastructure.Session;
using Microsoft.IdentityModel.Tokens;

namespace Mi.Fish.Infrastructure.Api.JwtBearer
{
    public class JwtToken
    {
        /// <summary>
        /// 获取登录TOKEN
        /// </summary>
        /// <param name="jwtSetting">The JWT setting.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="storageNo">The storage no.</param>
        /// <param name="terminalId">The terminal identifier.</param>
        /// <returns></returns>
        public static string GetToken(JwtSetting jwtSetting, string userId, string storageNo, string terminalId)
        {
            var symmetricKeyAsBase64 = jwtSetting.ServerSecret;
            var keyByteArray = Encoding.ASCII.GetBytes(symmetricKeyAsBase64);
            var signingKey = new SymmetricSecurityKey(keyByteArray);
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            var now = DateTime.UtcNow;
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.Name, userId),
                new Claim(FishClaimTypes.StorageNo, storageNo),
                new Claim(FishClaimTypes.TerminalId, terminalId)
            };
            var jwt = new JwtSecurityToken(
                issuer: jwtSetting.Issuer,
                audience: jwtSetting.Audience,
                claims: claims,
                notBefore: now,
                expires: now.Add(TimeSpan.FromDays(jwtSetting.ExpireDays)),
                signingCredentials: signingCredentials
            );
            var token = new JwtSecurityTokenHandler().WriteToken(jwt);

            return token;
        }

    }
}
