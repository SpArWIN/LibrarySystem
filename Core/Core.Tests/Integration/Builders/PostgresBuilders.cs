using Testcontainers.PostgreSql;

namespace Core.Tests.Integration.Builders;

public static class PostgresBuilders
{
    /// <summary>
    /// Собрать контейнер.
    /// </summary>
    /// <param name="image">Образ.</param>
    /// <param name="database">Название базы данных.</param>
    /// <param name="username">Имя пользователя.</param>
    /// <param name="password">Пароль.</param>
    /// <param name="hostPort">Порт.</param>
    /// <returns><see cref="PostgreSqlContainer"/>.</returns>
    public static PostgreSqlContainer BuildTestContainer(
        string image = "postgres:13.20-alpine",
        string database = "testdb",
        string username = "testuser", 
        string password = "testpassword",
        int? hostPort = null)
    {
        var builder = new PostgreSqlBuilder()
            .WithImage(image)
            .WithDatabase(database)
            .WithUsername(username)
            .WithPassword(password)
            .WithCleanUp(true);
    
        if (hostPort.HasValue)
        {
            builder.WithPortBinding(hostPort.Value, 5432);
        }
        else
        {
            builder.WithPortBinding(5432, true);
        }
    
        return builder.Build();
    }
}