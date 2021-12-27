using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Bot_Scene_Scripts {
    public class FindNearPlatesPos : MonoBehaviour {
        public float distance = 0.2f;
        public float range = 10f;
        public GameObject[] NearPlatesPos() {
            List<GameObject> positions = new List<GameObject>();
            Vector3 pos = transform.position;
            Vector3[] directions = {
                new Vector3(pos.x + distance, pos.y, pos.z),
                new Vector3(pos.x - distance, pos.y, pos.z),
                new Vector3(pos.x, pos.y, pos.z + distance),
                new Vector3(pos.x, pos.y, pos.z - distance)
            };
            foreach (Vector3 direction in directions) {
                Ray ray = new Ray(direction, Vector3.down);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, range) && hit.collider.gameObject.tag.Equals("RowPosition") ||
                    hit.collider.gameObject.tag.Equals("ColumnPosition")) {
                    positions.Add(hit.collider.gameObject);
                }
            }
            
            return positions.ToArray();
        }
    }
}