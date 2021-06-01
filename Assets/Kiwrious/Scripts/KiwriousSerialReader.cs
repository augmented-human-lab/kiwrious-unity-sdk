using kiwrious;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KiwriousSerialReader : MonoBehaviour {

    private KiwriousReader kiwriousReader;
    public static KiwriousSerialReader instance;

    public SensorData conductivity;
    public SensorData voc;
    public SensorData uv_lux;
    public SensorData humidity_temperature;
    public SensorData color;

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
            default:
                throw new Exception($"Platform {Application.platform} is not supported");
        }
    }

    void Update()
    {
        conductivity = kiwriousReader.GetConductivity();
        debug_log.text = $"{conductivity.values["Conductivity"]} ";
        voc = kiwriousReader.GetVOC();
        debug_log.text += $"{voc.values["VOC"]} ";
        uv_lux = kiwriousReader.GetUVLux();
        debug_log.text += $"{uv_lux.values["UV"]} ";
        debug_log.text += $"{uv_lux.values["Lux"]} ";
        humidity_temperature = kiwriousReader.GetHumidityTemperature();
        debug_log.text += $"{humidity_temperature.values["Humidity"]} ";
        debug_log.text += $"{humidity_temperature.values["Temperature"]} ";
        color = kiwriousReader.GetColor();
        debug_log.text += $"{color.values["ColorH"]} ";
        debug_log.text += $"{color.values["ColorS"]} ";
        debug_log.text += $"{color.values["ColorV"]} ";

    }

    void Start () {
        debug_log = GameObject.Find("debug_log").GetComponent<Text>();
        
    }


}
