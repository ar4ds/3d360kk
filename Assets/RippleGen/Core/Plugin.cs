// 插件对象，安装到Behaviour中的插件。

using System;

namespace RippleGen.Core
{
		public class Plugin : RippleGen.Core.GenObject
		{
				public virtual void Start ()
				{
				}

				public virtual void Update ()
				{
				}

				public virtual void OnGUI ()
				{
				}

				public RippleGen.Core.MonoBehaviour Target { set; get; }
		}
}

