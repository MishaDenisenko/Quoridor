using System.Collections.Generic;
using System.Linq;
using _Scripts.Game_Scripts;
using UnityEngine;

namespace _Scripts.Bot_Scene_Scripts {
    public class ShortestPath : MonoBehaviour {
        public float rayLength = 0.44f;
        public float height = 0.2f;
        public GameObject tops;

        // private bool _tryFind;
        // private bool _isWay = true;
        //
        // public bool IsWay => _isWay;
        //
        // public bool TryFind {
        //     set => _tryFind = value;
        // }

        private Vector3 _position;
        private Vector3 _rayPosition;
        private Vector3 _pos;
        private GameObject[] _finishRow = new GameObject[9];

        public enum PlayerInitial {
            First = -1, Second = 1
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

        public GameObject[] FindShortest() {
            Vector3 position = transform.position;
            _rayPosition = new Vector3(position.x, position.y + height, position.z);
            Ray ray = new Ray(_rayPosition, Vector3.down);
            RaycastHit hit;
            Physics.Raycast(ray, out hit, 50);
            List<Vertex> path = new List<Vertex>();
            if (hit.collider.tag.Equals("Vertex")) {
                 path = IsPath(hit.collider.gameObject, Vector3.zero);
            }
            return FindShortest(path);
        }
        
        private GameObject[] FindShortest(List<Vertex> path) {
            int k = 0;
            List<Vertex> newPath = new List<Vertex>();
            List<Vertex> tmpPath = new List<Vertex>();
            List<Vertex> iterimPath = new List<Vertex>();
            List<List<Vertex>> allWays = new List<List<Vertex>>();
            while (k < path.Count - 1) {
                Vertex vertex = path[k];
                Vector3 position = vertex.VertexObject.transform.position;
                _rayPosition = new Vector3(position.x, position.y + height, position.z);
                Transform trnsf = vertex.VertexObject.transform;
                Vector3 forward = trnsf.TransformDirection(Vector3.up) * (int) player;
                Vector3 right = trnsf.TransformDirection(Vector3.right) * (int) player;
                Vector3 left = trnsf.TransformDirection(Vector3.left) * (int) player;
                Vector3 back = trnsf.TransformDirection(Vector3.down) * (int) player;
                
                Vector3[] directions = {forward, right, left, back};
                
                foreach (Vector3 direction in directions) {
                    newPath = new List<Vertex>();
                    tmpPath = IsPath(vertex.VertexObject, direction);
                    if (tmpPath.Count > 0 && _finishRow.Contains(tmpPath[tmpPath.Count - 1].VertexObject)) {
                        foreach (Vertex v in iterimPath) {
                            newPath.Add(v);
                        }
                        foreach (Vertex v in tmpPath) {
                            if (!newPath.Contains(v))newPath.Add(v);
                        }
                        if (newPath.Count > 0) allWays.Add(newPath);
                    }   
                }
                
                iterimPath.Add(vertex);
                k++;
            }
            List<Vertex> shortest = allWays[0];
            GameObject[] shortWay = new GameObject[shortest.Count];
            for (int i = 0; i < shortest.Count; i++) {
                shortWay[i] = shortest[i].VertexObject;
            }
            return shortWay;
        }

        private List<Vertex> IsPath(GameObject currentVertex, Vector3 dir) {
            bool isWay = true;
            bool isRecursive = false;
            List<Vertex> passedVertices = new List<Vertex>();
            Vertex vertex;
            int index = 0;
            while (isWay) {
                if (_finishRow.Contains(currentVertex)) {
                    passedVertices.Add(new Vertex(currentVertex));
                    break;
                } 

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
                GameObject tempV = null;
                
                if (dir != Vector3.zero) {
                    vertex = new Vertex(currentVertex);
                    vertex.Directions = new[] {dir};
                    passedVertices.Add(vertex);
                    Vector3 pos = new Vector3();
                    ray = new Ray(_rayPosition, dir);
                    // Vector3 pos = Vector3.zero;
                    bool isHit = Physics.Raycast(ray, out hit, rayLength);
                    bool isPlayer = hit.collider ? hit.collider.tag.Equals("Player") : false;
                    if (dir.Equals(forward) && (!isHit || isPlayer))
                        pos = new Vector3(position.x, position.y + height, position.z + rayLength * (int) player);
                    else if (dir.Equals(right) && (!isHit || isPlayer))
                        pos = new Vector3(position.x + rayLength * (int) player, position.y + height, position.z);
                    else if (dir.Equals(left) && (!isHit || isPlayer))
                        pos = new Vector3(position.x - rayLength * (int) player, position.y + height, position.z);
                    else if (dir.Equals(back) && (!isHit || isPlayer))
                        pos = new Vector3(position.x, position.y + height, position.z - rayLength * (int) player);

                    if (pos != Vector3.zero) {
                        ray = new Ray(pos, Vector3.down);
                        Physics.Raycast(ray, out hit, rayLength);
                        tempV = hit.collider.tag.Equals("Vertex") ? hit.collider.gameObject : null;
                    }
                    
                    if (tempV) {
                        currentVertex = tempV;
                        isRecursive = false;
                    } else {
                        break;
                    }
                    dir = Vector3.zero;
                } else {
                    if (!isRecursive) vertex = new Vertex(currentVertex);
                    else vertex = passedVertices[index];
                    List<Vector3> directionsList = new List<Vector3>();

                    GameObject[] passed = new GameObject[passedVertices.Count];

                    for (int i = 0; i < passed.Length; i++) {
                        passed[i] = passedVertices[i].VertexObject;
                    }

                    foreach (Vector3 direction in directions) {
                        ray = new Ray(_rayPosition, direction);
                        Vector3 pos = Vector3.zero;
                        bool isHit = Physics.Raycast(ray, out hit, rayLength);
                        bool isPlayer = hit.collider ? hit.collider.tag.Equals("Player") : false;
                        if (direction.Equals(forward) && (!isHit || isPlayer))
                            pos = new Vector3(position.x, position.y + height, position.z + rayLength * (int) player);
                        else if (direction.Equals(right) && (!isHit || isPlayer))
                            pos = new Vector3(position.x + rayLength * (int) player, position.y + height, position.z);
                        else if (direction.Equals(left) && (!isHit || isPlayer))
                            pos = new Vector3(position.x - rayLength * (int) player, position.y + height, position.z);
                        else if (direction.Equals(back) && (!isHit || isPlayer))
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
                            bool isHit = Physics.Raycast(ray, out hit, rayLength);
                            bool isPlayer = hit.collider ? hit.collider.tag.Equals("Player") : false;
                            if (direction.Equals(forward) && (!isHit || isPlayer))
                                pos = new Vector3(position.x, position.y + height,
                                    position.z + rayLength * (int) player);
                            else if (direction.Equals(right) && (!isHit || isPlayer))
                                pos = new Vector3(position.x + rayLength * (int) player, position.y + height,
                                    position.z);
                            else if (direction.Equals(left) && (!isHit || isPlayer))
                                pos = new Vector3(position.x - rayLength * (int) player, position.y + height,
                                    position.z);
                            else if (direction.Equals(back) && (!isHit || isPlayer))
                                pos = new Vector3(position.x, position.y + height,
                                    position.z - rayLength * (int) player);

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
            }
            return passedVertices;
        }
    }
}