using System;
using System.IO;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

namespace RyanQuagliata.Extensions {
	public static class StreamExtensions {
		public static void Write(this Stream that, float value) {
			that.Write(BitConverter.GetBytes(value));
		}
        
        public static void Write(this Stream that, int value) {
			that.Write(BitConverter.GetBytes(value));
		}
		
		public static void Write(this Stream that, Vector3 value) {
			that.Write(BitConverter.GetBytes(value.x));
			that.Write(BitConverter.GetBytes(value.y));
			that.Write(BitConverter.GetBytes(value.z));
		}

		public static void Write(this Stream that, ushort value) {
			that.Write(BitConverter.GetBytes(value));
		}

		public static void Write(this Stream that, uint value) {
			that.Write(BitConverter.GetBytes(value));
		}

		public static void Write(this Stream that, byte[] buffer) {
			that.Write(buffer, 0, buffer.Length);
		}

		public static async Task WriteAsync(this Stream that, byte[] buffer) {
			await that.WriteAsync(buffer, 0, buffer.Length);
		}

		public static void SeekBegin([NotNull] this Stream that) {
			that.Seek(0, SeekOrigin.Begin);
		}
	}
}
