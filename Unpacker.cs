using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

internal class Unpacker
{
    private ILogger<Unpacker> _logger;
    private IConfiguration _configuration;

    public Unpacker(ILogger<Unpacker> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    
}