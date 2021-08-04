using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Json
{
    public static string LoadJson(string path) {
        string jsonFilePath = path.Replace(".json", "");
        TextAsset loadedJsonFile = Resources.Load<TextAsset>(jsonFilePath);
        return loadedJsonFile.text;
    }
}
