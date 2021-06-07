using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

	public Text sensor_value;
	public Image sensor_graphic;
	public bool isSensorOnline;
	public string sensorName;
	public string propertyName;
	public int noOfDecimals = 0;

	void Awake()
	{
		sensor_value = GetComponentInChildren<Text>();
		sensor_graphic = GetComponent<Image>();
		sensorName = name.Split('.')[0];
		propertyName = name.Split('.')[1];
	}
	
	void Update () {
		isSensorOnline = KiwriousSerialReader.instance.sensorData[sensorName].isOnline;
		sensor_value.enabled = sensor_graphic.enabled = isSensorOnline;
		if (isSensorOnline)
		{
			sensor_value.text = KiwriousSerialReader.instance.sensorData[sensorName].values[propertyName].ToString($"F{noOfDecimals}");
		}
	}
}
