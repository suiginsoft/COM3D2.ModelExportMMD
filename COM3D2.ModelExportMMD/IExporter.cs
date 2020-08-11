using System.Collections.Generic;
using UnityEngine;

namespace COM3D2.ModelExportMMD
{
    public interface IExporter
    {
        #region Properties

        string ExportFolder { get; set; }

        string ExportName { get; set; }

        bool SavePostion { get; set; }

        bool SaveTexture { get; set; }

        #endregion

        #region Methods

        void Export(List<SkinnedMeshRenderer> skinnedMeshes);

        #endregion
    }
}
