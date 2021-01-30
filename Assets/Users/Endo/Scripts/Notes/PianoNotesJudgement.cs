﻿using System;
using UnityEngine;

/// <summary>
/// 二重鍵盤演奏において、ノーツのタップによる判定を行う
/// </summary>
public class PianoNotesJudgement : NotesJudgementBase
{
    [SerializeField]
    private GameObject leftJudgeLine; // 左判定ライン

    [SerializeField]
    private GameObject rightJudgeLine; // 右判定ライン

    [SerializeField]
    private GameObject[] laneBg = new GameObject[8]; // レーンタップ時の背景

    [SerializeField]
    private GameObject[] mask = new GameObject[8]; // ロングノーツ用マスク

    private static Vector3 _leftJudgeLinePos;  // 左レーンの位置
    private static Vector3 _rightJudgeLinePos; // 右レーンの位置

    protected override void Start()
    {
        base.Start();

        _leftJudgeLinePos  = leftJudgeLine.transform.position;
        _rightJudgeLinePos = rightJudgeLine.transform.position;

        for (int i = 0; i < maxLaneNum; i++)
        {
            laneBg[i].SetActive(false);
            mask[i].SetActive(false);
        }
    }

    protected override void EvaluateGrades(int laneNum, TimingGrade tapGrade)
    {
        switch (tapGrade)
        {
            case TimingGrade.Perfect:
                currentCombo++;
                SoundManager.SESoundCue(2);

                break;

            case TimingGrade.Great:
                currentCombo++;
                SoundManager.SESoundCue(2);

                break;

            case TimingGrade.Good:
                currentCombo = 0;
                SoundManager.SESoundCue(3);

                break;

            case TimingGrade.Bad:
                currentCombo = 0;
                SoundManager.SESoundCue(4);

                break;

            case TimingGrade.Miss:
                // ロングノーツ終点のミス判定
                if (isHold[laneNum])
                {
                    if (currentCombo > bestCombo)
                    {
                        bestCombo = currentCombo;
                    }

                    currentCombo = 0;
                    TotalGrades[4]++;
                    comboMgr.DrawCombo(currentCombo);
                    DestroyNotes(laneNum);
                }
                // 空タップ
                else
                {
                    SoundManager.SESoundCue(5);
                }

                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    protected override void JudgeNotesType(int laneNum, NotesType notesType, SlideNotesSection? slideSection)
    {
        // ロングノーツか判別
        if ((notesType       == NotesType.LongAndSlide) &&
            (isHold[laneNum] == false))
        {
            isHold[laneNum] = true; // ホールド開始
        }
        else
        {
            DestroyNotes(laneNum);
        }
    }

    protected override void UpdateNotesDisplay(bool[] tappedLane, bool[] lastTappedLane)
    {
        for (int laneNum = 0; laneNum < maxLaneNum; laneNum++)
        {
            float absTiming              = 9999; // 初期化（0ではだめなので）
            bool  isThisLaneTapped       = tappedLane[laneNum];
            bool  isThisLaneTappedInPrev = lastTappedLane[laneNum];

            // タップを全く行っていなければ判定しない
            if (!isThisLaneTapped && !isThisLaneTappedInPrev) continue;

            // レーン内のノーツのインデックス
            // FIXME: レーン内の最終ノーツの場合、そのままノーツカウントを渡すとインデックス範囲外になるため、暫定的に-1している
            int laneNotesCount = (GOListArray[laneNum].Count == notesCount[laneNum])
                                     ? notesCount[laneNum] - 1
                                     : notesCount[laneNum];

            (GameObject notesObj, NotesSelector notesSel) = GOListArray[laneNum][laneNotesCount];
            bool isNotesObjNull = notesObj == null;
            bool isLeftLane     = laneNum  <= 3;
            bool isRightLane    = laneNum  >= 4;

            switch (isThisLaneTappedInPrev)
            {
                // タップ継続
                case true when isThisLaneTapped:
                {
                    // ロングノーツホールド中、終点が通過してたら破棄
                    if (isHold[laneNum])
                    {
                        Vector3 endNotesPos = notesSel.endNotes.transform.position;

                        // 左レーン
                        if (isLeftLane &&
                            _leftJudgeLinePos.y - GradesCriterion[3] > endNotesPos.y)
                        {
                            NotesCountUp(laneNum);
                            isHold[laneNum] = false;
                        }
                        // 右レーン
                        else if (isRightLane &&
                                 _rightJudgeLinePos.y - GradesCriterion[3] > endNotesPos.y)
                        {
                            NotesCountUp(laneNum);
                            isHold[laneNum] = false;
                        }
                    }

                    break;
                }

                // タップ開始
                case false when isThisLaneTapped:
                {
                    if (!isNotesObjNull)
                    {
                        Vector3 notesPos = notesObj.transform.position;

                        if (isLeftLane)
                        {
                            absTiming = GetAbsTiming(notesPos.y, _leftJudgeLinePos.y);
                        }
                        else if (isRightLane)
                        {
                            absTiming = GetAbsTiming(notesPos.y, _rightJudgeLinePos.y);
                        }
                    }

                    // 距離に応じて判定処理
                    JudgeGrade(laneNum, absTiming);

                    laneBg[laneNum].SetActive(true);

                    break;
                }

                // タップ終了
                case true when !isThisLaneTapped:
                {
                    if (isHold[laneNum])
                    {
                        Vector3 endNotesPos = notesSel.endNotes.transform.position;

                        if (!isNotesObjNull && isLeftLane)
                        {
                            absTiming = GetAbsTiming(endNotesPos.y, _leftJudgeLinePos.y);
                        }
                        else if (!isNotesObjNull && isRightLane)
                        {
                            absTiming = GetAbsTiming(endNotesPos.y, _rightJudgeLinePos.y);
                        }

                        JudgeGrade(laneNum, absTiming);

                        isHold[laneNum] = false;
                    }

                    laneBg[laneNum].SetActive(false);

                    break;
                }
            }

            mask[laneNum].SetActive(isHold[laneNum]);
        }
    }
}
