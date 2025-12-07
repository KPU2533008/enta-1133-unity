using DungeonGame.Enum;
using DungeonGame.Object;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonGame {
	public class DungeonGameRunner : MonoBehaviour {

		//private Dungeon dungeon;
		//private DungeonRoom currentRoom;
		//private DungeonGamePlayer player;

		private Dictionary<WorldAction, Action> worldActions;
		private Dictionary<InventoryAction, Action> inventoryActions;

		public void Start() {
			System.Random rng = new();

			//dungeon = new(new(4, 4));
			//currentRoom = dungeon.GetRandomRoom();
			//player = new();

			//Team playerTeam = new([player]);

			//Game.DialogBox.ClearText();
			//Game.DialogBox.ClearOptions();
			//Game.inventory.Clear();

			//player.inventoryChanged.Connect((bool _) => {
			//	Game.inventory.UpdateItems(player.GetInventoryItems());
			//});

			worldActions = new() {
				{WorldAction.Explore, () => {
					PromptMoveDirection();
				}},

				{WorldAction.Search, () => {
					//currentRoom?.OnSearched(player);
					PromptWorldAction();
				}},

				{WorldAction.ManageItems, () => {
					PromptInventoryAction();
				}},
			};

			inventoryActions = new() {
				{InventoryAction.Use, () => {
					//if (player.Team == null)
					//	return;

					// DO NOT call SelectConsumable() here. That is for COMBAT and will return an item ALWAYS.
					//if (player.PromptSelectItemForUse(typeof(Consumable)) is Consumable item) {
					//	Combatant target = player.SelectTarget(player.Team.GetAllegiantMembers(item.TargetAllegiance, item.TargetMortality));
					//	player.TakeItem(item);
					//	item.OnUse(player, target);
					//}

					PromptInventoryAction();
				}},

				{InventoryAction.Inspect, () => {
					//Item? item = player.PromptSelectItem(typeof(Item));
					//if (item != null) {
					//	Game.DialogBox.ShowDialog(item.Description);
					//}
					PromptInventoryAction();
				}},

				{InventoryAction.Drop, () => {
					//player.PromptDropItem();
					PromptInventoryAction();
				}},

				{InventoryAction.Back, () => {
					PromptWorldAction();
				}},
			};
		}

		private void PromptWorldAction() {
			//if ( !player.IsAlive ) {
			//	Game.DialogBox.ShowDialog("You have been defeated in battle.\n\nGAME OVER.", false);
			//	return;
			//}

			List<string> options = new();
			foreach ( WorldAction worldAction in System.Enum.GetValues(typeof(WorldAction)) ) {
				//options.Add(StringUtil.AddSpaces(worldAction.ToString()));
			}

			//Game.DialogBox.ShowDialog("[ World ]\nWhat would you like to do?", [.. options]);
			//Game.DialogBox.optionChosen.Once((int optionNum) => {
			//	WorldAction action = (WorldAction)optionNum;
			//	Task.Run(() => {
			//		worldActions[action]();
			//	});
			//});
		}

		private void PromptInventoryAction() {
			//List<string> options = [];
			//foreach ( InventoryAction inventoryAction in Enum.GetValues(typeof(InventoryAction)) ) {
			//	options.Add(StringUtil.AddSpaces(inventoryAction.ToString()));
			//}

			//Game.DialogBox.ShowDialog("Inventory Management", [.. options]);
			//Game.DialogBox.optionChosen.Once((int optionNum) => {
			//	InventoryAction action = (InventoryAction)optionNum;
			//	Task.Run(() => {
			//		inventoryActions[action]();
			//	});
			//});
		}

		private void PromptMoveDirection() {
			//Game.DialogBox.ShowDialog("In which direction would you like to explore?", ["North", "South", "East", "West"]);
			//Game.DialogBox.optionChosen.Once((int optionNum) => {
			//	Task.Run(() => {
			//		DungeonRoom? nextRoom = currentRoom.GetNextRoom((MoveDirection)optionNum);

			//		if ( nextRoom != null ) {
			//			MoveToRoom(nextRoom);
			//		} else {
			//			Game.DialogBox.ShowDialog("You look, but there doesn't seem to be another room in this direction.");
			//			PromptWorldAction();
			//		}
			//	});
			//});
		}

		private void MoveToRoom(/*DungeonRoom nextRoom*/) {
			//currentRoom.OnExited(player);
			//currentRoom = nextRoom;
			//currentRoom.OnEntered(player);
			PromptWorldAction();
		}

		public void RunGame() {
			//Game.DialogBox.ShowDialog("You awake in a strange place you've never seen before. Your head is fuzzy, and you don't remember how you got here.");
			//Game.DialogBox.ShowDialog("You can't remember your name either, but you think that it's...\n(TYPE YOUR NAME)", false);

			//UserInputService.GetValidInput((string input, out bool isValid) => {
			//	isValid = input.Length > 0;
			//	return input.Substring(0, Math.Min(input.Length, 8));
			//}, out string playerName);
			//player.SetName(playerName);

			//CombatantInfoCard playerInfoCard = new(player);
			//playerInfoCard.SetPosition(new(0.5f, -( playerInfoCard.container.AbsoluteSize.X + 2 ) / 2, 1, -4));

			//Game.DialogBox.ShowDialog($"{player.Name}... You think your name is {player.Name}!");

			//player.GiveItem(new RustySword());
			//player.GiveItem(new Bandage());
			//player.GiveItem(new Bandage());

			//Game.DialogBox.ShowDialog($"You feel something strapped to your thigh. Something cold. You look down and see an old and rusty sword hanging loosely from your hip.");
			//Game.DialogBox.ShowDialog($"You look around, dizzy and dazed, and try to identify your surroundings. You seem to be in some kind of very old dungeon.");

			//currentRoom.OnEntered(player);
			PromptWorldAction();
		}

	}
}
