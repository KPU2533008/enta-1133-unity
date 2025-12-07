using UnityEngine;

namespace DungeonGame.Abstract {
	public class Entity : MonoBehaviour {
		[field: SerializeField] public string Name { get; private set; } = "Entity";
		public virtual void Destroy() { }
	}
}
