COM3D2.ModelExportMMD
-------------------------------------------------------------------------------
A BepInEx plugin for COM3D2 1.48 and later that supports exporting your custom Maids as MikuMikuDance (MMD) or Wavefront (OBJ) models. This project was reverse engineered from the older CM3D2 ModelExportMMD plugin originally authored by 伊丽丝喵 and then heavily revised and reworked to fix a number of bugs and deficiences.

Things that have been changed or improved from the original plugin to tailor it for COM3D2:

- Updated user interface and introduction of a file browser dialog
- Packaged the window background image as an embedded resource instead of Base64 text, reducing the plugin file size by half
- Export preferences are now persisted across multiple sessions, saved to the plugin's configuration file
- Disabled the Maid's head and eye tracking when the T-pose transformation is applied
- Export the specular power to MMD PMX materials instead of leaving it fixed at a value of zero
- Utilize instance textures instead of shared textures when exporting, capturing instance-specific modifications
- Restore the prior active RenderTexture when transforming RenderTexture objects to Texture2D objects
- Workaround for blank face textures being exported due to the Multiply blending mode of certain composite layers being incompatible with Texture2D.ReadPixels
- More robust parent-child bone mapping of armature, fixing a crash when processing COM3D2 meshes
- Updated the embedded PmxLib to the latest version at 2.57 with support for PMX 2.1 file format features
- Improved logging and error recovery
- Updated the plugin to BepInEx, you need version 5.0.1 at least, check out "MaidEx AIO"

Installation
-------------------------------------------------------------------------------
Extract the archive in BepInEx/plugins folder.

Usage
-------------------------------------------------------------------------------
Once you're in game and have made it to the initial Maid customization screen, press the `F8` key on your keyboard to activate the Model Export MMD window.

From the export window you may select your export folder on the filesystem, the model file base name and your desired export format (either MMD A, B or OBJ). MMD A exports with English names for bones and morphs, MMD B exports with Japanese names. There are some more differences between them, but MMD B is from a random mod I found on hongfire, so I'm not sure exactly what else it does. Try one or the other, see which works for you. You can manually enter in the export folder file path and model base name using your keyboard or use the file browser dialog by pressing the `Browse` button. Before you export, ensure that you press the `Apply T-Pose` button to force your Maid into a fixed T-pose. Finally, click the `Export` button to export your Maid to the filesystem, or click `Close` to cancel the export process and close the window.

Hacking
-------------------------------------------------------------------------------
There are two PMX exporters - PMX A and PMX B. PMX A is a further enhanced version of suiginsoft's PMX exporter with better bone rotation on export and morphs (aka blend shapes). PMX B is copy-pasted from a decompiled mod I found on hongfire, I don't know its author. I'm not sure exactly what's going on in that code, but it's a bit spaghetti.
