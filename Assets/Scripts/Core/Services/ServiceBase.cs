using System;
using Scarlet.Core.Logging.Interfaces;
using Scarlet.Core.Services.Interfaces;

namespace Scarlet.Core.Services
{
	public abstract class ServiceBase : IService
	{
		protected readonly CoreSandbox Core;
		protected readonly ILogger Logger;

		public CoreId OwnerId => Core.Id;

		protected ServiceBase(CoreSandbox core)
		{
			Core = core ?? throw new ArgumentNullException(nameof(core));
			Logger = core.GetService<ILogger>();
		}

		public virtual void Initialize() { }
		public virtual void Cleanup() { }
		public virtual void Update() { }
	}
}
