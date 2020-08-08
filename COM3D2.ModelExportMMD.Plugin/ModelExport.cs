using COM3D2.ModelExportMMD.Gui;
using System;
using System.Collections.Generic;
using System.IO;
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
    public class ModelExport : PluginBase
    {
        #region Fields

        private ExportWindow window;
        private List<SkinnedMeshRenderer> skinnedMeshList;

        #endregion

        #region Methods

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.F8))
            {
                LoadModelMesh();
                window.Show();
            }
            if (Input.GetKeyDown(KeyCode.F9))
            {
                DumpGameObjects();
            }
        }

        public void OnGUI()
        {
            if (window == null)
            {
                window = new ExportWindow();
                window.PMXExportCallBack = (Action)Delegate.Combine(window.PMXExportCallBack, new Action(ExportPMX));
                window.OBJExportCallBack = (Action)Delegate.Combine(window.OBJExportCallBack, new Action(ExportOBJ));
            }
            window.DrawWindow();
        }

        private void PrintComponentsOf(Component root, string indent, HashSet<Component> printed)
        {
            string subindent = indent + indent[0];
            Debug.Log(indent + root.name + "(" + root.GetType().Name + ")");
            if (printed.Add(root))
            {
                List<Component> components = new List<Component>();
                root.GetComponents(components);
                foreach (Component c in components)
                {
                    PrintComponentsOf(c, subindent, printed);
                }
            }
            else
            {
                Debug.Log(subindent + "[already printed]");
            }
        }

        private void DumpComponentHierarchyOfModels()
        {
            SkinnedMeshRenderer[] array = UnityEngine.Object.FindObjectsOfType(typeof(SkinnedMeshRenderer)) as SkinnedMeshRenderer[];
            GameObject go = null;
            for (int i = 0; i < array.Length; i++)
            {
                go = array[i].gameObject;
                if (array[i].name.Contains("body"))
                {
                    break;
                }
            }
            Debug.Log("Component tree of " + go.name);
            List<Component> components = new List<Component>();
            go.GetComponents(components);
            string indent = ".";
            HashSet<Component> printed = new HashSet<Component>();
            foreach (Component c in components)
            {
                PrintComponentsOf(c, indent, printed);
            }
        }

        private void PrintComponentsToOf(StreamWriter w, Component root, string indent, HashSet<Component> printed)
        {
            string subindent = indent + indent[0];
            w.WriteLine(indent + root.name + "(" + root.GetType().Name + ")");
            if (printed.Add(root))
            {
                List<Component> components = new List<Component>();
                root.GetComponents(components);
                foreach (Component c in components)
                {
                    PrintComponentsToOf(w, c, subindent, printed);
                }
            }
            else
            {
                w.WriteLine(subindent + "[already printed]");
            }
        }

        private void DumpGameObjects()
        {
            GameObject[] _gos = UnityEngine.Object.FindObjectsOfType<GameObject>();
            List<GameObject> gos = new List<GameObject>(new HashSet<GameObject>(_gos));
            gos.Sort(delegate (GameObject go1, GameObject go2)
            {
                return go1.name.CompareTo(go2.name);
            });
            StreamWriter w = new StreamWriter(@"D:\Temp\gos.txt");
            HashSet<Component> printed = new HashSet<Component>();
            foreach (GameObject go in gos)
            {
                w.WriteLine(go.name + "[" + go.GetInstanceID() + "]");
                foreach (Component c in go.GetComponents<Component>())
                {
                    PrintComponentsToOf(w, c, ".", printed);
                }
            }
            w.Close();
        }

        private void LoadModelMesh()
        {
            this.skinnedMeshList = new List<SkinnedMeshRenderer>();
            SkinnedMeshRenderer[] array = UnityEngine.Object.FindObjectsOfType(typeof(SkinnedMeshRenderer)) as SkinnedMeshRenderer[];
            foreach (SkinnedMeshRenderer skinnedMeshRenderer in array)
            {
                if (skinnedMeshRenderer.name != "obj1")
                {
                    Debug.Log("Found mesh " + skinnedMeshRenderer.name);
                    this.skinnedMeshList.Add(skinnedMeshRenderer);
                }
            }
        }

        private void ExportPMX()
        {
            string filename = ExportWindow.ExportFolder + "\\" + ExportWindow.ExportName + ".pmx";
            PmxBuilder pmxBuilder = new PmxBuilder();
            pmxBuilder.CreateModelInfo();
            pmxBuilder.PrepareData(skinnedMeshList);
            pmxBuilder.CreateBoneList();
            foreach (SkinnedMeshRenderer skinnedMesh in skinnedMeshList)
            {
                pmxBuilder.CreateMeshList(skinnedMesh);
            }
            pmxBuilder.CreatePmxHeader();
            pmxBuilder.Save(filename);
        }

        private void ExportOBJ()
        {
            ObjBuilder objBuilder = new ObjBuilder();
            string filename = ExportWindow.ExportFolder + "\\" + ExportWindow.ExportName + ".obj";
            objBuilder.Export(this.skinnedMeshList, filename);
        }

        #endregion
    }
}