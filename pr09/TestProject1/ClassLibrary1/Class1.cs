using System;
using System.IO;
using System.Threading.Tasks;

public class TempFileFixture : IAsyncDisposable
{
    public string FilePath { get; private set; }

    public TempFileFixture()
    {
        FilePath = Path.GetTempFileName();
        File.WriteAllText(FilePath, "Initial content");
        Console.WriteLine($"TempFile создан: {FilePath}");
    }

    public async ValueTask DisposeAsync()
    {
        if (File.Exists(FilePath))
        {
            try
            {
                File.Delete(FilePath);
                Console.WriteLine($"TempFile удален: {FilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при удалении TempFile: {ex.Message}");
            }
        }
        await Task.CompletedTask;
    }
}
public class DirectoryFixture : IDisposable
{
    public string DirectoryPath { get; private set; }

    public DirectoryFixture()
    {
        DirectoryPath = Path.Combine(Path.GetTempPath(), $"TestDir_{Guid.NewGuid()}");
        Directory.CreateDirectory(DirectoryPath);
        Console.WriteLine($"Директория создана: {DirectoryPath}");
    }

    public void Dispose()
    {
        if (Directory.Exists(DirectoryPath))
        {
            try
            {
                Directory.Delete(DirectoryPath, true);
                Console.WriteLine($"Директория удалена: {DirectoryPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при удалении директории: {ex.Message}");
            }
        }
    }
}

public class FileStreamFixture : IDisposable
{
    public FileStream Stream { get; private set; }
    public string FilePath { get; }

    public FileStreamFixture()
    {
        FilePath = Path.GetTempFileName();
        Stream = new FileStream(FilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        Console.WriteLine($"Файл открыт: {FilePath}");
    }

    public void Dispose()
    {
        Stream?.Dispose();
        if (File.Exists(FilePath))
        {
            try
            {
                File.Delete(FilePath);
                Console.WriteLine($"Файл удален: {FilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при удалении файла: {ex.Message}");
            }
        }
    }
}