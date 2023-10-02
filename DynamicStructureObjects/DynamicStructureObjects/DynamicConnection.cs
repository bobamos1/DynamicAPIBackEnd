using DynamicSQLFetcher;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.IdentityModel.Tokens;
using ParserLib;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DynamicStructureObjects
{
    public record UserInfo(long userID, string username, string Email, byte[] passwordHash, byte[] passwordSalt);
    public static class DynamicConnection
    {
        internal static string apiKey { get; set; }
        internal static TimeSpan TokenLifetime = TimeSpan.FromHours(2);
        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
        private static UserInfo CreatePasswordHash(string username, string Email, string password)
        {
            using (var hmac = new HMACSHA512())
                return new UserInfo(-1, username, Email, hmac.Key, hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)));
        }

        private static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
        public static IEnumerable<long> ParseRoles(JwtSecurityToken token)
        {
            var rolesClaim = ParseClaim(token, ClaimTypes.Role);
            if (rolesClaim is null)
                return new long[0];
            return rolesClaim.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                    .Select(role => long.Parse(role.Trim()));
        }
        public static long ParseUserID(JwtSecurityToken token)
        {
            var userID = ParseClaim(token, ClaimTypes.UserData);
            if (userID is null)
                return -1;
            return long.Parse(userID);
        }
        public static JwtSecurityToken ParseClaim(string token)
        {
            if (token is null)
                return null;
            try
            {
                token = token.Substring(token.IndexOf(' ') + 1);
                return new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;
            }
            catch
            {
                return null;
            }
        }

        public static string ParseClaim(JwtSecurityToken token, string type)
        {
            var rolesClaim = token.Claims.FirstOrDefault(c => c.Type == type)?.Value;

            if (string.IsNullOrWhiteSpace(rolesClaim))
                return null;
            return rolesClaim;
        }
        public static string RefreshToken(string oldTokenString)
        {
            var oldToken = ParseClaim(oldTokenString);

            if (oldToken.ValidTo > DateTime.UtcNow)
                return null;
            return CreateToken(oldToken.Claims);

        }
        public static IEnumerable<Claim> getClaims(UserInfo userInfo, params long[] roles)
        {
            return new List<Claim>
            {
                new Claim(ClaimTypes.Name, userInfo.username),
                new Claim(ClaimTypes.Email, userInfo.Email),
                new Claim(ClaimTypes.UserData, userInfo.userID.ToString()),
                new Claim(ClaimTypes.Role, string.Join(',', roles))
            };
        }
        public static string CreateToken(UserInfo userInfo, params long[] roles)
        {
            return CreateToken(getClaims(userInfo, roles));
        }
        public static string CreateToken(IEnumerable<Claim> claims)
        {
            var token = new JwtSecurityToken(
            claims: claims,
                expires: DateTime.Now.Add(TokenLifetime),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(apiKey)), SecurityAlgorithms.HmacSha512Signature));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public static async Task<UserInfo> checkUserInfo(string Email, string password, Query readUserInfoQuery)
        {

            var userInfo = await DynamicController.executor.SelectValue<UserInfo>(readUserInfoQuery.setParam("Email", Email));
            if (userInfo is null)
                return null;
            if (!VerifyPasswordHash(password, userInfo.passwordHash, userInfo.passwordSalt))
                return null;
            return userInfo;
        }
        public static async Task<long[]> getRoles(Query getRolesQuery, long userID)
        {
            return (await DynamicController.executor.SelectArray<long>(getRolesQuery.setParam("UserID", userID))).ToArray();
        }
        public static async Task<IResult> makeConnection(string Email, string password, Query readUserInfoQuery, Query getRolesQuery = null, long defaultRole = -1)
        {
            var userInfo = await checkUserInfo(Email, password, readUserInfoQuery);
            if (userInfo is null)
                return Results.Forbid();
            var roles = new long[] { defaultRole };
            if (getRolesQuery is not null)
                roles = await getRoles(getRolesQuery, userInfo.userID);
            return Results.Ok(CreateToken(userInfo, roles));
        }
        public static async Task<IResult> makeConnection(string twoFactor, Query readUserInfoQuery, Query getRolesQuery = null, long defaultRole = -1)
        {
            var userInfo = await DynamicController.executor.SelectValue<UserInfo>(readUserInfoQuery.setParam("TowFactor", twoFactor));
            if (userInfo is null)
                return Results.Forbid();
            var roles = new long[] { defaultRole };
            if (getRolesQuery is not null)
                roles = await getRoles(getRolesQuery, userInfo.userID);
            return Results.Ok(CreateToken(userInfo, roles));
        }
        public static async Task<IResult> makeConnection2Factor(string Email, string password, Query readUserInfoQuery, Query write2Factor)
        {
            var userInfo = await checkUserInfo(Email, password, readUserInfoQuery);
            if (userInfo is null)
                return Results.Forbid();
            await DynamicController.executor.ExecuteQueryWithTransaction(write2Factor.setParam("twoFactor", new Random().Next(100000, 999999).ToString()));
            return Results.Ok();
        }
    }
}
