using Azure;
using Azure.AI.Translation.Text;

namespace MySolution.LocalizationGenerator;

public class TranslatorService
{
    private const string ApiKey = "3f8275bd13104b7eba40741eac340dcb";
    private const string Endpoint = "https://api.cognitive.microsofttranslator.com/";
    private const string Region = "eastus";
    
    
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