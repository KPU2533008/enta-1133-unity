using DungeonGame.Util;
using System.Collections;
using UnityEngine;

public static class SignalExtensions {
	public static IEnumerator WaitForSignal<T>(Signal<T> signal, System.Action<T> onReceived) {
		bool done = false;

		Signal<T>.SignalEventHandler handler = null;
		handler = args => {
			onReceived?.Invoke(args);
			done = true;
		};

		var conn = signal.Once(handler);

		yield return new WaitUntil(() => done);

		conn.Disconnect();
	}
}