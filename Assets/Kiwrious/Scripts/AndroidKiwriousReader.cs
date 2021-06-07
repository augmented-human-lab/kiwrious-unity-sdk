using Assets.Kiwrious.Scripts;
using kiwrious;
using System.Collections.Generic;
using UnityEngine;
using static Assets.Kiwrious.Scripts.Constants;

public class AndroidKiwriousReader : KiwriousReader
{

    private const string pluginName = "org.ahlab.kiwrious.android.Plugin";
    private static AndroidJavaClass _pluginClass;
    private static AndroidJavaObject _pluginInstance;

	public static AndroidJavaClass PluginClass
	{
		get
		{
			if (_pluginClass == null)
			{
				_pluginClass = new AndroidJavaClass(pluginName);
			}
			return _pluginClass;
		}
	}

	public static AndroidJavaObject PluginInstance
	{
		get
		{
			if (_pluginInstance == null)
			{
				_pluginInstance = PluginClass.CallStatic<AndroidJavaObject>("getInstance");
			}
			return _pluginInstance;
		}
	}

	public AndroidKiwriousReader() {
		Debug.Log("Android reader initiated");
	}

	public override SensorData GetConductivity()
	{
		float conductivity = callNative("getConductivity");
		SensorData data = new SensorData
		{
			isOnline = getNative("conductivity_online"),
			status = (int)SENSOR_STATUS.READY,
			values = new Dictionary<string, float>
		{
			{ OBSERVABLES.CONDUCTIVITY, conductivity }
		}
		};
		return data;
	}

	public override SensorData GetVOC()
	{
		float voc = callNative("getVoc");
		SensorData data = new SensorData
		{
			isOnline = getNative("voc_online"),
			status = (int)SENSOR_STATUS.READY,
			values = new Dictionary<string, float>
		{
			{ OBSERVABLES.VOC, voc }
		}
		};
		return data;
	}

    public override SensorData GetUVLux()
    {
		float uv = callNative("getUV");
		float lux = callNative("getLux");
		SensorData data = new SensorData
		{
			isOnline = getNative("uv_lux_online"),
			status = (int)SENSOR_STATUS.READY,
			values = new Dictionary<string, float>
		{
			{ OBSERVABLES.UV, uv },
			{ OBSERVABLES.LUX, lux }
		}
		};
		return data;
	}

    public override SensorData GetHumidityTemperature()
    {
		float humidity = callNative("getHumidity");
		float temperature = callNative("getTemperature");
		SensorData data = new SensorData
		{
			isOnline = getNative("humidity_temperature_online"),
			status = (int)SENSOR_STATUS.READY,
			values = new Dictionary<string, float>
		{
			{ OBSERVABLES.HUMIDITY, humidity },
			{ OBSERVABLES.TEMPERATURE, temperature }
		}
		};
		return data;
	}

    public override SensorData GetColor()
    {
		float color_h = callNative("getColorH");
		float color_s = callNative("getColorS");
		float color_v = callNative("getColorV");
		SensorData data = new SensorData
		{
			isOnline = getNative("color_online"),
			status = (int)SENSOR_STATUS.READY,
			values = new Dictionary<string, float>
		{
			{ OBSERVABLES.COLOR_H, color_h },
			{ OBSERVABLES.COLOR_S, color_s },
			{ OBSERVABLES.COLOR_V, color_v }
		}
		};
		return data;
	}

	private float callNative(string methodName) {
		return PluginInstance.Call<float>(methodName);
	}

	private bool getNative(string propertyName)
	{
		return PluginInstance.Get<bool>(propertyName);
	}

}
