using System.Collections;


namespace RippleGen.Utils {
	public class Coder {

		public static string EncodeMd5(string input) {
            input = input == null ? "" : input;
			System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
			byte[] hashBytes = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));

			string hashString = "";
			for (int i = 0; i < hashBytes.Length; i++)
			{
				hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
			}
			return hashString.PadLeft(32, '0');
		}

        public static Hashtable DecodeConfig(string config) {
            string[] lines = config.Split ('\n');
            Hashtable table = new Hashtable();

            foreach (string line in lines) {
                if (line.IndexOf('#') == 0) continue;
                string[] arr = line.Split ('=');
                if (arr.Length < 2)
                    continue;
                string key = arr[0].Trim ();

                table.Add (key, arr[1]);
            }

            return table;
        } 
	}
}
