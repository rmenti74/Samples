using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GOUtils
{
    public static GameObject GetChildGameObject(GameObject fromGameObject, string withName) {
        Transform[] children = fromGameObject.transform.GetComponentsInChildren<Transform>();
        foreach (var child in children) {
            if (child.name == withName) {
                return child.gameObject;
            }
        }
        return null;
    }

    public static GameObject GetActiveChildGameObject(GameObject fromGameObject, string withName) {
        Transform[] children = fromGameObject.transform.GetComponentsInChildren<Transform>();
        foreach (var child in children) {
            if (child.name == withName && child.gameObject.activeSelf) {
                return child.gameObject;
            }
        }
        return null;
    }

    public static GameObject GetRootGameObject(GameObject go) {
        return go.transform.root.gameObject;
	}

    public static Transform RecursiveFindChild(Transform parent, string childName) {
        Transform result = null;

        foreach (Transform child in parent) {
            if (child.name == childName)
                result = child.transform;
            else
                result = RecursiveFindChild(child, childName);

            if (result != null) break;
        }

        return result;
    }
}
