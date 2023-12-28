class ErrorModal {
    constructor() {
        this.errorModal = document.getElementById("error-modal");
        this.errorModalContent = document.getElementById("error-modal-content");
        this.dimmer = document.getElementById("dimmer");
        this.initialize();
    }

    initialize() {
        document.getElementById("error-modal-close-button").addEventListener("click", () => this.close());
    }

    open(errorMessage) {
        this.dimmer.style.display = "block";
        this.errorModal.style.display = "block";
        this.errorModalContent.innerText = errorMessage;
    }

    close() {
        this.errorModal.style.display = "none";
        this.dimmer.style.display = "none";
    }
}