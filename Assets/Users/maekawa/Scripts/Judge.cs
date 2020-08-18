﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

//判定ライン若干上かも　コライダー位置調整など

public class Judge : MonoBehaviour
{
    public static int[] totalGrades = { 0, 0, 0, 0, 0 };          // リザルト画面用の判定内訳
    public static float score = 0;
    public static int point = 0;
    public static int combo = 0;
    public static float comboMag = 1.0f;                          // コンボに応じたスコア倍率
    public static int bestcombo = 0;
    private static int[] _notesCount = { 0, 0, 0, 0, 0, 0, 0, 0 };// レーンごとのノーツカウント
    private static List<List<GameObject>> GOListArray = new List<List<GameObject>>();// ノーツ座標格納用2次元配列

    // 判定許容値**************************************
    [SerializeField] private float perfect;
    [SerializeField] private float great;
    [SerializeField] private float good;
    [SerializeField] private float bad;
    [SerializeField] private float miss;
    // **************************************判定許容値

    [SerializeField] private GameObject LeftJudgeLine;  // 右判定基準
    [SerializeField] private GameObject RightJudgeLine; // 左判定基準

    //タップ判定処理
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            int layerMask = 1;
            float maxDistance = 10f;

            Vector2 mousePosition = Input.mousePosition;

            Ray ray = Camera.main.ScreenPointToRay(mousePosition);

            RaycastHit2D hit = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction, maxDistance, layerMask);

            if ((hit.collider != null) && (hit.collider.gameObject.tag == ("Lane")))// tagでレーンを識別
            {
                string i = hit.collider.gameObject.name;  // レーン番号を取得
                int laneNumber = int.Parse(i);            // 文字列を数字に変換
                float absTiming = 9999;                   // nullにしたい

                //   GOListArray[何個目のノーツなのか[何番目のレーンの]]    [何番目のレーンなのか]
                if ((GOListArray[_notesCount[laneNumber]][laneNumber] != null) && (laneNumber <= 3))     // 左レーン
                {
                    absTiming = JudgeDistance(LeftJudgeLine.transform.position.y, GOListArray[_notesCount[laneNumber]][laneNumber].transform.position.y);
                }
                else if((GOListArray[_notesCount[laneNumber]][laneNumber] != null) && (laneNumber >= 4)) // 右レーン
                {
                    absTiming = JudgeDistance(RightJudgeLine.transform.position.y, GOListArray[_notesCount[laneNumber]][laneNumber].transform.position.y);
                }

                //判定分岐
                if (absTiming <= perfect)
                {
                    point = 300;
                    combo++;
                    totalGrades[0]++;
                    Debug.Log("perfect");
                    JudgeGrade(combo, point, laneNumber);
                }
                else if (absTiming <= great)
                {
                    point = 200;
                    combo++;
                    totalGrades[1]++;
                    Debug.Log("great");  
                    JudgeGrade(combo, point, laneNumber);
                }
                else if (absTiming <= good)
                {
                    point = 100;
                    combo = 0;
                    totalGrades[2]++;
                    Debug.Log("good");
                    JudgeGrade(combo, point, laneNumber);
                }
                else if (absTiming <= bad)
                {
                    point = 10;
                    combo = 0;
                    totalGrades[3]++;
                    Debug.Log("bad");
                    JudgeGrade(combo, point, laneNumber);
                }
                else if (absTiming <= miss)
                {
                    point = 0;
                    combo = 0;
                    totalGrades[4]++;
                    Debug.Log("miss");
                    JudgeGrade(combo, point, laneNumber);
                }
            }
        }
    }

    public static void ListImport()
    {
        GOListArray = NotesManager.NotesPositions;
        Debug.Log(GOListArray[0][7]);
    }

    // ノーツ通過処理
    public static void NotesCountUp(string i)
    {
        if (combo > bestcombo)//最大コンボ記憶
            bestcombo = combo;
        combo = 0;
        totalGrades[4]++;
        Debug.Log("NotesCountUp_miss");

        int tempLaneNum = int.Parse(i);//文字列を数字に変換
        Destroy(GOListArray[_notesCount[tempLaneNum]][tempLaneNum]);   // ノーツ破棄
        GOListArray[_notesCount[tempLaneNum]][tempLaneNum] = null;     // 多重タップを防ぐ
        _notesCount[tempLaneNum]++;                                    // 該当レーンのノーツカウント++
    }

    // タイミング誤差算出
    private float JudgeDistance(float i, float j)// 判定ライン　－　ノーツ
    {
        float tempTiming = i - j;
        float trueTiming = Mathf.Abs(tempTiming);// 絶対値に変換

        //Debug.Log("JUDGELINE" + i);// 判定調節用
        //Debug.Log("NOTE" + j);
        //Debug.Log("ABSTIMING" + trueTiming);

        return trueTiming;
    }

    // 判定後処理
    private void JudgeGrade(int combos, int points, int laneNumber)
    {
        if (combos > bestcombo)//最大コンボ記憶
            bestcombo = combos;

        // 各コンボ倍率
        if (combos > 250)
        {
            comboMag = 1.5f;
        }
        else if (combos > 150)
        {
            comboMag = 1.4f;
        }
        else if (combos > 100)
        {
            comboMag = 1.3f;
        }
        else if (combos > 50)
        {
            comboMag = 1.2f;
        }
        else if (combos > 10)
        {
            comboMag = 1.1f;
        }
        else
        {
            comboMag = 1.0f;
        }
        score += points * comboMag;
        //Mathf.Floor(score);

        Destroy(GOListArray[_notesCount[laneNumber]][laneNumber]);   // ノーツ破棄
        GOListArray[_notesCount[laneNumber]][laneNumber] = null;     // 多重タップを防ぐ
        _notesCount[laneNumber]++;                                   // 該当レーンのノーツカウント++
    }
}