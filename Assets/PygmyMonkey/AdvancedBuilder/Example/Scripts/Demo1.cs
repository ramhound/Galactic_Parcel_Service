using UnityEngine;
using PygmyMonkey.AdvancedBuilder;

public class Demo1 : MonoBehaviour
{
	void Start()
	{
		// You can use defaut defines from AdvancedBuilder
		#if PT_ANDROID
		Debug.Log("Android platform from define");
		#elif PT_IOS
		Debug.Log("iOS platform from define");
		#endif

		// Or just retrieve data from AppParameters
		if (AppParameters.Get.platformType.Equals("Android"))
		{
			Debug.Log("Android platform");
		}
		else if (AppParameters.Get.platformType.Equals("iOS"))
		{
			Debug.Log("iOS platform");
		}

		switch (AppParameters.Get.releaseType)
		{
		case "Dev":
			// Do something with the dev version of your app
			break;
			
		case "Beta":
			// Do something with the beta version of your app
			break;
			
		case "Release":
			// Do something with the release version of your app
			break;
		}
	}

	void OnGUI()
	{
		int paddingX = 5;
		int paddingY = 5;
		int width = 200;
		int height = 22;

		GUI.Label(new Rect(paddingX, paddingY, width, height), "Release type:");
		GUI.Label(new Rect(paddingX + width, paddingY, width, height), AppParameters.Get.releaseType);
		
		paddingY += height;
		GUI.Label(new Rect(paddingX, paddingY, width, height), "Platform type:");
		GUI.Label(new Rect(paddingX + width, paddingY, width, height), AppParameters.Get.platformType);
		
		paddingY += height;
		GUI.Label(new Rect(paddingX, paddingY, width, height), "Distribution platform:");
		GUI.Label(new Rect(paddingX + width, paddingY, width, height), AppParameters.Get.distributionPlatform);
		
		paddingY += height;
		GUI.Label(new Rect(paddingX, paddingY, width, height), "Platform architecture:");
		GUI.Label(new Rect(paddingX + width, paddingY, width, height), AppParameters.Get.platformArchitecture);
		
		paddingY += height;
		GUI.Label(new Rect(paddingX, paddingY, width, height), "Texture compression:");
		GUI.Label(new Rect(paddingX + width, paddingY, width, height), AppParameters.Get.textureCompression);

		paddingY += height;
		GUI.Label(new Rect(paddingX, paddingY, width, height), "Product name:");
		GUI.Label(new Rect(paddingX + width, paddingY, width, height), AppParameters.Get.productName);

		paddingY += height;
		GUI.Label(new Rect(paddingX, paddingY, width, height), "Bundle identifier:");
		GUI.Label(new Rect(paddingX + width, paddingY, width, height), AppParameters.Get.bundleIdentifier);

		paddingY += height;
		GUI.Label(new Rect(paddingX, paddingY, width, height), "Bundle version:");
		GUI.Label(new Rect(paddingX + width, paddingY, width, height), AppParameters.Get.bundleVersion);

		#if YOUR_CUSTOM_DEFINE
		paddingY += height;
		GUI.Label(new Rect(paddingX, paddingY, width, height), "YOUR_CUSTOM_DEFINE");
		#endif

		if (AppParameters.Get.platformType.Equals("Android"))
		{
			paddingY += height;
			GUI.Label(new Rect(paddingX, paddingY, width, height), "Texture Compression:");
			GUI.Label(new Rect(paddingX + width, paddingY, width, height), AppParameters.Get.textureCompression);
		}

		if (AppParameters.Get.platformType.Equals("Windows"))
		{
			paddingY += height;
			GUI.Label(new Rect(paddingX, paddingY, width, height), "Platform Architecture:");
			GUI.Label(new Rect(paddingX + width, paddingY, width, height), AppParameters.Get.platformArchitecture);
		}
		
		#if PT_ANDROID
		paddingY += height;
		GUI.Label(new Rect(paddingX, paddingY, width, height), "Platform Android (from define)");
		#elif PT_IOS
		paddingY += height;
		GUI.Label(new Rect(paddingX, paddingY, width, height), "Platform iOS (from define)");
		#elif PT_BLACKBERRY
		paddingY += height;
		GUI.Label(new Rect(paddingX, paddingY, width, height), "Platform BlackBerry (from define)");
		#elif PT_LINUX
		paddingY += height;
		GUI.Label(new Rect(paddingX, paddingY, width, height), "Platform Linux (from define)");
		#elif PT_MAC
		paddingY += height;
		GUI.Label(new Rect(paddingX, paddingY, width, height), "Platform Mac (from define)");
		#elif PT_WEBPLAYER
		paddingY += height;
		GUI.Label(new Rect(paddingX, paddingY, width, height), "Platform WebPlayer (from define)");
		#elif PT_WINDOWS
		paddingY += height;
		GUI.Label(new Rect(paddingX, paddingY, width, height), "Platform Windows (from define)");
		#elif PT_WINDOWSPHONE8
		paddingY += height;
		GUI.Label(new Rect(paddingX, paddingY, width, height), "Platform Windows Phone 8 (from define)");
		#elif PT_WINDOWSSTORE8
		paddingY += height;
		GUI.Label(new Rect(paddingX, paddingY, width, height), "Platform Windows Store 8 (from define)");
		#endif
	}
}
