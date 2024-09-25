using Azure;
using Azure.AI.Translation.Text;

namespace MySolution.LocalizationGenerator;

public class TranslatorService
{
    private const string ApiKey = "<your-api-key>";
    private const string Endpoint = "<your-endpoint>";
    private const string Region = "<your-region>";
    
    
    public string Translate(string value, string sourceLanguage, string targetLanguage)
    {
        var client = new TextTranslationClient(new AzureKeyCredential(ApiKey), new Uri(Endpoint), Region);
        
        if (sourceLanguage == targetLanguage)
        {
            return value;
        }
        
        var result = client.Translate(targetLanguage, value, sourceLanguage);
        return result.Value.First().Translations.First(x => x.TargetLanguage == targetLanguage).Text;
    }
}