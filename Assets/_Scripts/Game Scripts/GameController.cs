using System;
using UnityEngine;

public class GameController : MonoBehaviour {
    public GameObject player1;
    public GameObject player2;
    public GameObject tops;
    public GameObject bufflePlates1;
    public GameObject bufflePlates2;
    public float playerPosY = 0.145f;
    public float decayRate = 0.5f;

    private static bool _plateInstalled;

    public static bool PlateInstalled {
        get => _plateInstalled;
        set => _plateInstalled = value;
    }

    private GameObject[] _firstRowPlayer1 = new GameObject[9];
    private GameObject[] _firstRowPlayer2 = new GameObject[9];
    private GameObject[] _allVertices = new GameObject[81];
    private GameObject[] _outlinedObjects = new GameObject[85];
    private GameObject[] _nearVertices = new GameObject[4];
    private GameObject _previousInteractable;
    private GameObject _activeVertex;

    private bool _firstMoveP1;
    private bool _firstMoveP2;
    private bool _mouseOnObject;
    private bool _firstPlayerActive = true;
    private bool _secondPlayerActive;
    private bool _isOutlinedNearVertices;
    private bool _isDecayed;

    private int _player;
    private int[] _playerPosition = new int[2];
    private int[] _enemyPosition = new int[2];

    private Camera _mainCamera;

    private void Awake() {
        _mainCamera = Camera.main;
        _firstMoveP1 = true;
        _firstMoveP2 = true;

        _player = 1;

        for (int i = 0; i < tops.transform.GetChild(0).childCount; i++) {
            _firstRowPlayer1[i] = tops.transform.GetChild(0).GetChild(i).gameObject;
        }
        for (int i = 0; i < tops.transform.GetChild(8).childCount; i++) {
            _firstRowPlayer2[i] = tops.transform.GetChild(8).GetChild(i).gameObject;
        }
        int p = 0;
        for (int i = 0; i < tops.transform.childCount; i++) {
            for (int k = 0; k < tops.transform.GetChild(i).childCount; k++) {
                _allVertices[p] = tops.transform.GetChild(i).GetChild(k).gameObject;
                p++;
            }
        }
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int j = 0;

        foreach (GameObject gObject in allObjects) {
            if (gObject.layer == 11) {
                _outlinedObjects[j] = gObject;
                j++;
            }
        }
    }

    private void Update() {
        if (_plateInstalled && _player == 1) {
            ActivePlayer(Player.SecondPlayer);
            _plateInstalled = false;
        } else if (_plateInstalled && _player == 2) {
            ActivePlayer(Player.FirstPlayer);
            _plateInstalled = false;
        }

        if (_firstMoveP1 && _player == 1 && _firstPlayerActive) {
            FirstOutlineObjects(Player.FirstPlayer);
            _firstPlayerActive = false;
        } else if (_firstMoveP2 && _player == 2 && _secondPlayerActive) {
            FirstOutlineObjects(Player.SecondPlayer);
            _secondPlayerActive = false;
        } else if (!_firstMoveP1 && _player == 1 && _firstPlayerActive) {
            _nearVertices = NearVertices.GetNearVertices();
            _firstPlayerActive = false;
        } else if (!_firstMoveP2 && _player == 2 && _secondPlayerActive) {
            _nearVertices = NearVertices.GetNearVertices();
            _secondPlayerActive = false;
        }
        if (!_firstMoveP1 && !_firstMoveP2 && !_isOutlinedNearVertices) {
            foreach (GameObject vertex in _allVertices) {
                if (vertex) vertex.GetComponent<Outline>().enabled = false;
            }
            foreach (GameObject nearVertex in _nearVertices) {
                if (nearVertex) nearVertex.GetComponent<Outline>().enabled = true;
            }
            _isOutlinedNearVertices = true;
        }
        
        if (_activeVertex && !_isDecayed) {
            if (!_activeVertex.GetComponent<Outline>().enabled) {
                _activeVertex.GetComponent<Outline>().enabled = true;
                _activeVertex.GetComponent<Outline>().OutlineWidth = 20;
            }
            else if (_activeVertex.GetComponent<Outline>().enabled && _activeVertex.GetComponent<Outline>().OutlineWidth < decayRate/5) {
                _activeVertex.GetComponent<Outline>().enabled = false;
                _isDecayed = true;
            } else _activeVertex.GetComponent<Outline>().OutlineWidth -= decayRate * Time.deltaTime;
        }

        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100) && !MovePlates.MovePlate) {
            GameObject objectInFocus = hit.collider.gameObject;
            GameObject[] currentOutlinedObjects = new GameObject[11];
            switch (_player) {
                case 1:
                    currentOutlinedObjects = CurrentOutlinedObjects(Player.FirstPlayer, _firstMoveP1);
                    break;
                case 2:
                    currentOutlinedObjects = CurrentOutlinedObjects(Player.SecondPlayer, _firstMoveP2);
                    break;
            }
            ChangeOutline(currentOutlinedObjects, objectInFocus);
        }

        if (Input.GetMouseButtonDown(0)) {
            
            
            if (_player == 1 && _firstMoveP1) CreateSecondReaycast(Player.FirstPlayer, _firstRowPlayer1);
            else if (_player == 2 && _firstMoveP2) CreateSecondReaycast(Player.SecondPlayer, _firstRowPlayer2); 
            else {
                if (_player == 1) CreateSecondReaycast(Player.FirstPlayer, _nearVertices);
                else if (_player == 2) CreateSecondReaycast(Player.SecondPlayer, _nearVertices);
            }
        }
    }

    private void CreateSecondReaycast(Player player, GameObject[] vertices) {
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, 100)) {
            switch (player) {
                case Player.FirstPlayer:
                    if (hit.collider.gameObject.name.Equals("BufflePlates1")) MovePlate(Player.SecondPlayer, hit.transform.position);
                    else {
                        foreach (GameObject obj in vertices) {
                            if (hit.collider.gameObject == obj) {
                                MakeAMove(Player.FirstPlayer, hit);
                                break;
                            }
                        }
                    }
                    
                    break;
                case Player.SecondPlayer:
                    if (hit.collider.gameObject.name.Equals("BufflePlates2")) MovePlate(Player.FirstPlayer, hit.transform.position);
                    else {
                        foreach (GameObject obj in vertices) {
                            if (hit.collider.gameObject == obj) {
                                MakeAMove(Player.SecondPlayer, hit);
                                break;
                            }
                        }
                    }
                    
                    break;
            }
        }
    }

    private void MovePlate(Player player, Vector3 position) {
        MovePlates.BufflePosition = position;
        MovePlates.MovePlate = true;
    }

    private void MakeAMove(Player player, RaycastHit hit) {
        
        Vector3 hitPos;
        switch (player) {
            case Player.FirstPlayer:
                foreach (GameObject vertex in _allVertices) {
                    vertex.GetComponent<Outline>().enabled = false;
                }
                _activeVertex = hit.collider.gameObject;
                hitPos = _activeVertex.transform.position;
                player1.transform.position = new Vector3(hitPos.x, playerPosY, hitPos.z);
                NearVertices.EnemyPosition = CheckPlayerPosition(Player.FirstPlayer);
                NearVertices.PlayerPosition = CheckPlayerPosition(Player.SecondPlayer);
                ActivePlayer(Player.SecondPlayer);
                if (_firstMoveP1) _firstMoveP1 = false;
                break;
            case Player.SecondPlayer:
                foreach (GameObject vertex in _allVertices) {
                    vertex.GetComponent<Outline>().enabled = false;
                }
                _activeVertex = hit.collider.gameObject;
                hitPos = _activeVertex.transform.position;
                player2.transform.position = new Vector3(hitPos.x, playerPosY, hitPos.z);
                NearVertices.EnemyPosition = CheckPlayerPosition(Player.SecondPlayer);
                NearVertices.PlayerPosition = CheckPlayerPosition(Player.FirstPlayer);
                ActivePlayer(Player.FirstPlayer);
                if (_firstMoveP2) _firstMoveP2 = false;
                break;
        }

    }

    public void ActivePlayer(Player player) {
        _isDecayed = false;
        _isOutlinedNearVertices = false;
        switch (player) {
            case Player.FirstPlayer:
                _player = 1;
                player1.GetComponent<Outline>().enabled = true;
                player2.GetComponent<Outline>().enabled = false;
                bufflePlates1.GetComponent<Outline>().enabled = true;
                bufflePlates2.GetComponent<Outline>().enabled = false;
                _firstPlayerActive = true; 
                _secondPlayerActive = false; 
                break;
            case Player.SecondPlayer: 
                _player = 2;
                player1.GetComponent<Outline>().enabled = false;
                player2.GetComponent<Outline>().enabled = true;
                bufflePlates1.GetComponent<Outline>().enabled = false;
                bufflePlates2.GetComponent<Outline>().enabled = true;
                _secondPlayerActive = true; 
                _firstPlayerActive = false; 
                break;
        }
    }
    
    private void ChangeOutline(GameObject[] objets, GameObject objectInFocus) {
        if (objectInFocus) {
            foreach (GameObject obj in objets) {
                if (obj) {
                    if (obj == objectInFocus && objectInFocus.tag.Equals("Player"))
                        obj.GetComponent<Outline>().OutlineWidth = 15;
                    else if (obj == objectInFocus && !objectInFocus.tag.Equals("Player"))
                        obj.GetComponent<Outline>().OutlineWidth = 20;
                    else obj.GetComponent<Outline>().OutlineWidth = 10;
                }
            }
        } else {
            foreach (GameObject obj in objets) {
                if (obj.GetComponent<Outline>()) obj.GetComponent<Outline>().OutlineWidth = 10;
            }
        }
    }

    private void FirstOutlineObjects(Player player) {
        switch (player) {
            case Player.FirstPlayer:
                player1.GetComponent<Outline>().enabled = true;
                bufflePlates1.GetComponent<Outline>().enabled = true;
                foreach (GameObject obj in _firstRowPlayer1) {
                    if (!obj.GetComponent<Outline>().enabled) obj.GetComponent<Outline>().enabled = true;
                }
                foreach (GameObject obj in _firstRowPlayer2) {
                    if (obj.GetComponent<Outline>().enabled) obj.GetComponent<Outline>().enabled = false;
                }
                break;
            case Player.SecondPlayer:
                player1.GetComponent<Outline>().enabled = false;
                player2.GetComponent<Outline>().enabled = true;
                bufflePlates1.GetComponent<Outline>().enabled = false;
                bufflePlates2.GetComponent<Outline>().enabled = true;
                foreach (GameObject obj in _firstRowPlayer1) {
                    if (obj.GetComponent<Outline>().enabled) obj.GetComponent<Outline>().enabled = false;
                }
                foreach (GameObject obj in _firstRowPlayer2) {
                    if (!obj.GetComponent<Outline>().enabled) obj.GetComponent<Outline>().enabled = true;
                }
                break;
        }
    }

    private int[] CheckPlayerPosition(Player player) {
        int[] position = new int[2];
        Vector3 playerPos;
        Vector3 vertexPos;
        
        switch (player) {
            case Player.FirstPlayer:
                playerPos = player1.transform.position;
                for (int i = 0; i < 9; i++) {
                    for (int j = 0; j < 9; j++) {
                        vertexPos = tops.transform.GetChild(i).GetChild(j).position;
                        if (playerPos.x >= vertexPos.x - 0.01 && playerPos.x <= vertexPos.x + 0.01 &&
                            playerPos.z >= vertexPos.z - 0.01 && playerPos.z <= vertexPos.z + 0.01) {
                            String nameOfVertex = tops.transform.GetChild(i).GetChild(j).name.Substring(7);
                            position[0] = Int32.Parse(nameOfVertex[0].ToString());
                            position[1] = Int32.Parse(nameOfVertex[1].ToString());
                        }
                    }
                }
                break;
            case Player.SecondPlayer:
                playerPos = player2.transform.position;
                for (int i = 0; i < 9; i++) {
                    for (int j = 0; j < 9; j++) {
                        vertexPos = tops.transform.GetChild(i).GetChild(j).position;
                        if (playerPos.x >= vertexPos.x - 0.01 && playerPos.x <= vertexPos.x + 0.01 &&
                            playerPos.z >= vertexPos.z - 0.01 && playerPos.z <= vertexPos.z + 0.01) {
                            String nameOfVertex = tops.transform.GetChild(i).GetChild(j).name.Substring(7);
                            position[0] = Int32.Parse(nameOfVertex[0].ToString());
                            position[1] = Int32.Parse(nameOfVertex[1].ToString());
                        }
                    }
                }
                break;
        }
        return position;
    }

    private GameObject[] CurrentOutlinedObjects(Player player, bool firstMove) {
        int countOfObjects;

        if (firstMove) countOfObjects = 11;
        else countOfObjects = 2 + _nearVertices.Length;

        GameObject[] objects = new GameObject[countOfObjects];

        if (firstMove) {
            switch (player) {
                case Player.FirstPlayer:
                    objects[0] = player1;
                    objects[1] = bufflePlates1;
                    for (int i = 0; i < _firstRowPlayer1.Length; i++) {
                        objects[i + 2] = _firstRowPlayer1[i];
                    }
                    break;
                case Player.SecondPlayer:
                    objects[0] = player2;
                    objects[1] = bufflePlates2;
                    for (int i = 0; i < _firstRowPlayer2.Length; i++) {
                        objects[i + 2] = _firstRowPlayer2[i];
                    }
                    break;
            }
        } else {
            switch (player) {
                case Player.FirstPlayer:
                    objects[0] = player1;
                    objects[1] = bufflePlates1;
                    break;
                case Player.SecondPlayer:
                    objects[0] = player2;
                    objects[1] = bufflePlates2;
                    break;
            }
            for (int i = 0; i < _nearVertices.Length; i++) {
                objects[i + 2] = _nearVertices[i];
            }
        }

        return objects;
    }

    public enum Player {
        FirstPlayer, SecondPlayer
    }
}