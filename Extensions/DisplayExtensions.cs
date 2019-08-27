#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
#define WINDOW_POSTION_SUPPORTED
#endif

using System;
// ReSharper disable once RedundantUsingDirective
using System.Runtime.InteropServices;
using UnityEngine;

namespace RyanQuagliataUnity.Extensions {
	public static class DisplayExtensions {
		private const string X_POSITION_SWITCH_NAME = "-screen-x";
		private const string Y_POSITION_SWITCH_NAME = "-screen-y";
		private const string FULLSCREEN_MODE_SWITCH_NAME = "-screen-fullscreenMode";

		/// <summary>
		/// Runs automatically when the game starts
		/// </summary>
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		static void SetAtLaunch() {
			try {
				SetFullScreenModeFromCommandLineArguments();
			} catch (CommandLineArguments.CommandLineArgumentNotFoundException) {
				// Ignore, we really don't care if they didn't provide the arguments
			} catch (Exception ex) {
				// Something is actually wrong
				Debug.LogError($"Cannot set fullscreen mode, reason: {ex}");
			}
			
			try {
				SetScreenPositionFromCommandLineArguments();
			} catch (CommandLineArguments.CommandLineArgumentNotFoundException) {
				// Ignore, we really don't care if they didn't provide the arguments
			} catch (Exception ex) {
				// Something is actually wrong
				Debug.LogError($"Cannot set window position, reason: {ex}");
			}
		}

		/// <summary>
		/// Reads in the X_POSITION_SWITCH_NAME and Y_POSITION_SWITCH_NAME arguments and uses them to set the window position 
		/// </summary>
		/// <exception cref="Exception"></exception>
		public static void SetScreenPositionFromCommandLineArguments() {
			Vector2Int screenPosition = new Vector2Int {
				x = int.Parse(CommandLineArguments.ReadArgValue(X_POSITION_SWITCH_NAME)),
				y = int.Parse(CommandLineArguments.ReadArgValue(Y_POSITION_SWITCH_NAME))
			};
			if (Screen.fullScreen) throw new Exception("Cannot set window position in fullscreen");
			SetPosition(screenPosition);
			Debug.Log($"Successfully set screen position to {screenPosition.ToString()}");
		}

		public static void SetFullScreenModeFromCommandLineArguments() {
			Screen.fullScreenMode = (FullScreenMode) int.Parse(CommandLineArguments.ReadArgValue(FULLSCREEN_MODE_SWITCH_NAME));
		}


#if WINDOW_POSTION_SUPPORTED
		[DllImport("user32.dll", EntryPoint = "SetWindowPos")]
		private static extern bool SetWindowPos(IntPtr hwnd, int hWndInsertAfter, int x, int Y, int cx, int cy,
			int wFlags);

		[DllImport("user32.dll", EntryPoint = "FindWindow")]
		private static extern IntPtr FindWindow(string className, string windowName);

		[DllImport("user32.dll", EntryPoint = "GetActiveWindow")]
		private static extern IntPtr GetActiveWindow();
#endif
		/// <summary>
		/// Sets the window position
		/// </summary>
		/// <param name="position">Window position relative to the top left corner of the main display</param>
		/// <param name="size">Window size</param>
		public static void SetPosition(Vector2Int position, Vector2Int size = default) =>
			SetPosition(position.x, position.y, size.x, size.y);

		/// <summary>
		/// Sets the window position
		/// </summary>
		/// <param name="x">Window x position relative to the top left corner of the main display</param>
		/// <param name="y">Window y position relative to the top left corner of the main display</param>
		/// <param name="resX">Window width</param>
		/// <param name="resY">Window height</param>
		public static void SetPosition(int x, int y, int resX = 0, int resY = 0) {
#if WINDOW_POSTION_SUPPORTED
			SetWindowPos(GetActiveWindow(), 0, x, y, resX, resY, resX * resY == 0 ? 1 : 0);
#else
			throw new WindowPositionNotSupported();
#endif
		}

		public class WindowPositionNotSupported : Exception {
			public override string Message =>
				$"This platform <b>({Application.platform.ToString()})</b> does not support changing the window position, it is only supported on <b>{RuntimePlatform.WindowsPlayer.ToString()}</b>";
		}
	}
}