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
using System.Text;
using Newtonsoft.Json;

namespace Retro80Utilities.Palette.IO
{
    internal static class PalletJSonReader
    {
        /// <summary>
        /// JSONファイルパスからパレット情報を読み込む
        /// </summary>
        public static List<PaletteJsonDefinition> Read(string jsonFilePath)
        {
            if (string.IsNullOrEmpty(jsonFilePath))
                throw new ArgumentNullException(nameof(jsonFilePath));

            if (!File.Exists(jsonFilePath))
                throw new FileNotFoundException("JSONファイルが見つかりません。", jsonFilePath);

            string json = File.ReadAllText(jsonFilePath, Encoding.UTF8);
            return ReadJsonString(json);
        }

        /// <summary>
        /// JSON文字列からパレット情報を読み込む
        /// </summary>
        public static List<PaletteJsonDefinition> ReadJsonString(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                throw new ArgumentNullException(nameof(json));

            return JsonConvert.DeserializeObject<List<PaletteJsonDefinition>>(json);
        }
    }
}
