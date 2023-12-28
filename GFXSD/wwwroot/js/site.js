function onLoad() {
    new App();
}

function openErrorModal(errorMessage) {
    showDimmer();
    var errorModal = document.getElementById("error-modal");
    errorModal.style.display = "block";
    document.getElementById("error-modal-content").innerText = errorMessage;
}

function closeErrorModal() {
    var errorModal = document.getElementById("error-modal");
    errorModal.style.display = "none";
    hideDimmer();
}

function showDimmer() {
    document.getElementById("dimmer").style.display = "block";
}

function hideDimmer() {
    document.getElementById("dimmer").style.display = "none";
}

function handleError(ex) {
    openErrorModal(ex);
    hideSpinner();
}
