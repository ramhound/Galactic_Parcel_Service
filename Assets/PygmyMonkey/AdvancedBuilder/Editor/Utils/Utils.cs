using UnityEngine;
using UnityEditor;
using System.Linq;

namespace PygmyMonkey.AdvancedBuilder
{
	public static class Utils
	{
		public static string[] GetActiveScenePathArray()
		{
			return EditorBuildSettings.scenes.Where(x => x.enabled).Select(x => x.path).ToArray();
		}
	}
}