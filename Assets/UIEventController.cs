using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using MiniJSON;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class UIEventController : MonoBehaviour
{
   [SerializeField] private Text textArea; // 操作内容を表示するTextオブジェクト
   [SerializeField] private GameObject cube; // 操作対象のキューブ
   [SerializeField] Toggle toggle;
   [SerializeField] private Scrollbar scrollbar;
   // [SerializeField] private Dropdown ddtmp;
   [SerializeField] private Dropdown ddtmp;

   private GameObject avatar_model_object;
   private PlayerIKTarget playerIKTarget;

   private string[] json_file_name_array;


   // 開始時に呼ばれるメソッド
   void Start()
   {
       textArea.text = "";
       scrollbar.onValueChanged.AddListener((float val) => OnScrollbar(val));

       avatar_model_object = GameObject.Find("unitychan");
       playerIKTarget = avatar_model_object.GetComponent<PlayerIKTarget>();
       json_file_name_array = playerIKTarget.json_file_name;
       init_dropdown();
   }

   public void init_dropdown(){
        List<string> optionlist = new List<string>();

        foreach(var file_name in json_file_name_array){
            // Debug.Log("Skeleton coord file name : " + file_name);
            optionlist.Add(file_name);
        }

        //一度すべてのOptionsをクリア
        ddtmp.ClearOptions();

        //リストを追加
        ddtmp.AddOptions(optionlist);
   }

   // 左ボタン押下時の動作
   public void OnLeftButtonClick()
   {
       textArea.text = "Button Click: 左移動";
       Vector3 pos = cube.transform.position;
       pos.x += -0.1f;
       cube.transform.position = pos;
   }

   // 右ボタン押下時の動作
   public void OnRightButtonClick()
   {
       textArea.text = "Button Click: 右移動";
       Vector3 pos = cube.transform.position;
       pos.x += 0.1f;
       cube.transform.position = pos;
   }

   // 初期ボタン押下時の動作
   public void OnInitButtonClick()
   {
       textArea.text = "Button Click: 初期値位置";
       Vector3 pos = cube.transform.position;
       pos.x = -1.30f;
       cube.transform.position = pos;
   }

   // チェックボックス押下時の動作
   public void OnToggleClicked()
   {
       textArea.text = $"Toggle isOn Value:{toggle.isOn}";
       cube.SetActive(toggle.isOn);
   }

   // スクロールバー操作時の動作
   public void OnScrollbar(float value)
   {
       textArea.text = $"Scrollbar Value:{value}";
    //    Vector3 scale = cube.transform.localScale;
    //    scale.x = 0.3f + value;
    //    scale.y = 0.3f + value;
    //    scale.z = 0.3f + value;
    //    cube.transform.localScale = scale;

        float update_interval_scale = value * 0.01f;
        playerIKTarget.time_update_interval = playerIKTarget.init_time_update_interval + update_interval_scale;
   }

   public void OnDropdown(){
        // Dropdown ddtmp = GetComponent<Dropdown>();
        // ddtmp = GameObject.Find("Dropdown").GetComponent<Dropdown>();
        string selected_value = ddtmp.options[ddtmp.value].text;

        string datapath = Application.dataPath + "/Resources/" + selected_value;
        //Debug.LogFormat("datapath:{0}",datapath);
        StreamReader reader = new StreamReader(datapath); //受け取ったパスのファイルを読み込む
        string datastr = reader.ReadToEnd();//ファイルの中身をstring型としてすべて読み込む
        reader.Close();//ファイルを閉じる

        var json_data= Json.Deserialize(datastr) as Dictionary<string, object>;
        Debug.Log("json_data count:"+json_data.Count);

        //PlayerIKTarget.csのパラメーター制御
        playerIKTarget.json_data = json_data;
        playerIKTarget.current_frame_id = 0;
        playerIKTarget.FRAME_NUM = json_data.Count;
   }

   public void OnAthleteMotionPlay(){
        //is_athlete_motion_play
        playerIKTarget.Is_athlete_motion_play = true;
   }
}