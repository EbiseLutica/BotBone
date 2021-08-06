using System.Threading.Tasks;

namespace BotBone.Core
{
	public abstract class PluginBase
	{
		/// <summary>
		/// 有効化されたときに呼ばれます。
		/// </summary>
		public virtual Task OnEnabled(Server server) => Task.Delay(0);

		/// <summary>
		/// 無効化されたときに呼ばれます。
		/// </summary>
		public virtual Task OnDisabled(Server server) => Task.Delay(0);
	}
}
