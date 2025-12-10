using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static BuildMenuScript;

public class SymbolScript
{
    public static void ChangeSymbols(string addSymbols)
    {
        RemoveSymbols(BuildSettings.GetAllSymbol());
        AddSymbols(new List<string> { addSymbols });
    }

    private static void RemoveSymbols(List<string> symbols)
    {
        var currentGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
        var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(currentGroup);
        var definesList = new List<string>(defines.Split(';'));

        foreach (var symbol in symbols)
        {
            if (definesList.Contains(symbol))
            {
                definesList.Remove(symbol);
                Debug.Log($"Symbol '{symbol}' is now DISABLED. Reverted to default manifest.");
            }
        }

        PlayerSettings.SetScriptingDefineSymbolsForGroup(currentGroup, string.Join(";", definesList.ToArray()));
    }

    private static void AddSymbols(List<string> symbols)
    {
        var currentGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
        var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(currentGroup);
        var definesList = new List<string>(defines.Split(';'));

        foreach (var symbol in symbols)
        {
            if (!definesList.Contains(symbol)) 
            {
                definesList.Add(symbol);
                Debug.Log($"Symbol '{symbol}' is now ENABLED with custom manifest.");
            }
        }

        PlayerSettings.SetScriptingDefineSymbolsForGroup(currentGroup, string.Join(";", definesList.ToArray()));
    }
}
