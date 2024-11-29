/*****************************************************************
 * Description 
 * Email huxiaoheigame@gmail.com
 * Created on 2024/11/28 11:22:35
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

using Orleans.Runtime.Services;
using TChat.Abstractions.Grains;
using TChat.Abstractions.Message;

namespace TChat.ChatServer.Grains
{
    public class BaseGrainServiceClient : GrainServiceClient<IBaseGrainService>, IBaseGrainServiceClient
    {
        public IServiceProvider Provider { get; }
        public SiloAddress SiloAddress { get; }

        public BaseGrainServiceClient(IServiceProvider provider, ILocalSiloDetails localSiloDetails)
            : base(provider)
        {
            Provider = provider;
            SiloAddress = localSiloDetails.SiloAddress;
        }

        public async Task SendMessageAsync(SiloAddress siloAddress, long sessionId, ISCMessage message)
        {
            await GetGrainService(siloAddress).SendMessageAsync(sessionId, message);
        }

        public async Task CloseSessionAsync(SiloAddress siloAddress, long sessionId)
        {
            await GetGrainService(siloAddress).CloseSessionAsync(sessionId);
        }
    }
}