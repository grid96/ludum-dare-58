#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using StampType = StampModel.StampType;

[CreateAssetMenu(fileName = "ArtLibrary", menuName = "ScriptableObjects/ArtLibrary", order = 1)]
public class ArtLibrary : ScriptableObject
{
    [Serializable]
    private struct Entry
    {
        public StampType type;
        public Sprite sprite;
    }
    
    [SerializeField] private string folderPath;
    [SerializeField] private List<Entry> entries = new();

    private Dictionary<StampType, Sprite> Dict => dict ??= entries.ToDictionary(e => e.type, e => e.sprite);
    private Dictionary<StampType, Sprite> dict;

    public Sprite Get(StampType stampType) => Dict.GetValueOrDefault(stampType);

#if UNITY_EDITOR
    [ContextMenu("Populate")]
    private void Populate()
    {
        entries.Clear();
        var stamps = Enum.GetValues(typeof(StampType)).Cast<StampType>().ToList();
        foreach (var stamp in stamps)
        {
            var path = $"{folderPath}/{stamp}.png";
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (sprite)
                entries.Add(new Entry { type = stamp, sprite = sprite });
            else
                Debug.LogError($"[ArtLibrary] Missing {stamp} at {path}");
        }

        dict = null;
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
        Debug.Log($"[ArtLibrary] Populated {entries.Count} entries.");
    }
#endif
}