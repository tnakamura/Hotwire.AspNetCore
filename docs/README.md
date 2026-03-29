# Hotwire.AspNetCore Investigation Documentation

This directory contains comprehensive investigation reports for the Hotwire.AspNetCore library.

## 📚 Available Reports

### 1. [Complete Investigation Report (Japanese)](./hotwire-investigation-report.md)
**File**: `hotwire-investigation-report.md` (28KB)

包括的な調査レポート（日本語）。以下の内容を含みます：

- **リポジトリ構成の完全把握**: プロジェクト構造、ターゲットフレームワーク
- **実装済み機能の詳細**: Turbo Frames、Turbo Streams、Tag Helpers の全機能
- **本家 Hotwire (Rails) との比較**: 機能マトリクス、ヘルパー比較
- **未実装機能と拡張ポイント**: SignalR 統合、Turbo 8、Stimulus.js
- **.NET 10 対応状況と将来性**: 互換性検証、拡張性評価
- **ベストプラクティス**: 実装推奨事項、パフォーマンス最適化
- **結論と次のステップ**: 優先度付きロードマップ

### 2. [Executive Summary (English)](./INVESTIGATION_SUMMARY.md)
**File**: `INVESTIGATION_SUMMARY.md` (7.2KB)

Concise summary in English covering:

- **Quick Summary**: Key findings at a glance
- **Implementation Status**: What's working and what's missing
- **Comparison with Rails**: Feature parity analysis
- **.NET 10 Compatibility**: Test results and recommendations
- **Priority Recommendations**: Actionable next steps
- **Use Case Assessment**: When to use (and not use) this library

### 3. [Hotwire.AspNetCore 紹介ブログ記事 (Japanese)](./hotwire-aspnetcore-intro-blog-ja.md)
**File**: `hotwire-aspnetcore-intro-blog-ja.md`

日本語の紹介記事。以下の内容を含みます：

- **開発動機**: MVC/Razor Pages と Blazor の間を埋める狙い
- **ライブラリの特徴**: Turbo Drive / Frames / Streams / SignalR / Stimulus
- **使い方**: 導入手順と最小構成サンプル
- **実装の工夫ポイント**: ソースコードに基づく設計上の意図

## 🎯 Quick Findings

### ✅ Implemented Features
- Turbo Frames (complete with Tag Helper)
- Turbo Streams (14 Tag Helpers for all basic actions)
- Controller extensions for content-type handling
- Request detection helpers
- Working example applications (WireFrame, WireStream)
- .NET 10 compatibility verified

### ⚠️ Missing Features (Priority)
1. **SignalR Integration** (Critical) - Real-time updates via WebSocket/SSE
2. **Turbo 8 Features** - `morph` and `refresh` actions
3. **Stimulus.js Integration** - Client-side interactivity framework
4. **Documentation** - Comprehensive guides and API reference

### 📊 Overall Assessment
- **Rating**: ⭐⭐⭐⭐ (4/5)
- **Recommendation**: Strongly recommended for continued development
- **Status**: Solid foundation, production-ready for non-real-time use cases

## 🔍 How to Use These Reports

1. **For Quick Overview**: Start with [INVESTIGATION_SUMMARY.md](./INVESTIGATION_SUMMARY.md)
2. **For Technical Details**: Read [hotwire-investigation-report.md](./hotwire-investigation-report.md)
3. **For Development Planning**: Review the recommendations sections in either report

## 🛠️ Build & Test Results

All tests performed on:
- **Date**: February 11, 2026
- **.NET SDK**: 10.0.102
- **Build**: ✅ Success
- **Tests**: ✅ 4/4 Passed
- **Examples**: ✅ Functional

## 📖 Related Resources

### Official Hotwire Documentation
- [Hotwire Official Site](https://hotwired.dev/)
- [Turbo Handbook](https://turbo.hotwired.dev/handbook/introduction)
- [turbo-rails GitHub](https://github.com/hotwired/turbo-rails)

### ASP.NET Core Documentation
- [ASP.NET Core Tag Helpers](https://learn.microsoft.com/en-us/aspnet/core/mvc/views/tag-helpers/intro)
- [SignalR Documentation](https://learn.microsoft.com/en-us/aspnet/core/signalr/)
- [.NET 10 Overview](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-10/overview)

## 🤝 Contributing

If you're planning to contribute to Hotwire.AspNetCore, these reports will help you understand:
- Current implementation state
- Missing features and their priority
- Comparison with the Rails version
- Best practices to follow

## 📝 Report Metadata

- **Investigation Date**: February 11, 2026
- **Investigated By**: GitHub Copilot Agent
- **Report Version**: 1.0
- **Repository**: [tnakamura/Hotwire.AspNetCore](https://github.com/tnakamura/Hotwire.AspNetCore)

---

**Note**: These reports represent a point-in-time analysis. For the latest status, check the repository's commit history and open issues.
