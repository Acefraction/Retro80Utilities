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
    using Emgu.CV;
    using Emgu.CV.Structure;
    using Emgu.CV.XImgproc;
    using Emgu.CV.CvEnum;
    using Retro80Colorizer.ColorReducer.SimpleLChDistanceReducer;

    using static Emgu.CV.BitmapExtension;

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

            this.btnExtractLine.Click += (s, e) =>
            {
                ExtractLine();
            };


            this.btnTestLChDistanceColorRedude.Click += (s, e) =>
            {
                OpenFileDialog dialog = new OpenFileDialog
                {
                    Filter = "画像ファイル (*.png)|*.png",
                    Title = "画像を選択してください"
                };

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string inputPath = dialog.FileName;
                    string baseDir = Path.GetDirectoryName(inputPath);
                    string baseName = Path.GetFileNameWithoutExtension(inputPath);

                    try
                    {
                        using (Bitmap original = new Bitmap(inputPath))
                        {
                            if (chkDither.Checked)
                            {
                                // リサイズ
                                int reducedWidth = original.Width / 2;
                                int reducedHeight = original.Height / 2;
                                Bitmap resized = new Bitmap(reducedWidth, reducedHeight);

                                using (Graphics g = Graphics.FromImage(resized))
                                {
                                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;
                                    g.DrawImage(original, 0, 0, reducedWidth, reducedHeight);
                                }

                                if (resized.Width == 0 || resized.Height == 0)
                                {
                                    MessageBox.Show("Resized image has zero dimension.");
                                    return;
                                }

                                // ディノイズ
                                Image<Bgr, byte> image = resized.ToImage<Bgr, byte>();
                                Image<Bgr, byte> result = new Image<Bgr, byte>(image.Size);
                                CvInvoke.FastNlMeansDenoisingColored(image, result, 2, 2, 3, 5);
                                Bitmap bmpDenoised = result.ToBitmap();

                                double clusterDistance = double.Parse(txtClusterDistance.Text);
                                double quantizeDistance = double.Parse(txtQuantizeDistance.Text);
                                double diversityDistance = double.Parse(txtMinColorDistanceInPalette.Text);
                                int ditherPaletteSize = int.Parse(txtDitherPaletteSize.Text);

                                Bitmap reduced;
                                int[,] labelMap;
                                List<Color> clusterColors;


                                SimpleLChDistanceReducer.ReduceWithLabels(
                                    bmpDenoised,
                                    clusterDistance,
                                    quantizeDistance,
                                    out reduced,
                                    out labelMap,
                                    out clusterColors
                                );

                                string reducedPath = Path.Combine(baseDir, baseName + "_Reduced.png");
                                reduced.Save(reducedPath);
                                HashSet<Color> usedColors = new HashSet<Color>();
                                for (int y = 0; y < reduced.Height; y++)
                                {
                                    for (int x = 0; x < reduced.Width; x++)
                                    {
                                        usedColors.Add(reduced.GetPixel(x, y));
                                    }
                                }
                                txtQuantizeColorSize.Text = usedColors.Count.ToString();
                                txtClusterSize.Text = clusterColors.Count.ToString();

                                string labelPath = Path.Combine(baseDir, baseName + "_LabelMap.png");
                                SimpleLChDistanceReducer.ExportClusterLabelMapAsImage(labelMap, clusterColors, labelPath);

                                string ditherPath = Path.Combine(baseDir, baseName + "_Dithered.png");
                                var dithered = SimpleLChDistanceReducer.UpscaleWith2x2Dither(labelMap, clusterColors, ditherPaletteSize, diversityDistance);
                                dithered.Save(ditherPath);
                                MessageBox.Show("LCh減色＋ディザ展開が完了しました！", "完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                // 入力画像はそのまま
                                Bitmap bmpDenoised = original;

                                double clusterDistance = double.Parse(txtClusterDistance.Text);
                                double quantizeDistance = double.Parse(txtQuantizeDistance.Text);

                                Bitmap reduced;
                                int[,] labelMap;
                                List<Color> clusterColors;

                                // 減色処理（解像度変更せず）
                                SimpleLChDistanceReducer.ReduceWithLabels(
                                    bmpDenoised,
                                    clusterDistance,
                                    quantizeDistance,
                                    out reduced,
                                    out labelMap,
                                    out clusterColors
                                );

                                // 保存パス
                                string reducedPath = Path.Combine(baseDir, baseName + "_Reduced_NoDither.png");
                                reduced.Save(reducedPath);

                                // 色数をカウントしてUIに反映
                                HashSet<Color> usedColors = new HashSet<Color>();
                                for (int y = 0; y < reduced.Height; y++)
                                {
                                    for (int x = 0; x < reduced.Width; x++)
                                    {
                                        usedColors.Add(reduced.GetPixel(x, y));
                                    }
                                }
                                txtQuantizeColorSize.Text = usedColors.Count.ToString();
                                txtClusterSize.Text = clusterColors.Count.ToString();

                                // ラベルマップ出力（クラスタ別に確認可能）
                                string labelPath = Path.Combine(baseDir, baseName + "_LabelMap_NoDither.png");
                                SimpleLChDistanceReducer.ExportClusterLabelMapAsImage(labelMap, clusterColors, labelPath);

                                MessageBox.Show("LCh減色が完了しました！", "完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, "処理中にエラーが発生しました。");
                        MessageBox.Show("処理中にエラーが発生しました。\n" + ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            };
        }

        private void ExtractLine()
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "画像ファイル (*.png)|*.png",
                Title = "画像を選択してください"
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string inputPath = dialog.FileName;

                // ファイル名ベース
                string baseDir = Path.GetDirectoryName(inputPath);
                string baseName = Path.GetFileNameWithoutExtension(inputPath);

                string outputLinePath = Path.Combine(baseDir, baseName + "_LINE.png");
                string tempMaskPath = Path.Combine(baseDir, baseName + "_tmp.png");
                string outputNoLinePath = Path.Combine(baseDir, baseName + "_WITHOUTLINE.png");

                // 1. 線画抽出
                ExtractLineArtWithCanny(inputPath, tempMaskPath,
                    cannyThresh1: 80,
                    cannyThresh2: 180,
                    lineThickness: 1
                );

                // 2. マスク → α付きPNG
                ConvertLineToTransparent(tempMaskPath, outputLinePath);

                // 3. Inpaint処理で線を自然補間
                //expansionSize = 0：ほぼ線のピクセルのみ補完（最小）
                // 1〜2：おすすめの自然補完範囲
                // 3以上：塗りがボケすぎる可能性あるから要チェック！
                RemoveLinesWithInpaint(inputPath, tempMaskPath, outputNoLinePath, expansionSize:1);



                // 一時ファイル削除
                if (File.Exists(tempMaskPath)) File.Delete(tempMaskPath);

                MessageBox.Show($"3ファイルを生成しました！\n\n{outputLinePath}\n{outputNoLinePath}");
            }
        }

        /// <summary>
        /// 白黒の線画画像を読み込み、白い部分（輪郭線）を黒に塗り替え、
        /// その他の領域は完全透明なピクセルに変換して保存します。
        /// </summary>
        /// <param name="inputPath">入力ファイルのパス（PNG形式、線画画像）</param>
        /// <param name="outputPath">出力ファイルのパス（PNG形式、α付き）</param>
        /// <remarks>
        /// 入力画像は白背景に白い線（Canny出力など）を想定。
        /// 白い線（RGB値が128以上）を黒い線（#000000, α=255）に変換し、
        /// それ以外の背景部分は完全に透明（α=0）として扱います。
        /// 出力形式は 32bpp ARGB PNG です。
        /// </remarks>
        /// <example>
        /// inputPath: "image_LINE.png"
        /// outputPath: "image_LINE_alpha.png"
        /// 出力は、黒い輪郭線のみが見える透明背景の画像となります。
        /// </example>
        private void ConvertLineToTransparent(string inputPath, string outputPath)
        {
            using (Bitmap src = new Bitmap(inputPath))
            using (Bitmap dst = new Bitmap(src.Width, src.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                for (int y = 0; y < src.Height; y++)
                {
                    for (int x = 0; x < src.Width; x++)
                    {
                        Color pixel = src.GetPixel(x, y);
                        if (pixel.R > 128) // ← エッジ（白）を線とみなす
                        {
                            dst.SetPixel(x, y, Color.FromArgb(255, 0, 0, 0)); // 不透明な黒線
                        }
                        else
                        {
                            dst.SetPixel(x, y, Color.FromArgb(0, 0, 0, 0)); // 完全透明
                        }
                    }
                }

                dst.Save(outputPath, System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        /// <summary>
        /// 指定した画像に対して Canny エッジ検出を行い、線画画像（モノクロ）を生成・保存します。
        /// 線の太さは膨張処理により調整可能です。
        /// </summary>
        /// <param name="inputPath">入力画像ファイルのパス（カラー画像）</param>
        /// <param name="outputPath">出力画像ファイルのパス（モノクロ線画PNG）</param>
        /// <param name="cannyThresh1">Cannyエッジ検出の下限閾値（小さいほど細い線を検出）</param>
        /// <param name="cannyThresh2">Cannyエッジ検出の上限閾値（大きいほど強い輪郭のみ抽出）</param>
        /// <param name="lineThickness">線の太さ（1なら無加工、2以上で膨張）</param>
        /// <remarks>
        /// 入力画像は内部でグレースケールに変換されます。
        /// Cannyエッジ検出によって、白い線が黒背景上に描かれたモノクロ線画画像を生成します。
        /// 線の太さは膨張処理（dilation）により調整可能です。
        /// 出力画像は .png 形式で保存されます。
        /// </remarks>
        /// <example>
        /// ExtractLineArtWithCanny("input.png", "output_LINE.png", 80, 180, 2);
        /// </example>
        private void ExtractLineArtWithCanny(string inputPath, string outputPath,
             double cannyThresh1, double cannyThresh2, int lineThickness)
        {
            // 元画像を読み込み
            Mat src = CvInvoke.Imread(inputPath, ImreadModes.Color);

            // グレースケール化
            Mat gray = new Mat();
            CvInvoke.CvtColor(src, gray, ColorConversion.Bgr2Gray);

            // Cannyエッジ検出
            Mat edge = new Mat();
            CvInvoke.Canny(gray, edge, cannyThresh1, cannyThresh2);

            // 線の太さ調整（オプション）
            if (lineThickness > 1)
            {
                Mat kernel = CvInvoke.GetStructuringElement(ElementShape.Rectangle,
                    new Size(lineThickness, lineThickness), new Point(-1, -1));
                CvInvoke.Dilate(edge, edge, kernel, new Point(-1, -1), 1, BorderType.Default, default);
            }

            // 出力画像保存
            CvInvoke.Imwrite(outputPath, edge);
        }

        /// <summary>
        /// 輪郭線マスクを元に、線を自然に塗りつぶす（inpaint）
        /// </summary>
        /// <param name="originalPath">元画像</param>
        /// <param name="maskPath">白黒マスク（白＝消す）</param>
        /// <param name="outputPath">保存先</param>
        /// <param name="expansionSize">線の影響範囲（膨張サイズ）</param>
        private void RemoveLinesWithInpaint(string originalPath, string maskPath, string outputPath, int expansionSize)
        {
            Mat original = CvInvoke.Imread(originalPath, ImreadModes.Color);
            Mat maskInput = CvInvoke.Imread(maskPath, ImreadModes.Grayscale);

            // マスク化
            Mat mask = new Mat();
            CvInvoke.Threshold(maskInput, mask, 127, 255, ThresholdType.Binary);

            // 🔄 輪郭線マスクを膨張
            if (expansionSize > 0)
            {
                Mat kernel = CvInvoke.GetStructuringElement(ElementShape.Rectangle,
                    new Size(expansionSize * 2 + 1, expansionSize * 2 + 1), new Point(-1, -1));
                CvInvoke.Dilate(mask, mask, kernel, new Point(-1, -1), 1, BorderType.Default, default);
            }

            // Inpaintで補完
            Mat result = new Mat();
            CvInvoke.Inpaint(original, mask, result, 3, InpaintType.Telea);

            CvInvoke.Imwrite(outputPath, result);
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
