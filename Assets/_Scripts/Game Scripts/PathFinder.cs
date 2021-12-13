
using System.Collections.Generic;
using System.Linq;
using _Scripts.Game_Scripts;
using UnityEngine;

public class PathFinder : MonoBehaviour {
    public float rayLength = 0.44f;
    public float height = 0.2f;
    public GameObject tops;

    private bool _tryFind;
    private bool _isWay = true;

    public bool IsWay => _isWay;

    public bool TryFind {
        set => _tryFind = value;
    }

    private Vector3 _position;
    private Vector3 _rayPosition;
    private Vector3 _pos;
    private GameObject[] _finishRow = new GameObject[9];
    public enum PlayerInitial {
        First = -1,
        Second = 1
    }
    
    public PlayerInitial Player {
        get { return player; }
        set { player = value; }
    }

    [SerializeField] private PlayerInitial player;

    private void Start() {
        switch (Player) {
            case PlayerInitial.First:
                for (int i = 0; i < _finishRow.Length; i++) {
                    _finishRow[i] = tops.transform.GetChild(8).GetChild(i).gameObject;
                }
                break;
            case PlayerInitial.Second:
                for (int i = 0; i < _finishRow.Length; i++) {
                    _finishRow[i] = tops.transform.GetChild(0).GetChild(i).gameObject;
                }
                break;
        }
        
    }

    private void Update() {
        if (_tryFind) {
            Vector3 position = transform.position;
            _rayPosition = new Vector3(position.x, position.y + height, position.z);
            RaycastHit hit;
            Ray ray = new Ray(_rayPosition, Vector3.down);
            Physics.Raycast(ray, out hit, rayLength);
            if (hit.collider.tag.Equals("Vertex")) _isWay = IsPath(hit.collider.gameObject);
            
            _tryFind = false;
        }
    }

    private bool IsPath(GameObject currentVertex) {
        bool isWay = true;
        bool isRecursive = false;
        print(player);
        List<Vertex> passedVertices = new List<Vertex>();
        Vertex vertex;
        int index = 0;
        while (!_finishRow.Contains(currentVertex) && isWay) {

            if (!isRecursive) vertex = new Vertex(currentVertex);
            else vertex = passedVertices[index];
            
            Vector3 position = currentVertex.transform.position;
            _rayPosition = new Vector3(position.x, position.y + height, position.z);
            Transform trnsf = currentVertex.transform;
            Vector3 forward = trnsf.TransformDirection(Vector3.up) * (int) player;
            Vector3 right = trnsf.TransformDirection(Vector3.right) * (int) player;
            Vector3 left = trnsf.TransformDirection(Vector3.left) * (int) player;
            Vector3 back = trnsf.TransformDirection(Vector3.down) * (int) player;
            
            Vector3[] directions = {forward, right, left, back};
            
            RaycastHit hit;
            Ray ray;
            List<Vector3> directionsList = new List<Vector3>();

            GameObject[] passed = new GameObject[passedVertices.Count];
            GameObject tempV = null;

            for (int i = 0; i < passed.Length; i++) {
                passed[i] = passedVertices[i].VertexObject;
            }
            
            foreach (Vector3 direction in directions) {
                ray = new Ray(_rayPosition, direction);
                Vector3 pos = Vector3.zero;
                if (direction.Equals(forward) && !Physics.Raycast(ray, out hit, rayLength)) 
                    pos = new Vector3(position.x, position.y + height, position.z + rayLength * (int) player);
                else if (direction.Equals(right) && !Physics.Raycast(ray, out hit, rayLength)) 
                    pos = new Vector3(position.x + rayLength * (int) player, position.y + height, position.z);
                else if (direction.Equals(left) && !Physics.Raycast(ray, out hit, rayLength)) 
                    pos = new Vector3(position.x - rayLength * (int) player, position.y + height, position.z);
                else if (direction.Equals(back) && !Physics.Raycast(ray, out hit, rayLength)) 
                    pos = new Vector3(position.x, position.y + height, position.z - rayLength * (int) player);

                if (pos != Vector3.zero) {
                    ray = new Ray(pos, Vector3.down);
                    Physics.Raycast(ray, out hit, rayLength);
                    tempV = hit.collider.tag.Equals("Vertex") ? hit.collider.gameObject : null;
                    
                    if (!passed.Contains(tempV) && tempV) directionsList.Add(direction);
                }
            }
            
            vertex.Directions = directionsList.ToArray();
            if (!passed.Contains(vertex.VertexObject)) passedVertices.Add(vertex);
            
            if (vertex.Directions.Length > 0) {
                foreach (Vector3 direction in vertex.Directions) {
                    ray = new Ray(_rayPosition, direction);
                    Vector3 pos = Vector3.zero;
                    if (direction.Equals(forward) && !Physics.Raycast(ray, out hit, rayLength)) 
                        pos = new Vector3(position.x, position.y + height, position.z + rayLength * (int) player);
                    else if (direction.Equals(right) && !Physics.Raycast(ray, out hit, rayLength)) 
                        pos = new Vector3(position.x + rayLength * (int) player, position.y + height, position.z);
                    else if (direction.Equals(left) && !Physics.Raycast(ray, out hit, rayLength)) 
                        pos = new Vector3(position.x - rayLength * (int) player, position.y + height, position.z);
                    else if (direction.Equals(back) && !Physics.Raycast(ray, out hit, rayLength)) 
                        pos = new Vector3(position.x, position.y + height, position.z - rayLength * (int) player);
                    
                    if (pos != Vector3.zero) {
                        ray = new Ray(pos, Vector3.down);
                        Physics.Raycast(ray, out hit, rayLength);
                        tempV = hit.collider.tag.Equals("Vertex") ? hit.collider.gameObject : null;
                    }

                    if (tempV) {
                        currentVertex = tempV;
                        isRecursive = false;
                        break;
                    }
                }
            } else {
                index = passedVertices.IndexOf(vertex);
                isRecursive = true;
                while (vertex.Directions.Length == 0) {
                    index--;
                    if (index < 0) {
                        isRecursive = false;
                        break;
                    }
                    vertex = passedVertices[index];
                }
                if (isRecursive) {
                    currentVertex = vertex.VertexObject;
                } else {
                    isWay = false;
                }
            }
        }
        return isWay;
    }
}
