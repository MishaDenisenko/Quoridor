namespace _Scripts.Bot_Scene_Scripts {
    public class MatchController {
        public static BotOrPlayer.Type ActiveMove { get; set; }

        public static void ChangeMove() {
            if (ActiveMove == BotOrPlayer.Type.Bot) ActiveMove = BotOrPlayer.Type.Player;
            else if (ActiveMove == BotOrPlayer.Type.Player) ActiveMove = BotOrPlayer.Type.Bot;
        }
    }
}