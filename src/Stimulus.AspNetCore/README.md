# Stimulus.AspNetCore

A library that brings [Hotwired Stimulus](https://stimulus.hotwired.dev/) to ASP.NET Core with Tag Helpers and HTML Extensions, enabling you to write minimal JavaScript with maximum organization.

## Overview

Stimulus.AspNetCore provides ASP.NET Core Tag Helpers that generate data attributes for Stimulus.js controllers, making it easy to add interactive behavior to your server-rendered views without heavy JavaScript frameworks.

## Features

- ✅ **Tag Helpers** - Use `stimulus-*` attributes directly in Razor views
- ✅ **HTML Extensions** - Generate Stimulus data attributes programmatically
- ✅ **Type-Safe** - IntelliSense support for all attributes
- ✅ **Standards-Based** - Follows official Stimulus.js conventions
- ✅ **Minimal JavaScript** - Keep your JavaScript organized and maintainable

## Installation

Add the package reference to your ASP.NET Core project:

```xml
<ItemGroup>
  <ProjectReference Include="path/to/Stimulus.AspNetCore/Stimulus.AspNetCore.csproj" />
</ItemGroup>
```

Or use the consolidated Hotwire.AspNetCore package:

```xml
<ItemGroup>
  <ProjectReference Include="path/to/Hotwire.AspNetCore/Hotwire.AspNetCore.csproj" />
</ItemGroup>
```

## Quick Start

### 1. Add Stimulus.js to Your Layout

Add Stimulus.js from a CDN or install via npm:

```html
<!-- _Layout.cshtml -->
<script type="module">
  import { Application } from "https://unpkg.com/@hotwired/stimulus/dist/stimulus.js"
  window.Stimulus = Application.start()
</script>
```

### 2. Register Tag Helpers

Add to `_ViewImports.cshtml`:

```cshtml
@addTagHelper *, Stimulus.AspNetCore
```

### 3. Use Tag Helpers in Views

```html
<!-- Dropdown Example -->
<div stimulus-controller="dropdown" 
     stimulus-value-dropdown-open="false"
     stimulus-class-dropdown-active="show">
    
    <button stimulus-action="click->dropdown#toggle">
        Toggle Dropdown
    </button>
    
    <div stimulus-target="dropdown.menu" class="dropdown-menu">
        <a href="#" class="dropdown-item">Action</a>
        <a href="#" class="dropdown-item">Another action</a>
    </div>
</div>
```

This generates:

```html
<div data-controller="dropdown" 
     data-dropdown-open-value="false"
     data-dropdown-active-class="show">
    
    <button data-action="click->dropdown#toggle">
        Toggle Dropdown
    </button>
    
    <div data-dropdown-target="menu" class="dropdown-menu">
        <a href="#" class="dropdown-item">Action</a>
        <a href="#" class="dropdown-item">Another action</a>
    </div>
</div>
```

### 4. Create a Stimulus Controller

```javascript
// wwwroot/js/controllers/dropdown_controller.js
import { Controller } from "@hotwired/stimulus"

export default class extends Controller {
  static targets = ["menu"]
  static classes = ["active"]
  static values = {
    open: Boolean
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
}
```

## Tag Helpers

### StimulusControllerTagHelper

Registers one or more Stimulus controllers on an element.

```html
<div stimulus-controller="dropdown">...</div>
<!-- Output: <div data-controller="dropdown">...</div> -->

<div stimulus-controller="dropdown clipboard">...</div>
<!-- Output: <div data-controller="dropdown clipboard">...</div> -->
```

### StimulusActionTagHelper

Connects events to controller methods.

```html
<button stimulus-action="click->dropdown#toggle">Toggle</button>
<!-- Output: <button data-action="click->dropdown#toggle">Toggle</button> -->

<!-- Multiple actions -->
<button stimulus-action="click->dropdown#toggle mouseenter->dropdown#show">
    Hover or Click
</button>

<!-- Global events -->
<div stimulus-action="resize@window->layout#recalculate">...</div>
```

### StimulusTargetTagHelper

Defines targets that controllers can reference.

```html
<div stimulus-target="dropdown.menu">Menu</div>
<!-- Output: <div data-dropdown-target="menu">Menu</div> -->

<!-- Multiple targets for same controller -->
<input stimulus-target="form.name form.email" />
<!-- Output: <input data-form-target="name email" /> -->

<!-- Targets for different controllers -->
<div stimulus-target="dropdown.item list.item">Item</div>
<!-- Output: <div data-dropdown-target="item" data-list-target="item">Item</div> -->
```

### StimulusValueTagHelper

Passes values to controllers.

```html
<!-- Boolean -->
<div stimulus-value-dropdown-open="false">...</div>
<!-- Output: <div data-dropdown-open-value="false">...</div> -->

<!-- Number -->
<div stimulus-value-counter-count="42">...</div>
<!-- Output: <div data-counter-count-value="42">...</div> -->

<!-- String -->
<div stimulus-value-search-query="hello world">...</div>
<!-- Output: <div data-search-query-value="hello world">...</div> -->

<!-- JSON -->
<div stimulus-value-map-config='{"lat":35.6762,"lng":139.6503}'>...</div>
<!-- Output: <div data-map-config-value='{"lat":35.6762,"lng":139.6503}'>...</div> -->
```

### StimulusClassTagHelper

Defines CSS classes that controllers can toggle.

```html
<div stimulus-class-dropdown-active="show">...</div>
<!-- Output: <div data-dropdown-active-class="show">...</div> -->

<div stimulus-class-list-loading="spinner opacity-50">...</div>
<!-- Output: <div data-list-loading-class="spinner opacity-50">...</div> -->
```

## HTML Extensions

For programmatic generation of Stimulus attributes:

```csharp
@using Stimulus.AspNetCore

<!-- Single controller -->
<div @Html.Raw(string.Join(" ", Html.StimulusController("dropdown")
    .Select(kvp => $"{kvp.Key}=\"{kvp.Value}\"")))>
</div>

<!-- Multiple controllers -->
<div @Html.Raw(string.Join(" ", Html.StimulusController("dropdown", "clipboard")
    .Select(kvp => $"{kvp.Key}=\"{kvp.Value}\"")))>
</div>

<!-- Action -->
<button @Html.Raw(string.Join(" ", Html.StimulusAction("click->form#submit")
    .Select(kvp => $"{kvp.Key}=\"{kvp.Value}\"")))>
    Submit
</button>

<!-- Target -->
<input @Html.Raw(string.Join(" ", Html.StimulusTarget("form", "email")
    .Select(kvp => $"{kvp.Key}=\"{kvp.Value}\""))} />

<!-- Value -->
<div @Html.Raw(string.Join(" ", Html.StimulusValue("counter", "count", 0)
    .Select(kvp => $"{kvp.Key}=\"{kvp.Value}\"")))>
</div>

<!-- Class -->
<div @Html.Raw(string.Join(" ", Html.StimulusClass("dropdown", "active", "show")
    .Select(kvp => $"{kvp.Key}=\"{kvp.Value}\"")))>
</div>

<!-- Combined attributes -->
<input @Html.Raw(string.Join(" ", Html.StimulusAttributes(
    Html.StimulusController("form"),
    Html.StimulusTarget("form", "email"),
    Html.StimulusAction("blur->form#validate")
).Select(kvp => $"{kvp.Key}=\"{kvp.Value}\""))) />
```

## Sample Application

Check out the [WireStimulus](../examples/WireStimulus) sample application for comprehensive examples:

1. **Dropdown Controller** - Toggle menus with auto-close
2. **Clipboard Controller** - Copy to clipboard with feedback
3. **Counter Controller** - Increment/decrement with configurable step
4. **Form Validation Controller** - Real-time validation
5. **Slideshow Controller** - Image carousel with autoplay

## Best Practices

### 1. Keep JavaScript Minimal

Stimulus excels at adding small bits of behavior to server-rendered HTML:

```html
<!-- Good: Simple interaction -->
<div stimulus-controller="toggle">
    <button stimulus-action="click->toggle#show">Show Details</button>
    <div stimulus-target="toggle.content" hidden>Details here</div>
</div>
```

### 2. Use Multiple Controllers

Compose behavior by combining controllers:

```html
<div stimulus-controller="dropdown clipboard">
    <!-- This element has both dropdown and clipboard behavior -->
</div>
```

### 3. Leverage Values for Configuration

Pass configuration through values instead of hardcoding:

```html
<div stimulus-controller="counter"
     stimulus-value-counter-count="0"
     stimulus-value-counter-step="5">
    <!-- Initial count: 0, increment by: 5 -->
</div>
```

### 4. Use CSS Classes for Styling

Define reusable CSS class names:

```html
<div stimulus-controller="slideshow"
     stimulus-class-slideshow-active="slide-active"
     stimulus-class-slideshow-loading="spinner">
    <!-- Controller toggles these classes as needed -->
</div>
```

## Resources

- [Stimulus.js Documentation](https://stimulus.hotwired.dev/)
- [Stimulus.AspNetCore Implementation Plan](../../docs/stimulus-implementation-plan.md)
- [WireStimulus Sample App](../examples/WireStimulus)
- [Hotwire Official Site](https://hotwired.dev/)

## Contributing

Contributions are welcome! Please read the implementation plan and follow the existing code patterns.

## License

This project follows the same license as the parent Hotwire.AspNetCore repository.
