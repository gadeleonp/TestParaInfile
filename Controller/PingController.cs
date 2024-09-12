[Route("api/v1/[controller]")]
[ApiController]
public class PingController : ControllerBase
{
    [HttpGet("ping")]
    public async Task<IActionResult> Ping()
    {
        var client = new HttpClient();
        var response = await client.GetStringAsync("https://candidates-exam.herokuapp.com/api/v1/ping");
        return Ok(response);
    }
}
