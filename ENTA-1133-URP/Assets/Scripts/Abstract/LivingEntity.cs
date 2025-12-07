using DungeonGame.Enum;
using System;
using UnityEngine;

namespace DungeonGame.Abstract {
	public class LivingEntity : Entity {

		[field: SerializeField] public int MaxHP { get; private set; } = 10;
		[field: SerializeField] public int HP { get; private set; } = 10;

		public bool IsAlive => HP > 0;
		public Mortality Mortality => IsAlive ? Mortality.Alive : Mortality.Dead;

		private void SetHP(int hp) {
			HP = Math.Clamp(hp, 0, MaxHP);
		}

		public void SetMaxHP(int maxHp) {
			MaxHP = maxHp;
			SetHP(HP);
		}

		public bool TakeDamage(int damage) {
			SetHP(HP - damage);
			return HP > 0;
		}

		public void Heal(int health) {
			SetHP(HP + health);
		}

		public override void Destroy() {
			base.Destroy();
		}

	}
}
