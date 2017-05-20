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
Imports DotNetNuke

Namespace DotNetNuke.Modules.Documents

  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' The DataProvider Class Is an abstract class that provides the DataLayer
  ''' for the Documents Module.
  ''' </summary>
  ''' <remarks>
  ''' </remarks>
  ''' <history>
  ''' 	[cnurse]	9/22/2004	Moved Documents to a separate Project
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public MustInherit Class DataProvider

#Region "Shared/Static Methods"

        ' singleton reference to the instantiated object 
        Private Shared objProvider As DataProvider = Nothing

        ' constructor
        Shared Sub New()
            CreateProvider()
        End Sub

        ' dynamically create provider
        Private Shared Sub CreateProvider()
            objProvider = CType(Framework.Reflection.CreateObject("data", "DotNetNuke.Modules.Documents", ""), DataProvider)
        End Sub

        ' return the provider
        Public Shared Shadows Function Instance() As DataProvider
            Return objProvider
        End Function

#End Region

#Region "Abstract methods"

        Public MustOverride Function AddDocument(ByVal ModuleId As Integer, ByVal Title As String, ByVal URL As String, ByVal UserId As Integer, ByVal OwnedByUserID As Integer, ByVal Category As String, ByVal SortOrderIndex As Integer, ByVal Description As String, ByVal ForceDownload As Boolean) As Integer
        Public MustOverride Sub DeleteDocument(ByVal ModuleId As Integer, ByVal ItemID As Integer)
    Public MustOverride Function GetDocument(ByVal ItemId As Integer, ByVal ModuleId As Integer) As IDataReader
    Public MustOverride Function GetDocuments(ByVal ModuleId As Integer, ByVal PortalId As Integer) As IDataReader
        Public MustOverride Sub UpdateDocument(ByVal ModuleId As Integer, ByVal ItemId As Integer, ByVal Title As String, ByVal URL As String, ByVal UserId As Integer, ByVal OwnedByUserID As Integer, ByVal Category As String, ByVal SortOrderIndex As Integer, ByVal Description As String, ByVal ForceDownload As Boolean)

    Public MustOverride Function AddDocumentsSettings(ByVal ModuleId As Integer, ByVal ShowTitleLink As Boolean, ByVal SortOrder As String, ByVal DisplayColumns As String, ByVal UseCategoriesList As Boolean, ByVal DefaultFolder As String, ByVal CategoriesListName As String, ByVal AllowUserSort As Boolean) As Integer
    Public MustOverride Sub DeleteDocumentsSettings(ByVal ModuleID As Integer)
    Public MustOverride Function GetDocumentsSettings(ByVal ModuleId As Integer) As IDataReader
    Public MustOverride Sub UpdateDocumentsSettings(ByVal ModuleId As Integer, ByVal ShowTitleLink As Boolean, ByVal SortOrder As String, ByVal DisplayColumns As String, ByVal UseCategoriesList As Boolean, ByVal DefaultFolder As String, ByVal CategoriesListName As String, ByVal AllowUserSort As Boolean)

#End Region

  End Class

End Namespace