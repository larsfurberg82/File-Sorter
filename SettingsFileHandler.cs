using System.Xml.Linq;

public class SettingsFileHandler
{
    public List<PathsList> PathsList { get; private set; } = [];
    public string LogFilePath { get; private set; } = "";

    public bool Read(string settingsFile)
    {
        // Check if settings file exists
        if (!File.Exists(settingsFile))
        {
            Console.WriteLine($"{DateTime.Now} [ERROR] Could not find {settingsFile}.");
            return false;
        }

        // Load settings file and check Settings element exists
        var settingsElement = XDocument.Load(settingsFile).Element("Settings");
        if (settingsElement == null)
        {
            //Console.WriteLine($"{DateTime.Now} [ERROR] {settingsFile} does not contain <Settings> element.");
            return false;
        }

        // Check logFile element exists
        LogFilePath = settingsElement?.Element("Log")?.Value ?? "";
        if (string.IsNullOrWhiteSpace(LogFilePath))
        {
            Console.WriteLine($"{DateTime.Now} [ERROR] {settingsFile} does not contain <Log> element.");
            return false;
        }
        LogFilePath = LogFilePath.Replace('\\', Path.DirectorySeparatorChar);

        // Check logFile path exists
        if (Path.GetDirectoryName(LogFilePath) == "")
            LogFilePath = Path.Combine(Directory.GetCurrentDirectory(),LogFilePath);
        if (!Directory.Exists(Path.GetDirectoryName(LogFilePath)))
        {
            Console.WriteLine($"{DateTime.Now} [ERROR] {settingsFile} does not contain a valid log file path: {LogFilePath}");
            return false;
        }

        // Make a log file entry
        File.AppendAllText(LogFilePath, $"{DateTime.Now} [INFO] Log file found. Checking settings file...\n");

        // Check at least one Directory element exists (not empty)
        var directories = settingsElement!.Elements("Directory");
        if (!directories.Any())
        {
            File.AppendAllText(LogFilePath, $"{DateTime.Now} [ERROR] {settingsFile} does not contain at least one <Directory> element.\n");
            return false;
        }

        // Clear old paths list before adding any items
        PathsList.Clear();

        // Loop through all directory elements
        foreach (var directory in directories)
        {
            // Check Name element exists
            var name = directory.Element("Name")?.Value;
            if (name == null)
            {
                File.AppendAllText(LogFilePath, $"{DateTime.Now} [ERROR] {settingsFile} does not contain a <Name> element in each <Directory> element.\n");
                return false;
            }

            // Check Input element and path exists
            var input = directory.Element("Input")?.Value?.Replace('\\', Path.DirectorySeparatorChar);
            if (input == null)
            {
                File.AppendAllText(LogFilePath, $"{DateTime.Now} [ERROR] {settingsFile} does not contain an <Input> element in each <Directory> element.\n");
                return false;
            }
            if (!Directory.Exists(input))
            {
                File.AppendAllText(LogFilePath, $"{DateTime.Now} [ERROR] {settingsFile} contains an <Input> element with an invalid path: {input}\n");
                return false;
            }

            // Check Output element and path exists
            var output = directory.Element("Output")?.Value?.Replace('\\', Path.DirectorySeparatorChar);
            if (output == null)
            {
                File.AppendAllText(LogFilePath, $"{DateTime.Now} [ERROR] {settingsFile} does not contain an <Output> element in each <Directory> element.\n");
                return false;
            }
            if (!Directory.Exists(output))
            {
                File.AppendAllText(LogFilePath, $"{DateTime.Now} [ERROR] {settingsFile} contains an <Output> element with an invalid path: {output}\n");
                return false;
            }

            // Check at least one Type element exists
            var types = directory.Elements("Type");
            if (!types.Any())
            {
                File.AppendAllText(LogFilePath, $"{DateTime.Now} [ERROR] {settingsFile} does not contain at least one <Type> element in each <Directory> element.\n");
                return false;
            }

            // Create a new list item
            var newListItem = new PathsList
            {
                Name = name,
                From = input,
                To = output,
                Filters = [],
            };

            // Add all filters to the list item
            foreach (var type in types)
            {
                string filter = type.Value;
                if (filter.StartsWith("."))
                    filter = "*" + filter;
                newListItem.Filters.Add(filter);
            }

            // Add the created list item to the paths list
            PathsList.Add(newListItem);
        }
        File.AppendAllText(LogFilePath, $"{DateTime.Now} [INFO] Settings file read successfully.\n");
        return true;
    }
}

public class PathsList
{
    public required string Name { get; set; } = "";
    public required string From { get; set; } = "";
    public required string To { get; set; } = "";
    public required List<string> Filters { get; set; } = [];
}