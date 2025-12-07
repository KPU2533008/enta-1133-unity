using DungeonGame.Combat;
using DungeonGame.Enum;
using DungeonGame.Object;
using System.Collections;
using UnityEngine;

namespace DungeonGame.Item {
	public class HealthRestoreItem : Consumable {

		public HealthRestoreItem() {
			TargetAllegiance = Allegiance.Friendly;
		}

		public override IEnumerator OnUse(Combatant user, Combatant target) {
			yield return base.OnUse(user, target);

			int health = Roll();
			target.Heal(health);

			Game.DialogBox.ShowDialog($"{target.GetFullName()} is healed for {health} HP!");
			yield return new WaitUntil(() => !Game.DialogBox.IsShowing);
		}

	}
}
