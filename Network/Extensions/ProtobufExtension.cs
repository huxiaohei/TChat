/*****************************************************************
 * Description
 * Email huxiaoheigame@gmail.com
 * Created on 2024/11/26 14:51:22
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

using Google.Protobuf;
using Utils.LoggerUtil;
using System.Reflection;
using Abstractions.Grains;
using Abstractions.Message;
using Google.Protobuf.Reflection;
using Network.Message;
using Network.Protos;

namespace Network.Extensions
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class MessageCallbackAttribute(Type msg) : Attribute
    {
        public readonly Type Msg = msg;
    }

    public static class ProtobufExtension
    {
        private static readonly object _mutex = new();

        private static readonly Dictionary<string, MessageParser> _parsers = [];

        public delegate Task<IMessage?> MessageHandler(IPlayer player, ICSMessage message);

        public static readonly Dictionary<Type, MessageHandler> MessageHandlers = [];

        private static void LoadParseFromAssembly(Assembly ass)
        {
            var types = ass.GetTypes();
            foreach (var typeInfo in types)
            {
                if (typeInfo.FullName == null)
                {
                    continue;
                }
                if (!typeInfo.FullName.StartsWith("Network.Protos") && !typeInfo.FullName.StartsWith("Google.Protobuf"))
                {
                    continue;
                }
                var attrParser = typeInfo.GetProperty("Parser");
                var attrDescriptor = typeInfo.GetProperty("Descriptor");
                if (attrParser == null || attrDescriptor == null)
                {
                    continue;
                }
                try
                {
                    if (attrParser.GetValue("") is not MessageParser parser || attrDescriptor.GetValue("") is not MessageDescriptor parserDescriptor)
                    {
                        continue;
                    }
                    var name = parserDescriptor.Name;
                    _parsers.Add(string.Intern(name), parser);
                }
                catch (Exception e)
                {
                    Loggers.Network.Warn($"LoadParseFromAssembly typeInfo:{typeInfo} err:{e}");
                }
            }
        }

        private static void LoadMessageParsers()
        {
            lock (_mutex)
            {
                if (_parsers.Count > 0) return;
                var assList = AppDomain.CurrentDomain.GetAssemblies();
                foreach (var ass in assList)
                {
                    if (ass.IsDynamic)
                        continue;
                    LoadParseFromAssembly(ass);
                }
            }
        }

        public static IMessage? DecodeMessage(this byte[] bytes, string msgName, int offset)
        {
            if (_parsers.Count == 0)
            {
                LoadMessageParsers();
            }
            if (_parsers.TryGetValue(msgName, out var parser))
            {
                return parser.ParseFrom(bytes, offset, bytes.Length - offset);
            }
            return null;
        }

        public static void InitMessageHandlers()
        {
            if (MessageHandlers.Count > 0)
            {
                return;
            }
            lock (_mutex)
            {
                if (MessageHandlers.Count > 0)
                {
                    return;
                }
                var assembly = Assembly.LoadFrom("./bin/Debug/net8.0/Modules.dll");
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

                        Loggers.Chat.Warn($"======Method {method.Name} in {type.Name}");

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

        public static IMessage Msg(this ErrCode code)
        {
            return new SCErrResp()
            {
                ErrCode = code,
                ErrMsg = code.ToString()
            };
        }

        public static SCMessage Msg(this ErrCode code, long roleId, uint clientMsgSerialId = 0)
        {
            return new SCMessage(roleId, clientMsgSerialId, 0, code.Msg());
        }
    }
}