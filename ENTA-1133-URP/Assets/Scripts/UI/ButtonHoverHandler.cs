using UnityEngine;
using UnityEngine.EventSystems;
using DungeonGame.Util;

public class ButtonHoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
	public Signal<PointerEventData> OnHoverBegan = new();
	public Signal<PointerEventData> OnHoverEnded = new();

	public void OnPointerEnter(PointerEventData eventData) {
		OnHoverBegan.Fire(eventData);
	}

	public void OnPointerExit(PointerEventData eventData) {
		OnHoverEnded.Fire(eventData);
	}
}