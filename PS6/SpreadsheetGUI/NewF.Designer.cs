namespace SS
{
    partial class NewF
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.NewLabel = new System.Windows.Forms.Label();
            this.NewTextBox = new System.Windows.Forms.TextBox();
            this.NewOKButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // NewLabel
            // 
            this.NewLabel.AutoSize = true;
            this.NewLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NewLabel.Location = new System.Drawing.Point(12, 12);
            this.NewLabel.Name = "NewLabel";
            this.NewLabel.Size = new System.Drawing.Size(83, 13);
            this.NewLabel.TabIndex = 0;
            this.NewLabel.Text = "Enter a filename";
            // 
            // NewTextBox
            // 
            this.NewTextBox.Location = new System.Drawing.Point(120, 9);
            this.NewTextBox.Name = "NewTextBox";
            this.NewTextBox.Size = new System.Drawing.Size(207, 20);
            this.NewTextBox.TabIndex = 1;
            // 
            // NewOKButton
            // 
            this.NewOKButton.Location = new System.Drawing.Point(252, 48);
            this.NewOKButton.Name = "NewOKButton";
            this.NewOKButton.Size = new System.Drawing.Size(75, 23);
            this.NewOKButton.TabIndex = 2;
            this.NewOKButton.Text = "OK";
            this.NewOKButton.UseVisualStyleBackColor = true;
            this.NewOKButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // NewF
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(339, 83);
            this.Controls.Add(this.NewOKButton);
            this.Controls.Add(this.NewTextBox);
            this.Controls.Add(this.NewLabel);
            this.Name = "NewF";
            this.Text = "New Spreadsheet";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label NewLabel;
        private System.Windows.Forms.TextBox NewTextBox;
        private System.Windows.Forms.Button NewOKButton;
    }
}