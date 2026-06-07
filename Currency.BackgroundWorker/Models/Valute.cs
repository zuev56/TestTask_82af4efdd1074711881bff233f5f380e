using System.Globalization;
using System.Xml.Serialization;

namespace Currency.BackgroundWorker.Models;

[XmlRoot(ElementName="Valute")]
public sealed class Valute
{
    [XmlElement(ElementName="NumCode")]
    public required int NumCode { get; set; }

    [XmlElement(ElementName="Name")]
    public required string Name { get; set; }

    [XmlElement(ElementName="VunitRate")]
    public required string VunitRateString { get; set; }

    [XmlIgnore]
    public decimal VunitRate => decimal.Parse(VunitRateString, NumberStyles.Any, new CultureInfo("ru-RU"));
}