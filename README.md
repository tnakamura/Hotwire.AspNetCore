# Hotwire.AspNetCore

A [Hotwire](https://hotwired.dev/) implementation library for ASP.NET Core.

[![Tests](https://img.shields.io/badge/tests-56%20passing-brightgreen)](test/)
[![.NET](https://img.shields.io/badge/.NET-8%20%7C%209%20%7C%2010-blue)](https://dotnet.microsoft.com/)

## Overview

Hotwire is an approach for building fast, modern web applications while minimizing the use of JavaScript. This library makes it easy to use Hotwire in ASP.NET Core applications.

## Features

### ✅ Turbo Drive
- **Fast page transitions**: AJAXifies links and form submissions to prevent full page reloads
- **Progressive enhancement**: Works even when JavaScript is disabled
- **Persistent elements**: Elements that maintain state across page transitions (e.g., music players)
- **Tag Helper support**: `<turbo-drive-meta>`, `<turbo-permanent>`, and more

### ✅ Turbo Frames
- **Partial page updates**: Update only specific portions of the page
- **Lazy loading**: Load content on demand
- **Tag Helper support**: Easily define frames with `<turbo-frame>`

### ✅ Turbo Streams
- **Real-time updates**: Dynamically update pages using WebSocket or SSE
- **16 standard actions**: append, prepend, replace, update, remove, before, after, append_all, prepend_all, replace_all, update_all, remove_all, before_all, after_all, morph, refresh
- **Custom actions**: Define your own DOM manipulation logic (Rails parity achieved)
- **Tag Helper support**: Easily generate Turbo Streams with `<turbo-stream>` and `<turbo-stream-custom>`
- **SignalR integration**: Real-time broadcast functionality (see below)

### ✅ Stimulus
Full Stimulus support (separate package `Stimulus.AspNetCore`):
- **5 Tag Helpers**: Controller, Action, Target, Value, Class
- **9 HTML extension methods**: Programmatically generate Stimulus attributes
- **Lightweight JavaScript framework**: Minimal JavaScript for HTML manipulation
- **20 tests**: All passing
- **Sample app**: WireStimulus provides 5 controller examples

### ✅ SignalR Integration
Real-time Turbo Streams using SignalR:
- **TurboStreamsHub**: SignalR Hub for managing WebSocket connections
- **ITurboStreamBroadcaster**: Service for broadcasting views in real-time
- **Channel-based subscriptions**: Broadcast only to specific channels
- **Automatic reconnection**: Automatic retry on connection loss
- **turbo-signalr.js**: Client-side JavaScript library
- **Sample app**: WireSignal provides notification and chat demos

## Installation

```bash
dotnet add package Hotwire.AspNetCore
```

Or, individual packages:

```bash
dotnet add package Turbo.AspNetCore
dotnet add package Stimulus.AspNetCore
```

## Quick Start

### Using Turbo Drive

1. **Add Tag Helpers to _ViewImports.cshtml**:

```csharp
@addTagHelper *, Turbo.AspNetCore
```

2. **Set meta tags in _Layout.cshtml**:

```html
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>@ViewData["Title"]</title>
    
    @* Load Turbo.js *@
    <script type="module" src="https://cdn.jsdelivr.net/npm/@@hotwired/turbo@@latest/dist/turbo.es2017-esm.min.js"></script>
    
    @* Enable Turbo Drive *@
    <turbo-drive-meta enabled="true" transition="fade" />
</head>
<body>
    @RenderBody()
</body>
```

3. **Create persistent elements** (optional):

```html
<turbo-permanent id="music-player">
    <audio controls>
        <source src="/audio/music.mp3" type="audio/mpeg">
    </audio>
</turbo-permanent>
```

### Using Turbo Frames

```html
<turbo-frame id="messages">
    <h2>Messages</h2>
    <p>Latest messages will appear here</p>
    <a href="/messages/1">Read message</a>
</turbo-frame>
```

### Using Turbo Streams

**Controller**:

```csharp
using Turbo.AspNetCore;

public class MessagesController : Controller
{
    public IActionResult Create(MessageViewModel model)
    {
        // Validation...
        
        if (Request.IsTurboStreamRequest())
        {
            return TurboStream(model);
        }
        
        return RedirectToAction("Index");
    }
}
```

**View (Create.cshtml)**:

```html
<turbo-stream action="append" target="messages">
    <template>
        <div class="message">
            <p>@Model.Content</p>
        </div>
    </template>
</turbo-stream>
```

### Using Turbo Custom Actions

**JavaScript (Define custom action)**:

```javascript
// wwwroot/js/custom-actions.js
Turbo.StreamActions.notify = function() {
  const message = this.getAttribute("message");
  const type = this.getAttribute("type") || "info";
  alert(`[${type}] ${message}`);
}
```

**View (Tag Helper)**:

```html
<turbo-stream-custom action="notify" message="Saved successfully!" type="success"></turbo-stream-custom>
```

**Or HTML extension method**:

```csharp
@Html.TurboStreamCustom("notify", new { message = "Saved successfully!", type = "success" })
```

For details, see the [Turbo Custom Actions Guide](docs/turbo-custom-actions-guide.md).

### Using Stimulus

**Add Tag Helpers to _ViewImports.cshtml**:

```csharp
@addTagHelper *, Stimulus.AspNetCore
```

**View (Dropdown example)**:

```html
<div stimulus-controller="dropdown" 
     stimulus-value-dropdown-open="false"
     stimulus-class-dropdown-active="show">
    
    <button stimulus-action="click->dropdown#toggle" 
            class="btn btn-primary">
        Open Dropdown
    </button>
    
    <div stimulus-target="dropdown.menu" 
         class="dropdown-menu">
        <a class="dropdown-item" href="#">Action</a>
        <a class="dropdown-item" href="#">Another action</a>
    </div>
</div>
```

**JavaScript (Stimulus controller)**:

```javascript
// wwwroot/js/controllers/dropdown_controller.js
import { Controller } from "@hotwired/stimulus"

export default class extends Controller {
  static targets = ["menu"]
  static classes = ["active"]
  static values = { open: Boolean }

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

For details, see the [Stimulus.AspNetCore README](src/Stimulus.AspNetCore/README.md).

### Real-time Turbo Streams with SignalR

**Setup in Program.cs**:

```csharp
using Turbo.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add SignalR
builder.Services.AddSignalR();

// Add Turbo Stream Broadcaster
builder.Services.AddTurboStreamBroadcaster();

var app = builder.Build();

// Map SignalR Hub
app.MapHub<TurboStreamsHub>("/hubs/turbo-streams");

app.Run();
```

**Broadcasting in controller**:

```csharp
using Turbo.AspNetCore;

public class NotificationsController : Controller
{
    private readonly ITurboStreamBroadcaster _broadcaster;

    public NotificationsController(ITurboStreamBroadcaster broadcaster)
    {
        _broadcaster = broadcaster;
    }

    [HttpPost]
    public async Task<IActionResult> Create(Notification notification)
    {
        // Broadcast to all subscribers
        await _broadcaster.BroadcastViewAsync(
            "notifications",     // Channel name
            "_Notification",     // Partial view
            notification         // Model
        );

        return this.TurboStream("_Notification", notification);
    }
}
```

**Client-side connection (JavaScript)**:

```javascript
// After loading turbo-signalr.js
const turboSignalR = new TurboSignalR();
await turboSignalR.start();
await turboSignalR.subscribe('notifications');

// Event listener
document.addEventListener('turbo-signalr:streamReceived', (event) => {
    console.log('Real-time update received!');
});
```

For details, see the [SignalR Integration Guide](docs/turbo-streams-signalr-guide.md) and [WireSignal Sample](examples/WireSignal/README.md).

## Sample Applications

This repository contains 5 sample applications demonstrating each Hotwire feature.

### 1. WireDrive - Turbo Drive Demo
Demo of fast page transitions and persistent elements.

```bash
cd examples/WireDrive
dotnet run
```

**Key features**:
- Fast page transitions (no reload required)
- Persistent elements (e.g., music player)
- Progressive enhancement

For details, see the [WireDrive README](examples/WireDrive/README.md).

### 2. WireFrame - Turbo Frames Demo
Demo of partial page updates.

```bash
cd examples/WireFrame
dotnet run
```

**Key features**:
- Partial page updates
- Lazy loading
- Nested frames

### 3. WireStream - Turbo Streams Demo
Demo of real-time updates and custom actions.

```bash
cd examples/WireStream
dotnet run
# Navigate to http://localhost:5000/CustomActions
```

**Key features**:
- 16 standard Turbo Stream actions
- 5 custom actions (set_title, notify, slide_in, highlight, console_log)
- DOM manipulation demo

### 4. WireStimulus - Stimulus Demo
Comprehensive Stimulus controller demo.

```bash
cd examples/WireStimulus
dotnet run
```

**Key features**:
- 5 practical Stimulus controllers
  - **Dropdown**: Toggle + auto-close
  - **Clipboard**: Copy to clipboard + feedback
  - **Counter**: Increment/decrement
  - **Form**: Real-time validation
  - **Slideshow**: Image carousel + autoplay

For details, see the [WireStimulus README](examples/WireStimulus/README.md).

### 5. WireSignal - SignalR Integration Demo
Demo of real-time Turbo Streams using SignalR.

```bash
cd examples/WireSignal
dotnet run
```

**Key features**:
- Real-time notification system
- Live chat
- WebSocket connection via SignalR
- Channel-based subscriptions

For details, see the [WireSignal README](examples/WireSignal/README.md).

## Documentation

### Guides
- [Turbo Drive Guide](docs/turbo-drive-guide.md) - How to use Turbo Drive
- [Turbo Custom Actions Guide](docs/turbo-custom-actions-guide.md) - How to implement custom actions
- [SignalR Integration Guide](docs/turbo-streams-signalr-guide.md) - Real-time Turbo Streams with SignalR

### Implementation Documents
- [Hotwire Investigation Report](docs/hotwire-investigation-report.md) - Detailed implementation status and Rails parity evaluation
- [Turbo Custom Actions Implementation Plan](docs/turbo-custom-actions-plan.md) - Design and implementation plan for custom actions
- [Stimulus.AspNetCore README](src/Stimulus.AspNetCore/README.md) - Complete documentation for Stimulus Tag Helpers

### Sample Applications
- [WireDrive README](examples/WireDrive/README.md) - Turbo Drive examples
- [WireStimulus README](examples/WireStimulus/README.md) - Comprehensive Stimulus examples (5 controllers)
- [WireSignal README](examples/WireSignal/README.md) - SignalR integration examples

## Requirements

- .NET 8.0 / 9.0 / 10.0 (library)
- .NET 10.0+ (sample apps)

## Packages

This repository contains 3 packages:

### 1. Hotwire.AspNetCore
Integrated package with all features (Turbo + Stimulus)

```bash
dotnet add package Hotwire.AspNetCore
```

### 2. Turbo.AspNetCore
Implementation of Turbo Drive/Frames/Streams

```bash
dotnet add package Turbo.AspNetCore
```

### 3. Stimulus.AspNetCore
Stimulus Tag Helpers and HTML extensions

```bash
dotnet add package Stimulus.AspNetCore
```

## Build

```bash
dotnet build
```

## Tests

```bash
dotnet test
```

**Test results**: All 56 tests (36 Turbo + 20 Stimulus) passing ✅

## Project Structure

```
Hotwire.AspNetCore/
├── src/
│   ├── Hotwire.AspNetCore/      # Integrated package
│   ├── Turbo.AspNetCore/         # Turbo implementation
│   │   ├── TagHelpers/           # Turbo Tag Helpers (6)
│   │   ├── Hubs/                 # SignalR Hub
│   │   └── wwwroot/js/           # turbo-signalr.js
│   └── Stimulus.AspNetCore/      # Stimulus implementation
│       └── TagHelpers/           # Stimulus Tag Helpers (5)
├── test/
│   ├── Turbo.AspNetCore.Test/    # Turbo tests (36)
│   └── Stimulus.AspNetCore.Test/ # Stimulus tests (20)
├── examples/
│   ├── WireDrive/                # Turbo Drive demo
│   ├── WireFrame/                # Turbo Frames demo
│   ├── WireStream/               # Turbo Streams demo
│   ├── WireStimulus/             # Stimulus demo
│   └── WireSignal/               # SignalR demo
└── docs/                         # Documentation
```

## Feature Comparison

| Feature | Rails (turbo-rails) | Hotwire.AspNetCore | Status |
|---------|---------------------|-------------------|--------|
| Turbo Drive | ✅ | ✅ | Fully implemented |
| Turbo Frames | ✅ | ✅ | Fully implemented |
| Turbo Streams (standard actions) | ✅ 16 actions | ✅ 16 actions | Rails parity |
| Turbo Streams (custom actions) | ✅ turbo_stream.action() | ✅ TurboStreamCustom | Rails parity |
| Turbo 8 (morph/refresh) | ✅ | ✅ | Fully implemented |
| Real-time streams | ✅ ActionCable | ✅ SignalR | Rails parity |
| Stimulus | ✅ stimulus-rails | ✅ Stimulus.AspNetCore | Rails parity |
| Tag Helpers | ✅ Rails Helpers | ✅ ASP.NET Tag Helpers | ASP.NET optimized |

## License

MIT License

## References

- [Hotwire Official Site](https://hotwired.dev/)
- [Turbo Handbook](https://turbo.hotwired.dev/handbook/introduction)
- [Stimulus Handbook](https://stimulus.hotwired.dev/handbook/introduction)
