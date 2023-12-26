const tabs = [
    "Schema",
    "OutputXml",
];

function onLoad() {
    openTab("Schema");
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

async function generateXml(useCodeGen) {
    showSpinner();
    var schema = document.getElementById("schemaTextArea").value;
    var requestUri = window.location.protocol + "//" + window.location.host + "/Home/GenerateXmlFromSchema";
    var request = { method: "POST", headers: { "Content-Type": "application/json" }, body: JSON.stringify({ Content: schema }) };

    try {
        var response = await fetch(requestUri, request);
        var json = await response.json();
        document.getElementById("outputXmlTextArea").value = json.xml;
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
    var xml = document.getElementById("outputXmlTextArea").value;
    var requestUri = window.location.protocol + "//" + window.location.host + "/Home/RemoveNodes";
    var request = { method: "POST", headers: { "Content-Type": "application/json" }, body: JSON.stringify({ NodeName: nodeName, Xml: xml }) };

    try {
        var response = await fetch(requestUri, request);
        var json = await response.json();
        document.getElementById("outputXmlTextArea").value = json.result;
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

function handleError(ex) {
    // Probably could show a pretty little modal
    alert("Error occured: " + ex);
    hideSpinner();
}
