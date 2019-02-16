using UnityEngine;

using System.IO;
using System.Collections;
using System.Collections.Generic;
using RippleGen.Plugins;

using SimpleJSON;
using System.Text.RegularExpressions;
using System;

namespace RippleGen.Utils
{
    public class Loc {
        public const string OnChangeMessage = "LocalizationOnChange";

        static Loc instance;
        public static Loc Instance {
            get {
                if (instance == null) {
                    instance = new Loc();
                }
                return instance;
            }
        }

        const string K_Localization = "Localization";
        string language = "zh";
        public string Language {
            get {
                return language;
            }
            set {
                if (language != value) {
                    language = value;
                    PlayerPrefs.SetString(K_Localization, value);
                    MessageCenter.Instance.Trigger(OnChangeMessage);
                }
            }
        }
        JSONNode localizationSource;
        Dictionary<string, Dictionary<string, string>> localizationAppended = new Dictionary<string, Dictionary<string, string>>();

        public Loc() {
            if (PlayerPrefs.HasKey(K_Localization)) {
                Language = PlayerPrefs.GetString(K_Localization);
            }

            string path = Application.persistentDataPath + "/" + K_Localization + ".json";
            if (File.Exists(path)) {
                localizationSource = JSON.Parse(File.ReadAllText(path));
            }else {
                localizationSource = JSON.Parse(Resources.Load<TextAsset>("Localization").text);
            }
        }

        public string Get_(string key, string[] attrs) {
            string value = null;
            if (localizationAppended.ContainsKey(Language)) {
                Dictionary<string, string> dic = localizationAppended[Language];
                if (dic.ContainsKey(key)) {
                    value = dic[key];
                }
            }
            if (value == null) {
                JSONNode content = localizationSource[Language];
                if (content == null) 
                    content = localizationSource["en"];
                
                JSONNode tar = content[key];
                value = tar == null ? key : tar.Value;
            }
            value = value == null ? key : value;
            
            Regex regex = new Regex("{{\\d+}}");
            MatchCollection macths = regex.Matches(value);
            
            for (int n = 0, t = macths.Count; n < t; n++) {
                Regex r2 = new Regex("(?<={{)\\d+(?=}})");
                int num = Convert.ToInt32(r2.Match(macths[n].Value));
                if (num < attrs.Length) {
                    value = value.Replace(macths[n].Value, attrs[num]);
                }
            }
            
            return value;
        }

        public void Save() {
            var enu = localizationAppended.GetEnumerator();
            while (enu.MoveNext()) {
                JSONNode contents = localizationSource[enu.Current.Key];
                bool new_node = false;
                if (contents == null) {
                    contents = JSONNode.Parse("{}");
                    new_node = true;
                }
                var enu2 = enu.Current.Value.GetEnumerator();
                while (enu2.MoveNext()) {
                    contents.Remove(enu2.Current.Key);
                    contents.Add(enu2.Current.Key, new JSONString(enu2.Current.Value));
                }

                if (new_node) {
                    localizationSource.Add(enu.Current.Key, contents);
                }
            }
            localizationAppended.Clear();

            string path = Application.persistentDataPath + "/" + K_Localization + ".json";
            if (File.Exists(path)) {
                File.Delete(path);
            }
            File.WriteAllText(path, localizationSource.ToString());
        }

        public void Insert(string language, Dictionary<string, string> contents) {
            Dictionary<string, string> ls;
            if (localizationAppended.ContainsKey(language)) {
                ls = localizationAppended[language];
            }else {
                ls = new Dictionary<string, string>();
                localizationAppended.Add(language, ls);
            }
            Dictionary<string, string>.Enumerator enu = contents.GetEnumerator();
            while (enu.MoveNext()) {
                if (ls.ContainsKey(enu.Current.Key)) {
                    ls[enu.Current.Key] = enu.Current.Value;
                }else {
                    ls.Add(enu.Current.Key, enu.Current.Value);
                }
            }
        }

        public void Insert(string language, string key, string content) {
            Dictionary<string, string> ls;
            if (localizationAppended.ContainsKey(language)) {
                ls = localizationAppended[language];
            }else {
                ls = new Dictionary<string, string>();
                localizationAppended.Add(language, ls);
            }

            if (ls.ContainsKey(key)) {
                ls[key] = content;
            }else {
                ls.Add(key, content);
            }

        }

        public string Get(string key, params string[] attrs) {
            return Get_(key, attrs);
        }

        public static string g(string key, params string[] attrs) {
            return Instance.Get_(key, attrs);
        }

		public static void Initialize() {
			Loc l = Loc.Instance;
		}
    }
    
    public static class StringExtend {
        public static string g(this string key, params string[] attrs) {
            return Loc.g(key, attrs);
        }
    }
}
