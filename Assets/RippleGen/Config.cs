using System.IO;
using SimpleJSON;
using System.Collections;
using UnityEngine;

namespace RippleGen
{

		public class Config : RippleGen.Core.Plugin
		{
				public readonly string CONFIG_PATH = Application.persistentDataPath + "/Config";

				public static bool IsNewVerion{ get { return Instance.isNewVerion; } }

				private static Config instance;

				public static Config Instance { 
						get { 
								if (instance == null) {
										instance = new Config ();
								}
								return instance;
						}
				}

				private bool isNewVerion = false;
				private JSONNode config;
				private bool valueChanged = false;

				public float CurrentVersion;

				Config ()
				{
						this.config = configFromFile ();
						JSONNode node = currentConfig ();
						CurrentVersion = node ["version"].AsFloat;
						if (config == null || (node != null && config ["version"].AsFloat < node ["version"].AsFloat)) {
								isNewVerion = true;
								this.config = node;
								if (this.config != null)
										valueChanged = true;
								return;
						}
				}

				public new JSONNode this [string key] {
						get { 
								if (this.config != null) {
										return this.config [key];
								} else {
										return null;
								}
						}
						set { 
								if (this.config != null) {
										this.config [key] = value;
										valueChanged = true;
								} 
						}
				}

				// 更新控制文件
				public bool UpdateConfig (string n_config)
				{
						JSONNode node = _parser (n_config);
						if (config == null || (node != null && config ["version"].AsFloat < node ["version"].AsFloat)) {
								this.config = node;
								valueChanged = true;
								return true;
						}
						return false;
				}

				// 预置控制文件
				JSONNode configFromFile ()
				{
						if (File.Exists (CONFIG_PATH)) {
								return JSON.Parse (File.ReadAllText (CONFIG_PATH.ToString ()));
						} else
								return null;
				}


				// 当前控制文件
				JSONNode currentConfig ()
				{
						TextAsset asset = Resources.Load<TextAsset> ("Config");
						if (asset != null) {
								return _parser (asset.text);
						} else
								return null;
				}

				JSONNode _parser (string str)
				{
						if (str != null) {
								string[] lines = str.Split ('\n');
								JSONNode newNode = JSONNode.Parse ("{}");
								foreach (string line in lines) {
										if (line.IndexOf ('#') == 0)
												continue;
										string[] arr = line.Split ('=');
										if (arr.Length < 2)
												continue;
										string key = arr [0].Trim ();
										// JSONNode value = new JSONData (arr [1].Trim ());
										JSONNode value = new JSONString (arr [1].Trim ());
										newNode.Add (key, value);
								}
								return newNode;
						} else
								return null;
				}

				public override void Update ()
				{
						if (valueChanged && this.config != null) {
								File.WriteAllText (CONFIG_PATH, config.ToString ());
								valueChanged = false;
						}
				}
		}
}