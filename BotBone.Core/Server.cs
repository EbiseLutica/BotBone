using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using BotBone.Core.Api;
using BotBone.Core.Modules;

namespace BotBone.Core
{
	/// <summary>
	/// BotBone's Core.
	/// </summary>
	public class Server
	{
		/// <summary>
		/// バージョンを取得します。
		/// </summary>
		public static string Version => "1.3.0";

		/// <summary>
		/// 読み込まれているモジュール一覧を取得します。
		/// </summary>
		public List<IModule> Modules { get; } = new();

		/// <summary>
		/// 読み込まれているコマンド一覧を取得します。
		/// </summary>
		public List<ICommand> Commands { get; } = new();

		/// <summary>
		/// 読み込まれているプラグイン一覧を取得します。
		/// </summary>
		public List<PluginBase> Plugins { get; } = new();

		/// <summary>
		/// シェルを取得します。
		/// </summary>
		public IShell Shell { get; }

		public Logger Logger => new("Core");

		public static Server? Current { get; private set; }

		/// <summary>
		/// 文脈の一覧を取得します。
		/// </summary>
		public Dictionary<string, (IModule, Dictionary<string, object>)> ContextPostDictionary { get; } = new Dictionary<string, (IModule, Dictionary<string, object>)>();

		/// <summary>
		/// ユーザーの一覧を取得します。
		/// </summary>
		public Dictionary<string, (IModule, Dictionary<string, object>)> ContextUserDictionary { get; } = new Dictionary<string, (IModule, Dictionary<string, object>)>();

		/// <summary>
		/// ユーザーストレージを取得します。
		/// </summary>
		/// <returns></returns>
		public UserStorage Storage { get; } = new UserStorage();

		/// <summary>
		/// 乱数ジェネレーターのインスタンスを取得します。
		/// </summary>
		public Random Random { get; } = new Random();

		static Server()
		{
			Http.DefaultRequestHeaders.Add("User-Agent", $"Mozilla/5.0 BotBone/{Server.Version} (https://github.com/xeltica/BotBone) .NET/{Environment.Version}");
		}

		/// <summary>
		/// bot を初期化します。
		/// </summary>
		public Server(IShell shell)
		{
			if (Current != null)
			{
				Logger.Warn("Server instance has been generated multiple.");
			}

			Shell = shell;

			const string pluginsPath = "./plugins";

			if (!Directory.Exists(pluginsPath))
			{
				Directory.CreateDirectory(pluginsPath);
			}

			var types = Directory.EnumerateFiles(pluginsPath, "*.dll")
				.Select(path =>
				{
					Logger.Info("Loading plugin from " + path);
					return Assembly.LoadFrom(path);
				})
				.SelectMany(a =>
				{
					var types = a.GetTypes();
					Logger.Info($"Loaded {types.Length} types");
					return types;
				})
				.ToList();

			// ここでToListしておかないと下で毎回インスタンス生成されてしまう
			var plugins = types
				.Where(t => typeof(IModule).IsAssignableFrom(t) || typeof(ICommand).IsAssignableFrom(t))
				.Where(a => a.GetConstructor(Type.EmptyTypes) != null)
				.Select(a => Activator.CreateInstance(a))
				.ToList();

			plugins
				.OfType<IModule>()
				.OrderBy(mod => mod.Priority)
				.ForEach(AddModule);

			Logger.Info($"Loaded {Modules.Count} modules");

			plugins
				.OfType<ICommand>()
				.ForEach(AddCommand);
			Logger.Info($"Loaded {Commands.Count} commands");

			plugins
				.OfType<PluginBase>()
				.ForEach(AddPlugin);
			Logger.Info($"Loaded {Plugins.Count} plugins");


			string adminId = "";

			if (File.Exists("./admin"))
			{
				// マイグレ
				Logger.Warn("管理者名の古い保存形式を使用しています。コンフィグファイルへのマイグレーションを開始します。");
				adminId = File.ReadAllText("./admin").Trim().ToLower();
				Config.Instance.Admin = adminId;
				Config.Instance.Save();
				File.Delete("./admin");
				Logger.Info("管理者名のデータを移行しました。");
			}
			if (string.IsNullOrEmpty(Config.Instance.Admin))
			{
				Console.Write("Admin's ID > ");
				adminId = Console.ReadLine()?.Trim().ToLower() ?? "";
				Config.Instance.Admin = adminId;
				Config.Instance.Save();
			}

			Logger.Info($"管理者はID {Config.Instance.Admin ?? "null"}。");

			if (Config.Instance.Moderators.Count > 0)
			{
				Logger.Info($"モデレーターは {string.Join(", ", Config.Instance.Moderators)}。");
			}
			else
			{
				Logger.Info("モデレーターは いません。");
			}

			if (File.Exists("./nicknames"))
			{
				// マイグレ
				Logger.Warn("古いニックネーム保存形式を使用しています。新しい UserStorage へのマイグレーションを開始します。");
				var lines = File.ReadAllLines("./nicknames");
				lines.Select(l =>
				{
					var kv = l.Split(',');
					return new KeyValuePair<string, string>(kv[0], string.Concat(kv.Skip(1)));
				})
				.ForEach(kv => Storage[kv.Key].Set(StorageKey.Nickname, kv.Value));
				File.Delete("./nicknames");
				Logger.Info($"{lines.Length} 人のニックネームを、新しい UserStorage に移行しました!");
			}

			Current = this;

			Plugins.ForEach(pl => pl.OnEnabled(this));
		}

		/// <summary>
		/// モジュールを追加します。
		/// </summary>
		public void AddModule(IModule mod)
		{
			if (Modules.Contains(mod))
				return;

			if (Modules != null)
			{
				Modules.Add(mod);
				Modules.Sort((m1, m2) => m1.Priority - m2.Priority);
				Logger.Info("Registered a module " + mod.GetType().Name);
			}
		}

		/// <summary>
		/// コマンドを追加します。
		/// </summary>
		public void AddCommand(ICommand cmd)
		{
			if (Commands.Contains(cmd))
				return;
			Commands.Add(cmd);
			Logger.Info("Registered a command " + cmd.GetType().Name);
		}

		/// <summary>
		/// プラグインを追加します。
		/// </summary>
		public void AddPlugin(PluginBase plugin)
		{
			if (Plugins.Contains(plugin))
				return;
			Plugins.Add(plugin);
			Logger.Info("Registered a plugin " + plugin.GetType().Name);
		}

		public ICommand? TryGetCommand(string n) => Commands.FirstOrDefault(c =>
		{
			var cn = c.Name;
			var cnl = c.Name.ToLowerInvariant();
			var nl = n.ToLowerInvariant();
			var nameIsMatch = c.IgnoreCase ? (cnl == nl) : cn == n;
			var lowerAliases = c.Aliases.Select(a => a.ToLowerInvariant());
			return c.Aliases == null ? nameIsMatch : nameIsMatch || (c.IgnoreCase ? lowerAliases.Contains(nl) : c.Aliases.Contains(n));
		});

		public async Task<string> ExecCommand(ICommandSender sender, string command)
		{
			if (command == null)
				throw new ArgumentNullException(nameof(command));
			if (command.StartsWith("/"))
				command = command[1..].Trim();
			var splitted = Regex.Split(command, @"\s").Where(s => !string.IsNullOrWhiteSpace(s));
			var name = splitted.First();
			var cmd = TryGetCommand(name) ?? throw new NoSuchCommandException();

			if (cmd.Permission.HasFlag(PermissionFlag.AdminOnly) && !sender.IsAdmin)
				throw new AdminOnlyException();

			if (sender is PostCommandSender p)
			{
				if (cmd.Permission.HasFlag(PermissionFlag.LocalOnly) && !string.IsNullOrEmpty(p.User.Host))
					throw new LocalOnlyException();

				if (cmd.Permission.HasFlag(PermissionFlag.RemoteOnly) && string.IsNullOrEmpty(p.User.Host))
					throw new RemoteOnlyException();
			}

			try
			{
				return await cmd.OnActivatedAsync(sender, this, Shell, splitted.Skip(1).ToArray(), command[name.Length..].Trim());
			}
			catch (CommandException)
			{
				return cmd.Usage;
			}
		}

		/// <summary>
		/// コマンドを実行します。
		/// </summary>
		/// <param name="command"></param>
		/// <returns></returns>
		public Task<string> ExecCommand(string command)
		{
			return ExecCommand(InternalCommandSender.Instance, command);
		}

		/// <summary>
		/// Admin 権限としてコマンドを実行します。
		/// </summary>
		public Task<string> SudoCommand(string command)
		{
			return ExecCommand(SuperInternalCommandSender.Instance, command);
		}

		/// <summary>
		/// 指定したユーザーがローカルユーザーであるかどうかを取得します。
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		public bool IsLocal(IUser user) => string.IsNullOrEmpty(user.Host);

		/// <summary>
		/// 指定したユーザーが管理者またはモデレーターであるかどうかを取得します。
		/// </summary>mi
		/// <returns>管理者かモデレーターであれば <c>true</c>、そうでなければ<c>false</c>。</returns>
		/// <param name="user">ユーザー。</param>
		public bool IsSuperUser(IUser user) => IsAdministrator(user) || IsModerator(user);

		/// <summary>
		/// 指定したユーザーが管理者であるかどうかを取得します。
		/// </summary>mi
		/// <returns>管理者であれば <c>true</c>、そうでなければ<c>false</c>。</returns>
		/// <param name="user">ユーザー。</param>
		public bool IsAdministrator(IUser user) => IsLocal(user) &&
												   string.Equals(Config.Instance.Admin, user.Name, StringComparison.OrdinalIgnoreCase) ||
												   string.Equals(Config.Instance.Admin, $@"{user.Name}@{user.Host}", StringComparison.OrdinalIgnoreCase);

		/// <summary>
		/// 指定したユーザーがモデレーターであるかどうかを取得します。
		/// </summary>mi
		/// <returns>モデレーターであれば <c>true</c>、そうでなければ<c>false</c>。</returns>
		/// <param name="user">ユーザー。</param>
		public bool IsModerator(IUser user) => Config.Instance.Moderators?.Any(u =>
												   IsLocal(user) && string.Equals(u, user.Name, StringComparison.OrdinalIgnoreCase) ||
												   string.Equals(u, $@"{user.Name}@{user.Host}", StringComparison.OrdinalIgnoreCase)) ?? false;

		/// <summary>
		/// 指定したユーザーの好感度を取得します。
		/// </summary>
		public Rating GetRatingOf(IUser user) => GetRatingOf(user.Id);

		public Rating GetRatingOf(string user)
		{
			var r = GetRatingValueOf(user);
			return r < -3 ? Rating.Hate :
				   r < 4 ? Rating.Normal :
				   r < 8 ? Rating.Like :
				   r < 20 ? Rating.BestFriend : Rating.Partner;
		}

		/// <summary>
		/// 指定したユーザーの好感度を取得します。
		/// </summary>
		public int GetRatingValueOf(IUser user) => Storage[user].Get(StorageKey.Rating, 0);

		/// <summary>
		/// 指定したユーザーの好感度を取得します。
		/// </summary>
		public int GetRatingValueOf(string id) => Storage[id].Get(StorageKey.Rating, 0);

		/// <summary>
		/// ユーザーに対する好感度を上げます。
		/// </summary>
		public void Like(string userId, int amount = 1)
		{
			SetRatingValueOf(userId, GetRatingValueOf(userId) + amount);
		}

		/// <summary>
		/// 指定したユーザーの好感度を設定します。
		/// </summary>
		public void SetRatingValueOf(string userId, int value)
		{
			Storage[userId].Set(StorageKey.Rating, value);
		}

		/// <summary>
		/// 指定したユーザーの好感度を設定します。
		/// </summary>
		public void SetRatingValueOf(IUser user, int value)
		{
			Storage[user.Id].Set(StorageKey.Rating, value);
		}

		/// <summary>
		/// ユーザーに対する好感度を下げます。
		/// </summary>
		public void Dislike(string userId, int amount = 1) => Like(userId, -amount);

		/// <summary>
		/// ユーザーのニックネームを取得します。
		/// </summary>
		public string GetNicknameOf(IUser user) => Storage[user].Get(StorageKey.Nickname, $"{user.Name}さん");

		/// <summary>
		/// ユーザーのニックネームを設定します。
		/// </summary>
		public void SetNicknameOf(IUser user, string name)
		{
			Storage[user].Set(StorageKey.Nickname, name);
		}

		/// <summary>
		/// ユーザーのニックネームを破棄します。
		/// </summary>
		public void ResetNicknameOf(IUser user)
		{
			Storage[user].Clear(StorageKey.Nickname);
		}

		public async Task HandleMentionAsync(IPost mention)
		{
			try
			{
				if (mention.User.IsBot)
					return;
				await Task.Delay(1000);

				if (mention.Reply is IPost reply && ContextPostDictionary.ContainsKey(reply.Id))
				{
					var (mod, arg) = ContextPostDictionary[mention.Reply.Id];
					ContextPostDictionary.Remove(mention.Reply.Id);
					if (await mod.OnRepliedContextually(mention, mention.Reply, arg, Shell, this))
						return;
				}

				// 非同期実行中にモジュール追加されると例外が発生するので毎回リストをクローン
				foreach (var mod in Modules.ToList())
				{
					// module が true を返したら終わり
					if (await mod.ActivateAsync(mention, Shell, this))
						break;
				}
			}
			catch (Exception ex)
			{
				WriteException(ex);
				await Shell.ReplyAsync(mention, $"(エラーが発生したようです。@{Config.Instance.Admin} はコンソールを確認して下さい。)");
			}
		}

		public async Task HandleTimelineAsync(IPost post)
		{
			if (post.User.IsBot)
				return;
			await Task.Delay(1000);

			// 非同期実行中にモジュール追加されると例外が発生するので毎回リストをクローン
			foreach (var mod in Modules.ToList())
			{
				try
				{
					// module が true を返したら終わり
					if (await mod.OnTimelineAsync(post, Shell, this))
						break;
				}
				catch (Exception ex)
				{
					WriteException(ex);
				}
			}
		}

		public async Task HandleDmAsync(IPost post)
		{
			try
			{
				if (post.User.IsBot)
					return;
				await Task.Delay(250);

				if (ContextUserDictionary.ContainsKey(post.User.Id))
				{
					var (mod, arg) = ContextUserDictionary[post.User.Id];
					ContextUserDictionary.Remove(post.User.Id);
					if (await mod.OnRepliedContextually(post, null, arg, Shell, this))
						return;
                }

				// 非同期実行中にモジュール追加されると例外が発生するので毎回リストをクローン
				foreach (var mod in Modules.ToList())
				{
					// module が true を返したら終わり
					if (await mod.OnDmReceivedAsync(post, Shell, this))
						break;
				}
			}
			catch (Exception ex)
			{
				await Shell.ReplyAsync(post, $"(エラーが発生したようです。@{Config.Instance.Admin} はコンソールを確認して下さい。)");
				WriteException(ex);
			}
		}

		public async Task HandleFollowedAsync(IUser user)
		{
			await Task.Delay(400);

			// 非同期実行中にモジュール追加されると例外が発生するので毎回リストをクローン
			foreach (var mod in Modules.ToList())
			{
				try
				{
					// module が true を返したら終わり
					if (await mod.OnFollowedAsync(user, Shell, this))
						break;
				}
				catch (Exception ex)
				{
					WriteException(ex);
				}
			}
		}

		public static void OpenUrl(string url)
		{
			// from https://brockallen.com/2016/09/24/process-start-for-urls-on-net-core/
			// hack because of this: https://github.com/dotnet/corefx/issues/10361
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				url = url.Replace("&", "^&");
				Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
			}
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
			{
				Process.Start("xdg-open", url);
			}
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			{
				Process.Start("open", url);
			}
			else
			{
				throw new NotSupportedException("このプラットフォームはサポートされていません。");
			}
		}

		public void RegisterContext(IPost post, IModule mod, Dictionary<string, object>? args = null)
		{
			if (post is IDirectMessage dm)
			{
				ContextUserDictionary[dm.Recipient.Id] = (mod, args ?? new Dictionary<string, object>());
			}
			else
			{
				ContextPostDictionary[post.Id] = (mod, args ?? new Dictionary<string, object>());
			}
		}

		private void WriteException(Exception ex)
		{
			Logger.Error($"{ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
		}

		public static readonly HttpClient Http = new();
	}
}
