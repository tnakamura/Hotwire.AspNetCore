import { Controller } from "@hotwired/stimulus"

export default class extends Controller {
  static targets = ["slide", "prev", "next", "indicator"]
  static values = {
    index: { type: Number, default: 0 },
    autoplay: { type: Boolean, default: false },
    interval: { type: Number, default: 5000 }
  }

  connect() {
    this.showCurrentSlide()
    if (this.autoplayValue) {
      this.startAutoplay()
    }
  }

  disconnect() {
    this.stopAutoplay()
  }

  next() {
    this.indexValue = (this.indexValue + 1) % this.slideTargets.length
  }

  previous() {
    this.indexValue = this.indexValue === 0 
      ? this.slideTargets.length - 1 
      : this.indexValue - 1
  }

  goToSlide(event) {
    this.indexValue = parseInt(event.currentTarget.dataset.index)
  }

  indexValueChanged() {
    this.showCurrentSlide()
  }

  showCurrentSlide() {
    this.slideTargets.forEach((slide, index) => {
      slide.classList.toggle("active", index === this.indexValue)
    })

    if (this.hasIndicatorTarget) {
      this.indicatorTargets.forEach((indicator, index) => {
        indicator.classList.toggle("active", index === this.indexValue)
      })
    }

    // Update prev/next button states
    if (this.hasPrevTarget && this.hasNextTarget) {
      this.prevTarget.disabled = this.indexValue === 0
      this.nextTarget.disabled = this.indexValue === this.slideTargets.length - 1
    }
  }

  startAutoplay() {
    this.autoplayTimer = setInterval(() => {
      this.next()
    }, this.intervalValue)
  }

  stopAutoplay() {
    if (this.autoplayTimer) {
      clearInterval(this.autoplayTimer)
    }
  }

  pauseAutoplay() {
    this.stopAutoplay()
  }

  resumeAutoplay() {
    if (this.autoplayValue) {
      this.startAutoplay()
    }
  }
}
