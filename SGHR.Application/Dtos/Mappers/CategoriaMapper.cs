using SGHR.Application.Dtos.Categorias;
using SGHR.Domain.Entities.Configuration;

public static class CategoriaMapper
{
    // Convert Categoria entity to CategoriaDto
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

    // Convert SaveCategoriaDto to a new Categoria entity
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

    // Update existing Categoria entity from UpdateCategoriaDto
    public static void UpdateFromDto(this Categoria entity, UpdateCategoriasDto dto)
    {
        entity.Descripcion = dto.Descripcion;
        entity.Estado = dto.Estado;
        entity.ModifyDate = dto.ChangeDate;
        entity.ModifyUser = dto.ChangeUser;
    }

    // Update existing Categoria entity for Remove operation
    public static void RemoveFromDto(this Categoria entity, RemoveCategoriasDto dto)
    {
        entity.Estado = false;
        entity.Deleted = true;
        entity.ModifyDate = dto.ChangeDate;
        entity.ModifyUser = dto.ChangeUser;
    }

    // Update existing Categoria entity for Restore operation
    public static void RestoreFromDto(this Categoria entity, int userId)
    {
        entity.Estado = true;
        entity.Deleted = false;
        entity.ModifyDate = DateTime.Now;
        entity.ModifyUser = userId;
    }
}
