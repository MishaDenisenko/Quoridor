using UnityEngine;

namespace _Scripts.Bot_Scene_Scripts {
    public class GameController : MonoBehaviour {
        public GameObject firstRowTop;
        public GameObject firstRowBottom;
        public Color playerColor;
        
        private GameObject[] _firstRow = new GameObject[Constants.Size];
        private bool _firstMove;
        private bool _isOutlined;
        private BotOrPlayer.Type _type;
        
        private void Start() {
            // GetComponent<PlayerController>().enabled = GetComponent<BotOrPlayer>().PlayerType == BotOrPlayer.Type.Player;
            int size = Constants.Size;
            GameObject fr =
                gameObject.GetComponent<CheckVertexController>().Player == CheckVertexController.PlayerInitial.First
                    ? firstRowTop
                    : firstRowBottom;
            // print(fr.name + " " + name);
            for (int i = 0; i < size; i++) {
                _firstRow[i] = fr.transform.GetChild(i).gameObject;
            }
            _firstMove = true;
            _type = GetComponent<BotOrPlayer>().PlayerType;
        }

        private void Update() {
            if (MatchController.ActiveMove == BotOrPlayer.Type.Player) {
                if (_firstMove && !_isOutlined) {
                    print(11);
                    foreach (GameObject vertex in _firstRow) {
                        vertex.GetComponent<Outline>().OutlineColor = playerColor;
                        vertex.GetComponent<Outline>().enabled = true;
                    }
                    _isOutlined = true;
                }
            }
        }
    }
}