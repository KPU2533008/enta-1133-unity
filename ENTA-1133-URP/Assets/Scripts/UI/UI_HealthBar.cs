using DungeonGame;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_HealthBar : MonoBehaviour {
	[SerializeField] private Image backingBar;
	[SerializeField] private Image fillBar;
	[SerializeField] private TextMeshProUGUI label;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start() {

	}

	// Update is called once per frame
	void Update() {
		fillBar.rectTransform.localScale = new((float)Game.Player.HP / Game.Player.MaxHP, 1, 1);
		label.text = $"Health ({Game.Player.HP}/{Game.Player.MaxHP})";
	}
}
