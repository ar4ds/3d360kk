using System;
using System.Collections.Generic;

namespace RippleGen.Extends
{
    public static class ListExtentds
    {
        public static void InvokeActions(this List<Action> list)
        {
            Action[] actions = new Action[list.Count];
            list.CopyTo(actions);
            foreach (Action a in actions)
            {
                a();
            }
        }
        public static void InvokeActions<T>(this List<Action<T>> list, T obj) {
            Action<T>[] actions = new Action<T>[list.Count];
            list.CopyTo(actions);
            foreach (Action<T> a in actions)
            {
                a(obj);
            }
        }
    }

}

