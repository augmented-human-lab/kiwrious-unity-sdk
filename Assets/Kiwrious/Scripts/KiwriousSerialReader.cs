using Assets.Kiwrious.Scripts;
using kiwrious;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Assets.Kiwrious.Scripts.Constants;

public class KiwriousSerialReader : MonoBehaviour {

    private KiwriousReader kiwriousReader;
    public static KiwriousSerialReader instance;

    public Dictionary<string, SensorData> sensorData = new Dictionary<string, SensorData>();

    Text debug_log;

    void Awake()
    {
        instance = this;
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                kiwriousReader = new AndroidKiwriousReader();
                break;
            case RuntimePlatform.WindowsEditor:
                kiwriousReader = new WindowsKiwriousReader();
                break;
            case RuntimePlatform.WindowsPlayer:
                kiwriousReader = new WindowsKiwriousReader();
                break;
            case RuntimePlatform.OSXEditor:
                kiwriousReader = new WindowsKiwriousReader();
                break;
            case RuntimePlatform.OSXPlayer:
                kiwriousReader = new WindowsKiwriousReader();
                break;
            default:
                throw new Exception($"Platform {Application.platform} is not supported");
        }
        foreach (SENSOR_TYPE sensorType in Enum.GetValues(typeof(SENSOR_TYPE)))
        {
            sensorData[GetSensorName(sensorType)] = new SensorData();
        }
    }

    private string GetSensorName(SENSOR_TYPE type) {
        return Enum.GetName(typeof(SENSOR_TYPE), type);
    }

    void Update()
    {
        sensorData[GetSensorName(SENSOR_TYPE.Conductivity)] = kiwriousReader.GetConductivity();
        sensorData[GetSensorName(SENSOR_TYPE.VOC)] = kiwriousReader.GetVOC();
        sensorData[GetSensorName(SENSOR_TYPE.Uv)] = kiwriousReader.GetUVLux();
        sensorData[GetSensorName(SENSOR_TYPE.Humidity)] = kiwriousReader.GetHumidityTemperature();
        sensorData[GetSensorName(SENSOR_TYPE.Color)] = kiwriousReader.GetColor();
    }

    void Start () {
        debug_log = GameObject.Find("debug_log").GetComponent<Text>();
    }


}
