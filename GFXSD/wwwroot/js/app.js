const Schema = "Schema";
const OutputXml = "OutputXml";

class App {
    constructor() {
        this.tabs = [Schema, OutputXml];
        this.schemaEditor = new XmlEditor(this.tabs[0]);
        this.outputEditor = new XmlEditor(this.tabs[1]);
        this.errorModal = new ErrorModal();
        this.spinner = new Spinner();
        this.errorHandler = new ErrorHandler(this.errorModal, this.spinner);
        this.authToken = null;
        this.loginModal = new LoginModal(() => this.authenticate());
        this.loginButton = document.getElementById(`login-button`);
        this.initialize();
    }

    initialize() {
        document.getElementById(`${Schema}Button`).addEventListener("click", () => this.openTab(Schema));
        document.getElementById(`${OutputXml}Button`).addEventListener("click", () => this.openTab(OutputXml))
        document.getElementById(`generate-xml-button`).addEventListener("click", () => this.generateXml())
        document.getElementById(`clean-xml-button`).addEventListener("click", () => this.cleanXml());
        this.loginButton.addEventListener("click", () => this.loginModal.open());
        this.openTab("Schema");
    }

    async authenticate() {
        this.loginModal.close();
        this.spinner.show();

        var username = document.getElementById("username").value;
        var password = document.getElementById("password").value;
        var token = btoa(`${username}:${password}`);

        var requestUri = window.location.protocol + "//" + window.location.host + "/Authentication/Authenticate";
        var request = { method: "POST", headers: { "Authorization": `Basic ${token}` } };

        try {
            var response = await fetch(requestUri, request);
            if (response.status === 401) {
                this.errorHandler.handleError("Incorrect username or password.");
                return;
            }

            this.token = await response.text();

            this.loginButton.hidden = true;
            this.spinner.hide();
        } catch (ex) {
            this.errorHandler.handleError(ex);
        }
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
        var request = { method: "POST", headers: { "Content-Type": "application/json", "Authorization": this.token }, body: JSON.stringify({ Content: schema }) };

        try {
            var response = await fetch(requestUri, request);
            if (response.status === 401) {
                this.handleUnauthorised();
                return;
            }

            var json = await response.json();
            this.handleError(json);

            this.outputEditor.setValue(json.xml);
            this.outputEditor.clearHighlighting();
            this.spinner.hide();
            this.openTab(OutputXml);
        } catch (ex) {
            this.errorHandler.handleError(ex);
        }
    }

    async cleanXml() {
        try {
            this.spinner.show();
            await this.removeNodes("FlexibleData");
            await this.removeNodes("DynamicData");
            this.spinner.hide();
        } catch (ex) {
            this.errorHandler.handleError(ex);
        }
    }

    async removeNodes(nodeName) {
        var xml = this.outputEditor.getValue();
        var requestUri = window.location.protocol + "//" + window.location.host + "/Home/RemoveNodes";
        var request = { method: "POST", headers: { "Content-Type": "application/json", "Authorization": this.token }, body: JSON.stringify({ NodeName: nodeName, Xml: xml }) };

        var response = await fetch(requestUri, request);
        if (response.status === 401) {
            this.handleUnauthorised();
            return;
        }

        var json = await response.json();
        this.handleError(json);

        this.outputEditor.setValue(json.result);
        this.outputEditor.clearHighlighting();
    }

    handleUnauthorised() {
        this.errorHandler.handleError("You cannot use this service if you are not logged in.");
        this.loginButton.hidden = false;
    }

    handleError(json) {
        if (json.error || json.error != null) {
            this.errorModal.open(json.error);
        }
    }
}
