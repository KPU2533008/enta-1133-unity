using System.Collections.Generic;

public class Lock {

	private Dictionary<string, bool> locks = new();

	public bool IsLocked() {
		return locks.Count > 0;
	}

	//public bool Lock(string key, bool isLocked) {

	//	return IsLocked();
	//}

}