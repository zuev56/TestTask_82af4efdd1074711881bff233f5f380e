using System.Xml.Serialization;
using Currency.BackgroundWorker.Models;

namespace Currency.BackgroundWorker.Services;

public sealed class XmlDataService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<XmlDataService> _logger;

    public XmlDataService(IHttpClientFactory httpClientFactory, ILogger<XmlDataService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<ValCurs> GetValCursAsync(string url, CancellationToken cancellationToken = default)
    {
        var httpClient = _httpClientFactory.CreateClient("CurrencyUpdater");

        try
        {
            var xmlString = await httpClient.GetStringAsync(url, cancellationToken);

            var serializer = new XmlSerializer(typeof(ValCurs));
            using var reader = new StringReader(xmlString);
            var valCurs = serializer.Deserialize(reader) as ValCurs;

            if (valCurs == null || !valCurs.Valute.Any())
                throw new Exception("Deserialization failed.");

            return valCurs;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error when downloading XML from {Url}", url);
            throw;
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Error deserializing XML from {Url}", url);
            throw;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Operation cancelled when downloading XML from {Url}", url);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, message: null);
            throw;
        }
    }
}