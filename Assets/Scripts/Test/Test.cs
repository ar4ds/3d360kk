using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RippleGen.Utils;
using System.Runtime.Serialization.Formatters.Binary;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        string url = "http://www.3d360kk.com/images/2/1.jpg";
        string tmpMd5 = Coder.EncodeMd5 (url);
        Debug.LogError("md5="+tmpMd5);
        
        int tmpHashCode = url.GetHashCode();
        Debug.LogError("HashCode="+tmpHashCode);
        Debug.LogError("string=");

        Debug.LogError("done.");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
