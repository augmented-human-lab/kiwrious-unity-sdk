using Assets.Kiwrious.Scripts;
using kiwrious;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Assets.Kiwrious.Scripts.Constants;

public class KiwriousSerialReader : MonoBehaviour {

    private IKiwriousReader kiwriousReader;
    public static KiwriousSerialReader instance;

    public Dictionary<string, SensorData> sensorData = new Dictionary<string, SensorData>();

    Text debug_log;
    public bool readersInitiated;

    void Awake()
    {
        instance = this;
        debug_log = GameObject.Find("debug_log").GetComponent<Text>();
        InitiatePlatformReaders();
    }

    public void StartSerialReader() {
        SerialReader.instance.StartSerialReader();
    }

    public void StopSerialReader() {
        SerialReader.instance.StopSerialReader();
    }

    private void InitiatePlatformReaders() {
        try
        {
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
                // initiate sensor data objects for each sensor
                sensorData[GetSensorName(sensorType)] = new SensorData();
            }
            readersInitiated = true;
        }
        catch (Exception e)
        {
            debug_log.text = e.Message;
        }
    }

    private string GetSensorName(SENSOR_TYPE type) {
        return Enum.GetName(typeof(SENSOR_TYPE), type);
    }

    void Update()
    {
        if (readersInitiated) {
            try
            {
                sensorData[GetSensorName(SENSOR_TYPE.EC)] = kiwriousReader.GetConductivity();
                sensorData[GetSensorName(SENSOR_TYPE.VOC)] = kiwriousReader.GetVOC();
                sensorData[GetSensorName(SENSOR_TYPE.LIGHT)] = kiwriousReader.GetUVLux();
                sensorData[GetSensorName(SENSOR_TYPE.CLIMATE)] = kiwriousReader.GetHumidityTemperature();
                sensorData[GetSensorName(SENSOR_TYPE.COLOR)] = kiwriousReader.GetColor();
                sensorData[GetSensorName(SENSOR_TYPE.THERMAL)] = kiwriousReader.GetBodyTemperature();
                sensorData[GetSensorName(SENSOR_TYPE.THERMAL2)] = kiwriousReader.GetBodyTemperature2();
                sensorData[GetSensorName(SENSOR_TYPE.CARDIO)] = kiwriousReader.GetHeartRate();
            }
            catch (Exception ex)
            {
                //debug_log.text = ex.Message;
            }
        }
    }

}
