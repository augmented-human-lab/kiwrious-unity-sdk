using Assets.Kiwrious.Scripts;
using kiwrious;
using System.Collections.Generic;
using UnityEngine;
using static Assets.Kiwrious.Scripts.Constants;

public class AndroidKiwriousReader : IKiwriousReader
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
				_pluginInstance = PluginClass.CallStatic<AndroidJavaObject>("getInstance", getApplicationContext());
				_pluginInstance.Call<bool>("startSerialReader");
			}
			return _pluginInstance;
		}
	}

	private static AndroidJavaObject getApplicationContext() {
		AndroidJavaClass _unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject _unityInstance = _unityClass.GetStatic<AndroidJavaObject>("currentActivity");
		AndroidJavaObject _unityContext = _unityInstance.Call<AndroidJavaObject>("getApplicationContext");
		return _unityContext;
	}

	public AndroidKiwriousReader() {
		Debug.Log("Android reader initiated");
	}

	public SensorData GetConductivity()
	{
		float conductivity = PluginInstance.Call<float>("getConductivity");
		SensorData data = new SensorData
		{
			isOnline = getNative("isConductivityOnline"),
			status = (int)SENSOR_STATUS.READY,
			values = new Dictionary<string, float>
		{
			{ OBSERVABLES.CONDUCTIVITY, conductivity }
		}
		};
		return data;
	}

	public SensorData GetVOC()
	{
		float voc = PluginInstance.Call<int>("getVoc");
		SensorData data = new SensorData
		{
			isOnline = getNative("isVocOnline"),
			status = (int)SENSOR_STATUS.READY,
			values = new Dictionary<string, float>
		{
			{ OBSERVABLES.VOC, voc }
		}
		};
		return data;
	}

    public SensorData GetUVLux()
    {
		float uv = PluginInstance.Call<float>("getUV");
		float lux = PluginInstance.Call<float>("getLux");
		SensorData data = new SensorData
		{
			isOnline = getNative("isUvOnline"),
			status = (int)SENSOR_STATUS.READY,
			values = new Dictionary<string, float>
		{
			{ OBSERVABLES.UV, uv },
			{ OBSERVABLES.LUX, lux }
		}
		};
		return data;
	}

    public SensorData GetHumidityTemperature()
    {
		float humidity = PluginInstance.Call<float>("getHumidity");
		float temperature = PluginInstance.Call<float>("getTemperature");
		SensorData data = new SensorData
		{
			isOnline = getNative("isHumidityOnline"),
			status = (int)SENSOR_STATUS.READY,
			values = new Dictionary<string, float>
		{
			{ OBSERVABLES.HUMIDITY, humidity },
			{ OBSERVABLES.TEMPERATURE, temperature }
		}
		};
		return data;
	}

	public SensorData GetBodyTemperature() {

		float ambient = PluginInstance.Call<int>("getAmbientTemperature");
		float infrared = PluginInstance.Call<int>("getInfraredTemperature");

		SensorData data = new SensorData
		{
			isOnline = getNative("isBodyTempOnline"),
			status = (int)SENSOR_STATUS.READY,
			values = new Dictionary<string, float>
		{
			{ OBSERVABLES.A_TEMPERATURE, ambient },
			{ OBSERVABLES.D_TEMPERATURE, infrared }
		}
		};
		return data;
	}

	public SensorData GetBodyTemperature2()
	{
		float ambient = PluginInstance.Call<int>("getAmbientTemperature");
		float infrared = PluginInstance.Call<int>("getInfraredTemperature");
		SensorData data = new SensorData
		{
			isOnline = getNative("isBodyTempOnline"),
			status = (int)SENSOR_STATUS.READY,
			values = new Dictionary<string, float>
		{
			{ OBSERVABLES.A_TEMPERATURE, ambient },
			{ OBSERVABLES.D_TEMPERATURE, infrared }
		}
		};
		return data;
	}

	public SensorData GetHeartRate() {
		float hr = PluginInstance.Call<int>("getHeartRate");
		SensorData data = new SensorData
		{
			isOnline = getNative("isHeartRateOnline"),
			status = (int)SENSOR_STATUS.READY,
			values = new Dictionary<string, float>
		{
			{ OBSERVABLES.HEART_RATE, hr }
		}
		};
		return data;
	}

	public SensorData GetColor()
    {
        float color_h = PluginInstance.Call<int>("getR");
        float color_s = PluginInstance.Call<int>("getG");
        float color_v = PluginInstance.Call<int>("getB");
        SensorData data = new SensorData
        {
            isOnline = getNative("isColorOnline"),
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
