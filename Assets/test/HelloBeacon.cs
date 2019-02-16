using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelloBeacon : MonoBehaviour
{
    public Button ActiveBtn;
    public Text BeaconDataText;
    BroadcastState bs_State;
    private List<Beacon> mybeacons = new List<Beacon>();

    void Start()
    {
        bs_State = BroadcastState.inactive;
        ActiveBtn.onClick.AddListener(btn_StartStop);
    }

    // BroadcastState
    public void btn_StartStop()
    {
        //Debug.Log("Button Start / Stop pressed");
        /*** Beacon will start ***/
        if (bs_State == BroadcastState.inactive)
        {
            #region ReceiveMode
            iBeaconReceiver.BeaconRangeChangedEvent += OnBeaconRangeChanged;
            iBeaconReceiver.regions = new iBeaconRegion[] { new iBeaconRegion("", new Beacon()) };
            // !!! Bluetooth has to be turned on !!! TODO
            iBeaconReceiver.Scan();
            Debug.Log("Listening for beacons");
            #endregion
            bs_State = BroadcastState.active;
        }
        else
        {
            iBeaconReceiver.Stop();
            iBeaconReceiver.BeaconRangeChangedEvent -= OnBeaconRangeChanged;
            removeFoundBeacons();
            bs_State = BroadcastState.inactive;
        }
        SetBroadcastState();
    }
    private void OnBeaconRangeChanged(Beacon[] beacons)
    { // 
        foreach (Beacon b in beacons)
        {
            var index = mybeacons.IndexOf(b);
            if (index == -1)
            {
                mybeacons.Add(b);
            }
            else
            {
                mybeacons[index] = b;
            }
        }
        for (int i = mybeacons.Count - 1; i >= 0; --i)
        {
            if (mybeacons[i].lastSeen.AddSeconds(10) < DateTime.Now)
            {
                mybeacons.RemoveAt(i);
            }
        }
        DisplayOnBeaconFound();
    }
    private void removeFoundBeacons()
    {
        Debug.Log("removing all found Beacons");
        BeaconDataText.text = "关闭寻找Beacon设备";
    }
    private void SetBroadcastState()
    {
        // setText
        if (bs_State == BroadcastState.inactive)
        {
            ActiveBtn.image.color = Color.red;
        }
        else
        {
            ActiveBtn.image.color = Color.green;
        }
    }

    private void DisplayOnBeaconFound()
    {
        removeFoundBeacons();
        string tmpTxt = "";
        foreach (Beacon b in mybeacons)
        {
            tmpTxt = "###########################################";
            switch (b.type)
            {
                case BeaconType.iBeacon:
                    tmpTxt += "\nUUID:";
                    tmpTxt += b.UUID.ToString();
                    tmpTxt += "\nMajor:";
                    tmpTxt += b.major.ToString();
                    tmpTxt += "\nMinor:";
                    tmpTxt += b.minor.ToString();
                    tmpTxt += "\nRange:";
                    tmpTxt += b.range.ToString();
                    tmpTxt += "\nStrength:";
                    tmpTxt += b.strength.ToString() + " db";
                    tmpTxt += "\nAccuracy:";
                    tmpTxt += b.accuracy.ToString().Substring(0, 10) + " m";
                    tmpTxt += "\nRssi:";
                    tmpTxt += b.rssi.ToString() + " db";
                    break;
                default:
                    break;
            }
        }
        BeaconDataText.text = tmpTxt;
    }
}