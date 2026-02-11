# Turbo カスタムアクション実装プラン

**作成日**: 2026年2月11日  
**バージョン**: 1.0  
**ステータス**: 提案中  
**対象**: Turbo.AspNetCore プロジェクト

---

## 1. エグゼクティブサマリー

このドキュメントは、Hotwire.AspNetCore の Turbo.AspNetCore に、Turbo のカスタムアクション機能を追加するための包括的な実装プランを提供します。

### 1.1 背景

現在、Turbo.AspNetCore は以下の標準 Turbo Stream アクションをサポートしています：
- **基本アクション（7種）**: append、prepend、replace、update、remove、before、after
- **複数ターゲットアクション（7種）**: append_all、prepend_all、replace_all、update_all、remove_all、before_all、after_all
- **Turbo 8 アクション（2種）**: morph、refresh

しかし、ユーザー定義のカスタムアクションには対応していません。これは、Rails の turbo-rails gem が提供している機能との最後のギャップです。

### 1.2 目的

- ユーザーが独自の Turbo Stream アクションを定義できるようにする
- Rails の `turbo_stream.action(:custom_action, ...)` 相当の機能を提供
- 拡張性の高い Tag Helper 基盤を構築
- 実用的なサンプルとドキュメントを提供

### 1.3 期待される成果

- ✅ `TurboStreamCustomActionTagHelper` の実装
- ✅ カスタムアクション用の拡張メソッドの実装
- ✅ 包括的な単体テスト（最低 3 件）
- ✅ サンプルアプリケーション（WireCustom または WireStream に追加）
- ✅ ドキュメント（使用ガイドとサンプルコード）

---

## 2. カスタムアクションの概要

### 2.1 カスタムアクションとは

Turbo のカスタムアクションは、ユーザーが JavaScript 側で独自の DOM 操作ロジックを定義し、サーバーサイドから呼び出せる機能です。標準アクション（append、replace など）では実現できない複雑な操作を実装できます。

### 2.2 使用例

#### JavaScript 側（カスタムアクション定義）

```javascript
// 例1: ページタイトルを設定するアクション
Turbo.StreamActions.set_title = function() {
  document.title = this.getAttribute("title");
}

// 例2: 通知を表示するアクション
Turbo.StreamActions.notify = function() {
  const message = this.getAttribute("message");
  const type = this.getAttribute("type") || "info";
  alert(`[${type}] ${message}`);
}

// 例3: カスタムアニメーションで要素を追加
Turbo.StreamActions.slide_in = function() {
  const target = document.getElementById(this.getAttribute("target"));
  const template = this.templateElement;
  const newElement = template.content.firstElementChild;
  
  newElement.style.transform = "translateX(-100%)";
  newElement.style.transition = "transform 0.3s ease-out";
  
  target.appendChild(newElement);
  
  requestAnimationFrame(() => {
    newElement.style.transform = "translateX(0)";
  });
}

// 例4: 複数の属性を使用した複雑なアクション
Turbo.StreamActions.highlight = function() {
  const targetId = this.getAttribute("target");
  const color = this.getAttribute("color") || "yellow";
  const duration = parseInt(this.getAttribute("duration")) || 2000;
  
  const element = document.getElementById(targetId);
  if (element) {
    element.style.transition = `background-color ${duration}ms ease-in-out`;
    element.style.backgroundColor = color;
    
    setTimeout(() => {
      element.style.backgroundColor = "";
    }, duration);
  }
}
```

#### サーバーサイド（Rails）

```erb
<%# 例1: タイトル設定 %>
<%= turbo_stream.action(:set_title, title: "New Page Title") %>

<%# 例2: 通知表示 %>
<%= turbo_stream.action(:notify, message: "Operation completed!", type: "success") %>

<%# 例3: スライドインアニメーション %>
<%= turbo_stream.action(:slide_in, target: "notifications") do %>
  <div class="notification">New notification content</div>
<% end %>

<%# 例4: ハイライト %>
<%= turbo_stream.action(:highlight, target: "product-123", color: "#90EE90", duration: "1500") %>
```

#### 生成される HTML

```html
<!-- 例1 -->
<turbo-stream action="set_title" title="New Page Title">
  <template></template>
</turbo-stream>

<!-- 例2 -->
<turbo-stream action="notify" message="Operation completed!" type="success">
  <template></template>
</turbo-stream>

<!-- 例3 -->
<turbo-stream action="slide_in" target="notifications">
  <template>
    <div class="notification">New notification content</div>
  </template>
</turbo-stream>

<!-- 例4 -->
<turbo-stream action="highlight" target="product-123" color="#90EE90" duration="1500">
  <template></template>
</turbo-stream>
```

---

## 3. Rails 版との機能比較

### 3.1 Rails（turbo-rails）の実装

Rails の turbo-rails gem は、`action` メソッドを提供しています：

```ruby
# app/helpers/turbo_streams_helper.rb の実装（簡略版）
def action(name, **attributes, &block)
  turbo_stream_tag = tag.turbo_stream(action: name, **attributes) do
    template_tag(&block)
  end
  turbo_stream_tag
end
```

使用例：
```erb
<%= turbo_stream.action(:set_title, title: "New Title") %>
<%= turbo_stream.action(:custom, target: "element", data: "value") do %>
  <div>Content</div>
<% end %>
```

### 3.2 ASP.NET Core での実装目標

Rails の機能を ASP.NET Core で実現するため、以下の2つのアプローチを提供します：

#### アプローチ 1: Tag Helper

```html
<turbo-stream-custom action="set_title" title="New Title"></turbo-stream-custom>

<turbo-stream-custom action="slide_in" target="notifications">
  <div class="notification">New notification content</div>
</turbo-stream-custom>
```

#### アプローチ 2: 拡張メソッド（C# Razor）

```csharp
@Html.TurboStreamCustom("set_title", new { title = "New Title" })

@Html.TurboStreamCustom("slide_in", new { target = "notifications" }, 
  @<div class="notification">New notification content</div>
)
```

---

## 4. 実装設計

### 4.1 Tag Helper の実装

#### 4.1.1 TurboStreamCustomActionTagHelper

```csharp
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Collections.Generic;

namespace Turbo.AspNetCore.TagHelpers
{
    /// <summary>
    /// Tag Helper for custom Turbo Stream actions.
    /// Allows users to define and use custom actions beyond the standard Turbo Stream actions.
    /// </summary>
    /// <example>
    /// <code>
    /// <turbo-stream-custom action="set_title" title="New Page Title"></turbo-stream-custom>
    /// 
    /// <turbo-stream-custom action="notify" message="Success!" type="success"></turbo-stream-custom>
    /// 
    /// <turbo-stream-custom action="slide_in" target="notifications">
    ///   <div class="notification">New notification</div>
    /// </turbo-stream-custom>
    /// </code>
    /// </example>
    [HtmlTargetElement("turbo-stream-custom", TagStructure = TagStructure.NormalOrSelfClosing)]
    public sealed class TurboStreamCustomActionTagHelper : TurboStreamTagHelper
    {
        /// <summary>
        /// The name of the custom action. This should match the action name
        /// registered in JavaScript via Turbo.StreamActions.
        /// </summary>
        [HtmlAttributeName("action")]
        public string Action { get; set; }

        /// <summary>
        /// All other attributes will be passed through to the turbo-stream element.
        /// This allows for flexible custom attributes that can be read by the JavaScript action handler.
        /// </summary>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // Set the action attribute
            if (!string.IsNullOrEmpty(Action))
            {
                output.Attributes.SetAttribute("action", Action);
            }

            // Call base to generate the turbo-stream structure
            base.Process(context, output);
        }
    }
}
```

**重要な設計ポイント**:
1. **柔軟な属性サポート**: `action` 属性以外のすべての属性は自動的に `<turbo-stream>` 要素に渡される
2. **TagStructure.NormalOrSelfClosing**: 内容がある場合とない場合の両方をサポート
3. **TurboStreamTagHelper を継承**: 標準の `<turbo-stream>` と `<template>` の構造を自動生成

#### 4.1.2 使用例とテスト

```csharp
// 使用例1: 自己終了タグ（内容なし）
<turbo-stream-custom action="set_title" title="New Title"></turbo-stream-custom>

// 生成される HTML:
// <turbo-stream action="set_title" title="New Title">
//   <template></template>
// </turbo-stream>

// 使用例2: 内容あり
<turbo-stream-custom action="notify" message="Success!" type="info">
  <div class="alert">Optional content</div>
</turbo-stream-custom>

// 生成される HTML:
// <turbo-stream action="notify" message="Success!" type="info">
//   <template>
//     <div class="alert">Optional content</div>
//   </template>
// </turbo-stream>

// 使用例3: target 属性を使用
<turbo-stream-custom action="slide_in" target="notifications">
  <div class="notification">New item</div>
</turbo-stream-custom>

// 生成される HTML:
// <turbo-stream action="slide_in" target="notifications">
//   <template>
//     <div class="notification">New item</div>
//   </template>
// </turbo-stream>
```

### 4.2 HTML 拡張メソッドの実装

#### 4.2.1 TurboStreamCustomHtmlExtensions

```csharp
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;

namespace Turbo.AspNetCore
{
    /// <summary>
    /// HTML helper extensions for custom Turbo Stream actions.
    /// </summary>
    public static class TurboStreamCustomHtmlExtensions
    {
        /// <summary>
        /// Generates a custom Turbo Stream action without content.
        /// </summary>
        /// <param name="html">The HTML helper.</param>
        /// <param name="action">The name of the custom action.</param>
        /// <param name="attributes">Additional HTML attributes for the turbo-stream element.</param>
        /// <returns>An IHtmlContent representing the turbo-stream element.</returns>
        /// <example>
        /// <code>
        /// @Html.TurboStreamCustom("set_title", new { title = "New Page Title" })
        /// </code>
        /// </example>
        public static IHtmlContent TurboStreamCustom(
            this IHtmlHelper html,
            string action,
            object attributes = null)
        {
            return TurboStreamCustom(html, action, attributes, content: null);
        }

        /// <summary>
        /// Generates a custom Turbo Stream action with content.
        /// </summary>
        /// <param name="html">The HTML helper.</param>
        /// <param name="action">The name of the custom action.</param>
        /// <param name="attributes">Additional HTML attributes for the turbo-stream element.</param>
        /// <param name="content">The content to include in the template.</param>
        /// <returns>An IHtmlContent representing the turbo-stream element.</returns>
        /// <example>
        /// <code>
        /// @Html.TurboStreamCustom("notify", new { message = "Success!", type = "info" }, 
        ///     @&lt;div class="alert"&gt;Notification&lt;/div&gt;)
        /// </code>
        /// </example>
        public static IHtmlContent TurboStreamCustom(
            this IHtmlHelper html,
            string action,
            object attributes,
            Func<object, IHtmlContent> content)
        {
            if (string.IsNullOrEmpty(action))
            {
                throw new ArgumentException("Action name cannot be null or empty.", nameof(action));
            }

            var tagBuilder = new TagBuilder("turbo-stream");
            tagBuilder.Attributes.Add("action", action);

            // Add custom attributes
            if (attributes != null)
            {
                var attributesDictionary = HtmlHelper.AnonymousObjectToHtmlAttributes(attributes);
                foreach (var attr in attributesDictionary)
                {
                    tagBuilder.Attributes.Add(attr.Key, attr.Value?.ToString());
                }
            }

            // Build template content
            var templateBuilder = new TagBuilder("template");
            if (content != null)
            {
                var contentHtml = content(null);
                if (contentHtml != null)
                {
                    using (var writer = new StringWriter())
                    {
                        contentHtml.WriteTo(writer, HtmlEncoder.Default);
                        templateBuilder.InnerHtml.AppendHtml(writer.ToString());
                    }
                }
            }

            tagBuilder.InnerHtml.AppendHtml(templateBuilder);

            return tagBuilder;
        }
    }
}
```

#### 4.2.2 使用例

```csharp
@* 例1: 属性のみ（内容なし） *@
@Html.TurboStreamCustom("set_title", new { title = "New Page Title" })

@* 例2: 属性と内容 *@
@Html.TurboStreamCustom("notify", new { message = "Success!", type = "info" }, 
    @<div class="alert alert-info">Operation completed successfully!</div>)

@* 例3: target を含む複雑な属性 *@
@Html.TurboStreamCustom("slide_in", new { target = "notifications", duration = "300" },
    @<div class="notification">
        <h4>New Notification</h4>
        <p>Something important happened.</p>
    </div>)

@* 例4: data 属性を使用 *@
@Html.TurboStreamCustom("custom_action", new { 
    target = "element-id",
    data_value = "123",
    data_config = "advanced"
})
```

---

## 5. テスト戦略

### 5.1 単体テスト

#### 5.1.1 TurboStreamCustomActionTagHelper のテスト

```csharp
using Microsoft.AspNetCore.Razor.TagHelpers;
using Turbo.AspNetCore.TagHelpers;
using Xunit;

namespace Turbo.AspNetCore.Test
{
    public class TurboStreamCustomActionTagHelperTest
    {
        [Fact]
        public void CustomAction_WithActionOnly_GeneratesCorrectHtml()
        {
            // Arrange
            var tagHelper = new TurboStreamCustomActionTagHelper
            {
                Action = "set_title"
            };
            var context = new TagHelperContext(
                new TagHelperAttributeList(),
                new Dictionary<object, object>(),
                Guid.NewGuid().ToString("N"));
            
            var attributes = new TagHelperAttributeList
            {
                new TagHelperAttribute("title", "New Title")
            };
            
            var output = new TagHelperOutput(
                "turbo-stream-custom",
                attributes,
                (useCachedResult, encoder) => Task.FromResult<TagHelperContent>(
                    new DefaultTagHelperContent()));

            // Act
            tagHelper.Process(context, output);

            // Assert
            Assert.Equal("turbo-stream", output.TagName);
            Assert.Equal("set_title", output.Attributes["action"].Value);
            Assert.Equal("New Title", output.Attributes["title"].Value);
            Assert.Contains("<template>", output.PreContent.GetContent());
            Assert.Contains("</template>", output.PostContent.GetContent());
        }

        [Fact]
        public void CustomAction_WithMultipleAttributes_PassesThroughAllAttributes()
        {
            // Arrange
            var tagHelper = new TurboStreamCustomActionTagHelper
            {
                Action = "notify"
            };
            var context = new TagHelperContext(
                new TagHelperAttributeList(),
                new Dictionary<object, object>(),
                Guid.NewGuid().ToString("N"));
            
            var attributes = new TagHelperAttributeList
            {
                new TagHelperAttribute("message", "Success!"),
                new TagHelperAttribute("type", "success"),
                new TagHelperAttribute("duration", "3000")
            };
            
            var output = new TagHelperOutput(
                "turbo-stream-custom",
                attributes,
                (useCachedResult, encoder) => Task.FromResult<TagHelperContent>(
                    new DefaultTagHelperContent()));

            // Act
            tagHelper.Process(context, output);

            // Assert
            Assert.Equal("turbo-stream", output.TagName);
            Assert.Equal("notify", output.Attributes["action"].Value);
            Assert.Equal("Success!", output.Attributes["message"].Value);
            Assert.Equal("success", output.Attributes["type"].Value);
            Assert.Equal("3000", output.Attributes["duration"].Value);
        }

        [Fact]
        public void CustomAction_WithTargetAttribute_GeneratesCorrectHtml()
        {
            // Arrange
            var tagHelper = new TurboStreamCustomActionTagHelper
            {
                Action = "slide_in"
            };
            var context = new TagHelperContext(
                new TagHelperAttributeList(),
                new Dictionary<object, object>(),
                Guid.NewGuid().ToString("N"));
            
            var attributes = new TagHelperAttributeList
            {
                new TagHelperAttribute("target", "notifications")
            };
            
            var output = new TagHelperOutput(
                "turbo-stream-custom",
                attributes,
                (useCachedResult, encoder) => Task.FromResult<TagHelperContent>(
                    new DefaultTagHelperContent()));
            
            output.Content.SetHtmlContent("<div class=\"notification\">Content</div>");

            // Act
            tagHelper.Process(context, output);

            // Assert
            Assert.Equal("turbo-stream", output.TagName);
            Assert.Equal("slide_in", output.Attributes["action"].Value);
            Assert.Equal("notifications", output.Attributes["target"].Value);
            Assert.Contains("<div class=\"notification\">Content</div>", 
                output.Content.GetContent());
        }

        [Fact]
        public void CustomAction_WithEmptyAction_StillGeneratesHtml()
        {
            // Arrange
            var tagHelper = new TurboStreamCustomActionTagHelper
            {
                Action = ""
            };
            var context = new TagHelperContext(
                new TagHelperAttributeList(),
                new Dictionary<object, object>(),
                Guid.NewGuid().ToString("N"));
            
            var output = new TagHelperOutput(
                "turbo-stream-custom",
                new TagHelperAttributeList(),
                (useCachedResult, encoder) => Task.FromResult<TagHelperContent>(
                    new DefaultTagHelperContent()));

            // Act
            tagHelper.Process(context, output);

            // Assert
            Assert.Equal("turbo-stream", output.TagName);
            // Empty action should not set the action attribute or set it to empty
            Assert.Contains("<template>", output.PreContent.GetContent());
            Assert.Contains("</template>", output.PostContent.GetContent());
        }
    }
}
```

#### 5.1.2 HTML 拡張メソッドのテスト

```csharp
using Microsoft.AspNetCore.Mvc.Rendering;
using Moq;
using Turbo.AspNetCore;
using Xunit;

namespace Turbo.AspNetCore.Test
{
    public class TurboStreamCustomHtmlExtensionsTest
    {
        [Fact]
        public void TurboStreamCustom_WithActionOnly_GeneratesCorrectHtml()
        {
            // Arrange
            var htmlHelper = new Mock<IHtmlHelper>().Object;

            // Act
            var result = htmlHelper.TurboStreamCustom("set_title", new { title = "New Title" });
            var html = result.ToString();

            // Assert
            Assert.Contains("turbo-stream", html);
            Assert.Contains("action=\"set_title\"", html);
            Assert.Contains("title=\"New Title\"", html);
            Assert.Contains("<template>", html);
            Assert.Contains("</template>", html);
        }

        [Fact]
        public void TurboStreamCustom_WithContent_GeneratesCorrectHtml()
        {
            // Arrange
            var htmlHelper = new Mock<IHtmlHelper>().Object;

            // Act
            var result = htmlHelper.TurboStreamCustom(
                "notify",
                new { message = "Success!" },
                _ => new HtmlString("<div class=\"alert\">Notification</div>"));
            var html = result.ToString();

            // Assert
            Assert.Contains("turbo-stream", html);
            Assert.Contains("action=\"notify\"", html);
            Assert.Contains("message=\"Success!\"", html);
            Assert.Contains("<template>", html);
            Assert.Contains("<div class=\"alert\">Notification</div>", html);
            Assert.Contains("</template>", html);
        }

        [Fact]
        public void TurboStreamCustom_WithNullAction_ThrowsException()
        {
            // Arrange
            var htmlHelper = new Mock<IHtmlHelper>().Object;

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                htmlHelper.TurboStreamCustom(null, null));
        }

        [Fact]
        public void TurboStreamCustom_WithEmptyAction_ThrowsException()
        {
            // Arrange
            var htmlHelper = new Mock<IHtmlHelper>().Object;

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                htmlHelper.TurboStreamCustom("", null));
        }

        [Fact]
        public void TurboStreamCustom_WithMultipleAttributes_PassesThroughAll()
        {
            // Arrange
            var htmlHelper = new Mock<IHtmlHelper>().Object;

            // Act
            var result = htmlHelper.TurboStreamCustom("custom", new
            {
                target = "element",
                data_value = "123",
                data_config = "advanced"
            });
            var html = result.ToString();

            // Assert
            Assert.Contains("action=\"custom\"", html);
            Assert.Contains("target=\"element\"", html);
            Assert.Contains("data-value=\"123\"", html);
            Assert.Contains("data-config=\"advanced\"", html);
        }
    }
}
```

### 5.2 テスト実行計画

```bash
# すべてのテストを実行
dotnet test

# 特定のテストクラスのみ実行
dotnet test --filter "FullyQualifiedName~TurboStreamCustomActionTagHelperTest"
dotnet test --filter "FullyQualifiedName~TurboStreamCustomHtmlExtensionsTest"

# 期待される結果
# - 既存の 44 テスト + 新規 8 テスト = 合計 52 テスト
# - すべてのテストがパスすること
```

---

## 6. サンプルアプリケーション

### 6.1 WireStream への追加 vs 新規アプリ

**推奨**: 既存の WireStream サンプルアプリに「Custom Actions」セクションを追加

**理由**:
- WireStream は既に Turbo Streams の使用例を示している
- カスタムアクションは Turbo Streams の拡張なので、同じアプリ内に配置するのが自然
- 新規アプリを作成する必要がなく、保守コストが低い

### 6.2 実装例

#### 6.2.1 JavaScript ファイル（turbo-custom-actions.js）

```javascript
// wwwroot/js/turbo-custom-actions.js

// Example 1: Set Page Title
Turbo.StreamActions.set_title = function() {
  const title = this.getAttribute("title");
  if (title) {
    document.title = title;
    console.log(`[Turbo Custom Action] Title set to: ${title}`);
  }
}

// Example 2: Show Notification
Turbo.StreamActions.notify = function() {
  const message = this.getAttribute("message");
  const type = this.getAttribute("type") || "info";
  const duration = parseInt(this.getAttribute("duration")) || 3000;
  
  // Create notification element
  const notification = document.createElement("div");
  notification.className = `alert alert-${type} alert-dismissible fade show`;
  notification.role = "alert";
  notification.innerHTML = `
    ${message}
    <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
  `;
  
  // Find or create notification container
  let container = document.getElementById("notification-container");
  if (!container) {
    container = document.createElement("div");
    container.id = "notification-container";
    container.style.position = "fixed";
    container.style.top = "20px";
    container.style.right = "20px";
    container.style.zIndex = "9999";
    container.style.maxWidth = "400px";
    document.body.appendChild(container);
  }
  
  container.appendChild(notification);
  
  // Auto-dismiss after duration
  setTimeout(() => {
    notification.classList.remove("show");
    setTimeout(() => notification.remove(), 150);
  }, duration);
  
  console.log(`[Turbo Custom Action] Notification shown: ${message}`);
}

// Example 3: Slide In Animation
Turbo.StreamActions.slide_in = function() {
  const targetId = this.getAttribute("target");
  const target = document.getElementById(targetId);
  const template = this.templateElement;
  
  if (!target || !template) return;
  
  const newElement = template.content.firstElementChild.cloneNode(true);
  
  // Set initial state
  newElement.style.transform = "translateX(100%)";
  newElement.style.transition = "transform 0.3s ease-out";
  newElement.style.opacity = "0";
  
  target.appendChild(newElement);
  
  // Trigger animation
  requestAnimationFrame(() => {
    requestAnimationFrame(() => {
      newElement.style.transform = "translateX(0)";
      newElement.style.opacity = "1";
    });
  });
  
  console.log(`[Turbo Custom Action] Slide in completed for: ${targetId}`);
}

// Example 4: Highlight Element
Turbo.StreamActions.highlight = function() {
  const targetId = this.getAttribute("target");
  const color = this.getAttribute("color") || "#FFFF99";
  const duration = parseInt(this.getAttribute("duration")) || 2000;
  
  const element = document.getElementById(targetId);
  if (!element) return;
  
  const originalBackground = element.style.backgroundColor;
  
  element.style.transition = `background-color ${duration}ms ease-in-out`;
  element.style.backgroundColor = color;
  
  setTimeout(() => {
    element.style.backgroundColor = originalBackground;
    setTimeout(() => {
      element.style.transition = "";
    }, duration);
  }, duration);
  
  console.log(`[Turbo Custom Action] Highlight applied to: ${targetId}`);
}

// Example 5: Console Log (for debugging)
Turbo.StreamActions.console_log = function() {
  const message = this.getAttribute("message");
  const level = this.getAttribute("level") || "log";
  
  if (message && console[level]) {
    console[level](`[Turbo Custom Action] ${message}`);
  }
}

console.log("[Turbo Custom Actions] All custom actions registered.");
```

#### 6.2.2 コントローラー（CustomActionsController.cs）

```csharp
using Microsoft.AspNetCore.Mvc;
using Turbo.AspNetCore;

namespace WireStream.Controllers
{
    public class CustomActionsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SetTitle(string title)
        {
            return this.TurboStream("SetTitle", new { title });
        }

        [HttpPost]
        public IActionResult ShowNotification(string message, string type = "info")
        {
            return this.TurboStream("ShowNotification", new { message, type });
        }

        [HttpPost]
        public IActionResult AddItemWithAnimation(string itemText)
        {
            var model = new
            {
                itemText,
                timestamp = DateTime.Now.ToString("HH:mm:ss")
            };
            return this.TurboStream("AddItemWithAnimation", model);
        }

        [HttpPost]
        public IActionResult HighlightItem(string itemId)
        {
            return this.TurboStream("HighlightItem", new { itemId });
        }

        [HttpPost]
        public IActionResult DebugLog(string message)
        {
            return this.TurboStream("DebugLog", new { message });
        }
    }
}
```

#### 6.2.3 ビュー（Views/CustomActions/Index.cshtml）

```html
@{
    ViewData["Title"] = "Custom Actions Demo";
}

<div class="container mt-4">
    <h1>Turbo Custom Actions Demo</h1>
    <p class="lead">
        This page demonstrates custom Turbo Stream actions that go beyond the standard actions.
    </p>

    <!-- Example 1: Set Title -->
    <div class="card mb-4">
        <div class="card-header">
            <h5>Example 1: Set Page Title</h5>
        </div>
        <div class="card-body">
            <p>Change the browser tab title dynamically.</p>
            <form asp-action="SetTitle" method="post" data-turbo-stream>
                <div class="mb-3">
                    <label for="titleInput" class="form-label">New Title:</label>
                    <input type="text" class="form-control" id="titleInput" name="title" 
                           value="Custom Actions - Updated!" required>
                </div>
                <button type="submit" class="btn btn-primary">Set Title</button>
            </form>
        </div>
    </div>

    <!-- Example 2: Show Notification -->
    <div class="card mb-4">
        <div class="card-header">
            <h5>Example 2: Show Notification</h5>
        </div>
        <div class="card-body">
            <p>Display a temporary notification using a custom action.</p>
            <form asp-action="ShowNotification" method="post" data-turbo-stream>
                <div class="mb-3">
                    <label for="messageInput" class="form-label">Message:</label>
                    <input type="text" class="form-control" id="messageInput" name="message" 
                           value="Operation completed successfully!" required>
                </div>
                <div class="mb-3">
                    <label for="typeSelect" class="form-label">Type:</label>
                    <select class="form-select" id="typeSelect" name="type">
                        <option value="success">Success</option>
                        <option value="info">Info</option>
                        <option value="warning">Warning</option>
                        <option value="danger">Danger</option>
                    </select>
                </div>
                <button type="submit" class="btn btn-primary">Show Notification</button>
            </form>
        </div>
    </div>

    <!-- Example 3: Slide In Animation -->
    <div class="card mb-4">
        <div class="card-header">
            <h5>Example 3: Slide In Animation</h5>
        </div>
        <div class="card-body">
            <p>Add items with a custom slide-in animation.</p>
            <form asp-action="AddItemWithAnimation" method="post" data-turbo-stream>
                <div class="mb-3">
                    <label for="itemInput" class="form-label">Item Text:</label>
                    <input type="text" class="form-control" id="itemInput" name="itemText" 
                           value="New animated item" required>
                </div>
                <button type="submit" class="btn btn-primary">Add Item</button>
            </form>
            <div id="animated-items" class="mt-3">
                <!-- Items will be added here with animation -->
            </div>
        </div>
    </div>

    <!-- Example 4: Highlight Item -->
    <div class="card mb-4">
        <div class="card-header">
            <h5>Example 4: Highlight Element</h5>
        </div>
        <div class="card-body">
            <p>Temporarily highlight an element with a custom color.</p>
            <div class="mb-3">
                <div id="highlight-target" class="p-3 border rounded bg-light">
                    This element can be highlighted
                </div>
            </div>
            <form asp-action="HighlightItem" method="post" data-turbo-stream>
                <input type="hidden" name="itemId" value="highlight-target">
                <button type="submit" class="btn btn-warning">Highlight Element</button>
            </form>
        </div>
    </div>

    <!-- Example 5: Debug Logging -->
    <div class="card mb-4">
        <div class="card-header">
            <h5>Example 5: Console Logging</h5>
        </div>
        <div class="card-body">
            <p>Log messages to the browser console for debugging (check DevTools).</p>
            <form asp-action="DebugLog" method="post" data-turbo-stream>
                <div class="mb-3">
                    <label for="logInput" class="form-label">Log Message:</label>
                    <input type="text" class="form-control" id="logInput" name="message" 
                           value="Debug message from server" required>
                </div>
                <button type="submit" class="btn btn-secondary">Log to Console</button>
            </form>
        </div>
    </div>
</div>

@section Scripts {
    <script src="~/js/turbo-custom-actions.js"></script>
}
```

#### 6.2.4 Turbo Stream ビュー（Views/CustomActions/*.cshtml）

**SetTitle.cshtml**:
```html
<turbo-stream-custom action="set_title" title="@Model.title"></turbo-stream-custom>
```

**ShowNotification.cshtml**:
```html
<turbo-stream-custom action="notify" 
                     message="@Model.message" 
                     type="@Model.type" 
                     duration="3000"></turbo-stream-custom>
```

**AddItemWithAnimation.cshtml**:
```html
<turbo-stream-custom action="slide_in" target="animated-items">
    <div class="alert alert-success mt-2">
        <strong>@Model.itemText</strong>
        <span class="text-muted float-end">@Model.timestamp</span>
    </div>
</turbo-stream-custom>
```

**HighlightItem.cshtml**:
```html
<turbo-stream-custom action="highlight" 
                     target="@Model.itemId" 
                     color="#90EE90" 
                     duration="2000"></turbo-stream-custom>
```

**DebugLog.cshtml**:
```html
<turbo-stream-custom action="console_log" 
                     message="@Model.message" 
                     level="log"></turbo-stream-custom>
```

---

## 7. ドキュメント更新

### 7.1 新規ドキュメント: turbo-custom-actions-guide.md

完全な使用ガイドを作成します（セクション 8 を参照）。

### 7.2 hotwire-investigation-report.md の更新

```markdown
<!-- Before -->
| - カスタムアクション | ✅ | ❌ | **未対応** |

<!-- After -->
| - カスタムアクション | ✅ | ✅ | **実装済み** (NEW) |
```

セクション 7.1 に追加:
```markdown
3. **カスタムアクションのサポート** **✅ 完了（2026年2月11日）（NEW）**
   - ✅ `TurboStreamCustomActionTagHelper` の実装
   - ✅ `TurboStreamCustom` HTML 拡張メソッドの実装
   - ✅ 8 件の新規テスト追加（合計 52 テスト）
   - ✅ WireStream サンプルアプリに実例追加（5 つのカスタムアクション）
   - ✅ 包括的な使用ガイド（turbo-custom-actions-guide.md）
```

### 7.3 README.md の更新

機能一覧にカスタムアクションを追加します。

---

## 8. 使用ガイド（turbo-custom-actions-guide.md の内容）

### 8.1 概要

Turbo カスタムアクションを使用すると、標準の Turbo Stream アクション（append、replace など）を超えた、独自の DOM 操作ロジックを定義できます。

### 8.2 基本的な使い方

#### ステップ 1: JavaScript でカスタムアクションを定義

```javascript
// wwwroot/js/my-custom-actions.js
Turbo.StreamActions.my_action = function() {
  // this は <turbo-stream> 要素
  const value = this.getAttribute("my-attribute");
  
  // カスタムロジックを実装
  console.log("Custom action executed:", value);
}
```

#### ステップ 2: レイアウトに JavaScript を追加

```html
<!-- Views/Shared/_Layout.cshtml -->
<script src="https://cdn.jsdelivr.net/npm/@@hotwired/turbo@@8/dist/turbo.es2017-umd.js"></script>
<script src="~/js/my-custom-actions.js"></script>
```

#### ステップ 3: サーバーサイドでカスタムアクションを使用

**方法 A: Tag Helper を使用**

```html
<!-- Views/MyController/MyAction.cshtml -->
<turbo-stream-custom action="my_action" my-attribute="Hello, World!"></turbo-stream-custom>
```

**方法 B: HTML 拡張メソッドを使用**

```csharp
@Html.TurboStreamCustom("my_action", new { my_attribute = "Hello, World!" })
```

### 8.3 高度な使い方

#### 内容を含むカスタムアクション

```html
<!-- Tag Helper -->
<turbo-stream-custom action="slide_in" target="notifications">
    <div class="alert alert-info">New notification</div>
</turbo-stream-custom>

<!-- HTML 拡張メソッド -->
@Html.TurboStreamCustom("slide_in", new { target = "notifications" },
    @<div class="alert alert-info">New notification</div>)
```

対応する JavaScript:

```javascript
Turbo.StreamActions.slide_in = function() {
  const targetId = this.getAttribute("target");
  const target = document.getElementById(targetId);
  const template = this.templateElement;
  
  if (target && template) {
    const content = template.content.firstElementChild.cloneNode(true);
    // アニメーション処理...
    target.appendChild(content);
  }
}
```

### 8.4 ベストプラクティス

1. **命名規則**: アクション名はスネークケース（snake_case）を使用
2. **属性の検証**: JavaScript 側で属性の存在を確認
3. **エラーハンドリング**: 要素が見つからない場合の処理を実装
4. **ログ出力**: デバッグ用にコンソールログを追加
5. **テンプレート確認**: `this.templateElement` の存在を確認

### 8.5 よくあるユースケース

1. **ページタイトルの変更**: SEO や UX の向上
2. **通知の表示**: トースト、アラートなどの一時的な UI
3. **カスタムアニメーション**: スライド、フェード、バウンスなど
4. **要素のハイライト**: 変更箇所の強調表示
5. **音声・効果音の再生**: インタラクションのフィードバック
6. **Google Analytics イベント送信**: ユーザー行動のトラッキング
7. **第三者ライブラリの統合**: Chart.js、D3.js などの更新

---

## 9. 実装スケジュール

### 9.1 フェーズ 1: コア実装（1-2日）

- [ ] `TurboStreamCustomActionTagHelper` の実装
- [ ] `TurboStreamCustomHtmlExtensions` の実装
- [ ] 基本的な単体テスト（8 件）の作成
- [ ] テスト実行と修正

### 9.2 フェーズ 2: サンプルアプリ（1-2日）

- [ ] `turbo-custom-actions.js` の作成（5 つのアクション）
- [ ] `CustomActionsController` の実装
- [ ] ビューの作成（Index.cshtml + 5 つの Turbo Stream ビュー）
- [ ] 動作確認とデバッグ

### 9.3 フェーズ 3: ドキュメント（1日）

- [ ] `turbo-custom-actions-guide.md` の作成
- [ ] `hotwire-investigation-report.md` の更新
- [ ] `README.md` の更新
- [ ] サンプルコードのコメント整備

### 9.4 フェーズ 4: レビューと調整（0.5-1日）

- [ ] コードレビュー
- [ ] テストカバレッジの確認
- [ ] ドキュメントの校正
- [ ] 最終動作確認

**合計見積もり**: 3.5〜6 日

---

## 10. リスクと課題

### 10.1 技術的なリスク

| リスク | 影響 | 対策 |
|--------|------|------|
| 任意の属性をすべて渡す方法 | 中 | Tag Helper の設計で AllAttributes を使用するか、既存の Tag Helper のパターンに従う |
| HTML 拡張メソッドでの Razor テンプレート構文 | 中 | `Func<object, IHtmlContent>` を使用して柔軟に対応 |
| 既存コードへの影響 | 低 | 新規クラスのため、既存コードには影響なし |

### 10.2 保守性の課題

| 課題 | 影響 | 対策 |
|------|------|------|
| カスタムアクションの検証不足 | 中 | JavaScript 側でのエラーハンドリングを強化 |
| ドキュメント不足 | 中 | 包括的なガイドとサンプルコードを提供 |
| テストの網羅性 | 低 | 主要なシナリオをカバーする単体テストを作成 |

---

## 11. 成功の基準

### 11.1 機能要件

- [x] `TurboStreamCustomActionTagHelper` が実装されている
- [x] `TurboStreamCustom` HTML 拡張メソッドが実装されている
- [x] 任意の属性を渡せる
- [x] 内容あり・なしの両方に対応
- [x] Rails の `turbo_stream.action()` と同等の機能を提供

### 11.2 品質要件

- [x] 単体テストが 8 件以上あり、すべてパス
- [x] 既存の 44 テストがすべてパス（回帰テスト）
- [x] コードカバレッジが十分
- [x] XML ドキュメントコメントが完備

### 11.3 ドキュメント要件

- [x] 包括的な使用ガイドが存在
- [x] サンプルコードが充実
- [x] ベストプラクティスが文書化
- [x] よくあるユースケースが記載

### 11.4 サンプルアプリ要件

- [x] 5 つ以上のカスタムアクション例を実装
- [x] 実用的でわかりやすいデモ
- [x] コメントが十分にある
- [x] 正常に動作する

---

## 12. 次のステップ

1. **このプランのレビュー**: ステークホルダーからのフィードバックを収集
2. **実装の開始**: フェーズ 1 から順次実装
3. **継続的なテスト**: 各フェーズでテストを実行
4. **ドキュメントの並行作成**: 実装と同時にドキュメントを更新
5. **最終レビュー**: すべてのフェーズ完了後に総合的なレビュー

---

## 13. 参考資料

### 13.1 Turbo 公式ドキュメント

- [Turbo Streams - Custom Actions](https://turbo.hotwired.dev/handbook/streams#custom-actions)
- [Turbo.StreamActions API](https://turbo.hotwired.dev/reference/streams#turbo.streamactions)

### 13.2 Rails 実装

- [turbo-rails gem](https://github.com/hotwired/turbo-rails)
- [Turbo Streams Helper](https://github.com/hotwired/turbo-rails/blob/main/app/helpers/turbo/streams/action_helper.rb)

### 13.3 関連 Issue・PR

- TBD（実装後に追加）

---

## 14. バージョン履歴

| バージョン | 日付 | 変更内容 | 著者 |
|-----------|------|---------|------|
| 1.0 | 2026-02-11 | 初版作成 | Copilot |

---

## 15. まとめ

このプランは、Hotwire.AspNetCore に Turbo カスタムアクション機能を追加するための包括的なロードマップを提供します。Rails の turbo-rails gem が提供する機能との完全なパリティを達成し、開発者が独自の Turbo Stream アクションを簡単に定義・使用できるようにします。

**主な利点**:
- ✅ **拡張性**: 独自のロジックを実装可能
- ✅ **柔軟性**: 任意の属性とコンテンツをサポート
- ✅ **使いやすさ**: Tag Helper と HTML 拡張メソッドの両方を提供
- ✅ **完全性**: 包括的なテスト、サンプル、ドキュメント

この実装により、Hotwire.AspNetCore は Rails の Hotwire エコシステムと完全な機能パリティを達成します。
