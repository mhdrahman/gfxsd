# GFXSD
GFXSD is a tool to generate sample XML data from an XSD schema.

## Implementations
GFXSD currently contains three implementations of sample XML generation which do the following:
- XmlBeansGeneratorService.cs: uses XmlBeans under the hood to generate the sample XML
- XmlFixtureGeneratorService.cs: uses the xsd.exe tool shipped with .NET SDK to emit a dll containing a C# class representation of the schema. AutoFixture is then used to populate the fields.
- XmlSampleGeneratorService.cs: uses the XmlSampleGenerator project to generate the sample XML 

## Features
- Generates sample XML data based on the types and elements defined in the XSD
- Supports simple and complex types, attributes, repetition indicators, etc.

## Requirements
- .NET 6.0+