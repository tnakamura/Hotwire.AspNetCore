# Stimulus.AspNetCore 実装計画 - 完了サマリー

**作成日**: 2026年2月11日  
**ステータス**: ✅ 計画書作成完了

---

## 概要

`docs/hotwire-investigation-report.md` の調査結果に基づき、Stimulus.AspNetCore の完全な実装計画を作成しました。これにより、ASP.NET Core で Stimulus.js を効率的に使用するためのサーバーサイドヘルパーの実装方針が明確になりました。

---

## 作成したドキュメント

### 1. stimulus-implementation-plan.md (42KB, 1,374行)
**詳細な実装計画書**

**内容**:
- ✅ エグゼクティブサマリー
- ✅ Stimulus.js の基本概念（Controller、Target、Action、Value、Class）
- ✅ アーキテクチャ設計（コンポーネント構成）
- ✅ 5つの Tag Helper の完全な実装仕様とコード例
  - `StimulusControllerTagHelper`
  - `StimulusActionTagHelper`
  - `StimulusTargetTagHelper`
  - `StimulusValueTagHelper`
  - `StimulusClassTagHelper`
- ✅ HTML 拡張メソッドの設計（6メソッド）
- ✅ WireStimulus サンプルアプリの詳細設計（5 Controller）
  - Dropdown Controller（メニュー開閉）
  - Clipboard Controller（クリップボードコピー）
  - Counter Controller（カウンター）
  - Form Validation Controller（リアルタイムバリデーション）
  - Slideshow Controller（スライドショー）
- ✅ テスト戦略（50+ 単体テスト）
- ✅ ドキュメント構成（5ドキュメント）
- ✅ 実装スケジュール（8週間、Phase 1-6）
- ✅ Rails の stimulus-rails との比較
- ✅ セキュリティ考慮事項

**使用例**:
```html
<!-- Tag Helpers を使用 -->
<div stimulus-controller="dropdown">
    <button stimulus-action="click->dropdown#toggle">Toggle</button>
    <div stimulus-target="dropdown.menu">Menu</div>
</div>

<!-- 出力 HTML -->
<div data-controller="dropdown">
    <button data-action="click->dropdown#toggle">Toggle</button>
    <div data-dropdown-target="menu">Menu</div>
</div>
```

### 2. stimulus-github-issue.md (14KB, 468行)
**GitHub Issue テンプレート**

**内容**:
- ✅ 概要と背景
- ✅ 実装内容（Phase 1-6 に分割）
  - Phase 1: コア Tag Helper（3個）- Week 1-2
  - Phase 2: 拡張 Tag Helper（2個）- Week 3-4
  - Phase 3: HTML 拡張メソッド（6個）- Week 3-4
  - Phase 4: WireStimulus サンプルアプリ - Week 5-6
  - Phase 5: テスト（50+ テスト）- Week 1-7（並行）
  - Phase 6: ドキュメント（5ドキュメント）- Week 7-8
- ✅ 成功基準
  - 機能要件（すべての Tag Helper が動作）
  - 品質要件（カバレッジ 90%以上）
  - 互換性要件（netstandard2.0、Rails 互換）
- ✅ 詳細なチェックリスト
- ✅ 技術的考慮事項（セキュリティ、パフォーマンス、保守性）
- ✅ リスクと対策

**使用方法**:
1. GitHub Repository の Issues セクションで新しい Issue を作成
2. このファイルの内容をコピー＆ペースト
3. 必要に応じてカスタマイズして投稿

### 3. STIMULUS_PLAN_README.md (8.2KB, 277行)
**計画ドキュメント使用ガイド**

**内容**:
- ✅ ドキュメント一覧と説明
- ✅ クイックスタート手順
- ✅ 実装手順（Phase 1-5）
- ✅ 進捗管理のチェックリスト例
- ✅ コーディング規約（命名規則、ファイル構成）
- ✅ 参考資料リンク
- ✅ よくある質問（FAQ）
- ✅ トラブルシューティング

**使用方法**:
実装を開始する前に、このドキュメントを読んで全体像を把握してください。

---

## 実装計画サマリー

### Tag Helpers（5個）

| Tag Helper | 機能 | 使用例 |
|-----------|------|--------|
| **StimulusControllerTagHelper** | コントローラーを接続 | `<div stimulus-controller="dropdown">` |
| **StimulusActionTagHelper** | イベントをメソッドにバインド | `<button stimulus-action="click->dropdown#toggle">` |
| **StimulusTargetTagHelper** | 要素を参照 | `<div stimulus-target="dropdown.menu">` |
| **StimulusValueTagHelper** | 値を渡す | `<div stimulus-value-dropdown-open="false">` |
| **StimulusClassTagHelper** | CSS クラス名を参照 | `<div stimulus-class-dropdown-active="show">` |

### HTML 拡張メソッド（6個）

```csharp
// コントローラー
Html.StimulusController("dropdown")

// アクション
Html.StimulusAction("click->dropdown#toggle")

// ターゲット
Html.StimulusTarget("dropdown", "menu")

// 値
Html.StimulusValue("dropdown", "open", false)

// クラス
Html.StimulusClass("dropdown", "active", "show")

// 複数属性の統合
Html.StimulusAttributes(
    Html.StimulusController("dropdown"),
    Html.StimulusAction("click->dropdown#toggle")
)
```

### WireStimulus サンプルアプリ（5 Controller）

| Controller | 機能 | デモ内容 |
|-----------|------|---------|
| **Dropdown** | メニュー開閉 | クリックでドロップダウンメニューを表示/非表示 |
| **Clipboard** | クリップボードコピー | テキストをクリップボードにコピー |
| **Counter** | カウンター | インクリメント/デクリメント/リセット |
| **Form Validation** | フォームバリデーション | リアルタイムで入力を検証 |
| **Slideshow** | スライドショー | 画像を自動または手動で切り替え |

### テスト（50+ テスト）

| テスト対象 | テスト数 |
|----------|---------|
| StimulusControllerTagHelper | 5 |
| StimulusActionTagHelper | 7 |
| StimulusTargetTagHelper | 8 |
| StimulusValueTagHelper | 10 |
| StimulusClassTagHelper | 5 |
| StimulusHtmlExtensions | 15 |
| **合計** | **50+** |

**カバレッジ目標**: 90% 以上

### ドキュメント（5ドキュメント）

| ドキュメント | 内容 |
|----------|------|
| `README.md` | プロジェクト概要とクイックスタート |
| `docs/stimulus-guide.md` | Tag Helper と拡張メソッドの詳細 |
| `docs/stimulus-api-reference.md` | API リファレンス |
| `docs/stimulus-examples.md` | 実用的なコード例 |
| `docs/stimulus-rails-migration.md` | Rails からの移行ガイド |

---

## 実装スケジュール

| Phase | 期間 | 内容 |
|-------|------|------|
| **Phase 1** | Week 1-2 | コア Tag Helper（3個）+ テスト |
| **Phase 2** | Week 3-4 | 拡張 Tag Helper（2個）+ テスト |
| **Phase 3** | Week 3-4 | HTML 拡張メソッド（6個）+ テスト |
| **Phase 4** | Week 5-6 | WireStimulus サンプルアプリ（5 Controller） |
| **Phase 5** | Week 1-7 | テスト（並行作業） |
| **Phase 6** | Week 7-8 | ドキュメント（5ドキュメント） |

**合計**: 8週間

---

## Rails (stimulus-rails) との比較

### 機能パリティ

| 機能 | Rails | ASP.NET Core |
|-----|-------|-------------|
| コントローラー自動読み込み | ✅ | ⚠️ 手動 |
| Helper メソッド | ✅ | ✅ Tag Helpers |
| npm/yarn 統合 | ✅ | ✅ |
| Hot Module Replacement | ✅ | ⚠️ 手動リロード |

### 使用例比較

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

**結果**: 同じ HTML が生成されます！

---

## セキュリティ考慮事項

### XSS 対策
- ✅ Tag Helper は自動的に属性値をエスケープ
- ✅ JSON 値を渡す場合は適切にエスケープ

### CSP 互換性
- ✅ Stimulus.js は inline script を使用しない
- ✅ 厳格な Content Security Policy と互換

---

## 次のステップ

### 実装を開始する前に

1. **調査レポートを確認**
   ```bash
   cat docs/hotwire-investigation-report.md | grep -A 20 "Stimulus"
   ```

2. **実装計画を熟読**
   ```bash
   cat docs/stimulus-implementation-plan.md
   ```

3. **既存の実装パターンを確認**
   ```bash
   cat src/Turbo.AspNetCore/TagHelpers/*.cs
   ```

### GitHub Issue の作成

1. GitHub Repository → Issues → New Issue
2. `docs/stimulus-github-issue.md` の内容をコピー
3. カスタマイズして投稿

### 実装開始

Phase 1 から順番に実装を開始してください：

```bash
# 1. Tag Helper ディレクトリを作成
mkdir -p src/Stimulus.AspNetCore/TagHelpers

# 2. StimulusControllerTagHelper から実装開始
touch src/Stimulus.AspNetCore/TagHelpers/StimulusControllerTagHelper.cs

# 3. テストを並行して作成
mkdir -p test/Stimulus.AspNetCore.Test
touch test/Stimulus.AspNetCore.Test/StimulusControllerTagHelperTest.cs
```

---

## 参考資料

### 公式ドキュメント
- [Stimulus Handbook](https://stimulus.hotwired.dev/handbook/introduction)
- [Stimulus Reference](https://stimulus.hotwired.dev/reference/)
- [stimulus-rails GitHub](https://github.com/hotwired/stimulus-rails)
- [ASP.NET Core Tag Helpers](https://learn.microsoft.com/en-us/aspnet/core/mvc/views/tag-helpers/intro)

### 内部リソース
- `docs/hotwire-investigation-report.md` - 調査レポート（Section 4.2.A）
- `src/Turbo.AspNetCore/TagHelpers/` - 既存の Tag Helper 実装
- `examples/WireFrame/`, `examples/WireStream/` - 既存のサンプルアプリ

---

## まとめ

✅ **完了した作業**:
- Stimulus.AspNetCore の詳細な実装計画書（42KB、1,374行）
- GitHub Issue テンプレート（14KB、468行）
- 計画ドキュメント使用ガイド（8.2KB、277行）

📝 **計画内容**:
- 5つの Tag Helper の完全な実装仕様
- 6つの HTML 拡張メソッドの設計
- WireStimulus サンプルアプリ（5 Controller）
- 50+ 単体テストの戦略
- 5つのドキュメント構成
- 8週間の実装スケジュール

🎯 **次のステップ**:
1. GitHub Issue を作成
2. Phase 1（コア Tag Helper）の実装を開始
3. テストを並行して作成
4. WireStimulus サンプルアプリを実装
5. ドキュメントを作成

---

**作成者**: GitHub Copilot Agent  
**作成日**: 2026年2月11日  
**ステータス**: ✅ 完了
