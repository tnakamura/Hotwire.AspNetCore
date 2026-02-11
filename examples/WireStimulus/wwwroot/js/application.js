import { Application } from "@hotwired/stimulus"
import DropdownController from "./controllers/dropdown_controller.js"
import ClipboardController from "./controllers/clipboard_controller.js"
import CounterController from "./controllers/counter_controller.js"
import FormController from "./controllers/form_controller.js"
import SlideshowController from "./controllers/slideshow_controller.js"

window.Stimulus = Application.start()

// Register controllers
Stimulus.register("dropdown", DropdownController)
Stimulus.register("clipboard", ClipboardController)
Stimulus.register("counter", CounterController)
Stimulus.register("form", FormController)
Stimulus.register("slideshow", SlideshowController)

// Debug mode (optional)
Stimulus.debug = true
console.log("Stimulus application started")
