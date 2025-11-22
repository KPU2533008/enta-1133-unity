using UnityEditor;
using UnityEngine;

public class UI_TitleScreen : MonoBehaviour {
	public void StartGame() {
		this.gameObject.SetActive(false);
	}

	public void QuitGame() {
#if UNITY_EDITOR
		EditorApplication.ExitPlaymode();
#else
			Application.Quit();
#endif
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start() {

	}

	// Update is called once per frame
	void Update() {

	}
}
