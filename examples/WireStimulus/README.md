# WireStimulus - Stimulus.AspNetCore Sample Application

A comprehensive demonstration of **Stimulus.AspNetCore**, showcasing how to integrate Hotwired Stimulus with ASP.NET Core applications using Tag Helpers and HTML Helpers.

## Overview

WireStimulus demonstrates practical, production-ready examples of Stimulus controllers that enhance user interactions with minimal JavaScript. Each example shows how Stimulus pairs beautifully with server-side rendering, providing just enough structure to keep JavaScript organized and maintainable.

## Features

This sample application includes five interactive examples:

### 1. 🎯 Dropdown Controller
**Purpose**: Interactive dropdown menus with auto-close functionality

**Key Features**:
- Click to toggle dropdown visibility
- Automatic close on outside click (window event handling)
- CSS class toggling for animations
- Boolean value tracking for state management

**Demonstrates**:
- `stimulus-controller` - Controller registration
- `stimulus-target` - DOM element references
- `stimulus-action` - Event handlers including window events
- `stimulus-value` - Boolean state management
- `stimulus-class` - Dynamic CSS class application

### 2. 📋 Clipboard Controller
**Purpose**: Copy text to clipboard with visual feedback

**Key Features**:
- One-click text copying
- Visual success feedback with customizable duration
- Modern Clipboard API usage
- Temporary button state changes

**Demonstrates**:
- Multiple targets (source input and button)
- Number values for configuration (success duration)
- CSS class toggling for success states
- Async operations with promises

### 3. 🔢 Counter Controller
**Purpose**: Simple counter with increment/decrement/reset

**Key Features**:
- Increment and decrement operations
- Configurable step size
- Reset to initial value
- Real-time UI updates

**Demonstrates**:
- Reactive value changes with callbacks
- `countValueChanged()` lifecycle method
- Multiple numeric values (count and step)
- State initialization in `connect()`

### 4. ✅ Form Validation Controller
**Purpose**: Real-time form validation with instant feedback

**Key Features**:
- Validate-as-you-type capability
- Multiple validation rules (required, min length, pattern)
- Email format validation
- Visual feedback with CSS classes
- Form submission prevention on errors

**Demonstrates**:
- Multiple input targets
- Multiple error message targets
- Data attributes for target linking
- Form event handling
- CSS class manipulation for states

### 5. 🖼️ Slideshow Controller
**Purpose**: Image carousel with navigation and autoplay

**Key Features**:
- Previous/Next navigation
- Direct navigation via indicators
- Optional autoplay with configurable interval
- Pause on hover
- Circular navigation

**Demonstrates**:
- Complex multi-target management
- Timer-based autoplay
- Lifecycle hooks (`connect()`, `disconnect()`)
- Index-based state management
- Multiple simultaneous actions

## Getting Started

### Prerequisites

- .NET 8.0 SDK or later
- A modern web browser with JavaScript enabled

### Running the Application

1. Navigate to the WireStimulus directory:
   ```bash
   cd examples/WireStimulus
   ```

2. Restore dependencies:
   ```bash
   dotnet restore
   ```

3. Run the application:
   ```bash
   dotnet run
   ```

4. Open your browser and navigate to:
   ```
   https://localhost:5001
   ```

## Project Structure

```
WireStimulus/
├── Controllers/             # ASP.NET Core MVC Controllers
│   ├── HomeController.cs
│   ├── DropdownController.cs
│   ├── ClipboardController.cs
│   ├── CounterController.cs
│   ├── FormController.cs
│   └── SlideshowController.cs
├── Views/                   # Razor views with Stimulus Tag Helpers
│   ├── Home/
│   │   └── Index.cshtml
│   ├── Dropdown/
│   │   └── Index.cshtml
│   ├── Clipboard/
│   │   └── Index.cshtml
│   ├── Counter/
│   │   └── Index.cshtml
│   ├── Form/
│   │   └── Index.cshtml
│   ├── Slideshow/
│   │   └── Index.cshtml
│   └── Shared/
│       ├── _Layout.cshtml
│       ├── _ViewStart.cshtml
│       └── _ViewImports.cshtml
├── wwwroot/
│   ├── js/
│   │   ├── controllers/     # Stimulus JavaScript controllers
│   │   │   ├── dropdown_controller.js
│   │   │   ├── clipboard_controller.js
│   │   │   ├── counter_controller.js
│   │   │   ├── form_controller.js
│   │   │   └── slideshow_controller.js
│   │   └── application.js   # Stimulus application initialization
│   └── css/
│       └── site.css         # Application styles
├── Program.cs               # Application entry point
├── WireStimulus.csproj      # Project file
└── README.md                # This file
```

## Using Stimulus Tag Helpers

### Basic Controller Setup

```html
<div stimulus-controller="dropdown">
  <!-- Controller scope -->
</div>
```

### Adding Actions (Event Handlers)

```html
<button stimulus-action="click->dropdown#toggle">
  Toggle
</button>
```

### Multiple Actions

```html
<button stimulus-action="click->dropdown#toggle mouseenter->dropdown#preview">
  Toggle with Preview
</button>
```

### Defining Targets

```html
<div stimulus-target="dropdown.menu">
  Menu content
</div>
```

### Setting Values

```html
<div stimulus-controller="counter" 
     stimulus-value-counter-count="10"
     stimulus-value-counter-step="5">
</div>
```

### Setting CSS Classes

```html
<div stimulus-controller="dropdown" 
     stimulus-class-dropdown-active="show">
</div>
```

## Stimulus Concepts Demonstrated

### Controllers
Controllers are the basic organizational unit in Stimulus. They connect to HTML elements and provide behavior.

### Targets
Targets let you reference important elements within a controller's scope by name.

### Actions
Actions connect DOM events to controller methods. Format: `event->controller#method`

### Values
Values let you read and write data attributes on controller elements. They're automatically typed and trigger callbacks on change.

### Classes
Classes let you reference CSS class names in your controller, making it easy to apply styles dynamically.

### Lifecycle Callbacks
- `connect()` - Called when controller is connected to DOM
- `disconnect()` - Called when controller is removed from DOM
- `[name]ValueChanged()` - Called when a value changes

## Code Examples

### Dropdown Controller (JavaScript)

```javascript
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

  hide(event) {
    if (!this.element.contains(event.target)) {
      this.openValue = false
    }
  }
}
```

### Dropdown View (Razor with Tag Helpers)

```html
<div stimulus-controller="dropdown" 
     stimulus-value-dropdown-open="false"
     stimulus-class-dropdown-active="show"
     stimulus-action="click@window->dropdown#hide">
    
    <button stimulus-action="click->dropdown#toggle" 
            class="btn btn-primary">
        Toggle Dropdown
    </button>
    
    <div stimulus-target="dropdown.menu" 
         class="dropdown-menu">
        <a class="dropdown-item" href="#">Action</a>
        <a class="dropdown-item" href="#">Another action</a>
    </div>
</div>
```

## Key Benefits

### 1. Progressive Enhancement
All examples work with standard HTML and are enhanced with JavaScript, not replaced by it.

### 2. Server-Side Rendering
Perfect for ASP.NET Core MVC applications that render HTML on the server.

### 3. Minimal JavaScript
Small, focused controllers instead of large JavaScript frameworks.

### 4. Type Safety
Tag Helpers provide IntelliSense and compile-time checking in Razor views.

### 5. Maintainable
Clear separation between HTML structure, styling, and behavior.

### 6. Reusable
Controllers can be easily reused across different pages and applications.

## Browser Support

Stimulus works in all modern browsers:
- Chrome/Edge (latest)
- Firefox (latest)
- Safari (latest)
- Mobile browsers (iOS Safari, Chrome Mobile)

## Learn More

- [Stimulus Handbook](https://stimulus.hotwired.dev/handbook/introduction)
- [Hotwire](https://hotwired.dev/)
- [Stimulus.AspNetCore Documentation](../../src/Stimulus.AspNetCore/README.md)

## Related Projects

- **WireDrive** - Turbo Drive examples
- **WireFrame** - Turbo Frames examples
- **WireStream** - Turbo Streams examples
- **WireSignal** - SignalR integration with Turbo Streams

## License

This project is licensed under the MIT License - see the LICENSE.txt file in the repository root for details.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## Troubleshooting

### Stimulus not loading
- Check browser console for JavaScript errors
- Verify that `application.js` is being loaded
- Ensure Stimulus CDN is accessible

### Controllers not connecting
- Verify `data-controller` attribute is present (inspect in browser dev tools)
- Check that controller name matches the registered name
- Look for console log messages when controllers connect

### Actions not firing
- Check `data-action` attribute syntax
- Verify method name matches controller method
- Use browser dev tools to test event firing

### Targets not found
- Verify `data-[controller]-target` attribute is within controller scope
- Check target name matches static targets declaration
- Use `hasXxxTarget` to check for optional targets

## Support

For issues, questions, or contributions, please visit the main repository:
https://github.com/khalidabuhakmeh/Hotwire.AspNetCore
