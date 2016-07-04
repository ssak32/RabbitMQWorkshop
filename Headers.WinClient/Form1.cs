using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Headers.WinClient
{
    public partial class Form1 : Form
    {
        int messageCount = 0;
        public Form1()
        {
            InitializeComponent();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            var headers = new Dictionary<string, string>();
            var messageSender = new RabbitSender();

            headers.Add("category", this.GetComboItem(this.cboSpecies));
            headers.Add("type", this.GetComboItem(this.cboType));

            var message = string.Format("Message: {0}", messageCount);
            messageSender.Send(message, headers);
            MessageBox.Show(string.Format("Sending - {0}", message));
            messageCount++;
        }

        private string GetComboItem(ComboBox comboBox)
        {
            if (string.IsNullOrEmpty(comboBox.Text))
                return string.Empty;

            return comboBox.Text;
        }
    }
}
