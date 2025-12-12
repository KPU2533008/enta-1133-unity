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

		public virtual Allegiance TargetAllegiance => Allegiance.Hostile;
		public virtual Mortality TargetMortality => Mortality.Alive;

		protected Dice dice;
		public string DieType => dice.GetDieType();
		public int LastRoll => dice.GetLastRoll();

		public int Roll() {
			return dice.Roll();
		}

		public abstract IEnumerator OnUse(Combatant user, Combatant target);

		void Awake() {
			dice = new(Faces, NumDice);
		}
	}
}
