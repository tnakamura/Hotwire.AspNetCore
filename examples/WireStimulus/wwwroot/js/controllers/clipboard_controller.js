import { Controller } from "@hotwired/stimulus"

export default class extends Controller {
  static targets = ["source", "button"]
  static classes = ["success"]
  static values = {
    successDuration: { type: Number, default: 2000 }
  }

  copy(event) {
    event.preventDefault()
    
    navigator.clipboard.writeText(this.sourceTarget.value)
      .then(() => this.copied())
      .catch(() => console.error("Failed to copy"))
  }

  copied() {
    if (!this.hasButtonTarget) return
    
    const originalText = this.buttonTarget.textContent
    this.buttonTarget.textContent = "Copied!"
    this.buttonTarget.classList.add(this.successClass)
    
    setTimeout(() => {
      this.buttonTarget.textContent = originalText
      this.buttonTarget.classList.remove(this.successClass)
    }, this.successDurationValue)
  }
}
