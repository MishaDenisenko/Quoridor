using UnityEngine;

namespace _Scripts.Game_Scripts {
    public class Vertex {
        private GameObject _vertexObject;
        private Vector3[] _directions;

        public GameObject VertexObject {
            get => _vertexObject;
            set => _vertexObject = value;
        }
        
        public Vector3[] Directions {
            get => _directions;
            set => _directions = value;
        }

        public Vertex(GameObject vertexObject) {
            _vertexObject = vertexObject;
        }
    }
}