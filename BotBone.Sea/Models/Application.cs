#pragma warning disable CS8618 // API POCO クラスは除外
using Newtonsoft.Json;

namespace BotBone.Sea
{
    public class Application
	{
		[JsonProperty("id")]
		public int Id { get; set; }

		[JsonProperty("isAutomated")]
		public bool IsAutomated { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }
	}
}
