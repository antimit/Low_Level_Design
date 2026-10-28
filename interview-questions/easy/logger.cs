
using System;
using System.Collections.Generic;
using System.IO;

// 1. Define logging levels
public enum LogLevel
{
    Info,
    Warning,
    Error,
    Debug
}

// 2. Interface for all logging destinations
public interface ILogDestination
{
    void Write(string message, LogLevel level);
}

// 3. Console logging implementation
public class ConsoleLogger : ILogDestination
{
    public void Write(string message, LogLevel level)
    {
        Console.WriteLine($"[{level}] {message}");
    }
}

// 4. File logging implementation
public class FileLogger : ILogDestination
{
    private readonly string _filePath;

    public FileLogger(string filePath)
    {
        _filePath = filePath;
    }

    public void Write(string message, LogLevel level)
    {
        File.AppendAllText(
            _filePath,
            $"[{level}] {message}{Environment.NewLine}"
        );
    }
}

// 5. Singleton Logger
public sealed class Logger
{
    private static readonly Lazy<Logger> _instance =
        new(() => new Logger());

    public static Logger Instance => _instance.Value;

    private readonly List<ILogDestination> _destinations = new();
    private readonly object _lock = new();

    // Private constructor prevents external creation
    private Logger()
    {
    }

    // Register a destination
    public void AddDestination(ILogDestination destination)
    {
        ArgumentNullException.ThrowIfNull(destination);

        lock (_lock)
        {
            _destinations.Add(destination);
        }
    }

    // Send messages to all destinations
    public void Log(string message, LogLevel level)
    {
        lock (_lock)
        {
            foreach (ILogDestination destination in _destinations)
            {
                destination.Write(message, level);
            }
        }
    }
}

// 6. Application usage
public class Program
{
    public static void Main(string[] args)
    {
        // Get the Singleton instance
        Logger logger = Logger.Instance;

        // Register logging destinations
        logger.AddDestination(new ConsoleLogger());
        logger.AddDestination(new FileLogger("logs.txt"));

        // Write log messages
        logger.Log("Application started", LogLevel.Info);
        logger.Log("Memory usage is high", LogLevel.Warning);
        logger.Log("Database connection failed", LogLevel.Error);
        logger.Log("Debugging user authentication", LogLevel.Debug);

        // Verify Singleton behavior
        Logger anotherLogger = Logger.Instance;

        Console.WriteLine(
            "Same instance: " +
            ReferenceEquals(logger, anotherLogger)
        );

        // Both references use the same logger
        anotherLogger.Log("Application finished", LogLevel.Info);
    }
}
