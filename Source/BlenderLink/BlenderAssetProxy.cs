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
using FlaxEditor.Content;
using FlaxEditor.Windows;
using FlaxEditor.GUI.ContextMenu;
using System.IO;
using FlaxEngine.GUI;
using System.Collections.Generic;
using FlaxEngine.Tools;
using FlaxEditor.Content.Import;

namespace BlenderLink
{
    /// <inheritdoc/>
    public struct BlenderFileRefrence {}

    /// <summary>
    /// BlenderFileFactory Script.
    /// </summary>
    public class BlenderAssetProxy : AssetProxy
    {
        Window w;
        /// <inheritdoc/>
        public override string TypeName => typeof(BlenderFileRefrence).Name;
        /// <inheritdoc/>
        public override string Name => "Blender file Proxy";
        /// <inheritdoc/>
        public override string FileExtension => "blend";
        /// <inheritdoc/>
        public override Color AccentColor => Color.Orange;
        /// <inheritdoc/>
        public override AssetItem ConstructItem(string path, string typeName, ref Guid id) { return null; }
        /// <inheritdoc/>
        public override bool CanReimport(ContentItem item)
        {
            return false;
        }
        /// <inheritdoc/>
        public override void OnContentWindowContextMenu(ContextMenu menu, ContentItem item)
        {
            //remove Reimport opcion
            //and rename open to open blender
            for (int i = 0; i < menu.ItemsContainer.Children.Count; i++)
            {
                if (menu.ItemsContainer.Children[i] is ContextMenuButton bt)
                {
                    if (bt.Text == "Reimport")
                    {
                        menu.ItemsContainer.Children.RemoveAt(i);
                        i--;
                    }
                    if (bt.Text == "Open")
                    {
                        bt.Text = "Open blender";
                    }
                }
            }
            menu.AddSeparator();
            ImportSkinnedModelsOrAnimations(menu, item);
            menu.AddButton("Import Models");
            menu.AddSeparator();
        }
        /// <inheritdoc/>
        public override bool IsProxyFor(ContentItem item)
        {
            return item.FileName.EndsWith(".blend");
        }
        /// <inheritdoc/>
        public override EditorWindow Open(Editor editor, ContentItem item)
        {
            new BlenderLink.BlenderInstance(item, BlenderLinkOptions.Options.PathToBlender).Run();
            return null;
        }

        private CheckBox MakeEntry(bool Checked, string Name, int ID, CheckBox parent,string Type)
        {
            var index = w.GUI.ChildrenCount;
            var c = w.GUI.AddChild<CheckBox>();
            if (parent != null)
            {
                c.Location = new Float2(c.Location.X + c.Size.X, c.Size.Y * ID);
                c.Checked = Checked;
                parent.StateChanged += (CheckBox root) => { ((CheckBox)w.GUI.Children[index]).Checked = root.Checked; };
            }
            else
            {
                c.Location = new Float2(c.Location.X, c.Size.Y * ID);
                c.Checked = Checked;
            }
            var l = w.GUI.AddChild<Label>();
            l.Text = Name;
            l.HorizontalAlignment = TextAlignment.Near;
            l.Location = new Float2(c.Location.X + c.Size.X, c.Location.Y);

            var t = w.GUI.AddChild<Label>();
            t.Text = Type;
            t.AutoWidth = true;
            t.HorizontalAlignment = TextAlignment.Near;
            t.AnchorPreset = AnchorPresets.TopRight;
            t.UpdateBounds();
            t.Location = new Float2(0, l.Location.Y);
            return c;
        }

        private void ImportSkinnedModelsOrAnimations(ContextMenu menu, ContentItem item)
        {
            menu.AddButton("Import Skinned Models,\nAnimations", () =>
            {
                //run blender in headlessmode
                string path = Path.Combine(BlenderLinkPlugin.PathToBlenderScripts, "ExtractAnimationData.py");
                var bi = new BlenderLink.BlenderInstance(item, BlenderLinkOptions.Options.PathToBlender)
                {
                    ScriptMode = true,
                    PathToBlenderPythonScript = path,
                };

                bi.ExecutionComplited = (BlenderInstance bi) =>
                {
                    var p = Path.Combine(Globals.ProjectCacheFolder, "Blender Link Cache", bi.Item.NamePath.Remove(0, 8)) + ".BLTCashe";
                    string[] lines = File.ReadAllLines(p);
                    CheckBox parent = null;


                    CreateWindowSettings settings = new CreateWindowSettings()
                    {
                        IsRegularWindow = false,
                        IsTopmost = true,
                        Title = "Blender Link",
                        Size = new Float2(400, 600),
                        AllowInput = true,
                        ShowInTaskbar = true,
                        StartPosition = WindowStartPosition.CenterParent,
                        ActivateWhenFirstShown = true,
                        HasBorder = true,
                        //Parent = Editor.Instance.Windows.MainWindow
                    };

                    // Create window
                    w = Platform.CreateWindow(ref settings);
                    var windowGUI = w.GUI;



                    CheckBox[] boxes = new CheckBox[lines.Length];
                    for (int i = 0; i < lines.Length; i++)//last is "\n" 
                    {
                        if (lines[i].StartsWith("--")) //--<state> <objname>
                        {
                            boxes[i] = MakeEntry(lines[i][2] == 'O',lines[i].Remove(0, 4), i, parent, "[Animation]");
                        }
                        else if (lines[i].StartsWith("-")) //-<state> <objname>
                        {
                            boxes[i] = parent = MakeEntry(lines[i][1] == 'O', lines[i].Remove(0, 3), i, null, "[SkinnedModel]");
                        }
                    }

                    //Debug.Log(final);
                    Button Import = new Button()
                    {
                        AnchorPreset = AnchorPresets.HorizontalStretchBottom,
                        Size = new Float2(0, 16),
                        Location = new Float2(0, -18),
                        Text = "Import"
                    };
                    Import.Clicked += () =>
                    {

                        for (int i = 0; i < lines.Length; i++)
                        {
                            if (lines[i].StartsWith("--")) //- X <objname> (subdata)
                            {
                                var c = lines[i].ToCharArray();
                                c[2] = (boxes[i].Checked ? 'O' : 'X');
                                lines[i] = new string(c);
                            }
                            else if (lines[i].StartsWith("-")) //X <objname> (object)
                            {
                                var c = lines[i].ToCharArray();
                                c[1] = (boxes[i].Checked ? 'O' : 'X');
                                lines[i] = new string(c);
                            }
                        }
                        File.WriteAllLines(p, lines);
                        w.Close();

                        string path = Path.Combine(BlenderLinkPlugin.PathToBlenderScripts, "ExportAnimations.py");
                        var bi = new BlenderLink.BlenderInstance(item, BlenderLinkOptions.Options.PathToBlender)
                        {
                            ScriptMode = true,
                            PathToBlenderPythonScript = path,
                        };
                        bi.ExecutionComplited += (BlenderInstance bi) =>
                        {
                            var op = ModelTool.Options.Default;
                            op.Type = ModelTool.ModelType.Animation;

                            var Aop = ModelTool.Options.Default;
                            Aop.Type = ModelTool.ModelType.SkinnedModel;
                            var SkinnedModelPath = Path.Combine(Globals.ProjectContentFolder, Path.GetDirectoryName(item.NamePath.Remove(0, 8)));
                            var AnimationPath = Path.Combine(SkinnedModelPath, "Animations");

                            var AnimationContentFolder = (ContentFolder)Editor.Instance.ContentDatabase.Find(AnimationPath);
                            var SkinnedModelImportContentFolder = (ContentFolder)Editor.Instance.ContentDatabase.Find(SkinnedModelPath);
                            for (int i = 0; i < lines.Length; i++)
                            {
                                if (lines[i].StartsWith("--")) //- X <objname> (subdata)
                                {
                                    var FinalAnimationPath = Path.Combine(AnimationPath, lines[i].Remove(0, 4) + ".fbx");
                                    Editor.Instance.ContentImporting.Import(FinalAnimationPath, AnimationContentFolder, true, op);
                                }
                                if (lines[i].StartsWith("-")) //X <objname> (object)
                                {
                                    var p = Path.Combine(SkinnedModelPath, lines[i].Remove(0, 3) + ".fbx");
                                    Editor.Instance.ContentImporting.Import(p, SkinnedModelImportContentFolder, true, Aop);
                                }
                            }
                        };
                        bi.Run();
                    };
                    w.GUI.AddChild(Import);
                    w.ClientSize = new(w.ClientSize.X, (lines.Length * 18) + Import.Size.Y + 32);

                    w.GUI.PerformLayout(true);
                    w.Show();
                    w.Closed += () => { w = null; };
                };

                bi.Run();
            }
);
        }
    }

}
