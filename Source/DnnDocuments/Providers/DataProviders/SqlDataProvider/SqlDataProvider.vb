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
Imports System.Data
Imports System.Data.SqlClient
Imports Microsoft.ApplicationBlocks.Data

Namespace DotNetNuke.Modules.Documents

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The SqlDataProvider Class is an SQL Server implementation of the DataProvider Abstract
    ''' class that provides the DataLayer for the Documents Module.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	9/22/2004	Moved Documents to a separate Project
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class SqlDataProvider

        Inherits DataProvider

#Region "Private Members"

        Private Const ProviderType As String = "data"

        Private _providerConfiguration As Framework.Providers.ProviderConfiguration = Framework.Providers.ProviderConfiguration.GetProviderConfiguration(ProviderType)
        Private _connectionString As String
        Private _providerPath As String
        Private _objectQualifier As String
        Private _databaseOwner As String

#End Region

#Region "Constructors"

        Public Sub New()
            ' Read the configuration specific information for this provider
            Dim objProvider As Framework.Providers.Provider = CType(_providerConfiguration.Providers(_providerConfiguration.DefaultProvider), Framework.Providers.Provider)

            ' This code handles getting the connection string from either the connectionString / appsetting section and uses the connectionstring section by default if it exists.  
            ' Get Connection string from web.config
            _connectionString = DotNetNuke.Common.Utilities.Config.GetConnectionString()

            ' If above funtion does not return anything then connectionString must be set in the dataprovider section.
            If _connectionString = String.Empty Then
                ' Use connection string specified in provider
                _connectionString = objProvider.Attributes("connectionString")
            End If

            _providerPath = objProvider.Attributes("providerPath")

            _objectQualifier = objProvider.Attributes("objectQualifier")
            If _objectQualifier <> String.Empty And _objectQualifier.EndsWith("_") = False Then
                _objectQualifier += "_"
            End If

            _databaseOwner = objProvider.Attributes("databaseOwner")
            If _databaseOwner <> "" And _databaseOwner.EndsWith(".") = False Then
                _databaseOwner += "."
            End If

        End Sub

#End Region

#Region "Properties"

        Public ReadOnly Property ConnectionString() As String
            Get
                Return _connectionString
            End Get
        End Property

        Public ReadOnly Property ProviderPath() As String
            Get
                Return _providerPath
            End Get
        End Property

        Public ReadOnly Property ObjectQualifier() As String
            Get
                Return _objectQualifier
            End Get
        End Property

        Public ReadOnly Property DatabaseOwner() As String
            Get
                Return _databaseOwner
            End Get
        End Property

#End Region

#Region "Public Methods"

        Private Function GetNull(ByVal Field As Object) As Object
            Return Common.Utilities.Null.GetNull(Field, DBNull.Value)
        End Function

        Public Overloads Overrides Function AddDocument(ByVal ModuleId As Integer, ByVal Title As String, ByVal URL As String, ByVal UserId As Integer, ByVal OwnedByUserID As Integer, ByVal Category As String, ByVal SortOrderIndex As Integer, ByVal Description As String, ForceDownload As Boolean) As Integer
            Return CType(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & "AddDocument", ModuleId, Title, URL, UserId, OwnedByUserID, Category, SortOrderIndex, Description, ForceDownload), Integer)
        End Function

        Public Overrides Sub DeleteDocument(ByVal ModuleId As Integer, ByVal ItemId As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "DeleteDocument", ModuleId, ItemId)
        End Sub

        Public Overrides Function GetDocument(ByVal ItemId As Integer, ByVal ModuleId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "GetDocument", ItemId, ModuleId), IDataReader)
        End Function

        Public Overrides Function GetDocuments(ByVal ModuleId As Integer, ByVal PortalId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "GetDocuments", ModuleId, PortalId), IDataReader)
        End Function

        Public Overloads Overrides Sub UpdateDocument(ByVal moduleId As Integer, ByVal ItemId As Integer, ByVal Title As String, ByVal URL As String, ByVal UserId As Integer, ByVal OwnedByUserID As Integer, ByVal Category As String, ByVal SortOrderIndex As Integer, ByVal Description As String, ForceDownload As Boolean)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "UpdateDocument", moduleId, ItemId, Title, URL, UserId, Category, OwnedByUserID, SortOrderIndex, Description, ForceDownload)
        End Sub

        Public Overrides Function AddDocumentsSettings(ByVal ModuleId As Integer, ByVal ShowTitleLink As Boolean, ByVal SortOrder As String, ByVal DisplayColumns As String, ByVal UseCategoriesList As Boolean, ByVal DefaultFolder As String, ByVal CategoriesListName As String, ByVal AllowUserSort As Boolean) As Integer
            SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & "AddDocumentsSettings", ModuleId, ShowTitleLink, SortOrder, DisplayColumns, UseCategoriesList, DefaultFolder, CategoriesListName, AllowUserSort)
        End Function

        Public Overrides Sub DeleteDocumentsSettings(ByVal ModuleID As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "DeleteDocumentsSettings", ModuleID)
        End Sub

        Public Overrides Function GetDocumentsSettings(ByVal ModuleId As Integer) As System.Data.IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "GetDocumentsSettings", ModuleId), IDataReader)
        End Function

        Public Overrides Sub UpdateDocumentsSettings(ByVal ModuleId As Integer, ByVal ShowTitleLink As Boolean, ByVal SortOrder As String, ByVal DisplayColumns As String, ByVal UseCategoriesList As Boolean, ByVal DefaultFolder As String, ByVal CategoriesListName As String, ByVal AllowUserSort As Boolean)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "UpdateDocumentsSettings", ModuleId, ShowTitleLink, SortOrder, DisplayColumns, UseCategoriesList, DefaultFolder, CategoriesListName, AllowUserSort)
        End Sub

#End Region

    End Class

End Namespace