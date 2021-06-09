# Kiwrious Unity SDK

This project enables communication between kiwrious sensors and unity editor
Project made with `unity 2018.2.11f1`. You may use higher versions but avoid opening this on lower version.
Note: For higher versions there can be some API changes so you may need some migrations.
You can use this project in two different ways
- Clone this entire project and build your idea on top of this demo scene
- Download `kiwrious reader` unity package and import it to your own project

## Setup

- Clone this repository and open with `Unity 2018.2.11f1.`
- `Go to player settings --> other settings section`
- Set Api Compatibility Level to `.NET 4.X`
- Open Kiwrious > Scenes > `KIwrious Sensors Demo.unity`
- Run the project, Plugin any kiwrious sensor and observe readings

## Read Values
`Serial Reader` is the place to retrive kiwrious sensor readings.
You can use below code sniplet to access kiwrious sensor readings.
All sensor values are processed as float values.
- `KiwriousSerialReader.instance.sensorData[sensorName].values[propertyName]`
- Conductivity - `KiwriousSerialReader.instance.sensorData["Conductivity"].values["Conductivity"]`
- Humidity - `KiwriousSerialReader.instance.sensorData["Humidity"].values["Humidity"]`
- Temperature - `KiwriousSerialReader.instance.sensorData["Humidity"].values["Temperature"]`
- Uv - `KiwriousSerialReader.instance.sensorData["Uv"].values["UV"]`
- Color H - `KiwriousSerialReader.instance.sensorData["Color"].values["ColorH"]`
- Color S - `KiwriousSerialReader.instance.sensorData["Color"].values["ColorS"]`
- Color V - `KiwriousSerialReader.instance.sensorData["Color"].values["ColorV"]`
- VOC - `KiwriousSerialReader.instance.sensorData["VOC"].values["VOC"]`


## Supported Build Platforms
* Windows Standalone
* Mac Standalone
* Android


### Issues
---
> The type or namespace name `Ports' does not exist in the namespace `System.IO'. Are you missing an assembly reference?

Go to Edit > Project Settings > Player > Optimization > Api Compatibility Level

Set this to .Net 2.X or .NET 4.X (not subset)

---
