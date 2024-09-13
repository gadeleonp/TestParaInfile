using FluentValidation;
using TestInfileGAdLP.Models;

namespace TestInfileGAdLP
{
    public class UsuarioValidator : AbstractValidator<Usuario>
    {
        public UsuarioValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Formato de correo electrónico inválido.");
            RuleFor(x => x.Password).NotEmpty().MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.");
            RuleFor(x => x.Nombre).NotEmpty().WithMessage("El nombre es obligatorio.");
        }
    }
}
