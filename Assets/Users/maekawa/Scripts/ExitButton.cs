﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitButton : MonoBehaviour
{
    public void Onclick()
    {
        int life = SelectMusicScene.life;//PlayerPrefs.GetInt("Lifes", 3);

        if (life == 0)
        {
            //PlayerPrefs.SetInt("Lifes", 3);
            SelectMusicScene.life = 3;
            SceneLoadManager.LoadScene("PlayEnd");
        }
        else
        {
            //PlayerPrefs.SetInt("Lifes", life--);
            SelectMusicScene.life--;
            SceneLoadManager.LoadScene("SelectMusicV3");
        }
    }
}
