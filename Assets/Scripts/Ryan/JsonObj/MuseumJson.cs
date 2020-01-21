using System;
[Serializable]
public class MuseumJson
{
    public string id;
    public string name;
    public string city;
    public string museum;
    public string guid;

    public MuseumJson(string id, string name, string city, string museum, string guid)
    {
        this.id = id;
        this.name = name;
        this.city = city;
        this.museum = museum;
        this.guid = guid;
    }
}
