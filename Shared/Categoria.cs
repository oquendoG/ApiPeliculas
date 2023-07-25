using System.ComponentModel.DataAnnotations;

namespace ApiPeliculas.Shared;

public class Categoria
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public DateTimeOffset CreationDate { get; set; } = DateTimeOffset.UtcNow;
}
