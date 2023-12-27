const tabs = [
    "Schema",
    "OutputXml",
];

var schemaEditor = null;
var outputEditor = null;

function onLoad() {
    openTab("Schema");
    schemaEditor = makeEditor("Schema");
    outputEditor = makeEditor("OutputXml");
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

function makeEditor(editorName) {
    var editor = ace.edit(editorName, {
        wrap: true,
    });

    editor.setTheme("ace/theme/gruvbox");
    editor.session.setMode("ace/mode/xml");
    editor.setShowPrintMargin(false);
    editor.$blockScrolling = Infinity;

    return editor;
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

        outputEditor.clearSelection();
        hideSpinner();
        openTab("OutputXml");
    } catch (ex) {
        handleError(ex);
    }

}

async function cleanXml() {
    showSpinner();
    await removeNodes("FlexibleData");
    await removeNodes("DynamicData");
    hideSpinner()
}

async function removeNodes(nodeName) {
    var xml = outputEditor.getValue();
    var requestUri = window.location.protocol + "//" + window.location.host + "/Home/RemoveNodes";
    var request = { method: "POST", headers: { "Content-Type": "application/json" }, body: JSON.stringify({ NodeName: nodeName, Xml: xml }) };

    try {
        var response = await fetch(requestUri, request);
        var json = await response.json();
        outputEditor.setValue(json.result);
        outputEditor.clearSelection();
    } catch (ex) {
        handleError(ex);
    }
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
    // TODO: Probably could show a pretty little modal
    alert("Error occured: " + ex);
    hideSpinner();
}
