# Hotwire.AspNetCore Investigation Report - Executive Summary

**Date**: February 11, 2026  
**Investigation Target**: Hotwire implementation library for ASP.NET Core  
**Purpose**: Assess current implementation status, compare with Ruby on Rails version, verify .NET 10 compatibility

---

## Quick Summary

Hotwire.AspNetCore is a lightweight server-side library that brings the Hotwire framework to ASP.NET Core. It provides basic Turbo Frames and Turbo Streams functionality through Tag Helpers, enabling developers to build modern interactive web applications with minimal JavaScript.

**Key Findings**:
- ✅ Basic implementation of Turbo Frames and Turbo Streams is complete
- ✅ Builds and tests successfully on .NET 10
- ⚠️ Turbo Drive is not implemented (relies on JavaScript library)
- ⚠️ Stimulus.js integration is not implemented
- ⚠️ Several advanced features missing compared to Rails version

---

## Implementation Status

### ✅ Implemented Features

#### 1. **Turbo Frames** (Complete)
- `TurboFrameTagHelper` for generating `<turbo-frame>` elements
- Enables partial page updates without full reload
- Example application (WireFrame) demonstrates gallery navigation

#### 2. **Turbo Streams** (Basic - Complete)
- 14 Tag Helpers covering all basic stream actions:
  - Single target: `append`, `prepend`, `replace`, `update`, `remove`, `before`, `after`
  - Multiple targets: `append-all`, `prepend-all`, `replace-all`, `update-all`, `remove-all`, `before-all`, `after-all`
- Controller extension methods for content-type handling
- Request detection helpers
- Example application (WireStream) demonstrates real-time DOM updates

#### 3. **Developer Experience**
- Intuitive Tag Helper-based API similar to Rails ERB helpers
- Clean controller extensions
- Working example applications

### ⚠️ Missing Features

#### 1. **Turbo Drive** (Not Needed Server-Side)
- Relies entirely on client-side Turbo.js library
- No server-side implementation required

#### 2. **Turbo 8 New Features** (Not Implemented)
- `morph` action - Smart DOM updates preserving state
- `refresh` action - Page-level refresh with morphing support
- These features are available in Turbo.js 8+ but lack server-side Tag Helper support

#### 3. **Real-time Features** (Critical Gap)
- No SignalR integration for WebSocket/SSE
- Rails equivalent (ActionCable) is fully integrated in turbo-rails
- Major limitation for real-time applications

#### 4. **Stimulus.js Integration** (Empty Project)
- `Stimulus.AspNetCore` project exists but is empty
- No helpers for Stimulus controller integration

#### 5. **Advanced Features**
- Custom Turbo Stream actions
- Comprehensive test helpers
- Documentation (minimal README only)

---

## Comparison with Rails Version

### Similarities
- Core Turbo Frames and Streams API is conceptually equivalent
- Tag Helper approach mirrors Rails ERB helpers
- Basic functionality parity for standard use cases

### Key Differences

| Feature | Rails (turbo-rails) | ASP.NET Core | Gap |
|---------|---------------------|--------------|-----|
| Basic Streams | ✅ | ✅ | None |
| Real-time (WebSocket) | ✅ ActionCable | ❌ | **Critical** |
| Turbo 8 Features | ✅ | ❌ | High |
| Stimulus Integration | ✅ | ❌ | Medium |
| Documentation | ✅ Excellent | ❌ Minimal | High |
| Test Helpers | ✅ Comprehensive | ⚠️ Basic | Medium |

---

## .NET 10 Compatibility

### Test Results
- ✅ All projects build successfully on .NET 10
- ✅ All 4 unit tests pass
- ⚠️ Example apps target net6.0 (EOL, should be updated to net8.0+)
- ✅ Core libraries target netstandard2.0 (broad compatibility)

### Recommendations
1. Update example applications to net8.0 (LTS) or net9.0
2. Core libraries can remain on netstandard2.0 for maximum compatibility
3. Consider .NET 8+ features for future enhancements (Source Generators, Native AOT)

---

## Priority Recommendations

### High Priority (Essential for Production Use)

#### 1. **SignalR Integration**
Implement real-time Turbo Streams via SignalR to match Rails ActionCable functionality.

**Impact**: Enables real-time applications (chat, dashboards, notifications)

**Estimated Effort**: Medium (2-4 weeks)

#### 2. **Documentation**
Create comprehensive documentation including:
- Quick start guide
- API reference
- Migration guide from Rails
- Example patterns

**Impact**: Critical for adoption

**Estimated Effort**: Small (1-2 weeks)

#### 3. **Turbo 8 Support**
Add Tag Helpers for `morph` and `refresh` actions.

**Impact**: Modern UX with state preservation

**Estimated Effort**: Small (1 week)

### Medium Priority (Important for Feature Completeness)

#### 4. **Stimulus Integration**
Implement Tag Helpers for Stimulus controller integration.

**Impact**: Complete Hotwire experience

**Estimated Effort**: Medium (2-3 weeks)

#### 5. **Test Coverage**
Expand test suite to cover all Tag Helpers and integration scenarios.

**Impact**: Code quality and reliability

**Estimated Effort**: Small (1 week)

#### 6. **Custom Stream Actions**
Support user-defined custom Turbo Stream actions.

**Impact**: Advanced customization

**Estimated Effort**: Small (1 week)

### Low Priority (Nice to Have)

- IDE IntelliSense improvements
- View rendering optimizations
- Additional example applications
- Blazor integration guide

---

## Use Case Assessment

### ✅ Currently Suitable For:
- Form submissions with partial page updates
- Independent page widgets (comment sections, sidebars)
- Projects minimizing JavaScript
- Server-side rendering focused applications
- CRUD operations with turbo frames

### ❌ Currently Not Suitable For:
- Real-time applications (chat, collaboration) - **Until SignalR integration**
- Complex client-side state management - Use Blazor/SPA instead
- Offline-capable applications

---

## Conclusion

**Overall Rating**: ⭐⭐⭐⭐ (4/5)

**Strengths**:
- Solid foundation with basic Turbo features
- Clean, maintainable codebase
- Successfully adapts Rails design philosophy to ASP.NET Core
- Works well on modern .NET (including .NET 10)

**Weaknesses**:
- Missing real-time features (critical gap)
- Incomplete Turbo 8 support
- Minimal documentation
- No Stimulus integration

**Recommendation**: **Strongly recommend continued development and maintenance**

This library fills an important gap in the ASP.NET Core ecosystem by providing a JavaScript-minimal approach to interactive web applications. With the addition of SignalR integration and improved documentation, it could become a compelling alternative to heavier client-side frameworks.

---

## Next Steps

### Immediate (1-3 months)
1. ✅ Update example apps to net8.0/net9.0
2. ✅ Write comprehensive documentation
3. ✅ Expand test coverage

### Short-term (3-6 months)
1. ⚡ Implement SignalR integration
2. ⚡ Add Turbo 8 features (morph/refresh)
3. ⚡ Support custom stream actions

### Long-term (6+ months)
1. 🎯 Complete Stimulus.js integration
2. 🎯 Build community and accept contributions
3. 🎯 Create Blazor integration patterns
4. 🎯 Achieve feature parity with turbo-rails

---

**Full detailed report available in Japanese**: `hotwire-investigation-report.md`

**Investigation by**: GitHub Copilot Agent  
**Report Version**: 1.0  
**Last Updated**: February 11, 2026
