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
    
    var resourceFileService = new ResourceFileService();
    var translatorService = new TranslatorService();
    
    var files = resourceFileService.ListResourceFiles();

    AnsiConsole.WriteLine();
    
    foreach (var file in files)
    {
        var path = Path.Combine(resourceFileService.ResourcePath, file!);
        var languageCode = resourceFileService.ExtractLanguageCode(file);

        var targetLanguageValue = string.IsNullOrWhiteSpace(languageCode) ? 
            value : 
            translatorService.Translate(value, "pt", languageCode);
        
        resourceFileService.AddDataLocalization(path, key, targetLanguageValue);
        
        AnsiConsole.MarkupLine(string.Format(
            "\U0001F4AC The localization with key [bold yellow]{0}[/] and value [bold green]{1}[/] was added to [bold white]{2}[/]", 
            key, 
            targetLanguageValue, 
            file));
    }
    
    AnsiConsole.WriteLine();
        
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
    var isoName = AnsiConsole.Prompt(
        new TextPrompt<string>("\U0001F449 Type the language [bold white]ISO name[/] ?"));
    
    var resourceFileService = new ResourceFileService();
    var translatorService = new TranslatorService();
    
    var file = $"SharedResources.{isoName}.resx";
    var defaultFile = $"SharedResources.resx";
    var path = Path.Combine(resourceFileService.ResourcePath, file);
    var defaultPath = Path.Combine(resourceFileService.ResourcePath, defaultFile);
    
    resourceFileService.CreateFile(path);

    var localizations = resourceFileService.ListLocalization(defaultPath);

    var translatedLocalizations = localizations
        .Select(x => (x.Key, translatorService.Translate(x.Value, "pt", isoName)));
    
    resourceFileService.AddDataLocalizations(path, localizations);
    
    AnsiConsole.WriteLine();
    
    AnsiConsole.MarkupLine($"\U0001F4C4 The localization file [bold white]{file}[/] was created");
    
    Continue();
}

void Continue()
{
    while (true)
    {
        AnsiConsole.WriteLine();
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