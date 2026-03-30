# Hotwire.AspNetCore のリリースとアーキテクチャ解説

[Hotwire.AspNetCore](https://github.com/tnakamura/Hotwire.AspNetCore) を公開しました。ASP.NET Core で「少しリッチな」画面体験を作りたいときに、MVC / Razor Pages から大きく逸脱せずに Turbo の流儀を持ち込めるライブラリです。

* [https://github.com/tnakamura/Hotwire.AspNetCore](https://github.com/tnakamura/Hotwire.AspNetCore)

開発動機は明確で、ASP.NET Core で UI 体験を上げようとすると、MVC / Razor Pages の延長で頑張るか、Blazor へ寄せるかの二択になりがちだったことです。もちろん Blazor は強力ですが、既存の MVC / Razor Pages 資産を活かしながら「部分更新」「高速遷移」「サーバー主導のリアルタイム更新」を小さく導入したいケースも多い。そこで Rails の Hotwire がちょうど良い落としどころに見えたので、ASP.NET Core 向けに実装しました。

この記事では、まず何ができるのかと導入方法を整理し、その後にソースコードベースで実装時の工夫を解説します。

ライブラリで何ができるか
---

現在の `Hotwire.AspNetCore` は、Turbo を中心に次の機能を提供しています。

- **Turbo Drive**: ページ遷移高速化（`turbo-drive-meta`, `turbo-permanent`）
- **Turbo Frames**: 部分差し替え
- **Turbo Streams**: append / replace / remove などのサーバー主導 DOM 更新
- **Turbo Streams Custom Actions**: 標準外アクションを拡張
- **SignalR Integration**: Turbo Stream をリアルタイム push
- **Stimulus Integration**（別パッケージ `Stimulus.AspNetCore`）

実装位置:

- Turbo 本体: `/src/Turbo.AspNetCore/`
- Stimulus 本体: `/src/Stimulus.AspNetCore/`
- サンプル: `/examples/WireDrive`, `/examples/WireFrame`, `/examples/WireStream`, `/examples/WireSignal`, `/examples/WireStimulus`

テストは `Turbo.AspNetCore.Test` と `Stimulus.AspNetCore.Test` があり、いずれも通る状態です。

導入手順（最短）
---

導入は段階的にできるようにしてあります。最小だと次の 4 ステップです。

```bash
dotnet add package Hotwire.AspNetCore
```

```csharp
// Views/_ViewImports.cshtml
@addTagHelper *, Turbo.AspNetCore
@addTagHelper *, Stimulus.AspNetCore
```

```html
<!-- Views/Shared/_Layout.cshtml -->
<script type="module" src="https://cdn.jsdelivr.net/npm/@hotwired/turbo@latest/dist/turbo.es2017-esm.min.js"></script>
<turbo-drive-meta enabled="true" transition="fade" />
```

```csharp
// Controller
using Turbo.AspNetCore;

public IActionResult Create(MessageViewModel model)
{
    if (Request.IsTurboStreamRequest())
    {
        return this.TurboStream("_Message", model);
    }

    return RedirectToAction("Index");
}
```

```html
<!-- _Message.cshtml -->
<turbo-stream-append target="messages">
  <div class="message">@Model.Content</div>
</turbo-stream-append>
```

リアルタイム配信が必要なら、SignalR 側を追加します。

```csharp
builder.Services.AddSignalR();
builder.Services.AddTurboStreamBroadcaster();

var app = builder.Build();
app.MapTurboStreamsHub(); // default: /hubs/turbo-streams
```

```csharp
await _broadcaster.BroadcastViewAsync("notifications", "_Notification", model);
```

これで Turbo Stream の HTML フラグメントを SignalR 経由で配信できます。

実装の工夫 1: リクエスト判定を単純化する拡張メソッド
---

Controller で毎回ヘッダー判定を書くのは冗長なので、`TurboHttpRequestExtensions` に判定を集約しています。

```csharp
public static bool IsTurboDriveRequest(this HttpRequest request)
{
    return !request.Headers.ContainsKey("turbo-frame") &&
           request.GetTypedHeaders().Accept.Any(x => x.MediaType == "text/html");
}
```

`IsTurboFrameRequest()`, `IsTurboStreamRequest()`, `IsTurboRequest()` も同居させることで、分岐はアプリ側で簡潔に書けます。

- `/src/Turbo.AspNetCore/TurboHttpRequestExtensions.cs`

実装の工夫 2: Turbo Stream TagHelper の継承設計
---

Turbo Stream の標準アクションは種類が多いですが、各 TagHelper で同じ処理を繰り返すと保守性が落ちます。そこで 3 層の継承にしています。

- `TurboStreamTagHelper`（共通: `turbo-stream` 変換 + `<template>` 包装）
- `TurboStreamActionTagHelper`（単一 target）
- `TurboStreamActionAllTagHelper`（複数 targets）

```csharp
public abstract class TurboStreamActionTagHelper : TurboStreamTagHelper
{
    public string? Target { get; set; }
    private protected abstract string Action { get; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.Attributes.SetAttribute("target", Target);
        output.Attributes.SetAttribute("action", Action);
        base.Process(context, output);
    }
}

public sealed class TurboStreamAppendTagHelper : TurboStreamActionTagHelper
{
    private protected override string Action => "append";
}
```

この形により、追加アクション実装は `Action` の差分だけで済みます。Turbo 8 系の `morph` / `refresh` を増やしたときも、既存設計に自然に載せられました。

- `/src/Turbo.AspNetCore/TagHelpers/TurboStreamTagHelper.cs`

実装の工夫 3: Custom Action の属性受け渡し
---

カスタムアクションは、利用側の JavaScript 実装に合わせて任意属性を運びたいので、固定属性列挙ではなく辞書受けを採用しています。

```csharp
[HtmlAttributeName(DictionaryAttributePrefix = "")]
public IDictionary<string, string?> AdditionalAttributes { get; set; }
    = new Dictionary<string, string?>();
```

```csharp
foreach (var attribute in AdditionalAttributes)
{
    if (attribute.Key.Equals("action", StringComparison.OrdinalIgnoreCase))
    {
        continue;
    }
    output.Attributes.SetAttribute(attribute.Key, attribute.Value);
}
```

これで `turbo-stream-custom` から渡した任意属性を、`<turbo-stream ...>` にそのまま反映できます。

- `/src/Turbo.AspNetCore/TagHelpers/TurboStreamCustomActionTagHelper.cs`
- `/src/Turbo.AspNetCore/TurboStreamCustomHtmlExtensions.cs`

実装の工夫 4: SignalR 配信で View を直接レンダリング
---

`TurboStreamBroadcaster` は、単純な文字列送信だけでなく、Razor View をレンダリングして送る経路を持っています。

```csharp
public async Task BroadcastViewAsync(string channel, string viewName, object? model = null)
{
    var html = await RenderViewToStringAsync(viewName, model);
    await BroadcastAsync(channel, html);
}
```

さらに `HttpContext` が取れないケース（非HTTPコンテキスト）向けに `DefaultHttpContext` を補う実装にして、配信処理の実行コンテキスト依存を減らしています。

```csharp
var currentHttpContext = _httpContextAccessor.HttpContext;
var httpContext = currentHttpContext ?? new DefaultHttpContext { RequestServices = _serviceProvider };
```

- `/src/Turbo.AspNetCore/TurboStreamBroadcaster.cs`

実装の工夫 5: 再接続と再購読をクライアント側で吸収
---

`turbo-signalr.js` は、接続断の復帰後にチャネル再購読まで行います。

```javascript
this.connection.onreconnected((connectionId) => {
    this.dispatchEvent('reconnected', { connectionId });
    this.resubscribeChannels();
});
```

指数バックオフの再接続も入れてあり、運用で遭遇しがちな一時断に対して、アプリ側コードを増やさず耐えられるようにしています。

- `/src/Turbo.AspNetCore/wwwroot/js/turbo-signalr.js`

実装の工夫 6: 導入コストを下げる API デザイン
---

利用側が覚える API を減らすため、主要な配線は拡張メソッド化しています。

- `AddTurboStreamBroadcaster()`（DI 登録）
- `MapTurboStreamsHub()`（ルーティング）
- `TurboStream(...)`（Controller 応答）

```csharp
services.AddHttpContextAccessor();
services.TryAddScoped<ITurboStreamBroadcaster, TurboStreamBroadcaster>();
```

```csharp
return endpoints.MapHub<TurboStreamsHub>(pattern);
```

- `/src/Turbo.AspNetCore/TurboServiceCollectionExtensions.cs`
- `/src/Turbo.AspNetCore/TurboEndpointRouteBuilderExtensions.cs`
- `/src/Turbo.AspNetCore/TurboControllerExtensions.cs`

どんなケースで効くか
---

向いているのは、次のようなプロジェクトです。

- MVC / Razor Pages を維持しつつ UX を上げたい
- SPA 全面移行は重いが、部分更新や push 更新は欲しい
- サーバーサイドレンダリングを中心に進めたい

逆に、クライアントに重い状態管理を全面的に寄せたい場合は、Blazor / SPA のほうが適します。

まとめ
---

`Hotwire.AspNetCore` は、ASP.NET Core で「リッチさ」と「既存資産活用」を両立させるために作ったライブラリです。MVC / Razor Pages の開発体験を保ちながら、Turbo Drive / Frames / Streams と SignalR を段階導入できます。

まずは Turbo Drive と Turbo Streams を小さく導入し、必要になったら SignalR と Stimulus を足す、という進め方が最も効果的です。
