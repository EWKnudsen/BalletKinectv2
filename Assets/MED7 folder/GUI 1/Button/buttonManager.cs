using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class buttonManager : MonoBehaviour
{
	public void startBtn(string startScene)
	{
		SceneManager.LoadScene (startScene);
	}

	public void infoBtn(string infoScene)
	{
		SceneManager.LoadScene (infoScene);
	}

	public void backBtn1(string menuScene)
	{
		SceneManager.LoadScene (menuScene);
	}

	public void exitBtn()
	{
		Application.Quit ();
	}
}

