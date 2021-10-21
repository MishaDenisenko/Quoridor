using UnityEngine;

public class PlateController : MonoBehaviour {
    private bool _isRow;
    private bool _isColumn;
    private Color _color;
    private GameObject _parent;

    private static GameObject _position;

    public static GameObject Position {
        get => _position;
        set => _position = value;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.transform.CompareTag("RowPosition") || other.transform.CompareTag("ColumnPosition")) {
            _color = gameObject.GetComponentInChildren<Renderer>().material.color;
            gameObject.transform.GetChild(0).GetComponent<Renderer>().material.color = new Color(_color.r, _color.g, _color.b, 1f);
            Position = other.gameObject;
        }
    }
    
    private void OnTriggerExit(Collider other) {
        if (other.transform.CompareTag("RowPosition") || other.transform.CompareTag("ColumnPosition")) {
            _color = gameObject.GetComponentInChildren<Renderer>().material.color;
            gameObject.transform.GetChild(0).GetComponent<Renderer>().material.color = new Color(_color.r, _color.g, _color.b, 0.5f);
            Position = null;
        }
    }

    private void OnTriggerStay(Collider other) {
        if (other.transform.CompareTag("RowPosition")) {
            gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        } 
        else if (other.transform.CompareTag("ColumnPosition")) {
            gameObject.transform.rotation = Quaternion.Euler(0, 90, 0);
        }
    }
}
