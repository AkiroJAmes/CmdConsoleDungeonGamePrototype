using System;
using System.Collections.Generic;
using System.Text;

namespace TextBasedAdventureGame
{
    class Room {
        protected GameObject[] objects = new GameObject[3];

        public void AddGameObject(GameObject go, int row, int col) {
            for (int i = 0; i < objects.Length; i++) {
                if (objects[i] == null) { 
                    objects[i] = go;
                    objects[i].UpdatePosition(row, col);

                    // Swap so the most recent item is drawn on top
                    var temp = objects[0];
                    objects[0] = go;
                    objects[i] = temp;
                    break;
                }
            }
        }

        public void RemoveGameObject(GameObject go) {
            for (int i = 0; i < objects.Length; i++) {
                if (objects[i] == go) {
                    int index = i;
                    for (int j = index; j < objects.Length; j++) {
                        if(j + 1 > objects.Length - 1) {
                            objects[j] = null;
                        } else {
                            objects[j] = objects[j + 1];
                        }
                    }
                }
            }
        }

        public void Draw() {
            for (int i = 0; i < objects.Length; i++) {
                if (objects[i] != null) {
                    objects[0].Draw();
                    return;
                }
            }

            Console.Write("  ");
        }

        public bool CheckIfEmpty() {
            for (int i = 0; i < objects.Length; i++) {
                if (objects[i] != null) return false;
            }

            return true;
        }

        public GameObject CheckIfItem() {
            for (int i = 0; i < objects.Length; i++) {
                if (objects[i] != null && objects[i].IsItem()) return objects[i];
            }

            return null;
        }

        public GameObject CheckIfEnemy() {
            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i] != null && objects[i].IsEnemy()) return objects[i];
            }

            return null;
        }

        public bool CheckIfWall() { 
            for (int i = 0; i < objects.Length; i++) {
                if (objects[i] != null && objects[i].IsWall()) return true;
            }

            return false;
        }

        public GameObject[] GetRoomObjects() { 
            return objects;
        }
    }
}
