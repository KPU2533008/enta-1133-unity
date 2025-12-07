using DungeonGame.Enum;
using DungeonGame.Item;
using System;
using System.Collections;
using UnityEngine;

namespace DungeonGame.Combat.Enemy {
	public class CpuCombatant : Combatant {

		[SerializeField] private Weapon[] Weapons;
		[SerializeField] private Consumable[] Consumables;
		[SerializeField] private string DefeatMessage = "";
		[SerializeField] private string PassMessage = "";

		public override string GetDefeatMessage() {
			return DefeatMessage;
		}

		public override string GetPassMessage() {
			return PassMessage;
		}

		public override void SelectCombatAction(Selector<CombatAction> select) {
			System.Random rng = new();
			int roll = rng.Next(0, 10);

			/*
			 * Attack - 70%
			 * UseItem - 20%
			 * Pass - 10%
			 */
			if ( roll < 3 ) {
				select(CombatAction.UseItem);
			} else if ( Team?.GetAllegiantMembers(Allegiance.Hostile, Mortality.Alive).Length > 0 ) {
				select(CombatAction.Attack);
			} else {
				select(CombatAction.Pass);
			}
		}

		public override void SelectConsumable(Selector<Consumable> select) {
			System.Random rng = new();
			Consumable item = Instantiate(Consumables[rng.Next(0, Consumables.Length)]);
			select(CanUseItem(item) ? item : null);
		}

		public override void SelectWeapon(Selector<Weapon> select) {
			System.Random rng = new();
			Weapon item = Instantiate(Weapons[rng.Next(0, Consumables.Length)]);
			select(CanUseItem(item) ? item : null);
		}

		public override void SelectTarget(Combatant[] validTargets, Selector<Combatant> select) {
			System.Random rng = new();
			select(validTargets[rng.Next(0, validTargets.Length)]);
		}
	}
}
