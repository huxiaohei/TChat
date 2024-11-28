/*****************************************************************
 * Description 
 * Email huxiaoheigame@gmail.com
 * Created on 2024/11/28 11:22:35
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

using TChat.Abstractions.Grains;
using TChat.Abstractions.Message;

namespace TChat.ChatServer.Grains
{
    public class BaseGrainServiceClient : IBaseGrainServiceClient
    {
        public IServiceProvider Provider { get; }
        public SiloAddress SiloAddress { get; }

        public BaseGrainServiceClient(IServiceProvider provider, ILocalSiloDetails localSiloDetails)
        {
            Provider = provider;
            SiloAddress = localSiloDetails.SiloAddress;
        }

        public Task SendMessageAsync(SiloAddress siloAddress, long sessionId, IBaseMessage message)
        {
            throw new NotImplementedException();
        }

        public Task CloseSessionAsync(SiloAddress siloAddress, long sessionId)
        {
            throw new NotImplementedException();
        }
    }
}