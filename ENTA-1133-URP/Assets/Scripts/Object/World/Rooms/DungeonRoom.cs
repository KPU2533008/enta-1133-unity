using UnityEngine;

public class DungeonRoom : MonoBehaviour {

	[SerializeField] private GameObject NorthDoorway, EastDoorway, SouthDoorway, WestDoorway;
	private DungeonRoom _north, _east, _south, _west;
	private bool isVisited = false;

	public void SetRooms(DungeonRoom north, DungeonRoom east, DungeonRoom south, DungeonRoom west) {
		_north = north;
		_east = east;
		_south = south;
		_west = west;

		NorthDoorway.SetActive(_north == null);
		EastDoorway.SetActive(_east == null);
		SouthDoorway.SetActive(_south == null);
		WestDoorway.SetActive(_west == null);
	}

	public virtual void OnEntered(PlayerController player) {
		Debug.Log($"You find yourself in a room with a layout that's {( isVisited ? "" : "un" )}familiar to you. You've {( isVisited ? "" : "not " )}seen this place before...");
		isVisited = true;
	}

	public virtual void OnSearched(PlayerController player) {
		Debug.Log("You scour the room thoroughly, hoping to find something useful...");
	}

	public virtual void OnExited(PlayerController player) {
		Debug.Log("You leave the room, content with what you've seen, and press onward.");
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start() {

	}

	// Update is called once per frame
	void Update() {

	}
}
