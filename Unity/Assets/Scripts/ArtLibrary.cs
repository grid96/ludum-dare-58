using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using StampType = StampModel.StampType;

public class ArtLibrary : MonoBehaviour
{
    public static ArtLibrary Instance { get; private set; }
    
    [SerializeField] private string folderPath;

    private Dictionary<StampType, Sprite> Lookup => lookup ??= BuildLookup();
    private Dictionary<StampType, Sprite> lookup;

    private void Awake() => Instance = this;

    private Dictionary<StampType, Sprite> BuildLookup()
    {
        var dict = new Dictionary<StampType, Sprite>();
        var stamps = Enum.GetValues(typeof(StampType)).Cast<StampType>().ToList();
        foreach (var stamp in stamps)
        {
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{folderPath}/{stamp}.png");
            if (sprite)
                dict[stamp] = sprite;
            else
                Debug.LogError($"[IconLibrary] Missing {stamp} at {folderPath}");
        }

        return dict;
    }
    
    public Sprite Get(StampType stampType) => Lookup.GetValueOrDefault(stampType);
}