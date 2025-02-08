using System;
using System.Threading.Tasks;
using Scarlet.Core.Async.Interfaces;
using Scarlet.Core.Services.Interfaces;

namespace Scarlet.Core.Communication.Interfaces
{
	public interface ICoreCommunicator : IService
	{
		event Action<CoreId, ICoreMessage> MessageReceived;
		IAsyncOperation<bool> InitializeAsync(CoreId coreId);
		IAsyncOperation<bool> SendMessageAsync(CoreId targetId, ICoreMessage message);
		IAsyncOperation<bool> BroadcastAsync(ICoreMessage message);
		IAsyncOperation<bool> IsCoreAvailableAsync(CoreId coreId);
	}
}
