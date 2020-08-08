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
        #region Fields

        private ModelExportWindow window;

        #endregion

        #region Methods

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.F8))
            {
                window.Show();
            }
            if (Input.GetKeyDown(KeyCode.F9))
            {
                DumpGameObjects();
            }
            if (Input.GetKeyDown(KeyCode.F10))
            {
                DumpBodyModel();
            }
        }

        public void OnGUI()
        {
            if (window == null)
            {
                window = new ModelExportWindow();
                window.ExportObjCallBack = (Action)Delegate.Combine(window.ExportObjCallBack, new Action(ExportObj));
                window.ExportPmxCallBack = (Action)Delegate.Combine(window.ExportPmxCallBack, new Action(ExportPmx));
            }
            window.DrawWindow();
        }

        private void ExportObj()
        {
            string filepath = Path.Combine(ModelExportWindow.ExportFolder, ModelExportWindow.ExportName + ".obj");
            var skinnedMeshes = FetchAllSkinnedMeshes();

            ObjBuilder objBuilder = new ObjBuilder();
            objBuilder.Export(skinnedMeshes, filepath);
        }

        private void ExportPmx()
        {
            string filepath = Path.Combine(ModelExportWindow.ExportFolder, ModelExportWindow.ExportName + ".pmx");
            var skinnedMeshes = FetchAllSkinnedMeshes();

            PmxBuilder pmxBuilder = new PmxBuilder();
            pmxBuilder.CreateModelInfo();
            pmxBuilder.PrepareData(skinnedMeshes);
            pmxBuilder.CreateBoneList();

            foreach (var skinnedMesh in skinnedMeshes)
            {
                pmxBuilder.CreateMeshList(skinnedMesh);
            }

            pmxBuilder.CreatePmxHeader();
            pmxBuilder.Save(filepath);
        }

        private List<SkinnedMeshRenderer> FetchAllSkinnedMeshes()
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

            using (StreamWriter w = new StreamWriter(@"GameObjects.txt"))
            {
                if (gameObjects.Count == 0)
                {
                    w.WriteLine("No game objects found");
                    return;
                }

                var printed = new HashSet<Component>();

                foreach (var go in gameObjects)
                {
                    w.WriteLine(go.name + "[" + go.GetInstanceID() + "]");
                    foreach (var c in go.GetComponents<Component>())
                    {
                        DumpComponents(w, c, ".", printed);
                    }
                }
            }
        }

        private void DumpBodyModel()
        {
            var gameObject = FindObjectsOfType<SkinnedMeshRenderer>()
                .Select(smr => smr.gameObject)
                .Where(go => go.name.Contains("body"))
                .FirstOrDefault();

            using (StreamWriter w = new StreamWriter(@"BodyModel.txt"))
            {
                if (gameObject == null)
                {
                    w.WriteLine("No body model found");
                    return;
                }

                w.WriteLine($"Component tree of {gameObject.name}");

                var printed = new HashSet<Component>();

                var components = new List<Component>();
                gameObject.GetComponents(components);

                foreach (var c in components)
                {
                    DumpComponents(w, c, ".", printed);
                }
            }
        }

        private void DumpComponents(StreamWriter w, Component root, string indent, HashSet<Component> printed)
        {
            w.WriteLine($"{indent} {root.name}({root.GetType().Name})");

            var subindent = indent + indent[0];

            if (printed.Add(root))
            {
                var components = new List<Component>();
                root.GetComponents(components);

                foreach (var c in components)
                {
                    DumpComponents(w, c, subindent, printed);
                }
            }
            else
            {
                w.WriteLine($"{subindent} [already printed]");
            }
        }

        #endregion
    }
}