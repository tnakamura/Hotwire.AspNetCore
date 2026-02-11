# Turbo Drive 対応プラン

## 概要

`docs/hotwire-investigation-report.md` の調査結果に基づき、Turbo Drive のサーバーサイド対応を実装します。現在、Turbo Drive は JavaScript ライブラリ（Turbo.js）のみに依存しており、ASP.NET Core 側でのサポート機能が不足しています。

## 背景

調査レポートによると：
- ✅ Turbo Frames と Turbo Streams は実装済み
- ⚠️ **Turbo Drive は未実装**（JavaScript ライブラリのみに依存）
- Turbo Drive は、リンクとフォーム送信を AJAX 化し、ページ全体の再読み込みを防ぐ機能

## 目標

ASP.NET Core で Turbo Drive を効果的に使用するためのサーバーサイドサポートを提供する。

## 実装計画

### 1. Turbo Drive リクエストの検出 (優先度: 高)

#### 実装内容
`TurboHttpRequestExtensions.cs` に新しい拡張メソッドを追加：

```csharp
public static class TurboHttpRequestExtensions
{
    /// <summary>
    /// Turbo Drive によるリクエストかどうかを判定
    /// </summary>
    public static bool IsTurboDriveRequest(this HttpRequest request)
    {
        // Turbo Drive は "Turbo-Frame" ヘッダーが存在しない通常のリクエスト
        // かつ Accept ヘッダーに text/html が含まれる
        return !request.Headers.ContainsKey("Turbo-Frame") &&
               request.Headers.Accept.Any(a => a.Contains("text/html"));
    }
    
    /// <summary>
    /// Turbo によるリクエストかどうかを判定（Drive/Frame/Stream のいずれか）
    /// </summary>
    public static bool IsTurboRequest(this HttpRequest request)
    {
        return request.IsTurboDriveRequest() ||
               request.IsTurboFrameRequest() ||
               request.IsTurboStreamRequest();
    }
}
```

#### テスト
- `IsTurboDriveRequest()` の動作検証
- `IsTurboRequest()` の動作検証

### 2. Turbo Drive メタタグヘルパー (優先度: 高)

#### 実装内容
Turbo Drive の動作を制御するメタタグを簡単に設定できる Tag Helper を追加：

```csharp
/// <summary>
/// Turbo Drive の動作を制御するメタタグを生成
/// </summary>
[HtmlTargetElement("turbo-drive-meta")]
public class TurboDriveMetaTagHelper : TagHelper
{
    /// <summary>
    /// Turbo Drive を有効/無効にする (デフォルト: true)
    /// </summary>
    public bool Enabled { get; set; } = true;
    
    /// <summary>
    /// ページ遷移時のアニメーション (デフォルト: "")
    /// 指定可能な値: "fade", "slide", "none"
    /// </summary>
    public string Transition { get; set; } = "";
    
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = null; // タグ自体は出力しない
        output.TagMode = TagMode.StartTagAndEndTag;
        
        var sb = new StringBuilder();
        
        // Turbo Drive の有効/無効
        // "advance": Turbo Drive を有効化（デフォルト）
        // "reload": 完全なページリロードを強制
        sb.AppendLine($"<meta name=\"turbo-visit-control\" content=\"{(Enabled ? "advance" : "reload")}\">");
        
        // Turbo 8 の新機能: Page Refresh Method
        sb.AppendLine("<meta name=\"turbo-refresh-method\" content=\"morph\">");
        sb.AppendLine("<meta name=\"turbo-refresh-scroll\" content=\"preserve\">");
        
        if (!string.IsNullOrEmpty(Transition))
        {
            sb.AppendLine($"<meta name=\"turbo-transition\" content=\"{Transition}\">");
        }
        
        output.Content.SetHtmlContent(sb.ToString());
    }
}
```

#### 使用例
```html
<!-- _Layout.cshtml の <head> 内 -->
<turbo-drive-meta enabled="true" transition="fade" />
```

#### テスト
- メタタグの正しい生成を検証
- 各オプションの動作確認

### 3. レイアウト用 Tag Helper (優先度: 中)

#### 実装内容
Turbo Drive でページ遷移時に保持される要素を指定する Tag Helper：

```csharp
/// <summary>
/// Turbo Drive でページ遷移時に保持される永続的な要素を定義
/// </summary>
[HtmlTargetElement("turbo-permanent")]
public class TurboPermanentTagHelper : TagHelper
{
    /// <summary>
    /// 要素の一意な ID（必須）
    /// </summary>
    public string Id { get; set; }
    
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "div";
        output.Attributes.SetAttribute("id", Id);
        output.Attributes.SetAttribute("data-turbo-permanent", "");
    }
}
```

#### 使用例
```html
<!-- ページ遷移しても保持されるビデオプレーヤー -->
<turbo-permanent id="video-player">
    <video src="/video.mp4" controls></video>
</turbo-permanent>
```

### 4. プログレッシブエンハンスメント対応 (優先度: 中)

#### 実装内容
コントローラー拡張メソッドを追加して、Turbo Drive の有無に応じた適切なレスポンスを返す：

```csharp
public static class TurboControllerExtensions
{
    /// <summary>
    /// Turbo Drive 対応のビューを返す
    /// 注: Turbo Drive は自動的に <body> の内容のみを抽出して置換するため、
    /// サーバー側は通常のビュー（レイアウト付き）を返すだけで良い
    /// </summary>
    public static IActionResult TurboDriveView(
        this Controller controller,
        string viewName = null,
        object model = null)
    {
        // Turbo Drive でも通常のリクエストでも、同じビューを返す
        // クライアント側の Turbo.js が適切に処理する
        return controller.View(viewName, model);
    }
    
    /// <summary>
    /// Turbo Drive リクエストかどうかに応じて、異なる動作が必要な場合に使用
    /// （通常は不要。レガシーコードとの互換性のため）
    /// </summary>
    public static IActionResult ViewOrRedirect(
        this Controller controller,
        string viewName,
        string redirectUrl,
        object model = null)
    {
        if (controller.Request.IsTurboDriveRequest())
        {
            // Turbo Drive の場合は直接ビューをレンダリング
            return controller.View(viewName, model);
        }
        else
        {
            // 通常のリクエストの場合はリダイレクト
            return controller.Redirect(redirectUrl);
        }
    }
}
```

### 5. Turbo Drive 用の ViewResult (優先度: 低)

#### 実装内容
Turbo Drive 専用のレスポンス処理クラス：

```csharp
/// <summary>
/// Turbo Drive リクエストに最適化された ViewResult
/// </summary>
public class TurboDriveResult : ViewResult
{
    public override async Task ExecuteResultAsync(ActionContext context)
    {
        var httpContext = context.HttpContext;
        
        // Turbo-Visit-Action ヘッダーを設定（オプション）
        // "advance": 通常の遷移
        // "replace": 履歴を置換
        // "restore": ブラウザバックによる復元
        if (!httpContext.Response.Headers.ContainsKey("Turbo-Visit-Action"))
        {
            httpContext.Response.Headers.Add("Turbo-Visit-Action", "advance");
        }
        
        await base.ExecuteResultAsync(context);
    }
}
```

### 6. サンプルアプリケーション "WireDrive" (優先度: 高)

#### 実装内容
Turbo Drive の機能を実証する新しいサンプルアプリケーションを作成：

**機能**:
1. **基本的なページ遷移**: 複数のページ間をリンクで移動（ページ全体の再読み込みなし）
2. **フォーム送信**: POST リクエスト後のリダイレクトを Turbo Drive で処理
3. **永続的な要素**: ページ遷移しても保持される要素（ビデオプレーヤー、音楽プレーヤーなど）
4. **プログレッシブエンハンスメント**: JavaScript 無効時でも動作することを実証
5. **エラーハンドリング**: 404/500 エラー時の適切な処理

**プロジェクト構造**:
```
examples/WireDrive/
├── Controllers/
│   ├── HomeController.cs
│   ├── ProductsController.cs
│   └── OrdersController.cs
├── Views/
│   ├── Shared/
│   │   └── _Layout.cshtml (Turbo Drive メタタグを含む)
│   ├── Home/
│   │   ├── Index.cshtml
│   │   └── About.cshtml
│   ├── Products/
│   │   ├── Index.cshtml
│   │   └── Details.cshtml
│   └── Orders/
│       ├── New.cshtml
│       └── Confirmation.cshtml
└── wwwroot/
    └── js/
        └── site.js (Turbo.js の読み込み)
```

#### シナリオ例
1. **ホームページ** → 製品一覧 → 製品詳細（スムーズなページ遷移）
2. **注文フォーム** → POST → 確認ページ（フォーム送信も高速）
3. **ビデオプレーヤー**が全ページで再生を継続（data-turbo-permanent）

### 7. テストの追加 (優先度: 高)

#### テストケース
`Turbo.AspNetCore.Test/TurboDriveTests.cs` を作成：

```csharp
public class TurboDriveTests
{
    [Fact]
    public void IsTurboDriveRequest_WithoutTurboFrameHeader_ReturnsTrue()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Headers["Accept"] = "text/html";
        
        // Act
        var result = context.Request.IsTurboDriveRequest();
        
        // Assert
        Assert.True(result);
    }
    
    [Fact]
    public void IsTurboDriveRequest_WithTurboFrameHeader_ReturnsFalse()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Headers["Accept"] = "text/html";
        context.Request.Headers["Turbo-Frame"] = "gallery";
        
        // Act
        var result = context.Request.IsTurboDriveRequest();
        
        // Assert
        Assert.False(result);
    }
    
    [Fact]
    public void IsTurboRequest_WithAnyTurboRequest_ReturnsTrue()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Headers["Accept"] = "text/html";
        
        // Act
        var result = context.Request.IsTurboRequest();
        
        // Assert
        Assert.True(result);
    }
    
    [Fact]
    public void TurboDriveMetaTagHelper_GeneratesCorrectMetaTags()
    {
        // Tag Helper のテスト
        // (実装例は省略)
    }
}
```

### 8. ドキュメントの作成 (優先度: 中)

#### 作成するドキュメント

1. **README.md の更新**
   - Turbo Drive のセクションを追加
   - クイックスタートガイド
   - サンプルコード

2. **docs/turbo-drive-guide.md**
   - Turbo Drive の詳細な使い方
   - ベストプラクティス
   - よくある問題と解決策

3. **examples/WireDrive/README.md**
   - サンプルアプリの説明
   - 実行方法
   - 学習ポイント

## 実装の優先順位

### フェーズ 1: 基本機能（1-2週間）
1. ✅ リクエスト検出機能 (`IsTurboDriveRequest`, `IsTurboRequest`)
2. ✅ メタタグヘルパー (`TurboDriveMetaTagHelper`)
3. ✅ 基本的なテスト

### フェーズ 2: サンプルとドキュメント（2-3週間）
1. ✅ サンプルアプリ "WireDrive" の作成
2. ✅ ドキュメントの整備
3. ✅ テストの拡充

### フェーズ 3: 高度な機能（1-2週間、オプション）
1. ⚪ 永続的要素用 Tag Helper (`TurboPermanentTagHelper`)
2. ⚪ コントローラー拡張メソッド (`ViewOrPartial`)
3. ⚪ `TurboDriveResult` クラス

## 技術的な考慮事項

### 互換性
- **ターゲットフレームワーク**: netstandard2.0（既存ライブラリと同じ）
- **Turbo.js バージョン**: 8.0+ 推奨（morph、refresh 機能のサポート）
- **.NET バージョン**: .NET Framework 4.7.2+, .NET Core 2.0+, .NET 5+

### 設計原則
1. **最小限の変更**: 既存の API デザインを維持
2. **プログレッシブエンハンスメント**: JavaScript 無効時でも動作
3. **Rails との整合性**: turbo-rails の設計思想を踏襲
4. **ASP.NET Core らしさ**: Tag Helper、拡張メソッドなど ASP.NET Core の慣例に従う

### パフォーマンス
- メタタグ生成のオーバーヘッドは無視できる程度
- リクエスト検出はヘッダーチェックのみで高速
- サーバーサイドのレンダリングコストは従来と同じ

## 期待される効果

### ユーザー体験の向上
- ページ遷移が高速化（SPA ライクな UX）
- ネットワーク帯域幅の削減（`<head>` の重複送信を削減）
- プログレッシブエンハンスメント対応

### 開発者体験の向上
- Turbo Drive の機能を ASP.NET Core で簡単に利用可能
- Rails 開発者にとって馴染みやすい API
- 明確なドキュメントとサンプル

## 将来の拡張性

### Turbo 8 の新機能対応
- **Morph**: DOM の状態を保持しながら更新
- **Refresh**: ページ全体のリフレッシュ
- **Page Refresh Streams**: サーバーからのリフレッシュ指示

### SignalR との統合
- リアルタイムでの Turbo Drive リフレッシュ
- 複数クライアント間での同期

### Blazor との統合
- Turbo Drive と Blazor Server の共存
- ハイブリッドアプリケーションの構築

## 成功指標

1. ✅ すべてのテストがパス
2. ✅ サンプルアプリが正常に動作
3. ✅ ドキュメントが完備され、初心者でも理解できる
4. ✅ 既存の Turbo Frames/Streams 機能との統合が問題なく動作
5. ✅ パフォーマンス劣化がない

## リファレンス

- [Turbo Handbook - Drive](https://turbo.hotwired.dev/handbook/drive)
- [Turbo 8 Page Refreshes](https://jonsully.net/blog/turbo-8-page-refreshes-morphing-explained-at-length/)
- [turbo-rails ソースコード](https://github.com/hotwired/turbo-rails)
- [ASP.NET Core Tag Helpers](https://learn.microsoft.com/en-us/aspnet/core/mvc/views/tag-helpers/intro)

## まとめ

Turbo Drive のサーバーサイド対応により、Hotwire.AspNetCore はより完全な Hotwire 実装となり、ASP.NET Core 開発者が最小限の JavaScript で高速でインタラクティブな Web アプリケーションを構築できるようになります。

**推定工数**: 4-6週間  
**優先度**: 中〜高  
**実装難易度**: 中  
**ユーザーへの影響**: 大（UX の大幅な向上）
