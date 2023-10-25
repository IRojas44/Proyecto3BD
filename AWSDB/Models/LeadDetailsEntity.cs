using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;
using AWSDB.Models;

namespace AWSDB.Models
{
	public class LeadDetailsEntity
	{
		[Key]
		public int id { get; set; }
		public string Nombre { get; set; }
		public string Puesto { get; set; }
	}

    public class TipoDocumento
    {
        [Key]
        public int id { get; set; }
        public string Nombre { get; set; }
    }

    public class TipoPuesto
    {
        [Key]
        public int id { get; set; }
        public string Nombre { get; set; }
    }

    public class TipoDepartamento
    {
        [Key]
        public int id { get; set; }
        public string Nombre { get; set; }
    }
}

public class CombinedViewModel
{
	public IEnumerable<LeadDetailsEntity> LeadDetails { get; set; }

    public NuevoEmpleado NuevoEmpleado { get; set; }
	public Articulo NewArticulo { get; set; }

    public Empleado NewEmpleado { get; set; }

    public List<AWSDB.Models.TipoDocumento> NewTD { get; set; }

    public List<AWSDB.Models.TipoPuesto> NewP { get; set; }

    public List<AWSDB.Models.TipoDepartamento> NewD { get; set; }

    public bool showButton { get; set; }

    public string Codigo { get; set;}

	public string UserName { get; set;}

    [Required]
    public string NombreAdmin { get; set; }

    [Required]
    public string PasswordAdmin { get; set; }

    [Required]
    public string NombreEmpleado { get; set; }

    [Required]
    public string PasswordEmpleado { get; set; }

    public int IdEmpleado { get; set; }
}

public class Articulo
{
	[Required]
	public int id { get; set; }

	[Required]
	public string Codigo { get; set; }

	[Required]
	public string Nombre { get; set; }
	[Required]
	public string ClaseArticulo { get; set; }

	[Required]
	public string Precio { get; set; }

	[Required]
	public string Cantidad { get; set; }
}

public class Empleado
{
    [Required]
    public int id { get; set; }

    [Required]
    public string Nombre { get; set; }

    [Required]
    public string Puesto { get; set; }

}

public class NuevoEmpleado
{
    [Required]
    public int id { get; set; }

    [Required]
    public string Nombre { get; set; }
    [Required]
    public string ValorIdentidad { get; set; }

    [Required]
    public string TipoIdentificacion { get; set; }

    [Required]
    public string Puesto { get; set; }

    [Required]
    public string Departamento { get; set; }
}

public class ArchivoViewModel
{
	public IFormFile Archivo { get; set; }
}

public class Usuario
{
	[Required]
	public string Nombre { get; set; }

	[Required]
	public string Password { get; set; }

    [Required]
    public string NombreEmpleado { get; set; }

    [Required]
    public string PasswordEmpleado { get; set; }
}