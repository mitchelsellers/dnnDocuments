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
  Public Class DocumentsSortColumnInfo

    Private _ColumnName As String
    Private _LocalizedColumnName As String
    Private _Direction As SortDirection = SortDirection.Ascending

    Enum SortDirection As Integer
      Ascending
      Descending
    End Enum

    Public Property Direction() As SortDirection
      Get
        Return _Direction
      End Get
      Set(ByVal value As SortDirection)
        _Direction = value
      End Set
    End Property

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
  End Class
End Namespace