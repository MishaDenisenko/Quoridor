using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Bot_Scene_Scripts {
    public class OutlineManager : MonoBehaviour {
        private Camera _mainCamera;
        private float _rayLength;
        private GameObject[] _allObjects;
        private void Start() {
            _mainCamera = Camera.main;
            _rayLength = Constants.RayLength;
            _allObjects = FindObjectsOfType<GameObject>();
        }

        private void Update() {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, _rayLength)) {
                GameObject o = hit.collider.gameObject;
                List<GameObject> objectsWithOutline = new List<GameObject>();
                foreach (GameObject obj in _allObjects) {
                    if (obj && obj.GetComponent<Outline>()) objectsWithOutline.Add(obj);
                }
                ChangeOutline(objectsWithOutline.ToArray(), o);
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
    }
}