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

//#define ToBeEnabled

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
using System.Text;
using FlaxEditor.Content.Thumbnails;
using FlaxEditor.Content.GUI;
using System.Xml.Serialization;
using System.Threading;
using System.Threading.Tasks;
using static FlaxEditor.GUI.ItemsListContextMenu;
using static BlenderLink.BLTCashe;

namespace BlenderLink
{
    /// <inheritdoc/>
    public class BlenderFileAssetItem : AssetItem
    {   /// <inheritdoc/>
        public Texture icon;
        public Color acolor;
        /// <inheritdoc/>
        public override ContentItemType ItemType => ContentItemType.Other;
        /// <inheritdoc/>
        public BlenderFileAssetItem(string path, string typeName, ref Guid id) : base(path, typeName, ref id){}
        /// <inheritdoc/>
        public override ContentItemSearchFilter SearchFilter => ContentItemSearchFilter.Other;
        /// <inheritdoc/>
        public override void Draw()
        {
            var size = Size;
            var style = Style.Current;
            var view = Parent as ContentView;
            var isSelected = view.IsSelected(this);
            var clientRect = new Rectangle(Float2.Zero, size);
            var textRect = TextRectangle;
            Rectangle thumbnailRect;
            TextAlignment nameAlignment;
            switch (view.ViewType)
            {
                case ContentViewType.Tiles:
                    {
                        var thumbnailSize = size.X;
                        thumbnailRect = new Rectangle(0, 0, thumbnailSize, thumbnailSize);
                        nameAlignment = TextAlignment.Center;

                        // Small shadow
                        var shadowRect = new Rectangle(2, 2, clientRect.Width + 1, clientRect.Height + 1);
                        var color = Color.Black.AlphaMultiplied(0.2f);
                        Render2D.FillRectangle(shadowRect, color);

                        Render2D.FillRectangle(clientRect, style.Background.RGBMultiplied(1.25f));
                        Render2D.FillRectangle(TextRectangle, style.LightBackground);

                        var accentHeight = 2 * view.ViewScale;
                        var barRect = new Rectangle(0, thumbnailRect.Height - accentHeight, clientRect.Width, accentHeight);
                        Render2D.FillRectangle(barRect, acolor);

                        Render2D.DrawTexture(icon, thumbnailRect);
                        if (isSelected)
                        {
                            Render2D.FillRectangle(textRect, Parent.ContainsFocus ? style.BackgroundSelected : style.LightBackground);
                            Render2D.DrawRectangle(clientRect, Parent.ContainsFocus ? style.BackgroundSelected : style.LightBackground);
                        }
                        else if (IsMouseOver)
                        {
                            Render2D.FillRectangle(textRect, style.BackgroundHighlighted);
                            Render2D.DrawRectangle(clientRect, style.BackgroundHighlighted);
                        }

                        break;
                    }
                case ContentViewType.List:
                    {
                        var thumbnailSize = size.Y - 2 * DefaultMarginSize;
                        thumbnailRect = new Rectangle(DefaultMarginSize, DefaultMarginSize, thumbnailSize, thumbnailSize);
                        nameAlignment = TextAlignment.Near;

                        if (isSelected)
                            Render2D.FillRectangle(clientRect, Parent.ContainsFocus ? style.BackgroundSelected : style.LightBackground);
                        else if (IsMouseOver)
                            Render2D.FillRectangle(clientRect, style.BackgroundHighlighted);

                        Render2D.DrawTexture(icon, thumbnailRect);
                        break;
                    }
                default: throw new ArgumentOutOfRangeException();
            }

            // Draw short name
            Render2D.PushClip(ref textRect);
            Render2D.DrawText(style.FontMedium, ShowFileExtension || view.ShowFileExtensions ? FileName : ShortName, textRect, style.Foreground, nameAlignment, TextAlignment.Center, TextWrapping.WrapWords, 1f, 0.95f);
            Render2D.PopClip();
            
        }
    }

    /// <summary>
    /// BlenderAssetProxy.
    /// </summary>
    public class BlenderAssetProxy : AssetProxy
    {
        Window w;
#if ToBeEnabled
        /// <inheritdoc />
        protected override bool IsVirtual => true;
#endif
        /// <inheritdoc />
        public override bool CanExport => false;
        /// <inheritdoc />
        public override bool IsAsset => false;
        /// <inheritdoc/>
        public override string TypeName => "Blender file Proxy";
        /// <inheritdoc/>
        public override string Name => "Blender file Proxy";
        /// <inheritdoc/>
        public override string FileExtension => "blend";
        /// <inheritdoc/>
        public override Color AccentColor => Color.Orange;
        /// <inheritdoc/>
        public override AssetItem ConstructItem(string path, string typeName, ref Guid id)
        {
            return new BlenderFileAssetItem(path, typeName, ref id)
            {
                icon = Content.Load<Texture>(Path.Combine(Globals.ProjectFolder, "Plugins\\Blender Link\\Content\\BlederIcons\\Main\\blender.flax")),
                acolor = AccentColor
            };
        }
        /// <inheritdoc/>
        public override bool CanReimport(ContentItem item)
        {
            return true;
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
        private void ImportSkinnedModelsOrAnimations(ContextMenu menu, ContentItem item)
        {
            menu.AddButton("Import", () =>
            {
                //OnExtractDataComplited(item);
                //return;
                //run blender in headlessmode
                string path = Path.Combine(BlenderLinkPlugin.PathToBlenderScripts, "ExtractData.py");
                var bi = new BlenderLink.BlenderInstance(item, BlenderLinkOptions.Options.PathToBlender)
                {
                    ScriptMode = true,
                    PathToBlenderPythonScript = path,
                };
                
                bi.ExecutionComplited = (BlenderInstance bi) =>
                {
                    OnExtractDataComplited(item);
                };

                bi.Run();
            }
);
        }

        private void OnExtractDataComplited(ContentItem item)
        {
            var p = Path.Combine(Globals.ProjectCacheFolder, "Blender Link Cache", item.NamePath.Remove(0, 8)) + ".BLTCashe";
            BLTCashe cashe = new(p);

            CreateWindowSettings settings = new CreateWindowSettings()
            {
                IsRegularWindow = false,
                IsTopmost = true,
                Title = "Blender Link",
                Size = new Float2(400, 800),
                AllowInput = true,
                ShowInTaskbar = true,
                StartPosition = WindowStartPosition.CenterParent,
                ActivateWhenFirstShown = true,
                HasBorder = true,
                //Parent = Editor.Instance.Windows.MainWindow
            };

            // Create window
            w = Platform.CreateWindow(ref settings);

            var windowGUI = w.GUI.AddChild<ScrollableControl>();
            windowGUI.IsScrollable = true;
            windowGUI.AnchorPreset = AnchorPresets.StretchAll;
            var Csize = cashe.MakeUI(windowGUI);
            Button Import = new Button()
            {
                AnchorPreset = AnchorPresets.HorizontalStretchBottom,
                Size = new Float2(0, 16),
                Location = new Float2(0, -18),
                Text = "Import"
            };
            var Hscrollbar = new VScrollBar(windowGUI, 0, 16);
            Hscrollbar.LocalLocation = new Float2(-8, 0);
            //Hscrollbar.Size = new Float2(16, w.Size.Y);
            Hscrollbar.Maximum = Csize.Y - (Csize.Y - w.Size.Y);
            Hscrollbar.Minimum = 0;
            Hscrollbar.ValueChanged += () =>
            {
                windowGUI.ViewOffset = new Float2(windowGUI.ViewOffset.X, -Hscrollbar.Value);
            };
            Import.Clicked += () =>
            {
                w.Close();
                cashe.Seralize(p);
                string path = Path.Combine(BlenderLinkPlugin.PathToBlenderScripts, "Export.py");
                var bi = new BlenderLink.BlenderInstance(item, BlenderLinkOptions.Options.PathToBlender)
                {
                    ScriptMode = true,
                    PathToBlenderPythonScript = path,
                };
                bi.ExecutionComplited += (BlenderInstance bi) =>
                {
                    var p = Path.Combine(Globals.ProjectCacheFolder, "Blender Link Cache", bi.Item.NamePath.Remove(0, 8)) + ".BLTCashe";
                    BLTCashe cashe = new(p);
                    BLTCashe.Line parent = cashe.GetCasheLine(0);

                    var FolderSourcePath = Path.Combine(Globals.ProjectContentFolder, Path.GetDirectoryName(item.NamePath.Remove(0, 8)));
                    var FolderImportContentFolder = (ContentFolder)Editor.Instance.ContentDatabase.Find(FolderSourcePath);
                    var SkinnedModelOptions = ModelTool.Options.Default;
                    SkinnedModelOptions.Type = ModelTool.ModelType.SkinnedModel;
                    var ModelOptions = ModelTool.Options.Default;
                    ModelOptions.Type = ModelTool.ModelType.Model;
                    var AnimationOptions = ModelTool.Options.Default;
                    AnimationOptions.Type = ModelTool.ModelType.Animation;
                    //EDImport(Path.Combine(FolderSourcePath, item.ShortName, parent.ObjectName, "Animation", "Clips"), line.ObjectName, AnimationOptions);
                    
                    //;
                    for (int i = 1; i < cashe.Lenght; i++)
                    {
                        Line line = cashe.GetCasheLine(i);
                        if (line.Import)
                        {
                            if (line.Type == Line.ObjectType.MESH)
                            {
                                if (line.HasTypeAsParent(Line.ObjectType.ARMATURE, out var hit))
                                {
                                    if (hit.Import)
                                    {
                                        EDImport(Path.Combine(FolderSourcePath, item.ShortName, hit.ObjectName), hit.ObjectName,ref SkinnedModelOptions);
                                    }
                                }
                                else
                                {
                                    EDImport(Path.Combine(FolderSourcePath, item.ShortName, line.ObjectName), line.ObjectName,ref ModelOptions);
                                }
                            }
                            else if (line.Type == Line.ObjectType.NLA_TRACK)
                            {
                                if (line.HasTypeAsParent(Line.ObjectType.ARMATURE, out var hit) || line.HasTypeAsParent(Line.ObjectType.MESH, out hit))
                                {
                                    if (hit.Import)
                                    {
                                        EDImport(Path.Combine(FolderSourcePath, item.ShortName, hit.ObjectName, "Animation", "NLA Tracks"), line.ObjectName,ref AnimationOptions);
                                    }
                                }
                            }
                            if (line.Type == Line.ObjectType.CLIP)
                            {
                                if (line.HasTypeAsParent(Line.ObjectType.ARMATURE, out var hit) || line.HasTypeAsParent(Line.ObjectType.MESH, out hit))
                                {
                                    if (hit.Import)
                                    {
                                        EDImport(Path.Combine(FolderSourcePath, item.ShortName, hit.ObjectName, "Animation", "Clips"), line.ObjectName,ref AnimationOptions);
                                    }
                                }
                            }
                        }
                    }
                };
                bi.Run();
            };
            w.GUI.AddChild(Import);
            //w.ClientSize = new(w.ClientSize.X + Csize.X, Csize.Y + Import.Size.Y + 32);

            w.GUI.PerformLayout(true);
            w.Show();
            w.Closed += () => { w = null; };
        }
        private void EDImport(string to, string objName,ref ModelTool.Options settings)
        {
            var f = (ContentFolder)Editor.Instance.ContentDatabase.Find(to);
            if (f != null)
            {
                Editor.Instance.ContentImporting.Import(Path.Combine(to, objName.Replace(".", "_") + ".fbx"), f, true, settings);
            }
            else
            {
                Debug.LogError("Faled to import:" + objName + "\nTo:"+ to);
            }
        }
    }
    /// <summary>
    /// BlenderAssetProxy.
    /// </summary>
    public class BlenderBackupAssetProxy : AssetProxy
    {
#if ToBeEnabled
        /// <inheritdoc />
        protected override bool IsVirtual => true;
#endif
        /// <inheritdoc />
        public override bool CanExport => false;
        /// <inheritdoc />
        public override bool IsAsset => false;
        /// <inheritdoc/>
        public override string TypeName => "Blender Backup file Proxy";
        /// <inheritdoc/>
        public override string Name => "Blender Backup file Proxy";
        /// <inheritdoc/>
        public override string FileExtension => "blend1";
        /// <inheritdoc/>
        public override Color AccentColor => Color.DarkOrange.RGBMultiplied(0.5f);
        /// <inheritdoc/>
        public override AssetItem ConstructItem(string path, string typeName, ref Guid id) 
        {
            return new BlenderFileAssetItem(path, typeName, ref id)
            {
                icon = Content.Load<Texture>(Path.Combine(Globals.ProjectFolder, "Plugins\\Blender Link\\Content\\BlederIcons\\Main\\file_backup.flax")),
                acolor = AccentColor
            };
        }
        /// <inheritdoc/>
        public override bool CanReimport(ContentItem item)
        {
            return true;
        }
        /// <inheritdoc/>
        public override void OnContentWindowContextMenu(ContextMenu menu, ContentItem item)
        {
            //remove opcion
            for (int i = 0; i < menu.ItemsContainer.Children.Count; i++)
            {
                if (menu.ItemsContainer.Children[i] is ContextMenuButton bt)
                {
                    if (bt.Text == "Reimport" || bt.Text == "Open")
                    {
                        menu.ItemsContainer.Children.RemoveAt(i);
                        i--;
                    }
                }
            }
        }
        /// <inheritdoc/>
        public override bool IsProxyFor(ContentItem item)
        {
            return item.FileName.EndsWith(".blend1");
        }

        /// <inheritdoc/>
        public override EditorWindow Open(Editor editor, ContentItem item)
        {
            //new BlenderLink.BlenderInstance(item, BlenderLinkOptions.Options.PathToBlender).Run();
            return null;
        }
    }


    public class PythonScriptProxy : AssetProxy
    {
#if ToBeEnabled
        /// <inheritdoc />
        protected override bool IsVirtual => true;
#endif
        /// <inheritdoc />
        public override bool CanExport => false;
        /// <inheritdoc />
        public override bool IsAsset => false;
        /// <inheritdoc/>
        public override string TypeName => "Python Script file Proxy";
        /// <inheritdoc/>
        public override string Name => "Python Script file Proxy";
        /// <inheritdoc/>
        public override string FileExtension => "py";
        /// <inheritdoc/>
        public override Color AccentColor => Color.Blue;
        /// <inheritdoc/>
        public override AssetItem ConstructItem(string path, string typeName, ref Guid id)
        {
            return new BlenderFileAssetItem(path, typeName, ref id)
            {
                icon = Content.Load<Texture>(Path.Combine(Globals.ProjectFolder, "Plugins\\Blender Link\\Content\\BlederIcons\\Main\\file_script.flax")),
                acolor = AccentColor
            };
        }
        /// <inheritdoc/>
        public override bool CanReimport(ContentItem item)
        {
            return true;
        }
        /// <inheritdoc/>
        public override void OnContentWindowContextMenu(ContextMenu menu, ContentItem item)
        {
            //remove opcion
            for (int i = 0; i < menu.ItemsContainer.Children.Count; i++)
            {
                if (menu.ItemsContainer.Children[i] is ContextMenuButton bt)
                {
                    if (bt.Text == "Reimport")
                    {
                        menu.ItemsContainer.Children.RemoveAt(i);
                        i--;
                    }
                }
            }
        }
        /// <inheritdoc/>
        public override bool IsProxyFor(ContentItem item)
        {
            return item.FileName.EndsWith(".py");
        }

        /// <inheritdoc/>
        public override EditorWindow Open(Editor editor, ContentItem item)
        {
            //new BlenderLink.BlenderInstance(item, BlenderLinkOptions.Options.PathToBlender).Run();
            return null;
        }
    }

}
