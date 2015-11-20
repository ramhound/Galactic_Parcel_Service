-------------------------------------------------------------------------------------------------
                                        Advanced Builder
                                         Version 1.4.3
                                       PygmyMonkey Tools
                                     tools@pygmymonkey.com
                      http://www.pygmymonkey.com/tools/advanced-builder/
-------------------------------------------------------------------------------------------------

Thank you for buying Advanced Builder!

If you have questions, suggestions, comments or feature requests, please send us an email
at tools@pygmymonkey.com



-------------------------------------------------------------------------------------------------
                            Support, Documentation, Examples and FAQ
-------------------------------------------------------------------------------------------------

You can find everything at http://www.pygmymonkey.com/tools/advanced-builder/



-------------------------------------------------------------------------------------------------
                                  How to update Advanced Builder
-------------------------------------------------------------------------------------------------
-- IF YOU ARE UPGRADING FROM A VERSION OLDER THAN 1.4.0, PLEASE FOLLOW THESE SMALL STEPS --
1. Write down and save somewhere all the configurations you have in the Advanced Builder window
2. Close the AdvancedBuilder window,
3. Remove entirely the AdvancedBuilder folder,
4. Import the new version of AdvancedBuilder,
5. Open the AdvancedBuilder window
6. Rewrite all your configurations from what you saved on step 1
7. You're goo to go :)

-- NORMAL PROCEDURE --
1. Close the Advanced Builder window
2. Delete everything under the 'AdvancedBuilder' folder from the Project View, EXCEPT the file
PygmyMonkey/AdvancedBuilder/Editor/AdvancedBuilder.asset
3. Import the latest version from the Asset Store


-------------------------------------------------------------------------------------------------
                                           Get Started
-------------------------------------------------------------------------------------------------

This tool allows you to easily build different "versions" of your game on multiple platforms.
We're going to rapidly describe the different parts of the tool and use some examples.
You can launch the interface by going to "Window/PygmyMonkey/Advanced Builder"


-------------------------------------- Product Parameters ---------------------------------------
Here you will define the global parameters for your product.
Currently you can only specify the bundle version, but more will be added in future releases.
Thanks to AdvancedBuilder, you can access the bundle version of your product during gameplay,
using: "AppParameters.Get.bundleVersion" (something you can't do with the current Unity API).


----------------------------------------- Release Types -----------------------------------------
Let's say your game have a Free and a Paid version (or Demo and Full).
Before, you would have to change the bundle identifier, product name and more before each build,
and, if you needed to know which version you were on during runtime, you would have to set (and
update) a boolean somewhere in your code to store this information.
Now, you can just add two release types, define the bundle identifier and product name for each,
click on build and you will have two executables for your product.
Then, during runtime, you can retrieve the release type using "AppParameters.Get.releaseType" or
use default defines such as #RT_RELEASE.


------------------------------------------- Platforms -------------------------------------------
In this section, you can add all the platforms your product supports, and also select some
per-platform parameters such as:

--- Distribution platforms ---
This is very useful if you want to publish your product to different sub-platforms:
- For Android, you may want to deliver your product through Google Play, but also for the Amazon
Appstore and maybe the Samsung AppStore. Well, you just have to add these 3 distribution platforms
and Advanced Builder will automatically build 3 different builds for each of these sub-platforms.
This is handy if you want to support In-App purchase for example, because for that, you need to
call the IAP process from GooglePlay if users downloaded from the Google Play, and same thing
for the Amazon Store. As there is no way of determining if a user downloaded your product through
Google Play or Amazon Store, you can simply use 2 different APKs, and using the variable
AppParameters.Get.distributionPlatform, you can call the IAP method from the correct platform.
- For the Web, you may want to have different builds, for your own website and for Kongregate.
- Same thing for Windows, maybe you want a version that you will sell on your website, and another
that you will sell on Steam (and do different things in the code with it).

--- Textures Compression ---
(used by Android and BlackBerry)
If you use 'Generic', it will build using the default texture compression.
Let's take Android for example. If you want, you can use different texture compressions that will
generate multiple APKs, that you will upload to Google. The Play Store will then automatically
deliver the correct APK depending on the device downloading your product. With this, users will
have an optimized version with texture compressions adapted to their device.

--- Architectures ---
Using this you can simultaneously build for :
- Windows x86 and/or Windows x64
- OSX x86, OSX x64 and/or OSX Universal
- Linux x86, Linux x86_64 and/or Linux Universal
- Android x86, ARMv7 and/or FAT (ARMv7 + x86)
- WebPlayer and/or WebPlayer Streamed
- D3D11 C++, D3D11 C#, XAML C++ and/or XAML C# (for Windows Store 8)
- Local and/or Signed (for BlackBerry)


--------------------------------------- Advanced Settings ---------------------------------------
--- Build folder path ---
Here you can define the destination of your builds.
By default it will be in /Builds/BUILD_DATE/RELEASE_TYPE/PLATFORM/DISTRIB_PLATFORM/ARCHITECTURE/,
some examples:
	/Builds/13-12-16 10h23/Dev/iOS/MyProduct/
	/Builds/13-12-16 10h23/Dev/Android/Google Play/MyProduct - Generic.apk
	/Builds/13-12-16 10h23/Dev/Android/Amazon App Store/MyProduct - Generic.apk
	/Builds/13-12-16 10h23/Beta/Windows/Windows x86/MyProduct.exe
You can personalize the build destination by modifying the build folder path and using, or not,
the different parameters at your disposal :
- $BUILD_DATE
- $RELEASE_TYPE
- $PLATFORM
- $DISTRIB_PLATFORM
- $ARCHITECTURE
- $TEXT_COMPRESSION
- $PRODUCT_NAME
- $BUNDLE_VERSION
- $BUNDLE_IDENTIFIER

--- Custom settings ---
-- Custom Build script --
You can create a custom script that allow you to do actions before and after each build.
This script needs to implement the interface IAdvancedCustomBuild and be placed in an Editor
folder. You will have to implement the two methods OnPreBuild and OnPostBuild.
These two methods have different parameters that will tell you information on the build that is
currently processing, allowing you to do different things based on the platform, the release type,
the distribution platform, the platform architecture and the texture compression.
You can find examples of custom build scripts in "Example/Editor/".

-- Global custom defines --
You can specify custom scripting define symbols that will be used in your project. For example,
you could use DEBUG and PRINT_ERRORS defines by filling the global custom define field with
DEBUG;PRINT_ERRORS
With that, in your code, you could use precompiled conditions such as #if DEBUG etc...


------------------------------------- Project Configurations -------------------------------------
This section will list all the configurations available based on what you used in 'Release Types',
and 'Platforms' sections. You then add the ones you want so they can be used with the batch build.

You can then click on each configuration to see the different information associated with it.
You'll see :
- a field so you can specify the name of the configuration if you need it (optional)
- a list of default defines that are automatically created based on the release type you created,
the platform, the platform architecture and the texture compression of the configuration
- a field where you can specify your custom defines associated with this configuration, so for
each and every build, you can have a specific custom define. More information can be found in the
Unity documentation (http://docs.unity3d.com/Manual/PlatformDependentCompilation.html)
- a list of scenes you want to build for this configuration
- You'll also see all the information associated with this configuration
- You can also directly build this particular configuration, pressing the Build button
- And finally, the "Apply this config" button, that is very useful if you want to test a
configuration of your game directly in the Editor. It will apply all the parameters specified in
this configuration directly in Unity.
Please note that we do not switch platform when you select a configuration, but only update the
'AppParameters' data and scripting define symbols associated with the configuration. So everything
in your code relying on AppParameters and define symbols will work, but using Unity own scripting
define symbols as "#if UNITY_ANDROID" will work only if you switch platform yourself. Using "Apply
this config", you'll be able to test everything in your code using the parameters in AppParameters
and custom define symbols so you can test how your game will react on different configurations
without the need to switch platform.


----------------------------------------- Perform Build -----------------------------------------
In this section, you can batch build all the configurations you previously selected, in a single
click. Make sure to check the 'Warnings & Errors' section to see if everything is ok...


--------------------------------------- Warnings & Errors ---------------------------------------
This section will tell you if something is wrong. So pay attention before building!



-------------------------------------------------------------------------------------------------
                                            Examples
-------------------------------------------------------------------------------------------------

------------------------------------------- Example 1 -------------------------------------------
Let's say you have a game that you want to release for Android, iOS and OSX Universal.
And you want to have:
- a dev version, so you can test your game without erasing the release version on a device
(because of the same package name)
- a beta version, that you will send to some people
- a release version, that will be released to the public

1. Release types
You will just add 3 release types (dev, beta and release) each having a different bundle
identifier so you can have the 3 different apps on Android & iOS.

2. Platforms
Then you will add Android, iOS and Mac platforms.

3. Architecture
For the Mac platform, you will only select OSX Universal.

You will now have a total of 9 builds ready to go:
- Dev: Android, iOS & OSX
- Beta: Android, iOS & OSX
- Release: Android, iOS & OSX

You just need to add all the configurations in the 'Project Configurations' section and when
your game is ready, just click on the "Perform a total of 9 builds" button.
That's it!

BONUS
At runtime, you can then use "if (AppParameters.Get.releaseType.Equals("Dev"))" to only
call some debug methods, or send some stats to your webserver.
You could (and should) also use precompiled symbols such as "#if RT_DEV".

Don't forget you can use the custom build script, to do really anything before and/or after each
build.


------------------------------------------- Example 2 -------------------------------------------
Now let's say you're making a game for iOS only and you want a Demo and Full version.

1. Release types
You will just add 2 release types (demo and release) each having a different bundle identifier

2. Platforms
Then you will add only the iOS platform.

You will now have a total of 2 builds ready to go:
- Demo: iOS
- Full: iOS

Add these configurations in the 'Project Configurations' section, and when your game is ready,
just click on the Perform a total of 2 builds button.
That's it!

BONUS
At runtime, you can then use "if (AppParameters.Get.releaseType.Equals("Full"))"
to allow users to access different parts of your game.
You could (and should) also use precompiled symbols such as "#if RT_DEMO".

If you're using a custom build script, you could also, before each build, change/remove some
GameObjects from a scene, or anything you want in the OnPreBuild method if the release type
is "Demo".


-------------------------------------------------------------------------------------------------
                                          Release Notes
-------------------------------------------------------------------------------------------------
1.4.3
- NEW: Now requires at least Unity 5.0.0
- NEW: Bundle version can now use the format "xx.xx.xx.xx", instead of just "xx.xx.xx"
- NEW: 'Append project' is now available again for iOS. You can chose to append projects in
each configuration.
- NEW: You now open Advanced Builder via the menu "Window/PygmyMonkey/Advanced Builder", it was
previously in "Tools/PygmyMonkey/Advanced Builder".
- NEW: Added support for Windows Store
- NEW: Added $BUNDLE_VERSION and $BUNDLE_IDENTIFIER to set your custom build destination path

1.4.2
- NEW: Added buildDate to OnPreBuild/OnPostBuild events
- UPDATE: Moved some options (Open build folder after build, Dev build, Autoconnect profiler,
Allow script debugging, Autorun) to configurations, so you can set them for each configuration.

1.4.1
- NEW: You can now reorganize the scenes you selected for each configuration.
- NEW: You can now decide if you want to let Advanced Builder decide which scenes are build in
the 'AdvancedSettings' section.
- NEW: You can now set a custom file name for your build (in addition to the build folder path).
- NEW: Added $TEST_COMPRESSION and $PRODUCT_NAME constants for custom build folder path and
custom file name.
- FIX: Android bundle version code if you were using FAT/x86/ARMv7.

1.4.0
- WARNING: IF YOU ARE UPGRADING FROM A VERSION OLDER THAN THIS ONE (1.4.0), PLEASE FOLLOW THE
STEPS DESCRIBED IN "How to update Advanced Builder".
- NEW: You now open Advanced Builder via the menu "Tools/PygmyMonkey/Advanced Builder", it was
previously in "Windows/Advanced Builder".
- NEW: Changed all the namespaces from "com.pygmymonkey.tools" to "PygmyMonkey.AdvancedBuilder".
- NEW: Moved the entire "AdvancedBuilder" folder inside a "PygmyMonkey" folder.

1.3.4
- NEW: Added platform architectures for Android. You can now automatically build for ARMv7,
x86 (Intel) or FAT (ARMv7 and x86)

1.3.3
- NEW: Added support for Unity5.
- NEW: Added WebGL support.

1.3.1
- NEW: You can now manage the scenes you want to build per configuration.

1.3.0
- NEW: Added configurations. You now have a list of configurations defined by all the release
types, platforms, platform architectures, distribution platforms and texture compression you
selected. With each configuration, you can see all the build info, directly build a configuration
and apply a configuration to the UnityEditor to test your configuration directly in the Editor
without the need to build or switch platform.
- NEW: Add support for custom compilation defines. Now you can define your custom defines for
each configuration. There is also some default defines created by AdvancedBuilder based on
the parameters of the configuration.

1.2.6
- NEW: Added a custom build example showing you how you can chose different scenes depending on
the release type you're building
- NEW: Added a custom build example that will automatically set your Android keystore password

1.2.5
- FIX: Updated documentation
- NEW: Improved the way 'Project Configuration' works, so you can easily try multiple configuration
directly in the Unity Editor without switching platforms.
This update will probably create some errors in your scripts if you were using the AppParameters
class and/or custom build scripts.
To access data via AppParameters, you will now need to call "AppParameters.Get.DATA" where DATA
is the data you want to retrieve (for example, to retrieve the release type, you would call:
AppParameters.Get.releaseType). Don't forget to import the namespace of AdvancedBuilder at the
top of your class doing "using PygmyMonkey.AdvancedBuilder;"
For custom build scripts, the OnPreBuild/OnPostBuild methods now take strings instead of enums
as parameters.

1.2.2
- FIX: Fix setting configurations using Unity Free

1.2.1
- NEW: In the Advanced Settings section, you can now chose to display an error when you forgot
to set your Android Keystore and Alias passwords.
- FIX: Display errors when incorrect name format are set for Distribution Platform and Release
Types

1.2.0
- NEW: Added support for Distribution Platforms! (see Get Started at the top for more info)

1.1.1 f1
- FIX: Building for Mac on a Windows machine was disabled by mistake (it's now possible again)

1.1.1
- NEW: Added a 'Project Configuration' window. You can now apply different configuration directly
in the Editor to test your game on each of these configurations. This is also useful for people
using Unity Free. (see Get Started & FAQ for more info).
- NEW: You can now activate/deactivate platforms you want to support (instead of deleting them)
- NEW: Added warning message if you try to build for iOS on Windows or Linux
- NEW: Added error message if using the same name in different release types
- FIX: Scenes not enabled in the build settings will not be build anymore
- FIX: Custom build methods now use the enums in AppParameters.cs instead of classes

1.1.0 f1
- FIX: Fixed BlackBerry build issue (Unity bug if you have a space in the .bar file name)
- FIX: Fixed InvalidOperationException thrown by Unity Editor GUI after a build operation
- FIX: Fixed iOS build if you had the Facebook Unity plugin in your project

1.1.0
- NEW: Added support for Windows Phone 8
- NEW: Added support for Windows Store 8
- NEW: Added support for BlackBery

1.0.1
- FIX: Display warning and errors even when platforms or release types are not fold out
- FIX: Build date on the build folder was incorrect if the total builds took more than 1 minute

1.0.0
- NEW: Initial release


-------------------------------------------------------------------------------------------------
                                          Future Updates
-------------------------------------------------------------------------------------------------

- Tutorial videos -
We will release 1-2 tutorial videos showing how to do specific things with explanations

- Per platform parameters -
Add the possibility to specify some parameters that will change depending on your release type
and/or platform. This can be use for example to define a different AdmobID or FacebookID (or
anything) if you're building the Free or Pro version of your game etc...

- Automated builds -
We will add an easy to use bash script that will automatically launch every x hours and perform
builds for you in the background. For example, you could have your computer starting the build
process every day at midnight, updating the repository from your master branch on
git/mercurial/svn before building. With that, you could have builds for your game ready every day
without having to do anything!

- Build report tool -
Have a report generated after each build where you can see a lot of usefull information on your
build. We will also probably add a custom window that will display all these information with
ease of reading.

- Build verbose -
Add the possibility to add verbose when builds are done, or even have your computer say something
when the build is done (Mac only) etc...

- Version Updater -
Add a simple class that will allows you to check if a user updated the game or installed it for
the first time. You could use that to make database migrations between versions for examples,
or give coins to users that had an issue with an old version of your game etc...

- Custom icons -
Allow you to add a small banner on your icon depending on the release type. This is usefull is
you want to build a free version of the game for example and you want to add the banner "Free"
on the icon. Instead of manually updating the icon before building each version of your game,
this will be done automatically!

- AndroidManifest and info.plist -
Add helper methods that will allow you to easily modify things in your AndroidManifest (Android)
and/or info.plist (iOS) before/after builds


-------------------------------------------------------------------------------------------------
                                               FAQ
-------------------------------------------------------------------------------------------------

- Can I use Advanced Builder with Unity batch mode? -
Unity batch mode allows you to launch Unity via a command line instruction and perform builds
without having Unity opened. It allows you to perform automated builds on a production computer
for example, building your app every x hours after performing a version control update etc...
This will be added really soon in a future update.

- I use a version control software, what can I add to the list of ignore files?
You can safely ignore the file AppParameters.asset (in PygmyMonkey/AdvancedBuilder/Resources/),
this file will just save all the information from your configuration before making each build.
So every time you make a build, the file will change, but other people on your team don't need
to have the same file as you have.
The file AdvancedBuilder.asset (in PygmyMonkey/AdvancedBuilder/Editor/) contains all the
different configurations you have created, and it's a good thing to share that with the people
you're working with. But it's not absolutely necessary. So if each person need to have their
own configurations, you can add it to the list of ignored files.

- Will you add support for more platforms? -
We currently support Android, iOS, WebPlayer, Windows, Mac, Linux, Windows Phone 8, Windows Store 8
, BlackBerry and WebGL. If you need a platform that is not listed here, please contact us!

- Can I use Advanced Builder with Prime31 plugins? -
Yes, there is absolutely no issue using prime31 plugins, because Advanced Builder generate builds
using Unity native build system.

- Why my BlackBerry final file name has its spaces removed? -
It's currently a Unity bug, if you have any space in the name of the .bar file, the build will
just fail. So Advanced Builder remove them automatically.

- How can I help?
Thank you! You can take a few seconds and rate the tool in the Asset Store and leave a nice
comment, that would help a lot ;)

- What's the minimum Unity version required? -
Advanced Builder will work starting with Unity 5.0.0.

- Can I build for iOS using a Windows/Linux computer? -
As Advanced Builder uses the Unity API for the build process, and Unity does not allow you to build
for iOS on a machine that is not a Mac, building for iOS absolutely require having a Mac.


-------------------------------------------------------------------------------------------------
                                           Other tools
-------------------------------------------------------------------------------------------------

--- Color Palette (http://u3d.as/cbR) ---
Color Palette will help you manage all your color palettes directly inside Unity!.
Instead of manually setting each color from the color picker, you can just pick the color you
want from the Color Palette Window. You can even apply an entire palette on all the objects in
your scene with just one click.

--- Gif Creator (http://u3d.as/icC) ---
Gif Creator allows you to record a gif from a camera, or the entire game view, directly inside Unity.