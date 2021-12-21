using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using _Scripts.Game_Scripts;
using UnityEditor.SceneManagement;
using UnityEngine;
using Random = System.Random;

namespace _Scripts.Bot_Scene_Scripts {
    public class BotController : MonoBehaviour {
        public GameObject firstRowTop;
        public GameObject firstRowBottom;
        public GameObject tops;
        public Color playerColor;
        public GameObject firstPlatesStack;
        public GameObject secondPlatesStack;

        private Camera _mainCamera;
        private GameObject _activeVertex;
        private GameObject _previousVertex;
        private GameObject[] _firstRow = new GameObject[Constants.Size];
        private GameObject _platesStack;
        private GameObject[] _nearVertices;
        private GameObject[] _allVertices = new GameObject[81];
        private bool _firstMove;
        private bool _isOutlined;
        private bool _isOptimalCount = true;
        private bool _botActive;
        private bool _isOutlinedNearVertices;
        private bool _isDecayed;
        private bool _isCheckCount;
        
        private enum Action {
            MakeMove,
            MovePlate
        }
        
        private Action ChosenAction { get; set; }
        private void Awake() {
            
        }

        private void Start() {
            if (GetComponent<BotOrPlayer>().PlayerType != BotOrPlayer.Type.Bot) {
                Destroy(GetComponent<BotController>());
            } else {
                int size = Constants.Size;

                GameObject fr =
                    gameObject.GetComponent<CheckVertexController>().Player == CheckVertexController.PlayerInitial.First
                        ? firstRowTop
                        : firstRowBottom;

                _platesStack =
                    gameObject.GetComponent<CheckVertexController>().Player == CheckVertexController.PlayerInitial.First
                        ? firstPlatesStack
                        : secondPlatesStack;

                for (int i = 0; i < size; i++) {
                    _firstRow[i] = fr.transform.GetChild(i).gameObject;
                }
                _firstMove = true;
                _botActive = true;
                _mainCamera = Camera.main;
                int p = 0;
                for (int i = 0; i < tops.transform.childCount; i++) {
                    for (int k = 0; k < tops.transform.GetChild(i).childCount; k++) {
                        _allVertices[p] = tops.transform.GetChild(i).GetChild(k).gameObject;
                        p++;
                    }
                }
            }
        }

        private void Update() {
            if (MatchController.ActiveMove == BotOrPlayer.Type.Bot) {

                if (_firstMove) {
                    Random random = new Random();
                    Vector3 pos = _firstRow[random.Next(_firstRow.Length)].transform.position;
                    // Vector3 pos = _firstRow[4].transform.position;
                    transform.position = new Vector3(pos.x, Constants.PlayerPosY, pos.z);
                    _firstMove = false;
                    MatchController.ChangeMove();
                } else {
                    if (_isOptimalCount && _botActive) {
                        StartCoroutine(WaitForTime(2000));
                        ChosenAction = ChooseAction();
                        DoAction(ChosenAction);
                    } else if (!_isOptimalCount && _botActive) {
                        StartCoroutine(WaitForTime(2000));
                        DoAction(Action.MakeMove);
                    } 
                }
                // if (!_isCheckCount) {
                //     // _isOptimalCount = _platesStack.GetComponent<PlatesCountController>().IsOptimalCount;
                //     _isCheckCount = true;
                // }

                
            }
        }

        private void DoAction(Action action) {
            GetComponent<ShortestPath>().TryFind = true;
            Random random = new Random();
            switch (action) {
                case Action.MakeMove: 
                    _nearVertices = GetComponent<CheckVertexController>().GetNearVertices();
                    GameObject[] shortWay = GetComponent<ShortestPath>().FindShortest();
                    foreach (GameObject o in shortWay) {
                        o.GetComponent<Outline>().enabled = true;
                        o.GetComponent<Outline>().OutlineColor = Color.magenta;
                    }
                    print(shortWay.Length);
                    switch (random.Next(10)) {
                        case 0:
                        case 1:
                        case 2:
                        case 3:
                        case 4:
                        case 5:
                        case 6:
                        case 7:
                            foreach (GameObject vertex in _nearVertices) {
                                if (shortWay.Contains(vertex)) {
                                    GameObject newVertex = vertex;
                                    if (newVertex == _previousVertex) {
                                        _activeVertex = _nearVertices[random.Next(_nearVertices.Length) - 1];
                                    } else {
                                        _activeVertex = newVertex;
                                    }
                                }
                            }
                            break;
                        // case 8:
                        // case 9:
                        // case 10: _activeVertex = _nearVertices[random.Next(_nearVertices.Length) - 1]; break;
                    }
                    _previousVertex = _activeVertex;
                    Vector3 pos;
                    pos = _activeVertex.transform.position;
                    transform.position = new Vector3(pos.x, Constants.PlayerPosY, pos.z);
                    ActivePlayer();
                    break;
                
            }
        }
        
        public void ActivePlayer() {
            _isDecayed = false;
            _isOutlinedNearVertices = false;
            Thread.Sleep(200);
            GetComponent<CheckVertexController>().enabled = true;
            GetComponent<CheckVertexController>().PlayerMove = true;
            if (!_botActive) _botActive = true;
            MatchController.ChangeMove();
            _isOutlined = false;
            _firstMove = false;
            // _isOptimalCount = _platesStack.GetComponent<PlatesCountController>().IsOptimalCount;
        }

        private IEnumerator WaitForTime(int ms) {
            yield return new WaitForSeconds(ms);
        }

        private Action ChooseAction() {
            Random random = new Random();
            switch (random.Next(10)) {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5: return Action.MakeMove;
                case 6:
                case 7:
                case 8:
                case 9:
                case 10: return Action.MovePlate;
                default: return Action.MakeMove;
            }
        }
    }
}