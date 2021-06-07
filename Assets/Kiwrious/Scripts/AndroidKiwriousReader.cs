using Assets.Kiwrious.Scripts;
using kiwrious;
using System.Collections.Generic;
using UnityEngine;
using static Assets.Kiwrious.Scripts.Constants;

public class AndroidKiwriousReader : KiwriousReader
{

    const string pluginName = "org.ahlab.kiwrious.android.Plugin";

    static AndroidJavaClass _pluginClass;
    static AndroidJavaObject _pluginInstance;

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
		float conductivity = PluginInstance.Call<float>("getConductivity");
		SensorData data = new SensorData
		{
			isOnline = PluginInstance.Get<bool>("conductivity_online"),
			status = (int)SENSOR_STATUS.READY,
			values = new Dictionary<string, float>
		{
			{ "Conductivity", conductivity }
		}
		};
		return data;
	}

	public override SensorData GetVOC()
	{
		float voc = PluginInstance.Call<float>("getVoc");
		SensorData data = new SensorData
		{
			isOnline = PluginInstance.Get<bool>("voc_online"),
			status = (int)SENSOR_STATUS.READY,
			values = new Dictionary<string, float>
		{
			{ "VOC", voc }
		}
		};
		return data;
	}

    public override SensorData GetUVLux()
    {
		float uv = PluginInstance.Call<float>("getUV");
		float lux = PluginInstance.Call<float>("getLux");
		SensorData data = new SensorData
		{
			isOnline = PluginInstance.Get<bool>("uv_lux_online"),
			status = (int)SENSOR_STATUS.READY,
			values = new Dictionary<string, float>
		{
			{ "UV", uv },
			{ "Lux", lux }
		}
		};
		return data;
	}

    public override SensorData GetHumidityTemperature()
    {
		float humidity = PluginInstance.Call<float>("getHumidity");
		float temperature = PluginInstance.Call<float>("getTemperature");
		SensorData data = new SensorData
		{
			isOnline = PluginInstance.Get<bool>("humidity_temperature_online"),
			status = (int)SENSOR_STATUS.READY,
			values = new Dictionary<string, float>
		{
			{ "Humidity", humidity },
			{ "Temperature", temperature }
		}
		};
		return data;
	}

    public override SensorData GetColor()
    {
		float color_h = PluginInstance.Call<float>("getColorH");
		float color_s = PluginInstance.Call<float>("getColorS");
		float color_v = PluginInstance.Call<float>("getColorV");
		SensorData data = new SensorData
		{
			isOnline = PluginInstance.Get<bool>("color_online"),
			status = (int)SENSOR_STATUS.READY,
			values = new Dictionary<string, float>
		{
			{ "ColorH", color_h },
			{ "ColorS", color_s },
			{ "ColorV", color_v }
		}
		};
		return data;
	}

}
