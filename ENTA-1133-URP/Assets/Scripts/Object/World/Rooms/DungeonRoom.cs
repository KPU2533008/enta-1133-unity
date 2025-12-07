using DungeonGame;
using DungeonGame.Object;
using System.Collections;
using UnityEngine;

public class DungeonRoom : MonoBehaviour {

	[SerializeField] private GameObject NorthDoorway, EastDoorway, SouthDoorway, WestDoorway;
	private DungeonRoom _north, _east, _south, _west;
	protected bool isVisited = false;

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

	public virtual string GetRoomDescription() {
		return "Room";
	}

	public virtual void OnEntered(PlayerController player) {
		isVisited = true;
	}

	public virtual void OnSearched(PlayerController player) { }

	public virtual void OnExited(PlayerController player) { }

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start() {

	}

	// Update is called once per frame
	void Update() {

	}
}
