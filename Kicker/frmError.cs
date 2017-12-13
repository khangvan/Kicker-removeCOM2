using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Kicker
{
    public partial class frmError : Form
    {
        public frmError()
        {
            InitializeComponent();
        }


        public frmError(string strMessage)
        {
            label1.Text = strMessage;
        }
    }
}
