using System;

namespace DungeonGame.Enum {
	[Flags]
	public enum Mortality {
		Alive = 1,
		Dead = 2,
		Any = 3,
	}
}
