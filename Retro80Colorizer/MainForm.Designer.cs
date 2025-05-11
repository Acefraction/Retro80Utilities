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
            this.txtLCHDistance = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtLChClusterNumbers = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnPalleteChoice
            // 
            this.btnPalleteChoice.Location = new System.Drawing.Point(12, 12);
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
            this.btnExtractLine.Location = new System.Drawing.Point(12, 41);
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
            // txtLCHDistance
            // 
            this.txtLCHDistance.Location = new System.Drawing.Point(221, 70);
            this.txtLCHDistance.Name = "txtLCHDistance";
            this.txtLCHDistance.Size = new System.Drawing.Size(100, 19);
            this.txtLCHDistance.TabIndex = 4;
            this.txtLCHDistance.Text = "5.0";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(164, 75);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 12);
            this.label1.TabIndex = 5;
            this.label1.Text = "LCH距離";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(327, 73);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 6;
            this.label2.Text = "色数";
            // 
            // txtLChClusterNumbers
            // 
            this.txtLChClusterNumbers.Location = new System.Drawing.Point(384, 70);
            this.txtLChClusterNumbers.Name = "txtLChClusterNumbers";
            this.txtLChClusterNumbers.Size = new System.Drawing.Size(100, 19);
            this.txtLChClusterNumbers.TabIndex = 7;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1320, 628);
            this.Controls.Add(this.txtLChClusterNumbers);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtLCHDistance);
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
        private System.Windows.Forms.TextBox txtLCHDistance;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtLChClusterNumbers;
    }
}

