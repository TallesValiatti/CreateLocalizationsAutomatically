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
}