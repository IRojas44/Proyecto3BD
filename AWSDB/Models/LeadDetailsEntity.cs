﻿using Microsoft.AspNetCore.Mvc.RazorPages;
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

    public class PlanillaSemanal
    {
        [Key]
        public int id { get; set; }
        public int salarioBruto { get; set; }

        public int totalDeducciones { get; set; }

        public int salarioNeto { get; set; }

        public int Horas { get; set; }

        public int HorasExtras { get; set; }

        public int HorasDobles { get; set; }
    }
    public class SemanalDeducciones
    {
        [Key]
        public int id { get; set; }

        public string nombre { get; set; }

        public decimal porcentaje { get; set; }

        public int monto { get; set; }
    }
    public class SemanalBruto
    {
        [Key]
        public int id { get; set; }

        public string diaSemana { get; set; }

        public string horaEntrada { get; set; }

        public string horaSalida { get; set; }

        public int horas { get; set; }

        public int montoHoras { get; set; }

        public int horasExtras { get; set; }

        public int montoExtras { get; set; }

        public int horasDobles { get; set; }

        public int montoDobles { get; set; }
    }

    public class PlanillaMensual
    {
        [Key]
        public int id { get; set; }
        public int salarioBruto { get; set; }

        public int totalDeducciones { get; set; }

        public int salarioNeto { get; set; }
    }

    public class MensualDeducciones
    {
        [Key]
        public int id { get; set; }

        public string nombre { get; set; }

        public decimal porcentaje { get; set; }

        public int monto { get; set; }
    }
}

public class CombinedViewModel
{
	public IEnumerable<LeadDetailsEntity> LeadDetails { get; set; }

    public IEnumerable<PlanillaSemanal> PlanillaSemanal { get; set; }
    public IEnumerable<SemanalBruto> SemanalBruto { get; set; }
    public IEnumerable<SemanalDeducciones> semanalDeducciones { get; set; }
    public IEnumerable<PlanillaMensual> PlanillaMensual { get; set; }
    public IEnumerable<MensualDeducciones> mensualDeducciones { get; set; }
    public NuevoEmpleado NuevoEmpleado { get; set; }
	public Articulo NewArticulo { get; set; }

    public Empleado NewEmpleado { get; set; }

    public List<AWSDB.Models.TipoDocumento> NewTD { get; set; }

    public List<AWSDB.Models.TipoPuesto> NewP { get; set; }

    public List<AWSDB.Models.TipoDepartamento> NewD { get; set; }

    public int showButton { get; set; }

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

    public int IdPlanilla { get; set; }
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