using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.IO;
using Newtonsoft.Json.Linq;

namespace BotBone.Core.Api
{
	public class UserStorage
	{
		public ReadOnlyDictionary<string, UserRecord> Records => new ReadOnlyDictionary<string, UserRecord>(storage);

		public UserStorage()
		{
			Reload();
		}

		/// <summary>
		/// 指定したユーザーのレコードを取得します。
		/// </summary>
		/// <value>指定したユーザーのレコード。存在しなければ新規作成したものを返します。</value>
		public UserRecord this[IUser user] => this[user.Id];

		public UserRecord this[string userId]
		{
			get
			{
				lock (storage)
				{
					return storage.ContainsKey(userId) ? storage[userId] : (storage[userId] = CreateRecord());
				}
			}
		}

		/// <summary>
		/// ストレージをファイルへ手動保存します。
		/// </summary>
		public void Save()
		{
			lock (fileLock)
				File.WriteAllText("./storage.json", SerializeStorage());
			logger.Debug("Saved storage data!");
		}

		/// <summary>
		/// ストレージをファイルからリロードします。
		/// </summary>
		public void Reload()
		{
			if (File.Exists("./storage.json"))
				DeserializeStorage(File.ReadAllText("./storage.json"));
			logger.Debug("Reloaded storage data!");
		}

		/// <summary>
		/// UserStorage の引っ越しを行います。
		/// </summary>
		public void Migrate(string userIdFrom, string userIdTo)
		{
			storage[userIdTo] = new UserRecord(this[userIdFrom].InternalRecord.DeepClone() as JObject);
			storage[userIdFrom].ClearAll();
			Save();
		}

		private string SerializeStorage()
		{
			lock (storage)
			{
				var obj = new JObject();
				foreach (var kv in storage)
				{
					obj[kv.Key] = kv.Value.InternalRecord;
				}
				return obj.ToString();
			}
		}

		private void DeserializeStorage(string json)
		{
			var obj = JObject.Parse(json);
			lock (storage)
			{
				foreach (var kv in obj)
				{
					storage[kv.Key] = CreateRecord(kv.Value as JObject);
					logger.Debug($"Set {kv.Key}");
				}
			}
		}

		private UserRecord CreateRecord(JObject? initialRecord = null)
		{
			if (Config.Instance.LoggingLevel <= LoggingLevel.Debug)
			{
				if (initialRecord == null)
				{
					logger.Debug($"{nameof(initialRecord)} is null");
				}
				else
				{
					foreach (var kv in initialRecord)
						logger.Debug($"{kv.Key}= {kv.Value ?? "null"}");
				}
			}
			var rec = initialRecord == null ? new UserRecord() : new UserRecord(initialRecord);
			rec.Updated += () => Save();
			return rec;
		}

		private readonly ConcurrentDictionary<string, UserRecord> storage = new ConcurrentDictionary<string, UserRecord>();

		private readonly object fileLock = new object();

		protected static Logger logger = new Logger("UserStorage");

		public class UserRecord
		{
			public UserRecord() { }
			public UserRecord(JObject obj) => InternalRecord = obj;

			internal JObject InternalRecord { get; } = new JObject();

			public T Get<T>(string key, T defaultValue = default)
			{
				// キーに値がなければデフォルト値
				if (!Has(key))
					return defaultValue;

				// 正しい型の値があれば返す　なければデフォルト
				try
				{
					return InternalRecord[key].ToObject<T>();
				}
				catch (ArgumentException ex)
				{
					logger.Debug($"{ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
					return defaultValue;
				}
			}

			public bool Has(string key) => InternalRecord.ContainsKey(key);

			public bool Is<T>(string key) => Has(key);

			public void Set<T>(string key, T value)
			{
				InternalRecord[key] = JToken.FromObject(value);

				Updated?.Invoke();
			}

			public void Add(string key, int value = 1)
			{
				Set(key, Get(key, 0) + value);

				Updated?.Invoke();
			}

			public void Add(string key, float value)
			{
				Set(key, Get(key, 0f) + value);

				Updated?.Invoke();
			}

			public void Add(string key, double value)
			{
				Set(key, Get(key, 0d) + value);

				Updated?.Invoke();
			}

			public void Clear(string key)
			{
				if (!Has(key)) return;
				InternalRecord.Remove(key);
				Updated?.Invoke();
			}

			public void ClearAll()
			{
				InternalRecord.RemoveAll();
				Updated?.Invoke();
			}

			public event Action? Updated;
		}
	}
}
