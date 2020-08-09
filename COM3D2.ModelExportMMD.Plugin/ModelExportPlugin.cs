using COM3D2.ModelExportMMD.Gui;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        private static readonly string[] TPoseResetIdentityBones = new string[]
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
                window = new ModelExportWindow();
                window.ApplyTPoseClicked += this.ApplyTPose;
                window.ExportObjClicked += this.ExportObj;
                window.ExportPmxClicked += this.ExportPmx;
            }
            window.DrawWindow();
        }

        private void ApplyTPose(object sender, EventArgs args)
        {
            Debug.Assert(sender == this.window);

            Maid maid = GameMain.Instance.CharacterMgr.GetMaid(0);
            maid.body0.m_Bones.GetComponent<Animation>().Stop();

            Transform rootTransform = maid.body0.m_Bones.transform;

            foreach (var boneName in TPoseResetIdentityBones)
            {
                Transform transform = CMT.SearchObjName(rootTransform, boneName);
                transform.localRotation = Quaternion.identity;
            }

            foreach (var entry in TPoseBoneTransformRotations)
            {
                CMT.SearchObjName(rootTransform, entry.Key).localRotation *= entry.Value;
            }
        }

        private void ExportObj(object sender, EventArgs args)
        {
            Debug.Assert(sender == this.window);

            try
            {
                var filepath = GetExportFilePath(".obj");
                var skinnedMeshes = GetAllSkinnedMeshes();

                var objBuilder = new ObjBuilder();
                objBuilder.Export(skinnedMeshes, filepath);
            }
            catch (Exception error)
            {
                Debug.LogError($"Error exporting OBJ: {error.Message}\n\nStack trace:\n{error.StackTrace}");
            }
        }

        private void ExportPmx(object sender, EventArgs args)
        {
            Debug.Assert(sender == this.window);

            try
            {
                var filepath = GetExportFilePath(".pmx");
                var skinnedMeshes = GetAllSkinnedMeshes();

                var pmxBuilder = new PmxBuilder();
                pmxBuilder.PrepareData(skinnedMeshes);
                pmxBuilder.CreateBoneList();

                foreach (var skinnedMesh in skinnedMeshes)
                {
                    pmxBuilder.CreateMeshList(skinnedMesh);
                }

                pmxBuilder.CreatePmxHeader();
                pmxBuilder.Save(filepath);
            }
            catch (Exception error)
            {
                Debug.LogError($"Error exporting PMX: {error.Message}\n\nStack trace:\n{error.StackTrace}");
            }
        }

        private string GetExportFilePath(string extension)
        {
            return Path.Combine(ModelExportWindow.ExportFolder, ModelExportWindow.ExportName + extension);
        }

        private List<SkinnedMeshRenderer> GetAllSkinnedMeshes()
        {
            return FindObjectsOfType<SkinnedMeshRenderer>()
                .Where(smr => smr.name != "obj1")
                .Distinct()
                .ToList();
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