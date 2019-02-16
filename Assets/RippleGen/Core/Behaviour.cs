// 这个Behaviour等同于MVC中的一个Controller
// 每一个场景中只能存在一个
// 主要负责这个场景的业务逻辑
// 
// 其中主要实现了对Plugin以及Operation的支持

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using RippleGen.Extends;
using RippleGen.Core;
using RippleGen.Operation;

namespace RippleGen.Core
{
		public class MonoBehaviour : UnityEngine.MonoBehaviour
		{

				public static MonoBehaviour CurrentBehoviour;
				public RippleGen.Operation.Queue queue = new RippleGen.Operation.Queue ();

				private List<Plugin> plugins = new List<Plugin> ();
				private List<Action> soonDos = new List<Action> ();

				public GameObject NGUIPanel;

				protected virtual void Start ()
				{
						Application.targetFrameRate = 60;
						CurrentBehoviour = this;
						foreach (Plugin plugin in plugins) {
								plugin.Start ();
						}
						if (NGUIPanel == null) {
								NGUIPanel = GameObject.Find ("NGUI");
						}
				}

				protected virtual void Update ()
				{
						RippleGen.Operation.Queue.Instance.Update ();
						queue.Update ();
						foreach (Plugin plugin in plugins) {
								plugin.Update ();
						}
						if (soonDos.Count > 0) {
								soonDos.InvokeActions ();
								soonDos.Clear ();
						}
				}

				protected virtual void OnDestroy ()
				{
				}

				protected void addOperation (Entry entry)
				{
						queue.Add (entry);
				}

				protected void addPlugin (Plugin plugin)
				{
						if (plugin.Target) {
								plugin.Target.removePlugin (plugin);
						}
						plugins.Add (plugin);
						plugin.Target = this;
				}

				protected void removePlugin (Plugin plugin)
				{
						plugins.Remove (plugin);
				}

				protected void invokeSoon (Action active)
				{
						soonDos.Add (active);
				}

				// about UI
				protected virtual void OnGUI ()
				{
						foreach (Plugin plugin in plugins) {
								plugin.OnGUI ();
						}
				}

				// 对于动态变量的支持
				private Hashtable _dynamicMenbers = new Hashtable ();

				public object this [string key] {
						get {
								if (_dynamicMenbers [key] == null) {
										PropertyInfo property = this.GetType ().GetProperty (key);
										if (property != null)
												return property.GetValue (this, null);
										MethodInfo method = this.GetType ().GetMethod (key);
										if (method != null) {
												RippleGen.Core.Run run = (par) => {
														return method.Invoke (this, par);
												};
												return run;
										}
										return null;
								} else {
										return _dynamicMenbers [key];
								}
						}
						set { 
								PropertyInfo property = this.GetType ().GetProperty (key);
								if (property != null) {
										property.SetValue (this, value, null);
								} else {
										_dynamicMenbers [key] = value;
								}
						}
				}
		}
}
