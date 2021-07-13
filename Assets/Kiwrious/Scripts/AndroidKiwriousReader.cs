using Assets.Kiwrious.Scripts;
using kiwrious;
using System.Collections.Generic;
using UnityEngine;
using static Assets.Kiwrious.Scripts.Constants;

public class AndroidKiwriousReader : KiwriousReader
{

    //private const string pluginName = "org.ahlab.kiwrious.android.Plugin";
    private static AndroidJavaClass _pluginClass;
    private static AndroidJavaObject _pluginInstance;

	private const string applicationName = "org.ahlab.kiwrious.android.Application";
	private static AndroidJavaObject _applicatonObject;

	//private const string appName = "com.unity3d.player.UnityPlayer";
	private static AndroidJavaClass _unityClass;
	private static AndroidJavaObject _unityInstance;
	private static AndroidJavaObject _unityContext;

	public static AndroidJavaClass PluginClass
	{
		get
		{
			if (_pluginClass == null)
			{
				_pluginClass = new AndroidJavaClass("org.ahlab.kiwrious.android.Plugin");
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
				
				_unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
				_unityInstance = _unityClass.GetStatic<AndroidJavaObject>("currentActivity");
				_unityContext = _unityInstance.Call<AndroidJavaObject>("getApplicationContext");
				
				_applicatonObject = new AndroidJavaObject("org.ahlab.kiwrious.android.Application", _unityContext);
				
				_pluginInstance = PluginClass.CallStatic<AndroidJavaObject>("getInstance");
				_pluginInstance.Call("initiateReader");
				_pluginInstance.Call<bool>("startSerialReader");
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
			isOnline = getNative("isConductivityOnline"),
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

    public override SensorData GetUVLux()
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

    public override SensorData GetHumidityTemperature()
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

    public override SensorData GetColor()
    {
		//float color_h = callNative("getColorH");
		//float color_s = callNative("getColorS");
		//float color_v = callNative("getColorV");
		//SensorData data = new SensorData
		//{
		//	isOnline = getNative("color_online"),
		//	status = (int)SENSOR_STATUS.READY,
		//	values = new Dictionary<string, float>
		//{
		//	{ OBSERVABLES.COLOR_H, color_h },
		//	{ OBSERVABLES.COLOR_S, color_s },
		//	{ OBSERVABLES.COLOR_V, color_v }
		//}
		//};
		return new SensorData();
		//return data;
	}

	private float callNative(string methodName) {
		return PluginInstance.Call<float>(methodName);
	}

	private bool getNative(string propertyName)
	{
		return PluginInstance.Get<bool>(propertyName);
	}

}
