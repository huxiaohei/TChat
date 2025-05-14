/*****************************************************************
 * Description
 * Email huxiaoheigame@gmail.com
 * Created on 2024/12/2 23:45:55
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

using Google.Protobuf;
using Utils.LoggerUtil;
using System.Reflection;
using ChatServer.Grains;
using Abstractions.Message;

namespace ChatServer.Extensions
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class MessageCallbackAttribute(Type msg) : Attribute
    {
        public readonly Type Msg = msg;
    }

    public static class MessageExtension
    {
        private static readonly object mutex = new();

        public delegate Task<IMessage?> MessageHandler(PlayerGrain player, ICSMessage message);

        public static readonly Dictionary<Type, MessageHandler> MessageHandlers = [];

        public static void InitMessageHandlers()
        {
            if (MessageHandlers.Count > 0)
            {
                return;
            }
            lock (mutex)
            {
                if (MessageHandlers.Count > 0)
                {
                    return;
                }
                var assembly = Assembly.GetExecutingAssembly();
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    var methods = type.GetMethods();
                    foreach (var method in methods)
                    {
                        var attributes = method.GetCustomAttributes(typeof(MessageCallbackAttribute), false);
                        if (attributes.Length != 1)
                        {
                            continue;
                        }
                        if (attributes.First() is not MessageCallbackAttribute attribute)
                        {
                            Loggers.Chat.Warn($"Method {method.Name} in {type.Name} has invalid attributes.");
                            continue;
                        }
                        if (method.GetParameters().Length != 2 || !typeof(ICSMessage).IsAssignableFrom(method.GetParameters()[1].ParameterType))
                        {
                            Loggers.Chat.Warn($"Method {method.Name} in {type.Name} has invalid parameters.");
                            continue;
                        }
                        if (method.ReturnType != typeof(Task<IMessage?>))
                        {
                            Loggers.Chat.Warn($"Method {method.Name} in {type.Name} has invalid return type.");
                            continue;
                        }
                        var handler = method.CreateDelegate<MessageHandler>();
                        MessageHandlers.Add(attribute.Msg, handler);
                        Loggers.Chat.Info($"Method {method.Name} in {type.Name} has been added to message handlers.");
                    }
                }
            }
        }

        public static T Cast<T>(this ICSMessage message) where T : class, IMessage
        {
            if (message.Message is not T result)
            {
                throw new InvalidCastException("Failed to cast message to the specified type.");
            }
            return result;
        }
    }
}