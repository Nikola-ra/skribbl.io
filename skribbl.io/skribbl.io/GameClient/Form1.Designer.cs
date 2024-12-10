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
            this.penSizeSlider = new System.Windows.Forms.TrackBar();
            this.penWidthLabel = new System.Windows.Forms.Label();
            this.canvas = new System.Windows.Forms.PictureBox();
            this.guessButton = new System.Windows.Forms.Button();
            this.guessTextBox = new System.Windows.Forms.TextBox();
            this.colorPickerButton = new System.Windows.Forms.Button();
            this.roleLabel = new System.Windows.Forms.Label();
            this.eraserButton = new System.Windows.Forms.Button();
            this.playerNameLabel = new System.Windows.Forms.Label();
            this.wordChoice = new System.Windows.Forms.ComboBox();
            this.wordLabel = new System.Windows.Forms.Label();
            this.colorPanel = new System.Windows.Forms.Panel();
            this.guessesList = new System.Windows.Forms.ListBox();
            ((System.ComponentModel.ISupportInitialize)(this.penSizeSlider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.canvas)).BeginInit();
            this.SuspendLayout();
            // 
            // penSizeSlider
            // 
            this.penSizeSlider.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.penSizeSlider.Location = new System.Drawing.Point(25, 105);
            this.penSizeSlider.Maximum = 50;
            this.penSizeSlider.Name = "penSizeSlider";
            this.penSizeSlider.Size = new System.Drawing.Size(102, 45);
            this.penSizeSlider.TabIndex = 1;
            this.penSizeSlider.Scroll += new System.EventHandler(this.penSizeSlider_Scroll);
            // 
            // penWidthLabel
            // 
            this.penWidthLabel.AutoSize = true;
            this.penWidthLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.penWidthLabel.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.penWidthLabel.Location = new System.Drawing.Point(31, 84);
            this.penWidthLabel.Name = "penWidthLabel";
            this.penWidthLabel.Size = new System.Drawing.Size(95, 18);
            this.penWidthLabel.TabIndex = 2;
            this.penWidthLabel.Text = "Pen Width :";
            // 
            // canvas
            // 
            this.canvas.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.canvas.Location = new System.Drawing.Point(204, 55);
            this.canvas.Name = "canvas";
            this.canvas.Size = new System.Drawing.Size(599, 385);
            this.canvas.TabIndex = 3;
            this.canvas.TabStop = false;
            // 
            // guessButton
            // 
            this.guessButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guessButton.Location = new System.Drawing.Point(24, 396);
            this.guessButton.Name = "guessButton";
            this.guessButton.Size = new System.Drawing.Size(102, 40);
            this.guessButton.TabIndex = 5;
            this.guessButton.Text = "Guess";
            this.guessButton.UseVisualStyleBackColor = true;
            this.guessButton.Click += new System.EventHandler(this.guessButton_Click);
            // 
            // guessTextBox
            // 
            this.guessTextBox.Location = new System.Drawing.Point(24, 370);
            this.guessTextBox.Name = "guessTextBox";
            this.guessTextBox.Size = new System.Drawing.Size(102, 20);
            this.guessTextBox.TabIndex = 6;
            // 
            // colorPickerButton
            // 
            this.colorPickerButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.colorPickerButton.Location = new System.Drawing.Point(24, 156);
            this.colorPickerButton.Name = "colorPickerButton";
            this.colorPickerButton.Size = new System.Drawing.Size(102, 40);
            this.colorPickerButton.TabIndex = 7;
            this.colorPickerButton.Text = "Pen Color";
            this.colorPickerButton.UseVisualStyleBackColor = true;
            this.colorPickerButton.Click += new System.EventHandler(this.colorPickerButton_Click);
            // 
            // roleLabel
            // 
            this.roleLabel.AutoSize = true;
            this.roleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.roleLabel.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.roleLabel.Location = new System.Drawing.Point(720, 20);
            this.roleLabel.Name = "roleLabel";
            this.roleLabel.Size = new System.Drawing.Size(83, 18);
            this.roleLabel.TabIndex = 8;
            this.roleLabel.Text = "Your Role";
            // 
            // eraserButton
            // 
            this.eraserButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.eraserButton.Location = new System.Drawing.Point(24, 202);
            this.eraserButton.Name = "eraserButton";
            this.eraserButton.Size = new System.Drawing.Size(102, 40);
            this.eraserButton.TabIndex = 9;
            this.eraserButton.Text = "Eraser";
            this.eraserButton.UseVisualStyleBackColor = true;
            this.eraserButton.Click += new System.EventHandler(this.eraserButton_Click);
            // 
            // playerNameLabel
            // 
            this.playerNameLabel.AutoSize = true;
            this.playerNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.playerNameLabel.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.playerNameLabel.Location = new System.Drawing.Point(12, 9);
            this.playerNameLabel.Name = "playerNameLabel";
            this.playerNameLabel.Size = new System.Drawing.Size(181, 31);
            this.playerNameLabel.TabIndex = 10;
            this.playerNameLabel.Text = "Player Name";
            // 
            // wordChoice
            // 
            this.wordChoice.FormattingEnabled = true;
            this.wordChoice.Location = new System.Drawing.Point(376, 202);
            this.wordChoice.Name = "wordChoice";
            this.wordChoice.Size = new System.Drawing.Size(241, 21);
            this.wordChoice.TabIndex = 11;
            this.wordChoice.SelectionChangeCommitted += new System.EventHandler(this.wordChoice_SelectionChangeCommitted);
            // 
            // wordLabel
            // 
            this.wordLabel.AutoSize = true;
            this.wordLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 17F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.wordLabel.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.wordLabel.Location = new System.Drawing.Point(309, 11);
            this.wordLabel.Name = "wordLabel";
            this.wordLabel.Size = new System.Drawing.Size(147, 29);
            this.wordLabel.TabIndex = 12;
            this.wordLabel.Text = "Word Label";
            // 
            // colorPanel
            // 
            this.colorPanel.Location = new System.Drawing.Point(24, 248);
            this.colorPanel.Name = "colorPanel";
            this.colorPanel.Size = new System.Drawing.Size(102, 70);
            this.colorPanel.TabIndex = 13;
            // 
            // guessesList
            // 
            this.guessesList.FormattingEnabled = true;
            this.guessesList.Location = new System.Drawing.Point(805, 58);
            this.guessesList.Name = "guessesList";
            this.guessesList.Size = new System.Drawing.Size(192, 381);
            this.guessesList.TabIndex = 14;
            // 
            // Form1
            // 
            this.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.ClientSize = new System.Drawing.Size(1042, 605);
            this.Controls.Add(this.guessesList);
            this.Controls.Add(this.colorPanel);
            this.Controls.Add(this.wordLabel);
            this.Controls.Add(this.wordChoice);
            this.Controls.Add(this.playerNameLabel);
            this.Controls.Add(this.eraserButton);
            this.Controls.Add(this.roleLabel);
            this.Controls.Add(this.colorPickerButton);
            this.Controls.Add(this.guessTextBox);
            this.Controls.Add(this.guessButton);
            this.Controls.Add(this.canvas);
            this.Controls.Add(this.penWidthLabel);
            this.Controls.Add(this.penSizeSlider);
            this.Name = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.penSizeSlider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.canvas)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private TrackBar penSizeSlider;
        private Label penWidthLabel;
        private PictureBox canvas;
        private Button guessButton;
        private TextBox guessTextBox;
        private Button colorPickerButton;
        private Label roleLabel;
        private Button eraserButton;
        private Label playerNameLabel;
        private ComboBox wordChoice;
        private Label wordLabel;
        private Panel colorPanel;
        private ListBox guessesList;
    }
}

