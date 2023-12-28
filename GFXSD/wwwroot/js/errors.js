class ErrorHandler {
    constructor(errorModal, spinner) {
        this.errorModal = errorModal;
        this.spinner = spinner;
    }

    handleError(error) {
        this.spinner.hide();
        this.errorModal.open(error);
    }
}