using DungeonGame;
using DungeonGame.Combat;
using DungeonGame.Object;
using UnityEngine;

public class MonoDungeonGameRunner : MonoBehaviour {

	[SerializeField] private Dungeon dungeonMapPrefab;
	[SerializeField] private PlayerController playerControllerPrefab;

	private Dungeon _dungeon;
	private PlayerController _playerController;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start() {

	}

	public void StartGame() {
		transform.position = Vector3.zero;
		SetupMap();
		SpawnPlayer();
		RunGame();
	}

	private void SetupMap() {
		_dungeon = Instantiate(dungeonMapPrefab, transform);
		_dungeon.transform.position = Vector3.zero;
		_dungeon.GenerateMap();
		Game.Dungeon = _dungeon;
	}

	private void SpawnPlayer() {
		_playerController = Instantiate(playerControllerPrefab, transform);
		Team playerTeam = new(new() { _playerController });
		Game.Player = _playerController;
	}

	private void RunGame() {
		Debug.Log("Hello world");
		//var startRoom = _dungeon._rooms;
	}

	// Update is called once per frame
	public void Update() {

	}
}
