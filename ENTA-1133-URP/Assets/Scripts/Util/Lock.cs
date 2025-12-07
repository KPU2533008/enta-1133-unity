using DungeonGame.Util;
using System.Collections.Generic;

public class Lock {

	private Dictionary<string, bool> locks = new();
	public Signal<bool> Changed = new();
	public bool IsLocked => locks.Count > 0;

	public void Set(string key, bool isLocked) {
		bool wasLocked = IsLocked;

		if ( isLocked )
			locks[key] = isLocked;
		else
			locks.Remove(key);


		if ( isLocked != wasLocked ) {
			Changed.Fire(IsLocked);
		}
	}

}