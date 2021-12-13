using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    /*public float playerPosY = 0.145f;
    public float decayRate = 0.5f;

    private Camera _mainCamera;
    private CheckVertexController.PlayerInitial _playerInitial;
    private bool _isDecayed;
    private GameObject _activeVertex;

    public bool IsActive { get; set; }
    private GameObject[] AllVertices { get; set; }
    // Start is called before the first frame update
    void Start()
    {
        _mainCamera = Camera.main;
        _playerInitial = GetComponent<CheckVertexController>().Player;
        IsActive = true;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_activeVertex && !_isDecayed) {
            bool activeVertexOutlined = _activeVertex.GetComponent<Outline>().enabled;
            
            if (!activeVertexOutlined) {
                _activeVertex.GetComponent<Outline>().enabled = true;
                _activeVertex.GetComponent<Outline>().OutlineWidth = 20;
            }
            else if (_activeVertex.GetComponent<Outline>().OutlineWidth < decayRate/5) {
                _activeVertex.GetComponent<Outline>().enabled = false;
                _isDecayed = true;
            } else _activeVertex.GetComponent<Outline>().OutlineWidth -= decayRate * Time.deltaTime;
        }
    }
    
    public void CreateSecondReaycast(GameObject[] vertices) {
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, 100)) {
            if (hit.collider.gameObject.name.Equals("BufflePlates1")) MovePlate(hit.transform.position);
            else {
                foreach (GameObject obj in vertices) {
                    if (hit.collider.gameObject == obj && IsActive) {
                        MakeAMove(hit);
                        break;
                    }
                }
            }
        }
    }

    private void MovePlate(Vector3 position) {
        MovePlates.BufflePosition = position;
        MovePlates.MovePlate = true;
    }

    private void MakeAMove(RaycastHit hit) {
        
        Vector3 hitPos;
        foreach (GameObject vertex in AllVertices) {
            vertex.GetComponent<Outline>().enabled = false;
        }
        _activeVertex = hit.collider.gameObject;
        hitPos = _activeVertex.transform.position;
        transform.position = new Vector3(hitPos.x, playerPosY, hitPos.z);
        switch (_playerInitial) {
            case CheckVertexController.PlayerInitial.First: break;
            case CheckVertexController.PlayerInitial.Second: break;
        }
        
    }*/
}
