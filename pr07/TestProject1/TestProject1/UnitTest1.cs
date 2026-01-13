using Xunit;
using ExceptionTestingDemo;
using System;
using System.Threading.Tasks;

namespace ExceptionTestingDemo.Tests
{
    public class DatabaseAccessorTests
    {
        private const string ValidConnectionString =
            "Server=localhost;Database=TestDB;User Id=user;Password=securePass123;";

        // Тест 1: Конструктор с пустой строкой подключения
        [Fact]
        public void Constructor_EmptyConnectionString_ThrowsArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new DatabaseAccessor(""));

            Assert.Equal("connectionString", exception.ParamName);
            Assert.Contains("cannot be null or empty", exception.Message);
        }

        // Тест 2: Конструктор с null строкой подключения
        [Fact]
        public void Constructor_NullConnectionString_ThrowsArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new DatabaseAccessor(null));

            Assert.Equal("connectionString", exception.ParamName);
        }

        // Тест 3: Конструктор с строкой без информации о сервере
        [Fact]
        public void Constructor_ConnectionStringWithoutServer_ThrowsArgumentException()
        {
            // Arrange
            string invalidConnectionString = "Database=TestDB;User Id=user;Password=pass;";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new DatabaseAccessor(invalidConnectionString));

            Assert.Equal("connectionString", exception.ParamName);
            Assert.Contains("server information", exception.Message);
        }

        // Тест 4: Конструктор с строкой без информации о базе данных
        [Fact]
        public void Constructor_ConnectionStringWithoutDatabase_ThrowsArgumentException()
        {
            // Arrange
            string invalidConnectionString = "Server=localhost;User Id=user;Password=pass;";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new DatabaseAccessor(invalidConnectionString));

            Assert.Equal("connectionString", exception.ParamName);
            Assert.Contains("database name", exception.Message);
        }

        // Тест 5: Конструктор с простым паролем
        [Fact]
        public void Constructor_ConnectionStringWithWeakPassword_ThrowsArgumentException()
        {
            // Arrange
            string weakPasswordConnectionString =
                "Server=localhost;Database=TestDB;User Id=user;Password=123456;";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new DatabaseAccessor(weakPasswordConnectionString));

            Assert.Equal("connectionString", exception.ParamName);
            Assert.Contains("Weak password", exception.Message);
        }

        // Тест 6: ExecuteNonQueryAsync с пустым SQL
        [Fact]
        public async Task ExecuteNonQueryAsync_EmptySql_ThrowsArgumentException()
        {
            // Arrange
            var accessor = new DatabaseAccessor(ValidConnectionString);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                accessor.ExecuteNonQueryAsync(""));

            Assert.Equal("sql", exception.ParamName);
            Assert.Contains("cannot be null or empty", exception.Message);
        }

        // Тест 7: ExecuteNonQueryAsync без подключения
        [Fact]
        public async Task ExecuteNonQueryAsync_NotConnected_ThrowsInvalidOperationException()
        {
            // Arrange
            var accessor = new DatabaseAccessor(ValidConnectionString);
            accessor.Disconnect();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                accessor.ExecuteNonQueryAsync("UPDATE Users SET Name='Test'"));

            Assert.Contains("not connected", exception.Message);
        }

        // Тест 8: ExecuteNonQueryAsync с SELECT запросом
        [Fact]
        public async Task ExecuteNonQueryAsync_SelectQuery_ThrowsInvalidSqlException()
        {
            // Arrange
            var accessor = new DatabaseAccessor(ValidConnectionString);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidSqlException>(() =>
                accessor.ExecuteNonQueryAsync("SELECT * FROM Users"));

            Assert.Equal("SELECT_NOT_ALLOWED", exception.ErrorCode);
            Assert.Contains("cannot execute SELECT", exception.Message);
        }

        // Тест 9: ExecuteNonQueryAsync с опасной операцией
        [Fact]
        public async Task ExecuteNonQueryAsync_DangerousOperation_ThrowsInvalidSqlException()
        {
            // Arrange
            var accessor = new DatabaseAccessor(ValidConnectionString);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidSqlException>(() =>
                accessor.ExecuteNonQueryAsync("DROP DATABASE ProductionDB"));

            Assert.Equal("DANGEROUS_OPERATION", exception.ErrorCode);
            Assert.Contains("Dangerous operation", exception.Message);
        }

        // Тест 10: ExecuteScalarAsync с пустым SQL
        [Fact]
        public async Task ExecuteScalarAsync_EmptySql_ThrowsArgumentException()
        {
            // Arrange
            var accessor = new DatabaseAccessor(ValidConnectionString);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                accessor.ExecuteScalarAsync(""));

            Assert.Equal("sql", exception.ParamName);
        }

        // Тест 11: ExecuteScalarAsync с не-SELECT запросом
        [Fact]
        public async Task ExecuteScalarAsync_NonSelectQuery_ThrowsInvalidSqlException()
        {
            // Arrange
            var accessor = new DatabaseAccessor(ValidConnectionString);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidSqlException>(() =>
                accessor.ExecuteScalarAsync("INSERT INTO Users VALUES (1, 'Test')"));

            Assert.Equal("NON_SELECT_QUERY", exception.ErrorCode);
            Assert.Contains("can only execute SELECT", exception.Message);
        }

        // Тест 12: ExecuteScalarAsync без подключения
        [Fact]
        public async Task ExecuteScalarAsync_NotConnected_ThrowsInvalidOperationException()
        {
            // Arrange
            var accessor = new DatabaseAccessor(ValidConnectionString);
            accessor.Disconnect();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                accessor.ExecuteScalarAsync("SELECT COUNT(*) FROM Users"));

            Assert.Contains("not connected", exception.Message);
        }

        // Дополнительный тест: Успешное выполнение ExecuteNonQueryAsync
        [Fact]
        public async Task ExecuteNonQueryAsync_ValidQuery_ReturnsResult()
        {
            // Arrange
            var accessor = new DatabaseAccessor(ValidConnectionString);

            // Act
            var result = await accessor.ExecuteNonQueryAsync("UPDATE Users SET Name='Test' WHERE Id=1");

            // Assert
            Assert.Equal(1, result);
        }

        // Дополнительный тест: Успешное выполнение ExecuteScalarAsync
        [Fact]
        public async Task ExecuteScalarAsync_ValidSelect_ReturnsData()
        {
            // Arrange
            var accessor = new DatabaseAccessor(ValidConnectionString);

            // Act
            var result = await accessor.ExecuteScalarAsync("SELECT Name FROM Users WHERE Id=1");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("SampleData", result);
        }
    }
}