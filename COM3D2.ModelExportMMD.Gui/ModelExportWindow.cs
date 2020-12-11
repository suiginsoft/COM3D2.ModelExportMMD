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

        private static readonly string[] ExportClassLabels =
        {
            "MMD A (*.pmx)",
            "MMD B (*.pmx)",
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
        public ModelExportEventArgs.ExporterClass ExportClass { get; set; }
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
            modalRect = new Rect(Screen.width / 2 - FixPx(300), Screen.height / 2 - FixPx(300), FixPx(450), FixPx(450));

            var backgroundTexture = new Texture2D((int)modalRect.width, (int)modalRect.height, TextureFormat.RGBA32, false);

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

            buttonStyle.fontSize = FixPx(FontSizePx);
            buttonStyle.normal.textColor = white;
            labelStyle.fontSize = FixPx(FontSizePx);
            labelStyle.normal.textColor = pink;
            selectionGridStyle.fontSize = FixPx(FontSizePx);
            selectionGridStyle.normal.textColor = white;
            selectionGridStyle.active.textColor = pink;
            selectionGridStyle.focused.textColor = pink;
            selectionGridStyle.hover.textColor = pink;
            selectionGridStyle.onNormal.textColor = white;
            selectionGridStyle.onActive.textColor = pink;
            selectionGridStyle.onHover.textColor = pink;
            selectionGridStyle.onFocused.textColor = pink;
            textStyle.fontSize = FixPx(FontSizePx);
            textStyle.normal.textColor = white;
            toggleStyle.fontSize = FixPx(FontSizePx);
            toggleStyle.normal.textColor = pink;
            toggleStyle.onNormal.textColor = pink;
            toggleStyle.active.textColor = pink;
            toggleStyle.hover.textColor = pink;
            toggleStyle.onHover.textColor = pink;
            toggleStyle.onActive.textColor = pink;
            toggleStyle.focused.textColor = pink;
            toggleStyle.onFocused.textColor = pink;
            windowStyle.normal.textColor = pink;
            windowStyle.normal.background = backgroundTexture;
        }

        #endregion

        #region Methods

        private static int FixPx(int px)
        {
            return (int)((1f + (Screen.width / 1280f - 1f) * 0.6f) * px);
        }

        public void Show()
        {
            modalRect = new Rect(Screen.width / 2 - FixPx(300), Screen.height / 2 - FixPx(300), FixPx(450), FixPx(450));
            showSaveDialog = true;
        }

        public void DrawWindow()
        {
            if (showSaveDialog)
            {
                var windowTitle = $"Model Export MMD Version {PluginVersion}";
                modalRect = GUI.ModalWindow(0, modalRect, DoSaveModDialog, windowTitle, windowStyle);
            }
        }

        private void DoSaveModDialog(int winId)
        {
            labelStyle.fontSize = FixPx(FontSizePx);

            float margin = FixPx(MarginPx);
            float itemHeight = FixPx(ItemHeightPx);

            Rect position = new Rect(0f, 0f, modalRect.width - margin * 3f, itemHeight);
            position.x = margin;
            position.y = itemHeight + margin;
            position.width = modalRect.width * 0.2f - margin;
            GUI.Label(position, Labels[0], labelStyle);

            position.x += position.width;
            position.width = modalRect.width * 0.6f - margin;
            ExportFolderPath = GUI.TextField(position, ExportFolderPath, textStyle);

            position.x += position.width;
            position.width = modalRect.width * 0.2f;
            if (GUI.Button(position, Labels[1], buttonStyle))
            {
                BrowseClicked(this, EventArgs.Empty);
            }

            position.x = margin;
            position.y += position.height + margin;
            position.width = modalRect.width * 0.2f - margin;
            GUI.Label(position, Labels[2], labelStyle);

            position.x += position.width;
            position.width = modalRect.width * 0.8f - margin;
            ExportName = GUI.TextField(position, ExportName, textStyle);

            position.x = margin;
            position.y += position.height + margin;
            position.width = modalRect.width * 0.2f - margin;
            GUI.Label(position, Labels[3], labelStyle);

            position.x += position.width;
            position.width = modalRect.width * 0.8f - margin;
            ExportClass = (ModelExportEventArgs.ExporterClass)GUI.SelectionGrid(position, (int)ExportClass, ExportClassLabels, ExportClassLabels.Length, selectionGridStyle);

            position.x = margin;
            position.y += position.height + margin;
            SaveTextures = GUI.Toggle(position, SaveTextures, Labels[4], toggleStyle);

            position.x = margin;
            position.y += position.height + margin;
            SavePostion = GUI.Toggle(position, SavePostion, Labels[5], toggleStyle);

            position.x = margin;
            position.y += position.height + margin;
            position.width = modalRect.width - margin * 2f;
            if (GUI.Button(position, Labels[6], buttonStyle))
            {
                ApplyTPoseClicked(this, EventArgs.Empty);
            }

            position.x = margin;
            position.y += position.height + margin;
            position.width = modalRect.width - margin * 2f;
            if (GUI.Button(position, Labels[7], buttonStyle))
            {
                var args = new ModelExportEventArgs(
                    ExportFolderPath,
                    ExportName,
                    ExportClass,
                    SavePostion,
                    SaveTextures);
                ExportClicked(this, args);
                showSaveDialog = false;
            }

            position.x = margin;
            position.y += position.height + margin;
            if (GUI.Button(position, Labels[8], buttonStyle))
            {
                CloseClicked(this, EventArgs.Empty);
                showSaveDialog = false;
            }

            GUI.DragWindow(new Rect(0f, 0f, 10000f, 100f));
        }

        #endregion
    }
}