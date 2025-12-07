using DungeonGame.Abstract;
using DungeonGame.Enum;
using DungeonGame.Item;
using System;
using System.Collections;
using UnityEngine;

namespace DungeonGame.Combat {
	public abstract class Combatant : LivingEntity {

		public char Suffix { get; set; } = ' ';
		public Team? Team { get; private set; }

		private int Level;
		private int XP;
		public int MaxXP => GetMaxXpForLevel(Level);

		public delegate void Selector<T>(T value);

		public abstract void SelectCombatAction(Selector<CombatAction> select);
		public abstract void SelectWeapon(Selector<Weapon?> select);
		public abstract void SelectConsumable(Selector<Consumable?> select);
		public abstract void SelectTarget(Combatant[] validTargets, Selector<Combatant?> select);

		public abstract string GetPassMessage();
		public abstract string GetDefeatMessage();

		private static int GetMaxXpForLevel(int level) {
			return 10 + (int)( Math.Pow(5 * ( level - 1 ), 1.5f) );
		}

		public int GetXP() {
			return XP;
		}

		public void SetXP(int xp) {
			int sign = Math.Sign(xp);
			int lvlIncrease = Math.Min(sign, 0);

			while ( ( xp >= GetMaxXpForLevel(Level + lvlIncrease) || xp < 0 ) && sign == Math.Sign(xp) ) {
				xp -= GetMaxXpForLevel(Level + lvlIncrease) * sign;
				lvlIncrease += sign;
			}

			lvlIncrease -= Math.Min(sign, 0);
			XP = xp;
			if ( lvlIncrease != 0 )
				SetLevel(Level + lvlIncrease);
		}

		public int GetLevel() {
			return Level;
		}

		public void SetLevel(int level) {
			level = Math.Max(level, 1);
			Level = level;
			if ( Math.Min(XP, MaxXP - 1) != XP )
				SetXP(Math.Min(XP, MaxXP - 1));
		}

		public string GetFullName() {
			return Name + ( Suffix == ' ' ? "" : $" {Suffix}" );
		}

		public bool CanAct() {
			return true;
		}

		protected bool CanUseItem(AItem item) {
			Debug.Log($"[Combatant.cs] Checking can use {item} {item.Name}");
			if ( Team == null ) {
				Debug.Log($"[Combatant.cs] Cannot use because no team");
				return false;
			}

			Combatant[] allegiantMembers = Team.GetAllegiantMembers(item.TargetAllegiance, item.TargetMortality);

			Debug.Log($"[Combatant.cs] Allegiant member count: {allegiantMembers.Length}, {allegiantMembers.Length > 0}");
			return allegiantMembers.Length > 0;
		}

		public void SetTeam(Team team) {
			if ( Team == team ) {
				return;
			}

			Team lastTeam = Team;
			Team = team;

			lastTeam?.RemoveMember(this);
			team?.AddMember(this);
		}

	}
}
