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


namespace Retro80Utilities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using NLog;
    using Retro80Utilities.Palette.IO;

    /// <summary>
    /// MainFormはRetro80減色ユーティリティのエントリポイントUIです。
    /// アプリ起動時にパレットJSONと対応するPNGを読み込み、
    /// パレット選択ダイアログの表示・結果取得までを担当します。
    /// </summary>
    public partial class MainForm : Form
    {
        /// <summary>
        /// NLogロガー。パレットの読み込み処理やエラーを記録します。
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 読み込まれたパレット一覧を保持します。
        /// PaletteColorsはカテゴリ、名前、色配列を持つ単位。
        /// </summary>
        protected List<PaletteColors> _palettes { get; private set; }

        /// <summary>
        /// 現在選択されているパレットを保持します。
        /// </summary>
        protected List<PaletteColors> _selectedPalettes { get; private set; }


        /// <summary>
        /// ステータスバーを保持します。
        /// </summary>
        private ToolStripStatusLabel statusLabel;

        /// <summary>
        /// フォームの初期化。フォームロードイベントと、パレット選択ボタンのクリックイベントを登録します。
        /// </summary>
        public MainForm()
        {
            InitializeComponent();

            // コントロール初期化
            {
                statusLabel = new ToolStripStatusLabel();

                statusStrip.Items.Add(statusLabel);
                this.Controls.Add(statusStrip);
                statusLabel.Text = "減色ボタンをおして減色できます";
            }

            this.Load += MainForm_Load;

            // パレット選択ボタンクリック → パレット選択フォームの表示
            this.btnPalleteChoice.Click += (s, e) =>
            {
                var palettes = _palettes;
                var palletesChoiceForm = new frmPalletesChoice();
                palletesChoiceForm.SetPaletteSource(palettes);

                if (palletesChoiceForm.ShowDialog() == DialogResult.OK)
                {
                    _selectedPalettes = palletesChoiceForm.SelectedItems.Select(x => new PaletteColors()
                    {
                        category = x.Category,
                        name = x.Name,
                        Colors = x.Colors,
                    } ).ToList();

                    // 選択されたパレット名と色一覧をコンソール出力
                    foreach (var item in _selectedPalettes)
                    {
                        Console.WriteLine($"■ {item.name}({item.category})");
                        foreach (var color in item.Colors)
                        {
                            Console.WriteLine($" - {color}");
                        }
                    }

                    statusLabel.Text = $"{_selectedPalettes.Count}個のパレット - {String.Join(",", _selectedPalettes.Select(x => x.name).ToArray())}を選択しました。";
                }
            };
        }

        /// <summary>
        /// フォームロード時に、指定パスからJSONファイルとPNGを読み込みます。
        /// 成功時はMessageBoxで個数を表示し、失敗時はログとエラーダイアログを出力します。
        /// </summary>
        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                string jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources/Palettes/PalleteFilesDefinition.json");
                Logger.Info($"Attempting to load palette from: {jsonPath}");

                _palettes = PalleteLoader.Load(jsonPath);

                Logger.Info($"Loaded {_palettes.Count} palette items.");
                statusLabel.Text =$"{_palettes.Count}個のパレットを読み込みました。";
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "パレット読み込み中にエラーが発生しました。");
                MessageBox.Show("パレットの読み込みに失敗しました。\n" + ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
