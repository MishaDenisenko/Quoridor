using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class WinController : MonoBehaviour {
    public GameObject firstPlayer;
    public GameObject secondPlayer;

    public GameObject firstPlayerFinish;
    public GameObject secondPlayerFinish;

    public TMP_Text winText;
    public GameObject buttons;
    public GameObject playInfo;
    public float speed = 5f;

    private const int Length = 9;

    private Transform[] _finish1 = new Transform[Length];
    private Transform[] _finish2 = new Transform[Length];
    private Vector3 _vertexPos1;
    private Vector3 _vertexPos2;
    private Vector3 _firstPlayerPos;
    private Vector3 _secondPlayerPos;
    private Player _player;
    private string _info;

    private enum Player {
        First, Second
    }

    public static bool IsWin { get; set; }

    private void Start() {
        for (int i = 0; i < Length; i++) {
            _finish1[i] = firstPlayerFinish.transform.GetChild(i);
            _finish2[i] = secondPlayerFinish.transform.GetChild(i);
        }
    }

    private void Update() {
        _firstPlayerPos = firstPlayer.transform.position;
        _secondPlayerPos = secondPlayer.transform.position;
        if (!IsWin) {
            for (int i = 0; i < Length; i++) {
                _vertexPos1 = _finish1[i].position;
                _vertexPos2 = _finish2[i].position;

                if (_firstPlayerPos.x >= _vertexPos1.x - 0.01 && _firstPlayerPos.x <= _vertexPos1.x + 0.01 &&
                    _firstPlayerPos.z >= _vertexPos1.z - 0.01 && _firstPlayerPos.z <= _vertexPos1.z + 0.01) {
                    IsWin = true;
                    winText.text = GetInfo(Player.First);
                    break;
                }
                if (_secondPlayerPos.x >= _vertexPos2.x - 0.01 && _secondPlayerPos.x <= _vertexPos2.x + 0.01 &&
                    _secondPlayerPos.z >= _vertexPos2.z - 0.01 && _secondPlayerPos.z <= _vertexPos2.z + 0.01) {
                    IsWin = true;
                    winText.text = GetInfo(Player.Second);
                    break;
                }
            }
        }
        else  {
            if (winText.transform.localPosition.x >= 60) winText.transform.Translate(Vector3.down * Time.deltaTime * speed);
            if (buttons.transform.localPosition.x <= 0) buttons.transform.Translate(Vector3.right * Time.deltaTime * speed);
        }
    }

    private string GetInfo(Player player) {
        playInfo.SetActive(false);
        return $"{player} win!";
    }
}