using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_IOS
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.Text.RegularExpressions;
#endif

public enum eBuildScheme { RELEASE, DEBUG };

public class ChessgeonBuildScript
{
#if UNITY_ANDROID
    private static BuildTargetGroup buildTargetGroup = BuildTargetGroup.Android;
    private static BuildTarget buildTarget = BuildTarget.Android;
#elif UNITY_IOS
    private static BuildTargetGroup buildTargetGroup = BuildTargetGroup.iOS;
    private static BuildTarget buildTarget = BuildTarget.iOS;
#endif

    public static void BuildChessgeon(eBuildScheme inBuildScheme)
    {
        try
        {
            #region Scripting defines
            string scriptingDefines = "CHESSGEON_" + inBuildScheme.ToString() + ";";
            switch (inBuildScheme)
            {
                case eBuildScheme.RELEASE:
                {
                    //scriptingDefines += "";
                    break;
                }
                case eBuildScheme.DEBUG:
                {
                    //scriptingDefines += "";
                    break;
                }
            }
            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, scriptingDefines);
            #endregion

            #region PlayerSettings
            PlayerSettings.companyName = "RainBlastGames";
            PlayerSettings.productName = "Chessgeon";
            PlayerSettings.SetApplicationIdentifier(buildTargetGroup, "com.tanxuewenrayner.Chessgeon");
            PlayerSettings.bundleVersion = VersionClass.BUNDLE_VERSION;
#if UNITY_ANDROID
            // TODO: Future KeyStore stuff.
            //PlayerSettings.keystorePass = "";
            //PlayerSettings.keyaliasPass = "";
            PlayerSettings.Android.androidTVCompatibility = false;

            // NOTE: VersionClass.Version should have format like this "x.x.x"
            string[] versionNumber = VersionClass.BUNDLE_VERSION.Split('.');
            int numSegments = versionNumber.Length;
            Debug.Assert(numSegments == 3);
            int versionCode = 0;
            List<int> intSegments = new List<int>();
            for (int i = 0; i < numSegments; i++)
            {
                intSegments.Add(System.Convert.ToInt16(versionNumber[i]));
            }
            versionCode = intSegments[0] * 1000000 + intSegments[1] * 1000 + intSegments[2];
            // Set the Android bundle version code. PlayerSettings.Android.bundleVersionCode looks like: "1008009"
            PlayerSettings.Android.bundleVersionCode = versionCode;

            if (PlayerSettings.Android.minSdkVersion != AndroidSdkVersions.AndroidApiLevel16)
            {
                throw new System.Exception("Are you sure you want to change minSdkVersion to " + PlayerSettings.Android.minSdkVersion.ToString() + "?");
            }

            PlayerSettings.Android.forceSDCardPermission = true;
#elif UNITY_IOS
            // Doesn't really work. Maybe check again after upgrading Unity.
            // switch (inBuildScheme)
            // {
            //     case eBuildScheme.RELEASE:
            //         {
            //             EditorUserBuildSettings.iOSBuildConfigType = iOSBuildType.Release;
            //         }
            //         break;
            //     case eBuildScheme.DEBUG:
            //         {
            //             EditorUserBuildSettings.iOSBuildConfigType = iOSBuildType.Debug;
            //         }
            //         break;
            // }
			PlayerSettings.iOS.appleEnableAutomaticSigning = true;
            PlayerSettings.iOS.buildNumber = VersionClass.REVISION;
            PlayerSettings.iOS.allowHTTPDownload = false;

            if (PlayerSettings.iOS.targetOSVersionString != "7.0")
            {
                throw new System.Exception("Are you sure you want to change targetOSVersion to " + PlayerSettings.iOS.targetOSVersionString + "?");
            }
#endif
            #endregion

            // NOTE: Set unity scenes to be included in the build.
            string[] scenes = new string[]
            {
                EditorMenuItems.SCENE_PATH_LOADING_SCREEN, // NOTE: Has to be first so when building this is the default scene loaded.
                EditorMenuItems.SCENE_PATH_DUNGEON
            };

            string outputFileName;
            string buildDirectory;
            BuildOptions buildOptions = BuildOptions.ShowBuiltPlayer;
#if UNITY_ANDROID
            outputFileName = "Chessgeon_" + inBuildScheme.ToString() + "_v" + VersionClass.BUNDLE_VERSION + "_r" + VersionClass.REVISION + ".apk";
            buildDirectory = "D:\\Work\\GIT_Repositories\\Project-Chessgeon\\Builds";
            buildOptions |= BuildOptions.AutoRunPlayer;
#elif UNITY_IOS
            outputFileName = "Chessgeon_xcode/";
            buildDirectory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal) + "/Documents/Unity Projects/BUILDS/Project-Chessgeon/";
#endif

            if (!Directory.Exists(buildDirectory)) Directory.CreateDirectory(buildDirectory);
            if (inBuildScheme != eBuildScheme.RELEASE) // NOTE: Respect the settings in the Build Settings, only if it's not a release build.
            {
                if (EditorUserBuildSettings.development)
                {
                    buildOptions |= BuildOptions.Development;

                    if (EditorUserBuildSettings.connectProfiler)
                    {
                        buildOptions |= BuildOptions.ConnectWithProfiler;
                    }
                    if (EditorUserBuildSettings.allowDebugging)
                    {
                        buildOptions |= BuildOptions.AllowDebugging;
                    }
                }
            }

            string buildPath = Path.Combine(buildDirectory, outputFileName);
            // The actual build happens here!
            string message = BuildPipeline.BuildPlayer(scenes, buildPath, buildTarget, buildOptions);

            if (string.IsNullOrEmpty(message))
            {
                Debug.Log("Chessgeon " + inBuildScheme.ToString() + " build completed successfully.");
            }
            else
            {
                EditorUtility.DisplayDialog("Oops!", "Error encountered while building: " + message, "ok");
            }
        }
        catch (Exception e)
        {
            EditorUtility.DisplayDialog("Ooops!", "Error encountered while building: " + e, "ok");
        }

        // Remove the conditional compilation flag from the editor.
        PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, string.Empty);
    }


    // NOTE(Rayner): We should let Unity handle execution of PostProcessBuild rather than calling it ourselves at the end of our build process.
    //				 So that our PostProcessBuild will run AFTER all the other external plugin's PostProcessBuild.
    #region iOSPostProcessBuild
#if UNITY_IOS && UNITY_EDITOR
	// Runs the last.
	[PostProcessBuild(int.MaxValue)]
	public static void ExecuteiOSPostProcessBuild(BuildTarget buildTarget, string pathToBuiltProject)
	{
		Debug.Log("Starting PostProcessBuild for iOS...");

		if (buildTarget == BuildTarget.iOS)
		{
			SetInfoPListInXcode(buildTarget, pathToBuiltProject);
			//DisableBitcodeInXcode(buildTarget, pathToBuiltProject);
			//SetBuildSettingsInXcode(buildTarget, pathToBuiltProject);

			RemovePushNotificationEntitlement(buildTarget, pathToBuiltProject);
		}
		else
		{
			Debug.LogError("Trying to execute iOS ONLY post process build");
		}

		EditorUtility.DisplayDialog("Post Process Completed", "Post Process Build is complete. You may now open the project.", "OK");
	}

	private static void SetInfoPListInXcode(BuildTarget buildTarget, string pathToBuiltProject)
	{
		if (buildTarget == BuildTarget.iOS)
		{
			// Get plist root
			string plistPath = pathToBuiltProject + "/Info.plist";
			PlistDocument plist = new PlistDocument();
			plist.ReadFromString(File.ReadAllText(plistPath));
			PlistElementDict rootDict = plist.root;

    #region APIs requiring usage description
			if (!rootDict.values.ContainsKey("NSCameraUsageDescription"))
			{
				rootDict.SetString("NSCameraUsageDescription", "Advertisement would like to use the camera.");
			}

			if (!rootDict.values.ContainsKey("NSCalendarsUsageDescription"))
			{
				rootDict.SetString("NSCalendarsUsageDescription", "Advertisement would like to create a calendar event.");
			}

			if (!rootDict.values.ContainsKey("NSPhotoLibraryUsageDescription"))
			{
				rootDict.SetString("NSPhotoLibraryUsageDescription", "Advertisement would like to store a photo.");
			}

			if (!rootDict.values.ContainsKey("NSBluetoothPeripheralUsageDescription"))
			{
				rootDict.SetString("NSBluetoothPeripheralUsageDescription", "Advertisement would like to use bluetooth.");
			}
    #endregion

    #region Adding UIRequiredDeviceCapabilities
			string[] listOfRequiredDeviceCapabilities = { "gamekit", "wifi" };
			if (rootDict.values.ContainsKey("UIRequiredDeviceCapabilities"))
			{
				// Grab the "UIRequiredDeviceCapabilities" array and add listOfRequiredDeviceCapabilities only if not added.
				PlistElementArray requiredDeviceCapabilitiesArray = rootDict.values["UIRequiredDeviceCapabilities"] as PlistElementArray;
				for (int i = 0; i < listOfRequiredDeviceCapabilities.Length; i++)
				{
					// Have to use this roundabout check as requiredDeviceCapabilitiesArray.values.Contains() checks for
					// instances, but not the exact value of the string. So there can be two PListElement with same strings
					// but they would be two different objects.
					bool containsCapability = false;
					for (int jExistingCapability = 0; jExistingCapability < requiredDeviceCapabilitiesArray.values.Count; jExistingCapability++)
					{
						if (requiredDeviceCapabilitiesArray.values[jExistingCapability].AsString() == listOfRequiredDeviceCapabilities[i])
						{
							containsCapability = true;
							break;
						}
					}

					if (!containsCapability)
					{
						requiredDeviceCapabilitiesArray.AddString(listOfRequiredDeviceCapabilities[i]);
					}
				}
			}
			else // If key doesn't exist, create the array and add ALL listOfRequiredDeviceCapabilities.
			{
				PlistElementArray requiredDeviceCapabilitiesArray = rootDict.CreateArray("UIRequiredDeviceCapabilities");
				for (int i = 0; i < listOfRequiredDeviceCapabilities.Length; i++)
				{
					requiredDeviceCapabilitiesArray.AddString(listOfRequiredDeviceCapabilities[i]);
				}
			}
    #endregion

    #region Enforcing ATS Compliance
			const string ATS_KEY = "NSAppTransportSecurity";
			PlistElementDict appTransportSecurityDict = null;
			if (rootDict.values.ContainsKey(ATS_KEY))
			{
				// Grab the "NSAppTransportSecurity" array and ensure that "NSAppTransportSecurity" is set to false.
				appTransportSecurityDict = rootDict.values[ATS_KEY] as PlistElementDict;
			}
			else // If key doesn't exist, create the dictionary.
			{
				appTransportSecurityDict = rootDict.CreateDict(ATS_KEY);
			}
			appTransportSecurityDict.SetBoolean("NSAllowsArbitraryLoads", false);
    #endregion

			// Write to file
			File.WriteAllText(plistPath, plist.WriteToString());
		}
	}

	//private static void DisableBitcodeInXcode(BuildTarget buildTarget, string pathToBuiltProject)
	//{
	//	if (buildTarget == BuildTarget.iOS)
	//	{
	//		string projectPath = pathToBuiltProject + "/Unity-iPhone.xcodeproj/project.pbxproj";

	//		PBXProject pbxProject = new PBXProject();
	//		pbxProject.ReadFromFile(projectPath);

	//		string target = pbxProject.TargetGuidByName("Unity-iPhone");
	//		pbxProject.SetBuildProperty(target, "ENABLE_BITCODE", "NO");

	//		pbxProject.WriteToFile(projectPath);
	//	}
	//}

	//private static void SetBuildSettingsInXcode(BuildTarget buildTarget, string pathToBuiltProject)
	//{
	//	if (buildTarget == BuildTarget.iOS)
	//	{
	//		string projectPath = pathToBuiltProject + "/Unity-iPhone.xcodeproj/project.pbxproj";

	//		PBXProject pbxProject = new PBXProject();
	//		pbxProject.ReadFromFile(projectPath);

	//		string target = pbxProject.TargetGuidByName("Unity-iPhone");

	//		// note(Rayner):	We're adding $(inherited) to these two paths to remove warnings from pod install.
	//		//					Adding these $(inherited) simply tells xcode to merge other config settings with the project itself.
	//		//					Also, AddBuildProperty doesn't add the property if it is already there.
	//		//					So there won't be multiple $(inherited) values.
	//		pbxProject.AddBuildProperty(target, "HEADER_SEARCH_PATHS", "$(inherited)");
	//		pbxProject.AddBuildProperty(target, "OTHER_CFLAGS", "$(inherited)");

	//		pbxProject.WriteToFile(projectPath);
	//	}
	//}

	// Note(Rayner): This function is used to remove the following warning from uploading builds to iTunes connect.
	/* Missing Push Notification Entitlement - Your app includes an API for Apple's Push Notification service, but 
	 * the aps-environment entitlement is missing from the app's signature. To resolve this, make sure your App ID 
	 * is enabled for push notification in the Provisioning Portal. Then, sign your app with a distribution 
	 * provisioning profile that includes the aps-environment entitlement. This will create the correct signature, 
	 * and you can resubmit your app. See "Provisioning and Development" in the Local and Push Notification 
	 * Programming Guide for more information. If your app does not use the Apple Push Notification service, 
	 * no action is required. You may remove the API from future submissions to stop this warning. If you use a 
	 * third-party framework, you may need to contact the developer for information on removing the API.
	 */
	private static void RemovePushNotificationEntitlement(BuildTarget target, string pathToBuiltProject)
	{
		if (target == BuildTarget.iOS)
		{
			string appControllerFile = "UnityAppController.mm";

			// Remove instances of RemoteNotification
			// this matches (void)application:did..something..Notification..something... methods declaration
			string regexpForNotificationMethods = "-\\s?\\(void\\)application:\\(UIApplication\\s?\\*\\)application\\sdid.+RemoteNotification.+\\n?{[^-|#.+]+";
			// check if app controller file exists
			string classesDirectory = Path.Combine(pathToBuiltProject, "Classes");
			string pathToAppController = Path.Combine(classesDirectory, appControllerFile);

			bool fileExists = File.Exists(pathToAppController);
			if (!fileExists)
			{
				Debug.LogError("App Controller file doesn't exist.");
				return;
			}
			string code = File.ReadAllText(pathToAppController);
			string codeWithDeletedNotificationsMethod = Regex.Replace(code, regexpForNotificationMethods, "");

			// Note(Rayner):	However, by removing above code, the notification settings is never registered.
			//					This means that even local notifications won't work. You'll notice in settings, the game does not have notif settings.
			//					So we need to add the following bit of code into UnityAppController.mm under didFinishLaunchingWithOptions.
			//					This will ensure that local notificatins will still work and restore said settings.
			// Need to check if we've already run this part. Ensures that we don't duplicate code.
			string regexpForRegisterNotificationSettingsCode = 
				"UIUserNotificationType\\snotificationTypes\\s=\\sUIUserNotificationTypeBadge\\s\\|\\sUIUserNotificationTypeSound\\s\\|\\sUIUserNotificationTypeAlert;\\n\\t" +
				"UIUserNotificationSettings\\s\\*notificationSettings\\s=\\s\\[UIUserNotificationSettings\\ssettingsForTypes:notificationTypes\\scategories:nil\\];\\n\\t" +
				"\\[\\[UIApplication\\ssharedApplication\\]\\sregisterUserNotificationSettings:notificationSettings\\];\\n";
			if (Regex.IsMatch(codeWithDeletedNotificationsMethod, regexpForRegisterNotificationSettingsCode))
			{
				File.WriteAllText(pathToAppController, codeWithDeletedNotificationsMethod);
			}
			else
			{
				string regexpForDidFinishLaunching = "-\\s?\\(BOOL\\)application:\\(UIApplication\\s?\\*\\)application\\sdidFinishLaunchingWithOptions:\\(NSDictionary\\*\\)launchOptions\\n?{";
                // No need for local notications in BuildScript.
				// string codeToRegisterNotificationSettings =
				// 	"UIUserNotificationType notificationTypes = UIUserNotificationTypeBadge | UIUserNotificationTypeSound | UIUserNotificationTypeAlert;\n\t" +
				// 	"UIUserNotificationSettings *notificationSettings = [UIUserNotificationSettings settingsForTypes:notificationTypes categories:nil];\n\t" +
				// 	"[[UIApplication sharedApplication] registerUserNotificationSettings:notificationSettings];\n";
                string codeToRegisterNotificationSettings = string.Empty;
				MatchEvaluator evaluatorForDidFinishLaunching = new MatchEvaluator((m) =>
				{
					return string.Format("{0}\n\t{1}", m.Value, codeToRegisterNotificationSettings);
				});
				string codeWithNotificationSettings = Regex.Replace(codeWithDeletedNotificationsMethod, regexpForDidFinishLaunching, evaluatorForDidFinishLaunching);

				File.WriteAllText(pathToAppController, codeWithNotificationSettings);
			}
		}
	}
#endif
    #endregion
}
