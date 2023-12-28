const tabs = [
    "Schema",
    "OutputXml",
];

var schemaEditor = null;
var outputEditor = null;

function onLoad() {
    openTab("Schema");
    schemaEditor = new XmlEditor("Schema");
    outputEditor = new XmlEditor("OutputXml");
}

function openTab(tabNameToOpen) {
    for (var i = 0; i < tabs.length; i++) {
        var tabName = tabs[i];
        var tabContainer = document.getElementById(tabName);
        var buttonName = tabName + "Button";
        var button = document.getElementById(buttonName);
        if (tabName == tabNameToOpen) {
            tabContainer.style.display = "block";
            button.classList.add("active");
        } else {
            tabContainer.style.display = "none";
            button.classList.remove("active");
        }
    }
}

async function generateXml() {
    showSpinner();
    var schema = schemaEditor.getValue();
    var requestUri = window.location.protocol + "//" + window.location.host + "/Home/GenerateXmlFromSchema";
    var request = { method: "POST", headers: { "Content-Type": "application/json" }, body: JSON.stringify({ Content: schema }) };

    try {
        var response = await fetch(requestUri, request);
        var json = await response.json();
        outputEditor.setValue(json.xml);

        if (json.error || json.error != null) {
            openErrorModal(json.error);
        }

        outputEditor.clearHighlighting();
        hideSpinner();
        openTab("OutputXml");
    } catch (ex) {
        handleError(ex);
    }

}

async function cleanXml() {
    try {
        showSpinner();
        await removeNodes("FlexibleData");
        await removeNodes("DynamicData");
        hideSpinner();
    } catch (ex) {
        handleError(ex);
    }
}

async function removeNodes(nodeName) {
    var xml = outputEditor.getValue();
    var requestUri = window.location.protocol + "//" + window.location.host + "/Home/RemoveNodes";
    var request = { method: "POST", headers: { "Content-Type": "application/json" }, body: JSON.stringify({ NodeName: nodeName, Xml: xml }) };
    var response = await fetch(requestUri, request);
    var json = await response.json();
    outputEditor.setValue(json.result);
    outputEditor.clearHighlighting();
}

function showSpinner() {
    var spinnerContainer = document.getElementById("spinnerContainer");
    spinnerContainer.style.opacity = 0.2;
    var spinner = document.getElementById("spinner");
    spinner.removeAttribute('hidden');
}

function hideSpinner() {
    var spinnerContainer = document.getElementById("spinnerContainer");
    spinnerContainer.style.opacity = 0;
    var spinner = document.getElementById("spinner");
    spinner.setAttribute('hidden', '');
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
