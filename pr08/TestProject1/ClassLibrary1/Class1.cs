using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TestOrganizationDemo
{
    /// <summary>
    /// Класс для анализа логов
    /// </summary>
    public class LogAnalyzer
    {
        /// <summary>
        /// Анализирует строку лога и извлекает информацию
        /// </summary>
        public LogEntry ParseLogLine(string logLine)
        {
            if (string.IsNullOrWhiteSpace(logLine))
                throw new ArgumentException("Log line cannot be null or empty", nameof(logLine));

            if (logLine.Length > 10000)
                throw new ArgumentException("Log line is too long", nameof(logLine));

            var patterns = new Dictionary<string, string>
            {
                { "timestamp", @"\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}" },
                { "level", @"(INFO|WARN|ERROR|DEBUG)" },
                { "message", @":\s*(.+)$" }
            };

            string timestamp = ExtractPattern(logLine, patterns["timestamp"], "DateTime.Now");
            string level = ExtractPattern(logLine, patterns["level"], "INFO");
            string message = ExtractPattern(logLine, patterns["message"], logLine);

            return new LogEntry
            {
                Timestamp = timestamp,
                Level = level,
                Message = message,
                OriginalLine = logLine
            };
        }

        /// <summary>
        /// Фильтрует логи по уровню
        /// </summary>
        public List<LogEntry> FilterLogsByLevel(List<LogEntry> logs, string level)
        {
            if (logs == null)
                throw new ArgumentNullException(nameof(logs));

            if (string.IsNullOrWhiteSpace(level))
                throw new ArgumentException("Level cannot be empty", nameof(level));

            var validLevels = new[] { "INFO", "WARN", "ERROR", "DEBUG" };
            if (!validLevels.Contains(level.ToUpper()))
                throw new ArgumentException($"Invalid log level. Valid levels: {string.Join(", ", validLevels)}", nameof(level));

            return logs.Where(log => log.Level.Equals(level, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        /// <summary>
        /// Ищет логи по ключевому слову
        /// </summary>
        public List<LogEntry> SearchLogs(List<LogEntry> logs, string keyword, bool caseSensitive = false)
        {
            if (logs == null)
                throw new ArgumentNullException(nameof(logs));

            if (string.IsNullOrWhiteSpace(keyword))
                throw new ArgumentException("Keyword cannot be empty", nameof(keyword));

            if (keyword.Length > 100)
                throw new ArgumentException("Keyword is too long", nameof(keyword));

            var comparison = caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
            return logs.Where(log => log.Message.IndexOf(keyword, comparison) >= 0).ToList();
        }

        /// <summary>
        /// Анализирует статистику логов
        /// </summary>
        public LogStatistics AnalyzeLogs(List<LogEntry> logs)
        {
            if (logs == null)
                throw new ArgumentNullException(nameof(logs));

            if (logs.Count == 0)
                return new LogStatistics();

            var statistics = new LogStatistics
            {
                TotalEntries = logs.Count,
                InfoCount = logs.Count(l => l.Level.Equals("INFO", StringComparison.OrdinalIgnoreCase)),
                WarnCount = logs.Count(l => l.Level.Equals("WARN", StringComparison.OrdinalIgnoreCase)),
                ErrorCount = logs.Count(l => l.Level.Equals("ERROR", StringComparison.OrdinalIgnoreCase)),
                DebugCount = logs.Count(l => l.Level.Equals("DEBUG", StringComparison.OrdinalIgnoreCase)),
                FirstEntry = logs.Min(l => l.Timestamp),
                LastEntry = logs.Max(l => l.Timestamp)
            };

            if (logs.Any(l => !string.IsNullOrEmpty(l.Message)))
            {
                statistics.AverageMessageLength = (int)logs.Where(l => !string.IsNullOrEmpty(l.Message))
                    .Average(l => l.Message.Length);
            }

            return statistics;
        }

        /// <summary>
        /// Читает логи из файла
        /// </summary>
        public async Task<List<LogEntry>> ReadLogsFromFileAsync(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path cannot be empty", nameof(filePath));

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Log file not found: {filePath}");

            if (new FileInfo(filePath).Length > 10 * 1024 * 1024) // 10 MB
                throw new IOException("Log file is too large");

            var logs = new List<LogEntry>();
            var lines = await File.ReadAllLinesAsync(filePath, Encoding.UTF8);

            foreach (var line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    logs.Add(ParseLogLine(line));
                }
            }

            return logs;
        }

        /// <summary>
        /// Записывает логи в файл
        /// </summary>
        public async Task WriteLogsToFileAsync(List<LogEntry> logs, string filePath)
        {
            if (logs == null)
                throw new ArgumentNullException(nameof(logs));

            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path cannot be empty", nameof(filePath));

            var lines = logs.Select(log => log.OriginalLine);
            await File.WriteAllLinesAsync(filePath, lines, Encoding.UTF8);
        }

        /// <summary>
        /// Извлекает паттерн из строки
        /// </summary>
        private string ExtractPattern(string input, string pattern, string defaultValue)
        {
            var match = Regex.Match(input, pattern);
            return match.Success ? match.Value : defaultValue;
        }
    }

    /// <summary>
    /// Запись лога
    /// </summary>
    public class LogEntry
    {
        public string Timestamp { get; set; }
        public string Level { get; set; }
        public string Message { get; set; }
        public string OriginalLine { get; set; }
    }

    /// <summary>
    /// Статистика логов
    /// </summary>
    public class LogStatistics
    {
        public int TotalEntries { get; set; }
        public int InfoCount { get; set; }
        public int WarnCount { get; set; }
        public int ErrorCount { get; set; }
        public int DebugCount { get; set; }
        public string FirstEntry { get; set; }
        public string LastEntry { get; set; }
        public int AverageMessageLength { get; set; }
    }
}
namespace TestOrganizationDemo.Tests
{
    /// <summary>
    /// Класс для организации констант Trait
    /// </summary>
    public static class Traits
    {
        public const string Category = "Category";
        public const string Priority = "Priority";
        public const string Feature = "Feature";
        public const string Type = "Type";
        public const string Stability = "Stability";
        public const string Owner = "Owner";

        public static class Categories
        {
            public const string Unit = "Unit";
            public const string Integration = "Integration";
            public const string FileIO = "FileIO";
            public const string Performance = "Performance";
            public const string Smoke = "Smoke";
        }

        public static class Priorities
        {
            public const string Critical = "Critical";
            public const string High = "High";
            public const string Medium = "Medium";
            public const string Low = "Low";
        }

        public static class Features
        {
            public const string Parsing = "Parsing";
            public const string Filtering = "Filtering";
            public const string Analysis = "Analysis";
            public const string FileOperations = "FileOperations";
            public const string Search = "Search";
        }

        public static class Types
        {
            public const string Positive = "Positive";
            public const string Negative = "Negative";
            public const string Boundary = "Boundary";
            public const string Exception = "Exception";
            public const string Performance = "Performance";
        }

        public static class Stabilities
        {
            public const string Stable = "Stable";
            public const string Slow = "Slow";
            public const string Flaky = "Flaky";
            public const string Fast = "Fast";
        }

        public static class Owners
        {
            public const string TeamA = "TeamA";
            public const string TeamB = "TeamB";
            public const string DevOps = "DevOps";
        }
    }
}