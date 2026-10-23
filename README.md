# yClocky

[日本語](README.ja.md)

A minimal desktop clock for Windows. It shows the time as plain text with no window frame, and stays out of your way: it has no taskbar button, no Alt+Tab entry and no notification area icon, and clicks pass through it to the windows behind.

## Features

- Shows the time (`HH:mm:ss`) and the date (`yyyy/MM/dd (ddd)`). The date can be hidden
- Clicks pass through the clock unless you hold Ctrl
- Ghost mode: the clock fades out as the cursor approaches it, and disappears while the cursor is over it
- Can be excluded from screen capture, so it does not show up in screenshots, recordings or screen sharing
- Font, text color, background color and opacity can be changed
- Can start when you sign in

## Install

Requires Windows 10 or 11 (x64). The release `yClocky.exe` includes the .NET runtime, so .NET does not need to be installed.

With [Scoop](https://scoop.sh/):

```powershell
scoop bucket add yscoopy https://github.com/ycookiey/yscoopy
scoop install yscoopy/yclocky
```

With WinGet:

```powershell
winget install ycookiey.yClocky
```

Or download `yClocky.exe` from [Releases](https://github.com/ycookiey/yClocky/releases) and run it.

## Usage

The clock is controlled with Ctrl.

- Move: hold Ctrl and drag the clock. The position is kept across restarts
- Settings: hold Ctrl and click the clock to open the settings window. Changes apply immediately
- Quit: click **Exit Application** in the settings window
- While Ctrl is held, ghost mode is paused and the clock stays visible

Colors are hex values in the form `#AARRGGBB`; for example, `#FFFFFFFF` is opaque white. The default background `#00000000` is fully transparent. Settings are stored in `%APPDATA%\yClocky\settings.json`.

If the saved position is off screen at startup, the clock returns to the default position near the top-left corner of the main screen. To move it back there yourself, quit yClocky and start it with `--reset-position` or `-rp`:

```powershell
yClocky --reset-position
```

## Known limitations

- Ctrl works only on its own. While Shift or Alt is also held, clicks pass through the clock
- **Exclude from Capture** needs Windows 10 version 2004 or later. On earlier versions the clock still shows up in captures, but without its content
- The executable is about 160 MB, because it includes the .NET runtime

## Uninstall

1. Hold Ctrl and click the clock, and clear **Run on Startup** in the settings window
2. Click **Exit Application**
3. Run `scoop uninstall yclocky` or `winget uninstall ycookiey.yClocky`, or delete `yClocky.exe`
4. Delete `%APPDATA%\yClocky` (settings and crash log) and `%TEMP%\.net\yClocky` (native libraries that `yClocky.exe` extracts when it starts)

## Build

Requires the .NET 8 SDK.

```powershell
dotnet publish yClocky/yClocky.csproj -c Release
```

The executable is written to `yClocky\bin\Release\net8.0-windows\win-x64\publish\yClocky.exe`.

## License

[MIT](LICENSE)
