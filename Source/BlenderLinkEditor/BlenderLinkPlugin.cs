using System;
using FlaxEngine;
using FlaxEditor;
using FlaxEditor.Options;
using FlaxEngine.Json;
using System.IO;
using FlaxEngine.GUI;

namespace BlenderLink;
/// <summary>
/// </summary>
public class BlenderLinkPlugin : EditorPlugin
{
    /// <inheritdoc/>
    public BlenderAssetProxy proxy;
    /// <inheritdoc/>
    public static string PathToBlenderScripts { get; private set; }
    /// <inheritdoc/>
    public override void InitializeEditor()
    {
        PathToBlenderScripts = Path.Combine(Globals.ProjectFolder, "Plugins\\Blender Link\\Source\\BlenderScripts\\");
        Editor.Options.AddCustomSettings("Blender Link", new OptionsModule.CreateCustomSettingsDelegate(() => { return BlenderLinkOptions.Options; }));
        Editor.ContentDatabase.AddProxy(proxy = new BlenderAssetProxy());
        Editor.Options.OptionsChanged += Options_OptionsChanged;
        BlenderLinkOptions.Load();
    }
    private void Options_OptionsChanged(EditorOptions obj)
    {
    }
    /// <inheritdoc/>
    public override void DeinitializeEditor()
    {
        Editor.Options.OptionsChanged -= Options_OptionsChanged;
        Editor.Options.RemoveCustomSettings("Blender Link");
        Editor.ContentDatabase.RemoveProxy(proxy);
    }
}