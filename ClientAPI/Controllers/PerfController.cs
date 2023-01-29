using Common;
using Microsoft.AspNetCore.Mvc;

namespace ServerAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class PerfController : ControllerBase
{
    private readonly Perf perf;
    public PerfController(Perf perf)
    {
        this.perf = perf;
    }
    
    [HttpGet]
    public string Get()
    {
        return perf.Info();
    }
}
