using Xunit;
using TestOrganizationDemo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TestOrganizationDemo.Tests
{
    #region Unit тесты - парсинг логов

    [Trait(Traits.Category, Traits.Categories.Unit)]
    [Trait(Traits.Feature, Traits.Features.Parsing)]
    public class LogParsingTests
    {
        private readonly LogAnalyzer _analyzer = new LogAnalyzer();

        [Trait(Traits.Priority, Traits.Priorities.Critical)]
        [Trait(Traits.Type, Traits.Types.Positive)]
        [Trait(Traits.Stability, Traits.Stabilities.Stable)]
        [Fact]
        public void ParseLogLine_ValidInfoLog_ReturnsCorrectLogEntry()
        {
            // Arrange
            string logLine = "2024-01-15 10:30:45 INFO: User logged in successfully";

            // Act
            var entry = _analyzer.ParseLogLine(logLine);

            // Assert
            Assert.Equal("2024-01-15 10:30:45", entry.Timestamp);
            Assert.Equal("INFO", entry.Level);
            Assert.Equal("User logged in successfully", entry.Message);
        }

        [Trait(Traits.Priority, Traits.Priorities.High)]
        [Trait(Traits.Type, Traits.Types.Negative)]
        [Trait(Traits.Stability, Traits.Stabilities.Stable)]
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void ParseLogLine_EmptyOrNullLine_ThrowsArgumentException(string logLine)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => _analyzer.ParseLogLine(logLine));
        }

        [Trait(Traits.Priority, Traits.Priorities.Medium)]
        [Trait(Traits.Type, Traits.Types.Boundary)]
        [Trait(Traits.Stability, Traits.Stabilities.Stable)]
        [Fact]
        public void ParseLogLine_VeryLongLine_ThrowsArgumentException()
        {
            // Arrange
            string longLine = new string('A', 10001);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _analyzer.ParseLogLine(longLine));
        }

        [Trait(Traits.Priority, Traits.Priorities.Medium)]
        [Trait(Traits.Type, Traits.Types.Positive)]
        [Trait(Traits.Stability, Traits.Stabilities.Stable)]
        [Theory]
        [InlineData("2024-01-15 10:30:45 ERROR: Database connection failed")]
        [InlineData("2024-01-15 10:30:45 WARN: High memory usage detected")]
        [InlineData("2024-01-15 10:30:45 DEBUG: Processing request...")]
        public void ParseLogLine_DifferentLogLevels_ReturnsCorrectLevel(string logLine)
        {
            // Act
            var entry = _analyzer.ParseLogLine(logLine);

            // Assert
            Assert.NotNull(entry.Level);
            Assert.InRange(entry.Level.Length, 1, 10);
        }
    }

    #endregion

    #region Unit тесты - фильтрация логов

    [Trait(Traits.Category, Traits.Categories.Unit)]
    [Trait(Traits.Feature, Traits.Features.Filtering)]
    public class LogFilteringTests
    {
        private readonly LogAnalyzer _analyzer = new LogAnalyzer();
        private readonly List<LogEntry> _sampleLogs;

        public LogFilteringTests()
        {
            _sampleLogs = new List<LogEntry>
            {
                new LogEntry { Level = "INFO", Message = "System started" },
                new LogEntry { Level = "ERROR", Message = "Failed to connect" },
                new LogEntry { Level = "WARN", Message = "High CPU usage" },
                new LogEntry { Level = "INFO", Message = "User logged in" },
                new LogEntry { Level = "ERROR", Message = "Database timeout" }
            };
        }

        [Trait(Traits.Priority, Traits.Priorities.High)]
        [Trait(Traits.Type, Traits.Types.Positive)]
        [Trait(Traits.Stability, Traits.Stabilities.Stable)]
        [Fact]
        public void FilterLogsByLevel_ValidLevel_ReturnsFilteredLogs()
        {
            // Act
            var errorLogs = _analyzer.FilterLogsByLevel(_sampleLogs, "ERROR");

            // Assert
            Assert.Equal(2, errorLogs.Count);
            Assert.All(errorLogs, log => Assert.Equal("ERROR", log.Level));
        }

        [Trait(Traits.Priority, Traits.Priorities.Medium)]
        [Trait(Traits.Type, Traits.Types.Negative)]
        [Trait(Traits.Stability, Traits.Stabilities.Stable)]
        [Fact]
        public void FilterLogsByLevel_NullLogs_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _analyzer.FilterLogsByLevel(null, "INFO"));
        }

        [Trait(Traits.Priority, Traits.Priorities.Medium)]
        [Trait(Traits.Type, Traits.Types.Exception)]
        [Trait(Traits.Stability, Traits.Stabilities.Stable)]
        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("INVALID")]
        [InlineData("TRACE")]
        public void FilterLogsByLevel_InvalidLevel_ThrowsArgumentException(string level)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => _analyzer.FilterLogsByLevel(_sampleLogs, level));
        }

        [Trait(Traits.Priority, Traits.Priorities.Low)]
        [Trait(Traits.Type, Traits.Types.Boundary)]
        [Trait(Traits.Stability, Traits.Stabilities.Stable)]
        [Fact]
        public void FilterLogsByLevel_EmptyList_ReturnsEmptyList()
        {
            // Arrange
            var emptyLogs = new List<LogEntry>();

            // Act
            var result = _analyzer.FilterLogsByLevel(emptyLogs, "INFO");

            // Assert
            Assert.Empty(result);
        }
    }

    #endregion

    #region Unit тесты - поиск в логах

    [Trait(Traits.Category, Traits.Categories.Unit)]
    [Trait(Traits.Feature, Traits.Features.Search)]
    public class LogSearchTests
    {
        private readonly LogAnalyzer _analyzer = new LogAnalyzer();
        private readonly List<LogEntry> _sampleLogs;

        public LogSearchTests()
        {
            _sampleLogs = new List<LogEntry>
            {
                new LogEntry { Message = "Database connection established" },
                new LogEntry { Message = "Error connecting to database" },
                new LogEntry { Message = "User database updated" },
                new LogEntry { Message = "Cache cleared successfully" }
            };
        }

        [Trait(Traits.Priority, Traits.Priorities.High)]
        [Trait(Traits.Type, Traits.Types.Positive)]
        [Trait(Traits.Stability, Traits.Stabilities.Stable)]
        [Fact]
        public void SearchLogs_ExistingKeyword_ReturnsMatchingLogs()
        {
            // Act
            var results = _analyzer.SearchLogs(_sampleLogs, "database");

            // Assert
            Assert.Equal(3, results.Count);
            Assert.All(results, log => Assert.Contains("database", log.Message, StringComparison.OrdinalIgnoreCase));
        }

        [Trait(Traits.Priority, Traits.Priorities.Medium)]
        [Trait(Traits.Type, Traits.Types.Positive)]
        [Trait(Traits.Stability, Traits.Stabilities.Stable)]
        [Fact]
        public void SearchLogs_CaseSensitiveSearch_ReturnsCorrectResults()
        {
            // Act
            var results = _analyzer.SearchLogs(_sampleLogs, "Database", caseSensitive: true);

            // Assert
            Assert.Single(results);
            Assert.Contains("Database connection", results[0].Message);
        }

        [Trait(Traits.Priority, Traits.Priorities.Medium)]
        [Trait(Traits.Type, Traits.Types.Negative)]
        [Trait(Traits.Stability, Traits.Stabilities.Stable)]
        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void SearchLogs_EmptyKeyword_ThrowsArgumentException(string keyword)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => _analyzer.SearchLogs(_sampleLogs, keyword));
        }

        [Trait(Traits.Priority, Traits.Priorities.Low)]
        [Trait(Traits.Type, Traits.Types.Boundary)]
        [Trait(Traits.Stability, Traits.Stabilities.Stable)]
        [Fact]
        public void SearchLogs_TooLongKeyword_ThrowsArgumentException()
        {
            // Arrange
            string longKeyword = new string('A', 101);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _analyzer.SearchLogs(_sampleLogs, longKeyword));
        }
    }

    #endregion

    #region Unit тесты - анализ логов

    [Trait(Traits.Category, Traits.Categories.Unit)]
    [Trait(Traits.Feature, Traits.Features.Analysis)]
    public class LogAnalysisTests
    {
        private readonly LogAnalyzer _analyzer = new LogAnalyzer();

        [Trait(Traits.Priority, Traits.Priorities.Critical)]
        [Trait(Traits.Type, Traits.Types.Positive)]
        [Trait(Traits.Stability, Traits.Stabilities.Stable)]
        [Fact]
        public void AnalyzeLogs_ValidLogs_ReturnsCorrectStatistics()
        {
            // Arrange
            var logs = new List<LogEntry>
            {
                new LogEntry { Level = "INFO", Timestamp = "2024-01-15 10:00:00", Message = "Start" },
                new LogEntry { Level = "ERROR", Timestamp = "2024-01-15 10:01:00", Message = "Error1" },
                new LogEntry { Level = "INFO", Timestamp = "2024-01-15 10:02:00", Message = "Processing" },
                new LogEntry { Level = "WARN", Timestamp = "2024-01-15 10:03:00", Message = "Warning" },
                new LogEntry { Level = "ERROR", Timestamp = "2024-01-15 10:04:00", Message = "Error2" }
            };

            // Act
            var stats = _analyzer.AnalyzeLogs(logs);

            // Assert
            Assert.Equal(5, stats.TotalEntries);
            Assert.Equal(2, stats.InfoCount);
            Assert.Equal(1, stats.WarnCount);
            Assert.Equal(2, stats.ErrorCount);
            Assert.Equal(0, stats.DebugCount);
        }

        [Trait(Traits.Priority, Traits.Priorities.Medium)]
        [Trait(Traits.Type, Traits.Types.Boundary)]
        [Trait(Traits.Stability, Traits.Stabilities.Stable)]
        [Fact]
        public void AnalyzeLogs_EmptyLogList_ReturnsEmptyStatistics()
        {
            // Arrange
            var emptyLogs = new List<LogEntry>();

            // Act
            var stats = _analyzer.AnalyzeLogs(emptyLogs);

            // Assert
            Assert.Equal(0, stats.TotalEntries);
            Assert.Equal(0, stats.InfoCount);
            Assert.Equal(0, stats.WarnCount);
            Assert.Equal(0, stats.ErrorCount);
        }

        [Trait(Traits.Priority, Traits.Priorities.High)]
        [Trait(Traits.Type, Traits.Types.Negative)]
        [Trait(Traits.Stability, Traits.Stabilities.Stable)]
        [Fact]
        public void AnalyzeLogs_NullLogs_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _analyzer.AnalyzeLogs(null));
        }

        [Trait(Traits.Priority, Traits.Priorities.Low)]
        [Trait(Traits.Type, Traits.Types.Positive)]
        [Trait(Traits.Stability, Traits.Stabilities.Stable)]
        [Fact]
        public void AnalyzeLogs_CalculateAverageMessageLength_ReturnsCorrectValue()
        {
            // Arrange
            var logs = new List<LogEntry>
            {
                new LogEntry { Level = "INFO", Message = "Short" },
                new LogEntry { Level = "INFO", Message = "A bit longer message" },
                new LogEntry { Level = "INFO", Message = "The longest message in this collection" }
            };

            // Act
            var stats = _analyzer.AnalyzeLogs(logs);

            // Assert
            Assert.True(stats.AverageMessageLength > 0);
        }
    }

    #endregion

    #region FileIO тесты - работа с файлами

    [Trait(Traits.Category, Traits.Categories.FileIO)]
    [Trait(Traits.Feature, Traits.Features.FileOperations)]
    public class LogFileIOTests : IDisposable
    {
        private readonly LogAnalyzer _analyzer = new LogAnalyzer();
        private readonly string _testFilePath = "test_logs.txt";
        private readonly string _largeFilePath = "large_logs.txt";

        public LogFileIOTests()
        {
            // Создание тестового файла
            var testLogs = new[]
            {
                "2024-01-15 10:30:45 INFO: Application started",
                "2024-01-15 10:31:00 WARN: High memory usage",
                "2024-01-15 10:32:15 ERROR: Database connection failed",
                "2024-01-15 10:33:30 INFO: User logged in"
            };
            File.WriteAllLines(_testFilePath, testLogs);
        }

        [Trait(Traits.Priority, Traits.Priorities.Critical)]
        [Trait(Traits.Type, Traits.Types.Positive)]
        [Trait(Traits.Stability, Traits.Stabilities.Stable)]
        [Fact]
        public async Task ReadLogsFromFileAsync_ValidFile_ReturnsParsedLogs()
        {
            // Act
            var logs = await _analyzer.ReadLogsFromFileAsync(_testFilePath);

            // Assert
            Assert.Equal(4, logs.Count);
            Assert.All(logs, log => Assert.NotNull(log.Level));
        }

        [Trait(Traits.Priority, Traits.Priorities.High)]
        [Trait(Traits.Type, Traits.Types.Negative)]
        [Trait(Traits.Stability, Traits.Stabilities.Stable)]
        [Fact]
        public async Task ReadLogsFromFileAsync_NonExistentFile_ThrowsFileNotFoundException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<FileNotFoundException>(() =>
                _analyzer.ReadLogsFromFileAsync("nonexistent.txt"));
        }

        [Trait(Traits.Priority, Traits.Priorities.Medium)]
        [Trait(Traits.Type, Traits.Types.Exception)]
        [Trait(Traits.Stability, Traits.Stabilities.Stable)]
        [Fact]
        public async Task ReadLogsFromFileAsync_TooLargeFile_ThrowsIOException()
        {
            // Arrange - создание большого файла
            var largeContent = string.Join("\n", Enumerable.Repeat("2024-01-15 10:30:45 INFO: Test log", 1000000));
            await File.WriteAllTextAsync(_largeFilePath, largeContent);

            // Act & Assert
            await Assert.ThrowsAsync<IOException>(() =>
                _analyzer.ReadLogsFromFileAsync(_largeFilePath));
        }

        [Trait(Traits.Priority, Traits.Priorities.Medium)]
        [Trait(Traits.Type, Traits.Types.Positive)]
        [Trait(Traits.Stability, Traits.Stabilities.Slow)]
        [Fact]
        public async Task WriteLogsToFileAsync_ValidLogs_WritesToFile()
        {
            // Arrange
            var logs = new List<LogEntry>
            {
                new LogEntry { OriginalLine = "2024-01-15 10:30:45 INFO: Test 1" },
                new LogEntry { OriginalLine = "2024-01-15 10:31:00 WARN: Test 2" }
            };
            string outputFile = "output_logs.txt";

            // Act
            await _analyzer.WriteLogsToFileAsync(logs, outputFile);

            // Assert
            Assert.True(File.Exists(outputFile));
            var lines = await File.ReadAllLinesAsync(outputFile);
            Assert.Equal(2, lines.Length);

            // Cleanup
            File.Delete(outputFile);
        }

        [Trait(Traits.Priority, Traits.Priorities.Low)]
        [Trait(Traits.Type, Traits.Types.Negative)]
        [Trait(Traits.Stability, Traits.Stabilities.Stable)]
        [Fact]
        public async Task WriteLogsToFileAsync_NullLogs_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _analyzer.WriteLogsToFileAsync(null, "test.txt"));
        }

        public void Dispose()
        {
            // Очистка тестовых файлов
            if (File.Exists(_testFilePath))
                File.Delete(_testFilePath);

            if (File.Exists(_largeFilePath))
                File.Delete(_largeFilePath);
        }
    }

    #endregion

    #region Integration тесты - комплексные сценарии

    [Trait(Traits.Category, Traits.Categories.Integration)]
    [Trait(Traits.Feature, Traits.Features.Analysis)]
    public class IntegrationTests : IDisposable
    {
        private readonly LogAnalyzer _analyzer = new LogAnalyzer();
        private readonly string _integrationTestFile = "integration_logs.txt";

        public IntegrationTests()
        {
            // Создание тестовых данных
            var logs = new[]
            {
                "2024-01-15 09:00:00 INFO: System startup",
                "2024-01-15 09:05:00 ERROR: Database connection failed",
                "2024-01-15 09:10:00 WARN: High CPU usage",
                "2024-01-15 09:15:00 INFO: Backup completed",
                "2024-01-15 09:20:00 ERROR: File not found",
                "2024-01-15 09:25:00 INFO: User authentication",
                "2024-01-15 09:30:00 DEBUG: Cache cleared",
                "2024-01-15 09:35:00 INFO: System shutdown"
            };
            File.WriteAllLines(_integrationTestFile, logs);
        }

        [Trait(Traits.Priority, Traits.Priorities.High)]
        [Trait(Traits.Type, Traits.Types.Positive)]
        [Trait(Traits.Stability, Traits.Stabilities.Slow)]
        [Trait(Traits.Owner, Traits.Owners.DevOps)]
        [Fact]
        public async Task FullWorkflow_ReadFilterAnalyze_ReturnsCorrectResults()
        {
            // Act - чтение из файла
            var logs = await _analyzer.ReadLogsFromFileAsync(_integrationTestFile);

            // Act - фильтрация ошибок
            var errorLogs = _analyzer.FilterLogsByLevel(logs, "ERROR");

            // Act - анализ
            var stats = _analyzer.AnalyzeLogs(logs);

            // Assert
            Assert.Equal(8, logs.Count);
            Assert.Equal(2, errorLogs.Count);
            Assert.Equal(8, stats.TotalEntries);
            Assert.Equal(2, stats.ErrorCount);
            Assert.Equal(1, stats.WarnCount);
        }

        [Trait(Traits.Priority, Traits.Priorities.Medium)]
        [Trait(Traits.Type, Traits.Types.Positive)]
        [Trait(Traits.Stability, Traits.Stabilities.Slow)]
        [Fact]
        public async Task SearchInFile_KeywordExists_ReturnsMatchingLogs()
        {
            // Arrange
            var logs = await _analyzer.ReadLogsFromFileAsync(_integrationTestFile);

            // Act
            var databaseLogs = _analyzer.SearchLogs(logs, "database");
            var userLogs = _analyzer.SearchLogs(logs, "user", caseSensitive: false);

            // Assert
            Assert.Single(databaseLogs);
            Assert.Single(userLogs);
        }

        public void Dispose()
        {
            if (File.Exists(_integrationTestFile))
                File.Delete(_integrationTestFile);
        }
    }

    #endregion

    #region Smoke тесты - основные функции

    [Trait(Traits.Category, Traits.Categories.Smoke)]
    public class SmokeTests
    {
        private readonly LogAnalyzer _analyzer = new LogAnalyzer();

        [Trait(Traits.Priority, Traits.Priorities.Critical)]
        [Trait(Traits.Stability, Traits.Stabilities.Fast)]
        [Trait(Traits.Feature, Traits.Features.Parsing)]
        [Fact]
        public void Smoke_ParseLogLine_ShouldWork()
        {
            // Act & Assert
            var exception = Record.Exception(() =>
                _analyzer.ParseLogLine("2024-01-15 10:30:45 INFO: Test"));

            Assert.Null(exception);
        }

        [Trait(Traits.Priority, Traits.Priorities.Critical)]
        [Trait(Traits.Stability, Traits.Stabilities.Fast)]
        [Trait(Traits.Feature, Traits.Features.Filtering)]
        [Fact]
        public void Smoke_FilterLogs_ShouldWork()
        {
            // Arrange
            var logs = new List<LogEntry>
            {
                new LogEntry { Level = "INFO", Message = "Test" },
                new LogEntry { Level = "ERROR", Message = "Error" }
            };

            // Act & Assert
            var exception = Record.Exception(() =>
                _analyzer.FilterLogsByLevel(logs, "INFO"));

            Assert.Null(exception);
        }

        [Trait(Traits.Priority, Traits.Priorities.Critical)]
        [Trait(Traits.Stability, Traits.Stabilities.Fast)]
        [Trait(Traits.Feature, Traits.Features.Analysis)]
        [Fact]
        public void Smoke_AnalyzeLogs_ShouldWork()
        {
            // Arrange
            var logs = new List<LogEntry>
            {
                new LogEntry { Level = "INFO", Message = "Test" }
            };

            // Act & Assert
            var exception = Record.Exception(() =>
                _analyzer.AnalyzeLogs(logs));

            Assert.Null(exception);
        }
    }

    #endregion

    #region Performance тесты

    [Trait(Traits.Category, Traits.Categories.Performance)]
    public class PerformanceTests
    {
        private readonly LogAnalyzer _analyzer = new LogAnalyzer();

        [Trait(Traits.Priority, Traits.Priorities.Medium)]
        [Trait(Traits.Type, Traits.Types.Performance)]
        [Trait(Traits.Stability, Traits.Stabilities.Slow)]
        [Fact]
        public void ParseLogLine_Performance_LargeNumberOfLines()
        {
            // Arrange
            var logLines = Enumerable.Range(1, 10000)
                .Select(i => $"2024-01-15 10:30:{i:D2} INFO: Log entry {i}")
                .ToList();

            // Act & Assert - проверяем что не выбрасывается исключение
            var exception = Record.Exception(() =>
            {
                foreach (var line in logLines)
                {
                    _analyzer.ParseLogLine(line);
                }
            });

            Assert.Null(exception);
        }

        [Trait(Traits.Priority, Traits.Priorities.Low)]
        [Trait(Traits.Type, Traits.Types.Performance)]
        [Trait(Traits.Stability, Traits.Stabilities.Slow)]
        [Fact]
        public void FilterLogs_Performance_LargeDataSet()
        {
            // Arrange
            var logs = Enumerable.Range(1, 50000)
                .Select(i => new LogEntry
                {
                    Level = i % 4 == 0 ? "ERROR" : "INFO",
                    Message = $"Message {i}"
                })
                .ToList();

            // Act
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var errorLogs = _analyzer.FilterLogsByLevel(logs, "ERROR");
            stopwatch.Stop();

            // Assert
            Assert.InRange(errorLogs.Count, 10000, 15000); // Примерно 25% от 50000
            Assert.True(stopwatch.ElapsedMilliseconds < 1000, $"Filtering took {stopwatch.ElapsedMilliseconds}ms");
        }
    }

    #endregion
}