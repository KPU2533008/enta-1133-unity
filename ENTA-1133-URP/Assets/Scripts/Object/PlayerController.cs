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

		public IEnumerator PromptSelectItem(string prompt, Type itemType, ItemSelectValidator onSelect, Action? onCancel = null) {
			Game.DialogBox.ShowDialog(prompt);
			yield return new WaitUntil(() => !Game.DialogBox.IsShowing);
			Game.InventoryMenu.PromptItemSelection(inventory.ToArray(), itemType, onSelect, onCancel);
		}

		public IEnumerator PromptSelectItemForUse(string prompt, Type itemType, ItemSelectValidator onSelect, Action? onCancel = null) {
			yield return PromptSelectItem(prompt, itemType, (AItem selectedItem) => {
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
		public IEnumerator PromptDropItem() {
			int numWeapons = 0;

			foreach ( AItem item in inventory ) {
				if ( item is Weapon )
					numWeapons++;
			}

			Type allowedItems = numWeapons > 1 ? typeof(AItem) : typeof(Consumable);
			yield return PromptSelectItem("Please select an item to drop.", allowedItems, (AItem item) => {
				TakeItem(item);
				return true;
			});
		}

		public bool HasItem(AItem item) {
			return inventory.Contains(item);
		}

		public IEnumerator GiveItem(AItem item) {
			if ( HasItem(item) ) {
				yield return true;
			}

			/*
			* We're nice and we give the user the option to throw
			* away an item if their inventory is full.
			*/
			if ( inventory.Count >= INVENTORY_SIZE ) {
				Game.DialogBox.ShowDialog($"Your inventory is full. Would you like to throw away an item to make space?");
				yield return new WaitUntil(() => !Game.DialogBox.IsShowing);
				yield return PromptDropItem();
				yield return new WaitUntil(() => !Game.InventoryMenu.IsShowing);

			}

			if ( inventory.Count < INVENTORY_SIZE ) {
				inventory.Add(item);
			}

			yield return true;
		}

		public int TakeItem(AItem item) {
			if ( !HasItem(item) )
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
			StartCoroutine(PromptSelectItemForUse("Choose a weapon to attack with.", typeof(Weapon), (AItem item) => {
				select(item as Weapon);
				return true;
			}, () => { select(null); }));
		}

		public override void SelectConsumable(Selector<Consumable> select) {
			StartCoroutine(PromptSelectItemForUse("Choose an item to use.", typeof(Consumable), (AItem item) => {
				TakeItem(item);
				select(item as Consumable);
				return true;
			}, () => { select(null); }));
		}

		public override void SelectTarget(Combatant[] validTargets, Selector<Combatant> select) {
			List<string> options = new();

			foreach ( Combatant target in validTargets ) {
				options.Add(target.GetFullName());
			}
			options.Add("Cancel...");

			Game.DialogBox.ShowDialog($"Now choose a target for this action!", options.ToArray());
			Game.DialogBox.OptionChosen.Once((int optionNum) => {
				if ( optionNum == options.Count - 1 ) {
					select(null);
					return;
				}
				select(validTargets[optionNum]);
			});
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