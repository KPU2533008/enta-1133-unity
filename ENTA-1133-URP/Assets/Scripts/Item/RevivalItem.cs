using DungeonGame.Combat;
using DungeonGame.Enum;
using DungeonGame.Object;
using System.Collections;
using UnityEngine;

namespace DungeonGame.Item {
	public class RevivalItem : Consumable {

		public RevivalItem() {
			TargetAllegiance = Allegiance.Friendly;
			TargetMortality = Mortality.Dead;
		}

		public override IEnumerator OnUse(Combatant user, Combatant target) {
			yield return base.OnUse(user, target);

			int health = Roll();
			target.Heal(health);

			Game.DialogBox.ShowDialog($"{target.GetFullName()} is revived with {health} HP!");
			yield return new WaitUntil(() => !Game.DialogBox.IsShowing);
		}

	}
}
