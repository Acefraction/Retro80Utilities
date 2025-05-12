namespace Retro80Utilities
{
    partial class MainForm
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.btnPalleteChoice = new System.Windows.Forms.Button();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.btnExtractLine = new System.Windows.Forms.Button();
            this.btnTestLChDistanceColorRedude = new System.Windows.Forms.Button();
            this.txtQuantizeDistance = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtQuantizeColorSize = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtClusterDistance = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtClusterSize = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txtMinColorDistanceInPalette = new System.Windows.Forms.TextBox();
            this.txtDitherPaletteSize = new System.Windows.Forms.TextBox();
            this.chkDither = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnPalleteChoice
            // 
            this.btnPalleteChoice.Location = new System.Drawing.Point(12, 146);
            this.btnPalleteChoice.Name = "btnPalleteChoice";
            this.btnPalleteChoice.Size = new System.Drawing.Size(146, 23);
            this.btnPalleteChoice.TabIndex = 0;
            this.btnPalleteChoice.Text = "パレット選択";
            this.btnPalleteChoice.UseVisualStyleBackColor = true;
            // 
            // statusStrip
            // 
            this.statusStrip.Location = new System.Drawing.Point(0, 606);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1320, 22);
            this.statusStrip.TabIndex = 1;
            this.statusStrip.Text = "statusStrip1";
            // 
            // btnExtractLine
            // 
            this.btnExtractLine.Location = new System.Drawing.Point(12, 12);
            this.btnExtractLine.Name = "btnExtractLine";
            this.btnExtractLine.Size = new System.Drawing.Size(146, 23);
            this.btnExtractLine.TabIndex = 2;
            this.btnExtractLine.Text = "輪郭線抽出サンプル";
            this.btnExtractLine.UseVisualStyleBackColor = true;
            // 
            // btnTestLChDistanceColorRedude
            // 
            this.btnTestLChDistanceColorRedude.Location = new System.Drawing.Point(12, 70);
            this.btnTestLChDistanceColorRedude.Name = "btnTestLChDistanceColorRedude";
            this.btnTestLChDistanceColorRedude.Size = new System.Drawing.Size(146, 23);
            this.btnTestLChDistanceColorRedude.TabIndex = 3;
            this.btnTestLChDistanceColorRedude.Text = "LCH空間色圧縮";
            this.btnTestLChDistanceColorRedude.UseVisualStyleBackColor = true;
            // 
            // txtQuantizeDistance
            // 
            this.txtQuantizeDistance.Location = new System.Drawing.Point(303, 122);
            this.txtQuantizeDistance.Name = "txtQuantizeDistance";
            this.txtQuantizeDistance.Size = new System.Drawing.Size(100, 19);
            this.txtQuantizeDistance.TabIndex = 4;
            this.txtQuantizeDistance.Text = "6";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(163, 125);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 12);
            this.label1.TabIndex = 5;
            this.label1.Text = "量子化LCH距離";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(424, 125);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 6;
            this.label2.Text = "量子化色数";
            // 
            // txtQuantizeColorSize
            // 
            this.txtQuantizeColorSize.Location = new System.Drawing.Point(534, 126);
            this.txtQuantizeColorSize.Name = "txtQuantizeColorSize";
            this.txtQuantizeColorSize.Size = new System.Drawing.Size(100, 19);
            this.txtQuantizeColorSize.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(164, 101);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(106, 12);
            this.label3.TabIndex = 8;
            this.label3.Text = "クラスター化LCH距離";
            // 
            // txtClusterDistance
            // 
            this.txtClusterDistance.Location = new System.Drawing.Point(303, 94);
            this.txtClusterDistance.Name = "txtClusterDistance";
            this.txtClusterDistance.Size = new System.Drawing.Size(100, 19);
            this.txtClusterDistance.TabIndex = 9;
            this.txtClusterDistance.Text = "18";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(424, 101);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 12);
            this.label4.TabIndex = 10;
            this.label4.Text = "クラスター数";
            // 
            // txtClusterSize
            // 
            this.txtClusterSize.Location = new System.Drawing.Point(534, 98);
            this.txtClusterSize.Name = "txtClusterSize";
            this.txtClusterSize.Size = new System.Drawing.Size(100, 19);
            this.txtClusterSize.TabIndex = 11;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(424, 75);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(101, 12);
            this.label5.TabIndex = 12;
            this.label5.Text = "クラスタディザ色上限";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(164, 75);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(105, 12);
            this.label6.TabIndex = 13;
            this.label6.Text = "ディザカラーLCH距離";
            // 
            // txtMinColorDistanceInPalette
            // 
            this.txtMinColorDistanceInPalette.Location = new System.Drawing.Point(303, 68);
            this.txtMinColorDistanceInPalette.Name = "txtMinColorDistanceInPalette";
            this.txtMinColorDistanceInPalette.Size = new System.Drawing.Size(100, 19);
            this.txtMinColorDistanceInPalette.TabIndex = 14;
            this.txtMinColorDistanceInPalette.Text = "3";
            // 
            // txtDitherPaletteSize
            // 
            this.txtDitherPaletteSize.Location = new System.Drawing.Point(534, 68);
            this.txtDitherPaletteSize.Name = "txtDitherPaletteSize";
            this.txtDitherPaletteSize.Size = new System.Drawing.Size(100, 19);
            this.txtDitherPaletteSize.TabIndex = 15;
            this.txtDitherPaletteSize.Text = "16";
            // 
            // chkDither
            // 
            this.chkDither.AutoSize = true;
            this.chkDither.Checked = true;
            this.chkDither.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDither.Location = new System.Drawing.Point(95, 94);
            this.chkDither.Name = "chkDither";
            this.chkDither.Size = new System.Drawing.Size(63, 16);
            this.chkDither.TabIndex = 16;
            this.chkDither.Text = "ディザ化";
            this.chkDither.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1320, 628);
            this.Controls.Add(this.chkDither);
            this.Controls.Add(this.txtDitherPaletteSize);
            this.Controls.Add(this.txtMinColorDistanceInPalette);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtClusterSize);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtClusterDistance);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtQuantizeColorSize);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtQuantizeDistance);
            this.Controls.Add(this.btnTestLChDistanceColorRedude);
            this.Controls.Add(this.btnExtractLine);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.btnPalleteChoice);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Retro80減色ユーティリティ";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnPalleteChoice;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.Button btnExtractLine;
        private System.Windows.Forms.Button btnTestLChDistanceColorRedude;
        private System.Windows.Forms.TextBox txtQuantizeDistance;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtQuantizeColorSize;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtClusterDistance;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtClusterSize;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtMinColorDistanceInPalette;
        private System.Windows.Forms.TextBox txtDitherPaletteSize;
        private System.Windows.Forms.CheckBox chkDither;
    }
}

