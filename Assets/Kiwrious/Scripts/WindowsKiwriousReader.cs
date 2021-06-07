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
            { OBSERVABLES.CONDUCTIVITY, SerialReader.instance.conductivity }
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
            { OBSERVABLES.VOC, SerialReader.instance.voc1 }
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
            { OBSERVABLES.UV, SerialReader.instance.uv },
            { OBSERVABLES.LUX, SerialReader.instance.lux }
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
            { OBSERVABLES.HUMIDITY, SerialReader.instance.humidity },
            { OBSERVABLES.TEMPERATURE, SerialReader.instance.temperature }
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
            { OBSERVABLES.COLOR_H, SerialReader.instance.color_h },
            { OBSERVABLES.COLOR_S, SerialReader.instance.color_s },
            { OBSERVABLES.COLOR_V, SerialReader.instance.color_v }
        }
        };
        return data;
    }
}
