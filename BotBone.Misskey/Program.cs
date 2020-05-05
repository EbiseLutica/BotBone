using System;
using System.Threading.Tasks;
using BotBone.Core;
using static System.Console;

namespace BotBone.Misskey
{
	class Program
	{
		static async Task Main()
		{
			Console.WriteLine("BotBone version " + Server.Version);
			var logger = new Logger("Bootstrap");
			logger.Info("BotBone.Misskey " + Shell.Version);
			await Shell.InitializeAsync();
			logger.Info("シェルを初期化しました！");
			logger.Info("起動しました！");

			await Task.Delay(-1);
		}
	}
}
