using System.Collections.Generic;

namespace Routes.Service.Configuration;

public class SecretManager
{
    public IpRateLimiting IpRateLimiting { get; set; }
    public AuthenticatedRateLimit AuthenticatedRateLimit { get; set; }
    public ConnectionStrings ConnectionStrings { get; set; }
    public Logging Logging { get; set; }
    public TokenConfigurations TokenConfigurations { get; set; }
    public Asaas Asaas { get; set; }
    public Google Google { get; set; }
    public URL URL { get; set; }
    public string AllowedHosts { get; set; }

}

public class IpRateLimiting
{
    public bool EnableEndpointRateLimiting { get; set; }
    public bool StackBlockedRequests { get; set; }
    public string RealIpHeader { get; set; }
    public string ClientIdHeader { get; set; }
    public List<GeneralRule> GeneralRules { get; set; }
}

public class GeneralRule
{
    public string Endpoint { get; set; }
    public string Period { get; set; }
    public int Limit { get; set; }
}

public class AuthenticatedRateLimit
{
    public string Period { get; set; }
    public int Limit { get; set; }
}

public class ConnectionStrings
{
    public string DefaultConnection { get; set; }
    public string RedisConnection { get; set; }
    public string RabbitConnection { get; set; }
}

public class Logging
{
    public LogLevel LogLevel { get; set; }
}

public class LogLevel
{
    public string Default { get; set; }
    public string Microsoft { get; set; }
    public string MicrosoftHostingLifetime { get; set; }
}

public class TokenConfigurations
{
    public string Audience { get; set; }
    public string Issuer { get; set; }
    public string Secret { get; set; }
    public int Minutes { get; set; }
    public int DaysToExpiry { get; set; }
}

public class Asaas
{
    public string Url { get; set; }
    public string AcessToken { get; set; }
    public string TokenWebHookAsaas { get; set; }
}

public class Google
{
    public string BaseUrl { get; set; }
    public string Key { get; set; }
}

public class URL
{
    public string AuthAPI { get; set; }
}