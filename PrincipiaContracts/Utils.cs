using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PrincipiaContracts
{
	public static class Utils
	{
		private const string logPreix = "[PrincipiaContracts] - ";
		public static void Log(params string[] message)
		{
			var builder = StringBuilderCache.Acquire();
			builder.Append(logPreix);
			foreach (string part in message) {
				builder.Append(part);
			}
			UnityEngine.Debug.Log(builder.ToStringAndRelease());
		}

		public static string FormatDistance(double distance)
		{
			return distance.ToString();
		}

		public static string FormatAngle(double angle)
		{
			return angle.ToString();
		}
	}
}
