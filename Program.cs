
// Try to read the settings file
var handleSettingsFile = new SettingsFileHandler();
if (!handleSettingsFile.Read("Inställningar.xml"))
    return 1;

// Set up file watchers for all directories
var watcher = new FileSystemWatchers();
watcher.SetUpWatchers(handleSettingsFile.PathsList, handleSettingsFile.LogFilePath);
File.AppendAllText(handleSettingsFile.LogFilePath, $"{DateTime.Now} [INFO] Active file system watchers: {watcher.LiveFileWatchers}\n");

// Set up a file watcher for the settings file
using var settingsfilewatcher = new FileSystemWatcher(Directory.GetCurrentDirectory());
settingsfilewatcher.NotifyFilter = NotifyFilters.LastWrite;
settingsfilewatcher.Changed += SettingsFileChanged;
settingsfilewatcher.Filter = "Inställningar.xml";
settingsfilewatcher.EnableRaisingEvents = true;

// Wait infinite time
Task.Delay(Timeout.Infinite).Wait();

// End of main program
return 0;

void SettingsFileChanged(object sender, FileSystemEventArgs e) 
{
    if (e.ChangeType != WatcherChangeTypes.Changed)
    {
        return;
    }

    // Disable the watcher to prevent multiple entries
    settingsfilewatcher.EnableRaisingEvents = false;

    // Write a log in log file
    File.AppendAllText(handleSettingsFile.LogFilePath, $"{DateTime.Now} [INFO] Settings file change detected. Closing file watchers...\n");

    // Kill all file watchers
    watcher.Kill();
    while (watcher.LiveFileWatchers > 0) Thread.Sleep(100);

    // Read the new settings file
    File.AppendAllText(handleSettingsFile.LogFilePath, $"{DateTime.Now} [INFO] File watchers closed. Reloading settings...\n");
    int attempt = 0;
    while (true)
    {
        try
        {
            if (attempt >= 10)
            {
                File.AppendAllText(handleSettingsFile.LogFilePath, $"{DateTime.Now} [ERROR] Failed to reload settings file after 10 attempts.\n");
                Environment.Exit(1);
            }
            if (handleSettingsFile.Read("Inställningar.xml"))
            {
                File.AppendAllText(handleSettingsFile.LogFilePath, $"{DateTime.Now} [INFO] Settings file reloaded successfully. Starting file watchers...\n");
                break;
            }
        }
        catch
        {
            attempt += 1;
            Thread.Sleep(100);
        }
    }

    // Set up new file watchers for all directories
    watcher.SetUpWatchers(handleSettingsFile.PathsList, handleSettingsFile.LogFilePath);
    File.AppendAllText(handleSettingsFile.LogFilePath, $"{DateTime.Now} [INFO] Active file system watchers: {watcher.LiveFileWatchers}\n");

    // Enable the watcher
    settingsfilewatcher.EnableRaisingEvents = true;
}