using System;

namespace DungeonGame.Object {
	public class Dice {

		private readonly int faces;
		private readonly int numDice;
		private int lastRoll = -1;

		public bool IsCriticalRoll => lastRoll == numDice * faces;
		public bool IsCriticalFail => lastRoll == numDice;

		internal Dice(int faces = 6, int numDice = 1) {
			this.faces = faces;
			this.numDice = numDice;
		}

		public override string ToString() {
			return $"{numDice}{GetDieType()}[{lastRoll}]";
		}

		internal string GetDieType() {
			return numDice + "d" + faces;
		}

		internal int Roll() {
			Random rng = new();
			int roll = 0;

			for ( int i = 0; i < numDice; i++ ) {
				roll += rng.Next(0, faces) + 1;
			}

			lastRoll = roll;
			return roll;
		}

		internal int GetLastRoll() {
			return lastRoll;
		}
	}
}
