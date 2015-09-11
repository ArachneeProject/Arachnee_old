namespace DatabaseEditor
{
    partial class Form1
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
            this.MovieSelector = new System.Windows.Forms.ComboBox();
            this.UpdateButton = new System.Windows.Forms.Button();
            this.ArtistSelector = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.DirectorLabel = new System.Windows.Forms.Label();
            this.ActorsLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.JobSelector = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // MovieSelector
            // 
            this.MovieSelector.FormattingEnabled = true;
            this.MovieSelector.Location = new System.Drawing.Point(141, 12);
            this.MovieSelector.Name = "MovieSelector";
            this.MovieSelector.Size = new System.Drawing.Size(274, 24);
            this.MovieSelector.TabIndex = 0;
            this.MovieSelector.Text = "Select Movie...";
            this.MovieSelector.SelectedIndexChanged += new System.EventHandler(this.MovieSelector_SelectedIndexChanged);
            // 
            // UpdateButton
            // 
            this.UpdateButton.Location = new System.Drawing.Point(199, 438);
            this.UpdateButton.Name = "UpdateButton";
            this.UpdateButton.Size = new System.Drawing.Size(130, 43);
            this.UpdateButton.TabIndex = 1;
            this.UpdateButton.Text = "Update";
            this.UpdateButton.UseVisualStyleBackColor = true;
            this.UpdateButton.Click += new System.EventHandler(this.UpdateDatabase);
            // 
            // ArtistSelector
            // 
            this.ArtistSelector.Enabled = false;
            this.ArtistSelector.FormattingEnabled = true;
            this.ArtistSelector.Location = new System.Drawing.Point(42, 345);
            this.ArtistSelector.Name = "ArtistSelector";
            this.ArtistSelector.Size = new System.Drawing.Size(169, 24);
            this.ArtistSelector.TabIndex = 3;
            this.ArtistSelector.Text = "Select Artist...";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(54, 108);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 17);
            this.label2.TabIndex = 5;
            this.label2.Text = "Actors";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(54, 75);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 17);
            this.label3.TabIndex = 7;
            this.label3.Text = "Director";
            // 
            // DirectorLabel
            // 
            this.DirectorLabel.AutoSize = true;
            this.DirectorLabel.Location = new System.Drawing.Point(118, 75);
            this.DirectorLabel.Name = "DirectorLabel";
            this.DirectorLabel.Size = new System.Drawing.Size(69, 17);
            this.DirectorLabel.TabIndex = 8;
            this.DirectorLabel.Text = "John Doe";
            // 
            // ActorsLabel
            // 
            this.ActorsLabel.AutoSize = true;
            this.ActorsLabel.Location = new System.Drawing.Point(118, 108);
            this.ActorsLabel.Name = "ActorsLabel";
            this.ActorsLabel.Size = new System.Drawing.Size(150, 17);
            this.ActorsLabel.TabIndex = 9;
            this.ActorsLabel.Text = "John Doe Jeanne Doe";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(229, 348);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(23, 17);
            this.label1.TabIndex = 10;
            this.label1.Text = "as";
            // 
            // JobSelector
            // 
            this.JobSelector.Enabled = false;
            this.JobSelector.FormattingEnabled = true;
            this.JobSelector.Location = new System.Drawing.Point(271, 345);
            this.JobSelector.Name = "JobSelector";
            this.JobSelector.Size = new System.Drawing.Size(169, 24);
            this.JobSelector.TabIndex = 11;
            this.JobSelector.Text = "Select Job...";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(513, 540);
            this.Controls.Add(this.JobSelector);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ActorsLabel);
            this.Controls.Add(this.DirectorLabel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ArtistSelector);
            this.Controls.Add(this.UpdateButton);
            this.Controls.Add(this.MovieSelector);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox MovieSelector;
        private System.Windows.Forms.Button UpdateButton;
        private System.Windows.Forms.ComboBox ArtistSelector;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label DirectorLabel;
        private System.Windows.Forms.Label ActorsLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox JobSelector;

    }
}

