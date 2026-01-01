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

## インストール

### Scoop

```powershell
scoop bucket add yscoopy https://github.com/ycookiey/yscoopy
scoop install yscoopy/yclocky
```

### WinGet

```powershell
winget install ycookiey.yClocky
```

### 手動インストール

[Releases](https://github.com/ycookiey/yClocky/releases) ページから最新版の `yClocky.exe` をダウンロードして実行してください。

## 開発者向けビルド

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

このプロジェクトは `gh bump` でバージョン管理を行い、GitHub Actions で自動的にビルド・リリースを作成します。

### 前提条件

```bash
# gh bump拡張機能のインストール
gh extension install johnmanjiro13/gh-bump
```

### リリース作成手順

1. **変更をコミット**
   ```bash
   git add .
   git commit -m "変更内容"
   git push
   ```

2. **gh bumpでタグを作成**
   ```bash
   # パッチバージョン (1.0.0 -> 1.0.1)
   gh bump --bump-type patch -g -y

   # マイナーバージョン (1.0.0 -> 1.1.0)
   gh bump --bump-type minor -g -y

   # メジャーバージョン (1.0.0 -> 2.0.0)
   gh bump --bump-type major -g -y
   ```

これだけで、GitHub Actions が自動的に以下を実行します：

- ✅ .NET 8 でビルド
- ✅ リリース版の実行ファイルを作成
- ✅ GitHub Release を作成
- ✅ WinGet パッケージの更新 PR を作成

### ワークフローの詳細

リリースプロセスは以下の2つのワークフローで構成されています：

- **`.github/workflows/release.yml`**: タグプッシュ時にビルド・リリース作成
- **`.github/workflows/winget.yml`**: リリース公開時に WinGet PR 作成

### 備考

- ローカルでのビルドは不要です
- `WINGET_TOKEN` シークレットが設定されている必要があります