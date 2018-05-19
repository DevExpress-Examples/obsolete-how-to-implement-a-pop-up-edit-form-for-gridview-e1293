Imports Microsoft.VisualBasic
Imports System
Imports System.Windows.Forms
Imports DevExpress.XtraGrid.Views.Grid
Imports DevExpress.XtraGrid.Views.Grid.ViewInfo
Imports DevExpress.XtraGrid.Views.Base

Namespace S131267
	Partial Public Class Form1
		Inherits Form
		Public Sub New()
			InitializeComponent()
		End Sub

		Private Sub Form1_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
			dataSet1.ReadXml("..\..\nwind.xml")
			bindingSource1.DataMember = "Categories"
		End Sub

		Private Sub OnGridViewDoubleClick(ByVal sender As Object, ByVal e As EventArgs) Handles gridView1.DoubleClick
			Dim gView As GridView = CType(sender, GridView)
			Dim hInfo As GridHitInfo = gView.CalcHitInfo(gView.GridControl.PointToClient(Control.MousePosition))
			If hInfo.InRowCell Then
				CType(New EditForm(Control.MousePosition, gView.Columns, bindingSource1, BindingContext), EditForm).ShowDialog(Me)
				gView.UpdateCurrentRow()
			End If
		End Sub
	End Class
End Namespace