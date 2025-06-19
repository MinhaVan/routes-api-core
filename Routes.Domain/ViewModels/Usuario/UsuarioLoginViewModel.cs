namespace Routes.Domain.ViewModels
{
    public class UsuarioLoginViewModel
    {
        public string RefreshToken { get; set; }
        public string CPF { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public bool IsMotorista { get; set; } = false;
    }
}