#if UNITY_EDITOR && UNITY_5
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Collections;
using System.Xml.XPath;

public class AkPluginActivator
{
	public const string ALL_PLATFORMS = "All";
	public const string CONFIG_DEBUG = "Debug";
	public const string CONFIG_PROFILE = "Profile";
	public const string CONFIG_RELEASE = "Release";
	
	// Use reflection because projects that were created in Unity 4 won't have the CurrentPluginConfig field
	static string GetCurrentConfig()
	{		
		FieldInfo CurrentPluginConfigField = typeof(AkWwiseProjectData).GetField("CurrentPluginConfig");
		string CurrentConfig = string.Empty;
		if( CurrentPluginConfigField != null )
		{
			CurrentConfig = (string)CurrentPluginConfigField.GetValue(AkWwiseProjectInfo.GetData ());
            if (string.IsNullOrEmpty(CurrentConfig))
                CurrentConfig = "Profile";
		}
		
		return CurrentConfig;
	}


	static void SetCurrentConfig(string config)
	{		
		FieldInfo CurrentPluginConfigField = typeof(AkWwiseProjectData).GetField("CurrentPluginConfig");
		if( CurrentPluginConfigField != null )
		{
			CurrentPluginConfigField.SetValue(AkWwiseProjectInfo.GetData (), config);
		}
        EditorUtility.SetDirty(AkWwiseProjectInfo.GetData());
	}
	
	[MenuItem("Assets/Wwise/Activate Plugins/Debug")]
	public static void ActivateDebug()
	{
		if( GetCurrentConfig() != CONFIG_DEBUG )
		{
            ActivatePlugins( GetCurrentConfig(), false);
			SetCurrentConfig(CONFIG_DEBUG);
            ActivatePlugins( CONFIG_DEBUG, true);
		}
		else
		{
			Debug.Log ("WwiseUnity: AkSoundEngine Plugins already activated for Debug.");
		}
	}
	
	[MenuItem("Assets/Wwise/Activate Plugins/Profile")]
	public static void ActivateProfile()
	{
		if( GetCurrentConfig() != CONFIG_PROFILE )
		{
			ActivatePlugins ( GetCurrentConfig(), false);
			SetCurrentConfig(CONFIG_PROFILE);
            ActivatePlugins( CONFIG_PROFILE, true);
		}
		else
		{
			Debug.Log ("WwiseUnity: AkSoundEngine Plugins already activated for Profile.");		
		}
	}
	
	[MenuItem("Assets/Wwise/Activate Plugins/Release")]
	public static void ActivateRelease()
	{
		if( GetCurrentConfig() != CONFIG_RELEASE )
		{
            ActivatePlugins( GetCurrentConfig(), false);
			SetCurrentConfig(CONFIG_RELEASE);
			ActivatePlugins( CONFIG_RELEASE, true);
		}
		else
		{
			Debug.Log ("WwiseUnity: AkSoundEngine Plugins already activated for Release.");		
		}
	}
	
	public static void RefreshPlugins()
	{        
		ActivatePlugins ( GetCurrentConfig(), true);
	}
	
	private static void SetStandaloneTarget(PluginImporter pluginImporter, BuildTarget target)
	{
		switch(target)
		{
		case BuildTarget.StandaloneLinux:
			pluginImporter.SetPlatformData (BuildTarget.StandaloneLinux, "CPU", "x86");
			pluginImporter.SetPlatformData (BuildTarget.StandaloneLinux64, "CPU", "None");
			pluginImporter.SetPlatformData (BuildTarget.StandaloneLinuxUniversal, "CPU", "x86");
			pluginImporter.SetPlatformData (BuildTarget.StandaloneOSXIntel, "CPU", "None");
			pluginImporter.SetPlatformData (BuildTarget.StandaloneOSXIntel64, "CPU", "None");
			pluginImporter.SetPlatformData (BuildTarget.StandaloneOSXUniversal, "CPU", "None");
			pluginImporter.SetPlatformData (BuildTarget.StandaloneWindows, "CPU", "None");
			pluginImporter.SetPlatformData (BuildTarget.StandaloneWindows64, "CPU", "None");
			return;
		case BuildTarget.StandaloneLinux64:
			pluginImporter.SetPlatformData (BuildTarget.StandaloneLinux, "CPU", "None");
			pluginImporter.SetPlatformData (BuildTarget.StandaloneLinux64, "CPU", "x86_64");
			pluginImporter.SetPlatformData (BuildTarget.StandaloneLinuxUniversal, "CPU", "x86_64");
			pluginImporter.SetPlatformData (BuildTarget.StandaloneOSXIntel, "CPU", "None");
			pluginImporter.SetPlatformData (BuildTarget.StandaloneOSXIntel64, "CPU", "None");
			pluginImporter.SetPlatformData (BuildTarget.StandaloneOSXUniversal, "CPU", "None");
			pluginImporter.SetPlatformData (BuildTarget.StandaloneWindows, "CPU", "None");
			pluginImporter.SetPlatformData (BuildTarget.StandaloneWindows64, "CPU", "None");
			return;
		case BuildTarget.StandaloneOSXIntel:
		case BuildTarget.StandaloneOSXIntel64:
			pluginImporter.SetPlatformData (BuildTarget.StandaloneLinux, "CPU", "None");
			pluginImporter.SetPlatformData (BuildTarget.StandaloneLinux64, "CPU", "None");
			pluginImporter.SetPlatformData (BuildTarget.StandaloneLinuxUniversal, "CPU", "None");
			pluginImporter.SetPlatformData (BuildTarget.StandaloneOSXIntel, "CPU", "AnyCPU");
			pluginImporter.SetPlatformData (BuildTarget.StandaloneOSXIntel64, "CPU", "AnyCPU");
			pluginImporter.SetPlatformData (BuildTarget.StandaloneOSXUniversal, "CPU", "AnyCPU");
			pluginImporter.SetPlatformData (BuildTarget.StandaloneWindows, "CPU", "None");
			pluginImporter.SetPlatformData (BuildTarget.StandaloneWindows64, "CPU", "None");
			return;
		case BuildTarget.StandaloneWindows:
			pluginImporter.SetPlatformData (BuildTarget.StandaloneLinux, "CPU", "None");
			pluginImporter.SetPlatformData (BuildTarget.StandaloneLinux64, "CPU", "None");
			pluginImporter.SetPlatformData (BuildTarget.StandaloneLinuxUniversal, "CPU", "None");
			pluginImporter.SetPlatformData (BuildTarget.StandaloneOSXIntel, "CPU", "None");
			pluginImporter.SetPlatformData (BuildTarget.StandaloneOSXIntel64, "CPU", "None");
			pluginImporter.SetPlatformData (BuildTarget.StandaloneOSXUniversal, "CPU", "None");
			pluginImporter.SetPlatformData (BuildTarget.StandaloneWindows, "CPU", "AnyCPU");
			pluginImporter.SetPlatformData (BuildTarget.StandaloneWindows64, "CPU", "None");
			return;
		case BuildTarget.StandaloneWindows64:
			pluginImporter.SetPlatformData (BuildTarget.StandaloneLinux, "CPU", "None");
			pluginImporter.SetPlatformData (BuildTarget.StandaloneLinux64, "CPU", "None");
			pluginImporter.SetPlatformData (BuildTarget.StandaloneLinuxUniversal, "CPU", "None");
			pluginImporter.SetPlatformData (BuildTarget.StandaloneOSXIntel, "CPU", "None");
			pluginImporter.SetPlatformData (BuildTarget.StandaloneOSXIntel64, "CPU", "None");
			pluginImporter.SetPlatformData (BuildTarget.StandaloneOSXUniversal, "CPU", "None");
			pluginImporter.SetPlatformData (BuildTarget.StandaloneWindows, "CPU", "None");
			pluginImporter.SetPlatformData (BuildTarget.StandaloneWindows64, "CPU", "AnyCPU");
			return;
		default:
			return;
		}
	}
	
	// Properly set the platforms for Ak plugins. If platformToActivate is set to all platforms
	// will be activated.
	public static void ActivatePlugins(string configToActivate, bool Activate)
	{
		PluginImporter[] importers = PluginImporter.GetAllImporters();
		bool ChangedSomeAssets = false;        

        string iOSPluginsCPP =
        @"namespace AK {class PluginRegistration; };
    #define AK_STATIC_LINK_PLUGIN(_pluginName_) \
	extern AK::PluginRegistration _pluginName_##Registration; \
	void *_pluginName_##_fp = (void*)&_pluginName_##Registration;" + "\n";
		bool biOSPlatformActive = false;

		foreach(PluginImporter pluginImporter in importers)
		{
            if (pluginImporter.assetPath.StartsWith("Assets/Plugins", StringComparison.CurrentCultureIgnoreCase) && pluginImporter.assetPath.Contains("AkSoundEngine"))
			{
				AssetDatabase.DeleteAsset(pluginImporter.assetPath);
				continue;
			}

            if (!pluginImporter.assetPath.Contains("Wwise/Deployment/Plugins"))
				continue;
			
			string[] splitPath = pluginImporter.assetPath.Split('/');
			
			// Path is Assets/Wwise/Deployment/Plugins/Platform. We need the platform string
			string pluginPlatform = splitPath[4];            
			
			// Architecture and configuration (Debug, Profile, Release) are platform-dependent
			string pluginArch = string.Empty;
			string pluginConfig = string.Empty;
			string editorCPU = string.Empty;
			string editorOS = string.Empty;
			List<BuildTarget> targetsToSet = new List<BuildTarget>();
			bool setEditor = false;
            string pluginDSPPlatform = pluginPlatform;
			switch (pluginPlatform)			
			{
			case "Android":
				pluginConfig = splitPath[6];
				pluginArch = splitPath[5];
				targetsToSet.Add (BuildTarget.Android);
				if (pluginArch == "armeabi-v7a")			
					pluginImporter.SetPlatformData(BuildTarget.Android, "CPU", "ARMv7");
				else if (pluginArch == "x86")
					pluginImporter.SetPlatformData(BuildTarget.Android, "CPU", "x86");
				else
				{
					Debug.Log("WwiseUnity: Architecture not found: " + pluginArch);
				}
				break;
			case "iOS":                
			    pluginConfig = splitPath[5];
			    targetsToSet.Add (BuildTarget.iOS);                
				break;
#if UNITY_5 && !(UNITY_5_0 || UNITY_5_1 || UNITY_5_2)
			case "tvOS":
				pluginConfig = splitPath[5];
				targetsToSet.Add (BuildTarget.tvOS);
				break;
#endif
			case "Linux":
				pluginArch = splitPath[5];
				pluginConfig = splitPath[6];
				if( pluginArch == "x86" )
				{
					targetsToSet.Add (BuildTarget.StandaloneLinux);
					SetStandaloneTarget(pluginImporter, BuildTarget.StandaloneLinux);
				}
				else if( pluginArch == "x86_64" )
				{
					targetsToSet.Add (BuildTarget.StandaloneLinux64);
					SetStandaloneTarget(pluginImporter, BuildTarget.StandaloneLinux64);
				}
				else
				{
					Debug.Log("WwiseUnity: Architecture not found: " + pluginArch);
				}
				targetsToSet.Add (BuildTarget.StandaloneLinuxUniversal);
				break;
			case "Mac":
				pluginConfig = splitPath[5];
				SetStandaloneTarget(pluginImporter, BuildTarget.StandaloneOSXIntel);
				SetStandaloneTarget(pluginImporter, BuildTarget.StandaloneOSXIntel64);
				targetsToSet.Add (BuildTarget.StandaloneOSXIntel);
				targetsToSet.Add (BuildTarget.StandaloneOSXIntel64);
				targetsToSet.Add(BuildTarget.StandaloneOSXUniversal);
				editorCPU = "AnyCPU";
				editorOS = "OSX";
				setEditor = true;
				break;
			case "WSA":
                pluginArch = splitPath[5];
				pluginConfig = splitPath[6];
				targetsToSet.Add (BuildTarget.WSAPlayer);

                // For WSA, we use the plugin info for Windows, since they share banks.
                pluginDSPPlatform = "Windows";

                if (pluginArch == "WSA_ARM")
                {
                    pluginImporter.SetPlatformData(BuildTarget.WSAPlayer, "SDK", "SDK81");
                    pluginImporter.SetPlatformData(BuildTarget.WSAPlayer, "CPU", "ARM");
                }
                else if (pluginArch == "WSA_Win32")
                {
                    pluginImporter.SetPlatformData(BuildTarget.WSAPlayer, "SDK", "SDK81");
                    pluginImporter.SetPlatformData(BuildTarget.WSAPlayer, "CPU", "X86");
                }
                else if (pluginArch == "WSA_WindowsPhone81_ARM")
                {
                    pluginImporter.SetPlatformData(BuildTarget.WSAPlayer, "SDK", "PhoneSDK81");
                    pluginImporter.SetPlatformData(BuildTarget.WSAPlayer, "CPU", "ARM");
                }
                else if (pluginArch == "WSA_WindowsPhone81_Win32")
                {
                    pluginImporter.SetPlatformData(BuildTarget.WSAPlayer, "SDK", "PhoneSDK81");
                    pluginImporter.SetPlatformData(BuildTarget.WSAPlayer, "CPU", "X86");
                }
                else if (pluginArch == "WSA_UWP_Win32")
                {
                    pluginImporter.SetPlatformData(BuildTarget.WSAPlayer, "SDK", "UWP");
                    pluginImporter.SetPlatformData(BuildTarget.WSAPlayer, "CPU", "X86");
                }
                else if (pluginArch == "WSA_UWP_x64")
                {
                    pluginImporter.SetPlatformData(BuildTarget.WSAPlayer, "SDK", "UWP");
                    pluginImporter.SetPlatformData(BuildTarget.WSAPlayer, "CPU", "X64");
                }
                else if (pluginArch == "WSA_UWP_ARM")
                {
                    pluginImporter.SetPlatformData(BuildTarget.WSAPlayer, "SDK", "UWP");
                    pluginImporter.SetPlatformData(BuildTarget.WSAPlayer, "CPU", "ARM");
                }
				break;
			case "PS3":
				pluginConfig = splitPath[5];
				targetsToSet.Add(BuildTarget.PS3);

                // Workaround: the PS4 module tags all .prx files as a PS4 plugin. We need to deactivate this.
                pluginImporter.SetCompatibleWithPlatform(BuildTarget.PS4, false);
                break;
			case "PS4":
				pluginConfig = splitPath[5];
				targetsToSet.Add(BuildTarget.PS4);
				break;
			case "Vita":
                pluginArch = splitPath[5];
				pluginConfig = splitPath[6];
				targetsToSet.Add(BuildTarget.PSP2);
				break;
			case "Windows":
				pluginArch = splitPath[5];
				pluginConfig = splitPath[6];
				if( pluginArch == "x86" )
				{
					targetsToSet.Add (BuildTarget.StandaloneWindows);
					SetStandaloneTarget(pluginImporter, BuildTarget.StandaloneWindows);
					editorCPU = "X86";
				}
				else if( pluginArch == "x86_64" )
				{
					targetsToSet.Add (BuildTarget.StandaloneWindows64);
					SetStandaloneTarget(pluginImporter, BuildTarget.StandaloneWindows64);
					editorCPU = "X86_64";
				}
				else
				{
					Debug.Log("WwiseUnity: Architecture not found: " + pluginArch);
				}
				setEditor = true;
				editorOS = "Windows";
				break;
			case "XBox360":
				pluginConfig = splitPath[5];
				targetsToSet.Add(BuildTarget.XBOX360);
				break;
			case "XboxOne":
				pluginConfig = splitPath[5];
				targetsToSet.Add(BuildTarget.XboxOne);
				break;
#if UNITY_5 && !(UNITY_5_0 || UNITY_5_1)
			case "WiiU": // todo: wiiu not in buildtarget...
				pluginConfig = splitPath[5];
				targetsToSet.Add(BuildTarget.WiiU);
				break;
#endif
			default:
				if(!pluginPlatform.Contains("_new"))
				{
					Debug.Log ("WwiseUnity: Unknown platform: " + pluginPlatform);
				}
				continue;
			}
			
			bool AssetChanged = false;
			
			if( pluginImporter.GetCompatibleWithAnyPlatform() )
			{
				pluginImporter.SetCompatibleWithAnyPlatform(false);
				AssetChanged = true;
			}

            bool bActivate = true;
            if (pluginConfig == "DSP")
            {
                if (!s_PerPlatformPlugins.ContainsKey(pluginDSPPlatform))
                    continue;   //XML not parsed, don't touch anything.

                bActivate = IsPluginUsed(pluginDSPPlatform, Path.GetFileNameWithoutExtension(pluginImporter.assetPath));
            }
            else
            {
                bActivate = pluginConfig == configToActivate;
            }

            if (pluginPlatform == "Vita")
            {
#if AK_ARCH_VITA_HW
                bActivate = (pluginArch == "HW") ? bActivate : false;
#else
                bActivate = (pluginArch == "SW") ? bActivate : false;
#endif
            }

            if (!bActivate)
            {
                // This is not the configuration we want to activate, make sure it is deactivated
                foreach (BuildTarget target in targetsToSet)
                {
                    AssetChanged |= pluginImporter.GetCompatibleWithPlatform(target);
                    pluginImporter.SetCompatibleWithPlatform(target, false);
                }
                if (setEditor)
                {
                    AssetChanged |= pluginImporter.GetCompatibleWithEditor();
                    pluginImporter.SetCompatibleWithEditor(false);
                }
            }
		    else
		    {
                // Set this plugin
                foreach (BuildTarget target in targetsToSet)
                {
                    AssetChanged |= (pluginImporter.GetCompatibleWithPlatform(target) != Activate);
                    pluginImporter.SetCompatibleWithPlatform(target, Activate);
                    if (target == BuildTarget.iOS)
					{
						biOSPlatformActive = true;
                        if (pluginImporter.assetPath.Contains("AkSoundEngine.a"))
                            continue;

						if (pluginImporter.assetPath.Contains (".a")) {
							//Extract the lib name, generate the registration code.
							int begin = pluginImporter.assetPath.LastIndexOf ('/') + 4;
							int end = pluginImporter.assetPath.LastIndexOf ('.') - begin;
							string libName = pluginImporter.assetPath.Substring (begin, end);    //Remove the lib prefix and the .a extension                    
							iOSPluginsCPP += "#include \"" + libName + "Factory.h\"\n";
						}
                    }
                }

                if (setEditor)
                {
                    AssetChanged |= (pluginImporter.GetCompatibleWithEditor() != Activate);
                    pluginImporter.SetCompatibleWithEditor(Activate);
                    pluginImporter.SetEditorData("CPU", editorCPU);
                    pluginImporter.SetEditorData("OS", editorOS);
                }
            }

			if( AssetChanged )
			{                
				ChangedSomeAssets = true;
				AssetDatabase.ImportAsset(pluginImporter.assetPath);
			}
		}

		if (ChangedSomeAssets && biOSPlatformActive)
		{
			try
			{
				string cppPath = Path.Combine(Application.dataPath, "Wwise/Deployment/Plugins/iOS/DSP/AkiOSPlugins.cpp");
				File.WriteAllText(cppPath, iOSPluginsCPP);
			}
			catch(Exception e)
			{
				Debug.LogError("Wwise: Could not write AkiOSPlugins.cpp. Exception: " + e.Message);       
			}
		}                    

		
		if( Activate && ChangedSomeAssets )
		{
			Debug.Log ("WwiseUnity: Plugins successfully activated for " + configToActivate + ".");
		}
	}
	
	public static void DeactivateAllPlugins()
	{
		PluginImporter[] importers = PluginImporter.GetAllImporters();
		
		foreach(PluginImporter pluginImporter in importers)
		{
			pluginImporter.SetCompatibleWithAnyPlatform(false);
			AssetDatabase.ImportAsset(pluginImporter.assetPath);
		}
	}

    static Dictionary<string, DateTime> s_LastParsed = new Dictionary<string, DateTime>();
    static Dictionary<string, HashSet<string>> s_PerPlatformPlugins = new Dictionary<string, HashSet<string>>();

    static public bool IsPluginUsed(string in_UnityPlatform, string in_PluginName)
    {
        if (in_PluginName.Contains("AkSoundEngine"))
            return true;

        string pluginName = in_PluginName;
        if (in_PluginName.StartsWith("lib"))
        {
            //That's a unix-y type of plugin, just remove the prefix to find our name.
            pluginName = in_PluginName.Substring(3);
        }
    
        HashSet<string> plugins;

        if (s_PerPlatformPlugins.TryGetValue(in_UnityPlatform, out plugins))
        {
            if (in_UnityPlatform != "iOS")
                return plugins.Contains(pluginName);
            
            //iOS deals with the static libs directly, unlike all other platforms.
            //Luckily the DLL name is always a subset of the lib name.
            foreach(string pl in plugins)
            {
				if (!string.IsNullOrEmpty(pl) && pluginName.Contains(pl))
                    return true;
            }

            //Exceptions
			if (pluginName.Contains("AkiOSPlugins") || plugins.Contains("AkSoundSeedAir") && (pluginName.Contains("SoundSeedWind") || pluginName.Contains("SoundSeedWoosh")))
                return true;
        }

        return false;
    }


    public static void Update()
    {
        //Gather all GeneratedSoundBanks folder from the project
        IDictionary<string, string> allPaths = AkUtilities.GetAllBankPaths();
        bool bNeedRefresh = false;
        string projectPath = Path.GetDirectoryName(AkUtilities.GetFullPath(Application.dataPath, WwiseSettings.LoadSettings().WwiseProjectPath));

        IDictionary<string, HashSet<string>> pfMap = AkUtilities.GetPlatformMapping();
        //Go through all BasePlatforms 
        foreach (KeyValuePair<string, HashSet<string>> pairPF in pfMap)
        {
            //Go through all custom platforms related to that base platform and check if any of the bank files were updated.
            bool bParse = false;      
            List<string> fullPaths = new List<string>();
            foreach (string customPF in pairPF.Value)
            {
                string bankPath;
                if (!allPaths.TryGetValue(customPF, out bankPath)) 
                   continue;

                string pluginFile = "";                          
                try
                {
                    pluginFile = Path.Combine(projectPath, Path.Combine(bankPath, "PluginInfo.xml"));
                    pluginFile = pluginFile.Replace('/', Path.DirectorySeparatorChar);
                    if (!File.Exists(pluginFile))
                    {
                        //Try in StreamingAssets too.
                        pluginFile = Path.Combine(Path.Combine(AkBasePathGetter.GetFullSoundBankPath(), customPF), "PluginInfo.xml");
                        if (!File.Exists(pluginFile))
                            continue;
                    }
                    fullPaths.Add(pluginFile);

                    DateTime t = File.GetLastWriteTime(pluginFile);
                    DateTime lastTime = DateTime.MinValue;
                    s_LastParsed.TryGetValue(customPF, out lastTime);
                    if (lastTime < t)
                    {     
                        bParse = true;
                        s_LastParsed[customPF] = t;
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError("Wwise: " + pluginFile + " could not be parsed. " + ex.Message);
                }                              
            }
            
            if (bParse)
            {                
                HashSet<string> newDlls = ParsePluginsXML(fullPaths);
                HashSet<string> oldDlls = null;

                string platform = pairPF.Key;

                //Remap base Wwise platforms to Unity platform folders names
                if (platform == "WiiUSW")
                    platform = "WiiU";
                else if (platform.Contains("Vita"))
                    platform = "Vita";
                //else other platform already have the right name

                s_PerPlatformPlugins.TryGetValue(platform, out oldDlls);
                s_PerPlatformPlugins[platform] = newDlls;

                //Check if there was any change.
                if (!bNeedRefresh && oldDlls != null)
                {	
					if (oldDlls.Count == newDlls.Count)
                    	oldDlls.IntersectWith((IEnumerable<string>)newDlls);

                    bNeedRefresh |= oldDlls.Count != newDlls.Count;
                }
                else
                    bNeedRefresh |= newDlls.Count > 0;
            }
        }

        if (bNeedRefresh)
            RefreshPlugins();
    }

    static uint[] s_BuiltInPluginIDs = 
    {
        0x00640002, //Sine
        0x00650002, //Wwise Silence
        0x00660002, //Tone Generator
        0x00690003, //Wwise Parametric EQ
        0x006A0003, //Delay
        0x006C0003, //Wwise Compressor
        0x006D0003, //Wwise Expander
        0x006E0003, //Wwise Peak Limiter
        0x00730003, //Matrix Reverb
        0x00760003, //Wwise RoomVerb
        0x00810003, //Wwise Meter
        0x008B0003, //Gain
        0x008C0003, //Vita Reverb	
        0x008D0003, //Vita Compressor 
        0x008E0003, //Vita Delay
        0x008F0003, //Vita Distortion
        0x00900003 //Vita EQ
    };

    private static HashSet<string> ParsePluginsXML(List<string> in_pluginFiles)
    {
        HashSet<string> newDlls = new HashSet<string>();
        foreach (string pluginFile in in_pluginFiles)
        {
            if (!File.Exists(pluginFile))
            {
                continue;
            }

            try
            {                
                XmlDocument doc = new XmlDocument();
                doc.Load(pluginFile);
                XPathNavigator Navigator = doc.CreateNavigator();

                XPathNavigator pluginInfoNode = Navigator.SelectSingleNode("//PluginInfo");
                string boolMotion = pluginInfoNode.GetAttribute("Motion", "");

                XPathNodeIterator it = Navigator.Select("//Plugin");                

                if (boolMotion == "true")
                    newDlls.Add("AkMotion");

                foreach (XPathNavigator node in it)
                {
                    //Some plugins are built-in the integration.  Ignore those.
                    uint pid = UInt32.Parse(node.GetAttribute("ID", ""));
                    foreach (uint builtID in s_BuiltInPluginIDs)
                    {
                        if (builtID == pid)
                        {
                            pid = 0;
                            break;  //Built in, don't try to mark the DLL.
                        }
                    }

                    if (pid == 0)
                        continue;

                    string dll = node.GetAttribute("DLL", "");
                    newDlls.Add(dll);
                }
            }           
            catch (Exception ex)
            {
                Debug.LogError("Wwise: " + pluginFile + " could not be parsed. " + ex.Message);
            }     
        }

        return newDlls;
    }
}
#endif