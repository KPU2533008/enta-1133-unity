using DungeonGame.Object;
using TMPro;
using UnityEngine;

public class UI_DieRoller : MonoBehaviour {
	Dice die = new();

	public void Roll(TextMeshProUGUI outputGui) {
		outputGui.text = $"{die.GetDieType()}: {die.Roll()}";
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start() {

	}

	// Update is called once per frame
	void Update() {

	}
}
