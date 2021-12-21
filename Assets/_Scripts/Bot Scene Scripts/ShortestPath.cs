using System.Collections.Generic;
using System.Linq;
using _Scripts.Game_Scripts;
using UnityEngine;

namespace _Scripts.Bot_Scene_Scripts {
    public class ShortestPath : MonoBehaviour {
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
        public GameObject[] Shortest { get; set; }

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

        private void Update() {
            // if (_tryFind) {
            //     Vector3 position = transform.position;
            //     _rayPosition = new Vector3(position.x, position.y + height, position.z);
            //     RaycastHit hit;
            //     Ray ray = new Ray(_rayPosition, Vector3.down);
            //     Physics.Raycast(ray, out hit, rayLength);
            //     if (hit.collider.tag.Equals("Vertex")) {
            //         List<Vertex> path = IsPath(hit.collider.gameObject, Vector3.zero);
            //         Shortest = FindShortest(path);
            //         print(Shortest.Length + " ::: " + path.Count);
            //         // foreach (Vertex passedVertex in path) {
            //         //     passedVertex.VertexObject.GetComponent<Outline>().enabled = true;
            //         //     passedVertex.VertexObject.GetComponent<Outline>().OutlineColor = Color.red;
            //         // }
            //         foreach (GameObject passedVertex in Shortest) {
            //             passedVertex.GetComponent<Outline>().enabled = true;
            //             passedVertex.GetComponent<Outline>().OutlineColor = Color.green;
            //         }
            //     }
            //
            //     _tryFind = false;
            // }
        }

        public GameObject[] FindShortest() {
            Vector3 position = transform.position;
            _rayPosition = new Vector3(position.x, position.y + height, position.z);
            Ray ray = new Ray(_rayPosition, Vector3.down);
            RaycastHit hit;
            Physics.Raycast(ray, out hit, 50);
            Debug.DrawRay(_rayPosition, Vector3.down, Color.cyan, 50);
            List<Vertex> path = new List<Vertex>();
            print(hit.collider.gameObject +"=");
            if (hit.collider.tag.Equals("Vertex")) {
                 path = IsPath(hit.collider.gameObject, Vector3.zero);
            }
            return FindShortest(path);
        }
        private GameObject[] FindShortest(List<Vertex> path) {
            List<Vertex> newPath = new List<Vertex>();
            int k = 0;
            List<Vertex> tmpPath = new List<Vertex>();
            List<Vertex> tmpPath2 = new List<Vertex>();
            foreach (Vertex vertex in path) {
                if (vertex.Directions.Length > 0) newPath.Add(vertex);
            }
            foreach (Vertex vertex in newPath) {
                tmpPath.Add(vertex);
                if (vertex.Directions.Length > 1) {
                    foreach (Vector3 direction in vertex.Directions) {
                        tmpPath2 = IsPath(vertex.VertexObject, direction);
                        if (tmpPath2.Count + k < newPath.Count) {
                            newPath = tmpPath2;
                            foreach (Vertex v in tmpPath) {
                                if (!newPath.Contains(v)) newPath.Add(v);
                            }
                        }
                    }
                }
                k++;
            }
            GameObject[] shortWay = new GameObject[newPath.Count];
            for (int i = 0; i < shortWay.Length; i++) {
                shortWay[i] = newPath[i].VertexObject;
            }
            return shortWay;
        }

        private List<Vertex> IsPath(GameObject currentVertex, Vector3 dir) {
            bool isWay = true;
            bool isRecursive = false;
            List<Vertex> passedVertices = new List<Vertex>();
            Vertex vertex;
            int index = 0;
            while (!_finishRow.Contains(currentVertex) && isWay) {

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
                    Vector3 pos = new Vector3();
                    // ray = new Ray(_rayPosition, dir);
                    // Vector3 pos = Vector3.zero;
                    // bool isHit = Physics.Raycast(ray, out hit, rayLength);
                    // bool isPlayer = hit.collider ? hit.collider.tag.Equals("Player") : false;
                    if (dir.Equals(forward))
                        pos = new Vector3(position.x, position.y + height, position.z + rayLength * (int) player);
                    else if (dir.Equals(right))
                        pos = new Vector3(position.x + rayLength * (int) player, position.y + height, position.z);
                    else if (dir.Equals(left))
                        pos = new Vector3(position.x - rayLength * (int) player, position.y + height, position.z);
                    else if (dir.Equals(back))
                        pos = new Vector3(position.x, position.y + height, position.z - rayLength * (int) player);

                    
                    ray = new Ray(pos, Vector3.down);
                    Physics.Raycast(ray, out hit, rayLength);
                    tempV = hit.collider.tag.Equals("Vertex") ? hit.collider.gameObject : null;

                    if (tempV) {
                        currentVertex = tempV;
                        isRecursive = false;
                        dir = Vector3.zero;
                    }
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

            // foreach (Vertex passedVertex in passedVertices) {
            //     // passedVertex.VertexObject.GetComponent<Outline>().enabled = true;
            //     // passedVertex.VertexObject.GetComponent<Outline>().OutlineColor = Color.red;
            //     print(passedVertex.VertexObject);
            //     print(passedVertex.Directions.Length);
            // }
            return passedVertices;
        }
    }
}