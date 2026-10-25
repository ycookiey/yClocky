# yClocky

[English README](README.md)

Windowsのデスクトップに置くミニマルな時計。枠のないウィンドウに、時刻を文字だけで表示する。タスクバーのボタン・Alt+Tabの一覧の項目・通知領域のアイコンを出さず、時計をクリックしても背面のウィンドウがクリックされるため、作業の邪魔にならない。

## 特徴

- 時刻を`HH:mm:ss`の形で、日付を`yyyy/MM/dd (ddd)`の形で表示する。日付は非表示にできる
- Ctrlを押していない間は、時計をクリックしても背面のウィンドウがクリックされる
- ゴーストモード: カーソルが近づくと時計が薄くなり、カーソルが時計の上にある間は見えなくなる
- 画面キャプチャから除外できる。除外すると、スクリーンショット・録画・画面共有に時計が映らない
- フォント・文字色・背景色・不透明度を変更できる
- サインイン時に自動で起動できる

## インストール

対応環境はWindows 10と11のx64版。Releasesの`yClocky.exe`は.NETのランタイムを含むため、.NETのインストールは不要。

[Scoop](https://scoop.sh/)でインストールする場合:

```powershell
scoop bucket add yscoopy https://github.com/ycookiey/yscoopy
scoop install yscoopy/yclocky
```

WinGetでインストールする場合:

```powershell
winget install ycookiey.yClocky
```

または、[Releases](https://github.com/ycookiey/yClocky/releases)から`yClocky.exe`をダウンロードして起動する。

## 使い方

時計の操作にはCtrlを使う。

- 移動: Ctrlを押しながら時計をドラッグする。位置は再起動の後も保持される
- 設定: Ctrlを押しながら時計をクリックすると、設定のウィンドウが開く。変更はすぐに反映される
- 終了: 設定のウィンドウで「Exit Application」をクリックする
- Ctrlを押している間はゴーストモードが停止し、時計が見えたままになる

色は`#AARRGGBB`の形の16進数で指定する。例えば`#FFFFFFFF`は不透明な白。背景色の既定値の`#00000000`は完全な透明。設定の保存先は`%APPDATA%\yClocky\settings.json`。

起動時に、保存された位置が画面の外にあると、時計を既定の位置に戻す。既定の位置はメインの画面の左上の隅の近く。手動で既定の位置に戻すには、yClockyを終了してから、`--reset-position`か`-rp`を付けて起動する。

```powershell
yClocky --reset-position
```

## 既知の制限

- Ctrlは単独で押したときだけ効く。ShiftかAltも押している間は、クリックが背面のウィンドウに届く
- 「Exclude from Capture」が効くのはWindows 10のバージョン2004以降。2004より前のバージョンでは、画面キャプチャの中に時計が中身のない状態で映る
- `yClocky.exe`は.NETのランタイムを含むため、ファイルの大きさが約160MBある

## アンインストール

1. Ctrlを押しながら時計をクリックし、設定のウィンドウで「Run on Startup」のチェックを外す
2. 「Exit Application」をクリックして終了する
3. `scoop uninstall yclocky`か`winget uninstall ycookiey.yClocky`を実行する。Releasesからダウンロードした場合は、`yClocky.exe`を削除する
4. `%APPDATA%\yClocky`と`%TEMP%\.net\yClocky`を削除する。`%APPDATA%\yClocky`には設定とクラッシュのログが、`%TEMP%\.net\yClocky`には`yClocky.exe`が起動時に展開するネイティブのライブラリが入っている

## ビルド

.NET 8のSDKが必要。

```powershell
dotnet publish yClocky/yClocky.csproj -c Release
```

出力先は`yClocky\bin\Release\net8.0-windows\win-x64\publish\yClocky.exe`。

## ライセンス

[MIT](LICENSE)
