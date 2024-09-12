using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using YourNamespace.Models;
using System.ComponentModel.DataAnnotations;

namespace com.infilewebapp.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public UserController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Registro de usuario
        [HttpPost("registro")]
        [ValidateAntiForgeryToken] // Protección contra CSRF
        public async Task<IActionResult> Register(User user)
        {
            // Programación Defensiva: Validar campos
            if (!ModelState.IsValid)
            {
                return BadRequest("Datos inválidos.");
            }

            // Sanitize inputs to prevent XSS (ejemplo básico)
            user.Nombre = System.Net.WebUtility.HtmlEncode(user.Nombre);
            user.Email = System.Net.WebUtility.HtmlEncode(user.Email);

            var client = _httpClientFactory.CreateClient();
            var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://candidates-exam.herokuapp.com/api/v1/usuarios", content);

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest("Error en el registro de usuario.");
            }

            return RedirectToAction("Login");
        }

        // Login de usuario
        [HttpPost("login")]
        [ValidateAntiForgeryToken] // Protección contra CSRF
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            // Validación Defensiva: Validar Email y Password
            if (!ModelState.IsValid)
            {
                return BadRequest("Datos de login inválidos.");
            }

            var client = _httpClientFactory.CreateClient();
            var content = new StringContent(JsonConvert.SerializeObject(loginModel), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://candidates-exam.herokuapp.com/api/v1/auth/login", content);

            if (!response.IsSuccessStatusCode)
            {
                return Unauthorized("Login fallido.");
            }

            var responseData = await response.Content.ReadAsStringAsync();
            var loginResponse = JsonConvert.DeserializeObject<LoginResponse>(responseData);

            // Almacenar token de forma segura
            HttpContext.Session.SetString("AuthToken", loginResponse.Token);

            return RedirectToAction("Profile");
        }

        // Subir CV
        [HttpPost("cargar_cv")]
        [ValidateAntiForgeryToken] // Protección contra CSRF
        public async Task<IActionResult> UploadCV(IFormFile cv)
        {
            if (cv == null || cv.Length == 0 || cv.Length > 5242880 || !cv.FileName.EndsWith(".pdf"))
            {
                return BadRequest("El archivo debe ser un PDF de menos de 5MB.");
            }

            var client = _httpClientFactory.CreateClient();

            // Obtener token de sesión
            var token = HttpContext.Session.GetString("AuthToken");
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized("No autorizado.");
            }

            var url = "https://candidates-exam.herokuapp.com/api/v1/usuarios/{url}/cargar_cv";

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            using (var memoryStream = new MemoryStream())
            {
                await cv.CopyToAsync(memoryStream);
                var cvData = Convert.ToBase64String(memoryStream.ToArray());

                var cvModel = new { curriculum = cvData };
                var content = new StringContent(JsonConvert.SerializeObject(cvModel), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    return BadRequest("Error al subir el CV.");
                }

                return RedirectToAction("Profile");
            }
        }

        // Mostrar CV
        [HttpGet("mostrar_cv")]
        public async Task<IActionResult> ShowCV()
        {
            var token = HttpContext.Session.GetString("AuthToken");
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized("No autorizado.");
            }

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync("https://candidates-exam.herokuapp.com/api/v1/usuarios/mostrar_cv");

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest("Error al obtener el CV.");
            }

            var cvUrl = await response.Content.ReadAsStringAsync();
            return Redirect(cvUrl); // Redirige para ver el CV en el navegador
        }
    }
}
