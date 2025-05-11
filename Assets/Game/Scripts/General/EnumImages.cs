using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;


[Serializable]
public class BuildingImageData
{
    public string BuildingType;
    public string ImagePath;
}

[Serializable]
public class BuildingsImagesWrapper
{
    public List<BuildingImageData> Images;
}


public static class BuildingsImagesManager
{
    private static Dictionary<BuildingsTypes, Sprite> _imagesCache = new Dictionary<BuildingsTypes, Sprite>();

    public static void LoadImages(TextAsset jsonFile)
    {
        if (jsonFile == null)
        {
            Debug.LogError("JSON file is null");
            return;
        }

        var json = JSON.Parse(jsonFile.text);
        
        foreach (KeyValuePair<string, JSONNode> entry in json["Images"].AsArray)
        {
            string buildingTypeStr = entry.Value["BuildingType"].Value;
            string imagePath = entry.Value["ImagePath"].Value;

            if (System.Enum.TryParse(buildingTypeStr, out BuildingsTypes buildingType))
            {
                Sprite sprite = Resources.Load<Sprite>(imagePath);
                if (sprite != null)
                {
                    _imagesCache[buildingType] = sprite;
                }
                else
                {
                    Debug.LogError($"Failed to load sprite at path: {imagePath}");
                }
            }
            else
            {
                Debug.LogError($"Unknown building type: {buildingTypeStr}");
            }
        }
    }

    public static Sprite GetBuildingImage(BuildingsTypes buildingType)
    {
        if (_imagesCache.TryGetValue(buildingType, out Sprite sprite))
        {
            return sprite;
        }
        
        Debug.LogError($"No image found for building type: {buildingType}");
        return null;
    }
}