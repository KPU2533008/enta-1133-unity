using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_TitleScreen : MonoBehaviour {
	public void QuitGame() {
#if UNITY_EDITOR
		EditorApplication.ExitPlaymode();
#else
			Application.Quit();
#endif
	}

	public void LoadScene(string scene) {
		SceneManager.LoadScene(scene);
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start() {
		Cursor.lockState = CursorLockMode.None;
	}

	// Update is called once per frame
	void Update() {

	}
}
