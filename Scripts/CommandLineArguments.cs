using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using JetBrains.Annotations;
using UnityEngine;

namespace RyanQuagliataUnity {
	public static class CommandLineArguments {
		private static readonly List<string> Arguments = new List<string>();
		private static bool environmentArgsAdded = false;

		public static void AddArguments(IEnumerable<string> arguments) => Arguments.AddRange(arguments);

		private static void AddEnvironmentArgs() {
			Arguments.AddRange(Environment.GetCommandLineArgs());
			environmentArgsAdded = true;
		}

		/// <summary>
		/// Outputs all currently loaded arguments into the console
		/// </summary>
		private static void LogArguments() {
			Debug.Log($"{Arguments.Count} command line arguments");
			for (var i = 0; i < Arguments.Count; i++) {
				var arg = Arguments[i];
				Debug.Log($"[{i}]\t{arg}");
			}
		}

		/// <summary>
		/// Finds whether an argument exists
		/// </summary>
		/// <param name="argName">Name of the argument e.g. -windowed</param>
		/// <returns>True if the argument exists, false if it doesn't</returns>
		public static bool IsArgumentPresent(string argName) {
			if (!environmentArgsAdded) AddEnvironmentArgs();
			if (Arguments.Contains(argName, StringComparer.CurrentCulture)) return true;
			return false;
		}

		/// <summary>
		/// Finds an argument in the list and returns the following value
		/// </summary>
		/// <param name="argName">Name of the argument e.g. -windowWidth</param>
		/// <param name="enforceValueType">If true an exception will be thrown if the value after the provided argument starts with a '-' character</param>
		/// <exception cref="CommandLineArgumentNotValidException">Argument was not the valid type</exception>
		/// <exception cref="CommandLineArgumentNotFoundException">Argument was not provided</exception>
		/// <returns>The value after the argument name</returns>
		public static string ReadArgValue(string argName, bool enforceValueType = true) {
			if (!environmentArgsAdded) AddEnvironmentArgs();
			bool skippedDueToEnforcedValueType = false;
			
			for (var i = 0; i < Arguments.Count; i++) {
				var commandLineArgument = Arguments[i];
				// Skip arguments that don't match
				if (!string.Equals(commandLineArgument, argName, StringComparison.InvariantCultureIgnoreCase)) continue;
				
				// Once the argument matches get the following value
				string val = Arguments[i + 1];
				
				// If enforcing value type check that it doesn't start with a hyphen, if it does skip
				if (enforceValueType && val.StartsWith("-")) {
					skippedDueToEnforcedValueType = true;
					continue;
				}
				
				return val;
			}

			if (skippedDueToEnforcedValueType)
				throw new CommandLineArgumentNotValidException($"Argument \"{argName}\" was skipped as it did a valid value following it");
			throw new CommandLineArgumentNotFoundException($"Argument \"{argName}\" was not found in the list of command line arguments");
		}

		/// <summary>
		/// Finds an argument in the list and returns a number of following values
		/// </summary>
		/// <param name="argName">Name of the argument e.g. -windowSize</param>
		/// <param name="count">Number of values expected after the argument</param>
		/// <param name="enforceValueType">If true an exception will be thrown if the values after the provided argument starts with a '-' character</param>
		/// <returns>A set of values after the argument name</returns>
		/// <exception cref="IndexOutOfRangeException">Value count exceeded the length of the command line args</exception>
		public static string[] ReadArgValues(string argName, int count, bool enforceValueType = true) {
			if (!environmentArgsAdded) AddEnvironmentArgs();
			bool skippedDueToEnforcedValueType = false;
			for (var i = 0; i < Arguments.Count; i++) {
				var commandLineArgument = Arguments[i];
				if (!string.Equals(commandLineArgument, argName, StringComparison.InvariantCultureIgnoreCase)) continue;
				
				// If enforcing value type check that it doesn't start with a hyphen, if it does skip
				if (enforceValueType) {
					bool breakOut = false;
					for (int j = i+1; j < i+count+1; j++) {
						if (Arguments[j].StartsWith("-")) {
							breakOut = true;
							skippedDueToEnforcedValueType = true;
						}
					}

					if (breakOut) continue;
				}

				string[] result = new string[count];
				Arguments.CopyTo(i+1, result, 0, count);
				return result;
			}

			if (skippedDueToEnforcedValueType)
				throw new ArgumentException($"Argument \"{argName}\" was skipped as it did not have {count} valid values following it");
			throw new ArgumentException($"Argument \"{argName}\" was not found in the list of command line arguments");
		}

		public class CommandLineArgumentNotFoundException : Exception {
			public CommandLineArgumentNotFoundException() { }
			protected CommandLineArgumentNotFoundException([NotNull] SerializationInfo info, StreamingContext context) : base(info, context) { }
			public CommandLineArgumentNotFoundException(string message) : base(message) { }
			public CommandLineArgumentNotFoundException(string message, Exception innerException) : base(message, innerException) { }
		}
		public class CommandLineArgumentNotValidException : Exception {
			public CommandLineArgumentNotValidException() { }
			protected CommandLineArgumentNotValidException([NotNull] SerializationInfo info, StreamingContext context) : base(info, context) { }
			public CommandLineArgumentNotValidException(string message) : base(message) { }
			public CommandLineArgumentNotValidException(string message, Exception innerException) : base(message, innerException) { }
		}
	}
}