using System;

namespace COM3D2.ModelExportMMD.Gui
{
    public class ModelExportEventArgs : EventArgs
    {
        public enum ExporterClass
        {
            PmxA,
            PmxB,
            Obj
        };

        #region Properties

        public string Folder { get; }

        public string Name { get; }

        public ExporterClass Exporter { get; }

        public bool SavePosition { get; } = true;

        public bool SaveTexture { get; } = true;

        #endregion

        #region Constructors

        public ModelExportEventArgs(string folder, string name, ExporterClass exporter, bool savePosition, bool saveTexture)
        {
            Folder = folder;
            Name = name;
            Exporter = exporter;
            SavePosition = savePosition;
            SaveTexture = saveTexture;
        }

        #endregion
    }
}
