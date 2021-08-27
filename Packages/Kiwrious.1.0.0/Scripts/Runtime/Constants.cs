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
			THERMAL = 2,
			COLOR = 3,
			EC = 4,
			CARDIO = 5,
			VOC = 6,
			CLIMATE = 7,
			SOUND = 8,
			THERMAL2 = 9
		}
		public const uint MAX_CONDUCTANCE_VALUE = 200000;
		public const uint MIN_CONDUCTANCE_VALUE = 65535;
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
