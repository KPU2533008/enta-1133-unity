using DungeonGame.Abstract;
using DungeonGame.Combat;
using DungeonGame.Enum;
using DungeonGame.Object;
using System.Collections;
using UnityEngine;

namespace DungeonGame.Item {
	public abstract class AItem : Entity {

		[SerializeField] private int NumDice = 1;
		[SerializeField] private int Faces = 6;
		[SerializeField] public string Description = "A regular item. It doesn't do anything.";

		public Allegiance TargetAllegiance { get; protected set; } = Allegiance.Hostile;
		public Mortality TargetMortality { get; protected set; } = Mortality.Alive;

		protected readonly Dice dice;
		public int LastRoll => dice.GetLastRoll();

		public AItem() {
			dice = new(Faces, NumDice);
		}

		public int Roll() {
			return dice.Roll();
		}

		public abstract IEnumerator OnUse(Combatant user, Combatant target);

	}
}
