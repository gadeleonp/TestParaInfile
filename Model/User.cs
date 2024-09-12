public class User
{
    [Required(ErrorMessage = "El nombre es requerido.")]
    [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "El nombre solo puede contener letras.")]
    public string Nombre { get; set; }

    [Required(ErrorMessage = "El email es requerido.")]
    [EmailAddress(ErrorMessage = "El email no es válido.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "La contraseña es requerida.")]
    [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
    public string Password { get; set; }

    [Required(ErrorMessage = "La confirmación de contraseña es requerida.")]
    [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
    public string PasswordConfirmation { get; set; }
}
