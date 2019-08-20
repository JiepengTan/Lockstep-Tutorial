using System;
using System.Net;
using System.Threading.Tasks;

namespace Lockstep.Network
{
	public enum NetworkProtocol
	{
		TCP,
	}

	public abstract class AService :NetBase
	{
		public abstract AChannel GetChannel(long id);

		public abstract Task<AChannel> AcceptChannel();

		public abstract AChannel ConnectChannel(IPEndPoint ipEndPoint);

		public abstract void Remove(long channelId);

		public abstract void Update();
	}
}