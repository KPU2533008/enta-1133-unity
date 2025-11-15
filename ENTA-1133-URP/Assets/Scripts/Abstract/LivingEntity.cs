using DungeonGame.Enum;
using System;
using UnityEngine;

namespace DungeonGame.Abstract {
	internal abstract class LivingEntity/*(int maxHp)*/ : Entity {

		//public int MaxHP {
		//	get;
		//	set {
		//		field = value;
		//		//MaxHpChanged.Fire(HP);
		//		HP = HP;
		//	}
		//} = maxHp;

		//public int HP {
		//	get;
		//	private set {
		//		field = Math.Clamp(value, 0, MaxHP);
		//		//HpChanged.Fire(HP);
		//	}
		//} = maxHp;

		//public readonly Signal<int> HpChanged = new();
		//public readonly Signal<int> MaxHpChanged = new();

		private int MaxHP;
		private int HP;

		public bool IsAlive => HP > 0;
		public Mortality Mortality => IsAlive ? Mortality.Alive : Mortality.Dead;

		public LivingEntity(int maxHp) {
			MaxHP = maxHp;
			HP = maxHp;
		}

		private void SetHP(int hp) {
			HP = Math.Clamp(hp, 0, MaxHP);
		}

		public void SetMaxHP(int maxHp) {
			MaxHP = maxHp;
			SetHP(HP);
		}

		public bool TakeDamage(int damage) {
			SetHP(HP - damage);
			//HP -= damage;
			return HP > 0;
		}

		public void Heal(int health) {
			SetHP(HP + health);
			//HP += health;
		}

		public override void Destroy() {
			base.Destroy();
			//HpChanged.Destroy();
			//MaxHpChanged.Destroy();
		}

	}
}
