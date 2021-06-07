namespace Assets.Kiwrious.Scripts
{
    public class Constants
    {
		public enum SENSOR_STATUS
		{
			OFFLINE = 0,
			READY = 1,
			PROCESSING = 2
		}
		public enum SENSOR_TYPE
		{
			Uv = 1,
			Color = 3,
			Conductivity = 4,
			Humidity = 7,
			VOC = 6,
			BODY_TEMP = 2,
			SOUND = 8,
			HEART_RATE = 5
		}
	}

	public class OBSERVABLES {
		public static string UV = "UV";
		public static string CONDUCTIVITY = "Conductivity";
		public static string HUMIDITY = "Humidity";
		public static string TEMPERATURE = "Temperature";
		public static string LUX = "Lux";
		public static string COLOR_H = "ColorH";
		public static string COLOR_S = "ColorS";
		public static string COLOR_V = "ColorV";
		public static string VOC = "VOC";
	}

	
}
