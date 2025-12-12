using DungeonGame;
using DungeonGame.Combat;
using DungeonGame.Item;
using DungeonGame.Object;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MonoDungeonGameRunner : MonoBehaviour {

	[SerializeField] private Dungeon dungeonMapPrefab;
	[SerializeField] private PlayerController playerControllerPrefab;
	[SerializeField] private AItem[] starterItems;

	private Dungeon _dungeon;
	private PlayerController _playerController;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start() {
		StartGame();
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
		foreach ( var item in starterItems ) {
			StartCoroutine(_playerController.GiveItem(Instantiate(item)));
		}
	}

	private void RunGame() {
		//var startRoom = _dungeon._rooms;
	}

	// Update is called once per frame
	public void Update() { }
}
