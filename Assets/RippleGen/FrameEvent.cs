using System;
using System.Collections.Generic;
using RippleGen.Extends;
using UnityEngine;

namespace RippleGen
{
    public class FrameEvent : RippleGen.Core.Plugin
    {
        private static FrameEvent instance;
        public static FrameEvent Instance{
            get { 
                if (instance == null)
                {
                    instance = new FrameEvent();
                }
                return instance;
            }
        }

        private List<Action> onFrame = new List<Action>();
        private List<Action> needRemove = new List<Action>();

        public override void Update() {
            if (needRemove.Count > 0)
            {
                foreach (Action action in needRemove)
                {
                    onFrame.Remove(action);
                }
                needRemove.Clear();
            }
            onFrame.InvokeActions();
        }

        public virtual void Add(Action action) {
            onFrame.Add(action);
        }

        public virtual void Remove(Action action) {
            needRemove.Add(action);
        }

        public virtual void Clear() {
            onFrame.Clear();
        }
    }
    public static class ObjectExtend {
        // 下一帧的时候执行一次
        public static void Do(this object _, Action action) {
            Action act = () =>
            {
				action();
				//FrameEvent.Instance.Remove(act);
			};
            FrameEvent.Instance.Add(act);
        }
    }
}