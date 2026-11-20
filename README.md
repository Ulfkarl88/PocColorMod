# POC Color Mod (updated)

A Mount & Blade II: Bannerlord mod that changes the clothing colors, banners and shields of kingdoms, clans and units through a JSON config. It supports banner and shield pools, separate combat pools, and per-unit, per-tier and per-culture rules.

## Credits

- **[purpulez](https://www.nexusmods.com/profile/purpulez?gameId=3174)**: original author of POC Color Mod ([original mod page](https://www.nexusmods.com/mountandblade2bannerlord/mods/792)). All credit for the original design and code goes to them.
- **[Ulfkarl](https://www.nexusmods.com/profile/Ulfkarl?gameId=3174)**: updated the mod for current Bannerlord versions and maintains it, with permission from purpulez.

Download the updated mod on Nexus: https://www.nexusmods.com/mountandblade2bannerlord/mods/9778

Part of this source was recovered by decompiling the original DLL, so some files still have decompiler comments (`// Token: 0x...`).

## Building

Requirements:

- Visual Studio 2022 (or MSBuild) with .NET Framework 4.8.1
- Bannerlord installed through Steam, with these modules: Harmony, ButterLib, MCM

The project references the game DLLs with relative paths that assume the default Steam location (`C:\Program Files (x86)\Steam\steamapps\common\Mount & Blade II Bannerlord`) and the repo inside `C:\Users\<you>\source\repos\`. If your setup is different, update the `HintPath` entries in `PocColorMod.csproj`.

```
MSBuild PocColorMod.csproj -p:Configuration=Release
```

Copy the output `PocColor.dll` to `Modules\PocColor\bin\Win64_Shipping_Client\`. The DLL must be named `PocColor.dll`, because Raise Your Banner looks for it by that name for its compatibility mode.

## License

MIT. See [LICENSE](LICENSE).
