#if UNITY_EDITOR && !UNITY_5

using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System;

public class AkWwiseMenu_Android : MonoBehaviour {

	private static AkUnityPluginInstaller_Android m_installer = new AkUnityPluginInstaller_Android();
	// private static AkUnityIntegrationBuilder_Android m_builder = new AkUnityIntegrationBuilder_Android();

	[MenuItem("Assets/Wwise/Install Plugins/Android/armeabi-v7a/Debug", false, (int)AkWwiseMenuOrder.AndroidDebug)]
	public static void InstallPlugin_armeabiv7a_Debug () {
		m_installer.InstallPluginByArchConfig("armeabi-v7a", "Debug");
	}

	[MenuItem("Assets/Wwise/Install Plugins/Android/armeabi-v7a/Profile", false, (int)AkWwiseMenuOrder.AndroidProfile)]
	public static void InstallPlugin_armeabiv7a_Profile () {
		m_installer.InstallPluginByArchConfig("armeabi-v7a", "Profile");
	}

	[MenuItem("Assets/Wwise/Install Plugins/Android/armeabi-v7a/Release", false, (int)AkWwiseMenuOrder.AndroidRelease)]
	public static void InstallPlugin_armeabiv7a_Release () {
		m_installer.InstallPluginByArchConfig("armeabi-v7a", "Release");
	}
	
	[MenuItem("Assets/Wwise/Install Plugins/Android/x86/Debug", false, (int)AkWwiseMenuOrder.AndroidDebug)]
	public static void InstallPlugin_x86_Debug () {
		m_installer.InstallPluginByArchConfig("x86", "Debug");
	}

	[MenuItem("Assets/Wwise/Install Plugins/Android/x86/Profile", false, (int)AkWwiseMenuOrder.AndroidProfile)]
	public static void InstallPlugin_x86_Profile () {
		m_installer.InstallPluginByArchConfig("x86", "Profile");
	}

	[MenuItem("Assets/Wwise/Install Plugins/Android/x86/Release", false, (int)AkWwiseMenuOrder.AndroidRelease)]
	public static void InstallPlugin_x86_Release () {
		m_installer.InstallPluginByArchConfig("x86", "Release");
	}

//	[MenuItem("Assets/Wwise/Rebuild Integration/Android/armeabi-v7a/Debug")]
//	public static void RebuildIntegration_Debug_armeabiv7a () {
//		m_builder.BuildByConfig("Debug", "armeabi-v7a");
//	}
//
//	[MenuItem("Assets/Wwise/Rebuild Integration/Android/armeabi-v7a/Profile")]
//	public static void RebuildIntegration_Profile_armeabiv7a () {
//		m_builder.BuildByConfig("Profile", "armeabi-v7a");
//	}
//
//	[MenuItem("Assets/Wwise/Rebuild Integration/Android/armeabi-v7a/Release")]
//	public static void RebuildIntegration_Release_armeabiv7a () {
//		m_builder.BuildByConfig("Release", "armeabi-v7a");
//	}
}


public class AkUnityPluginInstaller_Android : AkUnityPluginInstallerBase
{
	public AkUnityPluginInstaller_Android()
	{
		m_platform = "Android";
		m_arches = new string[] { "armeabi-v7a", "x86" };
	}

	public override bool InstallPluginByArchConfig(string arch, string config)
	{
		string pluginSrc = GetPluginSrcPathByArchConfig(arch, config);
		string pluginDest = GetPluginDestPath(arch);

		bool isSuccess = RecursiveCopyDirectory(new DirectoryInfo(pluginSrc), new DirectoryInfo(pluginDest), m_excludes);
		if( !isSuccess )
		{	
			UnityEngine.Debug.LogError(string.Format("WwiseUnity: Failed to install plugin for {0} ({1}, {2}) from {3} to {4}.", m_platform, arch, config, pluginSrc, pluginDest));
			EditorUtility.ClearProgressBar();
			return false;
		}
						
		UnityEngine.Debug.Log(string.Format("WwiseUnity: Plugin for {0} {1} {2} installed from {3} to {4}.", m_platform, arch, config, pluginSrc, pluginDest));
		AssetDatabase.Refresh();
		return true;
	}	

	protected override string GetPluginDestPath(string arch)
	{
		return Path.Combine(Path.Combine(m_pluginDir, m_platform), Path.Combine("libs", arch));
	}	
}


public class AkUnityIntegrationBuilder_Android : AkUnityIntegrationBuilderBase
{
	private string m_androidSdkDir = "";
	private string m_androidNdkDir = "";
	private string m_apacheAntDir = "";

	public AkUnityIntegrationBuilder_Android()
	{
		m_platform = "Android";
	}

	protected override bool PreBuild()
	{
		// Try to parse config to get NDK location.
		string configPath = Path.Combine(m_buildScriptDir, "BuildWwiseUnityIntegration.json");
		FileInfo fi = new FileInfo(configPath);
		if ( fi.Exists )
		{
			string msg = string.Format("WwiseUnity: Use build variables defined in preference file: {0}. Edit or delete the file by hand if needed.", configPath);
			UnityEngine.Debug.Log(msg);
		}
		else 
		{
			const string AbortMessage = "WwiseUnity: User cancelled the build.";

			string msg = string.Format("WwiseUnity: Need user input because preference file is unavailable at: {0}.", configPath);
			UnityEngine.Debug.Log(msg);

			m_androidSdkDir = EditorUtility.OpenFolderPanel("Choose Android SDK folder", ".", "");

			bool isUserCancelledBuild = m_androidSdkDir == "";
			if (isUserCancelledBuild)
			{
				UnityEngine.Debug.Log(AbortMessage);
				return false;
			}

			m_androidNdkDir = EditorUtility.OpenFolderPanel("Choose Android NDK folder", ".", "");

			isUserCancelledBuild = m_androidNdkDir == "";
			if (isUserCancelledBuild)
			{
				UnityEngine.Debug.Log(AbortMessage);
				return false;
			}

			m_apacheAntDir = EditorUtility.OpenFolderPanel("Choose Apache Ant folder", ".", "");

			isUserCancelledBuild = m_apacheAntDir == "";
			if (isUserCancelledBuild)
			{
				UnityEngine.Debug.Log(AbortMessage);
				return false;
			}

		}

		return true;
	}

	protected override string GetProcessArgs(string config, string arch)
	{
		string args = base.GetProcessArgs(config, arch);

		if (m_androidSdkDir != "")
		{
			args += string.Format(" -s \"{0}\"", m_androidSdkDir);
		}

		if (m_androidNdkDir != "")
		{
			args += string.Format(" -n \"{0}\"", m_androidNdkDir);
		}

		if (m_apacheAntDir != "")
		{
			args += string.Format(" -t \"{0}\"", m_apacheAntDir);
		}

		return args;
	}
}
#endif // #if UNITY_EDITOR