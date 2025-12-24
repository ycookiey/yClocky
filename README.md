# yClocky

yClockyは、Windows用のミニマルでステルス性の高いデスクトップ時計アプリケーションです。
作業の邪魔にならないように設計されており、必要な時だけ確認できる控えめな存在感を目指しています。

## 特徴

*   **ミニマルデザイン**: ウィンドウ枠やタイトルバーがなく、文字だけが浮かんでいるようなデザインです。
*   **ステルスモード**: タスクバーやAlt+Tabの切り替え画面に表示されません。
*   **ゴーストモード**: マウスカーソルが近づくと自動的に透明になり、背面の操作を妨げません。
*   **キャプチャ保護**: スクリーンショットや画面共有に映り込まないように設定できます。
*   **カスタマイズ**: フォント、文字色、背景色、透明度などを自由に変更できます。
*   **スタートアップ**: Windows起動時に自動的に実行するように設定可能です。

## 操作方法

誤操作を防ぐため、主な操作には `Ctrl` キーを使用します。

*   **移動**: `Ctrl` キーを押しながらドラッグ
*   **設定画面を開く**: `Ctrl` キーを押しながらクリック
*   **ゴーストモード一時無効化**: `Ctrl` キーを押している間は、ゴーストモードが有効でも透明になりません。

## 動作環境

*   Windows 10 / 11
*   .NET 8 Runtime

## インストール・ビルド

このプロジェクトは .NET 8 (WPF) で開発されています。

```powershell
# ビルド
dotnet build

# 実行
dotnet run
```

## 設定項目

設定画面から以下の項目を変更できます。

*   **Appearance (外観)**
    *   Font Family: フォントの変更
    *   Text Color: 文字色 (Hex)
    *   Background Color: 背景色 (Hex)
    *   Opacity: 基本の透明度
*   **Behavior (動作)**
    *   Show Date: 日付表示のON/OFF
    *   Always on Top: 常に最前面に表示
    *   Run on Startup: スタートアップに登録
    *   Ghost Mode: マウス接近時の自動フェードアウト
    *   Exclude from Capture: 画面キャプチャからの除外
    *   Allow Multiple Instances: 多重起動の許可

## リリース手順 (開発者向け)

このプロジェクトは `gh bump` 拡張機能を使用してリリースを作成します。

### 前提条件

```powershell
# gh bump拡張機能のインストール
gh extension install johnmanjiro13/gh-bump
```

### リリース作成手順

1. **ビルドとパブリッシュ**
   ```powershell
   cd yClocky
   dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:PublishTrimmed=false
   ```

2. **変更をコミット**
   ```powershell
   git add .
   git commit -m "リリース準備"
   git push
   ```

3. **リリース作成**
   ```powershell
   # パッチバージョン (1.0.0 -> 1.0.1)
   gh bump --bump-type patch --asset-files "yClocky/bin/Release/net8.0-windows/win-x64/publish/yClocky.exe" -g -y

   # マイナーバージョン (1.0.0 -> 1.1.0)
   gh bump --bump-type minor --asset-files "yClocky/bin/Release/net8.0-windows/win-x64/publish/yClocky.exe" -g -y

   # メジャーバージョン (1.0.0 -> 2.0.0)
   gh bump --bump-type major --asset-files "yClocky/bin/Release/net8.0-windows/win-x64/publish/yClocky.exe" -g -y
   ```

### WinGetパッケージ更新

リリース作成後、WinGetパッケージも更新する必要があります：

```powershell
# wingetcreateを使用してマニフェストを更新
wingetcreate update ycookiey.yClocky --version <新しいバージョン> --urls <リリースURL> --submit
```