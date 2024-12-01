/*****************************************************************
 * Description 
 * Email huxiaoheigame@gmail.com
 * Created on 2024/11/28 18:03:31
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

using TChat.Abstractions.Grains;
using TChat.Abstractions.Message;

namespace TChat.ChatServer.Message
{
    public class ChatMessageHandler() : IMessageHandler
    {
        private IClusterClient? _clusterClient;

        private SiloAddress? _siloAddress;

        public IClusterClient ClusterClient
        {
            get
            {
                if (_clusterClient == null)
                {
                    throw new InvalidOperationException("ClusterClient is not bound.");
                }
                return _clusterClient;
            }
        }

        public SiloAddress SiloAddress
        {
            get
            {
                if (_siloAddress == null)
                {
                    throw new InvalidOperationException("SiloAddress is not bound.");
                }
                return _siloAddress;
            }
        }

        public void Bind(IClusterClient clusterClient, SiloAddress siloAddress)
        {
            _clusterClient = clusterClient;
            _siloAddress = siloAddress;
        }

        public async Task<ISCMessage?> HandleMessage(long sessionId, ICSMessage message)
        {
            return await ClusterClient.GetGrain<IPlayerGrain>(message.RoleId).ProcessMessage(SiloAddress, sessionId, message);
        }
    }

}