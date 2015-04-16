using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplane {
	class Log {
		public static string LogFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Shimika\";
		public static string Project = "Simplane";

		public static void WriteLog(string msg) {
			if (!Directory.Exists(LogFolder)) {
				Directory.CreateDirectory(LogFolder);
			}

			string path = String.Format("{0}.log", Path.Combine(LogFolder, Project));

			using (StreamWriter sw = new StreamWriter(path, true)) {
				sw.WriteLine("{0}: {1}", DateTime.Now, msg);
			}
		}
	}
}
