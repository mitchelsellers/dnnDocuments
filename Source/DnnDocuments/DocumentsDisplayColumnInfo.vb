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
  <Serializable()> _
  Public Class DocumentsDisplayColumnInfo
    Implements IComparable

    Public Const COLUMN_CREATEDBY As String = "CreatedBy"
    Public Const COLUMN_CREATEDDATE As String = "CreatedDate"
    Public Const COLUMN_TITLE As String = "Title"
    Public Const COLUMN_CATEGORY As String = "Category"
    Public Const COLUMN_OWNEDBY As String = "Owner"
    Public Const COLUMN_MODIFIEDBY As String = "ModifiedBy"
    Public Const COLUMN_MODIFIEDDATE As String = "ModifiedDate"
    Public Const COLUMN_SORTORDER As String = "SortIndex"
    Public Const COLUMN_DESCRIPTION As String = "Description"
    Public Const COLUMN_SIZE As String = "Size"
    Public Const COLUMN_DOWNLOADLINK As String = "DownloadLink"
    Public Const COLUMN_CLICKS As String = "Clicks"

    Public Shared AvailableDisplayColumns() As String = New String() _
    { _
      COLUMN_TITLE, _
      COLUMN_OWNEDBY, _
      COLUMN_CATEGORY, _
      COLUMN_MODIFIEDDATE, _
      COLUMN_SIZE, _
      COLUMN_DOWNLOADLINK, _
      COLUMN_CREATEDBY, _
      COLUMN_CREATEDDATE, _
      COLUMN_MODIFIEDBY, _
      COLUMN_DESCRIPTION, _
      COLUMN_CLICKS _
    }

    Public Shared AvailableSortColumns() As String = New String() _
    { _
      COLUMN_SORTORDER, _
      COLUMN_TITLE, _
      COLUMN_OWNEDBY, _
      COLUMN_CATEGORY, _
      COLUMN_MODIFIEDDATE, _
      COLUMN_SIZE, _
      COLUMN_CREATEDBY, _
      COLUMN_CREATEDDATE, _
      COLUMN_MODIFIEDBY, _
      COLUMN_DESCRIPTION, _
      COLUMN_CLICKS _
    }

#Region "Private Members"
    Private _ColumnName As String
    Private _DisplayOrder As Integer
    Private _Visible As Boolean
    Private _LocalizedColumnName As String
#End Region

#Region "Properties"
    Public Property ColumnName() As String
      Get
        Return _ColumnName
      End Get
      Set(ByVal Value As String)
        _ColumnName = Value
      End Set
    End Property

    Public Property LocalizedColumnName() As String
      Get
        Return _LocalizedColumnName
      End Get
      Set(ByVal Value As String)
        _LocalizedColumnName = Value
      End Set
    End Property

    Public Property DisplayOrder() As Integer
      Get
        Return _DisplayOrder
      End Get
      Set(ByVal Value As Integer)
        _DisplayOrder = Value
      End Set
    End Property

    Public Property Visible() As Boolean
      Get
        Return _Visible
      End Get
      Set(ByVal Value As Boolean)
        _Visible = Value
      End Set
    End Property
#End Region

#Region "ICompareable Interface"
    Public Function CompareTo(ByVal obj As Object) As Integer Implements System.IComparable.CompareTo
      Dim objYItem As DocumentsDisplayColumnInfo

      objYItem = CType(obj, DocumentsDisplayColumnInfo)
      Return Me.DisplayOrder.CompareTo(objYItem.DisplayOrder)
    End Function
#End Region


  End Class
End Namespace