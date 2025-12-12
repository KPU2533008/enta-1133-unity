using DungeonGame;
using DungeonGame.Enum;
using DungeonGame.Item;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureChest : MonoBehaviour {
	[SerializeField, Range(1, 8)] public int maxRolls = 4;
	[SerializeField] private ItemWeightTable lootTable;

	private List<AItem> contents = new();

	public IEnumerator Loot() {
		Game.Player.WalkSpeed = 0;

		foreach ( AItem item in contents.ToArray() ) {
			Game.DialogBox.ShowDialog($"You found a {item.Name} inside the chest!");
			yield return new WaitUntil(() => !Game.DialogBox.IsShowing);
			yield return Game.Player.GiveItem(item);
			if ( Game.Player.HasItem(item) ) {
				contents.Remove(item);
			} else {
				Game.DialogBox.ShowDialog($"You put the {item.Name} back.");
				yield return new WaitUntil(() => !Game.DialogBox.IsShowing);
				break;
			}
		}

		if ( contents.Count == 0 ) {
			Destroy(gameObject);
		}

		Game.Player.WalkSpeed = 2.5f;
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start() {
		System.Random rng = new();
		for ( int i = 0; i < rng.Next(1, maxRolls + 1); i++ ) {
			contents.Add(Instantiate(lootTable.Roll()));
		}
	}

	// Update is called once per frame
	void Update() {
		if ( Input.GetKeyDown(KeyCode.E) && contents.Count > 0 ) {
			Vector3 playerPos = Game.Player.gameObject.transform.position;
			Vector3 thisPos = gameObject.transform.position;

			if ( ( playerPos - thisPos ).magnitude < 1.25 )
				StartCoroutine(Loot());
		}
	}
}
