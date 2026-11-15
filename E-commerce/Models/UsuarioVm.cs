namespace E_commerce.Models
{
    public class UsuarioVm
    {
        public string? Id { get; set; }
        public string? Email { get; set; }
        public bool IsAdmin { get; set; }
        
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public DateTime FecheRegistro { get; set; }
        public string NombreCompleto
        {
            get
            {
                return string.IsNullOrEmpty(Apellido) ? Nombre : $"{Nombre} {Apellido}";
            }
        } 

    }
}
