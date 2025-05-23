﻿/*****************************************************************
 * Description
 * Email huxiaoheigame@gmail.com
 * Created on 2024/11/28 18:09:37
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

using Network.Protos;
using Google.Protobuf;
using Network.Message;
using Utils.Container;
using Utils.LoggerUtil;
using System.Reflection;
using Network.Extensions;
using Abstractions.Grains;
using Abstractions.Module;
using Abstractions.Message;
using ChatServer.Extensions;

namespace ChatServer.Grains
{
    public class PlayerGrain : Grain, IPlayerGrain, IPlayer
    {
        private readonly long PrimaryKey;
        private SiloAddress? _siloAddress;
        private long _sessionId;
        private uint _serverMsgSerialId = 0;
        private readonly IBaseGrainServiceClient _client;
        private readonly FixedQueue<ISCMessage> CacheMessageQueue = new(20);
        private readonly Dictionary<string, IBaseModule> _modules = [];

        public long RoleId => PrimaryKey;

        public PlayerGrain(IGrainContext grainContext, IGrainRuntime grainRuntime, IBaseGrainServiceClient client)
            : base(grainContext, grainRuntime)
        {
            PrimaryKey = this.GetPrimaryKeyLong();
            _client = client;
            _serverMsgSerialId = 0;
        }

        public override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            Loggers.Chat.Info($"PlayerGrain {RoleId} activated");
            await base.OnActivateAsync(cancellationToken);
            await InitModuleAsync();
        }

        public override async Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
        {
            Loggers.Chat.Info($"PlayerGrain {RoleId} deactivated reason:{reason}");
            await base.OnDeactivateAsync(reason, cancellationToken);
        }

        private async Task InitModuleAsync()
        {
            var moduleTypes = Assembly.LoadFrom("./bin/Debug/net8.0/Modules11.dll").GetTypes()
                .Where(t => t.GetInterfaces().Contains(typeof(IBaseModule)))
                .ToList();
            foreach (var moduleType in moduleTypes)
            {
                if (moduleType.FullName == null)
                {
                    continue;
                }
                Loggers.Chat.Info($"Init module -- {moduleType.FullName}");
                var module = Activator.CreateInstance(moduleType, this) as IBaseModule
                             ?? throw new InvalidOperationException($"Failed to create an instance of {moduleType.FullName}");
                _modules.Add(moduleType.FullName, module);
            }
            await Task.WhenAll(_modules.Values.Select(m => m.InitAsync()));
        }

        public async Task<bool> HotfixModuleAsync(string scriptPath)
        {
            try
            {
                Loggers.Chat.Info($"hotfix path {scriptPath}");
                var moduleTypes = Assembly.LoadFrom(scriptPath).GetTypes()
                    .Where(t => t.GetInterfaces().Contains(typeof(IBaseModule)))
                    .ToList();
                foreach (var moduleType in moduleTypes)
                {
                    var module = Activator.CreateInstance(moduleType, this) as IBaseModule
                                 ?? throw new InvalidOperationException($"Failed to create an instance of {moduleType.FullName}");
                    if (_modules.TryGetValue(module.GetType().FullName!, out var oldModule))
                    {
                        _modules.Remove(module.GetType().FullName!);
                        await oldModule.DestroyAsync();
                    }
                    _modules.Add(module.GetType().FullName!, module);
                    await module.InitAsync();
                    Loggers.Chat.Info($"PlayerGrain {RoleId} hotfix module {module.GetType().FullName} success");
                }
            }
            catch (Exception ex)
            {
                Loggers.Chat.Error($"PlayerGrain {RoleId} hotfix module error:{ex.Message} stack:{ex.StackTrace}");
                return false;
            }
            return true;
        }

        public T? GetModule<T>() where T : IBaseModule
        {
            if (_modules.TryGetValue(typeof(T).FullName!, out var module))
            {
                return (T)module;
            }
            return default;
        }

        private void EnqueueCacheMessage(SCMessage resp)
        {
            CacheMessageQueue.Enqueue(resp);
        }

        public async Task SendMessageAsync(IMessage message)
        {
            var msg = new SCMessage(RoleId, 0, ++_serverMsgSerialId, message);
            EnqueueCacheMessage(msg);
            if (_siloAddress == null || _sessionId == 0)
            {
                Loggers.Chat.Info($"Send message to RoleId:{msg.RoleId} MsgName:{msg.MsgName} ClientSerialId:{msg.ClientSerialId} ServerSerialId:{msg.ServerSerialId} Message:{msg.Message}");
                return;
            }
            await _client.SendMessageAsync(_siloAddress, _sessionId, msg);
        }

        public async Task SendMessageBatchAsync(IEnumerable<ISCMessage> messages, bool isCache = false)
        {
            if (_siloAddress == null)
            {
                return;
            }
            foreach (var message in messages)
            {
                message.IsCache = isCache;
                await _client.SendMessageAsync(_siloAddress, _sessionId, message);
            }
        }

        public async Task<bool> PingAsync()
        {
            return await Task.FromResult(true);
        }

        public async Task<ISCMessage?> ProcessMessageAsync(SiloAddress siloAddress, long sessionId, ICSMessage message)
        {
            if (_siloAddress == null)
            {
                _siloAddress = siloAddress;
                _sessionId = sessionId;
            }
            else
            {
                if (siloAddress != _siloAddress || sessionId != _sessionId)
                {
                    Loggers.Chat.Info($"PlayerGrain {RoleId} received message from wrong session {siloAddress} {sessionId}");
                    await _client.CloseSessionAsync(_siloAddress, _sessionId);
                }
                _siloAddress = siloAddress;
                _sessionId = sessionId;
            }

            ProtobufExtension.InitMessageHandlers();
            if (!ProtobufExtension.MessageHandlers.TryGetValue(message.Message.GetType(), out var handler))
            {
                Loggers.Chat.Warn($"PlayerGrain {RoleId} received unknown message {message.MsgName}");
                return default;
            }
            try
            {
                var rst = await handler(this, message);
                if (rst == null)
                {
                    return default;
                }
                var resp = new SCMessage(
                        RoleId,
                        message.ClientSerialId,
                        ++_serverMsgSerialId,
                        rst);
                EnqueueCacheMessage(resp);
                return resp;
            }
            catch (Exception ex)
            {
                Loggers.Chat.Error($"PlayerGrain process message:{message.MsgName} roleId:{RoleId} error:{ex}");
                return ErrCode.InternalError.Msg(RoleId, message.ClientSerialId);
            }
        }
    }
}