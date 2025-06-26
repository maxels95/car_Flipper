using Microsoft.AspNetCore.Mvc;

// GET http://yourpi.local/api/token
[ApiController]
[Route("api/token")]
public class TokenController : ControllerBase
{
    private readonly BlocketAuthService _authService;

    public TokenController(BlocketAuthService authService)
    {
        _authService = authService;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] bool force = false)
    {
        var token = await _authService.GetTokenAsync(force);
        return Ok(new { access_token = token });
    }
}
