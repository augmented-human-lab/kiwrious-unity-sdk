using Assets.Kiwrious.Scripts;
using kiwrious;
using System.Collections.Generic;
using UnityEngine;
using static Assets.Kiwrious.Scripts.Constants;

public class WindowsKiwriousReader : KiwriousReader {

    public WindowsKiwriousReader()
    {
        Debug.Log("Windows reader initiated");
    }

    public override SensorData GetConductivity()
    {
        SensorData data = new SensorData
        {
            isOnline = SerialReader.instance.sensorEvents[(int)SENSOR_TYPE.Conductivity],
            status = (int)SENSOR_STATUS.READY,
            values = new Dictionary<string, float>
        {
            { "Conductivity", SerialReader.instance.conductivity }
        }
        };
        return data;
    }

    public override SensorData GetVOC()
    {
        SensorData data = new SensorData
        {
            isOnline = SerialReader.instance.sensorEvents[(int)SENSOR_TYPE.VOC],
            status = (int)SENSOR_STATUS.READY,
            values = new Dictionary<string, float>
        {
            { "VOC", SerialReader.instance.voc1 }
        }
        };
        return data;
    }

    public override SensorData GetUVLux()
    {
        SensorData data = new SensorData
        {
            isOnline = SerialReader.instance.sensorEvents[(int)SENSOR_TYPE.Uv],
            status = (int)SENSOR_STATUS.READY,
            values = new Dictionary<string, float>
        {
            { "UV", SerialReader.instance.uv },
            { "Lux", SerialReader.instance.lux }
        }
        };
        return data;
    }

    public override SensorData GetHumidityTemperature()
    {
        SensorData data = new SensorData
        {
            isOnline = SerialReader.instance.sensorEvents[(int)SENSOR_TYPE.Humidity],
            status = (int)SENSOR_STATUS.READY,
            values = new Dictionary<string, float>
        {
            { "Humidity", SerialReader.instance.humidity },
            { "Temperature", SerialReader.instance.temperature }
        }
        };
        return data;
    }

    public override SensorData GetColor()
    {
        SensorData data = new SensorData
        {
            isOnline = SerialReader.instance.sensorEvents[(int)SENSOR_TYPE.Color],
            status = (int)SENSOR_STATUS.READY,
            values = new Dictionary<string, float>
        {
            { "ColorH", SerialReader.instance.color_h },
            { "ColorS", SerialReader.instance.color_s },
            { "ColorV", SerialReader.instance.color_v }
        }
        };
        return data;
    }
}
