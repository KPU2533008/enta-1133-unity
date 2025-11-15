namespace DungeonGame.Abstract {
	internal abstract class Entity {
		public string Name { get; protected set; } = "Entity";

		public virtual void Destroy() { }
	}
}
