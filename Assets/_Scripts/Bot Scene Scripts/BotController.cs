using System.Collections;
using System.Linq;
using System.Threading;
using UnityEngine;
using Random = System.Random;

namespace _Scripts.Bot_Scene_Scripts {
    public class BotController : MonoBehaviour {
        public GameObject firstRowTop;
        public GameObject firstRowBottom;
        public GameObject tops;
        public GameObject firstPlatesStack;
        public GameObject secondPlatesStack;
        public GameObject opponent;
        public GameObject platePrefab;
        
        private GameObject _activeVertex;
        private GameObject _previousVertex;
        private GameObject[] _firstRow = new GameObject[Constants.Size];
        private GameObject _platesStack;
        private GameObject[] _nearVertices;
        private GameObject[] _allVertices = new GameObject[81];
        private GameObject[] _shortWay;
        private bool _firstMove;
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
            if (MatchController.ActiveMove == BotOrPlayer.Type.Bot && !WinControllerBP.IsWin) {

                if (_firstMove) {
                    StartCoroutine(WaitForTime(10000));
                    Random random = new Random();
                    _activeVertex = _firstRow[random.Next(_firstRow.Length)];
                    Vector3 pos = _activeVertex.transform.position;
                    transform.position = new Vector3(pos.x, Constants.PlayerPosY, pos.z);
                    _firstMove = false;
                    _previousVertex = _activeVertex;
                    MatchController.ChangeMove();
                } else {
                    foreach (GameObject vertex in _allVertices) {
                        if (vertex) vertex.GetComponent<Outline>().enabled = false;
                    }
                    
                    if (_isOptimalCount && _botActive) {
                        StartCoroutine(WaitForTime(2000));
                        ChosenAction = ChooseAction();
                        DoAction(ChosenAction);
                        
                    } else if (!_isOptimalCount && _botActive) {
                        StartCoroutine(WaitForTime(2000));
                        DoAction(Action.MakeMove);
                    } 
                }
            }
        }

        private void DoAction(Action action) {
            // GetComponent<ShortestPath>().TryFind = true;
            Random random = new Random();
            switch (action) {
                case Action.MakeMove: 
                    _nearVertices = GetComponent<CheckVertexController>().GetNearVertices();
                    _shortWay = GetComponent<ShortestPath>().FindShortest();
                    foreach (GameObject vertex in _nearVertices) {
                        if (_shortWay.Contains(vertex)) {
                            GameObject newVertex = vertex;
                            if (newVertex.Equals(_previousVertex)) {
                                _previousVertex = _activeVertex;
                                int c = random.Next(_nearVertices.Length);
                                while (_nearVertices[c] == null) {
                                    c = random.Next(_nearVertices.Length);
                                }
                                _activeVertex = _nearVertices[c];
                                break;
                            }
                            _previousVertex = _activeVertex;
                            _activeVertex = newVertex;
                            break;
                        }
                    }
                    Vector3 pos;
                    pos = _activeVertex.transform.position;
                    transform.position = new Vector3(pos.x, Constants.PlayerPosY, pos.z);
                    ActivePlayer();
                    break;
                
                case Action.MovePlate:
                    GameObject[] positions = opponent.GetComponent<FindNearPlatesPos>().NearPlatesPos();
                    bool isTruePos = false;

                    GameObject newPlate = null;
                    for (int i = 0; i < positions.Length; i++) {
                        int index = random.Next(positions.Length);
                        Vector3 platePosition = positions[index].transform.position;
                        Quaternion plateRotation = positions[index].transform.rotation * Quaternion.Euler(270, 0, 0);
                        newPlate = Instantiate(platePrefab, platePosition + Vector3.up*0.15f, plateRotation);
                        Color color = newPlate.GetComponentInChildren<Renderer>().material.color;
                        newPlate.transform.GetChild(0).GetComponent<Renderer>().material.color = new Color(color.r, color.g, color.b, 0.5f);
                        newPlate.GetComponent<BoxCollider>().enabled = false;
                        Vector3 newPlatePos = newPlate.transform.position;
                        Ray rayc = new Ray(new Vector3(newPlatePos.x, newPlatePos.y + 1, newPlatePos.z),
                            -newPlate.transform.up);
                        Ray rayl = new Ray(new Vector3(newPlatePos.x + 1, newPlatePos.y, newPlatePos.z),
                            -newPlate.transform.forward);
                        Ray rayr = new Ray(new Vector3(newPlatePos.x - 1, newPlatePos.y, newPlatePos.z),
                            -newPlate.transform.forward);
                        if (newPlate.transform.rotation.eulerAngles.y >= 180) {
                            newPlate.transform.localScale = new Vector3(1, 1, -1);
                            rayl = new Ray(new Vector3(newPlatePos.x, newPlatePos.y, newPlatePos.z + 1),
                                -newPlate.transform.forward);
                            rayr = new Ray(new Vector3(newPlatePos.x, newPlatePos.y, newPlatePos.z - 1),
                                -newPlate.transform.forward);
                        }
                        RaycastHit hit;
                        int c = 3;
                        if (Physics.Raycast(rayc, out hit, 5f) && hit.collider.gameObject.tag.Equals("Plate")) c--;
                        if (Physics.Raycast(rayl, out hit, 5f) && hit.collider.gameObject.tag.Equals("Plate")) c--;
                        if (Physics.Raycast(rayr, out hit, 5f) && hit.collider.gameObject.tag.Equals("Plate")) c--;
                        if (c == 3) {
                            opponent.GetComponent<PathFinder>().TryFind = true;
                            isTruePos = opponent.GetComponent<PathFinder>().IsWay;
                        }
                        if (isTruePos) break;
                        Destroy(newPlate);
                    }

                    if (isTruePos && newPlate) {
                        Color color = newPlate.GetComponentInChildren<Renderer>().material.color;
                        newPlate.transform.GetChild(0).GetComponent<Renderer>().material.color =
                        new Color(color.r, color.g, color.b, 1f);
                        newPlate.GetComponent<BoxCollider>().enabled = true;
                        newPlate.GetComponent<BotPlateIntesection>().enabled = false;
                        ActivePlayer();
                    } else {
                        DoAction(Action.MakeMove);
                    }
                    break;
            }
        }
        
        public void ActivePlayer() {
            Thread.Sleep(200);
            GetComponent<CheckVertexController>().PlayerMove = true;
            _botActive = true;
            _firstMove = false;
            _isOptimalCount = _platesStack.GetComponent<PlatesCountController>().IsOptimalCount;
            MatchController.ChangeMove();
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
                case 5: 
                case 6: return Action.MakeMove;
                case 7:
                case 8:
                case 9: return Action.MovePlate;
                default: return Action.MakeMove;
            }
        }
    }
}