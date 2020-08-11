using System;
using System.Reflection;
using UnityEngine;

namespace COM3D2.ModelExportMMD.Gui
{
    public class ModelExportWindow
    {
        #region Constants

        private const int MarginPx = 18;
        private const int FontSizePx = 16;
        private const int ItemHeightPx = 24;
        private const string WindowBackgroundResource = "COM3D2.ModelExportMMD.Resources.WindowBackground.png";

        private static readonly string[] Labels =
        {
            "Folder",
            "Browse",
            "Name",
            "Format",
            "Save texture",
            "Save position",
            "Apply T-Pose",
            "Export",
            "Close"
        };

        private static readonly string[] ExportFormatLabels =
        {
            "MMD (*.pmx)",
            "OBJ (*.obj)"
        };

        #endregion

        #region Fields

        private readonly GUIStyle buttonStyle = new GUIStyle("button");
        private readonly GUIStyle labelStyle = new GUIStyle("label");
        private readonly GUIStyle selectionGridStyle = new GUIStyle("button");
        private readonly GUIStyle textStyle = new GUIStyle("textfield");
        private readonly GUIStyle toggleStyle = new GUIStyle("toggle");
        private readonly GUIStyle windowStyle = new GUIStyle("window");
        private bool showSaveDialog = false;
        private Rect modalRect;

        #endregion

        #region Properties

        public string PluginVersion { get; set; }
        public string ExportFolderPath { get; set; }
        public string ExportName { get; set; }
        public ModelFormat ExportFormat { get; set; }
        public bool SavePostion { get; set; }
        public bool SaveTextures { get; set; }

        #endregion

        #region Events

        public event EventHandler<EventArgs> BrowseClicked;
        public event EventHandler<EventArgs> ApplyTPoseClicked;
        public event EventHandler<ModelExportEventArgs> ExportClicked;
        public event EventHandler<EventArgs> CloseClicked;

        #endregion

        #region Constructors

        public ModelExportWindow()
        {
            this.modalRect = new Rect(Screen.width / 2 - FixPx(300), Screen.height / 2 - FixPx(300), FixPx(450), FixPx(450));

            var backgroundTexture = new Texture2D((int)this.modalRect.width, (int)this.modalRect.height, TextureFormat.RGBA32, false);

            using (var backgroundStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(WindowBackgroundResource))
            {
                byte[] imageData = new byte[backgroundStream.Length];
                backgroundStream.Read(imageData, 0, imageData.Length);
                backgroundTexture.LoadImage(imageData);
                backgroundTexture.Apply();
            }

            var white = new Color(1f, 1f, 1f, 1f);
            var grey = new Color(0.65f, 0.65f, 0.65f, 1f);
            var pink = new Color(1f, 0.329411775f, 0.478431374f, 1f);

            this.buttonStyle.fontSize = FixPx(FontSizePx);
            this.buttonStyle.normal.textColor = white;
            this.labelStyle.fontSize = FixPx(FontSizePx);
            this.labelStyle.normal.textColor = pink;
            this.selectionGridStyle.fontSize = FixPx(FontSizePx);
            this.selectionGridStyle.normal.textColor = white;
            this.selectionGridStyle.active.textColor = pink;
            this.selectionGridStyle.focused.textColor = pink;
            this.selectionGridStyle.hover.textColor = pink;
            this.selectionGridStyle.onNormal.textColor = white;
            this.selectionGridStyle.onActive.textColor = pink;
            this.selectionGridStyle.onHover.textColor = pink;
            this.selectionGridStyle.onFocused.textColor = pink;
            this.textStyle.fontSize = FixPx(FontSizePx);
            this.textStyle.normal.textColor = white;
            this.toggleStyle.fontSize = FixPx(FontSizePx);
            this.toggleStyle.normal.textColor = pink;
            this.toggleStyle.onNormal.textColor = pink;
            this.toggleStyle.active.textColor = pink;
            this.toggleStyle.hover.textColor = pink;
            this.toggleStyle.onHover.textColor = pink;
            this.toggleStyle.onActive.textColor = pink;
            this.toggleStyle.focused.textColor = pink;
            this.toggleStyle.onFocused.textColor = pink;
            this.windowStyle.normal.textColor = pink;
            this.windowStyle.normal.background = backgroundTexture;
        }

        #endregion

        #region Methods

        private static int FixPx(int px)
        {
            return (int)((1f + (Screen.width / 1280f - 1f) * 0.6f) * px);
        }

        public void Show()
        {
            this.modalRect = new Rect(Screen.width / 2 - FixPx(300), Screen.height / 2 - FixPx(300), FixPx(450), FixPx(450));
            this.showSaveDialog = true;
        }

        public void DrawWindow()
        {
            if (this.showSaveDialog)
            {
                var windowTitle = $"Model Export MMD Version {this.PluginVersion}";
                this.modalRect = GUI.ModalWindow(0, this.modalRect, this.DoSaveModDialog, windowTitle, this.windowStyle);
            }
        }

        private void DoSaveModDialog(int winId)
        {
            this.labelStyle.fontSize = FixPx(FontSizePx);

            float margin = FixPx(MarginPx);
            float itemHeight = FixPx(ItemHeightPx);

            Rect position = new Rect(0f, 0f, this.modalRect.width - margin * 3f, itemHeight);
            position.x = margin;
            position.y = itemHeight + margin;
            position.width = this.modalRect.width * 0.2f - margin;
            GUI.Label(position, Labels[0], this.labelStyle);

            position.x += position.width;
            position.width = this.modalRect.width * 0.6f - margin;
            this.ExportFolderPath = GUI.TextField(position, this.ExportFolderPath, this.textStyle);

            position.x += position.width;
            position.width = this.modalRect.width * 0.2f;
            if (GUI.Button(position, Labels[1], this.buttonStyle))
            {
                this.BrowseClicked(this, EventArgs.Empty);
            }

            position.x = margin;
            position.y += position.height + margin;
            position.width = this.modalRect.width * 0.2f - margin;
            GUI.Label(position, Labels[2], this.labelStyle);

            position.x += position.width;
            position.width = this.modalRect.width * 0.8f - margin;
            this.ExportName = GUI.TextField(position, this.ExportName, this.textStyle);

            position.x = margin;
            position.y += position.height + margin;
            position.width = this.modalRect.width * 0.2f - margin;
            GUI.Label(position, Labels[3], this.labelStyle);

            position.x += position.width;
            position.width = this.modalRect.width * 0.8f - margin;
            this.ExportFormat = (ModelFormat)GUI.SelectionGrid(position, (int)this.ExportFormat, ExportFormatLabels, ExportFormatLabels.Length, this.selectionGridStyle);

            position.x = margin;
            position.y += position.height + margin;
            this.SaveTextures = GUI.Toggle(position, this.SaveTextures, Labels[4], this.toggleStyle);

            position.x = margin;
            position.y += position.height + margin;
            this.SavePostion = GUI.Toggle(position, this.SavePostion, Labels[5], this.toggleStyle);

            position.x = margin;
            position.y += position.height + margin;
            position.width = this.modalRect.width - margin * 2f;
            if (GUI.Button(position, Labels[6], this.buttonStyle))
            {
                this.ApplyTPoseClicked(this, EventArgs.Empty);
            }

            position.x = margin;
            position.y += position.height + margin;
            position.width = this.modalRect.width - margin * 2f;
            if (GUI.Button(position, Labels[7], this.buttonStyle))
            {
                var args = new ModelExportEventArgs(
                    this.ExportFolderPath,
                    this.ExportName,
                    this.ExportFormat,
                    this.SavePostion,
                    this.SaveTextures);
                this.ExportClicked(this, args);
                this.showSaveDialog = false;
            }

            position.x = margin;
            position.y += position.height + margin;
            if (GUI.Button(position, Labels[8], this.buttonStyle))
            {
                this.CloseClicked(this, EventArgs.Empty);
                this.showSaveDialog = false;
            }

            GUI.DragWindow(new Rect(0f, 0f, 10000f, 100f));
        }

        #endregion
    }
}