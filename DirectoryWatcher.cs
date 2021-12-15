using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

internal class DirectoryWatcher : IHostedService
{
    private IConfiguration _configuration;
    private Unpacker _unpacker;
    private ILogger _logger;
    private FileSystemWatcher watcher;

    public DirectoryWatcher(IConfiguration configuration, Unpacker unpacker, ILogger<DirectoryWatcher> logger)
    {
        _configuration = configuration;
        _unpacker = unpacker;
        _logger = logger;
    }

    private void OnChanged(object sender, FileSystemEventArgs e)
    {
        if (e.ChangeType != WatcherChangeTypes.Changed)
        {
            return;
        }
        _logger.LogInformation("Changed: {filePath}", e.FullPath);
    }

    private void OnCreated(object sender, FileSystemEventArgs e)
    {
        _logger.LogInformation("Created: {filePath}", e.FullPath);
    }

    private void OnDeleted(object sender, FileSystemEventArgs e) =>
        _logger.LogInformation("Deleted: {filePath}", e.FullPath);

    private void OnRenamed(object sender, RenamedEventArgs e)
    {
        _logger.LogInformation("Renamed: {oldFilePath} to {filePath}", e.OldFullPath,e.FullPath);
    }

    private void OnError(object sender, ErrorEventArgs e) =>
        PrintException(e.GetException());

    private void PrintException(Exception? ex)
    {
        if (ex != null) _logger.LogError(ex, "Message: {errorMessage}", ex.Message);   
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        
        var watchPath = _configuration["WatchDirectory"];

        _logger.LogInformation("Setting up directory watch on {WatchPath}",watchPath);
        watcher = new FileSystemWatcher(watchPath);
        watcher.NotifyFilter = NotifyFilters.Attributes
                                 | NotifyFilters.CreationTime
                                 | NotifyFilters.DirectoryName
                                 | NotifyFilters.FileName
                                 | NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.Security
                                 | NotifyFilters.Size;

        _logger.LogInformation("Setting up change listeners");
        watcher.Changed += OnChanged;
        watcher.Created += OnCreated;
        watcher.Deleted += OnDeleted;
        watcher.Renamed += OnRenamed;
        watcher.Error += OnError;
        watcher.Filter = "*.txt";
        watcher.IncludeSubdirectories = true;
        watcher.EnableRaisingEvents = true;

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Shutting down...");
        watcher.Dispose();
        return Task.CompletedTask;
    }
}