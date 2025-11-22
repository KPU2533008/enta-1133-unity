using DungeonGame.Enum;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {

	[SerializeField]
	private Camera _camera;
	private const int LOOK_MULT = 12;
	private float lookUpDown = 0;

	private float WalkSpeed = 2.5f;
	private Vector2 MoveVector = new();
	private Vector3 MoveDirection = new();

	private DungeonRoom _currentRoom;
	private Rigidbody rigidBody;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	public void Setup() { }

	void OnMove(InputValue value) {
		Vector2 moveVector = value.Get<Vector2>();
		MoveVector = moveVector;
	}

	void OnLook(InputValue value) {
		if ( Cursor.lockState == CursorLockMode.None )
			return;

		Vector2 lookVector = value.Get<Vector2>();
		transform.rotation *= Quaternion.Euler(0, lookVector.x * Time.deltaTime * LOOK_MULT, 0);

		lookUpDown = Math.Clamp(lookUpDown - ( lookVector.y * Time.deltaTime * LOOK_MULT ), -80, 75);
		_camera.transform.rotation = transform.rotation * Quaternion.Euler(lookUpDown, 0, 0);
	}

	void OnInteract(InputValue value) {
		if ( _currentRoom != null ) {
			_currentRoom.OnSearched(this);
		}
	}

	private void Start() {
		rigidBody = GetComponent<Rigidbody>();
		rigidBody.maxLinearVelocity = WalkSpeed;
		rigidBody.useGravity = false;
		rigidBody.collisionDetectionMode = CollisionDetectionMode.Discrete;
		rigidBody.detectCollisions = true;
	}

	private void FixedUpdate() {
		rigidBody.angularVelocity = Vector3.zero;
		rigidBody.linearVelocity = MoveDirection * WalkSpeed;
	}

	// Update is called once per frame
	void Update() {
		//Cursor.lockState = CursorLockMode.Locked;
		MoveDirection = ( transform.rotation * Vector3.forward * MoveVector.y ) + ( transform.rotation * Vector3.right * MoveVector.x );
		if ( Input.GetKeyDown(KeyCode.Tab) ) {
			Cursor.lockState = Cursor.lockState == CursorLockMode.None ? CursorLockMode.Locked : CursorLockMode.None;
		}
	}

	private void OnTriggerEnter(Collider other) {
		DungeonRoom room = other.GetComponent<DungeonRoom>();
		if ( room != null ) {
			if ( _currentRoom != null ) {
				_currentRoom.OnExited(this);
			}
			_currentRoom = room;
			room.OnEntered(this);
		}
	}

	private void OnTriggerExit(Collider other) {
		DungeonRoom room = other.GetComponent<DungeonRoom>();
		if ( room == _currentRoom ) {
			room.OnExited(this);
		}
	}
}
