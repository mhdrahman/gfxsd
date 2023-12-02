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
    console.log(JSON.stringify({ Content: schema }));

    // TODO deal with any potential errors from request
    var response = await fetch(
        requestUri,
        {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ Content: schema }),
        });

    var json = await response.json();

    document.getElementById("outputCSharpTextArea").value = json;
    openOutputCSharp();
}

function onLoad() {
    openSchema();
}