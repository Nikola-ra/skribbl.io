using System;
using System.Drawing;
using System.Windows.Forms;



namespace GameClient
{
    partial class Form1
    {
        /// <summary>
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Pulire le risorse in uso.
        /// </summary>
        /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codice generato da Progettazione Windows Form

        /// <summary>
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            this.canvas = new System.Windows.Forms.PictureBox();
            this.roleLabel = new System.Windows.Forms.Label();
            this.roundLabel = new System.Windows.Forms.Label();
            this.statusLabel = new System.Windows.Forms.Label();
            this.toolboxPanel = new System.Windows.Forms.Panel();
            this.colorPickerButton = new System.Windows.Forms.Button();
            this.penSizeSlider = new System.Windows.Forms.TrackBar();
            this.guessTextBox = new System.Windows.Forms.TextBox();
            this.guessButton = new System.Windows.Forms.Button();
            this.guessesList = new System.Windows.Forms.ListBox();
            this.wordOptions = new System.Windows.Forms.ListBox();
            ((System.ComponentModel.ISupportInitialize)(this.canvas)).BeginInit();
            this.toolboxPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.penSizeSlider)).BeginInit();
            this.SuspendLayout();
            // 
            // canvas
            // 
            this.canvas.BackColor = System.Drawing.Color.White;
            this.canvas.Location = new System.Drawing.Point(150, 50);
            this.canvas.Name = "canvas";
            this.canvas.Size = new System.Drawing.Size(500, 400);
            this.canvas.TabIndex = 0;
            this.canvas.TabStop = false;
            // 
            // roleLabel
            // 
            this.roleLabel.Location = new System.Drawing.Point(20, 10);
            this.roleLabel.Name = "roleLabel";
            this.roleLabel.Size = new System.Drawing.Size(100, 23);
            this.roleLabel.TabIndex = 1;
            this.roleLabel.Text = "Your Role: Guesser";
            // 
            // roundLabel
            // 
            this.roundLabel.Location = new System.Drawing.Point(200, 10);
            this.roundLabel.Name = "roundLabel";
            this.roundLabel.Size = new System.Drawing.Size(100, 23);
            this.roundLabel.TabIndex = 2;
            this.roundLabel.Text = "Round 1 | 30 seconds left";
            // 
            // statusLabel
            // 
            // 
            this.statusLabel.Location = new System.Drawing.Point(400, 10);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(100, 23);
            this.statusLabel.TabIndex = 3;
            this.statusLabel.Text = "Waiting for guesser...";
            // 
            // toolboxPanel
            // 
            this.toolboxPanel.Controls.Add(this.colorPickerButton);
            this.toolboxPanel.Controls.Add(this.penSizeSlider);
            this.toolboxPanel.Location = new System.Drawing.Point(20, 50);
            this.toolboxPanel.Name = "toolboxPanel";
            this.toolboxPanel.Size = new System.Drawing.Size(100, 400);
            this.toolboxPanel.TabIndex = 4;
            // 
            // colorPickerButton
            // 
            this.colorPickerButton.Location = new System.Drawing.Point(0, 0);
            this.colorPickerButton.Name = "colorPickerButton";
            this.colorPickerButton.Size = new System.Drawing.Size(75, 23);
            this.colorPickerButton.TabIndex = 0;
            this.colorPickerButton.Text = "Color";
            // 
            // penSizeSlider
            // 
            this.penSizeSlider.Location = new System.Drawing.Point(0, 0);
            this.penSizeSlider.Minimum = 1;
            this.penSizeSlider.Name = "penSizeSlider";
            this.penSizeSlider.Size = new System.Drawing.Size(104, 45);
            this.penSizeSlider.TabIndex = 1;
            this.penSizeSlider.Value = 1;
            // 
            // guessTextBox
            // 
            this.guessTextBox.Location = new System.Drawing.Point(150, 470);
            this.guessTextBox.Name = "guessTextBox";
            this.guessTextBox.Size = new System.Drawing.Size(100, 20);
            this.guessTextBox.TabIndex = 5;
            // 
            // guessButton
            // 
            this.guessButton.Location = new System.Drawing.Point(300, 470);
            this.guessButton.Name = "guessButton";
            this.guessButton.Size = new System.Drawing.Size(75, 23);
            this.guessButton.TabIndex = 6;
            this.guessButton.Text = "Submit";

            // 
            // guessesList
            // 
            this.guessesList.Location = new System.Drawing.Point(650, 50);
            this.guessesList.Name = "guessesList";
            this.guessesList.Size = new System.Drawing.Size(200, 394);
            this.guessesList.TabIndex = 7;
            // 
            // wordOptions
            // 
            this.wordOptions.Location = new System.Drawing.Point(200, 200);
            this.wordOptions.Name = "wordOptions";
            this.wordOptions.Size = new System.Drawing.Size(200, 95);
            this.wordOptions.TabIndex = 8;
            this.wordOptions.Visible = false;
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(917, 605);
            this.Controls.Add(this.canvas);
            this.Controls.Add(this.roleLabel);
            this.Controls.Add(this.roundLabel);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.toolboxPanel);
            this.Controls.Add(this.guessTextBox);
            this.Controls.Add(this.guessButton);
            this.Controls.Add(this.guessesList);
            this.Controls.Add(this.wordOptions);
            this.Name = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.canvas)).EndInit();
            this.toolboxPanel.ResumeLayout(false);
            this.toolboxPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.penSizeSlider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}

