import { Controller } from "@hotwired/stimulus"

export default class extends Controller {
  static targets = ["input", "error"]
  static values = {
    required: { type: Boolean, default: false },
    minLength: { type: Number, default: 0 },
    pattern: String
  }

  validate(event) {
    const input = event.target
    const errorTarget = this.errorTargets.find(
      el => el.dataset.for === input.name
    )
    
    if (!errorTarget) return

    const value = input.value.trim()
    let errorMessage = ""

    // Required validation
    if (this.requiredValue && !value) {
      errorMessage = "This field is required"
    }
    // Min length validation
    else if (this.minLengthValue > 0 && value.length < this.minLengthValue) {
      errorMessage = `Minimum ${this.minLengthValue} characters required`
    }
    // Pattern validation
    else if (this.hasPatternValue && !new RegExp(this.patternValue).test(value)) {
      errorMessage = "Invalid format"
    }

    errorTarget.textContent = errorMessage
    input.classList.toggle("is-invalid", errorMessage !== "")
    input.classList.toggle("is-valid", errorMessage === "" && value !== "")
  }

  validateEmail(event) {
    const input = event.target
    const errorTarget = this.errorTargets.find(
      el => el.dataset.for === input.name
    )
    
    if (!errorTarget) return

    const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
    const value = input.value.trim()
    
    let errorMessage = ""
    if (!value) {
      errorMessage = "Email is required"
    } else if (!emailPattern.test(value)) {
      errorMessage = "Invalid email format"
    }

    errorTarget.textContent = errorMessage
    input.classList.toggle("is-invalid", errorMessage !== "")
    input.classList.toggle("is-valid", errorMessage === "")
  }

  submit(event) {
    event.preventDefault()
    
    // Validate all inputs
    this.inputTargets.forEach(input => {
      this.validate({ target: input })
    })

    // Check if there are any errors
    const hasErrors = this.errorTargets.some(error => error.textContent !== "")
    
    if (!hasErrors) {
      alert("Form submitted successfully!")
      // In a real app, you would submit the form here
    }
  }
}
