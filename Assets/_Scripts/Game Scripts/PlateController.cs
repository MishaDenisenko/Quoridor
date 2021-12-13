using UnityEngine;

public class PlateController : MonoBehaviour {
    private bool _isRow;
    private bool _isColumn;
    private Color _color;
    
    private static GameObject _position;
    private bool _isPaste;
    private bool _isWay = true;
    private bool _onPosition;

    private GameObject _firstPlayer;
    private GameObject _secondPlayer;

    public bool IsPaste {
        get => _isPaste;
        set => _isPaste = value;
    }

    public static GameObject Position {
        get => _position;
        set => _position = value;
    }

    private void Start() {
        _color = gameObject.GetComponentInChildren<Renderer>().material.color;
        _firstPlayer = GameObject.Find("_Player1");
        _secondPlayer = GameObject.Find("_Player2");
    }

    private void Update() {
        if (GameController.Opponent == GameController.Player.FirstPlayer) {
            _firstPlayer.GetComponent<PathFinder>().enabled = true;
            _secondPlayer.GetComponent<PathFinder>().enabled = false;
            _firstPlayer.GetComponent<PathFinder>().TryFind = true;
            _isWay = _firstPlayer.GetComponent<PathFinder>().IsWay;
        } 
        else if (GameController.Opponent == GameController.Player.SecondPlayer) {
            _firstPlayer.GetComponent<PathFinder>().enabled = false;
            _secondPlayer.GetComponent<PathFinder>().enabled = true;
            _secondPlayer.GetComponent<PathFinder>().TryFind = true;
            _isWay = _secondPlayer.GetComponent<PathFinder>().IsWay;
        }
        if (!gameObject.GetComponent<PlateController>()._isPaste && (PlateIntersections.IsCrossed || !_onPosition) ||
            (PlateIntersections.IsCrossed && !_onPosition) || !_isWay) {
            gameObject.transform.GetChild(0).GetComponent<Renderer>().material.color = new Color(_color.r, _color.g, _color.b, 0.5f);
            Position = null;
        }
        else if (!gameObject.GetComponent<PlateController>()._isPaste && !PlateIntersections.IsCrossed && _onPosition) {
            gameObject.transform.GetChild(0).GetComponent<Renderer>().material.color = new Color(_color.r, _color.g, _color.b, 1f);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.transform.CompareTag("RowPosition") || other.transform.CompareTag("ColumnPosition")) {
            _onPosition = true;
            Position = other.gameObject;
        }
    }
    
    private void OnTriggerExit(Collider other) {
        if (other.transform.CompareTag("RowPosition") || other.transform.CompareTag("ColumnPosition")) {
            _onPosition = false;
            Position = null;
        }
    }

    private void OnTriggerStay(Collider other) {
        if (other.transform.CompareTag("RowPosition")) {
            gameObject.transform.rotation = Quaternion.Euler(0, 90, 0);
        } 
        else if (other.transform.CompareTag("ColumnPosition")) {
            gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}
