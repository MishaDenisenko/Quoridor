
using UnityEngine;
using Random = System.Random;

namespace _Scripts.Bot_Scene_Scripts {
    public class ChoosePlayer : MonoBehaviour {
        public GameObject player1;
        public GameObject player2;
    
        private void Awake() {
            Random random = new Random();
            int whosPlayer = random.Next(1, 3);
            if (whosPlayer == 1) {
                player1.GetComponent<BotOrPlayer>().PlayerType = BotOrPlayer.Type.Player;
                player2.GetComponent<BotOrPlayer>().PlayerType = BotOrPlayer.Type.Bot;
            } else if (whosPlayer == 2){
                player1.GetComponent<BotOrPlayer>().PlayerType = BotOrPlayer.Type.Bot;
                player2.GetComponent<BotOrPlayer>().PlayerType = BotOrPlayer.Type.Player;
            }
            MatchController.ActiveMove = BotOrPlayer.Type.Player;
            // MatchController.ActiveMove = player1.GetComponent<BotOrPlayer>().PlayerType;
        }
    }
}

