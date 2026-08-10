using PharmacyContracts.Modules.Auth.Application.DTOs;
using PharmacyContracts.Modules.Auth.Domain.Entities;
using PharmacyContracts.Modules.Auth.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyContracts.Modules.Auth.Application.Mappings
{
    public static class UserMappings
    {
        public static UserResponseDto ToResponseDto(this User user)
        {
            return new UserResponseDto
            {
                Id = user.Id,
                Email = user.Email,
                Role = user.Role.ToString(),
                IsActive = user.IsActive,
                PharmacyName = user.PharmacyName,
                CreatedAt = user.CreatedAt
            };
        }

        public static User ToEntity(this CreateUserRequestDto dto, string passwordHash, UserRole role)
        {
            return new User
            {
                Email = dto.Email,
                PasswordHash = passwordHash,
                Role = role,
                PharmacyName = role == UserRole.Pharmacy ? dto.PharmacyName : null,
                IsActive = true
            };
        }
    }
}
