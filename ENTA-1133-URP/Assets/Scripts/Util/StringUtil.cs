namespace DungeonGame.Util {
	public class StringUtil {

		private static bool IsUpper(string str) {
			return ( str.ToUpper() == str && str.ToLower() != str );
		}

		// This has some pretty glaring issues and undesirable behavior but it's good enough
		public static string AddSpaces(string input) {
			string str = input.Substring(0, 1);
			char[] chars = input.ToCharArray();

			for ( int i = 1; i < chars.Length - 1; i++ ) {
				string thisChar = chars[i].ToString();
				string nextChar = chars[i + 1].ToString();

				if ( IsUpper(thisChar) && !IsUpper(nextChar) ) {
					str += " " + thisChar.ToLower();
				} else {
					str += thisChar;
				}
			}

			str += input.Substring(input.Length - 1, 1);

			return str;
		}

	}
}
