using ApiPeliculas.Shared;

namespace ApiPeliculas.Feats.Users.DTOs;

/// <summary>
/// Respuesta del servidor al autenticarse
/// </summary>
public class UserLoginResponseDto
{
    public required User User { get; set; }
    public string Token { get; set; }
}
