using System;
using System.IO;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MiniJSON;

//このスクリプトでは、IKを利用して3Dモデルを動かす
//IKのターゲットとなるオブジェクトを制御する

public class PlayerIKTarget : MonoBehaviour
{
    Animator animator;
    float init_hips_y;

    // IKのターゲットとなる関節のターゲットオブジェクト
    // 手
    GameObject[] target_left_hand;
    GameObject[] target_left_elbow;
    GameObject[] target_left_upperarm;
    GameObject[] target_right_hand;
    GameObject[] target_right_elbow;
    GameObject[] target_right_upperarm;
    // 足
    GameObject[] target_left_foot;
    GameObject[] target_left_knee;
    GameObject[] target_left_upperleg;
    GameObject[] target_right_foot;
    GameObject[] target_right_knee;
    GameObject[] target_right_upperleg;
    // 胴体
    GameObject[] target_body_lookat;
    GameObject[] target_head;
    GameObject[] target_neck;
    GameObject[] target_spine;
    GameObject[] target_hips;

    //骨格jsonファイル
    //Json skeleton data読み出し
    //
    public string[] json_file_name = new string[]{
        "skeleton_coord_ATP2020_00_47_Novak_Djokovic_postprocessed.json",
        "dance1.json",
        "momo_likey.json",
        "jyp.json",
        "jyp2.json",
        "jyp2New.json",
        "jyp3.json",
        "matt_AllIWannaDo.json"
    };
    
    
    //人数
    public int numberOfPerson = 0;
    //フォーメーション
    public int formationType = 0;
    public int FRAME_NUM = 0; //FRAME数
    public int current_frame_id = 0; //現在のフレーム番号
    private int init_frame_id = 0;
    public float init_time_update_interval=0.01f;
    public float time_update_interval=0.01f; //フレームアップデートタイミング
    private float timeElapsed = 0.0f; //前フレーム描画からの経過時間
    public Dictionary<string, object> json_data = new Dictionary<string, object>(); //

    private Dictionary<string,Vector3> skeleton_coord = new Dictionary<string, Vector3>(); //
    private Dictionary<string,float> skeleton_length = new Dictionary<string,float>(); //関節の長さ

    private Dictionary<string,Vector3> calibrated_skeleton_coord = new Dictionary<string, Vector3>();
    private bool is_athlete_motion_play=false;

    public bool Is_athlete_motion_play
    {
        get { return is_athlete_motion_play; }
        set { is_athlete_motion_play = value; }
    }

    /// <summary>
    /// 3D humanoidモデルの関節長を計算する
    /// </summary>
    /// <param name="x">関節のxyz座標</param>
    /// <param name="y">関節のxyz座標</param>
    /// <returns>joint_length</returns>
    public float get_joint_length(
        Vector3 x,
        Vector3 y
    ){
        Vector3 diff_vector = x - y;
        float joint_length = diff_vector.magnitude;

        return joint_length;
    }


    /// <summary>
    /// 3D humanoidモデルの関節長に合わせた、ターゲットとなる関節位置を計算する
    /// </summary>
    /// <param name="model_joint_init">較正済み3Dモデル関節のスタート地点座標</param>
    /// <param name="model_joint_length">3Dモデル関節の長さ</param>
    /// <param name="pred_joint_start">機械学習予測の関節のスタート地点座標</param>
    /// <param name="pred_joint_end">機械学習予測の関節のスタート地点座標</param>
    /// <returns>target_position</returns>
    public Vector3 get_calibrated_target_position(
        Vector3 model_joint_init,
        float model_joint_length,
        Vector3 pred_joint_start, 
        Vector3 pred_joint_end
    ){
        Vector3 diff_pred_joint = pred_joint_end - pred_joint_start;//機械学習予測のボーンベクトル
        Vector3 pred_tani_vector = diff_pred_joint.normalized;//機械学習予測のボーン単位ベクトル
        Vector3 target_vector = model_joint_length * pred_tani_vector; 
        Vector3 target_position = model_joint_init + target_vector;

        return target_position;
    }

    // Start is called before the first frame update
    void Start(){
        // ターゲットオブジェクトの初期化
        target_left_hand = new GameObject[numberOfPerson];
        target_left_elbow = new GameObject[numberOfPerson];
        target_left_upperarm = new GameObject[numberOfPerson];
        target_right_hand = new GameObject[numberOfPerson];
        target_right_elbow = new GameObject[numberOfPerson];
        target_right_upperarm = new GameObject[numberOfPerson];
        target_left_foot = new GameObject[numberOfPerson];
        target_left_knee = new GameObject[numberOfPerson];
        target_left_upperleg = new GameObject[numberOfPerson];
        target_right_foot = new GameObject[numberOfPerson];
        target_right_knee = new GameObject[numberOfPerson];
        target_right_upperleg = new GameObject[numberOfPerson];
        target_body_lookat = new GameObject[numberOfPerson];
        target_head = new GameObject[numberOfPerson];
        target_neck = new GameObject[numberOfPerson];
        target_spine = new GameObject[numberOfPerson];
        target_hips = new GameObject[numberOfPerson];

        this.animator = GetComponent<Animator>();
        // this.init_hips_y = animator.GetBoneTransform(HumanBodyBones.Hips).transform.position.y;
        // this.animator.transform.position = new Vector3(0,0.5f,0);

        Debug.Log("This script is attached to: " + gameObject.name);
        Debug.Log("Init_hips: " + this.init_hips_y);

        Debug.LogFormat("Time interval first:{0}",this.time_update_interval);
        
        //
        //3D Model Avatarを操作する際のIKのターゲットとなる関節オブジェクトの取得
        //
        //TODO: ENUM型?
        //体幹のIKターゲット
        //this.target_body_lookat = GameObject.Find("BodyLookAtTarget");
        for (int i = 0; i < numberOfPerson; i++) {
            this.target_neck[i] = GameObject.Find("NeckTarget" + (i + 1));
            this.target_spine[i] = GameObject.Find("SpineTarget" + (i + 1));
            this.target_hips[i] = GameObject.Find("HipsTarget" + (i + 1));
            this.target_head[i] = GameObject.Find("HeadTarget" + (i + 1));
            // Right arm IK targets
            this.target_right_upperarm[i] = GameObject.Find("RightUpperArmTarget" + (i + 1));
            this.target_right_elbow[i] = GameObject.Find("RightElbowTarget" + (i + 1));
            this.target_right_hand[i] = GameObject.Find("RightHandTarget" + (i + 1));
            // Left arm IK targets
            this.target_left_upperarm[i] = GameObject.Find("LeftUpperArmTarget" + (i + 1));
            this.target_left_elbow[i] = GameObject.Find("LeftElbowTarget" + (i + 1));
            this.target_left_hand[i] = GameObject.Find("LeftHandTarget" + (i + 1));
            // Right leg IK targets
            this.target_right_upperleg[i] = GameObject.Find("RightUpperLegTarget" + (i + 1));
            this.target_right_knee[i] = GameObject.Find("RightKneeTarget" + (i + 1));
            this.target_right_foot[i] = GameObject.Find("RightFootTarget" + (i + 1));
            // Left leg IK targets
            this.target_left_upperleg[i] = GameObject.Find("LeftUpperLegTarget" + (i + 1));
            this.target_left_knee[i] = GameObject.Find("LeftKneeTarget" + (i + 1));
            this.target_left_foot[i] = GameObject.Find("LeftFootTarget" + (i + 1));
        }

        //
        //3d humanoid modelの関節オブジェクト取得
        //
        //体幹
        Transform head = animator.GetBoneTransform(HumanBodyBones.Head);
        Transform neck = animator.GetBoneTransform(HumanBodyBones.Neck);
        Transform spine = animator.GetBoneTransform(HumanBodyBones.Spine);
        Transform spine1 = animator.GetBoneTransform(HumanBodyBones.Chest);
        Transform hips = animator.GetBoneTransform(HumanBodyBones.Hips);

        //右腕
        Transform right_shoulder = animator.GetBoneTransform(HumanBodyBones.RightShoulder);
        Transform right_upperarm = animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
        Transform right_lowerarm = animator.GetBoneTransform(HumanBodyBones.RightLowerArm);
        Transform right_hand = animator.GetBoneTransform(HumanBodyBones.RightHand);
        
        //右足
        Transform right_upperleg = animator.GetBoneTransform(HumanBodyBones.RightUpperLeg);
        Transform right_lowerleg = animator.GetBoneTransform(HumanBodyBones.RightLowerLeg);
        Transform right_foot = animator.GetBoneTransform(HumanBodyBones.RightFoot);
        
        //左腕
        Transform left_shoulder = animator.GetBoneTransform(HumanBodyBones.LeftShoulder);
        Transform left_upperarm = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
        Transform left_lowerarm = animator.GetBoneTransform(HumanBodyBones.LeftLowerArm);
        Transform left_hand = animator.GetBoneTransform(HumanBodyBones.LeftHand);
        
        //左足
        Transform left_upperleg = animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
        Transform left_lowerleg = animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
        Transform left_foot = animator.GetBoneTransform(HumanBodyBones.LeftFoot);

        //
        //Animator Avatarの関節長の取得
        //胴体
        this.skeleton_length["hips-spine"] = get_joint_length(hips.position,spine.position);
        this.skeleton_length["spine-neck"] = get_joint_length(spine.position,neck.position);
        this.skeleton_length["neck-head"] = get_joint_length(neck.position,head.position);
        //右腕
        this.skeleton_length["neck-right_upperarm"] = get_joint_length(neck.position,right_upperarm.position);
        this.skeleton_length["right_upperarm-right_lowerarm"] = get_joint_length(right_upperarm.position,right_lowerarm.position);
        this.skeleton_length["right_lowerarm-right_hand"] = get_joint_length(right_lowerarm.position,right_hand.position);
        //左腕
        this.skeleton_length["neck-left_upperarm"] = get_joint_length(neck.position,left_upperarm.position);
        this.skeleton_length["left_upperarm-left_lowerarm"] = get_joint_length(left_upperarm.position,left_lowerarm.position);
        this.skeleton_length["left_lowerarm-left_hand"] = get_joint_length(left_lowerarm.position,left_hand.position);
        //右足
        this.skeleton_length["hips-right_upperleg"] = get_joint_length(hips.position,right_upperleg.position);
        this.skeleton_length["right_upperleg-right_lowerleg"] = get_joint_length(right_upperleg.position,right_lowerleg.position);
        this.skeleton_length["right_lowerleg-right_foot"] = get_joint_length(right_lowerleg.position,right_foot.position);
        //左足
        this.skeleton_length["hips-left_upperleg"] = get_joint_length(hips.position,left_upperleg.position);
        this.skeleton_length["left_upperleg-left_lowerleg"] = get_joint_length(left_upperleg.position,left_lowerleg.position);
        this.skeleton_length["left_lowerleg-left_foot"] = get_joint_length(left_lowerleg.position,left_foot.position);
        foreach (KeyValuePair<string, float> pair in this.skeleton_length) {
            Debug.Log ("Dictionary:" + pair.Key + " : " + pair.Value);
        }

        //string datapath = Application.dataPath + "/Resources/" + json_file_name[0];
        //string datapath = "Assets/Resources/skeleton_coord_ATP2020_00_47_Novak_Djokovic_postprocessed.json";
        //ココ書き換える
        string datapath = "Assets/Resources/hulaDance_U4P4u6eSIHw_si=5XCbThpCddUNiwRs_220_243.json";
        Debug.Log("path: "+ datapath);
        StreamReader reader = new StreamReader(datapath); //受け取ったパスのファイルを読み込む
        string datastr = reader.ReadToEnd();//ファイルの中身をstring型としてすべて読み込む
        reader.Close();//ファイルを閉じる

        json_data= Json.Deserialize(datastr) as Dictionary<string, object>;
        Debug.Log("json_data count:"+json_data.Count);
        current_frame_id = 0;
        FRAME_NUM = json_data.Count;

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name=""></param>
    /// <param name=""></param>
    /// <param name=""></param>
    /// <returns></returns>
    void Update_skeleton_coord(){

        //jsonモーションファイルが読み込めていない場合は更新しない
        if(json_data.Count==0){
            return ;
        }

        if(current_frame_id+1==FRAME_NUM){
            current_frame_id = init_frame_id;
            is_athlete_motion_play = false;
        }
        //current frame id update
        current_frame_id = Math.Max(init_frame_id,(current_frame_id+1)%FRAME_NUM);

        

        // if(current_frame_id==319){
        //     current_frame_id=324;
        //     return ;
        // }
        Debug.LogFormat("Max Frame Num:{0}",FRAME_NUM);
        Debug.LogFormat("Current frame:{0}",current_frame_id);
        
        //TODO: シンプルなコードにする
        //TODO: FRAME_NUMここで読みだしたほうが良いかも
        foreach (KeyValuePair<string, object> item in this.json_data)
        {
            int frame_id = Convert.ToInt32(item.Key);
            if(frame_id!=current_frame_id){
                continue;
            }

            Dictionary<string, object> skeleton = (Dictionary<string, object>)item.Value;
            foreach (var entry in skeleton){
                String pos_name = Convert.ToString(entry.Key);
                List<object> coord =  (List<object>)entry.Value;

                float x = (float)Convert.ToDouble(coord[0]);
                float y = (float)Convert.ToDouble(coord[1]);
                float z = (float)Convert.ToDouble(coord[2]);
                Vector3 v = new Vector3(x, y, z);
                //Vector3 v = new Vector3(-x, y, -z);
                this.skeleton_coord[pos_name] = v;
            }
        }

        //3d humanoid modelの関節オブジェクト取得
        //体幹
        Transform head = animator.GetBoneTransform(HumanBodyBones.Head);
        Transform neck = animator.GetBoneTransform(HumanBodyBones.Neck);
        Transform spine = animator.GetBoneTransform(HumanBodyBones.Spine);
        Transform spine1 = animator.GetBoneTransform(HumanBodyBones.Chest);
        Transform hips = animator.GetBoneTransform(HumanBodyBones.Hips);

        //右腕
        Transform right_shoulder = animator.GetBoneTransform(HumanBodyBones.RightShoulder);
        Transform right_upperarm = animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
        Transform right_lowerarm = animator.GetBoneTransform(HumanBodyBones.RightLowerArm);
        Transform right_hand = animator.GetBoneTransform(HumanBodyBones.RightHand);
        
        //右足
        Transform right_upperleg = animator.GetBoneTransform(HumanBodyBones.RightUpperLeg);
        Transform right_lowerleg = animator.GetBoneTransform(HumanBodyBones.RightLowerLeg);
        Transform right_foot = animator.GetBoneTransform(HumanBodyBones.RightFoot);
        
        //左腕
        Transform left_shoulder = animator.GetBoneTransform(HumanBodyBones.LeftShoulder);
        Transform left_upperarm = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
        Transform left_lowerarm = animator.GetBoneTransform(HumanBodyBones.LeftLowerArm);
        Transform left_hand = animator.GetBoneTransform(HumanBodyBones.LeftHand);
        
        //左足
        Transform left_upperleg = animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
        Transform left_lowerleg = animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
        Transform left_foot = animator.GetBoneTransform(HumanBodyBones.LeftFoot);
        


        //体回転方向
        // Vector3 shoulder_direction = this.skeleton_coord["right_upperleg"]-this.skeleton_coord["left_upperleg"];
        // Vector3 body_direction = Vector3.Cross(shoulder_direction,new Vector3(0.0f,1.0f,0.0f));//向く方向
        // body_direction.y = 0.0f;
        // body_direction = body_direction.normalized;
        // Quaternion next_body_rotation = Quaternion.LookRotation(body_direction);
        // //this.target_body_lookat.transform.position = body_direction;
        // this.target_hips.transform.rotation = next_body_rotation;
        
        //3d humanoid model joint->ML predicted skeleton joint coordの対応
        //Hips->coord 0
        //Spine->coord 7 or 対応なし
        //Spine1==Chest->coord 7 or 対応なし
        //Neck->coord 8
        //対応なし->coord 9
        //Head->coord 10, head1


        //体幹
        //HipsTarget y座標高さ修正
        //TODO : y coord biasをモデルごとに任意に変更する
        this.calibrated_skeleton_coord["hips"] = new Vector3(
            this.skeleton_coord["hips"].x,
            this.skeleton_coord["hips"].y, //+0.06f,
            this.skeleton_coord["hips"].z
        );



        //spine位置、スケーリング
        this.calibrated_skeleton_coord["spine"] = get_calibrated_target_position(
            model_joint_init  : this.calibrated_skeleton_coord["hips"],
            model_joint_length: this.skeleton_length["hips-spine"],
            pred_joint_start  : this.skeleton_coord["hips"],
            pred_joint_end    : this.skeleton_coord["spine"]
        );

        //Neck位置、スケーリング
        this.calibrated_skeleton_coord["neck"] = get_calibrated_target_position(
            model_joint_init  : this.calibrated_skeleton_coord["spine"],
            model_joint_length: this.skeleton_length["spine-neck"],
            pred_joint_start  : this.skeleton_coord["spine"],
            pred_joint_end    : this.skeleton_coord["neck"]
        );

        //Head位置、スケーリング
        this.calibrated_skeleton_coord["head"] = get_calibrated_target_position(
            model_joint_init  : this.calibrated_skeleton_coord["neck"],
            model_joint_length: this.skeleton_length["neck-head"],
            pred_joint_start  : this.skeleton_coord["neck"],
            pred_joint_end    : this.skeleton_coord["head1"]
        );
        //TODO : headの座標 3d humanoid modelへ反映

        //体幹
        SetPositions("hips", this.target_hips, this.calibrated_skeleton_coord);
        SetPositions("spine", this.target_spine, this.calibrated_skeleton_coord);
        SetPositions("neck", this.target_neck, this.calibrated_skeleton_coord);

        //
        //右腕の処理
        //右肩位置、スケーリング
        this.calibrated_skeleton_coord["right_upperarm"] = get_calibrated_target_position(
            model_joint_init  : this.calibrated_skeleton_coord["neck"],
            model_joint_length: this.skeleton_length["neck-right_upperarm"],
            pred_joint_start  : this.skeleton_coord["neck"], 
            pred_joint_end    : this.skeleton_coord["right_upperarm"]
        );
        
        //右ひじ位置、スケーリング
        this.calibrated_skeleton_coord["right_lowerarm"] = get_calibrated_target_position(
            model_joint_init  : this.calibrated_skeleton_coord["right_upperarm"],
            model_joint_length: this.skeleton_length["right_upperarm-right_lowerarm"],
            pred_joint_start  : this.skeleton_coord["right_upperarm"],
            pred_joint_end    : this.skeleton_coord["right_lowerarm"]
        );

        //右手位置、スケーリング
        this.calibrated_skeleton_coord["right_hand"] = get_calibrated_target_position(
            model_joint_init  : this.calibrated_skeleton_coord["right_lowerarm"],
            model_joint_length: this.skeleton_length["right_lowerarm-right_hand"],
            pred_joint_start  : this.skeleton_coord["right_lowerarm"],
            pred_joint_end    : this.skeleton_coord["right_hand"]
        );

        //右手位置セット
        Debug.LogFormat("Right Upper Arm Object:{0}",this.target_right_upperarm);
        Debug.LogFormat("Right Upper Arm Coord:{0}",this.calibrated_skeleton_coord["right_upperarm"]);

        SetPositions("right_upperarm", this.target_right_upperarm, this.calibrated_skeleton_coord);
        SetPositions("right_lowerarm", this.target_right_elbow, this.calibrated_skeleton_coord);
        SetPositions("right_hand", this.target_right_hand, this.calibrated_skeleton_coord);

        //左腕の処理
        // TODO: calibrated skeleton coordがおかしい
        //左肩位置、スケーリング
        this.calibrated_skeleton_coord["left_upperarm"] = get_calibrated_target_position(
            model_joint_init  : this.calibrated_skeleton_coord["neck"],
            model_joint_length: this.skeleton_length["neck-left_upperarm"],
            pred_joint_start  : this.skeleton_coord["neck"], 
            pred_joint_end    : this.skeleton_coord["left_upperarm"]
        );
        //左ひじ位置、スケーリング
        this.calibrated_skeleton_coord["left_lowerarm"] = get_calibrated_target_position(
            //animator.GetBoneTransform(HumanBodyBones.LeftLowerArm).position,
            model_joint_init  : this.calibrated_skeleton_coord["left_upperarm"],
            model_joint_length: this.skeleton_length["left_upperarm-left_lowerarm"],
            pred_joint_start  : this.skeleton_coord["left_upperarm"],
            pred_joint_end    : this.skeleton_coord["left_lowerarm"]
        );
        //左手位置、スケーリング
        this.calibrated_skeleton_coord["left_hand"] = get_calibrated_target_position(
            model_joint_init  : this.calibrated_skeleton_coord["left_lowerarm"],
            model_joint_length: this.skeleton_length["left_lowerarm-left_hand"],
            pred_joint_start  : this.skeleton_coord["left_lowerarm"],
            pred_joint_end    : this.skeleton_coord["left_hand"]
        );
        //左手位置セット
        Debug.LogFormat("Left Upper Arm Object:{0}",this.target_left_upperarm);
        Debug.LogFormat("Left Upper Arm Coord:{0}",this.calibrated_skeleton_coord["left_upperarm"]);

        SetPositions("left_upperarm", this.target_left_upperarm, this.calibrated_skeleton_coord);
        SetPositions("left_lowerarm", this.target_left_elbow, this.calibrated_skeleton_coord);
        SetPositions("left_hand", this.target_left_hand, this.calibrated_skeleton_coord);

        //右足位置、スケーリング
        this.calibrated_skeleton_coord["right_upperleg"] = get_calibrated_target_position(
            model_joint_init  : this.calibrated_skeleton_coord["hips"],
            model_joint_length: this.skeleton_length["hips-right_upperleg"],
            pred_joint_start  : this.skeleton_coord["hips"],
            pred_joint_end    : this.skeleton_coord["right_upperleg"]
        );
        this.calibrated_skeleton_coord["right_lowerleg"] = get_calibrated_target_position(
            model_joint_init  : this.calibrated_skeleton_coord["right_upperleg"],
            model_joint_length: this.skeleton_length["right_upperleg-right_lowerleg"],
            pred_joint_start  : this.skeleton_coord["right_upperleg"],
            pred_joint_end    : this.skeleton_coord["right_lowerleg"]
        );
        this.calibrated_skeleton_coord["right_foot"] = get_calibrated_target_position(
            model_joint_init  : this.calibrated_skeleton_coord["right_lowerleg"],
            model_joint_length: this.skeleton_length["right_lowerleg-right_foot"],
            pred_joint_start  : this.skeleton_coord["right_lowerleg"],
            pred_joint_end    : this.skeleton_coord["right_foot"]
        );

        SetPositions("right_upperleg", this.target_right_upperleg, this.calibrated_skeleton_coord);
        SetPositions("right_lowerleg", this.target_right_knee, this.calibrated_skeleton_coord);
        SetPositions("right_foot", this.target_right_foot, this.calibrated_skeleton_coord);

        //左足位置、スケーリング
        this.calibrated_skeleton_coord["left_upperleg"] = get_calibrated_target_position(
            model_joint_init  : this.calibrated_skeleton_coord["hips"],
            model_joint_length: this.skeleton_length["hips-left_upperleg"],
            pred_joint_start  : this.skeleton_coord["hips"],
            pred_joint_end    : this.skeleton_coord["left_upperleg"]
        );
        this.calibrated_skeleton_coord["left_lowerleg"] = get_calibrated_target_position(
            model_joint_init  : this.calibrated_skeleton_coord["left_upperleg"],
            model_joint_length: this.skeleton_length["left_upperleg-left_lowerleg"],
            pred_joint_start  : this.skeleton_coord["left_upperleg"],
            pred_joint_end    : this.skeleton_coord["left_lowerleg"]
        );
        this.calibrated_skeleton_coord["left_foot"] = get_calibrated_target_position(
            model_joint_init  : this.calibrated_skeleton_coord["left_lowerleg"],
            model_joint_length: this.skeleton_length["left_lowerleg-left_foot"],
            pred_joint_start  : this.skeleton_coord["left_lowerleg"],
            pred_joint_end    : this.skeleton_coord["left_foot"]
        );
        //Debug.LogFormat("calibrated_skeleton_coord left_lowerleg:{0}",this.calibrated_skeleton_coord["left_lowerleg"]);

        SetPositions("left_upperleg", this.target_left_upperleg, this.calibrated_skeleton_coord);
        SetPositions("left_lowerleg", this.target_left_knee, this.calibrated_skeleton_coord);
        SetPositions("left_foot", this.target_left_foot, this.calibrated_skeleton_coord);
    }

    // Update is called once per frame

    //50fps固定
    void FixedUpdate(){
        float fps = 1f / Time.deltaTime;
        Debug.LogFormat("{0}fps", fps);

        this.timeElapsed += Time.deltaTime;

        if(this.timeElapsed>=this.time_update_interval){
            Update_skeleton_coord();
            this.timeElapsed = 0.0f;
        }
    }

    //各部位にポジションをセット
    void SetPositions(string key, GameObject[] targets, Dictionary<string, Vector3> calibratedCoords) {
        for (int i = 0; i < targets.Length; i++) {
            Vector3 pos = calibratedCoords[key];

            if (formationType == 1) {
                pos += RowCalculateOffset(i);
            } else if(formationType == 2){
                pos += PyramidCalculateOffset(i);
            } else if(formationType == 3){
                pos += SquareCalculateOffset(i);
            } else if(formationType == 4){
                pos += TrapeziumCalculateOffset(i);
            } else if(formationType == 5){
                pos += CircleCalculateOffset(i);
            }
            
            targets[i].transform.position = pos;
        }
    }
    Vector3 RowCalculateOffset(int i) {
        return new Vector3(0, 0, (i % 2 == 0) ? (-2 * (i / 2)) : (2 * ((i + 1) / 2)));   
    }
    Vector3 PyramidCalculateOffset(int i) {
        if (i == 0) {
            return new Vector3(2, 0, (i % 2 == 0) ? (-2 * (i / 2)) : (2 * ((i + 1) / 2)));   
        }else if (i == 1) {
            return new Vector3(0, 0, (i % 2 == 0) ? (-2 * (i / 2)) : (2 * ((i + 1) / 2)));   
        }else if (i == 2) {
            return new Vector3(0, 0, (i % 2 == 0) ? (-2 * (i / 2)) : (2 * ((i + 1) / 2)));   
        }else if (i == 3) {
            return new Vector3(-2, 0, (i % 2 == 0) ? (-2 * (i / 2)) : (2 * ((i + 1) / 2)));   
        }else {
            return new Vector3(-2, 0, (i % 2 == 0) ? (-2 * (i / 2)) : (2 * ((i + 1) / 2)));   
        }
    }

    Vector3 SquareCalculateOffset(int i) {
        if (i == 0) {
            return new Vector3(0, 0, 0);   
        }else if (i == 1) {
            return new Vector3(2, 0, 2);      
        }else if (i == 2) {
            return new Vector3(2, 0, -2);     
        }else if (i == 3) {
            return new Vector3(-2, 0, 2);      
        }else {
            return new Vector3(-2, 0, -2);   
        }
    }
    Vector3 TrapeziumCalculateOffset(int i) {
        if (i == 0) {
            return new Vector3(0, 0, 0);   
        }else if (i == 1) {
            return new Vector3(1, 0, 1);      
        }else if (i == 2) {
            return new Vector3(1, 0, -1);     
        }else if (i == 3) {
            return new Vector3(-1, 0, 2);      
        }else {
            return new Vector3(-1, 0, -2);   
        }
    }
    Vector3 CircleCalculateOffset(int i) {
        if (i == 0) {
            return new Vector3(-2, 0, 0);   
        }else if (i == 1) {
            return new Vector3(2, 0, 1);      
        }else if (i == 2) {
            return new Vector3(2, 0, -1);     
        }else if (i == 3) {
            return new Vector3(0, 0, 2);      
        }else {
            return new Vector3(0, 0, -2);   
        }
    }
}

