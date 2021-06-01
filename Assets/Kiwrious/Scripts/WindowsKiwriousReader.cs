using kiwrious;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowsKiwriousReader : KiwriousReader {

    public WindowsKiwriousReader()
    {
        Debug.Log("Windows reader initiated");
    }

    public override SensorData GetConductivity()
    {
        Debug.Log("Get Conductivity windows Reader");
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
        Debug.Log("Get VOC windows Reader");
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
        Debug.Log("Get UV Lux windows Reader");
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
        Debug.Log("Get Humidity Temperature windows Reader");
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
        Debug.Log("Get Color windows Reader");
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
