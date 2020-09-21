﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Judge : MonoBehaviour
{
    // プランナーレベルデザイン用 ******************************************************
    // perfect ～ badの順に入力
    public static float[] gradeCriterion = { 1.0f, 1.5f, 2, 3 }; // 判定許容値
    public static int[] gradePoint = { 300, 200, 100, 10 };      // 各判定に応じたスコア
    // *********************************************************************************

    // 曲情報を参照
    public static int maxCombo = 300;
    public static int gameType = 0;// 二重鍵盤用
    public static List<List<GameObject>> GOListArray = new List<List<GameObject>>();// ノーツ座標格納用2次元配列
    //
    // 使い方  GOListArray   [_notesCount[laneNumber]]                   [laneNumber]
    //         GOListArray   [何個目のノーツなのか[何番目のレーンの]]    [何番目のレーンなのか]


    // リザルト用
    public static int totalScore = 0;                    // 合計スコア
    public static int combo = 0;                         // 現在のコンボ
    public static int bestcombo = 0;                     // プレイヤー最大コンボ
    public static int[] totalGrades = new int[5];        // 判定内訳（perfect ～ miss）


    // 内部用
    public static int point = 0;                         // 判定に応じた得点
    public static int[] keyNotesCount = new int[8];      // 二重鍵盤用ノーツカウント
    public static int[] stNotesCount = new int[6];       // バイオリン用ノーツカウント
    static ScoreManager mg1;
    static ComboManager mg2;

    static DrawGrade[] dg = new DrawGrade[8];
    public static GameObject[] drawGrade = new GameObject[8];
    void Start()
    {
        //初期化
        totalScore = 0;
        combo = 0;
        bestcombo = 0;

        for (int i = 0; i < totalGrades.Length; i++)
        {
            totalGrades[i] = 0;
        }

        for (int i = 0; i < keyNotesCount.Length; i++)
        {
            keyNotesCount[i] = 0;
        }

        for(int i = 0; i < stNotesCount.Length; i++)
        {
            stNotesCount[i] = 0;
        }

        // 関数を呼ぶためにスクリプトを取得
        GameObject uiObj = GameObject.Find("UICanvas");
        mg1 = uiObj.GetComponent<ScoreManager>();
        mg2 = uiObj.GetComponent<ComboManager>();

        // 評価UI表示用のスクリプト配列をセット
        for(int i = 0; i < drawGrade.Length; i++)
        {
            string callObject = "drawGrade" + i;

            drawGrade[i] = GameObject.Find(callObject);

            dg[i] = drawGrade[i].GetComponent<DrawGrade>();
        }
        // 使い方
        // dg[laneNumber].DrawGrades(grade);
    }

    public static void ListImport()
    {
        GOListArray = NotesManager.NotesPositions;
        Debug.Log(GOListArray[0][7]);
    }

    /// <summary>
    /// タップした場所に応じてレーン番号を返します
    /// </summary>
    /// <param name="i">GetTouch</param>
    /// <returns>laneNumber</returns>
    public static int GetLaneNumber(int i)
    {
        int laneNum = -1;// 例外処理用
        GameObject clickObj = null; // 都度初期化

        // タッチ情報を取得
        Touch t = Input.GetTouch(i);

        // タップ時処理
        Ray ray = Camera.main.ScreenPointToRay(t.position);
        RaycastHit2D hit = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction, 10f, 1);

        if (hit)
        {
            clickObj = hit.transform.gameObject;

            if ((clickObj != null) && (clickObj.tag == ("Lane")))// tagでレーンを識別
            {
                string s = clickObj.name;  // レーン番号を取得
                laneNum = int.Parse(s);    // 文字列を数字に変換
            }
        }

        return laneNum;
    }

    /// <summary>
    /// 判定ライン - ノーツ座標でタップしたタイミングの正確さを返します
    /// </summary>
    /// <param name="i">laneNumber</param>
    /// <param name="j">judgeLine</param>
    /// <returns>absTiming</returns>
    public static float GetAbsTiming(int i, float j)// 判定ライン　－　ノーツ
    {
        float tempTiming = 9999;// 初期化（0ではだめなので）

        switch (gameType)
        {
            // 二重鍵盤
            case 0:
                tempTiming = j - GOListArray[keyNotesCount[i]][i].transform.position.y;
                break;

            // ********ここに判定式を書け********
            case 1:
                if(j < 10)// バイオリン縦レーン
                {
                    tempTiming = j - GOListArray[stNotesCount[i]][i].transform.position.y;
                }
                else// バイオリン横レーン
                {
                    tempTiming = j - GOListArray[stNotesCount[i]][i].transform.position.x;
                }
                break;
         
            default:
                break;
        }

        return Mathf.Abs(tempTiming);// 絶対値に変換
    }
    /// <summary>
    /// 判定からノーツ破棄処理まで
    /// </summary>
    /// <param name="i">absTiming</param>
    /// <param name="j">laneNumber</param>
    public static void JudgeGrade(float i, int j)
    {
        // 判定分岐 perfect ～ bad
        if (i <= gradeCriterion[0])
        {
            point = gradePoint[0];
            combo++;
            totalGrades[0]++;

            dg[j].DrawGrades(0);
            SoundManager.SESoundCue(2);
        }
        else if (i <= gradeCriterion[1])
        {
            point = gradePoint[1];
            combo++;
            totalGrades[1]++;

            dg[j].DrawGrades(1);
            SoundManager.SESoundCue(2);
        }
        else if (i <= gradeCriterion[2])
        {
            point = gradePoint[2];
            combo = 0;
            totalGrades[2]++;

            dg[j].DrawGrades(2);
            SoundManager.SESoundCue(3);
        }
        else if (i <= gradeCriterion[3])
        {
            point = gradePoint[3];
            combo = 0;
            totalGrades[3]++;

            dg[j].DrawGrades(4);
            SoundManager.SESoundCue(4);
        }
        else// 空タップ
        {
            point = 0;
            SoundManager.SESoundCue(5);
        }

        if (combo > bestcombo)
        {
            bestcombo = combo;// 最大コンボ記憶
        }

        // 空タップでなければ
        if (point > 0)
        {
            totalScore += point;

            mg1.DrawScore(totalScore);
            mg2.DrawCombo(combo);

            NotesDestroy(j);
        }
    }

    /// <summary>
    /// ノーツ破棄、配列カウントアップ
    /// </summary>
    /// <param name="i">laneNumber</param>
    public static void NotesDestroy(int i)
    {
        switch (gameType)
        {
            case 0:
                Destroy(GOListArray[keyNotesCount[i]][i]);   // 該当ノーツ破棄
                GOListArray[keyNotesCount[i]][i] = null;     // 多重タップを防ぐ
                keyNotesCount[i]++;                          // 該当レーンのノーツカウント++
                break;

            case 1:
                Destroy(GOListArray[stNotesCount[i]][i]);
                GOListArray[stNotesCount[i]][i] = null;
                stNotesCount[i]++;
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// ノーツがスルーされた時の処理です
    /// </summary>
    /// <param name="i">laneNumber</param>
    public static void NotesCountUp(string i)
    {
        if (combo > bestcombo)
        {
            bestcombo = combo;// 最大コンボ記憶
        }

        combo = 0;
        totalGrades[4]++;

        mg2.DrawCombo(combo);
        int tempLaneNum = int.Parse(i);// 文字列を数字に変換

        dg[tempLaneNum].DrawGrades(4);

        NotesDestroy(tempLaneNum);
    }
}
