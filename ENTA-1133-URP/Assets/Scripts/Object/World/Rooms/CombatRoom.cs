using DungeonGame;
using DungeonGame.Combat;
using DungeonGame.Combat.Enemy;
using DungeonGame.Object;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CombatRoom : DungeonRoom {
	[SerializeField] CombatEncounter CombatPrefab;
	[SerializeField] CombatantWeightTable CombatantWeights;

	private static readonly char[] LETTERS = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N' };

	public override string GetRoomDescription() {
		return "Combat Room";
	}

	private CpuCombatant PickRandomCombatant() {
		return Instantiate(CombatantWeights.Roll());
	}

	private Team AssembleEnemyTeam() {
		System.Random rng = new();
		int numEnemies = rng.Next(0, 2) + 1;

		Dictionary<Type, int> enemyCounts = new();
		List<Combatant> enemies = new();

		for ( int i = 0; i < numEnemies; i++ ) {
			CpuCombatant enemy = PickRandomCombatant();
			Type type = enemy.GetType();

			if ( !enemyCounts.ContainsKey(type) ) {
				enemyCounts[type] = 0;
			}

			enemyCounts[type]++;
			enemies.Add(enemy);
		}

		{
			Dictionary<Type, int> encountered = new();
			foreach ( Combatant enemy in enemies ) {
				Type type = enemy.GetType();

				if ( enemyCounts[type] < 2 )
					continue;

				if ( !encountered.ContainsKey(type) ) {
					encountered[type] = 0;
				}

				enemy.Suffix = LETTERS[encountered[type]];
				encountered[type]++;
			}
		}

		return new(enemies);
	}

	public override void OnEntered(PlayerController player) {
		bool didVisit = isVisited;
		base.OnEntered(player);

		if ( !didVisit && player.Team != null ) {
			Game.DialogBox.ShowDialog("A horde of enemies appears before you!");

			CombatEncounter battle = Instantiate(CombatPrefab);
			battle.AddTeam(player.Team);
			battle.AddTeam(AssembleEnemyTeam());
			battle.RunCombat();

			//	if ( player.IsAlive ) {
			//		Game.DialogBox.ShowDialog("You breathe a sigh of relief as the battle comes to an end.");
			//	}
		}
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start() {

	}

	// Update is called once per frame
	void Update() {

	}
}
