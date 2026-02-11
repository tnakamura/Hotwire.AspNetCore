# WireDrive - Turbo Drive Demo Application

WireDrive は、ASP.NET Core で Turbo Drive を使用する方法を実証するサンプルアプリケーションです。

## 概要

このアプリケーションは、Turbo Drive の主要な機能を実演します：

- **高速なページ遷移**: リンクをクリックしてもページ全体が再読み込みされず、`<body>` の内容のみが更新されます
- **フォーム送信の高速化**: POST リクエストも Turbo Drive により高速化されます
- **永続的な要素**: ページ遷移しても状態を保持する要素（音楽プレーヤー）
- **プログレッシブエンハンスメント**: JavaScript が無効でも通常通り動作します

## 実行方法

```bash
cd examples/WireDrive
dotnet run
```

ブラウザで https://localhost:5001 (または表示されたURL) を開いてください。

## 機能紹介

### 1. ホームページ (/)
- アプリケーションの概要と各機能へのリンク

### 2. About ページ (/Home/About)
- Turbo Drive の説明と特徴

### 3. 製品一覧 (/Products)
- 製品カタログの一覧表示
- 各製品へのリンク

### 4. 製品詳細 (/Products/Details/{id})
- 個別製品の詳細情報
- 注文ページへのリンク

### 5. 注文フォーム (/Orders/New)
- フォーム送信のデモ
- バリデーション機能

### 6. 注文確認 (/Orders/Confirmation)
- 注文完了後の確認ページ

## Turbo Drive の体験方法

1. **音楽プレーヤーで再生を開始**
   - ページ上部の音楽プレーヤーで再生ボタンをクリック

2. **ページ遷移を試す**
   - ナビゲーションバーのリンクをクリックしてページ間を移動
   - 音楽の再生が継続されることを確認

3. **開発者ツールで確認**
   - ブラウザの開発者ツール（F12）を開く
   - ネットワークタブを確認
   - ページ遷移時に CSS や JavaScript ファイルが再リクエストされていないことを確認

4. **フォーム送信を試す**
   - 注文ページでフォームを送信
   - ページ遷移が高速で、音楽が継続されることを確認

## 技術的な詳細

### Turbo Drive Meta Tag Helper

`_Layout.cshtml` で使用されている Tag Helper:

```html
<turbo-drive-meta enabled="true" transition="fade" />
```

これにより、以下の meta タグが生成されます:

```html
<meta name="turbo-visit-control" content="advance">
<meta name="turbo-refresh-method" content="morph">
<meta name="turbo-refresh-scroll" content="preserve">
<meta name="turbo-transition" content="fade">
```

### Turbo Permanent Tag Helper

音楽プレーヤーは `turbo-permanent` Tag Helper を使用して永続化されています:

```html
<turbo-permanent id="music-player">
    <div class="container mb-3">
        <!-- 音楽プレーヤーの内容 -->
    </div>
</turbo-permanent>
```

これにより、以下の HTML が生成されます:

```html
<div id="music-player" data-turbo-permanent="">
    <!-- 内容 -->
</div>
```

### Turbo.js の読み込み

Turbo.js は CDN から ESM として読み込まれています:

```html
<script type="module">
    import * as Turbo from 'https://cdn.jsdelivr.net/npm/@hotwired/turbo@8.0.12/+esm';
</script>
```

## 学習ポイント

1. **Tag Helper の使用**
   - `<turbo-drive-meta>` で Turbo Drive を設定
   - `<turbo-permanent>` で永続的な要素を定義

2. **ASP.NET Core との統合**
   - 通常の MVC パターンで開発可能
   - 特別なコードは不要

3. **プログレッシブエンハンスメント**
   - JavaScript が無効でもアプリは動作
   - Turbo Drive は段階的に機能を追加

4. **パフォーマンス**
   - ネットワークリクエストの削減
   - ページ遷移の高速化
   - ユーザー体験の向上

## 関連リソース

- [Turbo Drive ドキュメント](https://turbo.hotwired.dev/handbook/drive)
- [Hotwire.AspNetCore GitHub](https://github.com/tnakamura/Hotwire.AspNetCore)
- [実装プラン](../../docs/turbo-drive-implementation-plan.md)
