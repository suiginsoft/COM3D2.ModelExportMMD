using System;

namespace COM3D2.ModelExportMMD.Gui
{
    public class ModelExportEventArgs : EventArgs
    {
        #region Properties

        public string Folder { get; }

        public string Name { get; }

        public ModelExportFormat Format { get; }

        public bool SavePosition { get; } = true;

        public bool SaveTexture { get; } = true;

        #endregion

        #region Constructors

        public ModelExportEventArgs(string folder, string name, ModelExportFormat format, bool savePosition, bool saveTexture)
        {
            Folder = folder;
            Name = name;
            Format = format;
            SavePosition = savePosition;
            SaveTexture = saveTexture;
        }

        #endregion
    }
}
