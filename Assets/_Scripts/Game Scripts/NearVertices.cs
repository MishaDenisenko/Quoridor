using System;
using UnityEngine;

public class NearVertices : MonoBehaviour {
    /*private static int[] _enemyPosition = new int[2];
    private static int[] _playerPosition = new int[2];

    private static float _height = 0.2f;
    private static float rayLength = 0.5f;

    private static Vector3 _forward;
    private static Vector3 _right;
    private static Vector3 _left;
    private static Vector3 _back;

    public static int[] EnemyPosition {
        get => _enemyPosition;
        set => _enemyPosition = value;
    }

    public static int[] PlayerPosition {
        get => _playerPosition;
        set => _playerPosition = value;
    }

    public static GameObject[] GetNearVertices(GameObject player, GameController.Player pl) {
        GameObject[] nearVertices = new GameObject[5];

        int[][] verticesNumbers = new int[5][];
        for (int i = 0; i < verticesNumbers.Length; i++) {
            verticesNumbers[i] = new int[2];
        }
        
        Vector3 position = player.transform.position;
        Vector3 rayPosition = new Vector3(position.x, position.y + _height, position.z);
        verticesNumbers = CheckVertex(rayPosition, player.transform, _playerPosition, pl);

        for (int i = 0; i < nearVertices.Length; i++) {
            nearVertices[i] = GameObject.Find($"Vertex {verticesNumbers[i][0]}{verticesNumbers[i][1]}");
        }

        return nearVertices;
    }

    private static int[][] CheckVertex(Vector3 rayPosition, Transform trns, int[] positionXY,
        GameController.Player player) {
        _forward = trns.forward;
        _right = trns.right;
        _left = -_right;
        _back = -_forward;

        int[][] nearVertices = new int[5][];
        for (int i = 0; i < nearVertices.Length; i++) {
            nearVertices[i] = new int[2];
        }


        switch (player) {
            case GameController.Player.FirstPlayer:
                nearVertices[0][0] = positionXY[0] + 1; // forward
                nearVertices[0][1] = positionXY[1];

                nearVertices[1][0] = positionXY[0] - 1; // back
                nearVertices[1][1] = positionXY[1];

                nearVertices[2][0] = positionXY[0];
                nearVertices[2][1] = positionXY[1] - 1; // right

                nearVertices[3][0] = positionXY[0];
                nearVertices[3][1] = positionXY[1] + 1; // left
                break;
            case GameController.Player.SecondPlayer:
                nearVertices[0][0] = positionXY[0] - 1; // forward
                nearVertices[0][1] = positionXY[1];

                nearVertices[1][0] = positionXY[0] + 1; // back
                nearVertices[1][1] = positionXY[1];

                nearVertices[2][0] = positionXY[0];
                nearVertices[2][1] = positionXY[1] + 1; // right

                nearVertices[3][0] = positionXY[0];
                nearVertices[3][1] = positionXY[1] - 1; // left
                break;
        }


        Ray ray1 = new Ray(rayPosition, _forward);
        Ray ray2 = new Ray(rayPosition, _back);
        Ray ray3 = new Ray(rayPosition, _right);
        Ray ray4 = new Ray(rayPosition, _left);
        RaycastHit hit;

        if (Physics.Raycast(ray1, out hit, rayLength)) {
            if (hit.transform.tag.Equals("Plate")) {
                nearVertices[0][0] = 0;
                nearVertices[0][1] = 0;
            } else if (hit.transform.tag.Equals("Player")) {
                GetSecondCheck(ref nearVertices, positionXY, hit.transform.position, _forward, player);
            }
        }
        if (Physics.Raycast(ray2, out hit, rayLength)) {
            if (hit.transform.tag.Equals("Plate")) {
                nearVertices[1][0] = 0;
                nearVertices[1][1] = 0;
            }
        }
        if (Physics.Raycast(ray3, out hit, rayLength)) {
            if (hit.transform.tag.Equals("Plate")) {
                nearVertices[2][0] = 0;
                nearVertices[2][1] = 0;
            }
        }
        if (Physics.Raycast(ray4, out hit, rayLength)) {
            if (hit.transform.tag.Equals("Plate")) {
                nearVertices[3][0] = 0;
                nearVertices[3][1] = 0;
            }
        }
        return nearVertices;
    }

    private static void GetSecondCheck(ref int[][] nearVertices, int[] position, Vector3 rayPosition, Vector3 direction,
        GameController.Player player) {
        RaycastHit hit;
        if (direction == _forward) {
            Ray ray = new Ray(new Vector3(rayPosition.x, rayPosition.y + _height, rayPosition.z), direction);

            switch (player) {
                case GameController.Player.FirstPlayer:
                    nearVertices[0][0] += 1;
                    break;
                case GameController.Player.SecondPlayer:
                    nearVertices[0][0] -= 2;
                    break;
            }
            if (Physics.Raycast(ray, out hit, rayLength)) {
                if (hit.transform.tag.Equals("Plate")) {
                    nearVertices[4][0] = position[0] + 1;
                    nearVertices[4][0] = position[1] + 1;
                    nearVertices[0][0] = position[0] + 1;
                    nearVertices[0][1] = position[1] - 1;
                    RaycastHit hit2;
                    
                    Ray ray2 = new Ray(new Vector3(rayPosition.x, rayPosition.y + _height, rayPosition.z), direction);
                    Ray ray3 = new Ray(new Vector3(rayPosition.x, rayPosition.y + _height, rayPosition.z), direction);

                    if (Physics.Raycast(ray2, out hit2, rayLength)) {
                        if (hit2.transform.tag.Equals("Plate")) {
                            nearVertices[0][0] = 0;
                            nearVertices[0][0] = 0;
                        }
                    }

                    if (Physics.Raycast(ray3, out hit2, rayLength)) {
                        if (hit2.transform.tag.Equals("Plate")) {
                            nearVertices[4][0] = 0;
                            nearVertices[4][0] = 0;
                        }
                    }
                }
            }
        }
    }

    private static void EnemyNear(ref GameObject[] nearVertices) {
        int countOfVerteces = nearVertices.Length;
        int[][] nearPosition = new int[countOfVerteces][];
        for (int i = 0; i < countOfVerteces; i++) {
            nearPosition[i] = new int[2];
        }

        for (int i = 0; i < nearVertices.Length; i++) {
            String nameOfVertex = nearVertices[i].name.Substring(7);
            nearPosition[i][0] = Int32.Parse(nameOfVertex[0].ToString());
            nearPosition[i][1] = Int32.Parse(nameOfVertex[1].ToString());
        }

        bool identity;
        for (int i = 0; i < nearPosition.Length; i++) {
            if (_enemyPosition[0] == nearPosition[i][0] && _enemyPosition[1] == nearPosition[i][1]) identity = true;
            else identity = false;

            if (identity && _enemyPosition[1] == _playerPosition[1]) {
                if (_enemyPosition[0] == _playerPosition[0] + 1 && _playerPosition[0] != 8) {
                    nearPosition[i][0] = _playerPosition[0] + 2;
                } else if (_enemyPosition[0] == _playerPosition[0] + 1 && _playerPosition[0] == 8) {
                    nearPosition[i][0] = 0;
                } else if (_enemyPosition[0] == _playerPosition[0] - 1 && _playerPosition[0] != 2) {
                    nearPosition[i][0] = _playerPosition[0] - 2;
                } else if (_enemyPosition[0] == _playerPosition[0] + 1 && _playerPosition[0] == 2) {
                    nearPosition[i][0] = 0;
                }
            } else if (identity && _enemyPosition[0] == _playerPosition[0]) {
                if (_enemyPosition[1] == _playerPosition[1] + 1 && _playerPosition[1] != 8) {
                    nearPosition[i][1] = _playerPosition[1] + 2;
                } else if (_enemyPosition[1] == _playerPosition[1] + 1 && _playerPosition[1] == 8) {
                    nearPosition[i][1] = 0;
                } else if (_enemyPosition[1] == _playerPosition[1] - 1 && _playerPosition[1] != 2) {
                    nearPosition[i][1] = _playerPosition[1] - 2;
                } else if (_enemyPosition[1] == _playerPosition[1] - 1 && _playerPosition[1] == 2) {
                    nearPosition[i][1] = 0;
                }
            }
        }

        for (int i = 0; i < nearVertices.Length; i++) {
            nearVertices[i] = GameObject.Find($"Vertex {nearPosition[i][0]}{nearPosition[i][1]}");
        }
    }*/
}