class XmlEditor {
    constructor(editorName, theme = "gruvbox") {
        this.editor = ace.edit(editorName, { wrap: true });
        this.editor.setTheme(`ace/theme/${theme}`);
        this.editor.session.setMode("ace/mode/xml");
        this.editor.setShowPrintMargin(false);
        this.editor.$blockScrolling = Infinity;
    }

    getValue() {
        return this.editor.getValue();
    }

    setValue(value) {
        this.editor.setValue(value);
    }

    clearHighlighting() {
        this.editor.clearSelection();
    }
}