# [Feature] Stimulus.AspNetCore の実装

## 概要

ASP.NET Core で Stimulus.js を効率的に使用するためのサーバーサイドヘルパーを実装します。Tag Helpers と拡張メソッドにより、開発者が最小限の JavaScript でインタラクティブな UI を構築できるようにします。

## 背景

現在、Hotwire.AspNetCore は Turbo Drive、Turbo Frames、Turbo Streams を完全にサポートしていますが、Stimulus.js の統合は未実装です。Stimulus は Hotwire エコシステムの重要な部分であり、JavaScript を最小限にして DOM 操作を実現します。

関連ドキュメント:
- [調査レポート](docs/hotwire-investigation-report.md) - Section 4.2.A
- [実装計画](docs/stimulus-implementation-plan.md)

## 目標

- ✅ Stimulus data 属性を生成する Tag Helper の実装
- ✅ 型安全な HTML 拡張メソッドの提供
- ✅ Rails の stimulus-rails gem との設計思想の一致
- ✅ 実用的なサンプルアプリケーション（WireStimulus）の作成
- ✅ 包括的なドキュメントとテスト

## 実装内容

### Phase 1: コア Tag Helper（優先度: 高）

#### 1.1 StimulusControllerTagHelper

```csharp
[HtmlTargetElement(Attributes = "stimulus-controller")]
public class StimulusControllerTagHelper : TagHelper
{
    [HtmlAttributeName("stimulus-controller")]
    public string Controller { get; set; } = string.Empty;
    
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (!string.IsNullOrWhiteSpace(Controller))
        {
            output.Attributes.SetAttribute("data-controller", Controller.Trim());
        }
        output.Attributes.RemoveAll("stimulus-controller");
    }
}
```

**使用例**:
```html
<div stimulus-controller="dropdown">
    <!-- 出力: <div data-controller="dropdown"> -->
```

**タスク**:
- [ ] `StimulusControllerTagHelper` の実装
- [ ] 複数コントローラー対応（スペース区切り）
- [ ] 単体テストの作成（5 ケース以上）

#### 1.2 StimulusActionTagHelper

```csharp
[HtmlTargetElement(Attributes = "stimulus-action")]
public class StimulusActionTagHelper : TagHelper
{
    [HtmlAttributeName("stimulus-action")]
    public string Action { get; set; } = string.Empty;
    
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        // 実装詳細は docs/stimulus-implementation-plan.md を参照
    }
}
```

**使用例**:
```html
<button stimulus-action="click->dropdown#toggle">Toggle</button>
<!-- 出力: <button data-action="click->dropdown#toggle">Toggle</button> -->
```

**タスク**:
- [ ] `StimulusActionTagHelper` の実装
- [ ] 既存 `data-action` との統合
- [ ] 複数アクション対応
- [ ] 単体テストの作成（7 ケース以上）

#### 1.3 StimulusTargetTagHelper

```csharp
[HtmlTargetElement(Attributes = "stimulus-target")]
public class StimulusTargetTagHelper : TagHelper
{
    [HtmlAttributeName("stimulus-target")]
    public string Target { get; set; } = string.Empty;
    
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        // フォーマット: "controller.target" または複数対応
        // 実装詳細は docs/stimulus-implementation-plan.md を参照
    }
}
```

**使用例**:
```html
<div stimulus-target="dropdown.menu">Menu</div>
<!-- 出力: <div data-dropdown-target="menu">Menu</div> -->
```

**タスク**:
- [ ] `StimulusTargetTagHelper` の実装
- [ ] "controller.target" 形式のパース
- [ ] 複数ターゲット対応
- [ ] 異なるコントローラーのターゲット対応
- [ ] 単体テストの作成（8 ケース以上）

---

### Phase 2: 拡張 Tag Helper（優先度: 中）

#### 2.1 StimulusValueTagHelper

動的な Stimulus 値属性を生成します。

```csharp
[HtmlTargetElement(Attributes = "stimulus-value-*")]
public class StimulusValueTagHelper : TagHelper
{
    [HtmlAttributeName("stimulus-value-", DictionaryAttributePrefix = "stimulus-value-")]
    public IDictionary<string, string> Values { get; }
    
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        // stimulus-value-dropdown-open="false" → data-dropdown-open-value="false"
        // 実装詳細は docs/stimulus-implementation-plan.md を参照
    }
}
```

**使用例**:
```html
<div stimulus-value-dropdown-open="false">
    <!-- 出力: <div data-dropdown-open-value="false"> -->
```

**タスク**:
- [ ] `StimulusValueTagHelper` の実装
- [ ] 複数値の同時設定対応
- [ ] Boolean/数値/文字列/JSON 対応
- [ ] 単体テストの作成（10 ケース以上）

#### 2.2 StimulusClassTagHelper

CSS クラス名を動的に参照します。

```csharp
[HtmlTargetElement(Attributes = "stimulus-class-*")]
public class StimulusClassTagHelper : TagHelper
{
    [HtmlAttributeName("stimulus-class-", DictionaryAttributePrefix = "stimulus-class-")]
    public IDictionary<string, string> Classes { get; }
    
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        // stimulus-class-dropdown-active="show" → data-dropdown-active-class="show"
    }
}
```

**使用例**:
```html
<div stimulus-class-dropdown-active="show">
    <!-- 出力: <div data-dropdown-active-class="show"> -->
```

**タスク**:
- [ ] `StimulusClassTagHelper` の実装
- [ ] 複数クラス設定対応
- [ ] 単体テストの作成（5 ケース以上）

---

### Phase 3: HTML 拡張メソッド（優先度: 中）

#### 3.1 StimulusHtmlExtensions

プログラムで Stimulus 属性を生成する拡張メソッド。

```csharp
public static class StimulusHtmlExtensions
{
    public static IDictionary<string, object> StimulusController(
        this IHtmlHelper html, 
        string controller);
    
    public static IDictionary<string, object> StimulusAction(
        this IHtmlHelper html, 
        string action);
    
    public static IDictionary<string, object> StimulusTarget(
        this IHtmlHelper html, 
        string controller, 
        string target);
    
    public static IDictionary<string, object> StimulusValue(
        this IHtmlHelper html, 
        string controller, 
        string name, 
        object value);
    
    public static IDictionary<string, object> StimulusClass(
        this IHtmlHelper html, 
        string controller, 
        string name, 
        string className);
    
    public static IDictionary<string, object> StimulusAttributes(
        this IHtmlHelper html,
        params IDictionary<string, object>[] attributeSets);
}
```

**使用例**:
```csharp
@Html.TextBoxFor(m => m.Name, Html.StimulusAttributes(
    Html.StimulusController("form"),
    Html.StimulusTarget("form", "name"),
    Html.StimulusAction("blur->form#validate")
))
```

**タスク**:
- [ ] すべての拡張メソッドの実装
- [ ] 属性の統合機能（`StimulusAttributes`）
- [ ] 単体テストの作成（15 ケース以上）

---

### Phase 4: サンプルアプリケーション（優先度: 高）

#### 4.1 WireStimulus プロジェクト

実用的な Stimulus.AspNetCore の使用例を示すサンプルアプリ。

**デモ機能**:
1. **Dropdown Controller** - クリックでメニューを開閉
2. **Clipboard Controller** - テキストをクリップボードにコピー
3. **Counter Controller** - インクリメント/デクリメント
4. **Form Validation Controller** - リアルタイムバリデーション
5. **Slideshow Controller** - 画像スライドショー

**タスク**:
- [ ] WireStimulus プロジェクトの作成（net8.0）
- [ ] 5 つの Controller とそれぞれの View の実装
- [ ] Stimulus.js の統合（npm + bundler）
- [ ] スタイリング（Bootstrap または Tailwind）
- [ ] README.md の作成

---

### Phase 5: テスト（優先度: 高）

#### 5.1 単体テスト

**ファイル**: `test/Stimulus.AspNetCore.Test/StimulusTagHelperTest.cs`

**テストケース**:
- [ ] StimulusControllerTagHelper（5 テスト）
  - 単一コントローラー
  - 複数コントローラー
  - 空文字列の処理
  - 前後の空白の処理
  - 既存属性との競合
- [ ] StimulusActionTagHelper（7 テスト）
  - 単一アクション
  - 複数アクション
  - 既存 data-action との統合
  - グローバルイベント
  - イベント省略形
  - 空文字列の処理
  - フォーマット検証
- [ ] StimulusTargetTagHelper（8 テスト）
  - "controller.target" 形式
  - 複数ターゲット
  - 異なるコントローラー
  - 既存ターゲットとの統合
  - 不正なフォーマットの処理
  - 空文字列の処理
- [ ] StimulusValueTagHelper（10 テスト）
  - Boolean 値
  - 数値
  - 文字列
  - JSON オブジェクト
  - 複数値の同時設定
  - 空文字列の処理
- [ ] StimulusClassTagHelper（5 テスト）
  - 単一クラス
  - 複数クラス
  - 空文字列の処理
- [ ] StimulusHtmlExtensions（15 テスト）
  - 各拡張メソッドの動作検証
  - 属性統合のテスト

**合計**: 50 テスト以上

**タスク**:
- [ ] テストプロジェクトの作成
- [ ] すべてのテストケースの実装
- [ ] テストカバレッジ 90% 以上を達成

#### 5.2 統合テスト

**オプション**: Selenium/Playwright を使用したブラウザテスト

---

### Phase 6: ドキュメント（優先度: 高）

#### 6.1 必要なドキュメント

| ドキュメント | ファイル名 | 内容 |
|----------|----------|------|
| README | `README.md` | プロジェクト概要とクイックスタート |
| 使用ガイド | `docs/stimulus-guide.md` | Tag Helper と拡張メソッドの詳細 |
| API リファレンス | `docs/stimulus-api-reference.md` | すべての Tag Helper と拡張メソッド |
| サンプル集 | `docs/stimulus-examples.md` | 実用的なコード例 |
| Rails 移行 | `docs/stimulus-rails-migration.md` | Rails との違いと移行方法 |

**タスク**:
- [ ] README.md の作成
- [ ] stimulus-guide.md の作成
- [ ] stimulus-api-reference.md の作成
- [ ] stimulus-examples.md の作成
- [ ] stimulus-rails-migration.md の作成

---

## 成功基準

### 機能要件
- [ ] すべての Tag Helper が正しく動作
- [ ] HTML 拡張メソッドが型安全に動作
- [ ] WireStimulus サンプルアプリが 5 つの Controller を含む
- [ ] すべてのテストがパス（50 テスト以上）

### 品質要件
- [ ] テストカバレッジ 90% 以上
- [ ] ドキュメントが完備（5 ドキュメント以上）
- [ ] コードレビューで指摘事項なし
- [ ] セキュリティ脆弱性なし（XSS 対策）

### 互換性要件
- [ ] netstandard2.0 をターゲット
- [ ] .NET 6/8/9/10 で動作確認
- [ ] Rails の stimulus-rails との概念的な互換性

---

## 実装スケジュール

| Phase | 期間 | マイルストーン |
|-------|------|-----------|
| Phase 1 | Week 1-2 | コア Tag Helper 完成 |
| Phase 2 | Week 3-4 | 拡張 Tag Helper 完成 |
| Phase 3 | Week 3-4 | HTML 拡張メソッド完成 |
| Phase 4 | Week 5-6 | WireStimulus サンプル完成 |
| Phase 5 | Week 1-7 | 単体テスト完成（並行作業） |
| Phase 6 | Week 7-8 | ドキュメント完成 |

**合計**: 8 週間

---

## 技術的考慮事項

### セキュリティ
- [ ] XSS 対策: すべての属性値を自動エスケープ
- [ ] CSP 互換性: inline script を使用しない
- [ ] 入力検証: 不正なフォーマットの処理

### パフォーマンス
- [ ] Tag Helper は軽量（Reflection を最小化）
- [ ] 属性の統合は効率的なアルゴリズム
- [ ] メモリリークなし

### 保守性
- [ ] 明確な命名規則
- [ ] コメントとドキュメント
- [ ] 一貫したコーディングスタイル

---

## 依存関係

### パッケージ依存
- `Microsoft.AspNetCore.Razor.TagHelpers` (既存)
- `Microsoft.AspNetCore.Mvc.ViewFeatures` (既存)

### 外部依存
- Stimulus.js (`@hotwired/stimulus`) - クライアント側ライブラリ（npm）

---

## 参考資料

### 公式ドキュメント
- [Stimulus Handbook](https://stimulus.hotwired.dev/handbook/introduction)
- [Stimulus Reference](https://stimulus.hotwired.dev/reference/)
- [stimulus-rails GitHub](https://github.com/hotwired/stimulus-rails)

### 内部ドキュメント
- [実装計画](docs/stimulus-implementation-plan.md) - 詳細な実装仕様
- [調査レポート](docs/hotwire-investigation-report.md) - 現状分析

---

## リスクと対策

### リスク 1: Tag Helper の複雑性
**対策**: シンプルな API 設計、単一責任の原則

### リスク 2: Rails との互換性
**対策**: stimulus-rails のソースコードを参考に、同様の API 設計

### リスク 3: テストの不足
**対策**: TDD (Test-Driven Development) アプローチ、50 テスト以上

---

## チェックリスト

### 実装
- [ ] Phase 1: コア Tag Helper（3 個）
- [ ] Phase 2: 拡張 Tag Helper（2 個）
- [ ] Phase 3: HTML 拡張メソッド（6 個）
- [ ] Phase 4: WireStimulus サンプルアプリ（5 Controller）

### テスト
- [ ] 単体テスト 50 以上
- [ ] テストカバレッジ 90% 以上
- [ ] すべてのテストがパス

### ドキュメント
- [ ] README.md
- [ ] stimulus-guide.md
- [ ] stimulus-api-reference.md
- [ ] stimulus-examples.md
- [ ] stimulus-rails-migration.md

### 品質保証
- [ ] コードレビュー完了
- [ ] セキュリティ検証完了
- [ ] パフォーマンステスト完了

---

## 関連 Issue

- 関連: #XXX (Turbo Drive 実装)
- 関連: #XXX (Turbo Streams + SignalR 実装)
- ブロック: なし
- ブロック先: #XXX (Hotwire.AspNetCore v2.0 リリース)

---

**作成日**: 2026年2月11日  
**担当者**: TBD  
**ラベル**: `enhancement`, `stimulus`, `tag-helpers`  
**マイルストーン**: Hotwire.AspNetCore v2.0
