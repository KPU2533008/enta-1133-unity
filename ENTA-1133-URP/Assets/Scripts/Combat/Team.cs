using DungeonGame.Enum;
using System.Collections.Generic;
using System.Linq;

namespace DungeonGame.Combat {
	public class Team {

		private List<Combatant> teamMembers;
		private Dictionary<Team, Allegiance> allegiances;
		public bool IsDefeated => !teamMembers.Any(member => member.IsAlive);

		public Team(List<Combatant> members) {
			teamMembers = members;
			allegiances = new() { { this, Allegiance.Friendly } };
			foreach ( Combatant member in members ) {
				member.SetTeam(this);
			}
		}

		public bool IsMember(Combatant combatant) {
			return teamMembers.Contains(combatant);
		}

		public Combatant[] GetMembers(Mortality mortality) {
			return ( from member in teamMembers where ( mortality.HasFlag(member.Mortality) ) select member ).ToArray();
		}

		public bool AddMember(Combatant combatant) {
			if ( teamMembers.Contains(combatant) ) {
				return false;
			}
			teamMembers.Add(combatant);
			combatant.SetTeam(this);
			return true;
		}

		public bool RemoveMember(Combatant combatant) {
			int indexOf = teamMembers.IndexOf(combatant);

			if ( indexOf == -1 ) {
				return false;
			}

			teamMembers.RemoveAt(indexOf);

			if ( combatant.Team == this ) {
				combatant.SetTeam(null);
			}

			return true;
		}

		public void ResetAllegiances() {
			allegiances = new() { { this, Allegiance.Friendly } };
		}

		public void SetTeamAllegiance(Team team, Allegiance allegiance) {
			if ( team == this )
				return;
			allegiances[team] = allegiance;
		}

		public Team[] GetAllegiantTeams(Allegiance allegiance) {
			return ( from kvp in allegiances where ( kvp.Value == allegiance ) select kvp.Key ).ToArray();
		}

		public Combatant[] GetAllegiantMembers(Allegiance allegiance, Mortality mortality) {
			Combatant[] combatants = { };

			foreach ( Team team in GetAllegiantTeams(allegiance) ) {
				combatants = combatants.Concat(team.GetMembers(mortality)).ToArray();
			}

			return combatants;
		}

	}
}
