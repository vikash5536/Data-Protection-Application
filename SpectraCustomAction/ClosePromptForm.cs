using System;
using System.Windows.Forms;

namespace DataProtectionApplication.SpectraCustomAction
{
    public partial class ClosePromptForm : Form
    {
        public ClosePromptForm(string text)
        {
            InitializeComponent();
            messageText.Text = text;
        }

        private void OkButtonClick(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
