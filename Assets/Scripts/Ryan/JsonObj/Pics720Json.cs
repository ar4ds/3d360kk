using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Pics720Json
{
    public List<PicturePair> pics = new List<PicturePair>();
    public Pics720Json(List<PicturePair> pics){
        this.pics = pics;
    }
}
[Serializable]
public class PicturePair
{
    public string icon;
    public string large;

    public PicturePair(string icon, string large)
    {
        this.icon = icon;
        this.large = large;
    }
}
