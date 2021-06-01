using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ColorController : KiwriousController {

	public string colorProperty;

	void Awake()
	{
		sensor_value = GetComponentInChildren<Text>();
        sensor_graphic = GetComponent<Image>();
        colorProperty = name.Split('_')[0];
	}

    void Update () {
		sensor_value.enabled = KiwriousSerialReader.instance.color.isOnline;
		sensor_graphic.enabled = KiwriousSerialReader.instance.color.isOnline;
		if (sensor_value.enabled)
		{
			sensor_value.text = SerialReader.instance.hsv[colorProperty].ToString();
			sensor_value.text = KiwriousSerialReader.instance.color.values["ColorH"].ToString();
		}
	}

}
