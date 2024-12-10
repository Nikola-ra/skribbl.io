using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameClient
{
    public partial class WinForm : Form
    {
        Form1 initialForm;
        public WinForm(Form1 form)
        {
            InitializeComponent();
            initialForm = form;
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            var message = new
            {
                type = "ready",
                player = initialForm._playerName
            };

            initialForm.SendMessage(Newtonsoft.Json.JsonConvert.SerializeObject(message));
            this.Hide();
            initialForm.Show();
            initialForm.Activate();
        }

        private void WinForm_Load(object sender, EventArgs e)
        {
            winnerLabel.Text = $"{initialForm._winner} Wins!";
        }
    }
}
