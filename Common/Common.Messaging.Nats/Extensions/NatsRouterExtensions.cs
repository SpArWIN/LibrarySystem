using System.Linq.Expressions;
using System.Reflection;
using Common.Messaging.Nats.Contracts.Based;
using Common.Messaging.Nats.Handlers;
using Common.Messaging.Nats.Processors;
using Common.Messaging.Nats.Router;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Messaging.Nats.Extensions;

/// <summary>
/// Регистрация реейстра обработчика событий.
/// </summary>
public static class NatsRouterExtensions
{
    private static readonly Dictionary<(Type Request, Type Response),
            Action<NatsRequestRouter<LibraryMessageBase>>> 
        RegisterProcessorCache = new();
    
    /// <summary>
    /// Добавить обработчики процессоров в Nats.
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/>.</param>
    /// <param name="router"><see cref="NatsRequestRouter{TBaseMessage}"/>.</param>
    /// <param name="assemblies">Массив сборок.</param>
    /// <returns></returns>
    public static IServiceCollection AddNatsRequestProcessors(
        this IServiceCollection services,
        NatsRequestRouter<LibraryMessageBase> router,
        params Assembly[] assemblies
    )
    {
        var processorTypes = FindProcessorsType(assemblies);
        foreach (var processor in processorTypes)
        {
            RegisterProcessorType(services, router, processor);
        }
        return services;
    }

    /// <summary>
    /// Регистрирует все реализации <see cref="INatsMessageHandler{TMessage}"/> из указанных сборок.
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/>.</param>
    /// <param name="assemblies">Сборки для сканирования.</param>
    public static IServiceCollection AddNatsMessageHandlers(
        this IServiceCollection services,
        params Assembly[] assemblies)
    {
        var handlerTypes = FindHandlerTypes(assemblies);
        foreach (var handlerType in handlerTypes)
        {
            RegisterHandlerType(services, handlerType);
        }

        return services;
    }

    private static IEnumerable<Type> FindHandlerTypes(IEnumerable<Assembly> assemblies)
    {
        foreach (var assembly in assemblies)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (IsConcreteHandler(type))
                {
                    yield return type;
                }
            }
        }
    }

    private static void RegisterHandlerType(IServiceCollection services, Type handlerType)
    {
        var handlerInterfaces = handlerType.GetInterfaces().Where(IsHandlerInterface);
        foreach (var handlerInterface in handlerInterfaces)
        {
            var messageType = handlerInterface.GetGenericArguments()[0];
            ValidateHandlerMessageType(messageType, handlerType);
            services.AddScoped(handlerInterface, handlerType);
        }
    }

    private static void ValidateHandlerMessageType(Type messageType, Type handlerType)
    {
        if (!typeof(LibraryMessageBase).IsAssignableFrom(messageType))
        {
            throw new InvalidOperationException(
                $"Тип сообщения '{messageType.Name}' в обработчике '{handlerType.Name}' " +
                $"должен наследовать {nameof(LibraryMessageBase)}.");
        }
    }

    private static bool IsConcreteHandler(Type type)
    {
        return type is { IsAbstract: false, IsInterface: false }
               && type.GetInterfaces().Any(IsHandlerInterface);
    }

    private static bool IsHandlerInterface(Type @interface)
    {
        return @interface.IsGenericType
               && @interface.GetGenericTypeDefinition() == typeof(INatsMessageHandler<>);
    }

    private static IEnumerable<Type> FindProcessorsType(IEnumerable<Assembly> assemblies)
    {
        foreach (var assembly in assemblies)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (IsConcreteProcessor(type))
                {
                    yield return type;
                }
            }
        }
    }

    private static void RegisterProcessorType(
        IServiceCollection services,
        NatsRequestRouter<LibraryMessageBase> router,
        Type processorType
    )
    {
        var processorInterfaces = processorType.GetInterfaces().Where(IsProcessorInterface);
        foreach (var processorInterface in processorInterfaces)
        {
            RegisterSingleProcessor(
                services,
                router,
                processorType,
                processorInterface);
        }
    }

    private static void RegisterSingleProcessor(
        IServiceCollection services,
        NatsRequestRouter<LibraryMessageBase> router,
        Type implementationType,
        Type processorInterface
    )
    {
        var genericArguments = processorInterface.GetGenericArguments();
        var requestType = genericArguments[0];
        var responseType = genericArguments[1];
        ValidateRequestType(requestType, implementationType);
        services.AddScoped(processorInterface, implementationType);
        RegisterInRouter(router, requestType, responseType);
    }

    private static void RegisterInRouter(
        NatsRequestRouter<LibraryMessageBase> router,
        Type requestType,
        Type responseType
    )
    {
        var key = (requestType, responseType);
        if (!RegisterProcessorCache.TryGetValue(key, out var action))
        {
            action = BuildRegisterProcessorAction(requestType, responseType);
            RegisterProcessorCache.Add(key, action);
        }
        action(router);
    }
    
    private static void ValidateRequestType(
        Type requestType,
        Type processorType)
    {
        if (!typeof(LibraryMessageBase).IsAssignableFrom(requestType))
        {
            throw new InvalidOperationException(
                $"Request type '{requestType.Name}' in processor '{processorType.Name}' " +
                $"must inherit from {nameof(LibraryMessageBase)}.");
        }
    }

    private static bool IsConcreteProcessor(Type type)
    {
        return type is { IsAbstract: false, IsInterface: false }
               && type.GetInterfaces().Any(IsProcessorInterface);
    }

    private static bool IsProcessorInterface(Type @interface)
    {
        return @interface.IsInterface
               && @interface.GetGenericTypeDefinition() == typeof(INatsRequestProcessor<,>);
    }

    private static Action<NatsRequestRouter<LibraryMessageBase>> BuildRegisterProcessorAction(
        Type requestType,
        Type responseType)
    {
        var routerType = typeof(NatsRequestRouter<LibraryMessageBase>);
        var method = routerType
            .GetMethod(nameof(NatsRequestRouter<LibraryMessageBase>.RegisterProcessors))!
            .MakeGenericMethod(requestType, responseType);
        var routerParam = Expression.Parameter(routerType, "router");
        var call = Expression.Call(routerParam, method);
        var lamda = Expression.Lambda<Action<NatsRequestRouter<LibraryMessageBase>>>(
            call, routerParam);
        return lamda.Compile();
    }
}