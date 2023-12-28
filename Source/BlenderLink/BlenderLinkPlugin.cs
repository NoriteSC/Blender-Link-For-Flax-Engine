//-------------------------------------------------------------------------------------------------------------------------
// ________  ___       _______   ________   ________  _______   ________          ___       ___  ________   ___  __       
//|\   __  \|\  \     |\  ___ \ |\   ___  \|\   ___ \|\  ___ \ |\   __  \        |\  \     |\  \|\   ___  \|\  \|\  \     
//\ \  \|\ /\ \  \    \ \   __/|\ \  \\ \  \ \  \_|\ \ \   __/|\ \  \|\  \       \ \  \    \ \  \ \  \\ \  \ \  \/  /|_   
// \ \   __  \ \  \    \ \  \_|/_\ \  \\ \  \ \  \ \\ \ \  \_|/_\ \   _  _\       \ \  \    \ \  \ \  \\ \  \ \   ___  \  
//  \ \  \|\  \ \  \____\ \  \_|\ \ \  \\ \  \ \  \_\\ \ \  \_|\ \ \  \\  \|       \ \  \____\ \  \ \  \\ \  \ \  \\ \  \ 
//   \ \_______\ \_______\ \_______\ \__\\ \__\ \_______\ \_______\ \__\\ _\        \ \_______\ \__\ \__\\ \__\ \__\\ \__\
//    \|_______|\|_______|\|_______|\|__| \|__|\|_______|\|_______|\|__|\|__|        \|_______|\|__|\|__| \|__|\|__| \|__|
//                                                                                                                        
//-------------------------------------------------------------------------------------------------------------------------
//                                                    writen by Nori_SC
//                                                https://github.com/NoriteSC

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