﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;


public class ExportPackage
{
	[MenuItem ("Export/MyExport")]
	static void export ()
	{
		AssetDatabase.ExportPackage (
			AssetDatabase.GetAllAssetPaths (), 
			PlayerSettings.productName + ".unitypackage", 
			ExportPackageOptions.Interactive | ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies | ExportPackageOptions.IncludeLibraryAssets);
	}
}
#endif