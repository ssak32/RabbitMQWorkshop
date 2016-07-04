using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Topic.WinClient
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
            var topic = new List<string>();
            var messageSender = new RabbitSender();

            topic.Add(this.GetComboItem(this.cboCustType));
            topic.Add(this.GetComboItem(this.cboOrderSize));
            topic.Add(this.GetComboItem(this.cboProduct));

            var message = string.Format("Message: {0}", messageCount);
            var routingKey = messageSender.Send(message, topic);
            MessageBox.Show(string.Format("Sending - {0}, Routing Key - {1}", message, routingKey.ToString()));
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
