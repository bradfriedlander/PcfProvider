using System;
using System.Collections.Generic;
using System.IO;

namespace PcfProvider
{
	public static class Helpers
	{
		/// <summary>
		///     This method checks if <paramref name="value" /> is null, default value, or empty.
		/// </summary>
		/// <typeparam name="T">This is the type of <paramref name="value" />.</typeparam>
		/// <param name="value">This is the value to check.</param>
		/// <returns><c>true</c> if <paramref name="value" /> is null or empty; otherwise, <c>false</c>.</returns>
		public static bool CheckNullOrEmpty<T>(T value)
		{
			if (typeof(T) == typeof(string))
			{
				return string.IsNullOrEmpty(value as string);
			}
			return EqualityComparer<T>.Default.Equals(value, default(T));
		}

		/// <summary> <token method save saves the <paramref name="serializedJson"/> to a file using <see cref="DateTime.UtcNow"/> and <paramref
		/// name="label"/>. </summary> <param name="label">This is the label for the file.</param> <param name="serializedJson">This is the serialized
		/// json string to be saved.</param> <returns>This is always the unmodified <paramref name="serializedJson"/>. This is done to support fluent constructs.</returns>
		public static string SaveJson(string label, string serializedJson)
		{
			var filename = $"{DateTime.UtcNow.ToString("yyyy-MM-dd_HH-mm-ss")}_{label}.json";
			if (File.Exists(filename))
			{
				File.Delete(filename);
			}
			File.AppendAllText(filename, serializedJson);
			return serializedJson;
		}
	}
}