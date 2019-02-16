using System;
using System.Collections.Generic;

using RippleGen.Extends;

namespace RippleGen.Operation
{
	public class Entry
	{
		// Attributes
		public bool live = true;

		// Events
		private List<Action<Entry>> onComplete = new List<Action<Entry>>();
		public List<Action<Entry>> OnComplete{ get{ return onComplete; }}
		private List<Action<Entry, float>> onProgress = new List<Action<Entry, float>>();
        public List<Action<Entry, float>> OnProgress{ get{ return onProgress; }}
        internal List<Action<Entry, float>> onTreadProgress = new List<Action<Entry, float>>();
		internal List<Action> onTreadComplete = new List<Action>();
        public Queue Queue;

		// Intializers
		public Entry () {}

		// Need Overwrite
		// Main thread
		// <summary>中断任务，如果任务是或着的就调用abort.</summary>
		public virtual void Cancel() {if (live) abort ();}
        internal protected virtual void Start() {}
        internal protected virtual void Complete() {live = false;invoke (onComplete);}
        internal protected virtual void Progress(float progress) { invoke<float> (OnProgress, progress); }

		// Other thread
		// <summary>线程结束的回调.</summary>
        protected virtual void threadComplete() {invoke (onTreadComplete);}
        protected virtual void threadProgress(float progress) { invoke<float> (onTreadProgress, progress); }

		// Private methods
		private void abort() {
            Queue.Remove(this);
			onComplete.Clear ();
            onProgress.Clear ();
            threadComplete();
			onTreadComplete.Clear ();
			live = false;
            Cancel ();
		}
		protected void invoke(List<Action> callbacks) {
			foreach (Action callback in callbacks) {
				callback ();
			}
		}
		protected void invoke(List<Action<Entry>> callbacks) {
			foreach (Action<Entry> callback in callbacks) {
				callback (this);
			}
		}
		protected void invoke<T>(List<Action<Entry, T>> callbacks, T value){
			foreach (Action<Entry, T> callback in callbacks) {
				callback (this, value);
			}
		}
	}
}

