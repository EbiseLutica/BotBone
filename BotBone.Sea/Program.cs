using System;
using System.Threading.Tasks;
using BotBone.Core;

namespace BotBone.Sea
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("BotBone version " + Server.Version);
            var logger = new Logger("Bootstrap");
            logger.Info("BotBone.Sea " + Shell.Version);
            var sh = await Shell.InitializeAsync();
            logger.Info("シェルを初期化しました！");
            logger.Info("起動しました！");

            Console.CancelKeyPress += (s, e) =>
            {
                logger.Info("シェルを停止します。");
                logger.Info("Bye");
            };

            await Task.Delay(-1);
        }
    }
}
