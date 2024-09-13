using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using TestInfileGAdLP;

namespace TestInfileGAdLP.Controllers
{
    public class CVController : Controller
    {
        private readonly HttpClient _httpClient;

        public CVController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // GET: /CV/Upload
        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        // POST: /CV/Upload
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile cv)
        {
            if (cv == null || !cv.FileName.EndsWith(".pdf") || cv.Length > 5 * 1024 * 1024)
            {
                ModelState.AddModelError("", "El archivo debe ser PDF y menor a 5MB.");
                return View();
            }

            var memoryStream = new MemoryStream();
            await cv.CopyToAsync(memoryStream);
            var content = new ByteArrayContent(memoryStream.ToArray());

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Config.Token);
            var response = await _httpClient.PostAsync("https://candidates-exam.herokuapp.com/api/v1/usuarios/{url}/cargar_cv", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Perfil", "Usuario");
            }

            ModelState.AddModelError("", "Error al subir el CV.");
            return View();
        }

        // GET: /CV/View
        [HttpGet]
        public async Task<IActionResult> ViewCV()
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Config.Token);
            var response = await _httpClient.GetAsync("https://candidates-exam.herokuapp.com/api/v1/usuarios/mostrar_cv");

            if (response.IsSuccessStatusCode)
            {
                var cvUrl = await response.Content.ReadAsStringAsync();
                return Redirect(cvUrl);
            }

            return View("Error");
        }
    }
}
