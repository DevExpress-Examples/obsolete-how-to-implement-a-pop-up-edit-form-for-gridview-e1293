Imports Microsoft.VisualBasic
Imports System
Imports System.Drawing
Imports System.Windows.Forms
Imports DevExpress.XtraEditors
Imports System.Collections.Generic
Imports DevExpress.XtraGrid.Columns
Imports DevExpress.XtraEditors.Controls
Imports DevExpress.XtraEditors.Repository

Namespace S131267
	Partial Public Class EditForm
		Inherits Form
		Private Const EditorWidth As Integer = 100
		Private Const MultilineEditorHeight As Integer = 96
		Private Const SimpleEditorHeight As Integer = 20
		Private Const Indent As Integer = 5

		Private oldValues As New Dictionary(Of String, Object)()
		Private allowTrackValueChanges As Boolean

		Private xLocation As Integer = Indent
		Private yLocation As Integer = Indent
		Private maxWidth As Integer
		Private colNum As Integer = 1

		Private gridColumns As GridColumnCollection
		Private dataSource As Object

		Private simpleColumns As New List(Of String)()
		Private multilineColumns As New List(Of String)()

		Public Sub New()
			InitializeComponent()
		End Sub

		Public Sub New(ByVal location As Point, ByVal columns As GridColumnCollection, ByVal dataSource As Object, ByVal context As BindingContext)
			Me.New()
			StartPosition = FormStartPosition.Manual
			Me.Location = location
			BindingContext = context
			allowTrackValueChanges = False
			Me.dataSource = dataSource
			gridColumns = columns
			PopulateColumnLists()
			For j As Integer = 0 To colNum - 1
				CreateCol(j, colNum, simpleColumns, j, SimpleEditorHeight)
			Next j
			If multilineColumns.Count > 0 Then
				CreateCol(0, 1, multilineColumns, 4, MultilineEditorHeight)
			End If
			Width = xLocation
		End Sub

		Private Sub CreateEditor(ByVal column As GridColumn, ByVal name As String)
			Dim edit As BaseEdit
			If column.ColumnEdit Is Nothing Then
				edit = New TextEdit()
			Else
				edit = column.ColumnEdit.CreateEditor()
			End If
			edit.Name = name
			edit.Width = EditorWidth
			edit.Location = New Point(xLocation, yLocation)
			edit.DataBindings.Add("EditValue", dataSource, column.FieldName)
			AddHandler edit.EditValueChanging, AddressOf OnEditorEditValueChanging
			Controls.Add(edit)
		End Sub

		Private Sub CreateLabel(ByVal name As String, ByVal caption As String)
			Dim label As New LabelControl()
			label.Name = name
			label.Text = caption
			label.Location = New Point(xLocation, yLocation)
			maxWidth = Math.Max(maxWidth, label.Width)
			Controls.Add(label)
		End Sub

		Private Sub CreateCol(ByVal startIndex As Integer, ByVal [step] As Integer, ByVal columns As List(Of String), ByVal col As Integer, ByVal height As Integer)
			For i As Integer = startIndex To columns.Count - 1 Step [step]
				CreateLabel(String.Format("label_{1}_{0}", i, col), gridColumns(columns(i)).Caption)
				yLocation += height + Indent
			Next i
			xLocation += maxWidth + Indent
			yLocation = Indent
			For i As Integer = startIndex To columns.Count - 1 Step [step]
				CreateEditor(gridColumns(columns(i)), String.Format("edit_{1}_{0}", i, col))
				yLocation += height + Indent
			Next i
			Me.Height = Math.Max(Me.Height, yLocation)
			xLocation += EditorWidth + Indent
			yLocation = Indent
			maxWidth = 0
		End Sub

		Private Sub PopulateColumnLists()
			For Each column As GridColumn In gridColumns
				If column.Visible = False OrElse column.GroupIndex >= 0 Then
					Continue For
				End If
				If TypeOf column.ColumnEdit Is RepositoryItemMemoEdit OrElse TypeOf column.ColumnEdit Is RepositoryItemPictureEdit OrElse TypeOf column.ColumnEdit Is RepositoryItemImageEdit Then
					multilineColumns.Add(column.FieldName)
				Else
					simpleColumns.Add(column.FieldName)
				End If
			Next column
			If simpleColumns.Count > 4 Then
				If simpleColumns.Count > 6 AndAlso multilineColumns.Count = 0 Then
					colNum = 3
				Else
					colNum = 2
				End If
			End If
		End Sub

		Private Sub OnEditorEditValueChanging(ByVal sender As Object, ByVal e As ChangingEventArgs)
			If (Not allowTrackValueChanges) Then
				Return
			End If
			Dim edit As BaseEdit = CType(sender, BaseEdit)
			If (Not oldValues.ContainsKey(edit.Name)) Then
				oldValues.Add(edit.Name, e.OldValue)
			End If
		End Sub

		Private Sub OnApplyClick(ByVal sender As Object, ByVal e As EventArgs) Handles simpleButton1.Click
			Close()
		End Sub

		Private Sub OnCancelClick(ByVal sender As Object, ByVal e As EventArgs) Handles simpleButton2.Click
			For Each kvp As KeyValuePair(Of String, Object) In oldValues
				CType(Controls(kvp.Key), BaseEdit).EditValue = kvp.Value
			Next kvp
			Close()
		End Sub

		Private Overloads Sub OnShown(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Shown
			allowTrackValueChanges = True
		End Sub
	End Class
End Namespace