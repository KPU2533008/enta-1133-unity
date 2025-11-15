using System;

namespace DungeonGame.Enum {
	[Flags]
	internal enum Mortality {
		Alive = 1,
		Dead = 2,
		Any = 3,
	}
}
