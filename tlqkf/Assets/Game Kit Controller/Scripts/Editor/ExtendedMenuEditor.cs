using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ExtendedMenuEditor : EditorWindow {

	[MenuItem ("GameObject/GKC/Create New Character")]
	static void createNewPlayer ()
	{
		GetWindow<CharacterCreatorEditor> ();
	}
}
