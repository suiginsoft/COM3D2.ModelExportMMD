using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace COM3D2.ModelExportMMD.Extensions
{
    // Extensions to the Maid class for exporting meshes and materials.
    public static class MaidExtensions
    {
        #region Types

        // Captures material layer state for a Maid allowing for this state to
        // be restored after exporting a model.
        public class MaterialLayerState
        {
            public TBody.TexLay.OrderTex Layer;
            public GameUty.SystemMaterial OriginalBlendMode;
        }

        #endregion

        #region Constants

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

        #region Methods

        // Stops all animations, locks eye & head position so that the Maid
        // is looking straight forward and applies transformations to a Maid's
        // armature to put her into a T-pose.
        public static void ApplyTPose(this Maid maid)
        {
            maid.body0.m_Bones.GetComponent<Animation>().Stop();
            maid.body0.Face.AnimationStop();
            maid.EyeToReset();
            maid.LockHeadAndEye(true);

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

        // This is a workaround for the face textures being black-transparent
        // on export. Most head mesh materials are composed of multiple layers,
        // with each layer using the Multiply blending mode. For whatever
        // reason these textures end up being black-transparent when the pixels
        // are read from the final composite RenderTexture object. Changing the
        // blending mode of the layers to Alpha appears to fix this problem.
        // This function returns a list of layer states that can be passed to
        // RestoreMaterialsAfterExport in order to restore the material layers
        // to their previous states.
        public static List<MaterialLayerState> PrepareMaterialsForExport(this Maid maid)
        {
            var layerStates = new List<MaterialLayerState>();

            var fi = typeof(TBody).GetField("m_dicLaySlot", BindingFlags.NonPublic | BindingFlags.Instance);

            if (fi != null)
            {
                var dicLaySlot = (Dictionary<string, TBody.TexLay.Mat>)fi.GetValue(maid.body0);
                foreach (var mat in dicLaySlot)
                {
                    foreach (var prop in mat.Value.dicPropInMat)
                    {
                        foreach (var lay in prop.Value.dicLayInProp)
                        {
                            if (lay.Value.listLayer != null)
                            {
                                foreach (var tex in lay.Value.listLayer)
                                {
                                    if (mat.Key == "head" && prop.Key == 5 && tex.mode == GameUty.SystemMaterial.Multiply)
                                    {
                                        layerStates.Add(new MaterialLayerState() { Layer = tex, OriginalBlendMode = tex.mode });
                                        //tex.mode = GameUty.SystemMaterial.Alpha;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            foreach (var goItem in maid.body0.goSlot)
            {
                goItem.TextureCache.Update();
                goItem.ManColorUpdate();
            }

            maid.body0.MulTexProc();

            return layerStates;
        }

        // Restores the original blend modes of modified composite materials.
        // Should be passed the resulting list from PrepareMaterialsForExport.
        public static void RestoreMaterialsAfterExport(this Maid maid, List<MaterialLayerState> layerStates)
        {
            return;
            foreach (var layerState in layerStates)
            {
                layerState.Layer.mode = layerState.OriginalBlendMode;
            }

            foreach (var goItem in maid.body0.goSlot)
            {
                goItem.TextureCache.Update();
                goItem.ManColorUpdate();
            }

            maid.body0.MulTexProc();
        }

        #endregion
    }
}
