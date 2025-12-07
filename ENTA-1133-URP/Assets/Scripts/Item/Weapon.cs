using DungeonGame.Combat;
using DungeonGame.Object;
using System.Collections;
using UnityEngine;

namespace DungeonGame.Item {
	public class Weapon : AItem {

		[SerializeField] private string CriticalRollFlavorText = "The attack lands with expert precision";

		protected IEnumerator BasicAttack(Combatant aggressor, Combatant victim) {
			int damage = Roll();
			string aggressorName = aggressor.GetFullName();
			string victimName = victim.GetFullName();

			Game.DialogBox.ShowDialog($"{aggressorName} attacks {victimName} with {Name}!");
			yield return new WaitUntil(() => !Game.DialogBox.IsShowing);
			victim.TakeDamage(damage);

			if ( dice.IsCriticalRoll ) {
				Game.DialogBox.ShowDialog($"CRITICAL STRIKE! {CriticalRollFlavorText} and {victimName} receives {damage} HP of damage!");
			} else if ( dice.IsCriticalFail ) {
				Game.DialogBox.ShowDialog($"CRITICAL FAIL! {victimName} evades the attack with finesse and grace and receives only {damage} HP of damage!");
			} else {
				Game.DialogBox.ShowDialog($"{victimName} receives {damage} HP of damage!");
			}

			yield return new WaitUntil(() => !Game.DialogBox.IsShowing);
		}

		public override IEnumerator OnUse(Combatant user, Combatant target) {
			yield return BasicAttack(user, target);
		}

	}
}
