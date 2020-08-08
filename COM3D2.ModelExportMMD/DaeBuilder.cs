using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;

namespace COM3D2.ModelExportMMD
{
    internal class DaeBuilder
    {
        private ColladaExporter ce;

        public DaeBuilder(string filename)
        {
            ce = new ColladaExporter(filename);
        }

        public void AddMesh(SkinnedMeshRenderer skinnedMesh)
        {
            string id = skinnedMesh.name + "_mesh";
            ce.AddGeometry(id, skinnedMesh.sharedMesh, null);
            ce.AddGeometryToScene(id, skinnedMesh.name, skinnedMesh.gameObject.transform.localToWorldMatrix);
        }

        public void Finish()
        {
            ce.Save();
            ce.Dispose();
        }
    }
}