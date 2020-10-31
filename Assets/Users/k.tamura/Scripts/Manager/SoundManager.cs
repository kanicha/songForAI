﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// サウンドマネージャー
/// singleton化済
/// </summary>
public class SoundManager:SingletonMonoBehaviour<SoundManager>
{
    [SerializeField]
    CriAtomSource BGMSource = null;
    [SerializeField]
    CriAtomSource SESource = null;
    /// <summary>
    /// 初期化
    /// </summary>
    private void Start()
    {
        DontDestroyOnLoad(this);
    }
    /// <summary>
    /// BGMを再生する関数 static化済
    /// </summary>
    /// <param name="cueID">cueID</param>
    public static void BGMSoundCue(int cueID)
    {
        Instance.BGMSource.Play(cueID);
    }
    /// <summary>
    /// SEを再生する関数 static化済
    /// </summary>
    /// <param name="cueID">cueID</param>
    public static void SESoundCue(int cueID)
    {
        Instance.SESource.Play(cueID);
    }
    /// <summary>
    /// BGMを停止する関数 static化済
    /// </summary>
    public static void BGMSoundStop()
    {
        Instance.BGMSource.Stop();
    }
    /// <summary>
    /// BGMの現在のステータスを取得する関数 static化済
    /// </summary>
    /// <returns></returns>
    public static CriAtomSource.Status BGMStatus()
    {
        return Instance.BGMSource.status;
    }
    public static void BgmTime(ref long times)
    {
        times = Instance.BGMSource.time;
    }
}
