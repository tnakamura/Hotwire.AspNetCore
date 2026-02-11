# Turbo Custom Actions Guide

**Version**: 1.0  
**Date**: 2026-02-11  
**Status**: Production Ready

---

## Overview

Turbo Custom Actions allow you to define your own DOM manipulation logic beyond the standard Turbo Stream actions (append, replace, etc.). This feature provides Rails parity with turbo-rails' `turbo_stream.action()` method.

---

## Basic Usage

### Step 1: Define Custom Action in JavaScript

```javascript
// wwwroot/js/my-custom-actions.js
Turbo.StreamActions.my_action = function() {
  // 'this' refers to the <turbo-stream> element
  const value = this.getAttribute("my-attribute");
  
  // Implement your custom logic
  console.log("Custom action executed:", value);
}
```

### Step 2: Include JavaScript in Layout

```html
<!-- Views/Shared/_Layout.cshtml or similar -->
<script src="https://cdn.jsdelivr.net/npm/@hotwired/turbo@8/dist/turbo.es2017-umd.js"></script>
<script src="~/js/my-custom-actions.js"></script>
```

### Step 3: Use Custom Action from Server

**Option A: Tag Helper**

```html
<!-- Views/MyController/MyAction.cshtml -->
<turbo-stream-custom action="my_action" my-attribute="Hello, World!"></turbo-stream-custom>
```

**Option B: HTML Extension Method**

```csharp
@Html.TurboStreamCustom("my_action", new { my_attribute = "Hello, World!" })
```

---

## Advanced Usage

### Custom Actions with Content

**Tag Helper:**

```html
<turbo-stream-custom action="slide_in" target="notifications">
    <div class="alert alert-info">New notification</div>
</turbo-stream-custom>
```

**HTML Extension Method:**

```csharp
@Html.TurboStreamCustom("slide_in", new { target = "notifications" },
    @<div class="alert alert-info">New notification</div>)
```

**Corresponding JavaScript:**

```javascript
Turbo.StreamActions.slide_in = function() {
  const targetId = this.getAttribute("target");
  const target = document.getElementById(targetId);
  const template = this.templateElement;
  
  if (target && template) {
    const content = template.content.firstElementChild.cloneNode(true);
    // Add animation logic...
    target.appendChild(content);
  }
}
```

---

## Complete Examples

### Example 1: Set Page Title

**JavaScript:**

```javascript
Turbo.StreamActions.set_title = function() {
  const title = this.getAttribute("title");
  if (title) {
    document.title = title;
  }
}
```

**Server (Tag Helper):**

```html
<turbo-stream-custom action="set_title" title="New Page Title"></turbo-stream-custom>
```

**Server (HTML Extension):**

```csharp
@Html.TurboStreamCustom("set_title", new { title = "New Page Title" })
```

### Example 2: Show Notification

**JavaScript:**

```javascript
Turbo.StreamActions.notify = function() {
  const message = this.getAttribute("message");
  const type = this.getAttribute("type") || "info";
  const duration = parseInt(this.getAttribute("duration")) || 3000;
  
  const notification = document.createElement("div");
  notification.className = `alert alert-${type} alert-dismissible fade show`;
  notification.innerHTML = `
    ${message}
    <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
  `;
  
  document.body.appendChild(notification);
  
  setTimeout(() => notification.remove(), duration);
}
```

**Server:**

```html
<turbo-stream-custom action="notify" message="Success!" type="success" duration="3000"></turbo-stream-custom>
```

### Example 3: Highlight Element

**JavaScript:**

```javascript
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
  }, duration);
}
```

**Server:**

```html
<turbo-stream-custom action="highlight" target="product-123" color="#90EE90" duration="2000"></turbo-stream-custom>
```

---

## Best Practices

### 1. Naming Conventions
- Use **snake_case** for action names (e.g., `set_title`, `show_notification`)
- This matches Turbo's convention and Rails compatibility

### 2. Attribute Validation
- Always check if attributes exist before using them
- Provide sensible defaults

```javascript
const duration = parseInt(this.getAttribute("duration")) || 3000;
```

### 3. Error Handling
- Check if target elements exist
- Handle cases where template is empty

```javascript
const element = document.getElementById(targetId);
if (!element) {
  console.warn(`Element with id '${targetId}' not found`);
  return;
}
```

### 4. Debug Logging
- Add console logs for debugging during development
- Consider a debug flag to enable/disable logging

```javascript
console.log(`[Turbo Custom Action] ${actionName} executed`);
```

### 5. Template Access
- Use `this.templateElement` to access template content
- Always check for existence

```javascript
const template = this.templateElement;
if (template && template.content) {
  const content = template.content.firstElementChild.cloneNode(true);
  // Use content...
}
```

---

## Common Use Cases

### 1. Page Title Changes
Update browser tab title dynamically for better SEO and UX.

### 2. Notifications
Display toasts, alerts, or temporary UI elements without refreshing.

### 3. Custom Animations
Implement slide, fade, bounce, or other animation effects.

### 4. Element Highlighting
Draw attention to changed content or important elements.

### 5. Audio/Sound Effects
Provide auditory feedback for user interactions.

### 6. Analytics Events
Send Google Analytics or other tracking events on server actions.

### 7. Third-party Library Integration
Update Chart.js charts, D3.js visualizations, or other libraries.

### 8. Form State Management
Update form states, validation messages, or progress indicators.

---

## API Reference

### Tag Helper: `<turbo-stream-custom>`

**Attributes:**
- `action` (required): Name of the custom action
- Any other attributes: Passed through to the turbo-stream element

**Usage:**

```html
<turbo-stream-custom action="my_action" attribute1="value1" attribute2="value2">
    <!-- Optional content -->
</turbo-stream-custom>
```

### HTML Extension Method: `TurboStreamCustom`

**Signature:**

```csharp
IHtmlContent TurboStreamCustom(
    this IHtmlHelper html,
    string action,
    object attributes = null)

IHtmlContent TurboStreamCustom(
    this IHtmlHelper html,
    string action,
    object attributes,
    Func<object, IHtmlContent> content)
```

**Parameters:**
- `action`: Name of the custom action (required)
- `attributes`: Anonymous object with HTML attributes
- `content`: Razor template for content (optional)

**Usage:**

```csharp
// Without content
@Html.TurboStreamCustom("my_action", new { attr1 = "value1" })

// With content
@Html.TurboStreamCustom("my_action", new { target = "element" },
    @<div>Content here</div>)
```

**Note:** Underscores in attribute names are converted to hyphens (e.g., `data_value` → `data-value`).

---

## Troubleshooting

### Custom Action Not Executing

**Issue:** JavaScript action doesn't run when turbo-stream is received.

**Solutions:**
1. Verify JavaScript is loaded before Turbo receives the stream
2. Check action name matches exactly (case-sensitive)
3. Verify Turbo is properly initialized
4. Check browser console for JavaScript errors

### Attributes Not Being Passed

**Issue:** Attributes aren't available in JavaScript.

**Solutions:**
1. Verify attribute names don't conflict with reserved names
2. Check HTML output to ensure attributes are rendered
3. Use `this.getAttribute("name")` in JavaScript

### Template Content Empty

**Issue:** `this.templateElement` is null or empty.

**Solutions:**
1. Ensure content is provided in the turbo-stream
2. Check for proper template structure
3. Verify content isn't being escaped incorrectly

---

## Demo Application

See the WireStream example application for a complete working implementation:

- **Location:** `examples/WireStream/Views/CustomActions/`
- **Features:**
  - 5 complete custom action examples
  - Interactive demonstrations
  - Well-commented code

To run the demo:

```bash
cd examples/WireStream
dotnet run
```

Then navigate to `/CustomActions`.

---

## Rails Parity

This implementation provides complete feature parity with Rails' turbo-rails gem:

| Rails | Hotwire.AspNetCore |
|-------|-------------------|
| `turbo_stream.action(:my_action, attr: "value")` | `<turbo-stream-custom action="my_action" attr="value">` |
| | `@Html.TurboStreamCustom("my_action", new { attr = "value" })` |

Both approaches generate the same HTML output:

```html
<turbo-stream action="my_action" attr="value">
  <template></template>
</turbo-stream>
```

---

## Further Reading

- [Turbo Handbook - Custom Actions](https://turbo.hotwired.dev/handbook/streams#custom-actions)
- [Turbo.StreamActions API](https://turbo.hotwired.dev/reference/streams#turbo.streamactions)
- [turbo-rails Custom Actions](https://github.com/hotwired/turbo-rails)

---

## Version History

| Version | Date | Changes |
|---------|------|---------|
| 1.0 | 2026-02-11 | Initial release with full implementation |
