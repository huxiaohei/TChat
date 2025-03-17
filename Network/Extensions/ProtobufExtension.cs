/*****************************************************************
 * Description
 * Email huxiaoheigame@gmail.com
 * Created on 2024/11/26 14:51:22
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

using Google.Protobuf;
using Google.Protobuf.Reflection;
using System.Reflection;
using Utils.LoggerUtil;

namespace Network.Extensions
{
    public static class ProtobufExtension
    {
        private static readonly object _mutex = new();
        private static readonly Dictionary<string, MessageParser> _parsers = [];

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
                    Loggers.Network.Info($"-----fullName:{typeInfo.FullName} name:{name}");
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
    }
}