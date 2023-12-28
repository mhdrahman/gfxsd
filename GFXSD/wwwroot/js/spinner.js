class Spinner {
    constructor() {
        this.spinnerContainer = document.getElementById("spinnerContainer");
        this.spinner = document.getElementById("spinner");
    }
    
    show() {
        this.spinnerContainer.style.opacity = 0.2;
        this.spinner.removeAttribute('hidden');
    }

    hide() {
        this.spinnerContainer.style.opacity = 0;
        this.spinner.setAttribute('hidden', '');
    }
}