// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using UnityEditor;

/// <summary>
/// TODO: Troy - fill in
/// </summary>
public static class MixedRealityInputActions
{
    private const string RELATIVE_FILE_PATH = "ProjectPreferences.asset";
    private static string FilePath => MixedRealityToolkitFiles.MapRelativeFilePath(MixedRealityToolkitModuleType.Core, RELATIVE_FILE_PATH);

    private static MixedRealityInputActionsProfile defaultActionsProfile;

    /// <summary>
    /// TODO: Troy - Fill in
    /// </summary>
    public static MixedRealityInputActionsProfile Default
    {
        get
        {
            if (defaultActionsProfile == null)
            {
                defaultActionsProfile = AssetDatabase.LoadAssetAtPath<MixedRealityInputActionsProfile>(FilePath);
            }

            return defaultActionsProfile;
        }
    }

}
