﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rhithm : MonoBehaviour
{
    bool _isTaped = false;
    bool _faded = false;
    // Start is called before the first frame update
    [SerializeField]
    float Times;
    [SerializeField]
    GameObject StartImage;
    [SerializeField]
    GameObject NotesGen;
    Color _StartImageColor;
    void Start()
    {
        _isTaped = false;
        _StartImageColor = StartImage.GetComponent<Image>().color;
    }
    public void ReturnHome()
    {
        if (!_isTaped)
        {
            _isTaped = true;
            SceneLoadManager.LoadScene("MockHome");
        }
    }
    private void FixedUpdate()
    {
        Times += Time.deltaTime;
        if (Times > 5 && !_faded)
        {
            _StartImageColor.a -= 0.05f;
            StartImage.GetComponent<Image>().color = _StartImageColor;
            if (StartImage.GetComponent<Image>().color.a <= 0)
            {
                _faded = true;
                NotesGen.GetComponent<NotesGenerater>().ButtonPush();
            }
        }
    }
    private void Update()
    {

    }
}
