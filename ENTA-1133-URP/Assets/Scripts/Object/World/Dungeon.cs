using DungeonGame;
using UnityEngine;

public class Dungeon : MonoBehaviour {

	[SerializeField] private DungeonRoomWeightTable Rooms;
	[SerializeField] private int RoomSize = 7;
	[SerializeField] private int DungeonSize = 5;

	private DungeonRoom[,] dungeonMap;

	public DungeonRoom GetRoomAt(int x, int y) {
		if ( x < 0 || x > dungeonMap.GetLength(0) - 1 || y < 0 || y > dungeonMap.GetLength(1) - 1 )
			return null;
		return dungeonMap[x, y];
	}

	private void LinkRoom(DungeonRoom room, int roomX, int roomY) {
		Vector2 north = new(roomX, roomY + 1);
		Vector2 east = new(roomX + 1, roomY);
		Vector2 south = new(roomX, roomY - 1);
		Vector2 west = new(roomX - 1, roomY);

		DungeonRoom northRoom = GetRoomAt((int)north.y, (int)north.x);
		DungeonRoom eastRoom = GetRoomAt((int)east.y, (int)east.x);
		DungeonRoom southRoom = GetRoomAt((int)south.y, (int)south.x);
		DungeonRoom westRoom = GetRoomAt((int)west.y, (int)west.x);

		room.SetRooms(northRoom, eastRoom, southRoom, westRoom);
	}

	public void GenerateMap() {
		if ( dungeonMap != null )
			DestroyMap();

		dungeonMap = new DungeonRoom[DungeonSize, DungeonSize];
		int combatRooms = 0;

		for ( int z = 0; z < DungeonSize; z++ ) {
			for ( int x = 0; x < DungeonSize; x++ ) {
				Vector3 roomPos = new(x * RoomSize, 0, z * RoomSize);
				DungeonRoom room = Rooms.Roll();
				var roomInstance = Instantiate(room, transform);
				roomInstance.transform.position = roomPos;
				roomInstance.name += $"[{z},{x}]";
				dungeonMap[z, x] = roomInstance;
				if ( room is CombatRoom ) {
					combatRooms++;
				}
			}
		}

		Game.CombatRooms = combatRooms;
		Game.ClearedCombatRooms = 0;

		/*
		for ( int i = 0; i < DungeonSize * 1.5; i++ ) {
			int x = Random.Range(0, DungeonSize - 1);
			int z = Random.Range(0, DungeonSize - 1);

			if ( dungeonMap[x, z] != null ) {
				Destroy(dungeonMap[x, z].gameObject);
				dungeonMap[x, z] = null;
			}
		}
		*/

		for ( int z = 0; z < dungeonMap.GetLength(1); z++ ) {
			for ( int x = 0; x < dungeonMap.GetLength(0); x++ ) {
				DungeonRoom currentRoom = dungeonMap[z, x];

				if ( currentRoom == null )
					continue;

				LinkRoom(currentRoom, x, z);
			}
		}
	}

	public void DestroyMap() {
		for ( int x = 0; x < dungeonMap.GetLength(0); x++ ) {
			for ( int z = 0; z < dungeonMap.GetLength(1); z++ ) {
				DungeonRoom currentRoom = dungeonMap[x, z];
				Destroy(currentRoom);
			}
		}
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start() {

	}

	// Update is called once per frame
	void Update() {

	}
}
