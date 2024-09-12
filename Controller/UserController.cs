[Route("api/v1/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    [HttpPost("registro")]
    public async Task<IActionResult> Register([FromBody] User user)
    {
        var client = new HttpClient();
        var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
        var response = await client.PostAsync("https://candidates-exam.herokuapp.com/api/v1/usuarios", content);
        return Ok(await response.Content.ReadAsStringAsync());
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
    {
        var client = new HttpClient();
        var content = new StringContent(JsonConvert.SerializeObject(loginModel), Encoding.UTF8, "application/json");
        var response = await client.PostAsync("https://candidates-exam.herokuapp.com/api/v1/auth/login", content);
        return Ok(await response.Content.ReadAsStringAsync());
    }

    [HttpPost("{url}/cargar_cv")]
    public async Task<IActionResult> UploadCV(string url, [FromBody] CV cv, [FromHeader(Name = "Authorization")] string token)
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", token);

        var content = new StringContent(JsonConvert.SerializeObject(cv), Encoding.UTF8, "application/json");
        var response = await client.PostAsync($"https://candidates-exam.herokuapp.com/api/v1/usuarios/{url}/cargar_cv", content);
        return Ok(await response.Content.ReadAsStringAsync());
    }

    [HttpGet("mostrar_cv")]
    public async Task<IActionResult> ShowCV([FromHeader(Name = "Authorization")] string token)
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", token);
        var response = await client.GetStringAsync("https://candidates-exam.herokuapp.com/api/v1/usuarios/mostrar_cv");
        return Ok(response);
    }
}
