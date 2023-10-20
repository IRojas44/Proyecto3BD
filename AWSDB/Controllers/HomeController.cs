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
            connectionString = "server=tecdb.ctfxom3mv69f.us-east-2.rds.amazonaws.com,1433;Database=WebLeads;TrustServerCertificate=True;User ID=admin;Password=tecaws123;";
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
        public IActionResult Index(string user)
        {
            CombinedViewModel views = new CombinedViewModel();

            var getArticulo = _db.Articulo.FromSqlRaw("ObtenerArticulos").ToList();
            var getClaseArticulo = _db.ClaseArticulo.FromSqlRaw("ObtenerNombreClase").ToList();
            views.NewCA = getClaseArticulo;
            views.LeadDetails = getArticulo;
            views.UserName = user;
            return View(views);
        }

        public IActionResult IndexEmpleado(string userAdmin, string userEmpleado, bool mostrarBoton)
        {
            CombinedViewModel views = new CombinedViewModel();

            var getArticulo = _db.Articulo.FromSqlRaw("ObtenerArticulos").ToList();
            var getClaseArticulo = _db.ClaseArticulo.FromSqlRaw("ObtenerNombreClase").ToList();
            views.NewCA = getClaseArticulo;
            views.LeadDetails = getArticulo;
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
                        List<LeadDetailsEntity> ListArticulo = new List<LeadDetailsEntity>();
                        while (reader.Read())
                        {
                            LeadDetailsEntity resultArticulo = new LeadDetailsEntity();
                            resultArticulo.Codigo = reader["Codigo"].ToString();
                            resultArticulo.Nombre = reader["Nombre"].ToString();
                            resultArticulo.ClaseArticulo = reader["ClaseArticulo"].ToString();
                            resultArticulo.Precio = Convert.ToDecimal(reader["Precio"].ToString());

                            ListArticulo.Add(resultArticulo);
                        }

                        int resultCode = Convert.ToInt32(command.Parameters["@outResultCode"].Value);
                        connection.Close();


                        CombinedViewModel views = new CombinedViewModel();
                        var getClaseArticulo = _db.ClaseArticulo.FromSqlRaw("ObtenerNombreClase").ToList();
                        views.NewCA = getClaseArticulo;
                        views.LeadDetails = ListArticulo;
                        views.UserName = user;
                        return View(views);
                    }
                }
            }
        }
        public IActionResult IndexN(CombinedViewModel model)
        {
            if (model.NewArticulo.Nombre == null)
            {
                return RedirectToAction("Index", "Home", new { user = model.UserName });
            }
            return RedirectToAction("IndexNombre", "Home", new { user = model.UserName, nombre = model.NewArticulo.Nombre });
        }
        public IActionResult IndexCantidad(string user, int cantidad)

        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("FiltroCantidad", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@inUserName", user);
                    command.Parameters.AddWithValue("@inCantidad", cantidad);
                    command.Parameters.Add("@outResultCode", SqlDbType.Int).Direction = ParameterDirection.Output;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        List<LeadDetailsEntity> ListArticulo = new List<LeadDetailsEntity>();
                        while (reader.Read())
                        {
                            LeadDetailsEntity resultArticulo = new LeadDetailsEntity();
                            resultArticulo.Codigo = reader["Codigo"].ToString();
                            resultArticulo.Nombre = reader["Nombre"].ToString();
                            resultArticulo.ClaseArticulo = reader["ClaseArticulo"].ToString();
                            resultArticulo.Precio = Convert.ToDecimal(reader["Precio"].ToString());

                            ListArticulo.Add(resultArticulo);
                        }

                        int resultCode = Convert.ToInt32(command.Parameters["@outResultCode"].Value);
                        connection.Close();


                        CombinedViewModel views = new CombinedViewModel();
                        var getClaseArticulo = _db.ClaseArticulo.FromSqlRaw("ObtenerNombreClase").ToList();
                        views.NewCA = getClaseArticulo;
                        views.LeadDetails = ListArticulo;
                        views.UserName = user;
                        return View(views);
                    }
                }
            }
        }
        public IActionResult IndexC(CombinedViewModel model)
        {
            if (model.NewArticulo.Cantidad == null)
            {
                return RedirectToAction("Index", "Home", new { user = model.UserName });
            }
            if (validarEntero(model.NewArticulo.Cantidad) == true)
            {
                return RedirectToAction("IndexCantidad", "Home", new { user = model.UserName, cantidad = Convert.ToInt32(model.NewArticulo.Cantidad) });
            }
            TempData["Message"] = "La cantidad debe de ser un numero entero";
            return RedirectToAction("Index", "Home", new { user = model.UserName });
        }
        public IActionResult IndexClase(string user, string clase)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("FiltroClase", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@inUserName", user);
                    command.Parameters.AddWithValue("@inNombreClase", clase);
                    command.Parameters.Add("@outResultCode", SqlDbType.Int).Direction = ParameterDirection.Output;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        List<LeadDetailsEntity> ListArticulo = new List<LeadDetailsEntity>();
                        while (reader.Read())
                        {
                            LeadDetailsEntity resultArticulo = new LeadDetailsEntity();
                            resultArticulo.Codigo = reader["Codigo"].ToString();
                            resultArticulo.Nombre = reader["Nombre"].ToString();
                            resultArticulo.ClaseArticulo = reader["ClaseArticulo"].ToString();
                            resultArticulo.Precio = Convert.ToDecimal(reader["Precio"].ToString());

                            ListArticulo.Add(resultArticulo);
                        }

                        int resultCode = Convert.ToInt32(command.Parameters["@outResultCode"].Value);
                        connection.Close();


                        CombinedViewModel views = new CombinedViewModel();
                        var getClaseArticulo = _db.ClaseArticulo.FromSqlRaw("ObtenerNombreClase").ToList();
                        views.NewCA = getClaseArticulo;
                        views.LeadDetails = ListArticulo;
                        views.UserName = user;
                        return View(views);
                    }
                }
            }
        }
        public IActionResult IndexCl(CombinedViewModel model)
        {
            return RedirectToAction("IndexClase", "Home", new { user = model.UserName, clase = Request.Form["selectClase"] });
        }
        /*public IActionResult Index()
        {
            CombinedViewModel views = new CombinedViewModel();
            var getArticulo = (List<LeadDetailsEntity>)ViewBag.Articulos;
            var getClaseArticulo = (List<ClaseArticulo>)ViewBag.ClaseArticulos;
            views.NewCA = getClaseArticulo;
            views.LeadDetails = getArticulo;
            return View(views);
        }

     
        public CombinedViewModel IndexFiltro(List<LeadDetailsEntity> listaArticulo)
        {
            CombinedViewModel views = new CombinedViewModel();
            views.LeadDetails = listaArticulo;

            return views;
        }*/
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

                    command.Parameters.AddWithValue("@inUserName", model.UserName);
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

        public IActionResult Create(string user)
        {
            var getClaseArticulo = _db.ClaseArticulo.FromSqlRaw("ObtenerNombreClase").ToList();
            CombinedViewModel views = new CombinedViewModel();
            views.NewCA = getClaseArticulo;
            views.UserName = user;
            return View(views);
        }

        public IActionResult CreateV(CombinedViewModel model)
        {
            return RedirectToAction("Create", "Home", new { user = model.UserName });
        }
        public IActionResult CreateEmpleado()
        {
            return RedirectToAction("CreateV", "Home");
        }

        public IActionResult VolverCreate(CombinedViewModel model)
        {

            return RedirectToAction("Index", "Home", new { user = model.UserName });
        }
        public IActionResult ModifyValidation(string user)
        {
            CombinedViewModel views = new CombinedViewModel();
            views.UserName = user;
            return View(views);
        }

        public IActionResult ModifyV(CombinedViewModel model)
        {
            return RedirectToAction("ModifyValidation", "Home", new { user = model.UserName });
        }

        public IActionResult Modify(string user, string code)
        {
            var getClaseArticulo = _db.ClaseArticulo.FromSqlRaw("ObtenerNombreClase").ToList();
            CombinedViewModel views = new CombinedViewModel();
            views.NewCA = getClaseArticulo;
            views.Codigo = code;
            views.UserName = user;
            return View(views);
        }
        public IActionResult ModifyArticle()
        {
            return RedirectToAction("Modify", "Home");
        }

        public IActionResult VolverModify(CombinedViewModel model)
        {

            return RedirectToAction("Index", "Home", new { user = model.UserName });
        }

        public IActionResult Editar(string user)
        {
            var getClaseArticulo = _db.ClaseArticulo.FromSqlRaw("ObtenerNombreClase").ToList();
            CombinedViewModel views = new CombinedViewModel();
            views.NewCA = getClaseArticulo;
            views.UserName = user;
            return View(views);
        }

        public IActionResult EditarV(CombinedViewModel model)
        {
            return RedirectToAction("Editar", "Home", new { user = model.UserName });
        }
        public IActionResult EditarEmpleado()
        {
            return RedirectToAction("EditarV", "Home");
        }

        public IActionResult EraseValidation(string user)
        {
            CombinedViewModel views = new CombinedViewModel();
            views.UserName = user;
            return View(views);
        }
        public IActionResult EraseV(CombinedViewModel model)
        {
            return RedirectToAction("EraseValidation", "Home", new { user = model.UserName });
        }
        public IActionResult Erase(string user, string code)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("ObtenerArticuloCodigo", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@inCodigo", code);
                    command.Parameters.AddWithValue("@inUserName", user);
                    command.Parameters.Add("@outResultCode", SqlDbType.Int).Direction = ParameterDirection.Output;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        Articulo resultArticulo = new Articulo();
                        while (reader.Read())
                        {
                            resultArticulo.Codigo = reader["Codigo"].ToString();
                            resultArticulo.Nombre = reader["Nombre"].ToString();
                            resultArticulo.ClaseArticulo = reader["ClaseArticulo"].ToString();
                            resultArticulo.Precio = reader["Precio"].ToString();

                        }

                        int resultCode = Convert.ToInt32(command.Parameters["@outResultCode"].Value);
                        connection.Close();

                        CombinedViewModel views = new CombinedViewModel();
                        views.NewArticulo = resultArticulo;
                        views.Codigo = code;
                        views.UserName = user;
                        return View(views);
                    }
                }
            }

        }
        public IActionResult EraseArticle()
        {
            return RedirectToAction("Erase", "Home");
        }

        public IActionResult VolverErase(CombinedViewModel model)
        {

            return RedirectToAction("Index", "Home", new { user = model.UserName });
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
            /*
			if (validarDatos(usuario.Nombre, usuario.Precio) == false)
			{
				TempData["Message"] = "Ingrese la informacion del articulo de forma correcta. Nombre: Solo puede contener letras, espacio y guiones. Precio: Solo puede contener numeros enteros o decimales";
				return RedirectToAction("Login", "Home");
			}
			*/
            //----------CORREGIR----------
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

                    command.ExecuteNonQuery();

                    int resultCode = Convert.ToInt32(command.Parameters["@outResultCode"].Value);

                    //int resultCode = 3335;
                    connection.Close();
                    if (resultCode != 50002)
                    {
                        TempData["Message"] = "Combinacion de usuario/password no existe en la BD";
                        return RedirectToAction("Login", "Home");
                    }
                    else
                    {
                        TempData["Message"] = "Login exitoso";


                        return RedirectToAction("Index", "Home", new { user = Username });
                    }
                }
            }
        }

        public IActionResult Insertar(CombinedViewModel model)
        {
            if (validarDatos(model.NewArticulo.Codigo, model.NewArticulo.Nombre, model.NewArticulo.Precio) == false)
            {
                TempData["Message"] = "Ingrese la informacion del articulo de forma correcta. Nombre: Solo puede contener letras, espacio y guiones. Precio: Solo puede contener numeros enteros o decimales";
                return RedirectToAction("Create", "Home", new { user = model.UserName });
            }
            string nombre = model.NewArticulo.Nombre;
            decimal precio = Convert.ToDecimal(model.NewArticulo.Precio);
            string codigo = model.NewArticulo.Codigo;
            string username = model.UserName;
            string claseArticulo = Request.Form["selectClase"];


            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("AddArticulo", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@inNombre", nombre);
                    command.Parameters.AddWithValue("@inPrecio", precio);
                    command.Parameters.AddWithValue("@inCodigo", codigo);
                    command.Parameters.AddWithValue("@inClaseArticulo", claseArticulo);
                    command.Parameters.AddWithValue("@inUserName", username);
                    command.Parameters.Add("@outResultCode", SqlDbType.Int).Direction = ParameterDirection.Output;

                    command.ExecuteNonQuery();

                    int resultCode = Convert.ToInt32(command.Parameters["@outResultCode"].Value);
                    connection.Close();
                    if (resultCode == 50002)
                    {
                        TempData["Message"] = "Articulo con nombre duplicado";
                        return RedirectToAction("Create", "Home", new { user = username });
                    }
                    else if (resultCode == 50003)
                    {
                        TempData["Message"] = "Articulo con codigo duplicado";
                        return RedirectToAction("Create", "Home", new { user = username });
                    }
                    else
                    {
                        TempData["Message"] = "Insercion exitosa";
                        return RedirectToAction("Index", "Home", new { user = username });
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

        public async Task<IActionResult> UploadFile(ArchivoViewModel model)
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
                        using (SqlCommand command = new SqlCommand("ProcesarXml", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            // Pasa el contenido XML como parámetro
                            command.Parameters.AddWithValue("@Datos", xmlContent);

                            // Configura el parámetro de salida para capturar la contraseña
                            command.Parameters.Add("@outResult", SqlDbType.VarChar, 128).Direction = ParameterDirection.Output;

                            command.ExecuteNonQuery();

                            // Obtener la contraseña del parámetro de salida
                            string password = Convert.ToString(command.Parameters["@outResult"].Value);

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

    }    
}