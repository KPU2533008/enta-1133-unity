using DungeonGame.Combat;
using DungeonGame.Object;
using System.Collections;
using UnityEngine;

namespace DungeonGame.Item {
	public abstract class Consumable : AItem {

		[SerializeField] protected string UsageText = "@USER uses a @ITEM on @TARGET!";

		public override IEnumerator OnUse(Combatant user, Combatant target) {
			Game.DialogBox.ShowDialog(UsageText.Replace("@USER", user.GetFullName()).Replace("@ITEM", Name).Replace("@TARGET", ( user == target ? "themself" : target.GetFullName() )));
			yield return new WaitUntil(() => !Game.DialogBox.IsShowing);
		}

	}
}
