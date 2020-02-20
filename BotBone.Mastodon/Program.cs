using System;
using System.Threading.Tasks;
using BotBone.Core;

namespace BotBone.Mastodon
{
	class Program
	{
		static async Task Main(string[] args)
		{
			Console.WriteLine(Server.BotBoneAA + " version " + Server.Version);
			var logger = new Logger("Bootstrap");
			logger.Info("BotBone.Mastodon " + Shell.Version);
			var sh = await Shell.InitializeAsync();
			logger.Info("シェルを初期化しました！");
			logger.Info("起動しました！");

			await Task.Delay(-1);
		}
	}

}
