using DungeonGame.Combat;
using DungeonGame.Enum;
using DungeonGame.Item;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DungeonGame.Object {
	[RequireComponent(typeof(Rigidbody))]
	public class PlayerController : Combatant {

		private readonly int INVENTORY_SIZE = 10;
		private readonly List<AItem> inventory = new List<AItem>();

		[SerializeField]
		private Camera _camera;
		private const int LOOK_MULT = 12;
		private float lookUpDown = 0;

		public float WalkSpeed = 2.5f;
		private Vector2 MoveVector = new();
		private Vector3 MoveDirection = new();

		private DungeonRoom _currentRoom;
		private Rigidbody rigidBody;

		public readonly Lock MouseUnlocked = new Lock();

		public delegate bool ItemSelectValidator(AItem item);

		public void PromptSelectItem(Type itemType, ItemSelectValidator onSelect, Action? onCancel = null) {
			Debug.Log("PLAYER IS SELECTING ITEM");
			// TODO: We need an inventory UI
			AItem? selectedItem = null;
			Game.DialogBox.ShowDialog($"Choose a {itemType.ToString()} to use.");
			//bool success = onSelect(selectedItem);
			//if ( !success ) {
			//	Game.DialogBox.ShowDialog($"You cannot pick this item for this action.");
			//}
		}

		public void PromptSelectItemForUse(Type itemType, ItemSelectValidator onSelect, Action? onCancel = null) {
			PromptSelectItem(itemType, (AItem selectedItem) => {
				if ( !CanUseItem(selectedItem) ) {
					return false;
				}
				return onSelect(selectedItem);
			}, onCancel);
		}

		/*
		 * Prompt the player to drop an item in their inventory.
		 * Do not allow them to drop the only weapon they have.
		 */
		public void PromptDropItem() {
			/*
			AItem? chosenItem;

			while ( true ) {
				chosenItem = PromptSelectItem(typeof(AItem));

				if ( chosenItem == null || !chosenItem.GetType().IsSubclassOf(typeof(AWeapon)) ) {
					break;
				} else if ( chosenItem.GetType().IsSubclassOf(typeof(AWeapon)) ) {
					int numWeapons = 0;

					foreach ( AItem item in inventory ) {
						if ( item is AWeapon )
							numWeapons++;
					}

					if ( numWeapons <= 1 )
						Game.DialogBox.ShowDialog($"You can't throw away your only weapon!", true);
					else
						break;
				}

			}

			if ( chosenItem != null ) {
				TakeItem(chosenItem);
			}

			Game.DialogBox.ClearText();
			*/
		}

		public int GiveItem(AItem item) {
			if ( inventory.Contains(item) )
				return -1; // ??? hax or im bad

			/*
			* We're nice and we give the user the option to throw
			* away an item if their inventory is full.
			*/
			//if ( inventory.Count >= INVENTORY_SIZE ) {
			//	Game.DialogBox.ShowDialog($"Your inventory is full. Would you like to throw away an item to make space?", new string[] { "Yes", "No" });
			//	bool doPromptDropItem = false;

			//	Game.DialogBox.OptionChosen.Once((int optionNum) => {
			//		doPromptDropItem = optionNum == 0;
			//	});
			//	Game.DialogBox.OptionChosen.Wait();

			//	if ( doPromptDropItem )
			//		PromptDropItem();

			//	if ( inventory.Count >= INVENTORY_SIZE )
			//		return -2;
			//}

			inventory.Add(item);
			return 0;
		}

		public int TakeItem(AItem item) {
			if ( !inventory.Contains(item) )
				return -1;

			inventory.Remove(item);
			return 0;
		}

		public AItem[] GetInventoryItems() {
			return inventory.ToArray();
		}

		public override string GetDefeatMessage() {
			return $"{GetFullName()} sustains fatal injuries and meets their demise...";
		}

		public override string GetPassMessage() {
			return $"{GetFullName()} passes up their turn and bides their time.";
		}

		public override void SelectCombatAction(Selector<CombatAction> select) {
			List<string> options = new();
			foreach ( CombatAction action in System.Enum.GetValues(typeof(CombatAction)) ) {
				options.Add(Util.StringUtil.AddSpaces(action.ToString()));
			}

			Game.DialogBox.ShowDialog($"{GetFullName()}, choose an action to take this turn!", options.ToArray());
			Game.DialogBox.OptionChosen.Once((int optionNum) => {
				select((CombatAction)optionNum);
			});
		}

		public override void SelectWeapon(Selector<Weapon> select) {
			PromptSelectItemForUse(typeof(Weapon), (AItem item) => {
				select(item as Weapon);
				return true;
			});
		}

		public override void SelectConsumable(Selector<Consumable> select) {
			PromptSelectItemForUse(typeof(Consumable), (AItem item) => {
				TakeItem(item);
				select(item as Consumable);
				return true;
			});
		}

		public override void SelectTarget(Combatant[] validTargets, Selector<Combatant> select) {
			//List<string> options = new();

			//foreach ( Combatant target in validTargets ) {
			//	options.Add(target.GetFullName());
			//}

			//int chosenTargetNum = 0;

			//Game.DialogBox.ShowDialog($"Now choose a target for this action!", options.ToArray());
			//yield return SignalExtensions.WaitForSignal(Game.DialogBox.OptionChosen, optionNum => {
			//	chosenTargetNum = optionNum;
			//});

			//select(validTargets[chosenTargetNum]);
		}

		void OnMove(InputValue value) {

			Vector2 moveVector = value.Get<Vector2>();
			MoveVector = moveVector;
		}

		void OnLook(InputValue value) {
			if ( Cursor.lockState == CursorLockMode.None )
				return;

			Vector2 lookVector = value.Get<Vector2>();
			transform.rotation *= Quaternion.Euler(0, lookVector.x * Time.deltaTime * LOOK_MULT, 0);

			lookUpDown = Math.Clamp(lookUpDown - ( lookVector.y * Time.deltaTime * LOOK_MULT ), -80, 75);
			_camera.transform.rotation = transform.rotation * Quaternion.Euler(lookUpDown, 0, 0);
		}

		void OnInteract(InputValue value) {
			if ( _currentRoom != null ) {
				_currentRoom.OnSearched(this);
			}
		}

		private void Start() {
			rigidBody = GetComponent<Rigidbody>();
			rigidBody.maxLinearVelocity = WalkSpeed;
			rigidBody.useGravity = false;
			rigidBody.collisionDetectionMode = CollisionDetectionMode.Discrete;
			rigidBody.detectCollisions = true;
		}

		private void FixedUpdate() {
			rigidBody.angularVelocity = Vector3.zero;
			rigidBody.linearVelocity = MoveDirection * WalkSpeed;
		}

		// Update is called once per frame
		void Update() {
			Cursor.lockState = MouseUnlocked.IsLocked ? CursorLockMode.None : CursorLockMode.Locked;
			MoveDirection = ( transform.rotation * Vector3.forward * MoveVector.y ) + ( transform.rotation * Vector3.right * MoveVector.x );
		}

		private void OnTriggerEnter(Collider other) {
			DungeonRoom room = other.GetComponent<DungeonRoom>();
			if ( room != null ) {
				if ( _currentRoom != null ) {
					_currentRoom.OnExited(this);
				}
				_currentRoom = room;
				room.OnEntered(this);
			}
		}

		private void OnTriggerExit(Collider other) {
			DungeonRoom room = other.GetComponent<DungeonRoom>();
			if ( room == _currentRoom ) {
				room.OnExited(this);
			}
		}
	}
}