# BotBone

English ・ [日本語](README-ja.md)

BotBone is a open-source chatbot barebone for many kinds of social media.

It has a BotBone API that independent of a specific social-media platform, so you can run your original bot in all supported platform by writing and deploying your plugin.


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

### Contributors

[More...](//github.com/Xeltica/BotBone/graphs/contributors)

## License

[MIT License](LICENSE)
