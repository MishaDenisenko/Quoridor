using UnityEngine;

namespace _Scripts.Bot_Scene_Scripts {
    public class BotPlateIntesection : MonoBehaviour {
        public bool IsCrossed { get; set; }
        private Color _color;

        private void OnTriggerStay(Collider other) {
            if (other.transform.CompareTag("Plate")) {
                IsCrossed = true;
            }
        }

        private void OnTriggerExit(Collider other) {
            if (other.transform.CompareTag("Plate")) {
                IsCrossed = false;
            }
        }
    }
}