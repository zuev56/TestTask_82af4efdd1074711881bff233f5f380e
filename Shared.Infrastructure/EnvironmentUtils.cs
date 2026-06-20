namespace Shared.Infrastructure;

public static class EnvironmentUtils
{
    public static bool IsDevelopment() => Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
}