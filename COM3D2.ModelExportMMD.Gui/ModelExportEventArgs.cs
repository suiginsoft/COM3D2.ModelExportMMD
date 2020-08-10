using System;

namespace COM3D2.ModelExportMMD.Gui
{
    public enum ModelExportFormat
    {
        Obj,
        Pmx
    }

    public class ModelExportEventArgs : EventArgs
    {
        #region Properties

        public ModelExportFormat Format { get; }

        public string Folder { get; }

        public string Name { get; }

        public bool SavePosition { get; } = true;

        public bool SaveTexture { get; } = true;

        #endregion

        #region Constructors

        public ModelExportEventArgs(ModelExportFormat format, string folder, string name, bool savePosition, bool saveTexture)
        {
            Format = format;
            Folder = folder;
            Name = name;
            SavePosition = savePosition;
            SaveTexture = saveTexture;
        }

        #endregion
    }
}
