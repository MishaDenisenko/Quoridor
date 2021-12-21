using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasButtons : MonoBehaviour {
    private Transform _buttonTransform;
    private bool _isVsBot;
    
    private void Awake() {
        _buttonTransform = transform;
    }

    // private void Update() {
    //     if (isStart) _isVsBot = 
    // }

    public void PlayGame() {
        _isVsBot = CheckBotButton.IsVsBot;
        if (_isVsBot) StartCoroutine(LoadScene("BotGameScene"));
        else StartCoroutine(LoadScene("GameScene"));
    }
    
    public void OpenHomeScene() {
        StartCoroutine(LoadScene("Lobby"));
    }
    
    public void SetPressedButton() {
        _buttonTransform.Rotate(0, 0, -5);
        if (SceneManager.GetActiveScene().name.Equals("Lobby")) _buttonTransform.localScale = new Vector3(0.8f, 0.8f, 1f);
        else if (SceneManager.GetActiveScene().name.Equals("GameScene"))
            _buttonTransform.localScale = new Vector3(0.6f, 0.6f, 1f);
    }

    public void SetDefaultButton() {
        _buttonTransform.Rotate(0, 0, 5);
        if (SceneManager.GetActiveScene().name.Equals("Lobby")) _buttonTransform.localScale = new Vector3(1f, 1f, 1f);
        else if (SceneManager.GetActiveScene().name.Equals("GameScene"))
            _buttonTransform.localScale = new Vector3(0.8f, 0.8f, 1f);
    }

    IEnumerator LoadScene(string sceneName) {
        float fadeTime = Camera.main.GetComponent<Fading>().Fade(1);
        yield return new WaitForSeconds(fadeTime);
        SceneManager.LoadScene(sceneName);
    }
}