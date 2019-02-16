using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class StatusBarTest : MonoBehaviour {
        public Button toggleDimmedButton;
        public Button visibleButton;
        public Button visibleOverContentButton;
        public Button translucentOverContentButton;
        public Button hiddenButton;
        public Text DimLabel;
        // Use this for initialization
        void Start () {
                //当AndroidStatusBar.dimmed=false时，状态栏显示所有状态及通知图标
                //当AndroidStatusBar.dimmed=true时，状态栏仅电量和时间，不显示其他状态及通知
                if(toggleDimmedButton!=null){
                        toggleDimmedButton.onClick.AddListener (delegate {
                                AndroidStatusBar.dimmed=!AndroidStatusBar.dimmed;
                                DimLabel.text = "Dimmed: " + AndroidStatusBar.dimmed;
                        });
                }
                //显示状态栏，占用屏幕最上方的一部分像素
                if(visibleButton!=null){
                        visibleButton.onClick.AddListener (delegate {
                                AndroidStatusBar.statusBarState=AndroidStatusBar.States.Visible;
                        });
                }
                //悬浮显示状态栏，不占用屏幕像素
                if(visibleOverContentButton!=null){
                        visibleOverContentButton.onClick.AddListener (delegate {
                                AndroidStatusBar.statusBarState=AndroidStatusBar.States.VisibleOverContent;
                        });
                }
                //透明悬浮显示状态栏，不占用屏幕像素
                if(translucentOverContentButton!=null){
                        translucentOverContentButton.onClick.AddListener (delegate {
                                AndroidStatusBar.statusBarState=AndroidStatusBar.States.TranslucentOverContent;
                        });
                }
                //隐藏状态栏
                if(hiddenButton!=null){
                        hiddenButton.onClick.AddListener (delegate {
                                AndroidStatusBar.statusBarState=AndroidStatusBar.States.Hidden;
                        });
                }
        }
         
}