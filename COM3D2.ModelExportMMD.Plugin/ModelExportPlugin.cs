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

        private static readonly string[] TPoseBonesToReset = new string[]
        {
            "Bip01",
            "Bip01 Footsteps",
            "Bip01 Head",
            "Bip01 Neck",
            "Bip01 Spine1a",
            "Bip01 Spine1",
            "Bip01 Spine0a",
            "Bip01 Spine",
            "Bip01 Pelvis",
            "Bip01 L Thigh",
            "Bip01 L Calf",
            "Bip01 L Foot",
            "Bip01 L Toe0",
            "Bip01 L Toe01",
            "Bip01 L Toe1",
            "Bip01 L Toe11",
            "Bip01 L Toe2",
            "Bip01 L Toe21",
            "Bip01 R Thigh",
            "Bip01 R Calf",
            "Bip01 R Foot",
            "Bip01 R Toe0",
            "Bip01 R Toe1",
            "Bip01 R Toe11",
            "Bip01 R Toe2",
            "Bip01 R Toe21",
            "Bip01 L Clavicle",
            "Bip01 L UpperArm",
            "Bip01 L Forearm",
            "Bip01 L Hand",
            "_IK_handL",
            "Bip01 L Finger0",
            "Bip01 L Finger01",
            "Bip01 L Finger02",
            "Bip01 L Finger1",
            "Bip01 L Finger11",
            "Bip01 L Finger12",
            "Bip01 L Finger2",
            "Bip01 L Finger21",
            "Bip01 L Finger22",
            "Bip01 L Finger3",
            "Bip01 L Finger32",
            "Bip01 L Finger4",
            "Bip01 L Finger41",
            "Bip01 L Finger42",
            "Bip01 R Clavicle",
            "Bip01 R UpperArm",
            "Bip01 R Forearm",
            "Bip01 R Hand",
            "_IK_handR",
            "Bip01 R Finger0",
            "Bip01 R Finger01",
            "Bip01 R Finger02",
            "Bip01 R Finger1",
            "Bip01 R Finger11",
            "Bip01 R Finger12",
            "Bip01 R Finger2",
            "Bip01 R Finger21",
            "Bip01 R Finger22",
            "Bip01 R Finger3",
            "Bip01 R Finger31",
            "Bip01 R Finger32",
            "Bip01 R Finger4",
            "Bip01 R Finger41",
            "Bip01 R Finger42"
        };

        private static readonly Dictionary<string, Quaternion> TPoseBoneTransformRotations = new Dictionary<string, Quaternion>
        {
            { "Bip01", Quaternion.Euler(-90f, 0f, 90f) },
            { "Bip01 Head", Quaternion.Euler(0f, 0f, 15.8494f) },
            { "Bip01 Neck", Quaternion.Euler(0f, 0f, 342.2753f) },
            { "Bip01 Spine", Quaternion.Euler(-90f, 180f, -90f) },
            { "Bip01 R Thigh", Quaternion.Euler(0f, 180f, 0f) },
            { "Bip01 L Thigh", Quaternion.Euler(0f, 180f, 0f) },
            { "Bip01 Pelvis", Quaternion.Euler(-90f, 90f, 0f) },
            { "Bip01 R Clavicle", Quaternion.Euler(180f, -90f, 0f) },
            { "Bip01 L Clavicle", Quaternion.Euler(180f, 90f, 0f) },
            { "Bip01 R UpperArm", Quaternion.Euler(0f, 30f, 0f) },
            { "Bip01 L UpperArm", Quaternion.Euler(0f, -30f, 0f) },
            { "Bip01 R Hand", Quaternion.Euler(90f, 0f, 0f) },
            { "Bip01 L Hand", Quaternion.Euler(-90f, 0f, 0f) },
            { "Bip01 R Finger0", Quaternion.Euler(-70f, 55f, -30f) },
            { "Bip01 L Finger0", Quaternion.Euler(70f, -55f, -30f) },
            { "Bip01 L Toe0", Quaternion.Euler(10f, 0f, -80f) },
            { "Bip01 L Toe01", Quaternion.Euler(0f, 0f, 0f) },
            { "Bip01 L Toe1", Quaternion.Euler(-5f, 0f, -80f) },
            { "Bip01 L Toe11", Quaternion.Euler(0f, 0f, 0f) },
            { "Bip01 L Toe2", Quaternion.Euler(0f, 0f, -80f) },
            { "Bip01 L Toe21", Quaternion.Euler(0f, 0f, 0f) },
            { "Bip01 R Toe0", Quaternion.Euler(-10f, 0f, -80f) },
            { "Bip01 R Toe01", Quaternion.Euler(0f, 0f, 0f) },
            { "Bip01 R Toe1", Quaternion.Euler(5f, 0f, -80f) },
            { "Bip01 R Toe11", Quaternion.Euler(0f, 0f, 0f) },
            { "Bip01 R Toe2", Quaternion.Euler(0f, 0f, -80f) },
            { "Bip01 R Toe21", Quaternion.Euler(0f, 0f, 0f) },
        };

        #endregion

        #region Fields

        private ModelExportWindow window;

        #endregion

        #region Methods

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.F8) && window != null)
            {
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
                            this.window.ExportFormat = (ModelExportFormat)Enum.Parse(typeof(ModelExportFormat), iniFormat.RawValue, true);
                        }
                        catch { /* ignore and fallthrough*/ }
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
            dialog.Filter = "MMD Files(*.pmx)|*.pmx|OBJ Files(*.obj)|*.obj|All files (*.*)|*.*";
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
                this.window.ExportFormat = (ModelExportFormat)(1 <= dialog.FilterIndex && dialog.FilterIndex <= 2 ? dialog.FilterIndex - 1 : 0);
            }
        }

        private void ApplyTPose(object sender, EventArgs args)
        {
            Maid maid = GameMain.Instance.CharacterMgr.GetMaid(0);
            maid.body0.m_Bones.GetComponent<Animation>().Stop();

            Transform rootTransform = maid.body0.m_Bones.transform;

            foreach (var boneName in TPoseBonesToReset)
            {
                Transform transform = CMT.SearchObjName(rootTransform, boneName);
                transform.localRotation = Quaternion.identity;
            }

            foreach (var entry in TPoseBoneTransformRotations)
            {
                CMT.SearchObjName(rootTransform, entry.Key).localRotation *= entry.Value;
            }
        }

        private void ExportModel(object sender, ModelExportEventArgs args)
        {
            SaveUserPreferences();

            try
            {
                var meshes = FindObjectsOfType<SkinnedMeshRenderer>()
                    .Where(smr => smr.name != "obj1")
                    .Distinct()
                    .ToList();

                switch (args.Format)
                {
                    case ModelExportFormat.Pmx:
                        {
                            var pmxBuilder = new PmxBuilder(args.Folder, args.Name)
                            {
                                SavePostion = args.SavePosition,
                                SaveTexture = args.SaveTexture
                            };
                            pmxBuilder.Export(meshes);
                        }
                        break;
                    case ModelExportFormat.Obj:
                        {
                            var objBuilder = new ObjBuilder(args.Folder, args.Name)
                            {
                                SavePostion = args.SavePosition,
                                SaveTexture = args.SaveTexture
                            };
                            objBuilder.Export(meshes);
                        }
                        break;
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