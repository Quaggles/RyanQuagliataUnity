using QFSW.QC;
using UnityEngine;

namespace RyanQuagliataUnity.Extensions.QuantumConsole {
	[CommandPrefix("SystemInfo")]
	public static class SystemInfoCommands {
		[Command] public static string DeviceModel => SystemInfo.deviceModel;
		[Command] public static string DeviceName => SystemInfo.deviceName;
		[Command] public static string OperatingSystem => SystemInfo.operatingSystem;
		[Command] public static string ProcessorType => SystemInfo.processorType;
		[Command] public static string UnsupportedIdentifier => SystemInfo.unsupportedIdentifier;
		[Command] public static string DeviceUniqueIdentifier => SystemInfo.deviceUniqueIdentifier;
		[Command] public static string GraphicsDeviceName => SystemInfo.graphicsDeviceName;
		[Command] public static string GraphicsDeviceVendor => SystemInfo.graphicsDeviceVendor;
		[Command] public static string GraphicsDeviceVersion => SystemInfo.graphicsDeviceVersion;
		[Command] public static float BatteryLevel => SystemInfo.batteryLevel;
		[Command] public static int ProcessorCount => SystemInfo.processorCount;
		[Command] public static int ProcessorFrequency => SystemInfo.processorFrequency;
	}
}