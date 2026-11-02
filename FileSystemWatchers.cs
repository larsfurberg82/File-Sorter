
using System.Xml.Linq;

public class FileSystemWatchers
{
    public int LiveFileWatchers { get; private set; } = 0;
    private bool CloseAllFileWatchers = false;
    
    public void SetUpWatchers(List<PathsList> PathsList, string LogFilePath)
    {
        foreach (var pathlist in PathsList)
            _ = Add(pathlist.From, pathlist.To, pathlist.Filters, pathlist.Name, LogFilePath);
    }

    public async Task Add(string from, string to, List<string> filters, string name, string logfilepath)
    {
        using var watcher = new FileSystemWatcher(from);
        watcher.NotifyFilter = NotifyFilters.FileName;
        watcher.Created += (_, e) => _ = MoveFile(e.FullPath, Path.Combine(to, e.Name!), name, logfilepath);
        watcher.Renamed += (_, e) => _ = MoveFile(e.FullPath, Path.Combine(to, e.Name!), name, logfilepath);
        foreach (var filter in filters)
            watcher.Filters.Add(filter);
        watcher.EnableRaisingEvents = true;

        // Rename all existing files to trigger file watcher
        foreach (var filter in filters)
            foreach (string filename in Directory.GetFiles(from, filter))
                _ = RenameFile(filename);

        // Keep the file system watcher alive until CloseAllFileWatchers is True
        LiveFileWatchers += 1;
        while (CloseAllFileWatchers == false) await Task.Delay(100);
        LiveFileWatchers -= 1;
    }

    private async Task MoveFile(string fileToMove, string destination, string name, string logfilepath)
    {
        int attempt = 0;
        while (true)
        {
            try
            {
                if (attempt >= 50)
                    break;
                File.Move(fileToMove, destination, true);
                File.AppendAllText(logfilepath, $"{DateTime.Now} [{name}] File moved from \"{fileToMove}\" to \"{destination}\"\n");
                break;
            }
            catch
            {
                attempt += 1;
                await Task.Delay(100);
            }
        }
    }

    public async Task RenameFile(string fileName)
    {
        int attempt = 0;
        while (true)
        {
            try
            {
                if (attempt >= 50)
                    break;
                File.Move(fileName, fileName + ".triggerfilewatcher", true);
                break;
            }
            catch
            {
                attempt += 1;
                await Task.Delay(100);
            }
        }

        attempt = 0;
        while (true)
        {
            try
            {
                if (attempt >= 50)
                    break;
                File.Move(fileName + ".triggerfilewatcher", fileName, true);
                break;
            }
            catch
            {
                attempt += 1;
                await Task.Delay(100);
            }
        }
    }

    public bool Kill()
    {
        CloseAllFileWatchers = true;
        while (LiveFileWatchers > 0) Thread.Sleep(100);
        CloseAllFileWatchers = false;
        return true;
    }
}
