using Assets.Kiwrious.Scripts;
using kiwrious;
using System.Collections.Generic;
using UnityEngine;
using static Assets.Kiwrious.Scripts.Constants; 

public class WindowsKiwriousReader : IKiwriousReader {

    public WindowsKiwriousReader()
    {
        Debug.Log("Windows reader initiated");
    }

    public SensorData GetConductivity()
    {
        SensorData data = new SensorData
        {
            isOnline = SerialReader.instance.sensorEvents[SENSOR_TYPE.EC],
            status = (int)SENSOR_STATUS.READY,
            values = new Dictionary<string, float>
        {
            { OBSERVABLES.CONDUCTIVITY, SerialReader.instance.conductivity }
        }
        };
        return data;
    }

    public SensorData GetVOC()
    {
        SensorData data = new SensorData
        {
            isOnline = SerialReader.instance.sensorEvents[SENSOR_TYPE.VOC],
            status = (int)SENSOR_STATUS.READY,
            values = new Dictionary<string, float>
        {
            { OBSERVABLES.VOC, SerialReader.instance.voc1 }
        }
        };
        return data;
    }

    public SensorData GetUVLux()
    {
        SensorData data = new SensorData
        {
            isOnline = SerialReader.instance.sensorEvents[SENSOR_TYPE.LIGHT],
            status = (int)SENSOR_STATUS.READY,
            values = new Dictionary<string, float>
        {
            { OBSERVABLES.UV, SerialReader.instance.uv },
            { OBSERVABLES.LUX, SerialReader.instance.lux }
        }
        };
        return data;
    }

    public SensorData GetHumidityTemperature()
    {
        SensorData data = new SensorData
        {
            isOnline = SerialReader.instance.sensorEvents[SENSOR_TYPE.CLIMATE],
            status = (int)SENSOR_STATUS.READY,
            values = new Dictionary<string, float>
        {
            { OBSERVABLES.HUMIDITY, SerialReader.instance.humidity },
            { OBSERVABLES.TEMPERATURE, SerialReader.instance.temperature }
        }
        };
        return data;
    }

    public SensorData GetColor()
    {
        SensorData data = new SensorData
        {
            isOnline = SerialReader.instance.sensorEvents[SENSOR_TYPE.COLOR],
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

    public SensorData GetBodyTemperature()
    {
        SensorData data = new SensorData
        {
            isOnline = SerialReader.instance.sensorEvents[SENSOR_TYPE.THERMAL],
            status = (int)SENSOR_STATUS.READY,
            values = new Dictionary<string, float>
        {
            { OBSERVABLES.A_TEMPERATURE, SerialReader.instance.a_temperature },
            { OBSERVABLES.D_TEMPERATURE, SerialReader.instance.d_temperature }
        }
        };
        return data;
    }

    public SensorData GetBodyTemperature2()
    {
        SensorData data = new SensorData
        {
            isOnline = SerialReader.instance.sensorEvents[SENSOR_TYPE.THERMAL2],
            status = (int)SENSOR_STATUS.READY,
            values = new Dictionary<string, float>
        {
            { OBSERVABLES.A_TEMPERATURE, SerialReader.instance.a_temperature },
            { OBSERVABLES.D_TEMPERATURE, SerialReader.instance.d_temperature }
        }
        };
        return data;
    }

    public SensorData GetHeartRate()
    {
        SensorData data = new SensorData
        {
            isOnline = SerialReader.instance.sensorEvents[SENSOR_TYPE.CARDIO],
            status = (int)SENSOR_STATUS.READY,
            values = new Dictionary<string, float>
        {
            { OBSERVABLES.HEART_RATE, SerialReader.instance.heart_rate }
        }
        };
        return data;
    }
}
