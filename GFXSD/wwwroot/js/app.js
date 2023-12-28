const Schema = "Schema";
const OutputXml = "OutputXml";

class App {
    constructor() {
        this.tabs = [Schema, OutputXml];
        this.schemaEditor = new XmlEditor(this.tabs[0]);
        this.outputEditor = new XmlEditor(this.tabs[1]);
        this.spinner = new Spinner();
        this.initialize();
    }

    initialize() {
        document.getElementById(`${Schema}Button`).addEventListener("click", () => this.openTab(Schema));
        document.getElementById(`${OutputXml}Button`).addEventListener("click", () => this.openTab(OutputXml))
        document.getElementById(`generate-xml-button`).addEventListener("click", () => this.generateXml())
        document.getElementById(`clean-xml-button`).addEventListener("click", () => this.cleanXml())
        this.openTab("Schema");
    }

    openTab(tabNameToOpen) {
        for (var i = 0; i < this.tabs.length; i++) {
            var tabName = this.tabs[i];
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

    async generateXml() {
        this.spinner.show();
        var schema = this.schemaEditor.getValue();
        var requestUri = window.location.protocol + "//" + window.location.host + "/Home/GenerateXmlFromSchema";
        var request = { method: "POST", headers: { "Content-Type": "application/json" }, body: JSON.stringify({ Content: schema }) };

        try {
            var response = await fetch(requestUri, request);
            var json = await response.json();
            this.outputEditor.setValue(json.xml);

            if (json.error || json.error != null) {
                openErrorModal(json.error);
            }

            this.outputEditor.clearHighlighting();
            this.spinner.hide();
            this.openTab("OutputXml");
        } catch (ex) {
            handleError(ex);
        }
    }

    async cleanXml() {
        try {
            this.spinner.show();
            await this.removeNodes("FlexibleData");
            await this.removeNodes("DynamicData");
            this.spinner.hide();
        } catch (ex) {
            handleError(ex);
        }
    }

    async removeNodes(nodeName) {
        var xml = this.outputEditor.getValue();
        var requestUri = window.location.protocol + "//" + window.location.host + "/Home/RemoveNodes";
        var request = { method: "POST", headers: { "Content-Type": "application/json" }, body: JSON.stringify({ NodeName: nodeName, Xml: xml }) };
        var response = await fetch(requestUri, request);
        var json = await response.json();
        this.outputEditor.setValue(json.result);
        this.outputEditor.clearHighlighting();
    }
}
