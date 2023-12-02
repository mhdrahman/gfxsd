const { removeData } = require("jquery");

function openSchema() {
    resetTabs();
    var schemaContainer = document.getElementById("Schema");
    schemaContainer.style.display = "block";

    var outputContainer = document.getElementById("OutputCSharp");
    outputContainer.style.display = "none";

    var outputContainer = document.getElementById("OutputXml");
    outputContainer.style.display = "none";

    var button = document.getElementById("openSchemaButton");
    if (!button.classList.contains("active")) {
        button.classList.add("active");
    }
}

function openOutputCSharp(e) {
    resetTabs();
    var schemaContainer = document.getElementById("Schema");
    schemaContainer.style.display = "none";

    var outputContainer = document.getElementById("OutputCSharp");
    outputContainer.style.display = "block";

    var outputContainer = document.getElementById("OutputXml");
    outputContainer.style.display = "none";

    var button = document.getElementById("openOutputCSharpButton");
    if (!button.classList.contains("active")) {
        button.classList.add("active");
    }
}

function openOutputXml(e) {
    resetTabs();
    var schemaContainer = document.getElementById("Schema");
    schemaContainer.style.display = "none";

    var outputContainer = document.getElementById("OutputCSharp");
    outputContainer.style.display = "none";

    var outputContainer = document.getElementById("OutputXml");
    outputContainer.style.display = "block";

    var button = document.getElementById("openOutputXmlButton");
    if (!button.classList.contains("active")) {
        button.classList.add("active");
    }
}

function resetTabs() {
    var allTabs = document.getElementsByClassName("tablink");
    for (i = 0; i < allTabs.length; i++) {
        allTabs[i].classList.remove("active");
    }
}

async function generateXml() {
    // TODO get the host uri as the implementation can fail if not serving the default page
    var requestUri = window.location.href + "Home/GenerateXmlFromSchema";
    var schema = document.getElementById("schemaTextArea").value;

    var spinnerContainer = document.getElementById("spinnerContainer");
    spinnerContainer.style.opacity = 0.2;
    var spinner = document.getElementById("spinner");
    spinner.removeAttribute('hidden');

    //// TODO deal with any potential errors from request and swap to same async pattern as below
    fetch(requestUri,
        {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ Content: schema }),
        })
        .then(response => response.json())
        .then(data => {
            spinner.setAttribute('hidden', '');
            spinnerContainer.style.opacity = 0;
            document.getElementById("outputCSharpTextArea").value = data.cSharp;
            document.getElementById("outputXmlTextArea").value = data.xml;
            openOutputXml();
        });
}

async function cleanXml()
{
    await removeNodes("FlexibleData");
    await removeNodes("DynamicData");
    await removeNodes("ListOwner");
    await removeNodes("ListNo");
    await removeNodes("Description");
}

async function removeNodes(nodeName) {
    var requestUri = window.location.href + "Home/RemoveNodes";
    var xml = document.getElementById("outputXmlTextArea").value;

    var spinnerContainer = document.getElementById("spinnerContainer");
    spinnerContainer.style.opacity = 0.2;
    var spinner = document.getElementById("spinner");
    spinner.removeAttribute('hidden');

    //// TODO deal with any potential errors from request
    var response = await fetch(requestUri, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ NodeName: nodeName, Xml: xml }),
    });

    var json = await response.json();
    spinner.setAttribute('hidden', '');
    spinnerContainer.style.opacity = 0;
    document.getElementById("outputXmlTextArea").value = json.xml;
    openOutputXml();
}

function onLoad() {
    openSchema();
}