using System.Globalization;
using System.Xml.Serialization;

namespace Currency.BackgroundWorker.Models;

[XmlRoot(ElementName="ValCurs")]
public sealed class ValCurs {

    [XmlElement(ElementName="Valute")]
    public List<Valute> Valute { get; set; } = [];

    [XmlAttribute(AttributeName="Date")]
    public required string DateString { get; set; }

    [XmlIgnore]
    public DateOnly Date => DateOnly.Parse(DateString, new CultureInfo("ru-Ru"));

    [XmlAttribute(AttributeName="name")]
    public required string Name { get; set; }

    [XmlText]
    public required string Text { get; set; }
}