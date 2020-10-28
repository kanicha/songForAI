﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
/// <summary>
/// ピアノ用ノーツジェネレータ
/// </summary>
public class NotesGenerater : MonoBehaviour
{
    [SerializeField, Header("ノーツを生成する元(Prefab)")]
    GameObject NotesPrefab = null, longNotes = null, bar = null;
    [SerializeField,Header("生成先")]
    List<GameObject> NotesGen = null;
    [SerializeField]
    float NotesDistance = 0.001f;
    public static float NotesSpeed = 0.3f;
    public static float speed = 0.3f;
    public static float offset = 0;// 譜面の再生を遅らせる
    float bpm = 0;

    //string[] filePaths = System.IO.Directory.GetFiles(Application.streamingAssetsPath, "*.nts");
    //ノーツデータ
    NotesJson.MusicData musicData = new NotesJson.MusicData();
    bool Generated=false;
    bool PlayedBGM=false;
    KeyJudge _judge;
    float fps;

    Vector3 move;

    // とりあえずここで設定
    private float[] highSpeeds = { 0.3f, 0.4f, 0.3f };
    private float[] Speeds = { 0.256f, 0.075f, 0.335f };
    private float[] offsets = { 0, -8.0f, -9.0f };
    /// <summary>
    /// Update 主にノーツの移動部分の計算をしている
    /// </summary>
    private void Update()
    {
        fps = 1 / Time.deltaTime;
        //Debug.Log(fps);
        if (Generated)
        {
            float a = 60 / bpm;
            float b = a * fps;
            float c = b / 8;
            //Debug.Log(a+" "+b+" "+c);
            move = new Vector3(0, c*speed, 0);

            if (!PlayedBGM)
            {
                SoundManager.BGMSoundCue(MusicDatas.cueMusic);
                //SoundManager.BGMSoundCue(5);
                PlayedBGM = true;
            }
            // Debug.Log(NotesSpeed);
        }
    }
    /// <summary>
    /// LateUpdate 主にノーツの移動部分の処理をしている
    /// </summary>
    private void LateUpdate()
    {
        NotesGen[0].transform.root.gameObject.transform.position -= move * NotesSpeed;
    }
    /// <summary>
    /// 初期化
    /// </summary>
    private void Start()
    {
        NotesSpeed = highSpeeds[MusicDatas.MusicNumber];
        speed = Speeds[MusicDatas.MusicNumber];
        offset = offsets[MusicDatas.MusicNumber];

        // ノーツを遅らせる
        NotesGen[0].transform.root.gameObject.transform.position += new Vector3(0, offset, 0);
        Application.targetFrameRate = 60;
    }
    /// <summary>
    /// ノーツ生成を行う関数
    /// </summary>
    public void NotesGenerate()
    {
        //ファイルの読み込み
        FileInfo info = new FileInfo(Application.streamingAssetsPath + string.Format("/{0}_{1}.nts",MusicDatas.NotesDataName,MusicDatas.difficultNumber));
        //FileInfo info = new FileInfo(Application.streamingAssetsPath + "/Shining_3.nts");
        //Debug.Log(info);
        StreamReader reader = new StreamReader(info.OpenRead());
        string Musics_ = reader.ReadToEnd();
        musicData = JsonUtility.FromJson<NotesJson.MusicData>(Musics_);
        SpeedMgr.BPM = musicData.BPM;

        // ノーツ生成
        for (int i = 0; musicData.notes.Length > i; i++)
        {
            // リスト初期化
            NotesManager.NotesPositions.Add(new List<GameObject>()); //nex
            for (int e = 0; e < 8; e++)
            {
                NotesManager.NotesPositions[i].Add(null);
            }

            // ノーツデータを変数に代入
            int LaneNum = musicData.notes[i].block;
            int NotesType = musicData.notes[i].type;
            int NotesNum = musicData.notes[i].num;
            bpm = musicData.BPM;

            // ノーツの種類判別
            if (NotesType == 1)
            {
                //生成
                GameObject GenNotes = Instantiate(NotesPrefab, new Vector3(NotesGen[LaneNum].transform.position.x
                    , 0
                    , 0)
                    , Quaternion.identity) as GameObject;
                GenNotes.name = "notes_" + NotesNum.ToString();
                GenNotes.transform.parent = NotesGen[LaneNum].transform;
                Vector3 positions = new Vector3(0, (NotesNum +1) * NotesSpeed, 0);
                GenNotes.transform.localPosition = new Vector3(0, 0, 0);
                GenNotes.transform.localPosition += positions;
                notesPositionAdd(GenNotes, LaneNum, i);
            }
            else if(NotesType == 2)
            {
                int notesNum2 = musicData.notes[i].notes[0].num;
                GameObject GenNotes = Instantiate(longNotes, new Vector3(NotesGen[LaneNum].transform.position.x
                    , NotesNum * NotesSpeed
                    , 0)
                    , Quaternion.identity) as GameObject;
                GenNotes.name = "notes_" + NotesNum.ToString();
                GenNotes.transform.parent = NotesGen[LaneNum].transform;
                Vector2 longPos = new Vector2(0.23f,notesNum2-NotesNum);
                longPos.y *= 0.03f*(16/musicData.notes[i].LPB);
                //longPos.y *= 0.03f * (4 / musicData.notes[i].LPB);
                GenNotes.transform.localScale = longPos;
                notesPositionAdd(GenNotes, LaneNum, i);
            }

            move = new Vector3(0, 1.06f*Time.deltaTime, 0);
            NotesManager.NextNotesLine.Add(LaneNum);
            Generated = true;

        }
        ScoreManager.maxScore = Judge.gradesPoint[0] * musicData.notes.Length; // perfect時の得点 * 最大コンボ　で天井点を取得

        KeyJudge.ListImport();

    }
    /// <summary>
    /// ノーツリストに追加する関数
    /// </summary>
    /// <param name="notes"></param>
    /// <param name="Lane"></param>
    /// <param name="num"></param>
    private void notesPositionAdd(GameObject notes, int Lane, int num)
    {
        for (int i = 0; i < NotesManager.NotesPositions.Count; i++)
        {
            if (NotesManager.NotesPositions[i][Lane] == null)
            {
                NotesManager.NotesPositions[i][Lane] = notes;
                break;
            }
        }
    }

}