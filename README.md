# Kiwrious Unity SDK

This project enables communication between kiwrious sensors and unity editor.
Project made with `unity 2018.2.11f1`. You may use higher versions but avoid opening this on lower version.

You can use this project in two different ways
- Clone this entire project and build your idea on top of this demo scene
- Download `kiwrious reader` unity package and import it to your own project

## Setup

- Clone this repository and open with `Unity 2018.2.11f1.`
- `Go to player settings --> other settings section`
- Set Api Compatibility Level to `.NET 4.X`
- Open Kiwrious > Scenes > `KIwrious Sensors Demo.unity`
- Run the project, Plugin any kiwrious sensor and observe readings

## Start Reader
- Add `Serial Reader` prefab from `Assets/Kiwrious/Prefabs`
```csharp
KiwriousSerialReader.instance.StartSerialReader();
```
## Stop Reader
```csharp
KiwriousSerialReader.instance.StopSerialReader();
```

## Read Values

All sensor values are processed as float values.
```csharp
KiwriousSerialReader.instance.sensorData[sensorName].values[propertyName];
```

```csharp
  KiwriousSerialReader.instance.sensorData["Conductivity"].values["Conductivity"]
  KiwriousSerialReader.instance.sensorData["Humidity"].values["Humidity"]
  KiwriousSerialReader.instance.sensorData["Humidity"].values["Temperature"]
  KiwriousSerialReader.instance.sensorData["Uv"].values["UV"]
  KiwriousSerialReader.instance.sensorData["Color"].values["ColorH"]
  KiwriousSerialReader.instance.sensorData["Color"].values["ColorS"]
  KiwriousSerialReader.instance.sensorData["Color"].values["ColorV"]
  KiwriousSerialReader.instance.sensorData["VOC"].values["VOC"]
```

## Supported Build Platforms
* PC Standalone
* Mac Standalone
* Android


### Issues
---
> The type or namespace name `Ports' does not exist in the namespace `System.IO'. Are you missing an assembly reference?

Go to Edit > Project Settings > Player > Optimization > Api Compatibility Level

Set this to .Net 2.X or .NET 4.X (not subset)

---
