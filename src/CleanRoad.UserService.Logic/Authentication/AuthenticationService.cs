﻿using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CleanRoad.UserService.Entities;
using CleanRoad.UserService.Logic.Abstractions.Authentication;
using CleanRoad.UserService.Logic.Abstractions.Cryptography;
using CleanRoad.UserService.Repositories.Abstractions;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using CleanRoad.UserService.Constants;

namespace CleanRoad.UserService.Logic.Authentication
{
    public class AuthenticationService : IAuthService, IResourceOwnerPasswordValidator, IProfileService
    {
        private readonly ITbUsersRepository tbUsersRepository;
        private readonly IHasher hasher;

        public AuthenticationService(ITbUsersRepository tbUsersRepository, IHasher hasher)
        {
            this.tbUsersRepository = tbUsersRepository;
            this.hasher = hasher;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            if (!int.TryParse(context.Subject.Claims.FirstOrDefault(claim => claim.Type == JwtClaimTypes.Subject)?.Value, out var subjectId))
            {
                context.IssuedClaims = context.Subject.Claims.ToList();
                return;
            }

            var user = await this.tbUsersRepository.FindAsync(subjectId);

            if (user == null)
            {
                context.IssuedClaims = context.Subject.Claims.ToList();
                return;
            }

            context.IssuedClaims = AllocateClaims(user)
                .ToList();
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            return Task.FromResult(context.IsActive);
        }
        
        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var loginUser = await this.tbUsersRepository.FindUserByUserNameOrEmailAsync(context.UserName);

            if (loginUser == null)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant);
                return;
            }

            if (!this.hasher.ValidatePasswordEquality(context.Password, loginUser.Password))
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant);
                return;
            }

            context.Result = new GrantValidationResult(loginUser.Jid.ToString(), 
                "custom", 
                AllocateClaims(loginUser));
        }

        private static IEnumerable<Claim> AllocateClaims(TbUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtClaimTypes.PreferredUserName, user.StrUserId),
                new Claim(JwtClaimTypes.Role, RoleNames.Player)
            };

            if (!string.IsNullOrEmpty(user.Name))
            {
                claims.Add(new Claim(JwtClaimTypes.Name, user.Name));
            }

            return claims;
        }
    }
}