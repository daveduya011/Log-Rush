using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "SkinLibrary", menuName = "ShopLibrary/SkinLibrary", order = 1)]
public class SkinLibrary : ScriptableObject
{
    public enum SkinType {
        CHARACTER, LOG, RIVER
    }
    public SkinType activeSkinWindow = SkinType.CHARACTER;
    public List<CharacterAsset> characterAssets = new List<CharacterAsset>();
    public List<LogAsset> logAssets = new List<LogAsset>();
    public List<RiverAsset> riverAssets =  new List<RiverAsset>();
}
[System.Serializable]
public class SkinAsset
{
    public Sprite image;
    public string id;
    public string itemName;
    public string description;
    public int price;

    public bool isOwned;
    public bool isEquipped;

    public bool isActive = true;

    public SkinLibrary.SkinType skinType = SkinLibrary.SkinType.CHARACTER;

    public SkinAsset() {
    }

    public SkinAsset(SkinAsset temp) {
        this.image = temp.image;
        this.id = temp.id;
        this.itemName = temp.itemName;
        this.description = temp.description;
        this.price = temp.price;
        this.isOwned = temp.isOwned;
        this.isEquipped = temp.isEquipped;
        this.isActive = temp.isActive;
        this.skinType = temp.skinType;
    }

    public CharacterAsset ToCharacterAsset(CharacterAsset temp) {
        CharacterAsset asset = new CharacterAsset(this);
        asset.displayImage = temp.displayImage;
        return asset;
    }
    public LogAsset ToLogAsset(LogAsset temp) {
        LogAsset asset = new LogAsset(this);
        asset.crocodileImageFront = temp.crocodileImageFront;
        asset.crocodileImageBack = temp.crocodileImageBack;
        asset.textColor = temp.textColor;
        return asset;
    }

    public RiverAsset ToRiverAsset(RiverAsset temp) {
        RiverAsset asset = new RiverAsset(this);
        asset.splashColor = temp.splashColor;
        asset.riverColor = temp.riverColor;
        return asset;
    }
}
[System.Serializable]
public class CharacterAsset : SkinAsset
{
    public Sprite displayImage;

    public CharacterAsset() {
    }

    public CharacterAsset(SkinAsset temp) : base(temp) {
    }
}
[System.Serializable]
public class LogAsset : SkinAsset
{
    public Sprite crocodileImageFront;
    public Sprite crocodileImageBack;
    public Color textColor = Color.white;

    public LogAsset() {
    }

    public LogAsset(SkinAsset temp) : base(temp) {
    }

}
[System.Serializable]
public class RiverAsset : SkinAsset
{
    public Color riverColor = Color.white;
    public Color splashColor = Color.white;

    public RiverAsset() {
    }

    public RiverAsset(SkinAsset temp) : base(temp) {
    }

}

#if (UNITY_EDITOR)
[CustomEditor(typeof(SkinLibrary))]
public class CustomSkinLibraryEditor : Editor
{
    SkinLibrary skinLibrary;
    List<SkinAsset> currentAssets;

    void OnEnable() {
        skinLibrary = (SkinLibrary)target;
    }

    public override void OnInspectorGUI() {
        ReloadResources();

        EditorGUIUtility.labelWidth = 60;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Skins to Show: ");
        skinLibrary.activeSkinWindow = (SkinLibrary.SkinType)EditorGUILayout.EnumPopup(skinLibrary.activeSkinWindow);
        EditorGUILayout.EndHorizontal();


        Color[] colors = new Color[] { Color.gray, Color.red };
        GUIStyle style = EditorStyles.helpBox;


        EditorGUILayout.BeginHorizontal("Box");
        if (GUILayout.Button("Add " + skinLibrary.activeSkinWindow.ToString().ToLower())) {
            LibraryWindow.ShowWindow(skinLibrary);
        }
        if (GUILayout.Button("Delete all " + skinLibrary.activeSkinWindow.ToString().ToLower() + "s")) {
            DeleteAll();
        }

        EditorGUILayout.EndHorizontal();
        GUILayout.Space(10);


        if (currentAssets == null)
            return;


        foreach (SkinAsset asset in currentAssets) {
            if (asset.isActive)
                GUI.backgroundColor = colors[0];
            else
                GUI.backgroundColor = colors[1];


            EditorGUILayout.BeginVertical(style);

            EditorGUILayout.BeginVertical("Box");

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Name of " + skinLibrary.activeSkinWindow.ToString().ToLower());
            asset.itemName = EditorGUILayout.TextField(asset.itemName);
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(3);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Price");
            asset.price = EditorGUILayout.IntField(asset.price);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();

            if (skinLibrary.activeSkinWindow == SkinLibrary.SkinType.CHARACTER || skinLibrary.activeSkinWindow == SkinLibrary.SkinType.LOG) {
                asset.image = (Sprite)EditorGUILayout.ObjectField("Image", asset.image, typeof(Sprite), false);
            }
            if (skinLibrary.activeSkinWindow == SkinLibrary.SkinType.RIVER) {
                EditorGUILayout.LabelField("River Color");
                ((RiverAsset)asset).riverColor = EditorGUILayout.ColorField(((RiverAsset)asset).riverColor);
                EditorGUILayout.LabelField("Splash Color");
                ((RiverAsset)asset).splashColor = EditorGUILayout.ColorField(((RiverAsset)asset).splashColor);
            }
                

            if (asset.isActive) {
                if (GUILayout.Button("Disable")) {
                    asset.isActive = false;
                }
            } else {
                if (GUILayout.Button("Enable")) {
                    asset.isActive = true;
                }
            }
            if (GUILayout.Button("More Options")) {
                LibraryWindow.ShowEditWindow(asset, skinLibrary);
            }

            EditorGUILayout.EndVertical();
            GUILayout.Space(10);
        }

        EditorUtility.SetDirty(skinLibrary);
    }

    private void DeleteAll() {
        switch (skinLibrary.activeSkinWindow) {
            case SkinLibrary.SkinType.CHARACTER:
                skinLibrary.characterAssets.Clear();
                break;
            case SkinLibrary.SkinType.LOG:
                skinLibrary.logAssets.Clear();
                break;
            case SkinLibrary.SkinType.RIVER:
                skinLibrary.riverAssets.Clear();
                break;
        }
    }

    private void ReloadResources() {

        switch (skinLibrary.activeSkinWindow) {
            case SkinLibrary.SkinType.CHARACTER:
                currentAssets = skinLibrary.characterAssets.ConvertAll(x => (SkinAsset)x);
                break;
            case SkinLibrary.SkinType.LOG:
                currentAssets = skinLibrary.logAssets.ConvertAll(x => (SkinAsset)x);
                break;
            case SkinLibrary.SkinType.RIVER:
                currentAssets = skinLibrary.riverAssets.ConvertAll(x => (SkinAsset)x);
                break;
        }
    }
}

public class LibraryWindow : EditorWindow
{
    private static SkinAsset asset;
    private static CharacterAsset characterAsset;
    private static LogAsset logAsset;
    private static RiverAsset riverAsset;
    private static SkinLibrary library;
    private static EditorWindow window;
    private static bool isInEditMode;

    public static void ShowWindow(SkinLibrary skinLibrary) {
        asset = new SkinAsset();

        library = skinLibrary;
        asset.skinType = library.activeSkinWindow;
        window = EditorWindow.GetWindow(typeof(LibraryWindow));
        window.maxSize = new Vector2(400, 500);
        isInEditMode = false;
        
        switch(asset.skinType) {
            case SkinLibrary.SkinType.CHARACTER:
                characterAsset = new CharacterAsset();
                break;
            case SkinLibrary.SkinType.LOG:
                logAsset = new LogAsset();
                break;
            case SkinLibrary.SkinType.RIVER:
                riverAsset = new RiverAsset();
                break;
        }
    }

    public static void ShowEditWindow(SkinAsset skinAsset, SkinLibrary skinLibrary) {
        asset = skinAsset;
        library = skinLibrary;
        window = EditorWindow.GetWindow(typeof(LibraryWindow));
        window.maxSize = new Vector2(400, 500);
        isInEditMode = true;


        switch (asset.skinType) {
            case SkinLibrary.SkinType.CHARACTER:
                characterAsset = asset as CharacterAsset;
                break;
            case SkinLibrary.SkinType.LOG:
                logAsset = asset as LogAsset;
                break;
            case SkinLibrary.SkinType.RIVER:
                riverAsset = asset as RiverAsset;
                break;
        }
    }

    void OnGUI() {
        SkinLibrary.SkinType skinType = asset.skinType;
        EditorGUIUtility.labelWidth = 60;
        GUILayout.Space(20);

        EditorGUILayout.BeginVertical();

        EditorGUILayout.BeginHorizontal();

        if (skinType != SkinLibrary.SkinType.RIVER) {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Image");
            asset.image = (Sprite)EditorGUILayout.ObjectField("", asset.image, typeof(Sprite), false);
            EditorGUILayout.EndVertical();
        }
        


        if (skinType == SkinLibrary.SkinType.CHARACTER) {
            EditorGUILayout.BeginVertical();

            EditorGUILayout.LabelField("Store Display Image");
            characterAsset.displayImage = (Sprite)EditorGUILayout.ObjectField("", characterAsset.displayImage, typeof(Sprite), false);

            EditorGUILayout.EndVertical();
        }
        else if (skinType == SkinLibrary.SkinType.RIVER) {
            EditorGUILayout.BeginVertical();

            EditorGUILayout.LabelField("River Color");
            riverAsset.riverColor = EditorGUILayout.ColorField(riverAsset.riverColor);
            EditorGUILayout.LabelField("Splash Color");
            riverAsset.splashColor = EditorGUILayout.ColorField(riverAsset.splashColor);

            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);

        EditorGUILayout.BeginHorizontal();

        if (skinType == SkinLibrary.SkinType.LOG) {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Crocodile Front");
            logAsset.crocodileImageFront = (Sprite)EditorGUILayout.ObjectField("", logAsset.crocodileImageFront, typeof(Sprite), false);
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Crocodile Back");
            logAsset.crocodileImageBack = (Sprite)EditorGUILayout.ObjectField("", logAsset.crocodileImageBack, typeof(Sprite), false);
            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndHorizontal();

        if (skinType == SkinLibrary.SkinType.LOG) {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Text Color");
            logAsset.textColor = EditorGUILayout.ColorField(logAsset.textColor);
            EditorGUILayout.EndHorizontal();
        }

        GUILayout.Space(10);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Item name");
        asset.itemName = EditorGUILayout.TextField(asset.itemName);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Price");
        asset.price = EditorGUILayout.IntField(asset.price);
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(5);

        EditorGUILayout.LabelField("Description");
        asset.description = EditorGUILayout.TextArea(asset.description, GUILayout.MinHeight(100));

        GUILayout.FlexibleSpace();

        if (!asset.isEquipped) {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Owned by default");
            asset.isOwned = EditorGUILayout.Toggle(asset.isOwned);
            EditorGUILayout.EndHorizontal();
        }


        if (!isInEditMode) {

            if (GUILayout.Button("Add " + asset.skinType.ToString().ToLower())) {
                // Generate Unique ID
                asset.id = Guid.NewGuid().ToString();

                // Check if library is empty
                // if true, set as default character
                SetDefaultCharacter(skinType);
                SaveAsset(skinType);
                window.Close();
            }
        } else {

            if (!asset.isEquipped) {
                if (GUILayout.Button("Set as default " + asset.skinType.ToString().ToLower())) {

                    foreach(SkinAsset charAsset in library.characterAssets) {
                        charAsset.isEquipped = false;
                        charAsset.isOwned = false;
                    }

                    asset.isOwned = true;
                    asset.isEquipped = true;
                }
            } else {
                GUILayout.Button("This is your default " + asset.skinType.ToString().ToLower());
            }
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Delete")) {
                DeleteAsset(skinType);
                window.Close();
            }
            if (GUILayout.Button("Close")) {
                window.Close();
            }
            EditorGUILayout.EndHorizontal();

        }
        EditorGUILayout.EndVertical();

        EditorUtility.SetDirty(library);
    }

    private void DeleteAsset(SkinLibrary.SkinType skinType) {
        switch (skinType) {
            case SkinLibrary.SkinType.CHARACTER:
                library.characterAssets.Remove(asset as CharacterAsset);
                break;
            case SkinLibrary.SkinType.LOG:
                library.logAssets.Remove(asset as LogAsset);
                break;
            case SkinLibrary.SkinType.RIVER:
                library.riverAssets.Remove(asset as RiverAsset);
                break;
        }
    }

    private void SaveAsset(SkinLibrary.SkinType skinType) {
        switch (skinType) {
            case SkinLibrary.SkinType.CHARACTER:
                library.characterAssets.Add(asset.ToCharacterAsset(characterAsset));
                break;

            case SkinLibrary.SkinType.LOG:
                library.logAssets.Add(asset.ToLogAsset(logAsset));
                break;

            case SkinLibrary.SkinType.RIVER:
                library.riverAssets.Add(asset.ToRiverAsset(riverAsset));
                break;
        }
    }

    private void SetDefaultCharacter(SkinLibrary.SkinType skinType) {
        bool isNoExist = false;
        switch (skinType) {
            case SkinLibrary.SkinType.CHARACTER:
                isNoExist = library.characterAssets.Count == 0;
                break;
            case SkinLibrary.SkinType.LOG:
                isNoExist = library.logAssets.Count == 0;
                break;
            case SkinLibrary.SkinType.RIVER:
                isNoExist = library.riverAssets.Count == 0;
                break;
        }
        if (isNoExist) {
            asset.isOwned = true;
            asset.isEquipped = true;
        }
        
    }
}
#endif
