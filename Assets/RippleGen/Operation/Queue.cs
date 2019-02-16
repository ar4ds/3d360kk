using UnityEngine;
using System.Collections.Generic;
using System;

namespace RippleGen.Operation {
	public class Queue : RippleGen.Utils.Single<Queue> {
		private bool action = false;
		private List<Entry> entries = new List<Entry>();

		private static List<Action> methodsQueue = new List<Action>();
		
		public virtual void Update () {
            lock(methodsQueue) {
                int length = methodsQueue.Count;
                Action[] actions = new Action[length];
                methodsQueue.CopyTo(actions);
                methodsQueue.Clear();
                foreach(Action method in actions) {
                    if (method != null)
                        method();
                }
                entryAdded.Clear();
            }
		}

		// <summary>加入一个任务，可以使用队列方式或是并发方式。</summary>
		public virtual void Add (Entry entry) {Add (entry, true);}
		public virtual void Add (Entry entry, bool queue) {
            entry.Queue = this;
			if (queue) {
				AddToQueue (entry);
			} else {
				AddWithComplicating (entry);
			}
		}

        public virtual void Remove(Entry entry) {
            if (entries.Contains(entry)) {
                entries.Remove(entry);
            }
        }
        public virtual void Clear() {
            lock(methodsQueue) {
                methodsQueue.Clear();
            }
        }

		public virtual void AddToQueue(Entry entry) {
			entries.Add (entry);
			checkQueue ();
        }

        List<Entry> entryAdded = new List<Entry>();
        private void checkQueue() {
            if (entries.Count > 0 && !action) {
				action = true;
				Entry entry = entries[0];
				entry.onTreadComplete.Add (() => {
					entries.Remove(entry);
					action = false;
                    
                    lock(methodsQueue) {
    					methodsQueue.Add (() =>{
    						// 到主线程中调用结束方法。
                            entry.Complete();
                            checkQueue();
    					});
                    }
                });

                float progress;
                entry.onTreadProgress.Add ((en, p) => {
                    progress = p;
                    if (!entryAdded.Contains(en)) {
                        entryAdded.Add(en);
                        
                        lock(methodsQueue) {
                            methodsQueue.Add (() =>{
                                // 到主线程中调用结束方法。
                                entry.Progress(progress);
                            });
                        }
                    }
                });
				entry.Start ();
			}
		}

		public virtual void AddWithComplicating(Entry entry) {
            entry.onTreadComplete.Add (() => {
                lock(methodsQueue) {
    				methodsQueue.Add (() =>{
    					// 到主线程中调用结束方法。
    					entry.Complete();
    				});
                }
            });
            float progress;
            entry.onTreadProgress.Add ((en, p) => {
                progress = p;
                if (!entryAdded.Contains(en)) {
                    entryAdded.Add(en);
                    
                    lock(methodsQueue) {
                        methodsQueue.Add (() =>{
                            // 到主线程中调用结束方法。
                            entry.Progress(progress);
                        });
                    }
                }
            });
			entry.Start ();

		}
	}
}
