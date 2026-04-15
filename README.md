# ArcSysCostumeStringReplacer

A CLI tool for replacing costume and color string references in `.uasset` files from UE4-based ArcSys fighting games.

Supports processing a **folder of `.uasset` files** or a **`.pak` archive** directly.

## Requirements

- [![Download .NET 6.0](https://img.shields.io/badge/.NET_6.0-Download-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)

## Usage

### Input Modes

The tool accepts two types of input:

- **Folder** — Drag a folder containing `.uasset` files onto one of the included batch files. All `.uasset` files in the folder (and subfolders) will be processed in-place.
- **PAK file** — Drag a `.pak` archive onto one of the included batch files. The tool will extract the archive, process the `.uasset` files inside, rename directories to match the new costume/color IDs, and repack everything into a new `*_CostumeXX.pak` file alongside the original.

### Batch Files

The easiest way to use this tool is by dragging your folder or `.pak` file onto one of the included batch files:

| Batch File | Description |
|---|---|
| `Costume0X to Costume0Y.bat` | Prompts for a source and target costume ID, then replaces all `/CostumeXX` path strings. Skeleton path strings are left unchanged. |
| `Costume0X to Costume0Y (Also replace Skeleton strings).bat` | Same as above, but also replaces skeleton path strings (e.g. paths containing `Skeleton` or `skeleton`).  (This is needed for most characters with "external" props such as Bedman? and Vikala). |
| `ColorXX to ColorYY.bat` | Prompts for a source and target color ID, then replaces all `/ColorXX` path strings instead of costume strings. |

Each batch file will prompt you to enter the ID you want to replace **from** and the ID you want to replace **to**.

### Command Line

```
ArcSysCostumeStringReplacer <folder-or-pak> [targetCostumeID] [replaceSkeletonStrings] [costumeIDToReplace] [isReplaceColorMode]
```

| Argument | Default | Description |
|---|---|---|
| `folder-or-pak` | *(required)* | Path to a folder of `.uasset` files or a `.pak` archive |
| `targetCostumeID` | `2` | Costume/color ID to replace **to** |
| `replaceSkeletonStrings` | `false` | Also replace skeleton path strings |
| `costumeIDToReplace` | `1` | Costume/color ID to replace **from** |
| `isReplaceColorMode` | `false` | Replace `/ColorXX` strings instead of `/CostumeXX` |

> **Note:** The UE4 engine version is hardcoded to `VER_UE4_25` in by default, this is the engine version for both Guilty Gear Strive and DNF Duel. Change this in the `Open()` function before compiling if your target game uses a different engine version (such as changing it to 4.17 for DBFZ).

## Dependencies

- [UAssetAPI](https://github.com/atenfyr/UAssetAPI)
- [Amicitia.IO](https://github.com/tge-was-taken/Amicitia.IO)
- [Unpaker](https://www.nuget.org/packages/Unpaker) — PAK archive extraction and repacking
- [Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json) — JSON support (UAssetAPI dependency)

## License

This project is licensed under the [GNU General Public License v3.0](LICENSE).
