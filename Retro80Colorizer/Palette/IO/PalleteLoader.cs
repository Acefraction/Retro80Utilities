// @language-version: 7.3
// @framework: .NET Framework 4.8
// @target: Windows Forms
// @dependencies: Newtonsoft.Json, NLog
// @color-order: RGB
// @bitwise-color: RGB-aligned only
// @constraints:
// - Do NOT use C# 8.0+ features (e.g., records, switch expressions, pattern matching).
// - All color operations must use RGB order.
// - Bitwise color formats must preserve RGB structure unless explicitly required.
// - Avoid BRG, BGR, or platform-specific layouts unless documented.
// - If the processing logic is sufficiently complex, use NLog for debug logging and traceability.
// - All classes, methods, and important variables should include AI-friendly XML comments
//   describing their role, input/output behavior, and structural context.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using NLog;
using Retro80Colorizer.Palette.IO;

namespace Retro80Utilities.Palette.IO
{
    /// <summary>
    /// PalleteLoader is responsible for loading palette definitions from a JSON file,
    /// resolving referenced PNG image files, and converting them into a list of PaletteColors entries.
    /// PNG image files are parsed using PaletteFileDefinition, and colors are matched row-by-row
    /// with the labels defined in the JSON.
    /// </summary>
    internal static class PalleteLoader
    {
        /// <summary>
        /// NLog logger used for tracking loading progress, warnings, and errors.
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Loads a list of palette color definitions from a JSON configuration file.
        /// For each entry, it loads the corresponding PNG file and extracts row-wise color data.
        /// Each color row is matched to a label defined in the JSON.
        /// </summary>
        /// <param name="JSONFilePath">Path to the JSON file describing palettes and associated image files</param>
        /// <returns>List of PaletteColors, each containing a name, category, and associated color list</returns>
        /// <exception cref="FileNotFoundException">Thrown if JSON or referenced PNG files are missing</exception>
        /// <exception cref="InvalidDataException">Thrown if a color row cannot be resolved</exception>
        public static List<PaletteColors> Load(string JSONFilePath)
        {
            Logger.Info($"Loading palette definitions from JSON: {JSONFilePath}");

            string jsonText = File.ReadAllText(JSONFilePath);
            var jsonDefs = PalletJSonReader.ReadJsonString(jsonText);
            Logger.Debug($"Parsed {jsonDefs.Count} palette JSON entries.");

            var fileDefs = new List<PaletteFileDefinition>();

            foreach (var def in jsonDefs)
            {
                string path = Path.Combine(Path.GetDirectoryName(JSONFilePath), def.filename);
                Logger.Debug($"Attempting to load PNG file: {path}");

                try
                {
                    var file = new PaletteFileDefinition(path);
                    fileDefs.Add(file);
                    Logger.Info($"Loaded palette file: {path}");
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, $"Failed to load PNG file: {path}");
                    MessageBox.Show($"PNGファイルの読み込みに失敗しました: {path}", "ファイル読み込みエラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    throw;
                }
            }

            var result = new List<PaletteColors>();

            foreach (var def in jsonDefs)
            {
                var file = fileDefs.FirstOrDefault(f => Path.GetFileName(f.Filename) == def.filename);
                if (file == null)
                {
                    string msg = $"PNGファイルが見つかりません: {def.filename}";
                    Logger.Error(msg);
                    MessageBox.Show(msg, "読み込みエラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    throw new FileNotFoundException(msg, def.filename);
                }

                for (int i = 0; i < def.names.Count; i++)
                {
                    string label = def.names[i];
                    try
                    {
                        var colors = file.GetRow(i);

                        result.Add(new PaletteColors
                        {
                            category = def.category,
                            name = label,
                            Colors = colors
                        });

                        Logger.Debug($"Added palette: {label} ({def.category}) [row {i}]");
                    }
                    catch (IndexOutOfRangeException)
                    {
                        string errorMsg = $"色名 '{label}' に対応するパレット行が見つかりません。\nファイル: {def.filename}";
                        Logger.Error(errorMsg);
                        MessageBox.Show(errorMsg, "パレット行不足エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        throw;
                    }
                }
            }

            Logger.Info($"Successfully loaded {result.Count} palette items.");
            return result;
        }
    }
}
