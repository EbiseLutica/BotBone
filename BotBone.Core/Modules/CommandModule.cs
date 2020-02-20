using System.Threading.Tasks;
using BotBone.Core.Api;

namespace BotBone.Core.Modules
{
	public class CommandModule : ModuleBase
	{
		public override int Priority => -10000;
		public override async Task<bool> ActivateAsync(IPost n, IShell shell, Server core)
		{
			var t = n.Text?.TrimMentions();
			if (string.IsNullOrEmpty(t))
				return false;
			if (t.StartsWith("/"))
			{
				string response;
				try
				{
					response = await core.ExecCommand(new PostCommandSender(n, core.IsSuperUser(n.User)), t);
				}
				catch (AdminOnlyException)
				{
					response = "そのコマンドは管理者限定です。";
				}
				catch (LocalOnlyException)
				{
					response = "そのコマンドはローカルユーザー限定です。";
				}
				catch (RemoteOnlyException)
				{
					response = "そのコマンドはリモートユーザー限定です。";
				}
				catch (NoSuchCommandException)
				{
					response = "No such command.";
				}

				if (response != default)
				{
					await shell.ReplyAsync(n, response);
				}
				EconomyModule.Pay(n, shell, core);
				return true;
			}
			return false;
		}
	}
}
