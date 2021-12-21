using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Bot_Scene_Scripts {
    public class BotOrPlayer : MonoBehaviour {
        private Type _playerType = Type.None;
        public enum Type {
            None,
            Player,
            Bot
        }
    
        public Type PlayerType {
            get => _playerType;
            set => _playerType = value;
        }
    
        private void Start() {
            print(PlayerType + gameObject.name);
        }
    }
}

