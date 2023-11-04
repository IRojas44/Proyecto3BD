using AWSDB.Data;
using AWSDB.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.IO;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting;

namespace AWSDB.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDBContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        string connectionString;
        //private List<LeadDetailsEntity> getArticulos;
        //private List<ClaseArticulo> getClaseArticulos;

        public HomeController(ILogger<HomeController> logger, ApplicationDBContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            connectionString = "server=tecdb.ctfxom3mv69f.us-east-2.rds.amazonaws.com,1433;Database=WebLeads_2;TrustServerCertificate=True;User ID=admin;Password=tecaws123;";
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult Index(string userAdmin, string userEmpleado)
        {
            CombinedViewModel views = new CombinedViewModel();

            var getEmpleados = _db.Articulo.FromSqlRaw("ObtenerEmpleados").ToList();
            var getTipoDocumento = _db.TipoDocumento.FromSqlRaw("ObtenerTipoDocumento").ToList(); //ObtenerTipoDocumento //ObtenerPuesto //ObtenerDepartamento
            var getPuesto = _db.TipoPuesto.FromSqlRaw("ObtenerPuesto").ToList();
            var getDepartamento = _db.TipoDepartamento.FromSqlRaw("ObtenerDepartamento").ToList();
            views.NewTD = getTipoDocumento;
            views.NewP = getPuesto;
            views.NewD = getDepartamento;
            views.LeadDetails = getEmpleados;
            views.NombreAdmin = userAdmin;
            views.NombreEmpleado = userEmpleado;
            return View(views);
        }

        public IActionResult IndexEmpleado(string userAdmin, string userEmpleado, bool mostrarBoton)
        {
            CombinedViewModel views = new CombinedViewModel();

            //var getArticulo = _db.Articulo.FromSqlRaw("ObtenerArticulos").ToList();
            //var getClaseArticulo = _db.ClaseArticulo.FromSqlRaw("ObtenerNombreClase").ToList();
            //views.NewCA = getClaseArticulo;
            //views.LeadDetails = getArticulo;
            views.NombreAdmin = userAdmin;
            views.NombreEmpleado = userEmpleado;
            views.showButton = mostrarBoton;
            return View(views);
        }
        public IActionResult IndexNombre(string user, string nombre)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("FiltroNombre", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@inUserName", user);
                    command.Parameters.AddWithValue("@inNombre", nombre);
                    command.Parameters.Add("@outResultCode", SqlDbType.Int).Direction = ParameterDirection.Output;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        List<LeadDetailsEntity> ListEmpleados = new List<LeadDetailsEntity>();
                        while (reader.Read())
                        {
                            LeadDetailsEntity resultEmpleado = new LeadDetailsEntity();
                            resultEmpleado.Nombre = reader["Nombre"].ToString();
                            resultEmpleado.Puesto = reader["Puesto"].ToString();

                            ListEmpleados.Add(resultEmpleado);
                        }

                        int resultCode = Convert.ToInt32(command.Parameters["@outResultCode"].Value);
                        connection.Close();


                        CombinedViewModel views = new CombinedViewModel();
                        views.LeadDetails = ListEmpleados;
                        views.NombreAdmin = user;
                        return View(views);
                    }
                }
            }
        }
        public IActionResult IndexN(CombinedViewModel model)
        {
            if (model.NewEmpleado.Nombre == null)
            {
                return RedirectToAction("Index", "Home", new { userAdmin = model.NombreAdmin, userEmpleado = model.NombreEmpleado});
            }
            return RedirectToAction("IndexNombre", "Home", new { user = model.NombreAdmin, nombre = model.NewEmpleado.Nombre});
        }
        public IActionResult VolverLog()
        {
            return RedirectToAction("Login", "Home");
        }
        public IActionResult Volver()
        {
            return RedirectToAction("Index", "Home");
        }

        public IActionResult VolverLogin(CombinedViewModel model)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("Salir", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    if(model.NombreAdmin != "") {
                        command.Parameters.AddWithValue("@inUserName", model.NombreAdmin);
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@inUserName", model.NombreEmpleado);
                    }
                    command.Parameters.Add("@outResultCode", SqlDbType.Int).Direction = ParameterDirection.Output;
                    command.ExecuteNonQuery();

                    return RedirectToAction("Login", "Home");

                }
            }

        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Create(string userAdmin)
        {
            CombinedViewModel views = new CombinedViewModel();
            var getEmpleados = _db.Articulo.FromSqlRaw("ObtenerEmpleados").ToList();
            var getTipoDocumento = _db.TipoDocumento.FromSqlRaw("ObtenerTipoDocumento").ToList(); //ObtenerTipoDocumento //ObtenerPuesto //ObtenerDepartamento
            var getPuesto = _db.TipoPuesto.FromSqlRaw("ObtenerPuesto").ToList();
            var getDepartamento = _db.TipoDepartamento.FromSqlRaw("ObtenerDepartamento").ToList();
            views.NewTD = getTipoDocumento;
            views.NewP = getPuesto;
            views.NewD = getDepartamento;
            views.LeadDetails = getEmpleados;
            views.NombreAdmin = userAdmin;
            return View(views);
        }

        public IActionResult CreateV(CombinedViewModel model)
        {
            return RedirectToAction("Create", "Home", new { userAdmin = model.NombreAdmin});
        }
        public IActionResult CreateEmpleado()
        {
            return RedirectToAction("CreateV", "Home");
        }

        public IActionResult VolverCreate(CombinedViewModel model)
        {

            return RedirectToAction("Index", "Home", new { userAdmin = model.NombreAdmin, userEmpleado = "" });
        }

        public IActionResult Editar(string userAdmin, int IdEmpleado)
        {
            
            CombinedViewModel views = new CombinedViewModel();
            var getTipoDocumento = _db.TipoDocumento.FromSqlRaw("ObtenerTipoDocumento").ToList(); //ObtenerTipoDocumento //ObtenerPuesto //ObtenerDepartamento
            var getPuesto = _db.TipoPuesto.FromSqlRaw("ObtenerPuesto").ToList();
            var getDepartamento = _db.TipoDepartamento.FromSqlRaw("ObtenerDepartamento").ToList();
            views.NewTD = getTipoDocumento;
            views.NewP = getPuesto;
            views.NewD = getDepartamento;
            views.IdEmpleado = IdEmpleado;
            views.NombreAdmin = userAdmin;
            return View(views);
        }

        public IActionResult EditarV(int id ,CombinedViewModel model)
        {
            return RedirectToAction("Editar", "Home", new { userAdmin = model.NombreAdmin, IdEmpleado = id});
        }
        public IActionResult EditarEmpleado()
        {
            return RedirectToAction("EditarV", "Home");
        }

        public IActionResult Erase(CombinedViewModel model)
        {
            string nombreAdmin = model.NombreAdmin;
            int IdEmpleado = Convert.ToInt32(model.IdEmpleado);


            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("BorrarEmpleado", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@inUserName", nombreAdmin);
                    command.Parameters.AddWithValue("@inIdEmpleado", IdEmpleado);
                    command.Parameters.Add("@outResultCode", SqlDbType.Int).Direction = ParameterDirection.Output;

                    command.ExecuteNonQuery();

                    int resultCode = Convert.ToInt32(command.Parameters["@outResultCode"].Value);
                    connection.Close();

                    if (resultCode == 50001 || resultCode == 50005)
                    {
                        TempData["Message"] = "Error en el borrado";
                        return RedirectToAction("Editar", "Home", new { userAdmin = nombreAdmin, IdEmpleado = IdEmpleado });
                    }
                    return RedirectToAction("Index", "Home", new { userAdmin = nombreAdmin, userEmpleado = "" });
                }
            }

        }

        public IActionResult CancelarModificar(CombinedViewModel model)
        {

            return RedirectToAction("Index", "Home", new { userAdmin = model.NombreAdmin, userEmpleado = "" });
        }
        public IActionResult Upload()
        {
            return View();
        }
        public IActionResult VistaUpload()
        {
            return RedirectToAction("Upload", "Home");
        }

        public IActionResult Ingresar(CombinedViewModel model)
        {
            string Username = model.NombreAdmin;
            string password = model.PasswordAdmin;


            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("Login", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@inNombre", Username);
                    command.Parameters.AddWithValue("@inPassword", password);
                    command.Parameters.Add("@outResultCode", SqlDbType.Int).Direction = ParameterDirection.Output;
                    command.Parameters.Add("@outAdminOEmpleado", SqlDbType.Int).Direction = ParameterDirection.Output;

                    command.ExecuteNonQuery();

                    int resultCode = Convert.ToInt32(command.Parameters["@outResultCode"].Value);
                    int adminOEmpleado = Convert.ToInt32(command.Parameters["@outAdminOEmpleado"].Value);

                    //int resultCode = 3335;
                    connection.Close();
                    if (resultCode == 50002)
                    {
                        TempData["Message"] = "Combinacion de usuario/password no existe en la BD";
                        return RedirectToAction("Login", "Home");
                    }
                    else if (adminOEmpleado == 50004){
                        TempData["Message"] = "Login de administrador exitoso";
                        return RedirectToAction("Index", "Home", new { userAdmin = Username, userEmpleado = "" });
                    }
                    else 
                    {
                        TempData["Message"] = "Login exitoso";
                        return RedirectToAction("IndexEmpleado", "Home", new { userAdmin = "", userEmpleado = Username });

                    }
                }
            }
        }

        public IActionResult Insertar(CombinedViewModel model)
        {
            string nombre = model.NuevoEmpleado.Nombre;
            string valorIdentificacion = model.NuevoEmpleado.ValorIdentidad;
            string tipoId = Request.Form["selectTipoId"];
            string puesto = Request.Form["selectPuesto"];
            string departamanto = Request.Form["selectDepartamento"];
            string username = model.NombreAdmin;
          


            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("AddEmpleado", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@inUserName", username);
                    command.Parameters.AddWithValue("@inNombre", nombre);
                    command.Parameters.AddWithValue("@inTipoDocuIdentidad", tipoId);
                    command.Parameters.AddWithValue("@inValorTipoDocumento", valorIdentificacion);
                    command.Parameters.AddWithValue("@inPuesto", puesto);
                    command.Parameters.AddWithValue("@inDepartamento", departamanto);
                   
                    command.Parameters.Add("@outResultCode", SqlDbType.Int).Direction = ParameterDirection.Output;

                    command.ExecuteNonQuery();

                    int resultCode = Convert.ToInt32(command.Parameters["@outResultCode"].Value);
                    connection.Close();

                    if (resultCode == 50002)
                    {
                        TempData["Message"] = "Valor del Documento Duplicado";
                        return RedirectToAction("Editar", "Home", new { userAdmin = username});
                    }
                    else if (resultCode == 0)
                    {
                        TempData["Message"] = "Se inserto correctamente";
                        return RedirectToAction("Index", "Home", new { userAdmin = username, userEmpleado = "" });
                    }
                    else
                    {
                        TempData["Message"] = "Error en la base de datos";
                        return RedirectToAction("Editar", "Home", new { userAdmin = username});
                    }
                }
            }
        }

        public IActionResult Modificar(CombinedViewModel model)
        {
            string nombre = model.NuevoEmpleado.Nombre;
            string valorIdentificacion = model.NuevoEmpleado.ValorIdentidad;
            string tipoId = Request.Form["selectTipoId"];
            string puesto = Request.Form["selectPuesto"];
            string departamanto = Request.Form["selectDepartamento"];
            string username = model.NombreAdmin;
            int IdEmpleado = Convert.ToInt32(model.IdEmpleado);


            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("ModificarEmpleado", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@inUserName", username);
                    command.Parameters.AddWithValue("@inNombre", nombre);
                    command.Parameters.AddWithValue("@inTipoDocuIdentidad", tipoId);
                    command.Parameters.AddWithValue("@inValorTipoDocumentoNuevo", valorIdentificacion);
                    command.Parameters.AddWithValue("@inPuesto", puesto);
                    command.Parameters.AddWithValue("@inDepartamento", departamanto);
                    command.Parameters.AddWithValue("@inIdEmpleado", IdEmpleado);
                    command.Parameters.Add("@outResultCode", SqlDbType.Int).Direction = ParameterDirection.Output;

                    command.ExecuteNonQuery();

                    int resultCode = Convert.ToInt32(command.Parameters["@outResultCode"].Value);
                    connection.Close();

                    if (resultCode == 50002)
                    {
                        TempData["Message"] = "Valor del Documento Duplicado";
                        return RedirectToAction("Editar", "Home", new { userAdmin = username, IdEmpleado = IdEmpleado });
                    }
                    else if (resultCode ==0) {
                        TempData["Message"] = "Se inserto correctamente";
                        return RedirectToAction("Index", "Home", new { userAdmin = username, userEmpleado = "" });
                    }
                    else
                    {
                        TempData["Message"] = "Error en la base de datos";
                        return RedirectToAction("Editar", "Home", new { userAdmin = username, IdEmpleado = IdEmpleado });
                    }
           
                }
            }
        }

        public bool validarDatos(string codigo, string nombre, string precio)
        {
            if (nombre == null || precio == null || codigo == null) { return false; }
            var regex = @"^(?!.*\s{2,})[a-zA-Z\-][a-zA-Z\- ]*[a-zA-Z\-]$";
            var match = Regex.Match(nombre, regex, RegexOptions.IgnoreCase);
            var regex2 = @"^(?:\d+|\d+\.\d+)$";
            var match2 = Regex.Match(precio, regex2, RegexOptions.IgnoreCase);
            if (match.Success && match2.Success)
            {
                return true;
            }
            return false;
        }

        public bool validarEntero(string cantidad)
        {

            var regex = @"^-?\d+$";
            var match = Regex.Match(cantidad, regex, RegexOptions.IgnoreCase);

            if (match.Success)
            {
                return true;
            }
            return false;
        }

        public async Task<IActionResult> UploadCatalogo(ArchivoViewModel model)
        {
            if (model.Archivo != null && model.Archivo.Length > 0)
            {
                // Leer el contenido del archivo XML
                using (var reader = new StreamReader(model.Archivo.OpenReadStream()))
                {
                    string xmlContent = await reader.ReadToEndAsync();

                    // Llamar al Stored Procedure con el contenido XML como parámetro
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        using (SqlCommand command = new SqlCommand("ProcesarXmlCatalogo", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            // Pasa el contenido XML como parámetro
                            command.Parameters.AddWithValue("@inDatos", xmlContent);

                            // Configura el parámetro de salida para capturar la contraseña
                            command.Parameters.Add("OutResult", SqlDbType.VarChar, 128).Direction = ParameterDirection.Output;

                            command.ExecuteNonQuery();


                            // Realiza acciones adicionales si es necesario
                            connection.Close();
                            TempData["Message"] = "Carga de archivo exitosa y procesamiento del catalogo XML completado.";
                            return RedirectToAction("Login"); // Redirecciona a la página principal u otra página
                        }
                    }
                }
            }

            // Maneja el caso en que no se seleccionó ningún archivo
            TempData["Message"] = "Por favor, seleccione un archivo.";
            return RedirectToAction("VistaUpload");
        }
        public async Task<IActionResult> UploadSimular(ArchivoViewModel model)
        {
            if (model.Archivo != null && model.Archivo.Length > 0)
            {
                // Leer el contenido del archivo XML
                using (var reader = new StreamReader(model.Archivo.OpenReadStream()))
                {
                    string xmlContent = await reader.ReadToEndAsync();

                    // Llamar al Stored Procedure con el contenido XML como parámetro
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        using (SqlCommand command = new SqlCommand("ProcesarXmlSimular", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            // Pasa el contenido XML como parámetro
                            command.Parameters.AddWithValue("@inDatos", xmlContent);

                            // Configura el parámetro de salida para capturar la contraseña
                            command.Parameters.Add("@outResult", SqlDbType.VarChar, 128).Direction = ParameterDirection.Output;

                            command.ExecuteNonQuery();

                            // Obtener la contraseña del parámetro de salida
                            string ERROR = Convert.ToString(command.Parameters["@outResult"].Value);

                            // Realiza acciones adicionales si es necesario
                            connection.Close();
                            TempData["Message"] = "Carga de archivo exitosa y procesamiento XML completado.";
                            return RedirectToAction("Login"); // Redirecciona a la página principal u otra página
                        }
                    }
                }
            }

            // Maneja el caso en que no se seleccionó ningún archivo
            TempData["Message"] = "Por favor, seleccione un archivo.";
            return RedirectToAction("VistaUpload");
        }

        public IActionResult prueba(CombinedViewModel model)
        {
            string UsernameAdmin = model.NombreAdmin;
            string passwordAdmin = model.PasswordAdmin;

            string UsernameEmpleado = model.NombreEmpleado;
            string passwordEmpleado = model.PasswordEmpleado;


            return RedirectToAction("IndexEmpleado", "Home", new { userAdmin = UsernameAdmin, userEmpleado = UsernameEmpleado, mostrarBoton=true});
        }

        //---------------------------------------------------FUNCIONES EMPLEADO---------------------------------------------------
       

        public IActionResult IndexConsulMensual(string userEmpleado)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("ObtenerPlanillaMensual", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@inUserName", userEmpleado);
                    command.Parameters.Add("@outResultCode", SqlDbType.Int).Direction = ParameterDirection.Output;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        List<PlanillaMensual> ConsulMensual = new List<PlanillaMensual>();
                        while (reader.Read())
                        {
                            PlanillaMensual result = new PlanillaMensual();
                            result.id = Convert.ToInt32(reader["id"]);
                            result.salarioBruto = Convert.ToInt32(reader["salarioTotal"]);
                            result.totalDeducciones = Convert.ToInt32(reader["totalDeducciones"]);
                            result.salarioNeto = Convert.ToInt32(reader["salarioNeto"]);

                            ConsulMensual.Add(result);
                        }

                        int resultCode = Convert.ToInt32(command.Parameters["@outResultCode"].Value);
                        connection.Close();


                        CombinedViewModel views = new CombinedViewModel();
                        views.PlanillaMensual = ConsulMensual;
                        views.NombreAdmin = "";
                        views.NombreEmpleado = userEmpleado;
                        return View(views);
                    }
                }
            }
        }

        public IActionResult IndexConsulMensualV(CombinedViewModel model)
        {
            if (model.NombreEmpleado == null)
            {
                return RedirectToAction("IndexEmpleado", "Home", new { userAdmin = model.NombreAdmin, userEmpleado = model.NombreEmpleado });
            }
            return RedirectToAction("IndexConsulMensual", "Home", new { userEmpleado = model.NombreEmpleado });
        }

        public IActionResult MensualDeducciones(string userEmpleado, int idPlanilla)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("ObtenerPlanillaMensual", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@inUserName", userEmpleado);
                    command.Parameters.AddWithValue("@inIdPlanillaMensual", idPlanilla);
                    command.Parameters.Add("@outResultCode", SqlDbType.Int).Direction = ParameterDirection.Output;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        List<MensualDeducciones> DeduccionesMensual = new List<MensualDeducciones>();
                        while (reader.Read())
                        {
                            MensualDeducciones result = new MensualDeducciones();
                            result.id = Convert.ToInt32(reader["id"]);
                            result.nombre = reader["Nombre"].ToString();
                            result.porcentaje = Convert.ToInt32(reader["Porcentaje"]);
                            result.monto = Convert.ToInt32(reader["Monto"]);

                            DeduccionesMensual.Add(result);
                        }

                        int resultCode = Convert.ToInt32(command.Parameters["@outResultCode"].Value);
                        connection.Close();


                        CombinedViewModel views = new CombinedViewModel();
                        views.mensualDeducciones = DeduccionesMensual;
                        views.NombreAdmin = "";
                        views.NombreEmpleado = userEmpleado;
                        return View(views);
                    }
                }
            }
        }
        public IActionResult MensualDeduccionesV(int id, CombinedViewModel model)
        {
            return RedirectToAction("MensualDeducciones", "Home", new { userEmpleado = model.NombreEmpleado, idPlanilla = id });
        }

        public IActionResult IndexConsulSem(string userEmpleado)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("ObtenerPlanillaMensual", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@inUserName", userEmpleado);
                    command.Parameters.Add("@outResultCode", SqlDbType.Int).Direction = ParameterDirection.Output;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        List<PlanillaSemanal> ConsulSemanal = new List<PlanillaSemanal>();
                        while (reader.Read())
                        {
                            PlanillaSemanal result = new PlanillaSemanal();
                            result.id = Convert.ToInt32(reader["id"]);
                            result.salarioBruto = Convert.ToInt32(reader["salarioBruto"]);
                            result.totalDeducciones = Convert.ToInt32(reader["totalDeducciones"]);
                            result.salarioNeto = Convert.ToInt32(reader["salarioNeto"]);
                            result.Horas = Convert.ToInt32(reader["Horas"]);
                            result.HorasExtras = Convert.ToInt32(reader["HorasExtras"]);
                            result.HorasDobles = Convert.ToInt32(reader["HorasDobles"]);

                            ConsulSemanal.Add(result);
                        }

                        int resultCode = Convert.ToInt32(command.Parameters["@outResultCode"].Value);
                        connection.Close();


                        CombinedViewModel views = new CombinedViewModel();
                        views.PlanillaSemanal = ConsulSemanal;
                        views.NombreAdmin = "";
                        views.NombreEmpleado = userEmpleado;
                        return View(views);
                    }
                }
            }
        }

        public IActionResult IndexConsulSemV(CombinedViewModel model)
        {
            if (model.NombreEmpleado == null)
            {
                return RedirectToAction("IndexEmpleado", "Home", new { userAdmin = model.NombreAdmin, userEmpleado = model.NombreEmpleado });
            }
            return RedirectToAction("IndexConsulSem", "Home", new { userEmpleado = model.NombreEmpleado });
        }

        public IActionResult SemanalBruto(string userEmpleado, int idPlanilla)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("ObtenerSalarioBrutoSemanal", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@inUserName", userEmpleado);
                    command.Parameters.AddWithValue("@inIdPlanillaSemanal", idPlanilla);
                    command.Parameters.Add("@outResultCode", SqlDbType.Int).Direction = ParameterDirection.Output;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        List<SemanalBruto> semanalBruto = new List<SemanalBruto>();
                        while (reader.Read())
                        {
                            SemanalBruto result = new SemanalBruto();
                            result.id = Convert.ToInt32(reader["id"]);
                            result.diaSemana = reader["diaSemana"].ToString();
                            result.horaEntrada = reader["horaEntrada"].ToString();
                            result.horaSalida = reader["horaSalida"].ToString();
                            result.horas = Convert.ToInt32(reader["horas"]);
                            result.montoHoras = Convert.ToInt32(reader["montoHoras"]);
                            result.horasExtras = Convert.ToInt32(reader["horasExtras"]);
                            result.montoExtras = Convert.ToInt32(reader["montoExtras"]);
                            result.horasDobles = Convert.ToInt32(reader["horasDobles"]);
                            result.montoDobles = Convert.ToInt32(reader["montoDobles"]);

                            semanalBruto.Add(result);
                        }

                        int resultCode = Convert.ToInt32(command.Parameters["@outResultCode"].Value);
                        connection.Close();


                        CombinedViewModel views = new CombinedViewModel();
                        views.SemanalBruto = semanalBruto;
                        views.NombreAdmin = "";
                        views.NombreEmpleado = userEmpleado;
                        return View(views);
                    }
                }
            }
        }
        public IActionResult SemDeduccionesV(int id, CombinedViewModel model)
        {
            return RedirectToAction("SemanalBruto", "Home", new { userEmpleado = model.NombreEmpleado, idPlanilla = id });
        }

        public IActionResult SemanalDeducciones(string userEmpleado, int idPlanilla)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("ObtenerDeduccionSemanal", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@inUserName", userEmpleado);
                    command.Parameters.AddWithValue("@inIdPlanillaMensual", idPlanilla);
                    command.Parameters.Add("@outResultCode", SqlDbType.Int).Direction = ParameterDirection.Output;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        List<SemanalDeducciones> DeduccionesSemanal = new List<SemanalDeducciones>();
                        while (reader.Read())
                        {
                            SemanalDeducciones result = new SemanalDeducciones();
                            result.id = Convert.ToInt32(reader["id"]);
                            result.nombre = reader["Nombre"].ToString();
                            result.porcentaje = Convert.ToInt32(reader["Porcentaje"]);
                            result.monto = Convert.ToInt32(reader["Monto"]);

                            DeduccionesSemanal.Add(result);
                        }

                        int resultCode = Convert.ToInt32(command.Parameters["@outResultCode"].Value);
                        connection.Close();


                        CombinedViewModel views = new CombinedViewModel();
                        views.semanalDeducciones = DeduccionesSemanal;
                        views.NombreAdmin = "";
                        views.NombreEmpleado = userEmpleado;
                        return View(views);
                    }
                }
            }
        }
        public IActionResult SemBrutoV(int id, CombinedViewModel model)
        {
            return RedirectToAction("SemanalDeducciones", "Home", new { userEmpleado = model.NombreEmpleado, idPlanilla = id });
        }
    }
}