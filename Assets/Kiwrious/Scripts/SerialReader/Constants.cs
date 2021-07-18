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
			LIGHT = 1,
			COLOR = 3,
			EC = 4,
			CLIMATE = 7,
			VOC = 6,
			THERMAL = 2,
			SOUND = 8,
			CARDIO = 5
		}
	}

	public class OBSERVABLES {
		public static string UV = "uv";
		public static string CONDUCTIVITY = "conductivity";
		public static string HUMIDITY = "humidity";
		public static string TEMPERATURE = "temperature";
		public static string LUX = "lux";
		public static string COLOR_H = "color_h";
		public static string COLOR_S = "color_s";
		public static string COLOR_V = "color_v";
		public static string VOC = "voc";
		public static string HEART_RATE = "heart_rate";
		public static string D_TEMPERATURE = "d_temperature";
		public static string A_TEMPERATURE = "a_temperature";
	}


}
