using System.Collections;
using System.Linq;
using UnityEngine;

public class HumidityController : KiwriousController {

	void Update () {
		sensor_value.enabled = KiwriousSerialReader.instance.humidity_temperature.isOnline;
		sensor_graphic.enabled = KiwriousSerialReader.instance.humidity_temperature.isOnline;
		if (sensor_value.enabled)
		{
			sensor_value.text = KiwriousSerialReader.instance.humidity_temperature.values["Humidity"].ToString("F0");
		}
	}

}
