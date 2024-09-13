using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TestInfileGAdLP.Models;
using TestInfileGAdLP;

namespace TestInfileGAdLP.Controllers
{
    public class AuthController : Controller
    {
        private readonly HttpClient _httpClient;

        public AuthController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // GET: /Auth/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Auth/Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var response = await _httpClient.PostAsJsonAsync("https://candidates-exam.herokuapp.com/api/v1/auth/login", model);
                var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();

                if (authResponse.Tipo)
                {
                    Config.Token = authResponse.Token;
                    return RedirectToAction("Perfil", "Usuario");
                }
                ModelState.AddModelError("", "Error de autenticación.");
            }
            return View(model);
        }
    }
}
