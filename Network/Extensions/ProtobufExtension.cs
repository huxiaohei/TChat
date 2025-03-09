/*****************************************************************
 * Description
 * Email huxiaoheigame@gmail.com
 * Created on 2024/11/26 14:51:22
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

using Google.Protobuf;
using Google.Protobuf.Reflection;

using System.Reflection;
using Utils.Log;

namespace Network.Extensions
{
    public static class ProtobufExtension
    {
        private static object _mutex = new object();
        private static Dictionary<string, MessageParser> _parsers = new Dictionary<string, MessageParser>();

        private static void LoadParseFromAssembly(Assembly ass)
        {
            var types = ass.GetTypes();
            foreach (var typeInfo in types)
            {
                var attrParser = typeInfo.GetProperty("Parser");
                var attrDescriptor = typeInfo.GetProperty("Descriptor");
                if (attrParser == null || attrDescriptor == null)
                    continue;
                try
                {
                    var parser = attrParser.GetValue("") as MessageParser;
                    var parserDescriptor = attrDescriptor.GetValue("") as MessageDescriptor;
                    if (parser == null || parserDescriptor == null)
                        continue;
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
    }
}