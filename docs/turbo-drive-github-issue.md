# Turbo Drive のサーバーサイド対応

## 概要

`docs/hotwire-investigation-report.md` の調査結果に基づき、Turbo Drive のサーバーサイド対応を実装します。

## 背景

現在の実装状況：
- ✅ Turbo Frames: 実装済み
- ✅ Turbo Streams: 実装済み  
- ⚠️ **Turbo Drive: 未実装**（JavaScript のみに依存）

Turbo Drive は、リンクとフォーム送信を AJAX 化し、ページ全体の再読み込みを防ぐことで、SPA ライクな UX を実現する機能です。

## 実装内容

### 1. リクエスト検出機能

```csharp
// TurboHttpRequestExtensions.cs に追加
public static bool IsTurboDriveRequest(this HttpRequest request)
public static bool IsTurboRequest(this HttpRequest request)
```

### 2. メタタグヘルパー

```csharp
[HtmlTargetElement("turbo-drive-meta")]
public class TurboDriveMetaTagHelper : TagHelper
```

使用例：
```html
<turbo-drive-meta enabled="true" transition="fade" />
```

### 3. 永続的要素用 Tag Helper

```csharp
[HtmlTargetElement("turbo-permanent")]
public class TurboPermanentTagHelper : TagHelper
```

使用例：
```html
<turbo-permanent id="video-player">
    <video src="/video.mp4" controls></video>
</turbo-permanent>
```

### 4. サンプルアプリケーション "WireDrive"

以下の機能を実証するサンプルアプリを作成：
- 基本的なページ遷移（リンククリック）
- フォーム送信後のリダイレクト
- 永続的な要素（ビデオプレーヤー等）
- プログレッシブエンハンスメント
- エラーハンドリング

### 5. テストの追加

```csharp
// Turbo.AspNetCore.Test/TurboDriveTests.cs
- IsTurboDriveRequest() のテスト
- IsTurboRequest() のテスト
- Tag Helper の出力検証
```

### 6. ドキュメント

- README.md に Turbo Drive のセクションを追加
- `docs/turbo-drive-guide.md` を作成
- サンプルアプリの README を作成

## 実装の優先順位

### フェーズ 1: 基本機能（1-2週間）
- [ ] リクエスト検出機能の実装
- [ ] メタタグヘルパーの実装
- [ ] 基本的なテストの追加

### フェーズ 2: サンプルとドキュメント（2-3週間）
- [ ] サンプルアプリ "WireDrive" の作成
- [ ] ドキュメントの整備
- [ ] テストの拡充

### フェーズ 3: 高度な機能（1-2週間、オプション）
- [ ] 永続的要素用 Tag Helper
- [ ] コントローラー拡張メソッド
- [ ] TurboDriveResult クラス

## 期待される効果

### ユーザー体験
- ⚡ ページ遷移の高速化（SPA ライクな UX）
- 📉 ネットワーク帯域幅の削減
- ♿ プログレッシブエンハンスメント対応

### 開発者体験
- 🎯 Turbo Drive を ASP.NET Core で簡単に利用可能
- 📚 明確なドキュメントとサンプル
- 🔄 Rails 開発者にとって馴染みやすい API

## 技術的な考慮事項

- **ターゲットフレームワーク**: netstandard2.0
- **Turbo.js バージョン**: 8.0+ 推奨
- **互換性**: .NET Framework 4.7.2+, .NET Core 2.0+, .NET 5+

## 参考資料

- 詳細な実装計画: `docs/turbo-drive-implementation-plan.md`
- 調査レポート: `docs/hotwire-investigation-report.md`
- [Turbo Handbook - Drive](https://turbo.hotwired.dev/handbook/drive)
- [turbo-rails](https://github.com/hotwired/turbo-rails)

## 推定工数

**合計**: 4-6週間  
**優先度**: 中〜高  
**実装難易度**: 中
