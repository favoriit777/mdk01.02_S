using Xunit;

[CollectionDefinition("FileSystem collection")]
public class FileSystemCollection : ICollectionFixture<TempFileFixture>,
                                    ICollectionFixture<DirectoryFixture>,
                                    ICollectionFixture<FileStreamFixture>
{
    // Может оставаться пустым — только для регистрации фикстур
}

[Collection("FileSystem collection")]
public class FileSystemTests
{
    private readonly TempFileFixture _tempFile;
    private readonly DirectoryFixture _directory;
    private readonly FileStreamFixture _fileStream;

    public FileSystemTests(TempFileFixture tempFile, DirectoryFixture directory, FileStreamFixture fileStream)
    {
        _tempFile = tempFile;
        _directory = directory;
        _fileStream = fileStream;
    }

    [Fact]
    public void TempFile_ShouldExist()
    {
        Assert.True(File.Exists(_tempFile.FilePath));
    }

    [Fact]
    public void Directory_ShouldExist()
    {
        Assert.True(Directory.Exists(_directory.DirectoryPath));
    }

    [Fact]
    public void FileStream_ShouldWriteAndRead()
    {
        byte[] data = System.Text.Encoding.UTF8.GetBytes("Hello, FileStream!");
        _fileStream.Stream.Write(data, 0, data.Length);
        _fileStream.Stream.Position = 0;
        byte[] readData = new byte[data.Length];
        _fileStream.Stream.Read(readData, 0, readData.Length);
        string content = System.Text.Encoding.UTF8.GetString(readData);
        Assert.Equal("Hello, FileStream!", content);
    }
}