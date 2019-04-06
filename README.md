# Steam Account Generator
Web generator: https://accgen.cathook.club/

![screenshot](https://i.imgur.com/e1kIgzl.png)

Mass Steam Account generator features:
- [x] Auto verify mail
- [x] Auto generate email
- [x] Auto generate username
- [x] Auto generate password
- [x] Can add free games to account
  - [x] Export/import this list
  - Note: [How to find sub ID](https://github.com/EarsKilla/Steam-Account-Generator/wiki/Find-sub-ID)
- [x] Resolve Steam ID
- [x] Auto save to text file
  - Can be saved in CSV format and imported in KeePass
- [x] Manual and auto captcha services:
  - Captchasolutions
  - 2Captcha/RuCaptcha
  - Warning: Latest build can cause issues with auto captha services.
  Open issue if you are expecting problems.
- [x] Proxy support
  Open issue if you are expecting problems.
- [x] Save most settings in JSON file.

# Download
[Steam-Account-Generator/releases](https://github.com/EarsKilla/Steam-Account-Generator/releases)

# Communication
[![Join to our Discord](https://discordapp.com/api/guilds/557374041409716224/widget.png?style=banner2)](https://discord.gg/R96F2DA)

# Supported OS:
### Windows
|Requirement|Download|Tested version|
|---|---|---|
|.NET Framework|[4.7.2 download](https://dotnet.microsoft.com/download/dotnet-framework-runtime/net472)|`4.7.2`|
---
### Linux
|Requirement|Download|Tested version|
|---|---|---|
|Mono|[stable](https://www.mono-project.com/download/stable/)|`5.18.1.0`|
|xcb|See below|`2.4-4.3`|
##### **xcb** installation in Debian/Ubuntu:
```bash
# apt update
# apt install xcb
```
#### **Note:** Tested on Ubuntu 18.10
