# Turbo Drive 実装計画について

このディレクトリには、Turbo Drive のサーバーサイド対応に関する計画ドキュメントが含まれています。

## 📄 作成されたドキュメント

### 1. `turbo-drive-implementation-plan.md`
**用途**: 開発者向けの詳細な実装計画

**内容**:
- 実装する全機能の詳細な仕様とコード例
- 8つの主要な実装項目（リクエスト検出、Tag Helpers、サンプルアプリなど）
- 3つのフェーズに分けた実装スケジュール
- 技術的な考慮事項とベストプラクティス
- 期待される効果と成功指標

**対象読者**: このライブラリの開発者・コントリビューター

### 2. `turbo-drive-github-issue.md`
**用途**: GitHub Issue として登録するための簡潔版

**内容**:
- 実装内容の要約
- 優先順位付けされたタスクリスト
- 期待される効果（ユーザー体験・開発者体験）
- 推定工数と優先度

**対象読者**: プロジェクトマネージャー、Issue トラッキング用

## 🎯 GitHub Issue の作成方法

### オプション 1: GitHub Web UI を使用

1. リポジトリの Issues タブを開く
2. "New issue" をクリック
3. タイトルを入力: `Turbo Drive のサーバーサイド対応`
4. `turbo-drive-github-issue.md` の内容をコピー＆ペースト
5. "Submit new issue" をクリック

### オプション 2: GitHub CLI を使用

```bash
# リポジトリのルートディレクトリで実行
gh issue create \
  --title "Turbo Drive のサーバーサイド対応" \
  --body-file docs/turbo-drive-github-issue.md \
  --label enhancement
```

### オプション 3: GitHub API を使用

```bash
# GITHUB_TOKEN 環境変数が設定されている必要があります
curl -X POST \
  -H "Authorization: token $GITHUB_TOKEN" \
  -H "Accept: application/vnd.github.v3+json" \
  https://api.github.com/repos/tnakamura/Hotwire.AspNetCore/issues \
  -d @- <<EOF
{
  "title": "Turbo Drive のサーバーサイド対応",
  "body": "$(cat docs/turbo-drive-github-issue.md)",
  "labels": ["enhancement"]
}
EOF
```

## 📋 実装の概要

### Turbo Drive とは？

Turbo Drive は Hotwire フレームワークの一部で、以下の機能を提供します：
- リンククリックとフォーム送信を自動的に AJAX 化
- `<body>` の内容のみを置換し、`<head>` はマージ
- SPA ライクな高速なページ遷移を実現
- JavaScript を最小限に抑えた実装

### 現在の実装状況

| 機能 | 状態 |
|------|------|
| Turbo Frames | ✅ 実装済み |
| Turbo Streams | ✅ 実装済み |
| Turbo Drive | ⚠️ **未実装** |

### 主な実装項目（3フェーズ）

#### フェーズ 1: 基本機能（1-2週間）
- リクエスト検出機能
- メタタグヘルパー
- 基本的なテスト

#### フェーズ 2: サンプルとドキュメント（2-3週間）
- サンプルアプリ "WireDrive"
- ドキュメント整備
- テスト拡充

#### フェーズ 3: 高度な機能（1-2週間）
- 永続的要素用 Tag Helper
- コントローラー拡張メソッド
- 専用の Result クラス

## 🎨 期待される効果

### ユーザー体験
- ⚡ **高速化**: ページ遷移が SPA のように高速に
- 📉 **軽量化**: ネットワーク帯域幅の削減
- ♿ **アクセシビリティ**: JavaScript 無効時でも動作

### 開発者体験
- 🎯 **シンプル**: 最小限のコードで Turbo Drive を利用可能
- 📚 **わかりやすい**: 充実したドキュメントとサンプル
- 🔄 **親しみやすい**: Rails 開発者にとって馴染みやすい API

## 📊 推定工数

- **合計工数**: 4-6週間
- **優先度**: 中〜高
- **実装難易度**: 中

## 🔗 参考資料

- [調査レポート](./hotwire-investigation-report.md) - 現状分析と Rails 版との比較
- [Turbo Handbook - Drive](https://turbo.hotwired.dev/handbook/drive) - 公式ドキュメント
- [turbo-rails](https://github.com/hotwired/turbo-rails) - Ruby on Rails 実装

## ❓ よくある質問

### Q: Turbo Drive はサーバーサイドの実装が必要ですか？

A: 基本的な動作は Turbo.js（クライアントサイド）のみで可能ですが、サーバーサイドの対応により以下のメリットがあります：
- リクエストタイプの検出（通常 vs Turbo Drive）
- 適切なメタタグの設定
- プログレッシブエンハンスメント対応
- より洗練された開発者体験

### Q: 既存の Turbo Frames/Streams 実装への影響は？

A: ありません。Turbo Drive は独立した機能として実装され、既存機能との互換性を保ちます。

### Q: .NET Framework でも動作しますか？

A: はい。netstandard2.0 をターゲットとしているため、.NET Framework 4.7.2 以降で動作します。

## 📝 次のアクション

1. ✅ このドキュメントを確認
2. ⬜ GitHub Issue を作成（上記の方法を参照）
3. ⬜ 実装の優先順位を決定
4. ⬜ フェーズ 1 の実装を開始

---

**作成日**: 2026年2月11日  
**関連 PR**: [このブランチを確認してください]  
**問い合わせ**: Issue または Pull Request でご連絡ください
