using System;
using System.Threading.Tasks;

namespace Flexinets.Radius.Core
{
    public interface IPacketHandler : IDisposable
    {
        Task<IRadiusPacket?> HandlePacket(IRadiusPacket packet);
    }
}
