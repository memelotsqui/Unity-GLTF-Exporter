using System;
using UnityEngine;

namespace WEBGL_EXPORTER { 
    [Serializable]
    public class GameObjectArray {
        [SerializeField]
        public GameObject[] gameObjects;

        public GameObjectArray(GameObject[] gameObjectArray)
        {
            gameObjects = gameObjectArray;
        }
        public GameObjectArray()
        {
            gameObjects = new GameObject[0];
        }
    }
    
}
