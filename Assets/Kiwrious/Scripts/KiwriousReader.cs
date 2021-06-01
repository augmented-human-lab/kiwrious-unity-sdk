using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace kiwrious {
	public class KiwriousReader
	{

		private KiwriousReader kiwriousReader;
		private static KiwriousReader instance;

		private SensorData conductivity;
		private SensorData voc;
		private SensorData uv_lux;
		private SensorData humidity_temperature;
		private SensorData color;

		public virtual SensorData GetConductivity()
		{
			return null;
		}

		public virtual SensorData GetVOC()
		{
			return null;
		}

		public virtual SensorData GetUVLux()
		{
			return null;
		}

		public virtual SensorData GetHumidityTemperature()
		{
			return null;
		}

		public virtual SensorData GetColor()
		{
			return null;
		}


	}

	public enum SENSOR_STATUS
	{
		OFFLINE = 0,
		READY = 1,
		PROCESSING = 2
	}

	[Serializable]
	public class SensorData
	{
		public bool isOnline { get; set; } // to be removed, use status
		public int status { get; set; }
		public Dictionary<string, float> values { get; set; }
	}


}
