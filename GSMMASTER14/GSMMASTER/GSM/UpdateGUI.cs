using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows.Forms;

namespace GSMMASTER
{
    public class UpdateGUI
    {
        public static event UIViewRow dataForm;
        public static void ChangeRow(DataGridViewRow row, string name, string value) => dataForm(row, name, value);
    }
}
