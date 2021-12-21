using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckBotButton : MonoBehaviour {
    public Sprite onImage;
    public Sprite offImage;

    private Sprite _currentImage;
    private static bool _isVsBot;

    public static bool IsVsBot => _isVsBot;

    void Start() {
        _currentImage = GetComponent<Image>().sprite;
    }

    public void SetButton() {
        _isVsBot = !_isVsBot;
        if (_isVsBot) _currentImage = onImage;
        else _currentImage = offImage;

        GetComponent<Image>().sprite = _currentImage;
    }
}
