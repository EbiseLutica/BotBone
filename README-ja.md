# BotBone

[English](README.md) ・ 日本語

BotBone は、多くのソーシャルメディアに対応した bot を作成できるフレームワークです。

特定のソーシャルメディアに依存しない、独立した API を持ち、API に基づいたコードを書くだけで多くのソーシャルメディアで動作する bot を作れるようになっています。

## モジュール

モジュールは、BotBone の脳にあたります。リプライ, リアクションなどを行う為には、モジュールを bot の機能として作成し、読み込ませます。

## モジュールを自作する方法

BotBone に対応するモジュールを自作するためには、 [これを読んでください(工事中)](/docs/ja/module)

## プラットフォームアダプター

BotBone API は 各ソーシャルメディアの持つ API を抽象化したものです。

プラットフォームアダプターは BotBone API の実装であり、特定のプラットフォーム上で BotBone を動作させる為のものです。 hubot のアダプターのような役割を持ちます。

[✔] は実装済み, [ ] は計画中のもの。

- [x] Misskey
- [x] Mastodon
- [x] Standalone
	- 対話型シェル
- [x] Discord
- [x] [rinsuki/sea](https://github.com/rinsuki/sea)
- [ ] Slack
- [ ] LINE
- [ ] Twitter


### プラットフォームアダプターを自作する方法

BotBone に対応するプラットフォームアダプターを自作するためには、 [これを読んでください(工事中)](/docs/ja/adapter)

## 必要なソフトウェア

- .NET Core 3.0

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


## 貢献

- [バグ報告 / 要望](//github.com/xeltica/BotBone/issues/new)
- [プルリクエスト](//github.com/xeltica/BotBone/compare)

### コントリビューター

[その他...](//github.com/Xeltica/BotBone/graphs/contributors)

## ライセンス

[MIT License](LICENSE)
