using System;
using System.Windows.Forms;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Views.Base;

namespace S131267 {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) {
            dataSet1.ReadXml(@"..\..\nwind.xml");
            bindingSource1.DataMember = "Categories";
        }

        private void OnGridViewDoubleClick(object sender, EventArgs e) {
            GridView gView = (GridView)sender;
            GridHitInfo hInfo = gView.CalcHitInfo(gView.GridControl.PointToClient(Control.MousePosition));
            if (hInfo.InRowCell) {
                new EditForm(Control.MousePosition, gView.Columns, bindingSource1, BindingContext).ShowDialog(this);
                gView.UpdateCurrentRow();
            }
        }
    }
}