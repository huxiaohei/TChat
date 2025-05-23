using Google.Protobuf;
using Abstractions.Module;

namespace Abstractions.Grains
{
    public interface IPlayer
    {
        long RoleId { get; }

        T? GetModule<T>() where T : IBaseModule;

        Task SendMessageAsync(IMessage message);
    }
}