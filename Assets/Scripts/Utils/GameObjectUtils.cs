using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace utils {
    public static class GameObjectUtils
    {
        public static void DestroyAllChildren(GameObject o)
        {
            var children = new List<GameObject>();
            foreach (Transform child in o.transform) children.Add(child.gameObject);
            children.ForEach(child => GameObject.Destroy(child));
        }

        public static void DestroyAllChildrenTransitive(GameObject o)
        {
            var children = new List<GameObject>();
            foreach (Transform child in o.transform) children.Add(child.gameObject);

            foreach (GameObject child in children)
            {
                DestroyAllChildrenTransitive(child);
                GameObject.Destroy(child);
            }
        }
    }
}
