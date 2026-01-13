using System;
using System.Threading.Tasks;

namespace ExceptionTestingDemo
{
    // Пользовательское исключение для невалидных SQL-запросов
    public class InvalidSqlException : Exception
    {
        public string SqlQuery { get; }
        public string ErrorCode { get; }

        public InvalidSqlException(string sqlQuery, string errorCode, string message)
            : base(message)
        {
            SqlQuery = sqlQuery;
            ErrorCode = errorCode;
        }
    }

    public class DatabaseAccessor
    {
        private string _connectionString;
        private bool _isConnected;

        public DatabaseAccessor(string connectionString)
        {
            ValidateConnectionString(connectionString);
            _connectionString = connectionString;
            _isConnected = true; // После успешной валидации считаем подключенным
        }

        /// <summary>
        /// Выполняет SQL-запрос, не возвращающий данные (INSERT, UPDATE, DELETE)
        /// </summary>
        public async Task<int> ExecuteNonQueryAsync(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql))
                throw new ArgumentException("SQL query cannot be null or empty", nameof(sql));

            if (!_isConnected)
                throw new InvalidOperationException("Database is not connected");

            if (sql.Trim().ToUpper().StartsWith("SELECT"))
                throw new InvalidSqlException(sql, "SELECT_NOT_ALLOWED",
                    "ExecuteNonQueryAsync cannot execute SELECT queries");

            // Имитация опасной операции
            if (sql.ToUpper().Contains("DROP DATABASE"))
                throw new InvalidSqlException(sql, "DANGEROUS_OPERATION",
                    "Dangerous operation detected: DROP DATABASE");

            // Имитация выполнения запроса
            await Task.Delay(50);
            return 1; // Имитация успешного выполнения
        }

        /// <summary>
        /// Выполняет SQL-запрос, возвращающий одно значение
        /// </summary>
        public async Task<object> ExecuteScalarAsync(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql))
                throw new ArgumentException("SQL query cannot be null or empty", nameof(sql));

            if (!_isConnected)
                throw new InvalidOperationException("Database is not connected");

            if (!sql.Trim().ToUpper().StartsWith("SELECT"))
                throw new InvalidSqlException(sql, "NON_SELECT_QUERY",
                    "ExecuteScalarAsync can only execute SELECT queries");

            // Имитация выполнения запроса
            await Task.Delay(50);
            return "SampleData";
        }

        /// <summary>
        /// Валидирует строку подключения к базе данных
        /// </summary>
        public void ValidateConnectionString(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Connection string cannot be null or empty", nameof(connectionString));

            if (!connectionString.Contains("Server=") && !connectionString.Contains("Data Source="))
                throw new ArgumentException("Connection string must contain server information", nameof(connectionString));

            if (!connectionString.Contains("Database=") && !connectionString.Contains("Initial Catalog="))
                throw new ArgumentException("Connection string must contain database name", nameof(connectionString));

            // Проверка на простые пароли
            if (connectionString.Contains("Password="))
            {
                int passwordStart = connectionString.IndexOf("Password=") + 9;
                int passwordEnd = connectionString.IndexOf(";", passwordStart);
                if (passwordEnd == -1) passwordEnd = connectionString.Length;
                string password = connectionString.Substring(passwordStart, passwordEnd - passwordStart);

                if (password == "123456" || password == "password")
                    throw new ArgumentException("Weak password detected", nameof(connectionString));
            }
        }

        /// <summary>
        /// Метод для тестирования - отключает базу данных
        /// </summary>
        public void Disconnect()
        {
            _isConnected = false;
        }
    }
}