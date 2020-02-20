using System.Collections.Generic;
using System.Linq;
using BotBone.Core.Api;
using Disboard.Misskey.Models;

namespace BotBone.Misskey
{
	public class MiPoll : IPoll
	{
		public Poll Native { get; }
		public MiPoll(Poll p)
		{
			Native = p;
			// 一括変換
			Choices = from c in p.Choices select new MiChoice(c);
		}
		public IEnumerable<IChoice> Choices { get; }
	}
}
