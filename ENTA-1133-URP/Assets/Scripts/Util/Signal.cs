using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DungeonGame.Util {
	public class Connection {

		private Action disconnectHandler;
		public bool Connected { get; private set; } = true;

		public Connection(Action disconnectHandler) {
			this.disconnectHandler = disconnectHandler;
		}

		public void Disconnect() {
			if ( Connected ) {
				Connected = false;
				disconnectHandler();
			}
		}

	}

	public class Signal<T> {
		public delegate void SignalEventHandler(T args);
		private event SignalEventHandler InternalEvent;

		public void Fire(T args) {
			InternalEvent?.Invoke(args);
		}

		public Connection Connect(SignalEventHandler subscriber) {
			InternalEvent += subscriber;
			Connection conn = null;

			conn = new(() => {
				InternalEvent -= subscriber;
			});

			return conn;
		}

		public Connection Once(SignalEventHandler subscriber) {
			Connection conn = null;
			conn = Connect((args) => {
				subscriber(args);
				conn?.Disconnect();
			});
			return conn;
		}

		public void Wait() {
			bool yielding = true;

			Once((args) => {
				yielding = false;
			});

			while ( yielding ) {
				Thread.Sleep(1);
			}
		}

		public void DisconnectAll() {
			foreach ( SignalEventHandler connection in InternalEvent.GetInvocationList().Cast<SignalEventHandler>() ) {
				InternalEvent -= connection;
			}
		}

		public void Destroy() {
			DisconnectAll();
		}

	}
}
