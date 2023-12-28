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

class LoginModal {
    constructor(onSubmit) {
        this.loginModal = document.getElementById("login-modal");
        this.loginModalContent = document.getElementById("login-modal-content");
        this.dimmer = document.getElementById("dimmer");
        this.submitButton = document.getElementById("login-modal-submit-button");
        this.onSubmit = onSubmit;
        this.initialize();
    }

    initialize() {
        document.getElementById("login-modal-close-button").addEventListener("click", () => this.close());
        document.getElementById("login-modal-submit-button").addEventListener("click", () => this.onSubmit());
    }

    open() {
        this.dimmer.style.display = "block";
        this.loginModal.style.display = "block";
    }

    close() {
        this.loginModal.style.display = "none";
        this.dimmer.style.display = "none";
    }
}