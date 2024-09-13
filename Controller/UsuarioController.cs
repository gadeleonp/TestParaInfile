using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TestInfileGAdLP.Models;
using TestInfileGAdLP;

namespace TestInfileGAdLP.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly HttpClient _httpClient;

        public UsuarioController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // GET: /Usuario/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Usuario/Register
        [HttpPost]
        public async Task<IActionResult> Register(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                var response = await _httpClient.PostAsJsonAsync("https://candidates-exam.herokuapp.com/api/v1/usuarios", usuario);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Login", "Auth");
                }
                ModelState.AddModelError("", "Error en el registro.");
            }
            return View(usuario);
        }

        // GET: /Usuario/Perfil
        [HttpGet]
        public async Task<IActionResult> Perfil()
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Config.Token);
            var response = await _httpClient.GetAsync("https://candidates-exam.herokuapp.com/api/v1/usuarios/");
            if (response.IsSuccessStatusCode)
            {
                var usuario = await response.Content.ReadFromJsonAsync<Usuario>();
                return View(usuario);
            }
            return Unauthorized();
        }
    }
}
