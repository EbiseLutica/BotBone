# BotBone

English ・ [日本語](README-ja.md)

BotBone is a open-source framework to create chat-bot.

It has an API that independent of a specific social-media platform, so BotBone can run on a lots of social media services.

## Modules

Modules are BotBone's actual brain. To reply, react, repost etc, create modules as bot's features, and let BotBone load them.

## How to write your own module

To write your module for BotBone, [read this document(TBD)](/docs/module)

## Platform Adapters

BotBone API is an abstractive API of each social media.

Platform adapters are implemented BotBone API to run BotBone on the specified platform. It's same as hubot's adapter.

[✔] is implemented, and [ ] is in plan.

- [x] Misskey
- [x] Mastodon
- [x] Standalone
	- A REPL
- [x] Discord
- [x] [rinsuki/sea](https://github.com/rinsuki/sea)
- [ ] Slack
- [ ] LINE
- [ ] Twitter


### How to write your own platform adapter

To write your own platform adapter for BotBone, [read this doc(TBD)](/docs/adapter)

## Requirement

- .NET Core 3

## To build

```shell
git clone --recursive https://github.com/Xeltica/BotBone.git

cd BotBone

# If you forget cloning with --recursive
git submodule update --init

dotnet build

# Run BotBone for Misskey
cd BotBone.Misskey && dotnet run

# Run BotBone for Mastodon
cd BotBone.Mastodon && dotnet run

# Run BotBone for Discord
cd BotBone.Discord && dotnet run

# Run BotBone Interactive
cd BotBone.Standalone && dotnet run
```


## Contributing

- [Issues](//github.com/xeltica/BotBone/issues/new)
- [Pull Requests](//github.com/xeltica/BotBone/compare)

### Contributors

[More...](//github.com/Xeltica/BotBone/graphs/contributors)

## License

[MIT License](LICENSE)
