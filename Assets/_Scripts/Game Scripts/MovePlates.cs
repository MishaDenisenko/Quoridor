using UnityEngine;

public class MovePlates : MonoBehaviour {
    public GameObject bufflePrefab;
    public float speed = 5f;

    private bool _isCreate;
    private bool _installed;
    private Vector3 _targetPos;
    private Camera _mainCamera;
    private GameObject _bufflePlate;
    private Color _color;

    private static Vector3 _bufflePosition;
    private static bool _movePlate;

    public static bool MovePlate {
        get => _movePlate;
        set => _movePlate = value;
    }

    public static Vector3 BufflePosition {
        get => _bufflePosition;
        set => _bufflePosition = value;
    }

    private void Awake() {
        _mainCamera = Camera.main;
    }

    void Update() {
        if (_movePlate) {
            if (!_isCreate) {
                _bufflePlate = Instantiate(bufflePrefab, new Vector3(_bufflePosition.x, 0.26f, _bufflePosition.z),
                    Quaternion.identity);
                _color = _bufflePlate.GetComponentInChildren<Renderer>().material.color;
                _bufflePlate.transform.GetChild(0).GetComponent<Renderer>().material.color = new Color(_color.r, _color.g, _color.b, 0.5f);
                _isCreate = true;
            }

            _targetPos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            float step = speed * Time.deltaTime;
            if (_bufflePlate) {
                Vector3 bufflePlatePosition = _bufflePlate.transform.position;
                _bufflePlate.transform.position = Vector3.MoveTowards(bufflePlatePosition,
                    new Vector3(_targetPos.x, bufflePlatePosition.y, _targetPos.z), step);

                if (Input.GetMouseButtonDown(0) && PlateController.Position) {
                    _bufflePlate.transform.position =  PlateController.Position.transform.position;
                    _bufflePlate.transform.GetChild(0).GetComponent<Renderer>().material.color = new Color(_color.r, _color.g, _color.b, 1f);
                    _movePlate = false;
                    _isCreate = false;
                    PlateController.Position = null;
                    GameController.PlateInstalled = true;
                }
            }

        }
    }
}