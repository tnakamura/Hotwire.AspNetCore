# Stimulus.AspNetCore 実装計画

**作成日**: 2026年2月11日  
**目的**: ASP.NET Core で Stimulus.js を効率的に使用するためのサーバーサイドヘルパーの実装  
**ベースドキュメント**: docs/hotwire-investigation-report.md Section 4.2.A

---

## 1. エグゼクティブサマリー

本ドキュメントは、Hotwire.AspNetCore に Stimulus.js のサーバーサイドサポートを実装する詳細な計画を提供します。Stimulus は JavaScript を最小限にして DOM 操作を実現する軽量フレームワークです。

### 主な目標

- ✅ Stimulus data 属性（controller, action, target, value）を生成する Tag Helper
- ✅ 型安全で IntelliSense に対応した拡張メソッド
- ✅ 開発者フレンドリーな API
- ✅ Rails の stimulus-rails gem との設計思想の一致
- ✅ 実践的なサンプルアプリケーション（WireStimulus）

---

## 2. Stimulus.js の基本概念

### 2.1 コア概念

| 概念 | 説明 | HTML 例 |
|-----|------|---------|
| **Controller** | JavaScript のコントローラークラスを HTML 要素に接続 | `data-controller="dropdown"` |
| **Target** | コントローラーから参照可能な DOM 要素 | `data-dropdown-target="menu"` |
| **Action** | イベントをコントローラーのメソッドにバインド | `data-action="click->dropdown#toggle"` |
| **Value** | コントローラーに値を渡す | `data-dropdown-open-value="false"` |
| **Class** | CSS クラス名を動的に参照 | `data-dropdown-active-class="show"` |
| **Outlet** | 他のコントローラーを参照 | `data-dropdown-outlet="#search"` |

### 2.2 Stimulus の命名規則

```
data-{controller}-{identifier}-{type}

例:
- data-controller="dropdown"              # コントローラー
- data-dropdown-target="menu"             # ターゲット
- data-action="click->dropdown#toggle"    # アクション
- data-dropdown-open-value="true"         # 値
- data-dropdown-active-class="highlight"  # クラス
```

---

## 3. アーキテクチャ設計

### 3.1 全体構成

```
┌──────────────────┐
│   Razor View     │
│  (ASP.NET Core)  │
└────────┬─────────┘
         │ Tag Helpers で data 属性を生成
         ↓
┌──────────────────┐
│   HTML Output    │
│ data-controller, │
│ data-action, etc │
└────────┬─────────┘
         │ ブラウザで解釈
         ↓
┌──────────────────┐
│   Stimulus.js    │
│  (JavaScript)    │ → DOM 操作
└──────────────────┘
```

### 3.2 実装コンポーネント

| コンポーネント | 役割 | 実装ファイル |
|------------|------|------------|
| `StimulusControllerTagHelper` | `data-controller` 属性を生成 | `src/Stimulus.AspNetCore/TagHelpers/StimulusControllerTagHelper.cs` |
| `StimulusActionTagHelper` | `data-action` 属性を生成 | `src/Stimulus.AspNetCore/TagHelpers/StimulusActionTagHelper.cs` |
| `StimulusTargetTagHelper` | `data-{controller}-target` 属性を生成 | `src/Stimulus.AspNetCore/TagHelpers/StimulusTargetTagHelper.cs` |
| `StimulusValueTagHelper` | `data-{controller}-{name}-value` 属性を生成 | `src/Stimulus.AspNetCore/TagHelpers/StimulusValueTagHelper.cs` |
| `StimulusClassTagHelper` | `data-{controller}-{name}-class` 属性を生成 | `src/Stimulus.AspNetCore/TagHelpers/StimulusClassTagHelper.cs` |
| `StimulusHtmlExtensions` | HTML 属性を生成するヘルパーメソッド | `src/Stimulus.AspNetCore/StimulusHtmlExtensions.cs` |

---

## 4. 実装詳細

### 4.1 StimulusControllerTagHelper

**ファイル**: `src/Stimulus.AspNetCore/TagHelpers/StimulusControllerTagHelper.cs`

```csharp
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Stimulus.AspNetCore.TagHelpers
{
    /// <summary>
    /// Stimulus コントローラーを HTML 要素に接続する Tag Helper
    /// </summary>
    /// <example>
    /// &lt;div stimulus-controller="dropdown"&gt;&lt;/div&gt;
    /// → &lt;div data-controller="dropdown"&gt;&lt;/div&gt;
    /// </example>
    [HtmlTargetElement(Attributes = "stimulus-controller")]
    public class StimulusControllerTagHelper : TagHelper
    {
        /// <summary>
        /// Stimulus コントローラー名（複数指定可能、スペース区切り）
        /// </summary>
        [HtmlAttributeName("stimulus-controller")]
        public string Controller { get; set; } = string.Empty;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!string.IsNullOrWhiteSpace(Controller))
            {
                output.Attributes.SetAttribute("data-controller", Controller.Trim());
            }
            
            // stimulus-controller 属性自体は削除
            output.Attributes.RemoveAll("stimulus-controller");
        }
    }
}
```

**使用例**:
```html
<!-- Razor View -->
<div stimulus-controller="dropdown">
    <button>Toggle</button>
</div>

<!-- 出力 HTML -->
<div data-controller="dropdown">
    <button>Toggle</button>
</div>
```

**複数コントローラー**:
```html
<!-- Razor View -->
<div stimulus-controller="dropdown clipboard">
    Content
</div>

<!-- 出力 HTML -->
<div data-controller="dropdown clipboard">
    Content
</div>
```

---

### 4.2 StimulusActionTagHelper

**ファイル**: `src/Stimulus.AspNetCore/TagHelpers/StimulusActionTagHelper.cs`

```csharp
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Stimulus.AspNetCore.TagHelpers
{
    /// <summary>
    /// Stimulus アクションを HTML 要素に接続する Tag Helper
    /// </summary>
    /// <example>
    /// &lt;button stimulus-action="click->dropdown#toggle"&gt;Toggle&lt;/button&gt;
    /// → &lt;button data-action="click->dropdown#toggle"&gt;Toggle&lt;/button&gt;
    /// </example>
    [HtmlTargetElement(Attributes = "stimulus-action")]
    public class StimulusActionTagHelper : TagHelper
    {
        /// <summary>
        /// Stimulus アクション（フォーマット: "event->controller#method"）
        /// 複数指定可能（スペース区切り）
        /// </summary>
        [HtmlAttributeName("stimulus-action")]
        public string Action { get; set; } = string.Empty;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!string.IsNullOrWhiteSpace(Action))
            {
                // 既存の data-action と統合
                var existingAction = output.Attributes["data-action"]?.Value?.ToString();
                if (!string.IsNullOrWhiteSpace(existingAction))
                {
                    output.Attributes.SetAttribute("data-action", 
                        $"{existingAction} {Action.Trim()}");
                }
                else
                {
                    output.Attributes.SetAttribute("data-action", Action.Trim());
                }
            }
            
            // stimulus-action 属性自体は削除
            output.Attributes.RemoveAll("stimulus-action");
        }
    }
}
```

**使用例**:
```html
<!-- 基本的な使用 -->
<button stimulus-action="click->dropdown#toggle">Toggle</button>
<!-- 出力: <button data-action="click->dropdown#toggle">Toggle</button> -->

<!-- イベント省略（button では click がデフォルト） -->
<button stimulus-action="dropdown#toggle">Toggle</button>
<!-- 出力: <button data-action="dropdown#toggle">Toggle</button> -->

<!-- 複数アクション -->
<button stimulus-action="click->dropdown#toggle mouseenter->dropdown#show">
    Hover or Click
</button>
<!-- 出力: <button data-action="click->dropdown#toggle mouseenter->dropdown#show">Hover or Click</button> -->

<!-- グローバルイベント -->
<div stimulus-action="resize@window->layout#recalculate">
    Content
</div>
<!-- 出力: <div data-action="resize@window->layout#recalculate">Content</div> -->
```

---

### 4.3 StimulusTargetTagHelper

**ファイル**: `src/Stimulus.AspNetCore/TagHelpers/StimulusTargetTagHelper.cs`

```csharp
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Stimulus.AspNetCore.TagHelpers
{
    /// <summary>
    /// Stimulus ターゲットを HTML 要素に設定する Tag Helper
    /// </summary>
    /// <example>
    /// &lt;div stimulus-target="dropdown.menu"&gt;&lt;/div&gt;
    /// → &lt;div data-dropdown-target="menu"&gt;&lt;/div&gt;
    /// </example>
    [HtmlTargetElement(Attributes = "stimulus-target")]
    public class StimulusTargetTagHelper : TagHelper
    {
        /// <summary>
        /// Stimulus ターゲット（フォーマット: "controller.target" または "target"）
        /// 複数指定可能（スペース区切り）
        /// </summary>
        [HtmlAttributeName("stimulus-target")]
        public string Target { get; set; } = string.Empty;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!string.IsNullOrWhiteSpace(Target))
            {
                var targets = Target.Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
                
                foreach (var target in targets)
                {
                    var parts = target.Split('.');
                    
                    if (parts.Length == 2)
                    {
                        // "controller.target" 形式
                        var controller = parts[0].Trim();
                        var targetName = parts[1].Trim();
                        var attributeName = $"data-{controller}-target";
                        
                        // 既存の target 属性と統合
                        var existingTarget = output.Attributes[attributeName]?.Value?.ToString();
                        if (!string.IsNullOrWhiteSpace(existingTarget))
                        {
                            output.Attributes.SetAttribute(attributeName, 
                                $"{existingTarget} {targetName}");
                        }
                        else
                        {
                            output.Attributes.SetAttribute(attributeName, targetName);
                        }
                    }
                }
            }
            
            // stimulus-target 属性自体は削除
            output.Attributes.RemoveAll("stimulus-target");
        }
    }
}
```

**使用例**:
```html
<!-- 基本的な使用 -->
<div stimulus-target="dropdown.menu">Menu</div>
<!-- 出力: <div data-dropdown-target="menu">Menu</div> -->

<!-- 複数ターゲット -->
<input stimulus-target="form.name form.email" />
<!-- 出力: <input data-form-target="name email" /> -->

<!-- 異なるコントローラーのターゲット -->
<div stimulus-target="dropdown.item list.item">Item</div>
<!-- 出力: <div data-dropdown-target="item" data-list-target="item">Item</div> -->
```

---

### 4.4 StimulusValueTagHelper

**ファイル**: `src/Stimulus.AspNetCore/TagHelpers/StimulusValueTagHelper.cs`

```csharp
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Stimulus.AspNetCore.TagHelpers
{
    /// <summary>
    /// Stimulus 値を HTML 要素に設定する Tag Helper
    /// </summary>
    /// <example>
    /// &lt;div stimulus-value-dropdown-open="false"&gt;&lt;/div&gt;
    /// → &lt;div data-dropdown-open-value="false"&gt;&lt;/div&gt;
    /// </example>
    [HtmlTargetElement(Attributes = "stimulus-value-*")]
    public class StimulusValueTagHelper : TagHelper
    {
        private readonly IDictionary<string, string> _values = 
            new Dictionary<string, string>();

        /// <summary>
        /// 動的に Stimulus 値を受け付ける
        /// 例: stimulus-value-dropdown-open="true"
        /// </summary>
        [HtmlAttributeName("stimulus-value-", DictionaryAttributePrefix = "stimulus-value-")]
        public IDictionary<string, string> Values
        {
            get => _values;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            foreach (var kvp in Values)
            {
                // stimulus-value-dropdown-open → data-dropdown-open-value
                var key = kvp.Key; // 例: "dropdown-open"
                var value = kvp.Value;
                
                output.Attributes.SetAttribute($"data-{key}-value", value);
            }
            
            // stimulus-value-* 属性自体は削除
            var attributesToRemove = output.Attributes
                .Where(attr => attr.Name.StartsWith("stimulus-value-"))
                .ToList();
            
            foreach (var attr in attributesToRemove)
            {
                output.Attributes.Remove(attr);
            }
        }
    }
}
```

**使用例**:
```html
<!-- Boolean 値 -->
<div stimulus-value-dropdown-open="false">
    Content
</div>
<!-- 出力: <div data-dropdown-open-value="false">Content</div> -->

<!-- 数値 -->
<div stimulus-value-counter-count="42">
    Count: 42
</div>
<!-- 出力: <div data-counter-count-value="42">Count: 42</div> -->

<!-- 文字列 -->
<div stimulus-value-search-query="hello world">
    Search
</div>
<!-- 出力: <div data-search-query-value="hello world">Search</div> -->

<!-- JSON オブジェクト（エスケープが必要） -->
<div stimulus-value-map-config='{"lat":35.6762,"lng":139.6503}'>
    Map
</div>
<!-- 出力: <div data-map-config-value='{"lat":35.6762,"lng":139.6503}'>Map</div> -->
```

---

### 4.5 StimulusClassTagHelper

**ファイル**: `src/Stimulus.AspNetCore/TagHelpers/StimulusClassTagHelper.cs`

```csharp
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Stimulus.AspNetCore.TagHelpers
{
    /// <summary>
    /// Stimulus CSS クラス名を HTML 要素に設定する Tag Helper
    /// </summary>
    /// <example>
    /// &lt;div stimulus-class-dropdown-active="highlight"&gt;&lt;/div&gt;
    /// → &lt;div data-dropdown-active-class="highlight"&gt;&lt;/div&gt;
    /// </example>
    [HtmlTargetElement(Attributes = "stimulus-class-*")]
    public class StimulusClassTagHelper : TagHelper
    {
        private readonly IDictionary<string, string> _classes = 
            new Dictionary<string, string>();

        /// <summary>
        /// 動的に Stimulus CSS クラスを受け付ける
        /// 例: stimulus-class-dropdown-active="show"
        /// </summary>
        [HtmlAttributeName("stimulus-class-", DictionaryAttributePrefix = "stimulus-class-")]
        public IDictionary<string, string> Classes
        {
            get => _classes;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            foreach (var kvp in Classes)
            {
                // stimulus-class-dropdown-active → data-dropdown-active-class
                var key = kvp.Key; // 例: "dropdown-active"
                var value = kvp.Value;
                
                output.Attributes.SetAttribute($"data-{key}-class", value);
            }
            
            // stimulus-class-* 属性自体は削除
            var attributesToRemove = output.Attributes
                .Where(attr => attr.Name.StartsWith("stimulus-class-"))
                .ToList();
            
            foreach (var attr in attributesToRemove)
            {
                output.Attributes.Remove(attr);
            }
        }
    }
}
```

**使用例**:
```html
<!-- 単一クラス -->
<div stimulus-class-dropdown-active="show">
    Dropdown
</div>
<!-- 出力: <div data-dropdown-active-class="show">Dropdown</div> -->

<!-- 複数クラス -->
<div stimulus-class-list-loading="spinner opacity-50">
    Loading...
</div>
<!-- 出力: <div data-list-loading-class="spinner opacity-50">Loading...</div> -->
```

---

### 4.6 StimulusHtmlExtensions

**ファイル**: `src/Stimulus.AspNetCore/StimulusHtmlExtensions.cs`

```csharp
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace Stimulus.AspNetCore
{
    /// <summary>
    /// Stimulus data 属性を生成するための拡張メソッド
    /// </summary>
    public static class StimulusHtmlExtensions
    {
        /// <summary>
        /// Stimulus コントローラー属性を生成
        /// </summary>
        public static IDictionary<string, object> StimulusController(
            this IHtmlHelper html, 
            string controller)
        {
            return new Dictionary<string, object>
            {
                { "data-controller", controller }
            };
        }

        /// <summary>
        /// Stimulus コントローラー属性を生成（複数）
        /// </summary>
        public static IDictionary<string, object> StimulusController(
            this IHtmlHelper html, 
            params string[] controllers)
        {
            return new Dictionary<string, object>
            {
                { "data-controller", string.Join(" ", controllers) }
            };
        }

        /// <summary>
        /// Stimulus アクション属性を生成
        /// </summary>
        public static IDictionary<string, object> StimulusAction(
            this IHtmlHelper html, 
            string action)
        {
            return new Dictionary<string, object>
            {
                { "data-action", action }
            };
        }

        /// <summary>
        /// Stimulus アクション属性を生成（複数）
        /// </summary>
        public static IDictionary<string, object> StimulusAction(
            this IHtmlHelper html, 
            params string[] actions)
        {
            return new Dictionary<string, object>
            {
                { "data-action", string.Join(" ", actions) }
            };
        }

        /// <summary>
        /// Stimulus ターゲット属性を生成
        /// </summary>
        public static IDictionary<string, object> StimulusTarget(
            this IHtmlHelper html, 
            string controller, 
            string target)
        {
            return new Dictionary<string, object>
            {
                { $"data-{controller}-target", target }
            };
        }

        /// <summary>
        /// Stimulus ターゲット属性を生成（複数ターゲット）
        /// </summary>
        public static IDictionary<string, object> StimulusTarget(
            this IHtmlHelper html, 
            string controller, 
            params string[] targets)
        {
            return new Dictionary<string, object>
            {
                { $"data-{controller}-target", string.Join(" ", targets) }
            };
        }

        /// <summary>
        /// Stimulus 値属性を生成
        /// </summary>
        public static IDictionary<string, object> StimulusValue(
            this IHtmlHelper html, 
            string controller, 
            string name, 
            object value)
        {
            return new Dictionary<string, object>
            {
                { $"data-{controller}-{name}-value", value.ToString() }
            };
        }

        /// <summary>
        /// Stimulus クラス属性を生成
        /// </summary>
        public static IDictionary<string, object> StimulusClass(
            this IHtmlHelper html, 
            string controller, 
            string name, 
            string className)
        {
            return new Dictionary<string, object>
            {
                { $"data-{controller}-{name}-class", className }
            };
        }

        /// <summary>
        /// 複数の Stimulus 属性を結合
        /// </summary>
        public static IDictionary<string, object> StimulusAttributes(
            this IHtmlHelper html,
            params IDictionary<string, object>[] attributeSets)
        {
            var combined = new Dictionary<string, object>();
            
            foreach (var set in attributeSets)
            {
                foreach (var kvp in set)
                {
                    if (combined.ContainsKey(kvp.Key))
                    {
                        // data-action などは結合
                        combined[kvp.Key] = $"{combined[kvp.Key]} {kvp.Value}";
                    }
                    else
                    {
                        combined[kvp.Key] = kvp.Value;
                    }
                }
            }
            
            return combined;
        }
    }
}
```

**使用例**:

```csharp
// Razor View での使用例

// 1. コントローラー
@Html.TextBoxFor(m => m.Name, Html.StimulusController("form"))
// 出力: <input data-controller="form" />

// 2. アクション
@Html.ButtonFor(m => m.Submit, Html.StimulusAction("click->form#submit"))
// 出力: <button data-action="click->form#submit" />

// 3. ターゲット
@Html.TextBoxFor(m => m.Name, Html.StimulusTarget("form", "name"))
// 出力: <input data-form-target="name" />

// 4. 値
@Html.DivFor(m => m.Counter, Html.StimulusValue("counter", "count", 0))
// 出力: <div data-counter-count-value="0" />

// 5. 複数属性の結合
@Html.TextBoxFor(m => m.Email, Html.StimulusAttributes(
    Html.StimulusController("form"),
    Html.StimulusTarget("form", "email"),
    Html.StimulusAction("blur->form#validate")
))
// 出力: <input data-controller="form" 
//              data-form-target="email" 
//              data-action="blur->form#validate" />
```

---

## 5. サンプルアプリケーション (WireStimulus)

### 5.1 概要

**目的**: Stimulus.AspNetCore の実用的な使用例を示す

**デモ機能**:
1. **Dropdown Controller** - クリックでメニューを開閉
2. **Clipboard Controller** - テキストをクリップボードにコピー
3. **Counter Controller** - インクリメント/デクリメント
4. **Form Validation Controller** - リアルタイムバリデーション
5. **Slideshow Controller** - 画像スライドショー

### 5.2 プロジェクト構成

```
examples/WireStimulus/
├── Controllers/
│   ├── HomeController.cs
│   ├── DropdownController.cs
│   ├── ClipboardController.cs
│   ├── CounterController.cs
│   └── FormController.cs
├── Views/
│   ├── Home/
│   │   └── Index.cshtml
│   ├── Dropdown/
│   │   └── Index.cshtml
│   ├── Clipboard/
│   │   └── Index.cshtml
│   ├── Counter/
│   │   └── Index.cshtml
│   ├── Form/
│   │   └── Index.cshtml
│   └── Shared/
│       └── _Layout.cshtml
├── wwwroot/
│   ├── js/
│   │   ├── controllers/
│   │   │   ├── dropdown_controller.js
│   │   │   ├── clipboard_controller.js
│   │   │   ├── counter_controller.js
│   │   │   ├── form_controller.js
│   │   │   └── slideshow_controller.js
│   │   ├── application.js       # Stimulus Application
│   │   └── stimulus.min.js      # Stimulus ライブラリ
│   └── css/
│       └── site.css
├── Program.cs
├── WireStimulus.csproj
└── README.md
```

### 5.3 実装例: Dropdown Controller

**JavaScript** (`wwwroot/js/controllers/dropdown_controller.js`):

```javascript
import { Controller } from "@hotwired/stimulus"

export default class extends Controller {
  static targets = ["menu"]
  static classes = ["active"]
  static values = {
    open: Boolean
  }

  connect() {
    console.log("Dropdown controller connected")
  }

  toggle(event) {
    event.preventDefault()
    this.openValue = !this.openValue
  }

  openValueChanged() {
    if (this.openValue) {
      this.menuTarget.classList.add(this.activeClass)
    } else {
      this.menuTarget.classList.remove(this.activeClass)
    }
  }

  hide(event) {
    if (!this.element.contains(event.target)) {
      this.openValue = false
    }
  }
}
```

**Razor View** (`Views/Dropdown/Index.cshtml`):

```html
@{
    ViewData["Title"] = "Dropdown Example";
}

<h1>Stimulus Dropdown Example</h1>

<!-- Tag Helpers を使用 -->
<div stimulus-controller="dropdown" 
     stimulus-value-dropdown-open="false"
     stimulus-class-dropdown-active="show"
     stimulus-action="click@window->dropdown#hide">
    
    <button stimulus-action="click->dropdown#toggle" 
            class="btn btn-primary">
        Toggle Dropdown
    </button>
    
    <div stimulus-target="dropdown.menu" 
         class="dropdown-menu">
        <a class="dropdown-item" href="#">Action</a>
        <a class="dropdown-item" href="#">Another action</a>
        <a class="dropdown-item" href="#">Something else</a>
    </div>
</div>

<!-- または HTML Extensions を使用 -->
<div @Html.Raw(string.Join(" ", Html.StimulusAttributes(
    Html.StimulusController("dropdown"),
    Html.StimulusValue("dropdown", "open", false),
    Html.StimulusClass("dropdown", "active", "show"),
    Html.StimulusAction("click@window->dropdown#hide")
).Select(kvp => $"{kvp.Key}=\"{kvp.Value}\"")))>
    
    <button @Html.Raw(string.Join(" ", Html.StimulusAction("click->dropdown#toggle")
        .Select(kvp => $"{kvp.Key}=\"{kvp.Value}\""))) 
            class="btn btn-primary">
        Toggle Dropdown (HTML Helper)
    </button>
    
    <div @Html.Raw(string.Join(" ", Html.StimulusTarget("dropdown", "menu")
        .Select(kvp => $"{kvp.Key}=\"{kvp.Value}\""))) 
         class="dropdown-menu">
        <a class="dropdown-item" href="#">Action</a>
        <a class="dropdown-item" href="#">Another action</a>
    </div>
</div>

<style>
    .dropdown-menu {
        display: none;
        position: absolute;
        background: white;
        border: 1px solid #ccc;
        padding: 10px;
        margin-top: 5px;
    }
    
    .dropdown-menu.show {
        display: block;
    }
    
    .dropdown-item {
        display: block;
        padding: 5px 10px;
        text-decoration: none;
        color: #333;
    }
    
    .dropdown-item:hover {
        background-color: #f0f0f0;
    }
</style>
```

### 5.4 実装例: Clipboard Controller

**JavaScript** (`wwwroot/js/controllers/clipboard_controller.js`):

```javascript
import { Controller } from "@hotwired/stimulus"

export default class extends Controller {
  static targets = ["source", "button"]
  static classes = ["success"]
  static values = {
    successDuration: { type: Number, default: 2000 }
  }

  copy(event) {
    event.preventDefault()
    
    navigator.clipboard.writeText(this.sourceTarget.value)
      .then(() => this.copied())
      .catch(() => console.error("Failed to copy"))
  }

  copied() {
    if (!this.hasButtonTarget) return
    
    const originalText = this.buttonTarget.textContent
    this.buttonTarget.textContent = "Copied!"
    this.buttonTarget.classList.add(this.successClass)
    
    setTimeout(() => {
      this.buttonTarget.textContent = originalText
      this.buttonTarget.classList.remove(this.successClass)
    }, this.successDurationValue)
  }
}
```

**Razor View** (`Views/Clipboard/Index.cshtml`):

```html
@{
    ViewData["Title"] = "Clipboard Example";
}

<h1>Stimulus Clipboard Example</h1>

<div stimulus-controller="clipboard" 
     stimulus-value-clipboard-success-duration="3000"
     stimulus-class-clipboard-success="btn-success">
    
    <input stimulus-target="clipboard.source" 
           type="text" 
           value="Hello, Stimulus!" 
           readonly
           class="form-control mb-2" />
    
    <button stimulus-target="clipboard.button"
            stimulus-action="click->clipboard#copy" 
            class="btn btn-primary">
        Copy to Clipboard
    </button>
</div>
```

### 5.5 実装例: Counter Controller

**JavaScript** (`wwwroot/js/controllers/counter_controller.js`):

```javascript
import { Controller } from "@hotwired/stimulus"

export default class extends Controller {
  static targets = ["output"]
  static values = {
    count: { type: Number, default: 0 },
    step: { type: Number, default: 1 }
  }

  connect() {
    this.updateOutput()
  }

  increment() {
    this.countValue += this.stepValue
  }

  decrement() {
    this.countValue -= this.stepValue
  }

  reset() {
    this.countValue = 0
  }

  countValueChanged() {
    this.updateOutput()
  }

  updateOutput() {
    this.outputTarget.textContent = this.countValue
  }
}
```

**Razor View** (`Views/Counter/Index.cshtml`):

```html
@{
    ViewData["Title"] = "Counter Example";
}

<h1>Stimulus Counter Example</h1>

<div stimulus-controller="counter" 
     stimulus-value-counter-count="10"
     stimulus-value-counter-step="5">
    
    <h2>Count: <span stimulus-target="counter.output">0</span></h2>
    
    <button stimulus-action="click->counter#increment" 
            class="btn btn-success">
        + Increment
    </button>
    
    <button stimulus-action="click->counter#decrement" 
            class="btn btn-warning">
        - Decrement
    </button>
    
    <button stimulus-action="click->counter#reset" 
            class="btn btn-secondary">
        Reset
    </button>
</div>
```

---

## 6. テスト戦略

### 6.1 単体テスト

**テストファイル**: `test/Stimulus.AspNetCore.Test/StimulusTagHelperTest.cs`

```csharp
using Microsoft.AspNetCore.Razor.TagHelpers;
using Stimulus.AspNetCore.TagHelpers;
using Xunit;

namespace Stimulus.AspNetCore.Test
{
    public class StimulusControllerTagHelperTest
    {
        [Fact]
        public void StimulusControllerTagHelper_SetsDataController()
        {
            // Arrange
            var tagHelper = new StimulusControllerTagHelper
            {
                Controller = "dropdown"
            };
            var context = new TagHelperContext(
                new TagHelperAttributeList(),
                new Dictionary<object, object>(),
                Guid.NewGuid().ToString("N"));
            var output = new TagHelperOutput(
                "div",
                new TagHelperAttributeList 
                {
                    new TagHelperAttribute("stimulus-controller", "dropdown")
                },
                (useCachedResult, encoder) => 
                    Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

            // Act
            tagHelper.Process(context, output);

            // Assert
            Assert.Equal("dropdown", output.Attributes["data-controller"].Value);
            Assert.DoesNotContain(output.Attributes, attr => attr.Name == "stimulus-controller");
        }

        [Fact]
        public void StimulusControllerTagHelper_SupportsMultipleControllers()
        {
            // Arrange
            var tagHelper = new StimulusControllerTagHelper
            {
                Controller = "dropdown clipboard"
            };
            var context = new TagHelperContext(
                new TagHelperAttributeList(),
                new Dictionary<object, object>(),
                Guid.NewGuid().ToString("N"));
            var output = new TagHelperOutput(
                "div",
                new TagHelperAttributeList(),
                (useCachedResult, encoder) => 
                    Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

            // Act
            tagHelper.Process(context, output);

            // Assert
            Assert.Equal("dropdown clipboard", output.Attributes["data-controller"].Value);
        }
    }

    public class StimulusActionTagHelperTest
    {
        [Fact]
        public void StimulusActionTagHelper_SetsDataAction()
        {
            // Arrange
            var tagHelper = new StimulusActionTagHelper
            {
                Action = "click->dropdown#toggle"
            };
            var context = new TagHelperContext(
                new TagHelperAttributeList(),
                new Dictionary<object, object>(),
                Guid.NewGuid().ToString("N"));
            var output = new TagHelperOutput(
                "button",
                new TagHelperAttributeList(),
                (useCachedResult, encoder) => 
                    Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

            // Act
            tagHelper.Process(context, output);

            // Assert
            Assert.Equal("click->dropdown#toggle", output.Attributes["data-action"].Value);
            Assert.DoesNotContain(output.Attributes, attr => attr.Name == "stimulus-action");
        }

        [Fact]
        public void StimulusActionTagHelper_MergesWithExistingDataAction()
        {
            // Arrange
            var tagHelper = new StimulusActionTagHelper
            {
                Action = "click->dropdown#toggle"
            };
            var context = new TagHelperContext(
                new TagHelperAttributeList(),
                new Dictionary<object, object>(),
                Guid.NewGuid().ToString("N"));
            var output = new TagHelperOutput(
                "button",
                new TagHelperAttributeList
                {
                    new TagHelperAttribute("data-action", "mouseenter->dropdown#show")
                },
                (useCachedResult, encoder) => 
                    Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

            // Act
            tagHelper.Process(context, output);

            // Assert
            Assert.Equal("mouseenter->dropdown#show click->dropdown#toggle", 
                output.Attributes["data-action"].Value);
        }
    }

    public class StimulusTargetTagHelperTest
    {
        [Fact]
        public void StimulusTargetTagHelper_SetsDataTarget()
        {
            // Arrange
            var tagHelper = new StimulusTargetTagHelper
            {
                Target = "dropdown.menu"
            };
            var context = new TagHelperContext(
                new TagHelperAttributeList(),
                new Dictionary<object, object>(),
                Guid.NewGuid().ToString("N"));
            var output = new TagHelperOutput(
                "div",
                new TagHelperAttributeList(),
                (useCachedResult, encoder) => 
                    Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

            // Act
            tagHelper.Process(context, output);

            // Assert
            Assert.Equal("menu", output.Attributes["data-dropdown-target"].Value);
            Assert.DoesNotContain(output.Attributes, attr => attr.Name == "stimulus-target");
        }

        [Fact]
        public void StimulusTargetTagHelper_SupportsMultipleTargets()
        {
            // Arrange
            var tagHelper = new StimulusTargetTagHelper
            {
                Target = "form.name form.email"
            };
            var context = new TagHelperContext(
                new TagHelperAttributeList(),
                new Dictionary<object, object>(),
                Guid.NewGuid().ToString("N"));
            var output = new TagHelperOutput(
                "input",
                new TagHelperAttributeList(),
                (useCachedResult, encoder) => 
                    Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

            // Act
            tagHelper.Process(context, output);

            // Assert
            Assert.Equal("name email", output.Attributes["data-form-target"].Value);
        }

        [Fact]
        public void StimulusTargetTagHelper_SupportsDifferentControllers()
        {
            // Arrange
            var tagHelper = new StimulusTargetTagHelper
            {
                Target = "dropdown.item list.item"
            };
            var context = new TagHelperContext(
                new TagHelperAttributeList(),
                new Dictionary<object, object>(),
                Guid.NewGuid().ToString("N"));
            var output = new TagHelperOutput(
                "div",
                new TagHelperAttributeList(),
                (useCachedResult, encoder) => 
                    Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

            // Act
            tagHelper.Process(context, output);

            // Assert
            Assert.Equal("item", output.Attributes["data-dropdown-target"].Value);
            Assert.Equal("item", output.Attributes["data-list-target"].Value);
        }
    }
}
```

### 6.2 統合テスト

統合テストは、実際のブラウザ環境での動作確認を含めるべきです（Selenium、Playwright など）。

---

## 7. ドキュメント

### 7.1 必要なドキュメント

| ドキュメント | 内容 | ファイル名 |
|----------|------|----------|
| **README** | プロジェクト概要とクイックスタート | `README.md` |
| **使用ガイド** | Tag Helper と拡張メソッドの詳細 | `docs/stimulus-guide.md` |
| **API リファレンス** | すべての Tag Helper と拡張メソッドのリファレンス | `docs/stimulus-api-reference.md` |
| **サンプル集** | 実用的なコード例 | `docs/stimulus-examples.md` |
| **Rails からの移行** | rails stimulus-rails との違い | `docs/stimulus-rails-migration.md` |

### 7.2 README.md の例

```markdown
# Stimulus.AspNetCore

ASP.NET Core で Stimulus.js を効率的に使用するためのサーバーサイドヘルパー。

## クイックスタート

### 1. インストール

```bash
dotnet add package Stimulus.AspNetCore
```

### 2. `_ViewImports.cshtml` に追加

```csharp
@addTagHelper *, Stimulus.AspNetCore
@using Stimulus.AspNetCore
```

### 3. Stimulus.js をインストール

```bash
npm install @hotwired/stimulus
```

### 4. Tag Helpers を使用

```html
<div stimulus-controller="dropdown">
    <button stimulus-action="click->dropdown#toggle">Toggle</button>
    <div stimulus-target="dropdown.menu">Menu</div>
</div>
```

## 機能

- ✅ `stimulus-controller` - コントローラーを接続
- ✅ `stimulus-action` - イベントをメソッドにバインド
- ✅ `stimulus-target` - 要素を参照
- ✅ `stimulus-value-*` - 値を渡す
- ✅ `stimulus-class-*` - CSS クラス名を参照
- ✅ HTML 拡張メソッド - プログラムで属性を生成

## ドキュメント

- [使用ガイド](docs/stimulus-guide.md)
- [API リファレンス](docs/stimulus-api-reference.md)
- [サンプル集](docs/stimulus-examples.md)

## サンプルアプリ

`examples/WireStimulus` に実用的なサンプルが含まれています。

## ライセンス

MIT License
```

---

## 8. 実装スケジュール

### フェーズ 1: コアコンポーネント（Week 1-2）

- [ ] `StimulusControllerTagHelper` の実装
- [ ] `StimulusActionTagHelper` の実装
- [ ] `StimulusTargetTagHelper` の実装
- [ ] 単体テストの作成（3 つの Tag Helper）

### フェーズ 2: 拡張機能（Week 3-4）

- [ ] `StimulusValueTagHelper` の実装
- [ ] `StimulusClassTagHelper` の実装
- [ ] `StimulusHtmlExtensions` の実装
- [ ] 単体テストの追加（すべての機能）

### フェーズ 3: サンプルアプリ（Week 5-6）

- [ ] WireStimulus プロジェクトの作成
- [ ] Dropdown Controller の実装
- [ ] Clipboard Controller の実装
- [ ] Counter Controller の実装
- [ ] Form Validation Controller の実装
- [ ] Slideshow Controller の実装

### フェーズ 4: ドキュメントとテスト（Week 7-8）

- [ ] README.md の作成
- [ ] 使用ガイドの作成
- [ ] API リファレンスの作成
- [ ] サンプル集の作成
- [ ] Rails 移行ガイドの作成
- [ ] 統合テストの追加

---

## 9. Rails との比較

### 9.1 stimulus-rails との機能パリティ

| 機能 | Rails (stimulus-rails) | ASP.NET Core (Stimulus.AspNetCore) |
|-----|----------------------|----------------------------------|
| コントローラー自動読み込み | ✅ `stimulus-rails` gem | ⚠️ 手動 import（JavaScript 側） |
| Helper メソッド | ✅ `data: { controller: "..." }` | ✅ Tag Helpers + HTML Extensions |
| npm/yarn 統合 | ✅ 自動 | ✅ 手動（標準的な npm） |
| Hot Module Replacement | ✅ Webpack/Vite | ⚠️ ブラウザリロード（手動） |

### 9.2 Rails との使用例比較

**Rails (ERB)**:
```erb
<div data-controller="dropdown">
  <%= button_tag "Toggle", data: { action: "click->dropdown#toggle" } %>
  <div data-dropdown-target="menu">Menu</div>
</div>
```

**ASP.NET Core (Razor)**:
```html
<div stimulus-controller="dropdown">
    <button stimulus-action="click->dropdown#toggle">Toggle</button>
    <div stimulus-target="dropdown.menu">Menu</div>
</div>
```

---

## 10. 今後の拡張

### 10.1 追加機能の候補

1. **Outlet サポート**
   - `stimulus-outlet-*` Tag Helper
   - 他のコントローラーへの参照

2. **Param サポート**
   - `data-{controller}-{action}-{param}-param`
   - アクションにパラメータを渡す

3. **Source Generator**
   - コンパイル時に Stimulus 属性を検証
   - IntelliSense のさらなる改善

4. **Blazor 統合**
   - Blazor コンポーネントから Stimulus を使用
   - `@bind` との統合

---

## 11. セキュリティ考慮事項

### 11.1 XSS 対策

- Tag Helper は自動的に属性値をエスケープ
- JSON 値を渡す場合は `@Html.Raw()` を使わず、適切にエスケープ

### 11.2 CSP (Content Security Policy)

Stimulus.js は inline script を使用しないため、厳格な CSP と互換性があります。

---

## 12. まとめ

Stimulus.AspNetCore の実装により、ASP.NET Core 開発者は Rails 開発者と同様に、最小限の JavaScript で高度なインタラクティブ機能を実現できます。Tag Helpers と HTML 拡張メソッドにより、型安全で IntelliSense に対応した開発体験が提供されます。

---

**作成者**: GitHub Copilot Agent  
**レポートバージョン**: 1.0  
**最終更新**: 2026年2月11日
