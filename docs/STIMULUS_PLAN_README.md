# Stimulus.AspNetCore 実装計画ドキュメント

このディレクトリには、Stimulus.AspNetCore の実装に関する計画ドキュメントが含まれています。

## ドキュメント一覧

### 1. [stimulus-implementation-plan.md](stimulus-implementation-plan.md)
**詳細な実装計画書**

このドキュメントは、Stimulus.AspNetCore の完全な実装仕様を提供します。

**内容**:
- エグゼクティブサマリー
- Stimulus.js の基本概念
- アーキテクチャ設計
- 実装詳細（Tag Helper、拡張メソッド）
- サンプルアプリケーション（WireStimulus）の設計
- テスト戦略
- ドキュメント構成
- 実装スケジュール
- Rails との比較
- セキュリティ考慮事項

**対象読者**: 実装担当者、アーキテクト

**使用方法**:
1. まずこのドキュメントを読んで全体像を把握
2. 各 Phase の実装詳細を参考にコーディング
3. コード例を活用して実装を進める

### 2. [stimulus-github-issue.md](stimulus-github-issue.md)
**GitHub Issue テンプレート**

GitHub の Issue として投稿するための形式化されたテンプレートです。

**内容**:
- 概要と背景
- 実装内容（Phase 別）
- 成功基準
- 実装スケジュール
- 技術的考慮事項
- チェックリスト

**対象読者**: プロジェクトマネージャー、実装担当者

**使用方法**:
1. GitHub Repository の Issues セクションに移動
2. 新しい Issue を作成
3. このファイルの内容をコピー＆ペースト
4. 必要に応じてカスタマイズ

### 3. [hotwire-investigation-report.md](hotwire-investigation-report.md)
**Hotwire 調査レポート（参照）**

既存のレポート。Stimulus に関する調査結果が含まれています。

**関連セクション**:
- Section 4.2.A: Stimulus.js の統合（実装案）
- Section 7.2: ロードマップ（長期計画）

---

## クイックスタート

### 実装を開始する前に

1. **調査レポートを確認**
   ```bash
   # Section 4.2.A と Section 7.2 を確認
   cat docs/hotwire-investigation-report.md | grep -A 20 "Stimulus"
   ```

2. **実装計画を熟読**
   ```bash
   cat docs/stimulus-implementation-plan.md
   ```

3. **既存の実装パターンを確認**
   ```bash
   # Turbo.AspNetCore の Tag Helper を参考にする
   cat src/Turbo.AspNetCore/TagHelpers/TurboFrameTagHelper.cs
   cat src/Turbo.AspNetCore/TagHelpers/TurboPermanentTagHelper.cs
   ```

### 実装手順

#### Phase 1: 環境準備
```bash
# Stimulus.AspNetCore プロジェクトに Tag Helper ディレクトリを作成
mkdir -p src/Stimulus.AspNetCore/TagHelpers

# テストプロジェクトを作成
dotnet new xunit -n Stimulus.AspNetCore.Test -o test/Stimulus.AspNetCore.Test
```

#### Phase 2: コア Tag Helper の実装
1. `StimulusControllerTagHelper` を実装
2. `StimulusActionTagHelper` を実装
3. `StimulusTargetTagHelper` を実装
4. 各 Tag Helper の単体テストを作成

#### Phase 3: 拡張機能の実装
1. `StimulusValueTagHelper` を実装
2. `StimulusClassTagHelper` を実装
3. `StimulusHtmlExtensions` を実装
4. 拡張機能の単体テストを作成

#### Phase 4: サンプルアプリの作成
1. WireStimulus プロジェクトを作成
2. 5 つの Stimulus Controller を実装
3. 対応する Razor View を作成
4. README を記述

#### Phase 5: ドキュメント作成
1. README.md
2. stimulus-guide.md
3. stimulus-api-reference.md
4. stimulus-examples.md
5. stimulus-rails-migration.md

---

## 実装の進捗管理

### チェックリスト

実装の進捗は GitHub Issue のチェックリストで管理してください。

**例**:
```markdown
### Phase 1: コア Tag Helper
- [ ] StimulusControllerTagHelper の実装
- [ ] StimulusActionTagHelper の実装
- [ ] StimulusTargetTagHelper の実装
- [ ] 単体テスト（15 テスト）

### Phase 2: 拡張 Tag Helper
- [ ] StimulusValueTagHelper の実装
- [ ] StimulusClassTagHelper の実装
- [ ] 単体テスト（15 テスト）

... 以下続く
```

---

## コーディング規約

### 命名規則

- **Tag Helper クラス**: `Stimulus{機能}TagHelper` (例: `StimulusControllerTagHelper`)
- **拡張メソッド**: `Stimulus{機能}` (例: `StimulusController()`)
- **属性名**: `stimulus-{機能}` (例: `stimulus-controller`)

### ファイル構成

```
src/Stimulus.AspNetCore/
├── TagHelpers/
│   ├── StimulusControllerTagHelper.cs
│   ├── StimulusActionTagHelper.cs
│   ├── StimulusTargetTagHelper.cs
│   ├── StimulusValueTagHelper.cs
│   └── StimulusClassTagHelper.cs
├── StimulusHtmlExtensions.cs
└── Stimulus.AspNetCore.csproj

test/Stimulus.AspNetCore.Test/
├── StimulusControllerTagHelperTest.cs
├── StimulusActionTagHelperTest.cs
├── StimulusTargetTagHelperTest.cs
├── StimulusValueTagHelperTest.cs
├── StimulusClassTagHelperTest.cs
├── StimulusHtmlExtensionsTest.cs
└── Stimulus.AspNetCore.Test.csproj
```

---

## 参考資料

### 外部リソース

- [Stimulus Handbook](https://stimulus.hotwired.dev/handbook/introduction)
- [Stimulus Reference](https://stimulus.hotwired.dev/reference/)
- [stimulus-rails GitHub](https://github.com/hotwired/stimulus-rails)
- [ASP.NET Core Tag Helpers](https://learn.microsoft.com/en-us/aspnet/core/mvc/views/tag-helpers/intro)

### 内部リソース

- `src/Turbo.AspNetCore/TagHelpers/` - 既存の Tag Helper 実装
- `examples/WireFrame/`, `examples/WireStream/` - 既存のサンプルアプリ
- `test/Turbo.AspNetCore.Test/` - 既存のテスト

---

## よくある質問 (FAQ)

### Q1: Tag Helper と HTML 拡張メソッドの違いは？

**A**: 
- **Tag Helper**: Razor View で宣言的に使用 (`<div stimulus-controller="dropdown">`)
- **HTML 拡張メソッド**: プログラムで属性を生成 (`Html.StimulusController("dropdown")`)

どちらも同じ HTML を生成しますが、使用シーンが異なります。

### Q2: なぜ `stimulus-*` 属性なのか？

**A**: ASP.NET Core の Tag Helper は、カスタム属性を使用して HTML 要素を拡張します。`stimulus-*` は開発者にとって直感的で、最終的な `data-*` 属性に変換されます。

### Q3: Rails の stimulus-rails との互換性は？

**A**: **概念的には互換**ですが、実装は異なります。Rails は Ruby の Helper メソッドを使用し、ASP.NET Core は Tag Helper を使用します。しかし、生成される HTML は同じです。

### Q4: Stimulus.js 自体はどこから読み込む？

**A**: Stimulus.js は npm でインストールし、アプリケーションにバンドルします。Stimulus.AspNetCore はサーバーサイドのヘルパーのみを提供します。

```bash
npm install @hotwired/stimulus
```

---

## トラブルシューティング

### 問題: Tag Helper が動作しない

**解決方法**:
1. `_ViewImports.cshtml` に `@addTagHelper *, Stimulus.AspNetCore` が追加されているか確認
2. Stimulus.AspNetCore パッケージが正しくインストールされているか確認
3. ビルドエラーがないか確認

### 問題: 属性が重複する

**解決方法**:
Tag Helper は既存の `data-*` 属性と統合します。`StimulusActionTagHelper` などは既存の `data-action` と統合する設計になっています。

### 問題: テストが失敗する

**解決方法**:
1. `TagHelperContext` と `TagHelperOutput` を正しく初期化しているか確認
2. 期待される属性名と実際の属性名を確認
3. 空白やエスケープの処理を確認

---

## 貢献

このプロジェクトへの貢献を歓迎します！

### 貢献方法

1. Issue を作成（バグ報告、機能提案）
2. Fork してブランチを作成
3. 変更を加える
4. テストを追加
5. Pull Request を作成

---

## ライセンス

MIT License - 詳細は [LICENSE.txt](../LICENSE.txt) を参照

---

## 連絡先

- **Repository**: [Hotwire.AspNetCore](https://github.com/tnakamura/Hotwire.AspNetCore)
- **Issues**: [GitHub Issues](https://github.com/tnakamura/Hotwire.AspNetCore/issues)

---

**作成日**: 2026年2月11日  
**バージョン**: 1.0  
**最終更新**: 2026年2月11日
