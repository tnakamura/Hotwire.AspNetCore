// wwwroot/js/turbo-custom-actions.js

// Example 1: Set Page Title
Turbo.StreamActions.set_title = function() {
  const title = this.getAttribute("title");
  if (title) {
    document.title = title;
    console.log(`[Turbo Custom Action] Title set to: ${title}`);
  }
}

// Example 2: Show Notification
Turbo.StreamActions.notify = function() {
  const message = this.getAttribute("message");
  const type = this.getAttribute("type") || "info";
  const duration = parseInt(this.getAttribute("duration")) || 3000;
  
  // Create notification element
  const notification = document.createElement("div");
  notification.className = `alert alert-${type} alert-dismissible fade show`;
  notification.role = "alert";
  notification.innerHTML = `
    ${message}
    <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
  `;
  
  // Find or create notification container
  let container = document.getElementById("notification-container");
  if (!container) {
    container = document.createElement("div");
    container.id = "notification-container";
    container.style.position = "fixed";
    container.style.top = "20px";
    container.style.right = "20px";
    container.style.zIndex = "9999";
    container.style.maxWidth = "400px";
    document.body.appendChild(container);
  }
  
  container.appendChild(notification);
  
  // Auto-dismiss after duration
  setTimeout(() => {
    notification.classList.remove("show");
    setTimeout(() => notification.remove(), 150);
  }, duration);
  
  console.log(`[Turbo Custom Action] Notification shown: ${message}`);
}

// Example 3: Slide In Animation
Turbo.StreamActions.slide_in = function() {
  const targetId = this.getAttribute("target");
  const target = document.getElementById(targetId);
  const template = this.templateElement;
  
  if (!target || !template) return;
  
  const newElement = template.content.firstElementChild.cloneNode(true);
  
  // Set initial state
  newElement.style.transform = "translateX(100%)";
  newElement.style.transition = "transform 0.3s ease-out";
  newElement.style.opacity = "0";
  
  target.appendChild(newElement);
  
  // Trigger animation
  requestAnimationFrame(() => {
    requestAnimationFrame(() => {
      newElement.style.transform = "translateX(0)";
      newElement.style.opacity = "1";
    });
  });
  
  console.log(`[Turbo Custom Action] Slide in completed for: ${targetId}`);
}

// Example 4: Highlight Element
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
    setTimeout(() => {
      element.style.transition = "";
    }, duration);
  }, duration);
  
  console.log(`[Turbo Custom Action] Highlight applied to: ${targetId}`);
}

// Example 5: Console Log (for debugging)
Turbo.StreamActions.console_log = function() {
  const message = this.getAttribute("message");
  const level = this.getAttribute("level") || "log";
  
  if (message && console[level]) {
    console[level](`[Turbo Custom Action] ${message}`);
  }
}

console.log("[Turbo Custom Actions] All custom actions registered.");
