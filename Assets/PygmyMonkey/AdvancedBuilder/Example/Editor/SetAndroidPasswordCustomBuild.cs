using UnityEngine;
using UnityEditor;
using System;
using PygmyMonkey.AdvancedBuilder;

// This class MUST be in an "Editor" folder!
// This is an example of a custom build script that will set your Android keystore passwords so you don't have to enter them manually every time.
// WARNING: If you're using this method, this means your Android password is stored in clear text inside this file!
// This file will not be part of the final binary (build) because this file is inside an Editor folder, BUT it will
// be available to anyone working with your source code, and will be commited to your versioning file system (if you
// use one). So if you don't want your password to be seen by others having access to this file. Do not use it :)
public class SetAndroidPasswordCustomBuild : IAdvancedCustomBuild
{
	/// <summary>
	/// Callback method that is called before each build
	/// </summary>
	public void OnPreBuild(Configuration configuration, DateTime buildDate)
	{
		if (configuration.platform.getPlatformProperties().platformType.Equals(PlatformType.Android))
		{
			// Here is the keystore file that you normaly select via the file browser using the "Browse Keystore" button in PlayerSettings
			// If you have the keystore file inside this Unity project, simply set: "Assets/MyFolder/MyKeystore.keystore"
			// If the keystore file is outside this Unity project, you have to specify the full path, example:
			// Mac: "/Users/xxx/Projects/Assets/Other/mykeystore.keystore"
			// Windows: @"C:\Projects\Assets\Other\mykeystore.keystore"
			PlayerSettings.Android.keystoreName = "FULL_PATH_TO_KEYSTORE_FILE";

			// Your Android keystore file password
            PlayerSettings.Android.keystorePass = "KEYSTORE_PASSWORD";

			// The name of the alias in the keystore file
			PlayerSettings.Android.keyaliasName = "KEY_ALIAS_NAME";

			// THe password for the alias
			PlayerSettings.Android.keyaliasPass = "KEY_ALIAS_PASSWORD";

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}
	}

	/// <summary>
	/// Callback method that is called after each build
	/// </summary>
	public void OnPostBuild(Configuration configuration, DateTime buildDate)
	{
		if (configuration.platform.getPlatformProperties().platformType.Equals(PlatformType.Android))
		{
			PlayerSettings.Android.keystoreName = null;
			PlayerSettings.Android.keystorePass = null;
			PlayerSettings.Android.keyaliasName = null;
			PlayerSettings.Android.keyaliasPass = null;

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}
	}
}
