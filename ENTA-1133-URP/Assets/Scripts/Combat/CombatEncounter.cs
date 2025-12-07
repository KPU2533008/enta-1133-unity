using DungeonGame.Combat.Enemy;
using DungeonGame.Enum;
using DungeonGame.Item;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static DungeonGame.Combat.Combatant;

namespace DungeonGame.Combat {
	public class CombatEncounter : MonoBehaviour {

		/*
		 * THE PLAN:
		 *
		 * Combat takes place between two "teams", each with one or more `Combatant`s
		 * 
		 * Each team takes turns, with each `Combatant` on a team taking their turn before the next team takes theirs.
		 * 
		 * When a `Combatant` takes their turn, they will choose one `CombatAction` to perform:
		 *		- Attack:
		 *			They will choose a `Weapon` currently in their inventory and then a valid `Combatant` target.
		 *			The `Weapon` will Roll() and the target will take that amount of damage. The weapon will also
		 *			apply any side effects.
		 *		- UseItem
		 *			They will choose a `Consumable` currently in their inventory and then a valid `Combatant` target.
		 *			The `Consumable` will apply its side effects to the target `Combatant`. The `Consumable` will be
		 *			removed from their inventory.
		 *		- Flee
		 *			They will flee from the combat encounter and be removed from their team.
		 *			
		 * The combat encounter continues until all `Combatant`s on a given team have their HP reduced to zero ("defeated").
		 * 
		 */

		private bool combatStarted = false;
		private List<Team> teams = new();

		private int currentTeamIdx = 0;
		private int currentCombatantIdx = 0;

		private CombatTurnState turnState = CombatTurnState.Begin;
		private CombatAction? chosenAction = null;
		private AItem? chosenItem = null;
		private Combatant? chosenTarget = null;

		public void AddTeam(Team team) {
			if ( !combatStarted )
				teams.Add(team);
		}

		// We do this so that any classes we pass a selector to don't accidentally make 2 selections and overwrite their first one
		private Selector<T> SingleUseSelector<T>(Selector<T> selector) {
			bool selected = false;
			return (T value) => {
				if ( selected )
					return;
				selected = true;
				selector(value);
			};
		}

		private Team[] GetUndefeatedTeams() {
			return ( from team in teams where !team.IsDefeated select team ).ToArray();
		}

		private void AssignAllegiances() {
			foreach ( Team team in teams ) {
				team.ResetAllegiances();
				foreach ( Team otherTeam in teams ) {
					if ( team == otherTeam )
						continue;
					team.SetTeamAllegiance(otherTeam, Allegiance.Hostile);
				}
			}
		}

		private IEnumerator TakeTurn(Combatant combatant, CombatAction action, AItem? item, Combatant? target) {
			turnState = CombatTurnState.Executing;
			if ( chosenAction == CombatAction.Pass ) {
				Game.DialogBox.ShowDialog(combatant.GetPassMessage());
				yield return new WaitUntil(() => !Game.DialogBox.IsShowing);
			} else {
				bool wasTargetAlive = chosenTarget.IsAlive;

				yield return chosenItem.OnUse(combatant, chosenTarget);

				if ( !chosenTarget.IsAlive && wasTargetAlive ) {
					Game.DialogBox.ShowDialog(chosenTarget.GetDefeatMessage());
					yield return new WaitUntil(() => !Game.DialogBox.IsShowing);
				}
			}
			turnState = CombatTurnState.End;
		}

		public void RunCombat() {
			AssignAllegiances();
			combatStarted = true;
		}

		void Start() { }

		private void OnDestroy() {
			foreach ( Team team in teams ) {
				foreach ( Combatant combatant in team.GetMembers(Mortality.Any) ) {
					if ( combatant is CpuCombatant cpu ) {
						Destroy(cpu.gameObject);
					}
				}
			}
		}

		void Update() {
			if ( Game.DialogBox.IsShowing ) {
				return;
			}

			if ( GetUndefeatedTeams().Length <= 1 ) {
				foreach ( Team team in teams ) {
					team.ResetAllegiances();
				}
				Destroy(gameObject);
				return;
			}

			if ( combatStarted ) {
				Team team = teams[currentTeamIdx];
				Combatant combatant = team.GetMembers(Mortality.Any)[currentCombatantIdx];

				if ( turnState == CombatTurnState.Begin ) {
					Debug.Log($"[CombatEncounter.cs] Ask {combatant.Name} for action");
					chosenAction = null;
					turnState = CombatTurnState.ChoosingAction;

					combatant.SelectCombatAction(SingleUseSelector((CombatAction chosenAction) => {
						Debug.Log($"[CombatEncounter.cs] {combatant.Name} selected action {chosenAction}");
						this.chosenAction = chosenAction;
						if ( chosenAction == CombatAction.Pass ) {
							turnState = CombatTurnState.Ready;
						}
					}));
				} else if ( turnState == CombatTurnState.ChoosingAction && chosenAction != null ) {
					chosenItem = null;
					turnState = CombatTurnState.ChoosingItem;

					if ( chosenAction == CombatAction.Attack ) {
						Debug.Log($"[CombatEncounter.cs] Ask {combatant.Name} for weapon");
						combatant.SelectWeapon(SingleUseSelector((Weapon? chosenItem) => {
							Debug.Log($"[CombatEncounter.cs] {combatant.Name} selected weapon {chosenItem}");
							this.chosenItem = chosenItem;
							if ( chosenItem == null ) {
								Debug.Log($"[CombatEncounter.cs] {combatant.Name} did not choose weapon, go back");
								turnState = CombatTurnState.Begin;
							}
						}));
					} else if ( chosenAction == CombatAction.UseItem ) {
						Debug.Log($"[CombatEncounter.cs] Ask {combatant.Name} for consumable");
						combatant.SelectConsumable(SingleUseSelector((Consumable? chosenItem) => {
							Debug.Log($"[CombatEncounter.cs] {combatant.Name} selected consumable {chosenItem}");
							this.chosenItem = chosenItem;
							if ( chosenItem == null ) {
								Debug.Log($"[CombatEncounter.cs] {combatant.Name} did not choose consumalbe, go back");
								turnState = CombatTurnState.Begin;
							}
						}));
					}
				} else if ( turnState == CombatTurnState.ChoosingItem && chosenItem != null ) {
					Debug.Log($"[CombatEncounter.cs] Ask {combatant.Name} for target");
					chosenTarget = null;
					turnState = CombatTurnState.ChoosingTarget;
					Combatant[] validTargets = team.GetAllegiantMembers(chosenItem.TargetAllegiance, chosenItem.TargetMortality);
					combatant.SelectTarget(validTargets, SingleUseSelector((Combatant? chosenTarget) => {
						Debug.Log($"[CombatEncounter.cs] {combatant.Name} selected target {chosenTarget}");
						this.chosenTarget = chosenTarget;
						if ( chosenTarget == null ) {
							Debug.Log($"[CombatEncounter.cs] {combatant.Name} did not choose target, go back");
							turnState = CombatTurnState.ChoosingAction;
						} else {
							turnState = CombatTurnState.Ready;
						}
					}));
				}

				if ( turnState == CombatTurnState.Ready ) {
					Debug.Log($"[CombatEncounter.cs] {combatant.Name} ready");
					StartCoroutine(TakeTurn(combatant, chosenAction ?? CombatAction.Pass, chosenItem, chosenTarget));
				}

				if ( turnState == CombatTurnState.Executing ) {
					Debug.Log($"[CombatEncounter.cs] Waiting for {combatant.Name} turn finish...");
					return;
				} else if ( turnState == CombatTurnState.End ) {
					Debug.Log($"[CombatEncounter.cs] {combatant.Name} turn finished");
					turnState = CombatTurnState.Begin;
					chosenAction = null;
					chosenItem = null;
					chosenTarget = null;

					do {
						currentCombatantIdx++;

						if ( currentCombatantIdx == teams[currentTeamIdx].GetMembers(Mortality.Any).Length ) {
							currentCombatantIdx = 0;
							currentTeamIdx++;
							if ( currentTeamIdx == teams.Count ) {
								currentTeamIdx = 0;
							}
						}

					} while ( teams[currentTeamIdx].GetMembers(Mortality.Any)[currentCombatantIdx].Mortality != Mortality.Alive );
				}
			}
		}

	}
}
