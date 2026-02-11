import { Controller } from "@hotwired/stimulus"

export default class extends Controller {
  static targets = ["output"]
  static values = {
    count: { type: Number, default: 0 },
    step: { type: Number, default: 1 }
  }

  connect() {
    this.updateOutput()
  }

  increment() {
    this.countValue += this.stepValue
  }

  decrement() {
    this.countValue -= this.stepValue
  }

  reset() {
    this.countValue = 0
  }

  countValueChanged() {
    this.updateOutput()
  }

  updateOutput() {
    this.outputTarget.textContent = this.countValue
  }
}
