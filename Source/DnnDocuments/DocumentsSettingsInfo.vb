'
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2002-2011
' by DotNetNuke Corporation
'
' Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
' documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
' the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
' to permit persons to whom the Software is furnished to do so, subject to the following conditions:
'
' The above copyright notice and this permission notice shall be included in all copies or substantial portions 
' of the Software.
'
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
' DEALINGS IN THE SOFTWARE.
'

Imports System
Imports System.Configuration
Imports System.Data
Imports DotNetNuke


Namespace DotNetNuke.Modules.Documents

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' The DocumentSettings Class provides the Documents Settings Object
  ''' </summary>
  ''' <remarks>
  ''' </remarks>
  ''' <history>
  ''' 	[aglenwright]	18 Feb 2006	Created
  ''' </history>
  ''' -----------------------------------------------------------------------------

  Public Class DocumentsSettingsInfo

    Sub New()

    End Sub

    Sub New(ByVal LocalResourceFile As String)
      _LocalResourceFile = _LocalResourceFile
    End Sub

#Region "Private Members"
    Private _LocalResourceFile As String
    Private _ModuleId As Integer
    Private _ShowTitleLink As Boolean = True
    Private _SortOrder As String
    Private _DisplayColumns As String = _
      DocumentsDisplayColumnInfo.COLUMN_TITLE & ";true," & _
      DocumentsDisplayColumnInfo.COLUMN_OWNEDBY & ";true," & _
      DocumentsDisplayColumnInfo.COLUMN_CATEGORY & ";true," & _
      DocumentsDisplayColumnInfo.COLUMN_MODIFIEDDATE & ";true," & _
      DocumentsDisplayColumnInfo.COLUMN_SIZE & ";true," & _
      DocumentsDisplayColumnInfo.COLUMN_DOWNLOADLINK & ";true"

    Private _UseCategoriesList As Boolean = False
    Private _DefaultFolder As String = ""
    Private _CategoriesListName As String = "Document Categories"
    Private _AllowUserSort As Boolean

#End Region

#Region "Properties"
    Public Property LocalResourceFile() As String
      Get
        Return _LocalResourceFile
      End Get
      Set(ByVal Value As String)
        _LocalResourceFile = Value
      End Set
    End Property

    Public Property ModuleId() As Integer
      Get
        Return _ModuleId
      End Get
      Set(ByVal Value As Integer)
        _ModuleId = Value
      End Set
    End Property

    Public Property ShowTitleLink() As Boolean
      Get
        Return _ShowTitleLink
      End Get
      Set(ByVal Value As Boolean)
        _ShowTitleLink = Value
      End Set
    End Property

    Public Property SortOrder() As String
      Get
        Return _SortOrder
      End Get
      Set(ByVal Value As String)
        _SortOrder = Value
      End Set
    End Property

    Public Property DisplayColumns() As String
      Get
        Return _DisplayColumns
      End Get
      Set(ByVal Value As String)
        _DisplayColumns = Value
      End Set
    End Property

    Public Property UseCategoriesList() As Boolean
      Get
        Return _UseCategoriesList
      End Get
      Set(ByVal Value As Boolean)
        _UseCategoriesList = Value
      End Set
    End Property

    Public Property AllowUserSort() As Boolean
      Get
        Return _AllowUserSort
      End Get
      Set(ByVal value As Boolean)
        _AllowUserSort = value
      End Set
    End Property

    Public Property DefaultFolder() As String
      Get
        Return _DefaultFolder
      End Get
      Set(ByVal value As String)
        _DefaultFolder = value
      End Set
    End Property

    Public Property CategoriesListName() As String
      Get
        Return _CategoriesListName
      End Get
      Set(ByVal value As String)
        _CategoriesListName = value
      End Set
    End Property

    Public ReadOnly Property DisplayColumnList() As ArrayList
      Get
        Dim strColumnData As String
        Dim objColumnInfo As DocumentsDisplayColumnInfo
        Dim objColumnSettings As New ArrayList

        If Me.DisplayColumns <> String.Empty Then
          ' read "saved" column sort orders in first
          For Each strColumnData In Me.DisplayColumns.Split(Char.Parse(","))
            objColumnInfo = New DocumentsDisplayColumnInfo
            objColumnInfo.ColumnName = strColumnData.Split(Char.Parse(";"))(0)
            objColumnInfo.DisplayOrder = objColumnSettings.Count + 1
            objColumnInfo.Visible = Boolean.Parse(strColumnData.Split(Char.Parse(";"))(1))
            objColumnInfo.LocalizedColumnName = Services.Localization.Localization.GetString(objColumnInfo.ColumnName & ".Header", _LocalResourceFile)

            objColumnSettings.Add(objColumnInfo)
          Next
        End If

        Return objColumnSettings
      End Get
    End Property

    Public ReadOnly Property SortColumnList() As ArrayList
      Get
        Dim objSortColumn As DocumentsSortColumnInfo
        Dim strSortColumn As String
        Dim objSortColumns As New ArrayList

        If Me.SortOrder <> String.Empty Then
          For Each strSortColumn In Me.SortOrder.Split(Char.Parse(","))
            objSortColumn = New DocumentsSortColumnInfo
            If Left(strSortColumn, 1) = "-" Then
              objSortColumn.Direction = DocumentsSortColumnInfo.SortDirection.Descending
              objSortColumn.ColumnName = strSortColumn.Substring(1)
            Else
              objSortColumn.Direction = DocumentsSortColumnInfo.SortDirection.Ascending
              objSortColumn.ColumnName = strSortColumn
            End If

            objSortColumn.LocalizedColumnName = Services.Localization.Localization.GetString(objSortColumn.ColumnName & ".Header", _LocalResourceFile)

            objSortColumns.Add(objSortColumn)
          Next
        End If

        Return objSortColumns
      End Get
    End Property

    Public Shared Function FindColumn(ByVal ColumnName As String, ByVal List As ArrayList, ByVal VisibleOnly As Boolean) As Integer
      ' Find a display column in the list and return it's index 
      Dim intIndex As Integer

      For intIndex = 0 To List.Count - 1
        With CType(List(intIndex), DocumentsDisplayColumnInfo)
          If .ColumnName = ColumnName AndAlso (Not VisibleOnly OrElse .Visible) Then
            Return intIndex
          End If
        End With
      Next

      Return -1
    End Function

    Public Shared Function FindGridColumn(ByVal ColumnName As String, ByVal List As ArrayList, ByVal VisibleOnly As Boolean) As Integer
      ' Find a display column in the list and return it's "column" index 
      ' as it will be displayed within the grid.  This function differs from FindColumn
      ' in that it "ignores" invisible columns when counting which column index to 
      ' return.
      Dim intIndex As Integer
      Dim intResult As Integer

      For intIndex = 0 To List.Count - 1
        With CType(List(intIndex), DocumentsDisplayColumnInfo)
          If .ColumnName = ColumnName AndAlso (Not VisibleOnly OrElse .Visible) Then
            Return intResult
          End If
          If .Visible Then
            intResult = intResult + 1
          End If
        End With
      Next

      Return -1
    End Function
#End Region

  End Class
End Namespace