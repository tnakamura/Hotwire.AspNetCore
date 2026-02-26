# WireDrive - Turbo Drive Demo Application

WireDrive is a sample application that demonstrates how to use Turbo Drive with ASP.NET Core.

## Overview

This application demonstrates the key capabilities of Turbo Drive:

- **Fast page navigation**: Clicking links updates only the `<body>` content instead of reloading the entire page
- **Faster form submissions**: POST requests are also accelerated by Turbo Drive
- **Persistent elements**: Elements that keep their state across page transitions (music player)
- **Progressive enhancement**: The app still works normally even if JavaScript is disabled

## Run the app

```bash
cd examples/WireDrive
dotnet run
```

Open https://localhost:5001 (or the URL shown in the console) in your browser.

## Features

### 1. Home page (/)
- Application overview and links to each feature

### 2. About page (/Home/About)
- Explanation of Turbo Drive and its behavior

### 3. Product catalog (/Products)
- List view of product catalog
- Links to each product detail page

### 4. Product details (/Products/Details/{id})
- Detailed view for each product
- Link to order page

### 5. Order form (/Orders/New)
- Form submission demo
- Validation behavior

### 6. Order confirmation (/Orders/Confirmation)
- Confirmation page shown after order completion

## How to experience Turbo Drive

1. **Start playback in the music player**
   - Click play on the music player at the top of the page

2. **Try page navigation**
   - Use links in the navigation bar to move across pages
   - Confirm that music playback continues

3. **Inspect with developer tools**
   - Open browser developer tools (F12)
   - Check the Network tab
   - Confirm that CSS/JavaScript files are not re-requested on navigation

4. **Try form submission**
   - Submit the form on the order page
   - Confirm fast navigation and uninterrupted music playback

## Technical details

### Turbo Drive Meta Tag Helper

Tag Helper used in `_Layout.cshtml`:

```html
<turbo-drive-meta enabled="true" transition="fade" />
```

This generates the following meta tags:

```html
<meta name="turbo-visit-control" content="advance">
<meta name="turbo-refresh-method" content="morph">
<meta name="turbo-refresh-scroll" content="preserve">
<meta name="turbo-transition" content="fade">
```

### Turbo Permanent Tag Helper

The music player is persisted using the `turbo-permanent` Tag Helper:

```html
<turbo-permanent id="music-player">
    <div class="container mb-3">
        <!-- Music player content -->
    </div>
</turbo-permanent>
```

This generates the following HTML:

```html
<div id="music-player" data-turbo-permanent="">
    <!-- Content -->
</div>
```

### Loading Turbo.js

Turbo.js is loaded as an ESM module from CDN:

```html
<script type="module" src="https://cdn.jsdelivr.net/npm/@hotwired/turbo@latest/dist/turbo.es2017-esm.min.js"></script>
```

## Learning points

1. **Using Tag Helpers**
   - Configure Turbo Drive with `<turbo-drive-meta>`
   - Define persistent elements with `<turbo-permanent>`

2. **Integration with ASP.NET Core**
   - Works with standard MVC patterns
   - No special controller code required

3. **Progressive enhancement**
   - App still works without JavaScript
   - Turbo Drive adds capability incrementally

4. **Performance**
   - Reduced network requests
   - Faster page transitions
   - Improved user experience

## Related resources

- [Turbo Drive Documentation](https://turbo.hotwired.dev/handbook/drive)
- [Hotwire.AspNetCore GitHub](https://github.com/tnakamura/Hotwire.AspNetCore)
- [Implementation Plan](../../docs/turbo-drive-implementation-plan.md)
