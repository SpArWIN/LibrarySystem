using System.Buffers;
using System.Runtime.Serialization;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Common.Messaging.Nats.Contracts.Based;
using NATS.Client.Core;
using Serilog;

namespace Common.Messaging.Nats.Serialize;

public sealed class NatsSerialize<T> :  INatsSerializer<T>
where T : ILibraryMessage
{
    private static readonly ILogger Logger = Log.ForContext<NatsSerialize<T>>();
    private readonly JsonSerializerOptions _options;

    /// <summary>
    /// Ctor с настройками по дефолту.
    /// </summary>
    public NatsSerialize()
    {
        _options = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            Converters = { new JsonStringEnumConverter() },
        };
    }

    /// <summary>
    /// Конструктор при передаче настроек сериализации.
    /// </summary>
    /// <param name="options"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public NatsSerialize(JsonSerializerOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }
    
    /// <inheritdoc />
    public void Serialize(IBufferWriter<byte> bufferWriter, T value)
    {
        if (bufferWriter is null)
        {
            throw new ArgumentNullException(nameof(bufferWriter));
        }

        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        try
        {
            var jsonBytes = JsonSerializer.SerializeToUtf8Bytes(value, _options);
            bufferWriter.Write(jsonBytes);
        }
        catch (SerializationException e)
        {
            Logger.Fatal(e, "Failed to deserialize  message");
            throw new InvalidOperationException($"Failed to deserialize  message type {typeof(T).Name}", e);
        }
        catch (JsonException ex)
        {
            Logger.Fatal(ex, "Failed to deserialize  message");
        }
    }

    public T? Deserialize(in ReadOnlySequence<byte> buffer)
    {
        if (buffer.IsEmpty)
        {
            return default;
        }

        if (buffer.IsSingleSegment)
        {
            return JsonSerializer.Deserialize<T>(buffer.FirstSpan, _options);
        }

        return JsonSerializer.Deserialize<T>(buffer.ToArray(), _options);
    }

    public INatsSerializer<T> CombineWith(INatsSerializer<T> next)
    {
        throw new NotImplementedException();
    }
}