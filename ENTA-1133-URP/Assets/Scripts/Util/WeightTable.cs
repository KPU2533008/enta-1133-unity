using System;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonGame.Util {
	public class WeightTable<T> : SerializableDictionary<T, int> {

		//[SerializeField]

		private int SumTableWeights() {
			int totalWeight = 0;
			foreach ( KeyValuePair<T, int> kvp in this ) {
				totalWeight += kvp.Value;
			}
			return totalWeight;
		}

		public T Roll() {
			System.Random rng = new();
			T? result = default(T);
			int remainingDistance = (int)( SumTableWeights() * rng.NextDouble() );

			foreach ( KeyValuePair<T, int> kvp in this ) {
				remainingDistance -= kvp.Value;
				if ( remainingDistance < 0 ) {
					result = kvp.Key;
				}
			}

			return (T)result;
		}

	}
}