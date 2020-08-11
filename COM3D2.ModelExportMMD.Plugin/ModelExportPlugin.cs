using COM3D2.ModelExportMMD.Extensions;
using COM3D2.ModelExportMMD.Gui;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using UnityEngine;
using UnityInjector;
using UnityInjector.Attributes;

namespace COM3D2.ModelExportMMD.Plugin
{
    [PluginName("COM3D2 Model Export to MMD")]
    [PluginVersion("2.0")]
    [PluginFilter("COM3D2OHx64")]
    [PluginFilter("COM3D2VRx64")]
    [PluginFilter("COM3D2OHVRx64")]
    [PluginFilter("COM3D2x64")]
    public class ModelExportPlugin : PluginBase
    {
        #region Constants

        private const string IniSection = "ExportPreferences";
        private const string IniKeyFolderPath = "FolderPath";
        private const string IniKeyFormat = "Format";
        private const string IniKeySavePosition = "SavePosition";
        private const string IniKeySaveTextures = "SaveTextures";

        #endregion

        #region Fields

        private ModelExportWindow window;

        #endregion

        #region Methods

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.F8))
            {
                Debug.Assert(this.window != null);
                this.window.Show();
            }

        #if DEBUG

            if (Input.GetKeyDown(KeyCode.F9))
            {
                DumpGameObjects();
            }

            if (Input.GetKeyDown(KeyCode.F10))
            {
                DumpBodyModel();
            }

        #endif
        }

        public void OnGUI()
        {
            if (this.window == null)
            {
                this.window = new ModelExportWindow()
                {
                    PluginVersion = ((PluginVersionAttribute)GetType().GetCustomAttributes(typeof(PluginVersionAttribute), false)[0]).Version,
                    ExportFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Maids"),
                    ExportName = "Maid",
                    SavePostion = true,
                    SaveTextures = true
                };

                this.window.BrowseClicked += BrowseForExportFolder;
                this.window.ApplyTPoseClicked += ApplyTPose;
                this.window.ExportClicked += ExportModel;
                this.window.CloseClicked += delegate(object s, EventArgs a) { SaveUserPreferences(); };

                LoadUserPreferences();
            }

            this.window.DrawWindow();
        }

        private void LoadUserPreferences()
        {
            try
            {
                var iniSection = this.Preferences.GetSection(IniSection);
                if (iniSection != null)
                {
                    var iniExportFolder = iniSection.GetKey(IniKeyFolderPath);
                    if (iniExportFolder != null && iniExportFolder.RawValue != null)
                    {
                        this.window.ExportFolderPath = iniExportFolder.RawValue;
                    }

                    var iniFormat = iniSection.GetKey(IniKeyFormat);
                    if (iniFormat != null && !string.IsNullOrEmpty(iniFormat.RawValue))
                    {
                        try
                        {
                            // NOTE: .NET Framework 3.5 doesn't support Enum.TryParse
                            this.window.ExportFormat = (ModelFormat)Enum.Parse(typeof(ModelFormat), iniFormat.RawValue, true);
                        }
                        catch
                        {
                            // ignore and fallthrough
                        }
                    }

                    var iniSavePosition = iniSection.GetKey(IniKeySavePosition);
                    if (iniSavePosition != null && bool.TryParse(iniSavePosition.RawValue, out bool savePosition))
                    {
                        this.window.SavePostion = savePosition;
                    }

                    var iniSaveTextures = iniSection.GetKey(IniKeySaveTextures);
                    if (iniSaveTextures != null && bool.TryParse(iniSaveTextures.RawValue, out bool saveTextures))
                    {
                        this.window.SaveTextures = saveTextures;
                    }
                }
            }
            catch (Exception error)
            {
                Debug.LogError($"Error loading user exporter preferences: {error.Message}\n\nStack trace:\n{error.StackTrace}");
            }
        }

        private void SaveUserPreferences()
        {
            try
            {
                var iniSection = this.Preferences.CreateSection(IniSection);
                iniSection.CreateKey(IniKeyFolderPath).Value = this.window.ExportFolderPath;
                iniSection.CreateKey(IniKeyFormat).Value = this.window.ExportFormat.ToString();
                iniSection.CreateKey(IniKeySavePosition).Value = this.window.SavePostion.ToString();
                iniSection.CreateKey(IniKeySaveTextures).Value = this.window.SaveTextures.ToString();
                SaveConfig();
            }
            catch (Exception error)
            {
                Debug.LogError($"Error saving user exporter preferences: {error.Message}\n\nStack trace:\n{error.StackTrace}");
            }
        }

        private void BrowseForExportFolder(object sender, EventArgs args)
        {
            var dialog = new SaveFileDialog();
            dialog.Title = "Select the folder where the model and textures will be exported";
            dialog.Filter = "MikuMikuDance (*.pmx)|*.pmx|Wavefront (*.obj)|*.obj|All files (*.*)|*.*";
            dialog.FilterIndex = (int)this.window.ExportFormat + 1;
            dialog.FileName = this.window.ExportName;
            dialog.InitialDirectory = this.window.ExportFolderPath;
            if (!Directory.Exists(dialog.InitialDirectory))
            {
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }

            var result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                this.window.ExportFolderPath = Path.GetDirectoryName(dialog.FileName);
                this.window.ExportName = Path.GetFileNameWithoutExtension(dialog.FileName);
                this.window.ExportFormat = (ModelFormat)(1 <= dialog.FilterIndex && dialog.FilterIndex <= 2 ? dialog.FilterIndex - 1 : 0);
            }
        }

        private void ApplyTPose(object sender, EventArgs args)
        {
            try
            {
                var maid = GameMain.Instance.CharacterMgr.GetMaid(0);
                maid.ApplyTPose();
            }
            catch (Exception error)
            {
                Debug.LogError($"Error applying T-pose: {error.Message}\n\nStack trace:\n{error.StackTrace}");
            }
        }

        private void ExportModel(object sender, ModelExportEventArgs args)
        {
            try
            {
                SaveUserPreferences();

                var maid = GameMain.Instance.CharacterMgr.GetMaid(0);
                var materialState = maid.PrepareMaterialsForExport();

                try
                {
                    var meshes = FindObjectsOfType<SkinnedMeshRenderer>()
                        .Where(smr => smr.name != "obj1")
                        .Distinct()
                        .ToList();

                    IExporter exporter;

                    switch (args.Format)
                    {
                        case ModelFormat.Pmx:
                            exporter = new PmxExporter();
                            break;
                        case ModelFormat.Obj:
                            exporter = new ObjExporter();
                            break;
                        default:
                            throw new Exception($"Unknown model format: {args.Format}");
                    }

                    exporter.ExportFolder = args.Folder;
                    exporter.ExportName = args.Name;
                    exporter.SavePostion = args.SavePosition;
                    exporter.SaveTexture = args.SaveTexture;
                    exporter.Export(meshes);
                }
                finally
                {
                    maid.RestoreMaterialsAfterExport(materialState);
                }
            }
            catch (Exception error)
            {
                Debug.LogError($"Error exporting {args.Format}: {error.Message}\n\nStack trace:\n{error.StackTrace}");
            }
        }

        private void DumpGameObjects()
        {
            var gameObjects = FindObjectsOfType<GameObject>()
                .OrderBy(go => go.name)
                .ToList();

            using (var writer = new StreamWriter(@"GameObjects.txt"))
            {
                if (gameObjects.Count == 0)
                {
                    writer.WriteLine("No game objects found");
                    return;
                }

                var printed = new HashSet<Component>();

                foreach (var go in gameObjects)
                {
                    writer.WriteLine(go.name + "[" + go.GetInstanceID() + "]");
                    foreach (var component in go.GetComponents<Component>())
                    {
                        DumpComponents(writer, component, ".", printed);
                    }
                }
            }
        }

        private void DumpBodyModel()
        {
            var gameObject = FindObjectsOfType<SkinnedMeshRenderer>()
                .Select(smr => smr.gameObject)
                .Where(go => go.name.StartsWith("body"))
                .FirstOrDefault();

            using (var writer = new StreamWriter(@"BodyModel.txt"))
            {
                if (gameObject == null)
                {
                    writer.WriteLine("No body model found");
                    return;
                }

                writer.WriteLine($"Component tree of {gameObject.name}");

                var printed = new HashSet<Component>();

                var components = new List<Component>();
                gameObject.GetComponents(components);

                foreach (var component in components)
                {
                    DumpComponents(writer, component, ".", printed);
                }
            }
        }

        private void DumpComponents(StreamWriter writer, Component root, string indent, HashSet<Component> printed)
        {
            writer.WriteLine($"{indent} {root.name}({root.GetType().Name})");

            var subindent = indent + indent[0];

            if (printed.Add(root))
            {
                var components = new List<Component>();
                root.GetComponents(components);

                foreach (var component in components)
                {
                    DumpComponents(writer, component, subindent, printed);
                }
            }
            else
            {
                writer.WriteLine($"{subindent} [already printed]");
            }
        }

        #endregion
    }
}