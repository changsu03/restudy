using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.UI;
using System;

public class songsManager : MonoBehaviour
{
	public string absolutePathBuild = ".";

	public string absolutePathEditor = "Music";

	public List<AudioClip> clips = new List<AudioClip> ();

	List<string> validExtensions = new List<string> { ".ogg", ".wav" };
	string absolutePath;

	public bool songsLoaded;
	int numberOfSongs;

	void Start ()
	{
		if (Application.isEditor) {
			absolutePath = absolutePathEditor;
		} else {
			absolutePath = absolutePathBuild;
		}
		ReloadSounds ();
	}

	public List<AudioClip> getSongsList ()
	{
		return clips;
	}

	void ReloadSounds ()
	{
		clips.Clear ();
		if (Directory.Exists (absolutePath)) {
			
			//print ("directory found");
			absolutePath += "/";
			// get all valid files
			System.IO.DirectoryInfo info = new System.IO.DirectoryInfo (absolutePath);
			var fileInfo = info.GetFiles ()
				.Where (f => IsValidFileType (f.Name))
				.ToArray ();

			numberOfSongs = fileInfo.Length;
			//print ("Number of songs found: "+numberOfSongs);
			//var fileInfo = info.GetFiles ().OrderBy (p => p.CreationTime).ToArray ();
			//			foreach (FileInfo file in fileInfo) {
			//				if (File.Exists (absolutePath + file.Name)) {
			//					print (file.Name);
			//				}
			//			}
			// and load them
			foreach (FileInfo s in fileInfo) {
				StartCoroutine (LoadFile (s.FullName));
			}
		} else {
			print ("Directory with song files doesn't exist. If you want to use the radio system, place some .wav files on the folde " + absolutePathEditor + " inside the project folder.");
		}
	}

	bool IsValidFileType (string fileName)
	{
		return validExtensions.Contains (Path.GetExtension (fileName));
		// Alternatively, you could go fileName.SubString(fileName.LastIndexOf('.') + 1); that way you don't need the '.' when you add your extensions
	}

	IEnumerator LoadFile (string path)
	{
		WWW www = new WWW ("file://" + path);
		//	print ("loading " + path);

		AudioClip clip = www.GetAudioClip (false);
		while (!clip.isReadyToPlay) {
			yield return www;
		}
		//print ("done loading");
		clip.name = Path.GetFileName (path);
		clips.Add (clip);

		if (clips.Count == numberOfSongs) {
			songsLoaded = true;
		}
	}

	public bool allSongsLoaded ()
	{
		return songsLoaded;
	}
}