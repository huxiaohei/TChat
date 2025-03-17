/*****************************************************************
 * Description
 * Email huxiaoheigame@gmail.com
 * Created on 2024/12/2 23:14:49
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

using Abstractions.Message;

namespace ChatServer.Message
{
    public class MessageHandler : IMessageHandler
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

        public virtual Task<ISCMessage?> HandleMessageAsync(long sessionId, ICSMessage message)
        {
            throw new NotImplementedException("HandleMessage is not implemented.");
        }
    }
}