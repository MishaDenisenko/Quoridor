using TMPro;
using UnityEngine;

public class PlatesCountController : MonoBehaviour {

    private static int _startCountOfPlates = 10;
    public TMP_Text visualCount;
    private Camera _mainCamera;
    private int _currentCountOfPlates;
    public bool IsOptimalCount { get; private set; }

    void Start() {
        _mainCamera = Camera.main;
        _currentCountOfPlates = _startCountOfPlates;
        IsOptimalCount = true;
        visualCount.text = $"x{_startCountOfPlates}";
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) ChangeCount();
    }

    private void ChangeCount() {
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100) && hit.collider.gameObject == gameObject) {
            if (_currentCountOfPlates > 1) {
                _currentCountOfPlates--;
                visualCount.text = $"x{_currentCountOfPlates}";
            } else {
                IsOptimalCount = false;
                visualCount.text = "x0";
            }
        }
    }
}
