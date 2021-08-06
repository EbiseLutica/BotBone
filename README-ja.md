# BotBone

[English](README.md) ・ 日本語

BotBone は、多くのソーシャルメディアに対応したチャットボットのベアボーンです。

特定のソーシャルメディアに依存しないよう設計された BotBone API を提供しているため、プラグインを作成し所定の場所に配置するだけで、あなただけの bot を対応する全てのプラットフォームで動かせます。

## ドキュメント

整備中です。

## 必要なソフトウェア

- .NET 5

## ビルド方法

```shell
git clone --recursive https://github.com/Xeltica/BotBone

cd BotBone

# --recursive を忘れた場合
git submodule update --init

dotnet build

# BotBone for Misskey を実行
cd BotBone.Misskey && dotnet run

# BotBone for Mastodon を実行
cd BotBone.Mastodon && dotnet run

# BotBone for Discord を実行
cd BotBone.Discord && dotnet run

# BotBone Interactive を実行
cd BotBone.Standalone && dotnet run
```

### コントリビューター

[その他...](//github.com/Xeltica/BotBone/graphs/contributors)

## ライセンス

[MIT License](LICENSE)
