using DungeonGame;
using DungeonGame.Item;
using DungeonGame.Util;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_InventoryMenu : MonoBehaviour {
	[SerializeField] private GameObject Container;
	[SerializeField] private TextMeshProUGUI ItemCountLabel;

	[SerializeField] private VerticalLayoutGroup ItemsList;
	[SerializeField] private UI_TextDisplayButton ItemsListItemPrefab;

	[SerializeField] private TextMeshProUGUI ItemNameLabel;
	[SerializeField] private TextMeshProUGUI ItemDieTypeLabel;
	[SerializeField] private TextMeshProUGUI ItemDescriptionLabel;

	[SerializeField] private UI_TextDisplayButton DiscardItemButton;
	[SerializeField] private UI_TextDisplayButton UseItemButton;
	[SerializeField] private UI_TextDisplayButton CancelButton;

	public bool IsShowing => Container.activeSelf;
	private Signal<AItem> onItemChosen = new();
	private Signal<bool> onSelectionCancelled = new();

	private void ClearItemInspection() {
		ItemNameLabel.text = string.Empty;
		ItemDieTypeLabel.text = string.Empty;
		ItemDescriptionLabel.text = string.Empty;
	}

	private void InspectItem(AItem item) {
		ClearItemInspection();
		if ( item != null ) {
			ItemNameLabel.text = item.Name;
			ItemDieTypeLabel.text = $"Die Type: {item.DieType}";
			ItemDescriptionLabel.text = item.Description.Replace("@DIE_TYPE", item.DieType);
		}
	}

	private void ClearItemDisplayList() {
		foreach ( Transform child in ItemsList.transform ) {
			Destroy(child.gameObject);
		}
	}

	private void DisplayItemList(AItem[] items, Type itemFilterType) {
		ItemCountLabel.text = $"{items.Length}/10";
		ClearItemInspection();
		ClearItemDisplayList();
		for ( int i = 0; i < items.Length; i++ ) {
			AItem item = items[i];
			UI_TextDisplayButton itemsListItem = Instantiate(ItemsListItemPrefab);
			itemsListItem.SetText(item.Name);
			itemsListItem.gameObject.name = $"Item{i + 1}";
			itemsListItem.gameObject.transform.SetParent(ItemsList.transform);
			itemsListItem.gameObject.SetActive(true);
			itemsListItem.SetInteractable(item.GetType() == itemFilterType || item.GetType().IsSubclassOf(itemFilterType));
			itemsListItem.BindToHover(() => {
				InspectItem(item);
			});
			itemsListItem.BindToClick(() => {
				onItemChosen.Fire(item);
			});
		}
	}

	public void PromptItemSelection(AItem[] items, Type itemFilterType, DungeonGame.Object.PlayerController.ItemSelectValidator onSelect, Action? onCancel = null) {
		Container.SetActive(true);
		DisplayItemList(items, itemFilterType);
		Game.Player.MouseUnlocked.Set("Inventory", true);

		Connection connSelect = null;
		Connection connCancel = null;

		Action cleanup = () => {
			Game.Player.MouseUnlocked.Set("Inventory", false);
			Container.SetActive(false);
			connSelect?.Disconnect();
			connCancel?.Disconnect();
		};

		connSelect = onItemChosen.Connect((AItem item) => {
			if ( Game.DialogBox.IsShowing )
				return;
			bool success = onSelect(item);
			if ( success ) {
				cleanup();
			}
		});

		connCancel = onSelectionCancelled.Connect((bool _) => {
			if ( Game.DialogBox.IsShowing )
				return;
			onCancel?.Invoke();
			cleanup();
		});
	}

	void Awake() {
		Game.InventoryMenu = this;
		CancelButton.BindToClick(() => {
			onSelectionCancelled.Fire(false);
		});
	}
}
