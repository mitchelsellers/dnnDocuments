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
Imports DotNetNuke
Imports System.Web.UI.WebControls
Imports DotNetNuke.Services.FileSystem

Namespace DotNetNuke.Modules.Documents

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The Document Class provides the UI for displaying the Documents
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	9/22/2004	Moved Documents to a separate Project
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public MustInherit Class Document
        Inherits Entities.Modules.PortalModuleBase
        Implements Entities.Modules.IActionable

        Private Const NOT_READ As Integer = -2

        Private mobjSettings As DocumentsSettingsInfo
        Private mobjDocumentList As ArrayList

        Private mintTitleColumnIndex As Integer = NOT_READ
        Private mintDownloadLinkColumnIndex As Integer = NOT_READ
        Private mblnReadComplete As Boolean = False

#Region "    Controls    "

        Protected WithEvents grdDocuments As System.Web.UI.WebControls.DataGrid
        Protected WithEvents button1 As System.Web.UI.WebControls.Button

#End Region

#Region "    Event Handlers    "
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Page_Load runs when the control is loaded
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/22/2004	Moved Documents to a separate Project
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                mobjSettings = LoadSettings()
                grdDocuments.AllowSorting = mobjSettings.AllowUserSort
                LoadColumns()
                LoadData()
                grdDocuments.DataSource = mobjDocumentList
                grdDocuments.DataBind()

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' <summary>
        ''' Process user-initiated sort operation
        ''' </summary>
        ''' <param name="source"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        ''' <history>
        ''' 	[msellers]	5/17/2007	 Added
        ''' </history>
        Public Sub grdDocuments_SortCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridSortCommandEventArgs) Handles grdDocuments.SortCommand
            Dim objCustomSortList As New ArrayList
            Dim objCustomSortColumn As New DocumentsSortColumnInfo
            Dim objCustomSortDirecton As DocumentsSortColumnInfo.SortDirection = DocumentsSortColumnInfo.SortDirection.Ascending
            Dim strSortDirectionString As String = "ASC"


            ' Set the sort column name
            objCustomSortColumn.ColumnName = e.SortExpression

            ' Determine if we need to reverse the sort.  This is needed if an existing sort on the same column existed that was desc
            If ViewState("CurrentSortOrder") IsNot Nothing AndAlso ViewState("CurrentSortOrder").ToString <> String.Empty Then
                Dim existingSort As String = ViewState("CurrentSortOrder").ToString()
                If existingSort.StartsWith(e.SortExpression) AndAlso existingSort.EndsWith("ASC") Then
                    objCustomSortDirecton = DocumentsSortColumnInfo.SortDirection.Descending
                    strSortDirectionString = "DESC"
                End If
            End If

            ' Set the sort
            objCustomSortColumn.Direction = objCustomSortDirecton
            objCustomSortList.Add(objCustomSortColumn)

            mobjDocumentList.Sort(New DocumentComparer(objCustomSortList))
            grdDocuments.DataSource = mobjDocumentList
            grdDocuments.DataBind()

            ' Save the sort to viewstate
            ViewState("CurrentSortOrder") = e.SortExpression & " " & strSortDirectionString

            ' Mark as a user selected sort
            IsReadComplete = True
        End Sub

        ''' <summary>
        ''' If the datagrid was not sorted and bound via the "_Sort" method it will be bound at this time using
        ''' default values
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        ''' <history>
        '''   Mitchel Sellers 6/4/2007  Added method
        ''' </history>
        Private Sub Page_PreRender(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.PreRender
            ' Only bind if not a user selected sort

            If Not IsReadComplete Then
                LoadData()

                ' Use Documents IComparer to do sort based on the default sort order (mobjSettings.SortOrder)
                mobjDocumentList.Sort(New DocumentComparer(mobjSettings.SortColumnList))

                'Bind the grid
                grdDocuments.DataSource = mobjDocumentList
                grdDocuments.DataBind()
            End If

            ' Localize the Data Grid
            Localization.LocalizeDataGrid(grdDocuments, Me.LocalResourceFile)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' grdDocuments_ItemCreated runs when an item in the grid is created
        ''' </summary>
        ''' <remarks>
        ''' Set NavigateUrl for title, download links.  Also sets "scope" on 
        ''' header rows so that text-to-speech readers can interpret the header row.
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/22/2004	Moved Documents to a separate Project
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub grdDocuments_ItemCreated(ByVal sender As Object, ByVal e As DataGridItemEventArgs) Handles grdDocuments.ItemCreated
            Dim intCount As Integer
            Dim objDocument As DocumentInfo

            Try
                Select Case e.Item.ItemType
                    Case ListItemType.Header
                        ' Setting "scope" to "col" indicates to for text-to-speech
                        ' or braille readers that this row containes headings
                        For intCount = 1 To e.Item.Cells.Count - 1
                            e.Item.Cells(intCount).Attributes.Add("scope", "col")
                        Next

                    Case ListItemType.AlternatingItem, ListItemType.Item, ListItemType.SelectedItem
                        ' If ShowTitleLink is true, the title column is generated dynamically
                        ' as a template, which we can't data-bind, so we need to set the text
                        ' value here
                        objDocument = CType(mobjDocumentList(e.Item.ItemIndex), DocumentInfo)

                        If mobjSettings.ShowTitleLink Then
                            If mintTitleColumnIndex = NOT_READ Then
                                mintTitleColumnIndex = DocumentsSettingsInfo.FindGridColumn(DocumentsDisplayColumnInfo.COLUMN_TITLE, mobjSettings.DisplayColumnList, True)
                            End If

                            If mintTitleColumnIndex >= 0 Then
                                ' Dynamically set the title link URL
                                With CType(e.Item.Controls(mintTitleColumnIndex + 1).FindControl("ctlTitle"), Web.UI.WebControls.HyperLink)
                                    .Text = objDocument.Title
                                    ' Note: The title link should display inline if possible, so set
                                    ' ForceDownload=False
                                    .NavigateUrl = DotNetNuke.Common.Globals.LinkClick(objDocument.Url, TabId, ModuleId, objDocument.TrackClicks, objDocument.ForceDownload)
                                    If objDocument.NewWindow Then
                                        .Target = "_blank"
                                    End If
                                End With
                            End If
                        End If

                        ' If there's a "download" link, set the NavigateUrl 
                        If mintDownloadLinkColumnIndex = NOT_READ Then
                            mintDownloadLinkColumnIndex = DocumentsSettingsInfo.FindGridColumn(DocumentsDisplayColumnInfo.COLUMN_DOWNLOADLINK, mobjSettings.DisplayColumnList, True)
                        End If
                        If mintDownloadLinkColumnIndex >= 0 Then
                            With CType(e.Item.Controls(mintDownloadLinkColumnIndex).FindControl("ctlDownloadLink"), Web.UI.WebControls.HyperLink)
                                ' Note: The title link should display open/save dialog if possible, 
                                ' so set ForceDownload=True
                                .NavigateUrl = DotNetNuke.Common.Globals.LinkClick(objDocument.Url, TabId, ModuleId, objDocument.TrackClicks, objDocument.ForceDownload)
                                If objDocument.NewWindow Then
                                    .Target = "_blank"
                                End If
                            End With
                        End If
                End Select

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub


#End Region

#Region "    Optional Interfaces    "
        Public ReadOnly Property ModuleActions() As Entities.Modules.Actions.ModuleActionCollection Implements Entities.Modules.IActionable.ModuleActions
            Get
                Dim Actions As New Entities.Modules.Actions.ModuleActionCollection
                Actions.Add(GetNextActionID, Localization.GetString(Entities.Modules.Actions.ModuleActionType.AddContent, LocalResourceFile), Entities.Modules.Actions.ModuleActionType.AddContent, "", "", EditUrl(), False, Security.SecurityAccessLevel.Edit, True, False)
                Return Actions
            End Get
        End Property
#End Region

#Region "    Web Form Designer Generated Code    "

        'This call is required by the Web Form Designer.
        <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

        End Sub

        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
            'CODEGEN: This method call is required by the Web Form Designer
            'Do not modify it using the code editor.
            InitializeComponent()

            ' declare module actions
        End Sub
#End Region

#Region "    Private Methods    "
        Private Sub LoadColumns()
            Dim objDisplayColumn As DocumentsDisplayColumnInfo

            ' Add columns dynamically
            For Each objDisplayColumn In mobjSettings.DisplayColumnList
                If objDisplayColumn.Visible Then

                    Select Case objDisplayColumn.ColumnName
                        Case DocumentsDisplayColumnInfo.COLUMN_CATEGORY
                            AddDocumentColumn(Localization.GetString("Category", LocalResourceFile), "Category", "Category")

                        Case DocumentsDisplayColumnInfo.COLUMN_CREATEDBY
                            AddDocumentColumn(Localization.GetString("CreatedBy", LocalResourceFile), "CreatedBy", "CreatedByUser", "")

                        Case DocumentsDisplayColumnInfo.COLUMN_CREATEDDATE
                            AddDocumentColumn(Localization.GetString("CreatedDate", LocalResourceFile), "CreatedDate", "CreatedDate", "{0:d}")

                        Case DocumentsDisplayColumnInfo.COLUMN_DESCRIPTION
                            AddDocumentColumn(Localization.GetString("Description", LocalResourceFile), "Description", "Description")

                        Case DocumentsDisplayColumnInfo.COLUMN_DOWNLOADLINK
                            AddDownloadLink("", "ctlDownloadLink")

                        Case DocumentsDisplayColumnInfo.COLUMN_MODIFIEDBY
                            AddDocumentColumn(Localization.GetString("ModifiedBy", LocalResourceFile), "ModifiedBy", "ModifiedByUser")

                        Case DocumentsDisplayColumnInfo.COLUMN_MODIFIEDDATE
                            AddDocumentColumn(Localization.GetString("ModifiedDate", LocalResourceFile), "ModifiedDate", "ModifiedDate", "{0:d}")

                        Case DocumentsDisplayColumnInfo.COLUMN_OWNEDBY
                            AddDocumentColumn(Localization.GetString("Owner", LocalResourceFile), "Owner", "OwnedByUser")

                        Case DocumentsDisplayColumnInfo.COLUMN_SIZE
                            AddDocumentColumn(Localization.GetString("Size", LocalResourceFile), "Size", "FormatSize")

                        Case DocumentsDisplayColumnInfo.COLUMN_CLICKS
                            AddDocumentColumn(Localization.GetString("Clicks", LocalResourceFile), "Clicks", "Clicks")

                        Case DocumentsDisplayColumnInfo.COLUMN_TITLE
                            If mobjSettings.ShowTitleLink Then
                                AddDownloadLink(Localization.GetString("Title", LocalResourceFile), "ctlTitle")
                            Else
                                AddDocumentColumn(Localization.GetString("Title", LocalResourceFile), "Title", "Title")
                            End If
                    End Select
                End If
            Next
        End Sub

        Private Sub LoadData()
            Dim strCacheKey As String
            Dim objDocuments As DocumentController

            If IsReadComplete Then Exit Sub

            ' Only read from the cache if the users is not logged in
            strCacheKey = Me.CacheKey & ";anon-doclist"
            If Not Request.IsAuthenticated Then
                mobjDocumentList = CType(DataCache.GetCache(strCacheKey), ArrayList)
            End If

            If mobjDocumentList Is Nothing Then
                objDocuments = New DocumentController
                mobjDocumentList = objDocuments.GetDocuments(ModuleId, PortalId)

                ' Check security on files
                Dim intCount As Integer
                Dim objDocument As DocumentInfo

                For intCount = mobjDocumentList.Count - 1 To 0 Step -1
                    objDocument = CType(mobjDocumentList(intCount), DocumentInfo)
                    If objDocument.Url.ToLower.IndexOf("fileid=") >= 0 Then
                        ' document is a file, check security
                        Dim objFiles As New FileController
                        Dim objFile As DotNetNuke.Services.FileSystem.FileInfo = objFiles.GetFileById(Integer.Parse(objDocument.Url.Split(Char.Parse("="))(1)), PortalId)

                        If Not objFile Is Nothing AndAlso Not DotNetNuke.Security.PortalSecurity.IsInRoles(FileSystemUtils.GetRoles(objFile.Folder, PortalSettings.PortalId, "READ")) Then
                            ' remove document from the list
                            mobjDocumentList.Remove(objDocument)
                        End If
                    End If
                Next

                ' Only write to the cache if the user is not logged in
                If Not Request.IsAuthenticated Then
                    DataCache.SetCache(strCacheKey, mobjDocumentList, New TimeSpan(0, 5, 0))
                End If
            End If

            'Sort documents
            mobjDocumentList.Sort(New DocumentComparer(mobjSettings.SortColumnList))

            IsReadComplete = True
        End Sub

        Private Property IsReadComplete() As Boolean
            Get
                Return mblnReadComplete
            End Get
            Set(ByVal value As Boolean)
                mblnReadComplete = value
            End Set
        End Property


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Dynamically adds a column to the datagrid
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="Title">The name of the property to read data from</param>
        ''' <param name="DataField">The name of the property to read data from</param>
        ''' -----------------------------------------------------------------------------
        Private Sub AddDocumentColumn(ByVal Title As String, ByVal CssClass As String, ByVal DataField As String)
            AddDocumentColumn(Title, CssClass, DataField, "")
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Dynamically adds a column to the datagrid
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="Title">The name of the property to read data from</param>
        ''' <param name="DataField">The name of the property to read data from</param>
        ''' <param name="Format">Format string for value</param>
        ''' -----------------------------------------------------------------------------
        Private Sub AddDocumentColumn(ByVal Title As String, ByVal CssClass As String, ByVal DataField As String, ByVal Format As String)
            Dim objBoundColumn As System.Web.UI.WebControls.BoundColumn

            objBoundColumn = New System.Web.UI.WebControls.BoundColumn

            objBoundColumn.DataField = DataField
            objBoundColumn.DataFormatString = Format
            objBoundColumn.HeaderText = Title
            'Added 5/17/2007
            'By Mitchel Sellers
            If mobjSettings.AllowUserSort Then
                objBoundColumn.SortExpression = DataField
            End If

            objBoundColumn.HeaderStyle.CssClass = CssClass & "Header" '"NormalBold"
            objBoundColumn.ItemStyle.CssClass = CssClass & "Cell"  '"Normal"

            Me.grdDocuments.Columns.Add(objBoundColumn)

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Dynamically adds a DownloadColumnTemplate column to the datagrid.  Used to
        ''' add the download link and title (if "title as link" is set) columns.
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="Title">The name of the property to read data from</param>
        ''' <param name="Name">The name of the property to read data from</param>
        ''' -----------------------------------------------------------------------------
        Private Sub AddDownloadLink(ByVal Title As String, ByVal Name As String)
            Dim objTemplateColumn As System.Web.UI.WebControls.TemplateColumn
            Dim strCellPrefix As String

            objTemplateColumn = New System.Web.UI.WebControls.TemplateColumn
            objTemplateColumn.ItemTemplate = New DownloadColumnTemplate(Name, Localization.GetString("DownloadLink.Text", LocalResourceFile), ListItemType.Item)
            objTemplateColumn.HeaderText = Title

            strCellPrefix = Title
            If strCellPrefix = String.Empty AndAlso Name = "ctlDownloadLink" Then
                strCellPrefix = "Download"
            End If

            objTemplateColumn.HeaderStyle.CssClass = strCellPrefix & "Header" '"NormalBold"
            objTemplateColumn.ItemStyle.CssClass = strCellPrefix & "Cell" '"Normal"

            'Added 5/17/2007
            'By Mitchel Sellers
            ' Add the sort expression, however ensure that it is NOT added for download
            If mobjSettings.AllowUserSort AndAlso Not Name.Equals("ctlDownloadLink") Then
                objTemplateColumn.SortExpression = Title
            End If
            Me.grdDocuments.Columns.Add(objTemplateColumn)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Load module settings from the database.
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' -----------------------------------------------------------------------------
        Private Function LoadSettings() As DocumentsSettingsInfo
            Dim objDocumentsSettings As DocumentsSettingsInfo
            ' Load module instance settings
            With New DocumentController
                objDocumentsSettings = .GetDocumentsSettings(ModuleId)
            End With

            ' first time around, no existing documents settings will exist
            If objDocumentsSettings Is Nothing Then
                objDocumentsSettings = New DocumentsSettingsInfo
            End If

            Return objDocumentsSettings
        End Function
#End Region

    End Class

End Namespace
