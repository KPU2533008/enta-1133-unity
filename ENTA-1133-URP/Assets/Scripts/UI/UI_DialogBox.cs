using DungeonGame;
using DungeonGame.Util;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UI_DialogBox : MonoBehaviour {

	[SerializeField] private Image Container;
	[SerializeField] private TextMeshProUGUI DialogText;
	[SerializeField] private VerticalLayoutGroup OptionsList;
	[SerializeField] private UI_DialogOption OptionPrefab;

	private string[] options = { };
	private string[] dialog = { };
	private int currentIdx = 0;

	public Signal<int> OptionChosen = new();
	public bool IsShowing => Container.gameObject.activeSelf;

	private void ClearOptions() {
		OptionsList.gameObject.SetActive(false);
		foreach ( Transform child in OptionsList.transform ) {
			if ( child == OptionPrefab.transform )
				continue;
			Destroy(child.gameObject);
		}
		options = new string[] { };
	}

	private void Advance() {
		if ( currentIdx < dialog.Length - 1 ) {
			currentIdx++;
			DialogText.text = string.Empty;
		} else {
			ClearOptions();
			Game.Player.MouseUnlocked.Set("Dialog", false);
			Game.Player.WalkSpeed = 2.5f;
			Container.gameObject.SetActive(false);
		}
	}

	public void ShowDialog(string[] dialog, string[] options = null) {
		Container.gameObject.SetActive(true);
		Game.Player.MouseUnlocked.Set("Dialog", true);
		Game.Player.WalkSpeed = 0;
		this.options = options ?? new string[] { };
		this.dialog = dialog;
		this.currentIdx = -1;
		Advance();
	}

	public void ShowDialog(string dialog, string[] options = null) {
		ShowDialog(new string[] { dialog }, options);
	}

	public void ShowOptions(string[] options) {
		for ( int i = 0; i < options.Length; i++ ) {
			int optionNum = i; // why does the value of i change. literally wtf c#
			UI_DialogOption option = Instantiate(OptionPrefab);
			option.TextObject.text = $"{options[i]}";
			option.gameObject.name = $"Option{i + 1}";
			option.gameObject.transform.SetParent(OptionsList.transform);
			option.gameObject.SetActive(true);
			option.ButtonObject.onClick.AddListener(() => {
				OptionChosen.Fire(optionNum);
				ClearOptions();
				Advance();
			});
		}
		OptionsList.gameObject.SetActive(true);
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Awake() {
		Game.DialogBox = this;
	}

	// Update is called once per frame
	void Update() {
		if ( !IsShowing )
			return;

		if ( dialog.Length == 0 || currentIdx >= dialog.Length || currentIdx < 0 )
			return;

		if ( Input.GetMouseButtonDown(0) ) {
			if ( DialogText.text == dialog[currentIdx] && options.Length == 0 ) {
				Advance();
			} else {
				DialogText.text = dialog[currentIdx];
			}
		}

		if ( DialogText.text.Length != dialog[currentIdx].Length ) {
			DialogText.text = dialog[currentIdx].Substring(0, DialogText.text.Length + 1);
		} else if ( !OptionsList.gameObject.activeSelf && options.Length > 0 ) {
			ShowOptions(options);
		}
	}
}
