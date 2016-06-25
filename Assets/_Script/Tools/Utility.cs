namespace TentacleGuitarUnity {
using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Text;
using JsonOrg;


public class FileUtility {

	

	public static string ReadText (string path) {
		try {
			StreamReader sr = File.OpenText(path);
			string data = sr.ReadToEnd();
			sr.Close();
			return data;
		} catch (Exception) {
			return "";
		}
	}

	public static void WriteText (string data, string path) {
		try {
			FileStream fs = new FileStream(path, FileMode.Create);
			StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
			sw.Write(data);
			sw.Close();
			fs.Close();
		} catch (Exception) {
			return;
		}
	}



	public static JsonObject ReadJsonObject (string path) {
		try {
			return new JsonObject(ReadText(path));
		} catch (System.Exception) {
			return null;
		}
	}


	public static JsonArray ReadJsonArray (string path) {
		try {
			return new JsonArray(ReadText(path));
		} catch (System.Exception) {
			return null;
		}
	}



	public static void WriteJson (JsonObject jo, string path) {
		WriteText(JsonHelper.FormatJson(jo.ToString()), path);
	}

	public static void WriteJson (JsonArray jo, string path) {
		WriteText(JsonHelper.FormatJson(jo.ToString()), path);
	}



	public static byte[] FileToByte (string path) {
		if (File.Exists(path)) {
			byte[] bytes = null;
			try {
				bytes = File.ReadAllBytes(path);
			} catch (System.Exception) {
				return null;
			}
			return bytes;
		} else {
			return null;
		}
	}

	public static bool ByteToFile (byte[] bytes, string path) {
		try {
			string parentPath = new FileInfo(path).Directory.FullName;
			CreateFolder(parentPath);
			FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
			fs.Write(bytes, 0, bytes.Length);
			fs.Close();
			return true;
		} catch (System.Exception) {
			return false;
		}
	}



	public static void CreateFolder (string path) {
		if (Directory.Exists(path))
			return;
		string parentPath = new FileInfo(path).Directory.FullName;
		if (Directory.Exists(parentPath)) {
			Directory.CreateDirectory(path);
		} else {
			CreateFolder(parentPath);
			Directory.CreateDirectory(path);
		}
	}


	public static string FixPath (string path) {
		string fixedPath = path.Replace(@"\", @"/");
		fixedPath = fixedPath.Replace(@"//", @"/");
		return fixedPath;
	}


	public static string GetFileURL (string path) {
		return (new System.Uri(path)).AbsoluteUri;
	}


}
}