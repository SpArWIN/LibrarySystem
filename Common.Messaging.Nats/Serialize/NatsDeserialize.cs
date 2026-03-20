using System.Buffers;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Common.Messaging.Nats.Contracts.Based;
using NATS.Client.Core;
using Serilog;

namespace Common.Messaging.Nats.Serialize;

/// <summary>
/// Десериализатор сообщений Nats.
/// </summary>
/// <typeparam name="T">.</typeparam>
public sealed class NatsDeserialize<T> : INatsDeserialize<T>
where T : ILibraryMessage
{
    private readonly  JsonSerializerOptions _options;
    private static readonly ILogger Logger = Log.ForContext<NatsSerialize<T>>();
    /// <summary>
    /// Конструктор с настройками по умолчанию.
    /// </summary>
    public NatsDeserialize()
    {
        _options = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };
    }
    
    /// <summary>
    /// Конструктор с пользовательскими настройками JSON.
    /// </summary>
    /// <param name="options">Настройки JSON сериализации.</param>
    public NatsDeserialize(JsonSerializerOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }
    
    /// <inheritdoc />
    public T? Deserialize(in ReadOnlySequence<byte> buffer)
    {
        if (buffer.IsEmpty)
        {
            Logger.Fatal("Buffer is empty");
            throw new ArgumentNullException(nameof(buffer));
        }

        try
        {
            var bytes = buffer.IsSingleSegment 
                ? 
                buffer.First.ToArray() : 
                buffer.ToArray();
            var result = JsonSerializer.Deserialize<T>(bytes, _options);
            if (result is not null) return result;
            Logger.Fatal("Deserialization failed");
            throw new InvalidOperationException($" Deserialize return null for type {typeof(T).Name}");
            
        }
        catch (Exception e)
        {
           Logger.Error( e,"Error Deserialization");
            throw;
        }
    }
}