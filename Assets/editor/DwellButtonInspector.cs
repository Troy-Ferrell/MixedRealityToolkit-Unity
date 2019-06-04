using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DwellButton))]
public class DwellButtonInspector : UnityEditor.Editor
{
    protected DwellButton instance;
    protected SerializedProperty profileList;
    protected bool showProfiles;

    protected List<InspectorUIUtility.ListSettings> listSettings;
    protected InteractableTypesContainer themeOptions;

    protected virtual void OnEnable()
    {
        instance = (DwellButton)target;

        themeOptions = InteractableProfileItem.GetThemeTypes();

        profileList = serializedObject.FindProperty("Profiles");
        listSettings = InspectorUIUtility.AdjustListSettings(null, profileList.arraySize);
        //enabled = true;
    }

    public sealed override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        RenderProfileInspector();
    }

    protected virtual State[] GetStates()
    {
        if (instance == null || instance.EyeGazeStates == null) return null;

        return instance.EyeGazeStates.GetStates();
    }

    // TODO: These helpers need to be saved in a utility (ThemeInspector?)

    protected void AddProfile(int index)
    {
        profileList.InsertArrayElementAtIndex(profileList.arraySize);
        SerializedProperty newItem = profileList.GetArrayElementAtIndex(profileList.arraySize - 1);

        SerializedProperty newTarget = newItem.FindPropertyRelative("Target");
        SerializedProperty themes = newItem.FindPropertyRelative("Themes");
        newTarget.objectReferenceValue = null;

        themes.ClearArray();

        listSettings.Add(new InspectorUIUtility.ListSettings() { Show = false, Scroll = new Vector2() });
    }

    protected void RemoveProfile(int index, SerializedProperty prop = null)
    {
        profileList.DeleteArrayElementAtIndex(index);
    }

    protected void CreateTheme(int[] arr, SerializedProperty prop = null)
    {
        SerializedProperty sItem = profileList.GetArrayElementAtIndex(arr[0]);
        SerializedProperty themes = sItem.FindPropertyRelative("Themes");
        SerializedProperty themeItem = themes.GetArrayElementAtIndex(arr[1]);

        SerializedProperty gameObject = sItem.FindPropertyRelative("Target");

        GameObject host = gameObject.objectReferenceValue as GameObject;
        string path = "Assets/Themes";

        if (host != null)
        {
            string themeName = host.name + "Theme.asset";

            path = EditorUtility.SaveFilePanelInProject(
               "Save New Theme",
               themeName,
               "asset",
               "Create a name and select a location for this theme");

            if (path.Length != 0)
            {
                Theme newTheme = ScriptableObject.CreateInstance<Theme>();
                AssetDatabase.CreateAsset(newTheme, path);
                themeItem.objectReferenceValue = newTheme;
            }
        }
    }

    protected virtual void AddThemeProperty(int[] arr, SerializedProperty prop = null)
    {
        int profile = arr[0];
        int theme = arr[1];

        SerializedProperty sItem = profileList.GetArrayElementAtIndex(profile);
        SerializedProperty themes = sItem.FindPropertyRelative("Themes");
        SerializedProperty serializedTarget = sItem.FindPropertyRelative("Target");

        SerializedProperty themeItem = themes.GetArrayElementAtIndex(theme);
        SerializedObject themeObj = new SerializedObject(themeItem.objectReferenceValue);
        themeObj.Update();

        SerializedProperty themeObjSettings = themeObj.FindProperty("Settings");
        themeObjSettings.InsertArrayElementAtIndex(themeObjSettings.arraySize);

        SerializedProperty settingsItem = themeObjSettings.GetArrayElementAtIndex(themeObjSettings.arraySize - 1);
        SerializedProperty className = settingsItem.FindPropertyRelative("Name");
        SerializedProperty assemblyQualifiedName = settingsItem.FindPropertyRelative("AssemblyQualifiedName");
        if (themeObjSettings.arraySize == 1)
        {
            className.stringValue = "ScaleOffsetColorTheme";
            assemblyQualifiedName.stringValue = typeof(ScaleOffsetColorTheme).AssemblyQualifiedName;
        }
        else
        {
            className.stringValue = themeOptions.ClassNames[0];
            assemblyQualifiedName.stringValue = themeOptions.AssemblyQualifiedNames[0];
        }

        SerializedProperty easing = settingsItem.FindPropertyRelative("Easing");

        SerializedProperty time = easing.FindPropertyRelative("LerpTime");
        SerializedProperty curve = easing.FindPropertyRelative("Curve");
        time.floatValue = 0.5f;
        curve.animationCurveValue = AnimationCurve.Linear(0, 1, 1, 1);

        themeObjSettings = ThemeInspector.ChangeThemeProperty(themeObjSettings.arraySize - 1, themeObjSettings, serializedTarget, GetStates(), true);

        themeObj.ApplyModifiedProperties();
    }

    public virtual void RenderProfileInspector()
    {
        //bool isOPen = InspectorUIUtility.DrawSectionStart(profileTitle, indentOnSectionStart + 1, showProfiles, InspectorUIUtility.LableStyle(InspectorUIUtility.TitleFontSize, InspectorUIUtility.ColorTint50).fontStyle, false, InspectorUIUtility.TitleFontSize);

        showProfiles = EditorGUILayout.Foldout(showProfiles, "Profiles", true);

        /*
        if (profileList.arraySize < 1)
        {
            AddProfile(0);
        }*/

        if (showProfiles)
        {
            for (int i = 0; i < profileList.arraySize; i++)
            {
                EditorGUILayout.BeginVertical("Box");
                // get profiles
                SerializedProperty sItem = profileList.GetArrayElementAtIndex(i);

                SerializedProperty gameObject = sItem.FindPropertyRelative("Target");
                string targetName = "Profile " + (i + 1);
                if (gameObject.objectReferenceValue != null)
                {
                    targetName = gameObject.objectReferenceValue.name;
                }

                EditorGUILayout.BeginHorizontal();
                InspectorUIUtility.DrawLabel(targetName, 12, InspectorUIUtility.ColorTint100);

                bool triggered = InspectorUIUtility.SmallButton(new GUIContent(InspectorUIUtility.Minus, "Remove Profile"), i, RemoveProfile);

                if (triggered)
                {
                    continue;
                }

                EditorGUILayout.EndHorizontal();

                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(gameObject, new GUIContent("Target", "Target gameObject for this theme properties to manipulate"));

                SerializedProperty themes = sItem.FindPropertyRelative("Themes");

                for (int t = 0; t < themes.arraySize; t++)
                {
                    SerializedProperty themeItem = themes.GetArrayElementAtIndex(t);
                    EditorGUILayout.PropertyField(themeItem, new GUIContent("Theme", "Theme properties for interaction feedback"));

                    if (themeItem.objectReferenceValue != null && gameObject.objectReferenceValue)
                    {
                        if (themeItem.objectReferenceValue.name == "DefaultTheme")
                        {
                            EditorGUILayout.BeginHorizontal();
                            InspectorUIUtility.DrawWarning("DefaultTheme should not be edited.  ");
                            bool newTheme = InspectorUIUtility.FlexButton(new GUIContent("Create Theme", "Create a new theme"), new int[] { i, t, 0 }, CreateTheme);
                            if (newTheme)
                            {
                                continue;
                            }
                            EditorGUILayout.EndHorizontal();
                        }

                        SerializedProperty hadDefault = sItem.FindPropertyRelative("HadDefaultTheme");
                        hadDefault.boolValue = true;
                        EditorGUI.indentLevel++;

                        // TODO: Rename as not to conflict with interactableinspector
                        string prefKey = themeItem.objectReferenceValue.name + "Profiles" + i + "_Theme" + t + "_Edit";
                        bool hasPref = EditorPrefs.HasKey(prefKey);
                        bool showSettings = EditorPrefs.GetBool(prefKey);
                        if (!hasPref)
                        {
                            showSettings = true;
                        }

                        InspectorUIUtility.ListSettings settings = listSettings[i];
                        //bool show = InspectorUIUtility.DrawSectionStart(themeItem.objectReferenceValue.name + " (Click to edit)", indentOnSectionStart + 3, showSettings, FontStyle.Normal, false);

                        /*
                        if (show != showSettings)
                        {
                            EditorPrefs.SetBool(prefKey, show);
                            settings.Show = show;
                        }*/

                        SerializedObject themeObj = new SerializedObject(themeItem.objectReferenceValue);
                        SerializedProperty themeObjSettings = themeObj.FindProperty("Settings");
                        themeObj.Update();

                        /*
                        if (themeObjSettings.arraySize < 1)
                        {
                            AddThemeProperty(new int[] { i, t, 0 });
                        }*/

                        int[] location = new int[] { i, t, 0 };

                        State[] iStates = GetStates();

                        ThemeInspector.RenderThemeSettings(themeObjSettings, themeObj, themeOptions, gameObject, location, iStates);

                        InspectorUIUtility.FlexButton(new GUIContent("+", "Add Theme Property"), location, AddThemeProperty);

                        ThemeInspector.RenderThemeStates(themeObjSettings, iStates, 30);

                        themeObj.ApplyModifiedProperties();

                        //InspectorUIUtility.DrawSectionEnd(indentOnSectionStart + 2);
                        listSettings[i] = settings;
                    }
                }

                EditorGUI.indentLevel = 0;

                EditorGUILayout.EndVertical();
            }

            if (GUILayout.Button(new GUIContent("Add Profile")))
            {
                AddProfile(profileList.arraySize);
            }
        }
    }
}
