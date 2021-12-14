using System;
using UnityEngine;

public class CheckVertexController : MonoBehaviour {
    public float rayLength = 0.44f;
    public float height = 0.2f;

    private Vector3 _position;
    private Vector3 _rayPosition;
    private Vector3 _pos;
    private int[] _playerPosition = new int[2];
    private GameObject[] _nearVertices = new GameObject[5];
    
    public bool PlayerMove { get; set; }

    public int[] PlayerPosition {
        get => _playerPosition;
        set => _playerPosition = value;
    }

    public GameObject[] NearVertices {
        get => _nearVertices;
        set => _nearVertices = value;
    }
    
    public enum PlayerInitial {
        First = 1,
        Second = -1
    }
    
    public PlayerInitial Player {
        get { return player; }
        set { player = value; }
    }

    [SerializeField] private PlayerInitial player;

    private void Update() {
        if (PlayerMove) {
            Vector3 position = transform.position;
            _rayPosition = new Vector3(position.x, position.y + height, position.z);
            PlayerPosition = GetPosition(_rayPosition);
            
            NearVertices = GetNearVertices();
            PlayerMove = false;
        }
    
    }

    private int[] GetPosition(Vector3 position) {
        int[] pos = new int[2];
        
        Ray ray = new Ray(position, -transform.up);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayLength)) {
            if (hit.collider.tag.Equals("Vertex")) {
                string[] nameOfVertex = hit.collider.name.Split(' ');
                pos[0] = Int32.Parse(nameOfVertex[1][0].ToString());
                pos[1] = Int32.Parse(nameOfVertex[1][1].ToString());
            }
        }
        return pos;
    }

    public GameObject[] GetNearVertices() {
        Vector3 position = transform.position;
        _rayPosition = new Vector3(position.x, position.y + height, position.z);
        PlayerPosition = GetPosition(_rayPosition);
        GameObject[] vertices = new GameObject[5];
        int[][] verticesNumbers = new int[5][];
        for (int i = 0; i < verticesNumbers.Length; i++) {
            verticesNumbers[i] = new int[2];
        }
        verticesNumbers[0][0] = PlayerPosition[0] + (int) Player; // forward
        verticesNumbers[0][1] = PlayerPosition[1];
    
        verticesNumbers[1][0] = PlayerPosition[0] - (int) Player; // back
        verticesNumbers[1][1] = PlayerPosition[1];
    
        verticesNumbers[2][0] = PlayerPosition[0];
        verticesNumbers[2][1] = PlayerPosition[1] - (int) Player; // right
    
        verticesNumbers[3][0] = PlayerPosition[0];
        verticesNumbers[3][1] = PlayerPosition[1] + (int) Player;

        CheckVertex(ref verticesNumbers);
            
        for (int i = 0; i < vertices.Length; i++) {
            vertices[i] = GameObject.Find($"Vertex {verticesNumbers[i][0]}{verticesNumbers[i][1]}");
        }
        return vertices;
    }
    
    private void CheckVertex(ref int[][] verticesNumbers) {
        Transform trnsf = transform;
        Vector3 forward = trnsf.forward;
        Vector3 right = trnsf.right;
        Vector3 left = -right;
        Vector3 back = -forward;

        Vector3[] directions = {forward, right, left, back};
        
        int[][] nearVertices = new int[5][];
        for (int i = 0; i < nearVertices.Length; i++) {
            nearVertices[i] = new int[2];
        }
        
        RaycastHit hit;
        foreach (Vector3 direction in directions) {
            Ray ray = new Ray(_rayPosition, direction);
            if (Physics.Raycast(ray, out hit, rayLength) && hit.collider.tag.Equals("Plate")) {
                if (direction == forward) {
                    verticesNumbers[0][0] = 0;
                    verticesNumbers[0][1] = 0;
                }
                else if (direction == back) {
                    verticesNumbers[1][0] = 0;
                    verticesNumbers[1][1] = 0;
                }
                else if (direction == right) {
                    verticesNumbers[2][0] = 0;
                    verticesNumbers[2][1] = 0;
                }
                else if (direction == left) {
                    verticesNumbers[3][0] = 0;
                    verticesNumbers[3][1] = 0;
                }
            }
            else if (Physics.Raycast(ray, out hit, rayLength) && hit.collider.tag.Equals("Player")) {
                Vector3 position = hit.transform.position;
                
                if (direction == forward) {
                    Ray rayF = new Ray(new Vector3(position.x, position.y + height, position.z), forward);
                    if (Physics.Raycast(rayF, out hit, rayLength) && hit.collider.tag.Equals("Plate")) {
                        Ray rayL = new Ray(new Vector3(position.x, position.y + height, position.z), left);
                        Ray rayR = new Ray(new Vector3(position.x, position.y + height, position.z), right);
                        if (Physics.Raycast(rayL, out hit, rayLength) && hit.collider.tag.Equals("Plate")) {
                            verticesNumbers[0][0] = 0;
                            verticesNumbers[0][1] = 0;
                        } 
                        else verticesNumbers[0][1] += (int) Player;
                            
                        if (Physics.Raycast(rayR, out hit, rayLength) && hit.collider.tag.Equals("Plate")) {
                            verticesNumbers[4][0] = 0;
                            verticesNumbers[4][1] = 0;
                        } 
                        else {
                            verticesNumbers[4][0] = PlayerPosition[0] + (int) Player;
                            verticesNumbers[4][1] = PlayerPosition[1] - (int) Player;
                        }
                    } 
                    else verticesNumbers[0][0] += (int) Player;
                }
                
                // bck
                
                else if (direction == back) {
                    Ray rayB = new Ray(new Vector3(position.x, position.y + height, position.z), back);
                    if (Physics.Raycast(rayB, out hit, rayLength) && hit.collider.tag.Equals("Plate")) {
                        Ray rayL = new Ray(new Vector3(position.x, position.y + height, position.z), left);
                        Ray rayR = new Ray(new Vector3(position.x, position.y + height, position.z), right);
                        if (Physics.Raycast(rayL, out hit, rayLength) && hit.collider.tag.Equals("Plate")) {
                            verticesNumbers[1][0] = 0;
                            verticesNumbers[1][1] = 0;
                        } 
                        else verticesNumbers[1][1] += (int) Player;
                            
                        if (Physics.Raycast(rayR, out hit, rayLength) && hit.collider.tag.Equals("Plate")) {
                            verticesNumbers[4][0] = 0;
                            verticesNumbers[4][1] = 0;
                        } 
                        else verticesNumbers[4][1] = PlayerPosition[1] - (int) Player;
                        
                    } 
                    else verticesNumbers[1][0] -= (int) Player;
                }
                
                // rgh
                
                else if (direction == right) {
                    Ray rayR = new Ray(new Vector3(position.x, position.y + height, position.z), right);
                    if (Physics.Raycast(rayR, out hit, rayLength) && hit.collider.tag.Equals("Plate")) {
                        Ray rayB = new Ray(new Vector3(position.x, position.y + height, position.z), back);
                        Ray rayF = new Ray(new Vector3(position.x, position.y + height, position.z), forward);
                        if (Physics.Raycast(rayB, out hit, rayLength) && hit.collider.tag.Equals("Plate")) {
                            verticesNumbers[2][0] = 0;
                            verticesNumbers[2][1] = 0;
                        } 
                        else verticesNumbers[2][0] -= (int) Player;
                        
                        if (Physics.Raycast(rayF, out hit, rayLength) && hit.collider.tag.Equals("Plate")) {
                            verticesNumbers[4][0] = 0;
                            verticesNumbers[4][1] = 0;
                        } 
                        else {
                            verticesNumbers[4][0] = PlayerPosition[0] + (int) Player;
                            verticesNumbers[4][1] = PlayerPosition[1] - (int) Player;
                        }
                    } 
                    else verticesNumbers[2][1] -= (int) Player;
                }
                
                // lft
                
                else if (direction == left) {
                    Ray rayL = new Ray(new Vector3(position.x, position.y + height, position.z), left);
                    if (Physics.Raycast(rayL, out hit, rayLength) && hit.collider.tag.Equals("Plate")) {
                        Ray rayB = new Ray(new Vector3(position.x, position.y + height, position.z), back);
                        Ray rayF = new Ray(new Vector3(position.x, position.y + height, position.z), forward);
                        if (Physics.Raycast(rayB, out hit, rayLength) && hit.collider.tag.Equals("Plate")) {
                            verticesNumbers[2][0] = 0;
                            verticesNumbers[2][1] = 0;
                        } 
                        else verticesNumbers[2][0] -= (int) Player;
                        
                        if (Physics.Raycast(rayF, out hit, rayLength) && hit.collider.tag.Equals("Plate")) {
                            verticesNumbers[4][0] = 0;
                            verticesNumbers[4][1] = 0;
                        } 
                        else {
                            verticesNumbers[4][0] = PlayerPosition[0] + (int) Player;
                            verticesNumbers[4][1] = PlayerPosition[1] + (int) Player;
                        }
                    } 
                    else verticesNumbers[3][1] += (int) Player;
                }
            }
        }
    }

}