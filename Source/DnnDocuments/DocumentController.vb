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
Imports DotNetNuke.Services.Search
Imports System
Imports System.Configuration
Imports System.Data
Imports DotNetNuke
Imports System.XML
Imports DotNetNuke.Common.Utilities.XmlUtils

Namespace DotNetNuke.Modules.Documents

  ''' -----------------------------------------------------------------------------
  ''' Namespace:  DotNetNuke.Modules.Documents
  ''' Project:    DotNetNuke
  ''' Class:      DocumentController
  ''' -----------------------------------------------------------------------------
  ''' <summary>
  ''' The DocumentController Class represents the Documents Business Layer
  ''' Methods in this class call methods in the Data Layer
  ''' </summary>
  ''' <remarks>
  ''' </remarks>
  ''' <history>
  ''' 	[cnurse]	9/22/2004	Moved Documents to a separate Project
  ''' </history>
  ''' -----------------------------------------------------------------------------
  Public Class DocumentController
    Implements Entities.Modules.ISearchable
    Implements Entities.Modules.IPortable

#Region "Public Methods"

    Public Sub AddDocument(ByVal objDocument As DocumentInfo)

            DataProvider.Instance().AddDocument(objDocument.ModuleId, objDocument.Title, objDocument.Url, objDocument.CreatedByUserId, objDocument.OwnedByUserId, objDocument.Category, objDocument.SortOrderIndex, objDocument.Description, objDocument.ForceDownload)

    End Sub

        Public Sub DeleteDocument(ByVal ModuleId As Integer, ByVal ItemID As Integer)

            DataProvider.Instance().DeleteDocument(ModuleId, ItemID)

        End Sub

    Public Function GetDocument(ByVal ItemId As Integer, ByVal ModuleId As Integer) As DocumentInfo

      Return CType(CBO.FillObject(DataProvider.Instance().GetDocument(ItemId, ModuleId), GetType(DocumentInfo)), DocumentInfo)

    End Function

    Public Function GetDocuments(ByVal ModuleId As Integer, ByVal PortalId As Integer) As ArrayList

      Return CBO.FillCollection(DataProvider.Instance().GetDocuments(ModuleId, PortalId), GetType(DocumentInfo))

    End Function

    Public Sub UpdateDocument(ByVal objDocument As DocumentInfo)
            DataProvider.Instance().UpdateDocument(objDocument.ModuleId, objDocument.ItemId, objDocument.Title, objDocument.Url, objDocument.CreatedByUserId, objDocument.OwnedByUserId, objDocument.Category, objDocument.SortOrderIndex, objDocument.Description, objDocument.ForceDownload)
    End Sub

    Public Sub AddDocumentsSettings(ByVal objDocumentsSettings As DocumentsSettingsInfo)
      DataProvider.Instance().AddDocumentsSettings(objDocumentsSettings.ModuleId, _
      objDocumentsSettings.ShowTitleLink, _
      objDocumentsSettings.SortOrder, _
      objDocumentsSettings.DisplayColumns, _
      objDocumentsSettings.UseCategoriesList, _
      objDocumentsSettings.DefaultFolder, _
      objDocumentsSettings.CategoriesListName, _
      objDocumentsSettings.AllowUserSort)
    End Sub

    Public Sub DeleteDocumentsSettings(ByVal ModuleID As Integer)
      DataProvider.Instance().DeleteDocumentsSettings(ModuleID)
    End Sub

    Public Function GetDocumentsSettings(ByVal ModuleId As Integer) As DocumentsSettingsInfo
      Return CType(CBO.FillObject(DataProvider.Instance().GetDocumentsSettings(ModuleId), GetType(DocumentsSettingsInfo)), DocumentsSettingsInfo)
    End Function

    Public Sub UpdateDocumentsSettings(ByVal objDocumentsSettings As DocumentsSettingsInfo)
      DataProvider.Instance().UpdateDocumentsSettings(objDocumentsSettings.ModuleId, objDocumentsSettings.ShowTitleLink, objDocumentsSettings.SortOrder, objDocumentsSettings.DisplayColumns, objDocumentsSettings.UseCategoriesList, objDocumentsSettings.DefaultFolder, objDocumentsSettings.CategoriesListName, objDocumentsSettings.AllowUserSort)
    End Sub

#End Region

#Region "Optional Interfaces"

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' GetSearchItems implements the ISearchable Interface
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <param name="ModInfo">The ModuleInfo for the module to be Indexed</param>
    ''' <history>
    '''		[cnurse]	    17 Nov 2004	documented
    '''   [aglenwright] 18 Feb 2006 Altered to accomodate change to CreatedByUser
    '''                             field (changed from string to integer)
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function GetSearchItems(ByVal ModInfo As Entities.Modules.ModuleInfo) As Services.Search.SearchItemInfoCollection Implements Entities.Modules.ISearchable.GetSearchItems
      Dim SearchItemCollection As New SearchItemInfoCollection
      Dim Documents As ArrayList = GetDocuments(ModInfo.ModuleID, ModInfo.PortalID)

      ' TODO: Add new fields

      Dim objDocument As Object
      For Each objDocument In Documents
        Dim SearchItem As SearchItemInfo
        With CType(objDocument, DocumentInfo)
          Dim UserId As Integer = Null.NullInteger
          'If IsNumeric(.CreatedByUser) Then
          '    UserId = Integer.Parse(.CreatedByUser)
          'End If
          UserId = .CreatedByUserID
                    SearchItem = New SearchItemInfo(ModInfo.ModuleTitle & " - " & .Title, .Title, UserId, .CreatedDate, ModInfo.ModuleID, .ItemId.ToString, .Title & " " & .Category & " " & .Description, "ItemId=" & .ItemId.ToString)
          SearchItemCollection.Add(SearchItem)
        End With
      Next

      Return SearchItemCollection
    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' ExportModule implements the IPortable ExportModule Interface
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <param name="ModuleID">The Id of the module to be exported</param>
    ''' <history>
    '''		[cnurse]	    17 Nov 2004	documented
    '''		[aglenwright]	18 Feb 2006	Added new fields: Createddate, description, 
    '''                             modifiedbyuser, modifieddate, OwnedbyUser, SortorderIndex
    '''                             Added DocumentsSettings
    '''   [togrean]     10 Jul 2007 Fixed issues with importing documet settings since new fileds 
    '''                             were added: AllowSorting, default folder, list name
    '''   [togrean]     13 Jul 2007 Added support for exporting documents Url tracking options  
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function ExportModule(ByVal ModuleID As Integer) As String Implements Entities.Modules.IPortable.ExportModule

      Dim objModules As New Entities.Modules.ModuleController
      Dim objModule As Entities.Modules.ModuleInfo = objModules.GetModule(ModuleID, Null.NullInteger)
      Dim objDocumentsSettings As DocumentsSettingsInfo = GetDocumentsSettings(ModuleID)

      Dim strXML As StringBuilder = New StringBuilder("<documents>")
      Try
        If Not objDocumentsSettings Is Nothing Then
          strXML.Append("<documentssettings>")
          strXML.AppendFormat("<displaycolumns>{0}</displaycolumns>", DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(objDocumentsSettings.DisplayColumns))
          strXML.AppendFormat("<showtitlelink>{0}</showtitlelink>", DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(objDocumentsSettings.ShowTitleLink.ToString))
          strXML.AppendFormat("<sortorder>{0}</sortorder>", DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(objDocumentsSettings.SortOrder))
          strXML.AppendFormat("<usecategorieslist>{0}</usecategorieslist>", DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(objDocumentsSettings.UseCategoriesList.ToString))
          strXML.AppendFormat("<allowusersort>{0}</allowusersort>", DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(objDocumentsSettings.AllowUserSort.ToString()))
          strXML.AppendFormat("<defaultfolder>{0}</defaultfolder>", DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(objDocumentsSettings.DefaultFolder.ToString()))
          strXML.AppendFormat("<categorieslistname>{0}</categorieslistname>", DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(objDocumentsSettings.CategoriesListName.ToString()))
          strXML.Append("</documentssettings>")
        End If

        Dim arrDocuments As ArrayList = GetDocuments(ModuleID, objModule.PortalID)
        If arrDocuments.Count <> 0 Then
          Dim objDocument As DocumentInfo
          For Each objDocument In arrDocuments
            strXML.Append("<document>")
            strXML.AppendFormat("<title>{0}</title>", XMLEncode(objDocument.Title))
            strXML.AppendFormat("<url>{0}</url>", XMLEncode(objDocument.Url))
            strXML.AppendFormat("<category>{0}</category>", XMLEncode(objDocument.Category))

            strXML.AppendFormat("<createddate>{0}</createddate>", XMLEncode(objDocument.CreatedDate.ToString("dd-MMM-yyyy hh:mm:ss tt")))
            strXML.AppendFormat("<description>{0}</description>", XMLEncode(objDocument.Description))
                        strXML.AppendFormat("<createdbyuserid>{0}</createdbyuserid>", XMLEncode(objDocument.CreatedByUserId.ToString))
                        strXML.AppendFormat("<forcedownload>{0}</forcedownload>", XMLEncode((objDocument.ForceDownload.ToString())))

            strXML.AppendFormat("<ownedbyuserid>{0}</ownedbyuserid>", XMLEncode(objDocument.OwnedByUserID.ToString()))
            strXML.AppendFormat("<modifiedbyuserid>{0}</modifiedbyuserid>", XMLEncode(objDocument.ModifiedByUserID.ToString))
            strXML.AppendFormat("<modifieddate>{0}</modifieddate>", XMLEncode(objDocument.ModifiedDate.ToString("dd-MMM-yyyy hh:mm:ss tt")))
            strXML.AppendFormat("<sortorderindex>{0}</sortorderindex>", XMLEncode(objDocument.SortOrderIndex.ToString))

            ' Export Url Tracking options too
            Dim objUrlController As New UrlController()
            Dim objUrlTrackingInfo As UrlTrackingInfo = objUrlController.GetUrlTracking(objModule.PortalID, objDocument.Url, ModuleID)

            If Not objUrlTrackingInfo Is Nothing Then
              strXML.AppendFormat("<logactivity>{0}</logactivity>", XMLEncode(objUrlTrackingInfo.LogActivity.ToString()))
              strXML.AppendFormat("<trackclicks>{0}</trackclicks>", XMLEncode(objUrlTrackingInfo.TrackClicks.ToString()))
              strXML.AppendFormat("<newwindow>{0}</newwindow>", XMLEncode(objUrlTrackingInfo.NewWindow.ToString()))
            End If
            strXML.Append("</document>")
          Next

        End If
      Catch
        ' Catch errors but make sure XML is valid
      Finally
        strXML.Append("</documents>")
      End Try

      Return strXML.ToString()

    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' ImportModule implements the IPortable ImportModule Interface
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <param name="ModuleID">The Id of the module to be imported</param>
    ''' <history>
    '''		[cnurse]	    17 Nov 2004	documented
    '''		[aglenwright]	18 Feb 2006	Added new fields: Createddate, description, 
    '''                             modifiedbyuser, modifieddate, OwnedbyUser, SortorderIndex
    '''                             Added DocumentsSettings
    '''   [togrean]     10 Jul 2007 Fixed issues with importing documet settings since new fileds 
    '''                             were added: AllowSorting, default folder, list name    
    '''   [togrean]     13 Jul 2007 Added support for importing documents Url tracking options     
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Sub ImportModule(ByVal ModuleID As Integer, ByVal Content As String, ByVal Version As String, ByVal UserId As Integer) Implements Entities.Modules.IPortable.ImportModule
        Dim isNewSettings As Boolean
        Dim xmlDocumentsSettings As XmlNode = GetContent(Content, "documents/documentssettings")
        If (Not xmlDocumentsSettings Is Nothing) Then
            ' Need to check before adding settings - update may be required instead
            Dim objDocumentsSettings As DocumentsSettingsInfo = GetDocumentsSettings(ModuleID)
            If (objDocumentsSettings Is Nothing) Then
              objDocumentsSettings = New DocumentsSettingsInfo()
              isNewSettings = True
            End If
            objDocumentsSettings.ModuleId = ModuleID
            objDocumentsSettings.DisplayColumns = GetNodeValue(xmlDocumentsSettings, "displaycolumns")
            objDocumentsSettings.ShowTitleLink = GetNodeValueBoolean(xmlDocumentsSettings, "showtitlelink")
            objDocumentsSettings.SortOrder = GetNodeValue(xmlDocumentsSettings, "sortorder")
            objDocumentsSettings.UseCategoriesList = GetNodeValueBoolean(xmlDocumentsSettings, "usecategorieslist")
            objDocumentsSettings.AllowUserSort = GetNodeValueBoolean(xmlDocumentsSettings, "allowusersort")
            objDocumentsSettings.DefaultFolder = GetNodeValue(xmlDocumentsSettings, "defaultfolder")
            objDocumentsSettings.CategoriesListName = GetNodeValue(xmlDocumentsSettings, "categorieslistname")
            If isNewSettings Then
              AddDocumentsSettings(objDocumentsSettings)
            Else
              UpdateDocumentsSettings(objDocumentsSettings)
            End If

        End If

      Dim xmlDocument As XmlNode
      Dim strUrl As String = String.Empty
      Dim xmlDocuments As XmlNode = GetContent(Content, "documents")
      Dim documentNodes As XmlNodeList = xmlDocuments.SelectNodes("document")
      For Each xmlDocument In documentNodes
        Dim objDocument As New DocumentInfo
        objDocument.ModuleId = ModuleID
        objDocument.Title = xmlDocument.Item("title").InnerText
        strUrl = xmlDocument.Item("url").InnerText
        If (strUrl.ToLower().StartsWith("fileid=")) Then
            objDocument.Url = strUrl
        Else
            objDocument.Url = ImportUrl(ModuleID, strUrl)
        End If

        objDocument.Category = xmlDocument.Item("category").InnerText
        objDocument.CreatedDate = GetNodeValueDate(xmlDocument, "createddate", Now)
        objDocument.Description = GetNodeValue(xmlDocument, "description")
        objDocument.CreatedByUserID = UserId

        objDocument.OwnedByUserID = GetNodeValueInt(xmlDocument, "ownedbyuserid")
        objDocument.ModifiedByUserID = GetNodeValueInt(xmlDocument, "modifiedbyuserid")
        objDocument.ModifiedDate = GetNodeValueDate(xmlDocument, "modifieddate", Now)
                objDocument.SortOrderIndex = GetNodeValueInt(xmlDocument, "sortorderindex")
                objDocument.ForceDownload = GetNodeValueBoolean(xmlDocument, "forcedownload")

        AddDocument(objDocument)

      ' Update Tracking options
        Dim objModules As New Entities.Modules.ModuleController
        Dim objModule As Entities.Modules.ModuleInfo = objModules.GetModule(ModuleID, Null.NullInteger)
        Dim urlType As String = "U"
        If objDocument.Url.StartsWith("FileID") Then
          urlType = "F"
        End If
        Dim urlController As New UrlController()
        ' If nodes not found, all values will be false
        urlController.UpdateUrl(objModule.PortalID, objDocument.Url, urlType, GetNodeValueBoolean(xmlDocument, "logactivity"), GetNodeValueBoolean(xmlDocument, "trackclicks", True), ModuleID, GetNodeValueBoolean(xmlDocument, "newwindow"))

      Next
    End Sub

#End Region

  End Class

End Namespace
