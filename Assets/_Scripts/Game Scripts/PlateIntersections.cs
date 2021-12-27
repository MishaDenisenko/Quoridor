using System;
using UnityEngine;

public class PlateIntersections : MonoBehaviour {
    private static bool _isCrossed;
    private Color _color;

    public static bool IsCrossed {
        get => _isCrossed;
        set => _isCrossed = value;
    }

    private void OnTriggerStay(Collider other) {
        if (other.transform.CompareTag("Plate")) {
            _isCrossed = true;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.transform.CompareTag("Plate")) {
            _isCrossed = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.transform.CompareTag("Plate")) {
            _isCrossed = false;
        }
    }
}
