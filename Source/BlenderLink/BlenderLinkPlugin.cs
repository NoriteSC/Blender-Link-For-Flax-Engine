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
    /// <summary>
    /// Initializes a new instance of the <see cref="BlenderLinkPlugin"/> class.
    /// </summary>
    public BlenderLinkPlugin()
    {
        // Initialize plugin description
        _description = new PluginDescription
        {
            Name = "Blender Link",
            Category = "Tools",
            Description = "batch import of models and animations from single blend file,\nAPI for running scripts in blender",
            Author = "Norite SC",
            AuthorUrl = "https://github.com/NoriteSC",
            RepositoryUrl = "https://github.com/NoriteSC/Blender-Link-For-Flax-Engine?tab=readme-ov-file",
            IsAlpha = true,
            Version = new Version(0,0,1,0),
        };
    }
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