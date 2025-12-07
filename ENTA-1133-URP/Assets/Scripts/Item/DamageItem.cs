using DungeonGame.Combat;
using DungeonGame.Enum;
using DungeonGame.Object;
using System.Collections;
using UnityEngine;

namespace DungeonGame.Item {
	public class DamageItem : Consumable {

		public DamageItem() {
			TargetAllegiance = Allegiance.Hostile;
		}

		public override IEnumerator OnUse(Combatant user, Combatant target) {
			yield return base.OnUse(user, target);

			int damage = Roll();
			target.TakeDamage(damage);

			Game.DialogBox.ShowDialog($"{target.GetFullName()} takes {damage} HP of damage!");
			yield return new WaitUntil(() => !Game.DialogBox.IsShowing);
		}

	}
}
