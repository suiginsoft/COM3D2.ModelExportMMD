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
            "File",
            "Save texture",
            "Save position",
            "Apply T-Pose",
            "Save as MMD",
            "Save as OBJ",
            "Close"
        };

        #endregion

        #region Fields

        private readonly GUIStyle lStyle = "label";
        private readonly GUIStyle bStyle = "button";
        private readonly GUIStyle tStyle = "toggle";
        private readonly GUIStyle textStyle = "textField";
        private readonly GUIStyle windowStyle = "window";
        private bool showSaveDialog = false;
        private Rect modalRect;

        #endregion

        #region Properties

        public string PluginVersion { get; set; }
        public string ExportFolderPath { get; set; }
        public string ExportName { get; set; }
        public bool SavePostion { get; set; }
        public bool SaveTextures { get; set; }

        #endregion

        #region Events

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

            var textColor = new Color(1f, 0.329411775f, 0.478431374f, 1f);
            var textColor2 = new Color(1f, 1f, 1f, 1f);

            this.windowStyle.normal.textColor = textColor;
            this.windowStyle.normal.background = backgroundTexture;
            this.lStyle.fontSize = FixPx(FontSizePx);
            this.lStyle.normal.textColor = textColor;
            this.bStyle.fontSize = FixPx(FontSizePx);
            this.bStyle.normal.textColor = textColor2;
            this.tStyle.fontSize = FixPx(FontSizePx);
            this.tStyle.normal.textColor = textColor;
            this.tStyle.onNormal.textColor = textColor;
            this.tStyle.active.textColor = textColor;
            this.tStyle.hover.textColor = textColor;
            this.tStyle.onHover.textColor = textColor;
            this.tStyle.onActive.textColor = textColor;
            this.tStyle.focused.textColor = textColor;
            this.tStyle.onFocused.textColor = textColor;
            this.textStyle.fontSize = FixPx(FontSizePx);
            this.textStyle.normal.textColor = textColor2;
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
                var windowTitle = $"Model Export to MMD Version {this.PluginVersion}";
                this.modalRect = GUI.ModalWindow(0, this.modalRect, this.DoSaveModDialog, windowTitle, this.windowStyle);
            }
        }

        private void DoSaveModDialog(int winId)
        {
            this.lStyle.fontSize = FixPx(FontSizePx);

            float margin = FixPx(MarginPx);
            float itemHeight = FixPx(ItemHeightPx);

            Rect position = new Rect(0f, 0f, this.modalRect.width - margin * 2f, itemHeight);
            position.x = margin;
            position.y = itemHeight + margin;
            position.width = this.modalRect.width * 0.2f - margin;
            GUI.Label(position, Labels[0], this.lStyle);

            position.x += position.width;
            position.width = this.modalRect.width * 0.8f - margin;
            this.ExportFolderPath = GUI.TextField(position, this.ExportFolderPath, this.textStyle);

            position.x = margin;
            position.y += position.height + margin;
            position.width = this.modalRect.width * 0.2f - margin;
            GUI.Label(position, Labels[1], this.lStyle);

            position.x += position.width;
            position.width = this.modalRect.width * 0.8f - margin;
            this.ExportName = GUI.TextField(position, this.ExportName, this.textStyle);

            position.x = margin;
            position.y += position.height + margin;
            this.SaveTextures = GUI.Toggle(position, this.SaveTextures, Labels[2], this.tStyle);

            position.x = margin;
            position.y += position.height + margin;
            this.SavePostion = GUI.Toggle(position, this.SavePostion, Labels[3], this.tStyle);

            position.x = margin;
            position.y += position.height + margin;
            position.width = this.modalRect.width - margin * 2f;
            if (GUI.Button(position, Labels[4], this.bStyle))
            {
                this.ApplyTPoseClicked(this, EventArgs.Empty);
            }

            position.x = margin;
            position.y += position.height + margin;
            position.width = this.modalRect.width - margin * 2f;
            if (GUI.Button(position, Labels[5], this.bStyle))
            {
                var args = new ModelExportEventArgs(
                    ModelExportFormat.Pmx,
                    this.ExportFolderPath,
                    this.ExportName,
                    this.SavePostion,
                    this.SaveTextures);
                this.ExportClicked(this, args);
                this.showSaveDialog = false;
            }

            position.x = margin;
            position.y += position.height + margin;
            position.width = this.modalRect.width - margin * 2f;
            if (GUI.Button(position, Labels[6], this.bStyle))
            {
                var args = new ModelExportEventArgs(
                    ModelExportFormat.Obj,
                    this.ExportFolderPath,
                    this.ExportName,
                    this.SavePostion,
                    this.SaveTextures);
                this.ExportClicked(this, args);
                this.showSaveDialog = false;
            }

            position.x = margin;
            position.y += position.height + margin;
            if (GUI.Button(position, Labels[7], this.bStyle))
            {
                this.CloseClicked(this, EventArgs.Empty);
                this.showSaveDialog = false;
            }

            GUI.DragWindow(new Rect(0f, 0f, 10000f, 100f));
        }

        #endregion
    }
}