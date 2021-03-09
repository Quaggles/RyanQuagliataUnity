// ReSharper disable RedundantUsingDirective
using UnityEngine;
using UnityEngine.iOS;

namespace RyanQuagliataUnity.Extensions {
	public static class ScreenExtensions {
		public static float DpiExtended {
			get {
				var unityDpi = Screen.dpi;
				if (unityDpi > 0) return unityDpi; 
#if UNITY_IOS
				// ReSharper disable once RedundantNameQualifier
				switch (UnityEngine.iOS.Device.generation) {
					// iPod touch
					case DeviceGeneration.iPodTouch1Gen:
					case DeviceGeneration.iPodTouch2Gen:
					case DeviceGeneration.iPodTouch3Gen:
						return 126;
					case DeviceGeneration.iPodTouch4Gen:
					case DeviceGeneration.iPodTouch5Gen:
					case DeviceGeneration.iPodTouch6Gen:
					case DeviceGeneration.iPodTouch7Gen:
						return 326;
					// iPad
					case DeviceGeneration.iPad1Gen:
					case DeviceGeneration.iPad2Gen:
						return 132;
					case DeviceGeneration.iPad3Gen:
					case DeviceGeneration.iPad4Gen:
					case DeviceGeneration.iPad5Gen:
					case DeviceGeneration.iPad6Gen:
					case DeviceGeneration.iPad7Gen:
						return 264;
					// iPad Air
					case DeviceGeneration.iPadAir1:
					case DeviceGeneration.iPadAir2:
					case DeviceGeneration.iPadAir3Gen:
						return 264;
					// iPad Pro
					case DeviceGeneration.iPadPro1Gen:
					case DeviceGeneration.iPadPro2Gen:
					case DeviceGeneration.iPadPro3Gen:
					case DeviceGeneration.iPadPro4Gen:
					case DeviceGeneration.iPadPro10Inch1Gen:
					case DeviceGeneration.iPadPro11Inch:
					case DeviceGeneration.iPadPro10Inch2Gen:
					case DeviceGeneration.iPadPro11Inch2Gen:
						return 264;
					case DeviceGeneration.iPadUnknown:
						return 264;
					// iPad Mini
					case DeviceGeneration.iPadMini1Gen:
					case DeviceGeneration.iPadMini2Gen:
					case DeviceGeneration.iPadMini3Gen:
					case DeviceGeneration.iPadMini4Gen:
					case DeviceGeneration.iPadMini5Gen:
						return 326;
					// iPhone
					case DeviceGeneration.iPhone:
					case DeviceGeneration.iPhone3G:
					case DeviceGeneration.iPhone3GS:
						return 163;
					case DeviceGeneration.iPhone4:
					case DeviceGeneration.iPhone4S:
					case DeviceGeneration.iPhone5:
					case DeviceGeneration.iPhone5C:
					case DeviceGeneration.iPhone5S:
					case DeviceGeneration.iPhone6:
					case DeviceGeneration.iPhone6S:
					case DeviceGeneration.iPhone7:
					case DeviceGeneration.iPhone8:
					case DeviceGeneration.iPhoneSE1Gen:
					case DeviceGeneration.iPhoneSE2Gen:
						return 326;
					// iPhone Plus
					case DeviceGeneration.iPhone6Plus:
					case DeviceGeneration.iPhone6SPlus:
					case DeviceGeneration.iPhone7Plus:
					case DeviceGeneration.iPhone8Plus:
						return 401;
					case DeviceGeneration.iPhoneXR:
						return 326;
					case DeviceGeneration.iPhoneX:
					case DeviceGeneration.iPhoneXS:
					case DeviceGeneration.iPhoneXSMax:
					case DeviceGeneration.iPhone11:
					case DeviceGeneration.iPhone11Pro:
					case DeviceGeneration.iPhone11ProMax:
						return 458;
					// These enums not defined in Unity 2018.4.22f1
					// case DeviceGeneration.iPhone12
					// return 460;
					// case DeviceGeneration.iPhone12Mini
					// return 476;
					case DeviceGeneration.Unknown:
					case DeviceGeneration.iPhoneUnknown:
					case DeviceGeneration.iPodTouchUnknown:
						return 326;
					default:
						return 326;
				}
#elif UNITY_ANDROID
				AndroidJavaClass activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			    AndroidJavaObject activity = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
			 
			    AndroidJavaObject metrics = new AndroidJavaObject("android.util.DisplayMetrics");
			    activity.Call<AndroidJavaObject>("getWindowManager").Call<AndroidJavaObject>("getDefaultDisplay").Call("getMetrics", metrics);
			 
			    return (int)((metrics.Get<float>("xdpi") + metrics.Get<float>("ydpi")) * 0.5f);
#endif
				return 0;
			}
		}
	}
}