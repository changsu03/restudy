using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class homeMenu : MonoBehaviour
{
	public void confirmExit ()
	{
		Application.Quit ();
	}

	public void loadScene (int sceneNumber)
	{
		SceneManager.LoadScene (sceneNumber);
	}

}