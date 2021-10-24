using System;
using UnityEngine;

public class NearVertices : MonoBehaviour {
    private static int[] _enemyPosition = new int[2];
    private static int[] _playerPosition = new int[2];

    public static int[] EnemyPosition {
        get => _enemyPosition;
        set => _enemyPosition = value;
    }

    public static int[] PlayerPosition {
        get => _playerPosition;
        set => _playerPosition = value;
    }
    
    public static GameObject[] GetNearVertices() {
            int countOfVertices;
    
            if (_playerPosition[0] == 1 && _playerPosition[1] == 1 || _playerPosition[0] == 9 && _playerPosition[1] == 1 ||
                _playerPosition[0] == 1 && _playerPosition[1] == 9 || _playerPosition[0] == 9 && _playerPosition[1] == 9) countOfVertices = 2;
    
            else if (_playerPosition[0] == 1 || _playerPosition[0] == 9 || _playerPosition[1] == 1 || _playerPosition[1] == 9) countOfVertices = 3;
    
            else countOfVertices = 4;
    
            GameObject[] nearVertices = new GameObject[countOfVertices];
    
            switch (countOfVertices) {
                case 2:
                    if (_playerPosition[0] == 1 && _playerPosition[1] == 1) {
                        nearVertices[0] = GameObject.Find($"Vertex {_playerPosition[0] + 1}{_playerPosition[1]}");
                        nearVertices[1] = GameObject.Find($"Vertex {_playerPosition[0]}{_playerPosition[1] + 1}");
                    } else if (_playerPosition[0] == 9 && _playerPosition[1] == 1) {
                        nearVertices[0] = GameObject.Find($"Vertex {_playerPosition[0] - 1}{_playerPosition[1]}");
                        nearVertices[1] = GameObject.Find($"Vertex {_playerPosition[0]}{_playerPosition[1] + 1}");
                    } else if (_playerPosition[0] == 1 && _playerPosition[1] == 9) {
                        nearVertices[0] = GameObject.Find($"Vertex {_playerPosition[0] + 1}{_playerPosition[1]}");
                        nearVertices[1] = GameObject.Find($"Vertex {_playerPosition[0]}{_playerPosition[1] - 1}");
                    } else if (_playerPosition[0] == 9 && _playerPosition[1] == 9) {
                        nearVertices[0] = GameObject.Find($"Vertex {_playerPosition[0] - 1}{_playerPosition[1]}");
                        nearVertices[1] = GameObject.Find($"Vertex {_playerPosition[0]}{_playerPosition[1] - 1}");
                    }
                    break;
                case 3:
                    if (_playerPosition[0] == 1) {
                        nearVertices[0] = GameObject.Find($"Vertex {_playerPosition[0] + 1}{_playerPosition[1]}");
                        nearVertices[1] = GameObject.Find($"Vertex {_playerPosition[0]}{_playerPosition[1] - 1}");
                        nearVertices[2] = GameObject.Find($"Vertex {_playerPosition[0]}{_playerPosition[1] + 1}");
                    } else if (_playerPosition[0] == 9) {
                        nearVertices[0] = GameObject.Find($"Vertex {_playerPosition[0] - 1}{_playerPosition[1]}");
                        nearVertices[1] = GameObject.Find($"Vertex {_playerPosition[0]}{_playerPosition[1] - 1}");
                        nearVertices[2] = GameObject.Find($"Vertex {_playerPosition[0]}{_playerPosition[1] + 1}");
                    } else if (_playerPosition[1] == 1) {
                        nearVertices[0] = GameObject.Find($"Vertex {_playerPosition[0] + 1}{_playerPosition[1]}");
                        nearVertices[1] = GameObject.Find($"Vertex {_playerPosition[0] - 1}{_playerPosition[1]}");
                        nearVertices[2] = GameObject.Find($"Vertex {_playerPosition[0]}{_playerPosition[1] + 1}");
                    } else if (_playerPosition[1] == 9) {
                        nearVertices[0] = GameObject.Find($"Vertex {_playerPosition[0] + 1}{_playerPosition[1]}");
                        nearVertices[1] = GameObject.Find($"Vertex {_playerPosition[0] - 1}{_playerPosition[1]}");
                        nearVertices[2] = GameObject.Find($"Vertex {_playerPosition[0]}{_playerPosition[1] - 1}");
                    }
                    break;
                case 4:
                    nearVertices[0] = GameObject.Find($"Vertex {_playerPosition[0] + 1}{_playerPosition[1]}");
                    nearVertices[1] = GameObject.Find($"Vertex {_playerPosition[0] - 1}{_playerPosition[1]}");
                    nearVertices[2] = GameObject.Find($"Vertex {_playerPosition[0]}{_playerPosition[1] + 1}");
                    nearVertices[3] = GameObject.Find($"Vertex {_playerPosition[0]}{_playerPosition[1] - 1}");
                    break;
            }

            EnemyNear(ref nearVertices);
    
            return nearVertices;
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
                }
                else if (_enemyPosition[0] == _playerPosition[0] + 1 && _playerPosition[0] == 8) {
                    nearPosition[i][0] = 0;
                }
                else if (_enemyPosition[0] == _playerPosition[0] - 1 && _playerPosition[0] != 2) {
                    nearPosition[i][0] = _playerPosition[0] - 2;
                }
                else if (_enemyPosition[0] == _playerPosition[0] + 1 && _playerPosition[0] == 2) {
                    nearPosition[i][0] = 0;
                }
            }
            else if (identity && _enemyPosition[0] == _playerPosition[0]) {
                if (_enemyPosition[1] == _playerPosition[1] + 1 && _playerPosition[1] != 8) {
                    nearPosition[i][1] = _playerPosition[1] + 2;
                }
                else if (_enemyPosition[1] == _playerPosition[1] + 1 && _playerPosition[1] == 8) {
                    nearPosition[i][1] = 0;
                }
                else if (_enemyPosition[1] == _playerPosition[1] - 1 && _playerPosition[1] != 2) {
                    nearPosition[i][1] = _playerPosition[1] - 2;
                }
                else if (_enemyPosition[1] == _playerPosition[1] - 1 && _playerPosition[1] == 2) {
                    nearPosition[i][1] = 0;
                }
            }
        }

        for (int i = 0; i < nearVertices.Length; i++) {
            nearVertices[i] = GameObject.Find($"Vertex {nearPosition[i][0]}{nearPosition[i][1]}");
        }
    }
}