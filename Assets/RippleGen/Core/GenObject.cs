using System.Collections;
using System.Reflection;
using UnityEngine;

namespace RippleGen.Core
{

		// 功能是实现一个动态类
		public delegate object Run (params object[] par);
		public class GenObject
		{
				// 动态成员
				private Hashtable _dynamicMenbers = new Hashtable ();

				public virtual object this [string key] {
						get {
								if (_dynamicMenbers [key] == null) {
										PropertyInfo property = this.GetType ().GetProperty (key);
										if (property != null)
												return property.GetValue (this, null);
										MethodInfo method = this.GetType ().GetMethod (key);
										if (method != null) {
												Run run = (par) => {
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
