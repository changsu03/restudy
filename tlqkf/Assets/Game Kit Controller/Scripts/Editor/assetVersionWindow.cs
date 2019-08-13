using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;
public class assetVersionWindow : EditorWindow
{
	GUISkin guiSkin;

	Texture2D GKCLogo = null;
	Vector2 rect = new Vector2(380, 250);

	void OnEnable()
	{
		GKCLogo = (Texture2D)Resources.Load("GKC_Logo", typeof(Texture2D));
	}

	[MenuItem ("Game Kit Controller/About GKC")]
	public static void AboutGKC ()
	{
		GetWindow<assetVersionWindow> ();
	}

	void OnGUI()
	{
		this.titleContent = new GUIContent("About");
		this.minSize = rect;

		EditorGUILayout.Space();

		EditorGUILayout.Space();

		EditorGUILayout.Space();

		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label(GKCLogo, GUILayout.MaxHeight(130));
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();

		if (!guiSkin) {
			guiSkin = Resources.Load ("GUI") as GUISkin;
		}
		GUI.skin = guiSkin;

		GUILayout.BeginVertical("window");

		GUILayout.BeginHorizontal("box");
		GUILayout.FlexibleSpace();
		GUILayout.Label("Game Kit Controller Version: 3.01b", EditorStyles.boldLabel);
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();

		EditorGUILayout.Space();

		if (GUILayout.Button("Open Tutorial Videos"))
		{
			Application.OpenURL ("https://www.youtube.com/watch?v=r0cKbNYUCZA&list=PLYVCbGEtbhxVjZ9C41fwTDynTpVkCP9iA");
		}

		EditorGUILayout.Space();

		if (GUILayout.Button("Go to the Forum"))
		{
			Application.OpenURL ("https://forum.unity.com/threads/released-game-kit-controller-engine-with-weapons-vehicles-more-2-9-local-multiplayer.351456/");
		}

		EditorGUILayout.Space();

		if (GUILayout.Button("Join Discord"))
		{
			Application.OpenURL ("https://discord.gg/kUpeRZ8https://discord.gg/kUpeRZ8");
		}

		EditorGUILayout.Space();

		if (GUILayout.Button("Close"))
		{
			this.Close();
		}

		EditorGUILayout.HelpBox("IMPORTANT: If you update your GKC version, delete the previous GKC folder. Make sure you have a backup of your project before", MessageType.Info);

		GUILayout.EndVertical();
	}
}
#endif