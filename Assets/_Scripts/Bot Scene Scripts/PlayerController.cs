using System;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace _Scripts.Bot_Scene_Scripts {
    public class PlayerController : MonoBehaviour {
        public GameObject firstRowTop;
        public GameObject firstRowBottom;
        public GameObject tops;
        public Color playerColor;
        public GameObject firstPlatesStack;
        public GameObject secondPlatesStack;
        public static bool PlateInstalled { get; set; }

        private Camera _mainCamera;
        private GameObject _activeVertex;
        private GameObject[] _firstRow = new GameObject[Constants.Size];
        private GameObject _platesStack;
        private GameObject[] _nearVertices;
        private GameObject[] _allVertices = new GameObject[81];
        private bool _firstMove;
        private bool _isOutlined;
        private bool _isOptimalCount = true;
        private bool _playerActive;
        private bool _isOutlinedNearVertices;
        private bool _isDecayed;

        private void Awake() {
            
        }

        private void Start() {
            if (GetComponent<BotOrPlayer>().PlayerType != BotOrPlayer.Type.Player) Destroy(GetComponent<PlayerController>());
            else {
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
                _playerActive = true;
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
            // print(MatchController.ActiveMove);
            if (MatchController.ActiveMove == BotOrPlayer.Type.Player) {
                if (PlateInstalled) {
                    MatchController.ChangeMove();
                }
                
                if (_firstMove && _playerActive) {
                    _playerActive = false;
                    OutlineObjects(_firstRow);
                } 
                else if (_playerActive) {
                    if (!_isOutlined) {
                        foreach (GameObject vertex in _allVertices) {
                            if (vertex) vertex.GetComponent<Outline>().enabled = false;
                        }
                        _nearVertices = GetComponent<CheckVertexController>().GetNearVertices();
                        // foreach (GameObject nearVertex in _nearVertices) {
                        //     print(nearVertex);
                        // }
                        OutlineObjects(_nearVertices);
                        _isOutlined = true;
                    }
                }

                if (Input.GetMouseButtonDown(0) && !MovePlates.MovePlate) {
                    if (_firstMove) CreateSecondReaycast(_firstRow);
                    else CreateSecondReaycast(_nearVertices);
                }
                
                // if (_activeVertex && !_isDecayed) {
                //     bool activeVertexOutlined = _activeVertex.GetComponent<Outline>().enabled;
                //
                //     if (!activeVertexOutlined) {
                //         _activeVertex.GetComponent<Outline>().enabled = true;
                //         _activeVertex.GetComponent<Outline>().OutlineWidth = 20;
                //     } else if (_activeVertex.GetComponent<Outline>().OutlineWidth < Constants.DacayRate / 5) {
                //         _activeVertex.GetComponent<Outline>().enabled = false;
                //         _isDecayed = true;
                //     } else {
                //         print(_activeVertex.GetComponent<Outline>().OutlineWidth);
                //         _activeVertex.GetComponent<Outline>().OutlineWidth -= Constants.DacayRate * Time.deltaTime;
                //     }
                // }
            }
        }

        private void CreateSecondReaycast(GameObject[] vertices) {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
        
            if (Physics.Raycast(ray, out hit, 100) && hit.collider.gameObject == _platesStack) 
                MovePlate(_isOptimalCount, hit.transform.position);
            else {
                if (vertices.Contains(hit.collider.gameObject)) MakeAMove(hit.collider.gameObject);
            }
        }

        private void OutlineObjects(GameObject[] objects) {
            GetComponent<Outline>().enabled = true;
            _platesStack.GetComponent<Outline>().enabled = true;
            foreach (GameObject o in objects) {
                if (o) o.GetComponent<Outline>().enabled = true;
            }
        }
        
        private void MovePlate(bool isOptimalCount, Vector3 position) {
            if (isOptimalCount) {
                MovePlates.BufflePosition = position;
                MovePlates.MovePlate = true;
                
            }
        }
        
        private void MakeAMove(GameObject vertex) {
        
            Vector3 hitPos;
            
            foreach (GameObject v in _allVertices) {
                v.GetComponent<Outline>().enabled = false;
            }
            _activeVertex = vertex;
            hitPos = _activeVertex.transform.position;
            float playerPosY = Constants.PlayerPosY;
            transform.position = new Vector3(hitPos.x, playerPosY, hitPos.z);
            ActivePlayer();
        }

        public void ActivePlayer() {
            _isDecayed = false;
            _isOutlinedNearVertices = false;
            Thread.Sleep(200);
            GetComponent<CheckVertexController>().enabled = true;
            GetComponent<CheckVertexController>().PlayerMove = true;
            if (!_playerActive) _playerActive = true;
            MatchController.ChangeMove();
            _isOutlined = false;
            _firstMove = false;
            _isOptimalCount = _platesStack.GetComponent<PlatesCountController>().IsOptimalCount;
        }
    }
}