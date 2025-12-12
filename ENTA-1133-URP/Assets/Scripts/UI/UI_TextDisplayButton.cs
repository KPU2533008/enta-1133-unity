using DungeonGame.Util;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_TextDisplayButton : MonoBehaviour {
	[SerializeField] private Button Button;
	[SerializeField] private TextMeshProUGUI TextMesh;
	[SerializeField] private ButtonHoverHandler HoverHandler;

	public void SetText(string text) {
		TextMesh.text = text ?? string.Empty;
	}

	public void SetInteractable(bool interactable) {
		Button.interactable = interactable;
	}

	public Connection BindToHover(Action callback) {
		return HoverHandler.OnHoverBegan.Connect((PointerEventData _) => {
			callback();
		});
	}

	public void BindToClick(Action callback) {
		Button.onClick.AddListener(() => {
			if ( Button.interactable )
				callback();
		});
	}
}
