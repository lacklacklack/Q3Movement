using System.Collections;
using System.Collections.Generic;
using UnityEditor;

class LevelProvider : IEnumerable<string> {
    IEnumerator<string> IEnumerable<string>.GetEnumerator() {
        var allLevelGUIDs = AssetDatabase.FindAssets("t:Scene", new[] { "Assets/Scenes/Levels" });
        foreach (var levelGUID in allLevelGUIDs) {
            var levelPath = AssetDatabase.GUIDToAssetPath(levelGUID);
            yield return levelPath;
        }
    }

    public IEnumerator GetEnumerator() => (this as IEnumerable<string>).GetEnumerator();
}
