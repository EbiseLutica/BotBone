using System;
using BotBone.Core.Api;

namespace BotBone.Misskey
{
	public static class VisiblityExtension
	{
		public static Visibility ToVisiblity(this string visiblity)
		{
			return visiblity switch
			{
				"public" => Visibility.Public,
				"specified" => Visibility.Direct,
				"home" => Visibility.Limited,
				"followers" => Visibility.Private,
				_ => throw new ArgumentOutOfRangeException(),
			};
		}
		public static string? ToStr(this Visibility visiblity)
		{
			return visiblity switch
			{
				Visibility.Default => null,
				Visibility.Public => "public",
				Visibility.Limited => "home",
				Visibility.Private => "followers",
				Visibility.Direct => "specified",
				_ => throw new ArgumentOutOfRangeException(nameof(visiblity)),
			};
		}
	}

}
