using UnityEngine;
using UnityEditor;
using PygmyMonkey.AdvancedBuilder;

// This class MUST be in an "Editor" folder!
public class MyCustomBuild : IAdvancedCustomBuild
{
	/// <summary>
	/// Callback method that is called before each build
	/// </summary>
	public void OnPreBuild(Configuration configuration, System.DateTime buildDate)
	{
		AdvancedBuilder advancedBuilder = (AdvancedBuilder)AssetDatabase.LoadAssetAtPath("Assets/PygmyMonkey/AdvancedBuilder/Editor/AdvancedBuilder.asset", typeof(AdvancedBuilder));

		// Print the build destination path
		string buildDestinationPath = configuration.getBuildDestinationPath(advancedBuilder.getAdvancedSettings(), buildDate, advancedBuilder.getProductParameters().bundleVersion);
		Debug.Log(buildDestinationPath);

		// You can retrieve multiple variables from the configuration class
		// Release type name: configuration.releaseType.name
		// Platform name: configuration.platformType.ToString()
		// Distribution platform name: configuration.distributionPlatform.name
		// Platform architecture name: configuration.platformArchitecture.name
		// Texture compression name: configuration.textureProperties.name
		Debug.Log("Do stuff before build");
	}

	/// <summary>
	/// Callback method that is called after each build
	/// </summary>
	public void OnPostBuild(Configuration configuration, System.DateTime buildDate)
	{
		Debug.Log("Do stuff after build");
	}
}
