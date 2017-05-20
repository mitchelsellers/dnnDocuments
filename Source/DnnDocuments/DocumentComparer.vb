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

Namespace DotNetNuke.Modules.Documents
  Public Class DocumentComparer
    Implements IComparer

    Private mobjSortColumns As ArrayList

    Sub New(ByVal SortColumns As ArrayList)
      mobjSortColumns = SortColumns
    End Sub

    Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer Implements System.Collections.IComparer.Compare
      If mobjSortColumns.Count = 0 Then Return 0

      Return Compare(0, CType(x, DocumentInfo), CType(y, DocumentInfo))
    End Function

    Private Function Compare(ByVal SortColumnIndex As Integer, ByVal objX As DocumentInfo, ByVal objY As DocumentInfo) As Integer
      Dim objSortColumn As DocumentsSortColumnInfo
      Dim intResult As Integer

      If SortColumnIndex >= mobjSortColumns.Count Then
        Return 0
      End If

      objSortColumn = CType(mobjSortColumns(SortColumnIndex), DocumentsSortColumnInfo)

      If objSortColumn.Direction = DocumentsSortColumnInfo.SortDirection.Ascending Then
        intResult = CompareValues(objSortColumn.ColumnName, objX, objY)
      Else
        intResult = CompareValues(objSortColumn.ColumnName, objY, objX)
      End If

      ' Difference not found, sort by next sort column
      If intResult = 0 Then
        Return Compare(SortColumnIndex + 1, objX, objY)
      Else
        Return intResult
      End If
    End Function

    Private Function CompareValues(ByVal ColumnName As String, ByVal ObjX As DocumentInfo, ByVal ObjY As DocumentInfo) As Integer
      Select Case ColumnName
        Case DocumentsDisplayColumnInfo.COLUMN_SORTORDER
          If ObjX.SortOrderIndex.CompareTo(ObjY.SortOrderIndex) <> 0 Then
            Return ObjX.SortOrderIndex.CompareTo(ObjY.SortOrderIndex)
          End If
        Case DocumentsDisplayColumnInfo.COLUMN_CATEGORY
          If ObjX.Category.CompareTo(ObjY.Category) <> 0 Then
            Return ObjX.Category.CompareTo(ObjY.Category)
          End If
        Case DocumentsDisplayColumnInfo.COLUMN_CREATEDBY
          If ObjX.CreatedByUser.CompareTo(ObjY.CreatedByUser) <> 0 Then
            Return ObjX.CreatedByUser.CompareTo(ObjY.CreatedByUser)
          End If
        Case DocumentsDisplayColumnInfo.COLUMN_CREATEDDATE
          If ObjX.CreatedDate.CompareTo(ObjY.CreatedDate) <> 0 Then
            Return ObjX.CreatedDate.CompareTo(ObjY.CreatedDate)
          End If
        Case DocumentsDisplayColumnInfo.COLUMN_DESCRIPTION
          If ObjX.Description.CompareTo(ObjY.Description) <> 0 Then
            Return ObjX.Description.CompareTo(ObjY.Description)
          End If
        Case DocumentsDisplayColumnInfo.COLUMN_MODIFIEDBY
          If ObjX.ModifiedByUser.CompareTo(ObjY.ModifiedByUser) <> 0 Then
            Return ObjX.ModifiedByUser.CompareTo(ObjY.ModifiedByUser)
          End If
        Case DocumentsDisplayColumnInfo.COLUMN_MODIFIEDDATE
          If ObjX.ModifiedDate.CompareTo(ObjY.ModifiedDate) <> 0 Then
            Return ObjX.ModifiedDate.CompareTo(ObjY.ModifiedDate)
          End If
        Case DocumentsDisplayColumnInfo.COLUMN_OWNEDBY
          If ObjX.OwnedByUser.CompareTo(ObjY.OwnedByUser) <> 0 Then
            Return ObjX.OwnedByUser.CompareTo(ObjY.OwnedByUser)
          End If
        Case DocumentsDisplayColumnInfo.COLUMN_SIZE
          If ObjX.Size.CompareTo(ObjY.Size) <> 0 Then
            Return ObjX.Size.CompareTo(ObjY.Size)
          End If
        Case DocumentsDisplayColumnInfo.COLUMN_TITLE
          If ObjX.Title.CompareTo(ObjY.Title) <> 0 Then
            Return ObjX.Title.CompareTo(ObjY.Title)
          End If
        Case DocumentsDisplayColumnInfo.COLUMN_CLICKS
          If ObjX.Clicks.CompareTo(ObjY.Clicks) <> 0 Then
            Return ObjX.Clicks.CompareTo(ObjY.Clicks)
          End If
      End Select

      Return 0
    End Function
  End Class
End Namespace