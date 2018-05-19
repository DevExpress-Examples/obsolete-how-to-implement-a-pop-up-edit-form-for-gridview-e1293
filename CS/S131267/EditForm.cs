using System;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Collections.Generic;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;

namespace S131267 {
    public partial class EditForm : Form {
        private const int EditorWidth = 100;
        private const int MultilineEditorHeight = 96;
        private const int SimpleEditorHeight = 20;
        private const int Indent = 5;

        private Dictionary<string, object> oldValues = new Dictionary<string, object>();
        private bool allowTrackValueChanges;

        private int xLocation = Indent;
        private int yLocation = Indent;
        private int maxWidth;
        private int colNum = 1;

        private GridColumnCollection gridColumns;
        private object dataSource;

        List<string> simpleColumns = new List<string>();
        List<string> multilineColumns = new List<string>();

        public EditForm() {
            InitializeComponent();
        }

        public EditForm(Point location, GridColumnCollection columns, object dataSource, BindingContext context)
            : this() {
            StartPosition = FormStartPosition.Manual;
            Location = location;
            BindingContext = context;
            allowTrackValueChanges = false;
            this.dataSource = dataSource;
            gridColumns = columns;
            PopulateColumnLists();
            for (int j = 0; j < colNum; j++) {
                CreateCol(j, colNum, simpleColumns, j, SimpleEditorHeight);
            }
            if (multilineColumns.Count > 0)
                CreateCol(0, 1, multilineColumns, 4, MultilineEditorHeight);
            Width = xLocation;
        }

        private void CreateEditor(GridColumn column, string name) {
            BaseEdit edit = column.ColumnEdit == null ? new TextEdit() : column.ColumnEdit.CreateEditor();
            edit.Name = name;
            edit.Width = EditorWidth;
            edit.Location = new Point(xLocation, yLocation);
            edit.DataBindings.Add("EditValue", dataSource, column.FieldName);
            edit.EditValueChanging += new ChangingEventHandler(OnEditorEditValueChanging);
            Controls.Add(edit);
        }

        private void CreateLabel(string name, string caption) {
            LabelControl label = new LabelControl();
            label.Name = name;
            label.Text = caption;
            label.Location = new Point(xLocation, yLocation);
            maxWidth = Math.Max(maxWidth, label.Width);
            Controls.Add(label);
        }

        private void CreateCol(int startIndex, int step, List<string> columns, int col, int height) {
            for (int i = startIndex; i < columns.Count; i += step) {
                CreateLabel(string.Format("label_{1}_{0}", i, col), gridColumns[columns[i]].Caption);
                yLocation += height + Indent;
            }
            xLocation += maxWidth + Indent;
            yLocation = Indent;
            for (int i = startIndex; i < columns.Count; i += step) {
                CreateEditor(gridColumns[columns[i]], string.Format("edit_{1}_{0}", i, col));
                yLocation += height + Indent;
            }
            Height = Math.Max(Height, yLocation);
            xLocation += EditorWidth + Indent;
            yLocation = Indent;
            maxWidth = 0;
        }

        private void PopulateColumnLists() {
            foreach (GridColumn column in gridColumns) {
                if (column.Visible == false || column.GroupIndex >= 0) continue;
                if (column.ColumnEdit is RepositoryItemMemoEdit ||
                    column.ColumnEdit is RepositoryItemPictureEdit
                    || column.ColumnEdit is RepositoryItemImageEdit)
                    multilineColumns.Add(column.FieldName);
                else simpleColumns.Add(column.FieldName);
            }
            if (simpleColumns.Count > 4)
                if (simpleColumns.Count > 6 && multilineColumns.Count == 0)
                    colNum = 3;
                else colNum = 2;
        }

        private void OnEditorEditValueChanging(object sender, ChangingEventArgs e) {
            if (!allowTrackValueChanges) return;
            BaseEdit edit = (BaseEdit)sender;
            if (!oldValues.ContainsKey(edit.Name))
                oldValues.Add(edit.Name, e.OldValue);
        }

        private void OnApplyClick(object sender, EventArgs e) {
            Close();
        }

        private void OnCancelClick(object sender, EventArgs e) {
            foreach (KeyValuePair<string, object> kvp in oldValues)
                ((BaseEdit)Controls[kvp.Key]).EditValue = kvp.Value;
            Close();
        }

        private void OnShown(object sender, EventArgs e) {
            allowTrackValueChanges = true;
        }
    }
}