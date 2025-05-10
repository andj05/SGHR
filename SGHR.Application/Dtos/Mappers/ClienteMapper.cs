using SGHR.Application.Dtos.Cliente;
using SGHR.Domain.Entities.Users;

namespace SGHR.Application.Mappers
{
    public static class ClienteMapper
    {
        public static ClienteDto ToDto(Cliente entity)
        {
            return new ClienteDto
            {
                IdCliente = entity.Id,
                TipoDocumento = entity.TipoDocumento,
                Documento = entity.Documento,
                NombreCompleto = entity.NombreCompleto,
                Correo = entity.Correo,
                Telefono = entity.Telefono,
                Nacionalidad = entity.Nacionalidad,
                ChangeDate = entity.ModifyDate,
                ChangeUser = entity.ModifyUser,
                Estado = entity.Estado,
            };
        }

        public static Cliente ToEntity(SaveClienteDto dto)
        {
            return new Cliente
            {
                TipoDocumento = dto.TipoDocumento,
                Documento = dto.Documento,
                NombreCompleto = dto.NombreCompleto,
                Correo = dto.Correo,
                Clave = dto.Clave,
                Telefono = dto.Telefono,
                Nacionalidad = dto.Nacionalidad,
                CreationUser = dto.ChangeUser,
                FechaCreacion = DateTime.Now,
                Deleted = false,
            };
        }

        public static void UpdateFromDto(this Cliente entity, UpdateClienteDto dto)
        {
            entity.TipoDocumento = dto.TipoDocumento;
            entity.Documento = dto.Documento;
            entity.NombreCompleto = dto.NombreCompleto;
            entity.Correo = dto.Correo;
            if (!string.IsNullOrEmpty(dto.Clave))
                entity.Clave = dto.Clave;
            entity.Telefono = dto.Telefono;
            entity.Nacionalidad = dto.Nacionalidad;
            entity.Estado = dto.Estado;
            entity.ModifyDate = DateTime.Now;
            entity.ModifyUser = dto.ChangeUser ?? 1;
        }

        public static void RemoveFromDto(this Cliente entity, RemoveClienteDto dto)
        {
            entity.Estado = false;
            entity.Deleted = true;
            entity.ModifyUser = dto.ChangeUser ?? 1;
            entity.DeletedUser = dto.ChangeUser ?? 1;
        }

        public static void RestoreFromDto(this Cliente entity, int userId)
        {
            entity.Estado = true;
            entity.Deleted = false;
            entity.ModifyDate = DateTime.Now;
            entity.ModifyUser = userId;
            entity.DeletedUser = null;
        }
    }
}