using MySolution.LocalizationGenerator;
using MySolution.SharedLocalizations;
using Spectre.Console;

const string availableLanguages = "\U0001F440 See available languages";
const string createdResourceFiles = "\U0001F440 See existings resource files";
const string addLocalization = "\U0001F449 Add localization";
const string addNewResourceFile = "\U0001F4D6 Add new resource file";

string[] options = [
    availableLanguages,
    createdResourceFiles,
    addLocalization,
    addNewResourceFile
];

while (true)
{
    AnsiConsole.Clear();

    var panel = new Panel("Localizations generator")
    {
        Header = new PanelHeader("My Solution"),
        Padding = new Padding(3, 0, 3, 0),
        Border = BoxBorder.Rounded
    };

    AnsiConsole.Write(panel);
    
    var option = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .PageSize(10)
            .Title("Select an action:")
            .AddChoices(options));

    switch (option)
    {
        case availableLanguages:
            HandleAvailableLanguages();
            break;
    
        case createdResourceFiles:
            HandleCreatedResourceFiles();
            break;
        
        case addLocalization:
            HandleAddLocalization();
            break;
    
        case addNewResourceFile:
            HandleAddNewResourceFile();
            break;
    }
}

void HandleAddLocalization()
{
    AnsiConsole.WriteLine($"Add localization:");

    var key = AnsiConsole.Prompt(
        new TextPrompt<string>("\U0001F449 Type the localization [bold yellow]KEY[/] ?"));
    
    var value = AnsiConsole.Prompt(
        new TextPrompt<string>("\U0001F449 Type the localization [bold green]DEFAULT VALUE[/] ?"));

    AnsiConsole.WriteLine($"{key}:{value}");

    var xmlService = new XmlService();
    var resourceFileService = new ResourceFileService();
    var translatorService = new TranslatorService();
    
    var files = resourceFileService.ListResourceFiles();

    foreach (var file in files)
    {
        var path = Path.Combine(resourceFileService.ResourcePath, file!);
        var languageCode = resourceFileService.ExtractLanguageCode(file);
        
        if (string.IsNullOrWhiteSpace(languageCode))
        {
            // Default resource
            xmlService.AddDataElement(path, key, value);
        }
        else
        {
            var targetLanguageValue = translatorService.Translate(value, "pt", languageCode);
            xmlService.AddDataElement(path, key, targetLanguageValue);
        }
    }
        
    Continue();
}

void HandleCreatedResourceFiles()
{
    var service = new ResourceFileService();
    var files = service.ListResourceFiles();
    
    AnsiConsole.WriteLine("Existing resources:");
    
    var table = new Table()
    {
        Border = TableBorder.Rounded,
        ShowRowSeparators = true
    };
    
    table.AddColumn(new TableColumn("File name").Centered());
    table.AddColumn(new TableColumn("ISO language name").Centered());

    foreach (var file in files)
    {
        var isoCode = service.ExtractLanguageCode(file);
        table.AddRow(file!, string.IsNullOrWhiteSpace(isoCode) ? "--//--" : isoCode);
    }
    
    AnsiConsole.Write(table);
    
    Continue();
}

void HandleAvailableLanguages()
{
    AnsiConsole.WriteLine("Available languages:");
    
    var table = new Table()
    {
        Border = TableBorder.Rounded,
        ShowRowSeparators = true
    };
    
    table.AddColumn(new TableColumn("Name").Centered());
    table.AddColumn(new TableColumn("ISO name").Centered());
    table.AddColumn(new TableColumn("Is default?").Centered());

    foreach (var culture in SupportedCultures.Cultures)
    {
        table.AddRow(
            culture.DisplayName, 
            culture.CultureInfo.Name, 
            culture.IsDefault.ToString());
        
    }
    
    AnsiConsole.Write(table);

    Continue();
}

void HandleAddNewResourceFile()
{
    AnsiConsole.WriteLine($"Add new resource file");
    
    Continue();
}

void Continue()
{
    while (true)
    {
        var confirmation = AnsiConsole.Prompt(
            new TextPrompt<bool>("[bold green]Continue[/] ?")
                .AddChoice(true)
                .AddChoice(false)
                .DefaultValue(true)
                .WithConverter(choice => choice ? "y" : "n"));
        
        if (confirmation)
        {
            break;
        }
    }
}