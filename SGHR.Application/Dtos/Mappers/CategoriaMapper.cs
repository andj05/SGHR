using SGHR.Application.Dtos.Categorias;
using SGHR.Domain.Entities.Configuration;

public static class CategoriaMapper
{
    // Convertir Categoria a CategoriasDto
    public static CategoriasDto ToDto(Categoria entity)
    {
        return new CategoriasDto
        {
            IdCategoria = entity.Id,
            Descripcion = entity.Descripcion,
            Estado = entity.Estado,
            FechaCreacion = entity.FechaCreacion,
            ChangeDate = entity.ModifyDate,
            ChangeUser = entity.ModifyUser
        };
    }

    // Convertir SaveCategoriasDto a una nueva Categoria
    public static Categoria ToEntity(SaveCategoriasDto dto)
    {
        return new Categoria
        {
            Descripcion = dto.Descripcion,
            Estado = dto.Estado,
            FechaCreacion = DateTime.UtcNow,
            CreationUser = dto.ChangeUser,
            Deleted = false
        };
    }

    // Actualizar Categoria existente desde UpdateCategoriasDto
    public static void UpdateFromDto(this Categoria entity, UpdateCategoriasDto dto)
    {
        entity.Descripcion = dto.Descripcion;
        entity.Estado = dto.Estado;
        entity.ModifyDate = dto.ChangeDate ?? DateTime.Now;
        entity.ModifyUser = dto.ChangeUser;
    }

    // Marcar como borrada Categoria existente desde RemoveCategoriasDto
    public static void RemoveFromDto(this Categoria entity, RemoveCategoriasDto dto)
    {
        entity.Estado = false;
        entity.Deleted = true;
        entity.ModifyDate = dto.ChangeDate ?? DateTime.Now;
        entity.ModifyUser = dto.ChangeUser ?? 1;
    }

    // Restaurar Categoria existente desde RestoreCategoriasDto
    public static void RestoreFromDto(this Categoria entity, int userId)
    {
        entity.Estado = true;
        entity.Deleted = false;
        entity.ModifyDate = DateTime.Now;
        entity.ModifyUser = userId;
    }
}



