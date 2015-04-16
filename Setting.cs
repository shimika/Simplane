using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Simplane {
	class Setting {
		static string SettingPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Simplane.txt";
		public static string SavePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Simplane\";

		public static string Server = "";
		public static string Token = "";

		public static bool Load() {
			if (!File.Exists(SettingPath)) {
				Save();
				return false;
			}

			try {
				using (StreamReader sr = new StreamReader(SettingPath)) {
					string text = sr.ReadToEnd();

					JsonTextParser parser = new JsonTextParser();
					JsonObjectCollection collect = (JsonObjectCollection)parser.Parse(text);

					Server = collect["Server"].GetValue().ToString();
					Token = collect["Token"].GetValue().ToString();
				}
			} catch (Exception ex) {
				//MessageBox.Show(ex.Message);
			}
			return true;
		}

		public static void Save() {
			JsonObjectCollection root = new JsonObjectCollection();
			root.Add(new JsonStringValue("Server", Server));
			root.Add(new JsonStringValue("Token", Token));

			using (StreamWriter sw = new StreamWriter(SettingPath)) {
				sw.Write(root);
			}
		}
	}
}
