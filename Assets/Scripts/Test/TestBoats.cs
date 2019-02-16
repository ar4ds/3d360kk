using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestBoats : MonoBehaviour
{
	void Start()
	{
		object obj = 1;
		List<object> objList = new List<object>();
		objList.Add(new List<bool> { true, true, true, false });
		objList.Add("Hello World!");
		objList.Add(123);
	}

	interface Nose
	{
		int Ear();
		string Face { get; }
	}

	public abstract class Picasso : Nose
	{
		public virtual int Ear()
		{
			return 7;
		}
		public Picasso(string face)
		{
			this.face = face;
		}
		string face;
		public virtual string Face
		{
			get { return face; }
		}
	}

	public class Clowns : Picasso
	{
		public Clowns() : base("Clowns") { }
	}

	public class Acts : Picasso
	{
		public Acts() : base("Acts") { }
		public override int Ear()
		{
			return 5;
		}
	}

	public class Of76 : Clowns
	{
		public override string Face
		{
			get
			{
				return "Of76";
			}
		}
	}
}
