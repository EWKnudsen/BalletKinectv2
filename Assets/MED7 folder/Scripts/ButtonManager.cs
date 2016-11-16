using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
	public void ChangeScene(string sceneName) {
		SceneManager.LoadScene (sceneName);
	}

	public void ExitApplication() {
		Application.Quit ();
	}
}

