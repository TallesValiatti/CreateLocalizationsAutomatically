using System.Xml.Linq;

namespace MySolution.LocalizationGenerator;

public class ResourceFileService
{
    public string ResourcePath
    {
        get
        {
            string currentDirectory = Environment.CurrentDirectory;
            
            DirectoryInfo parentDirectory = Directory.GetParent(currentDirectory)!;
            
            string newPath = Path.Combine(parentDirectory.FullName, "MySolution.SharedLocalizations","Resources");
            
            return newPath;
        }
    }

    public string?[] ListResourceFiles()
    {
        string[] files = Directory.GetFiles(ResourcePath);
        
        return files
            .Select(Path.GetFileName)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToArray();
    }
    
    public string?[] ListResourceFilesIsoLanguages()
    {
        return ListResourceFiles()
            .Select(ExtractLanguageCode)
            .ToArray();
    }
    
    public string ExtractLanguageCode(string? filename)
    {
        if (string.IsNullOrWhiteSpace(filename))
        {
            return string.Empty;
        }
        
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filename);
        
        string[] parts = fileNameWithoutExtension.Split('.');
        if (parts.Length == 2)
        {
            return parts[1];
        }

        return string.Empty;
    }
    
    public void AddDataLocalization(string filePath, string key, string value)
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
    
    public void AddDataLocalizations(string filePath, IList<(string Key, string Value)> localizations)
    {
        XDocument doc = XDocument.Load(filePath);
        
        var resx = doc.Element("root");

        foreach (var localization in localizations)
        {
            resx!.Add(new XElement("data",
                new XAttribute("name", localization.Key),
                new XAttribute(XNamespace.Xml + "space", "preserve"),
                new XElement("value", localization.Value)
            ));
        }
        
        resx!.Save(filePath);
    }
    
    public void CreateFile(string filePath)
    {
        XDocument doc = new XDocument(
            new XDeclaration("1.0", "utf-8", null),
            new XElement("root",
                new XElement(XNamespace.Get("http://www.w3.org/2001/XMLSchema") + "schema",
                    new XAttribute("id", "root"),
                    new XAttribute(XNamespace.Xmlns + "xsd", "http://www.w3.org/2001/XMLSchema"),
                    new XAttribute(XNamespace.Xmlns + "msdata", "urn:schemas-microsoft-com:xml-msdata"),
                    new XElement(XNamespace.Get("http://www.w3.org/2001/XMLSchema") + "element",
                        new XAttribute("name", "root"),
                        new XAttribute(XNamespace.Get("urn:schemas-microsoft-com:xml-msdata") + "IsDataSet", "true")
                    )
                ),
                new XElement("resheader",
                    new XAttribute("name", "resmimetype"),
                    new XElement("value", "text/microsoft-resx")
                ),
                new XElement("resheader",
                    new XAttribute("name", "version"),
                    new XElement("value", "1.3")
                ),
                new XElement("resheader",
                    new XAttribute("name", "reader"),
                    new XElement("value", "System.Resources.ResXResourceReader, System.Windows.Forms, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")
                ),
                new XElement("resheader",
                    new XAttribute("name", "writer"),
                    new XElement("value", "System.Resources.ResXResourceWriter, System.Windows.Forms, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")
                )
            )
        );
         
        doc.Save(filePath);
    }
    
    public IList<(string Key, string Value)> ListLocalization(string filePath)
    {
        XDocument doc = XDocument.Load(filePath);
        
        var dataElements = doc.Descendants("data");
        
        var dataKeys = dataElements
            .Select(data => (
                Key: data.Attribute("name")?.Value ?? string.Empty,
                Value: data.Element("value")?.Value ?? string.Empty
            ))
            .ToList();

        return dataKeys;
    }
}