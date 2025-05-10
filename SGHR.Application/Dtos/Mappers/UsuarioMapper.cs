using SGHR.Application.Dtos.Usuario;
using SGHR.Domain.Entities.Users;
using System;

namespace SGHR.Application.Mappers
{
    public static class UsuarioMapper
    {
        public static UsuarioDto ToDto(Usuario entity)
        {
            return new UsuarioDto
            {
                IdUsuario = entity.Id,
                NombreCompleto = entity.NombreCompleto,
                Correo = entity.Correo,
                IdRolUsuario = entity.IdRolUsuario,
                ChangeDate = entity.ModifyDate,
                ChangeUser = entity.ModifyUser,
                Estado = !entity.Deleted
            };
        }

        public static Usuario ToEntity(SaveUsuarioDto dto)
        {
            return new Usuario
            {
                NombreCompleto = dto.NombreCompleto,
                Correo = dto.Correo,
                Clave = dto.Clave,
                IdRolUsuario = dto.IdRolUsuario,
            };
        }

        public static void UpdateFromDto(this Usuario entity, UpdateUsuarioDto dto)
        {
            entity.NombreCompleto = dto.NombreCompleto;
            entity.Correo = dto.Correo;
            entity.IdRolUsuario = dto.IdRolUsuario;
            if (!string.IsNullOrEmpty(dto.Clave))
                entity.Clave = dto.Clave;
            entity.ModifyDate = DateTime.Now;
            entity.ModifyUser = dto.ChangeUser ?? 1;
        }

        public static void RemoveFromDto(this Usuario entity, RemoveUsuarioDto dto)
        {
            entity.Deleted = true;
            entity.ModifyDate = dto.ChangeDate ?? DateTime.Now;
            entity.ModifyUser = dto.ChangeUser ?? 1;
        }

        public static void RestoreFromDto(this Usuario entity, int userId)
        {
            entity.Deleted = false;
            entity.ModifyDate = DateTime.Now;
            entity.ModifyUser = userId;
        }
    }
}