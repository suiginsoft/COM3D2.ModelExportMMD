using COM3D2.ModelExportMMD.Extensions;
using COM3D2.ModelExportMMD.Gui;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using UnityEngine;
using BepInEx;
using BepInEx.Configuration;

namespace COM3D2.ModelExportMMD.Plugin
{
    [BepInPlugin("pleaserespond.mmdexport", "COM3D2 Model Export to MMD", "3.1")]
    public class ModelExportPlugin : BaseUnityPlugin
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
        private ConfigEntry<string> configFolderPath;
        private ConfigEntry<ModelExportEventArgs.ExporterClass> configFormat;
        private ConfigEntry<bool> configSavePosition;
        private ConfigEntry<bool> configSaveTextures;

        #endregion

        #region Methods

        public void Awake()
        {
            string defaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Maids");
            configFolderPath = Config.Bind(IniSection, IniKeyFolderPath, defaultPath, "Output folder");
            configFormat = Config.Bind(IniSection, IniKeyFormat, ModelExportEventArgs.ExporterClass.PmxA, "File format");
            configSavePosition = Config.Bind(IniSection, IniKeySavePosition, true, "Save position");
            configSaveTextures = Config.Bind(IniSection, IniKeySaveTextures, true, "Save textures");
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.F8))
            {
                window.Show();
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
            if (window == null)
            {
                window = new ModelExportWindow()
                {
                    PluginVersion = ((BepInPlugin)GetType().GetCustomAttributes(typeof(BepInPlugin), false)[0]).Version.ToString(),
                    ExportFolderPath = configFolderPath.Value,
                    ExportName = "Maid",
                    ExportClass = configFormat.Value,
                    SavePostion = configSavePosition.Value,
                    SaveTextures = configSaveTextures.Value,
                };

                window.BrowseClicked += BrowseForExportFolder;
                window.ApplyTPoseClicked += ApplyTPose;
                window.ExportClicked += ExportModel;
                window.CloseClicked += delegate (object s, EventArgs a) { SaveUserPreferences(); };
            }
            window.DrawWindow();
        }

        private void SaveUserPreferences()
        {
            try
            {
                configFolderPath.Value = window.ExportFolderPath;
                configFormat.Value = window.ExportClass;
                configSavePosition.Value = window.SavePostion;
                configSaveTextures.Value = window.SaveTextures;
                Config.Save();
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
            if (window.ExportClass == ModelExportEventArgs.ExporterClass.Obj)
            {
                dialog.FilterIndex = 2;
            }
            else
            {
                dialog.FilterIndex = 1;
            }
            dialog.FileName = window.ExportName;
            dialog.InitialDirectory = window.ExportFolderPath;
            if (!Directory.Exists(dialog.InitialDirectory))
            {
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }

            var result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                window.ExportFolderPath = Path.GetDirectoryName(dialog.FileName);
                window.ExportName = Path.GetFileNameWithoutExtension(dialog.FileName);
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
                var meshes = FindObjectsOfType<SkinnedMeshRenderer>()
                    .Where(smr => smr.name != "obj1")
                    .Distinct()
                    .ToList();

                IExporter exporter;

                switch (args.Exporter)
                {
                    case ModelExportEventArgs.ExporterClass.Obj:
                        exporter = new ObjExporter();
                        break;
                    case ModelExportEventArgs.ExporterClass.PmxA:
                        exporter = new PmxExporter();
                        break;
                    case ModelExportEventArgs.ExporterClass.PmxB:
                        exporter = new PmxBuilder();
                        break;
                    default:
                        throw new Exception($"Unknown model format: {args.Exporter}");
                }

                exporter.ExportFolder = args.Folder;
                exporter.ExportName = args.Name;
                exporter.SavePosition = args.SavePosition;
                exporter.SaveTexture = args.SaveTexture;
                exporter.Export(meshes);
            }
            catch (Exception error)
            {
                Debug.LogError($"Error exporting {args.Exporter}: {error.Message}\n\nStack trace:\n{error.StackTrace}");
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