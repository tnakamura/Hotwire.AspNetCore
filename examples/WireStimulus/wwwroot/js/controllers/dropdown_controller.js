import { Controller } from "@hotwired/stimulus"

export default class extends Controller {
  static targets = ["menu"]
  static classes = ["active"]
  static values = {
    open: Boolean
  }

  connect() {
    console.log("Dropdown controller connected")
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
