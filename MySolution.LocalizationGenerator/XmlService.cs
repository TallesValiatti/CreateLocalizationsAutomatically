using System.Xml.Linq;

namespace MySolution.LocalizationGenerator;

public class XmlService
{
    public void AddDataElement(string filePath, string key, string value)
    {
        XDocument doc = XDocument.Load(filePath);
        
        var resx = doc.Element("root");
        
        resx!.Add(new XElement("data",
            new XAttribute("name", key),
            new XAttribute(XNamespace.Xml + "space", "preserve"),
            new XElement("value", value)
        ));
        
        resx.Save(filePath);
    }
}