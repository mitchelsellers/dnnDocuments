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

Imports System.IO
Imports System.Web
Imports DotNetNuke.Modules.Documents.DocumentController

Namespace DotNetNuke.Modules.Documents

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The EditDocs Class provides the UI for manaing the Documents
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	9/22/2004	Moved Documents to a separate Project
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public MustInherit Class EditDocumentsSettings
        Inherits Entities.Modules.ModuleSettingsBase

        Private Const VIEWSTATE_SORTCOLUMNSETTINGS As String = "SortColumnSettings"
        Private Const VIEWSTATE_DISPLAYCOLUMNSETTINGS As String = "DisplayColumnSettings"

#Region " Web Form Designer Generated Code "

        'This call is required by the Web Form Designer.
        <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

        End Sub

        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
            'CODEGEN: This method call is required by the Web Form Designer
            'Do not modify it using the code editor.
            InitializeComponent()
        End Sub

#End Region

#Region "Controls"

        Protected WithEvents chkShowTitleLink As System.Web.UI.WebControls.CheckBox
        Protected WithEvents chkUseCategoriesList As System.Web.UI.WebControls.CheckBox
        Protected WithEvents lstSortFields As System.Web.UI.WebControls.DropDownList
        Protected WithEvents grdSortColumns As System.Web.UI.WebControls.DataGrid
        Protected WithEvents lnkAddSortColumn As System.Web.UI.WebControls.LinkButton
        Protected WithEvents grdDisplayColumns As System.Web.UI.WebControls.DataGrid
        Protected WithEvents cboCategoriesList As System.Web.UI.WebControls.DropDownList
        Protected WithEvents cboDefaultFolder As System.Web.UI.WebControls.DropDownList
        Protected WithEvents lnkEditLists As System.Web.UI.WebControls.HyperLink
        Protected WithEvents lstNoListsAvailable As System.Web.UI.WebControls.Label
        Protected WithEvents cboSortOrderDirection As System.Web.UI.WebControls.DropDownList
        Protected WithEvents chkAllowUserSort As System.Web.UI.WebControls.CheckBox
        Protected WithEvents lblCannotEditLists As System.Web.UI.WebControls.Label
        'footer

#End Region

#Region "Event Handlers"
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' LoadSettings loads the settings from the Databas and displays them
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub LoadSettings()
            Dim objDocumentsSettings As DocumentsSettingsInfo
            Dim objColumnSettings As New ArrayList
            Dim objColumnInfo As DocumentsDisplayColumnInfo
            Dim strColumnName As String

            Try
                If (Page.IsPostBack = False) Then
                    LoadFolders()
                    LoadLists()

                    ' Read current documentsSettings object
                    With New DocumentController
                        objDocumentsSettings = .GetDocumentsSettings(ModuleId)
                        If Not objDocumentsSettings Is Nothing Then
                            objDocumentsSettings.LocalResourceFile = Me.LocalResourceFile
                        Else
                            ' first time around, no existing documents settings will exist
                            objDocumentsSettings = New DocumentsSettingsInfo(MyBase.LocalResourceFile)
                        End If
                    End With

                    chkShowTitleLink.Checked = objDocumentsSettings.ShowTitleLink
                    chkUseCategoriesList.Checked = objDocumentsSettings.UseCategoriesList
                    chkAllowUserSort.Checked = objDocumentsSettings.AllowUserSort

                    Try
                        cboDefaultFolder.SelectedValue = objDocumentsSettings.DefaultFolder
                    Catch exc As Exception
                        ' suppress exception.  Can be caused if the selected folder has been deleted
                    End Try

                    Try
                        cboCategoriesList.SelectedValue = objDocumentsSettings.CategoriesListName
                    Catch ex As Exception
                        ' suppress exception.  Can be caused if the selected list has been deleted
                    End Try

                    ' read "saved" column sort orders in first
                    objColumnSettings = objDocumentsSettings.DisplayColumnList
                    For Each objColumnInfo In objColumnSettings
                        ' Set localized column names
                        objColumnInfo.LocalizedColumnName = Services.Localization.Localization.GetString(objColumnInfo.ColumnName & ".Header", MyBase.LocalResourceFile)
                    Next

                    ' Add any missing columns to the end
                    For Each strColumnName In DocumentsDisplayColumnInfo.AvailableDisplayColumns
                        If DocumentsSettingsInfo.FindColumn(strColumnName, objColumnSettings, False) < 0 Then
                            objColumnInfo = New DocumentsDisplayColumnInfo
                            objColumnInfo.ColumnName = strColumnName
                            objColumnInfo.LocalizedColumnName = Services.Localization.Localization.GetString(objColumnInfo.ColumnName & ".Header", MyBase.LocalResourceFile)
                            objColumnInfo.DisplayOrder = objColumnSettings.Count + 1
                            objColumnInfo.Visible = False

                            objColumnSettings.Add(objColumnInfo)
                        End If
                    Next

                    ' Sort by DisplayOrder
                    BindColumnSettings(objColumnSettings)

                    ' Load sort columns 
                    Dim strSortColumn As String
                    Dim objSortColumns As New ArrayList
                    For Each strSortColumn In DocumentsDisplayColumnInfo.AvailableSortColumns
                        lstSortFields.Items.Add(New Web.UI.WebControls.ListItem(Services.Localization.Localization.GetString(strSortColumn & ".Header", MyBase.LocalResourceFile), strSortColumn))
                    Next

                    BindSortSettings(objDocumentsSettings.SortColumnList)
                End If
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Sub LoadFolders()
            cboDefaultFolder.Items.Clear()

            For Each objFolder As DotNetNuke.Services.FileSystem.FolderInfo In FileSystemUtils.GetFoldersByUser(PortalId, True, True, "READ, WRITE")
                Dim FolderItem As New Web.UI.WebControls.ListItem

                If objFolder.FolderPath = Null.NullString Then
                    FolderItem.Text = Localization.GetString("Root.Text", Me.LocalResourceFile)
                    FolderItem.Value = ""
                Else
                    FolderItem.Text = objFolder.FolderPath
                    FolderItem.Value = objFolder.FolderPath
                End If
                FolderItem.Value = objFolder.FolderPath

                cboDefaultFolder.Items.Add(FolderItem)
            Next
        End Sub

        Sub LoadLists()
            With New DotNetNuke.Common.Lists.ListController
                For Each objList As DotNetNuke.Common.Lists.ListInfo In .GetListInfoCollection
                    If Not objList.SystemList Then
                        ' for some reason, the "DataType" is not marked as a system list, but we want to exclude that one too
                        If objList.DisplayName <> "DataType" Then
                            cboCategoriesList.Items.Add(New Web.UI.WebControls.ListItem(objList.DisplayName, objList.DisplayName))
                        End If
                    End If
                Next
            End With

            If cboCategoriesList.Items.Count = 0 Then
                lstNoListsAvailable.Text = Services.Localization.Localization.GetString("msgNoListsAvailable.Text", MyBase.LocalResourceFile)
                lstNoListsAvailable.Visible = True
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' UpdateSettings saves the modified settings to the Database
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub UpdateSettings()
            Try
                Dim objDocumentsSettings As DocumentsSettingsInfo

                If Page.IsValid Then

                    With New DocumentController
                        objDocumentsSettings = .GetDocumentsSettings(ModuleId)
                    End With

                    If objDocumentsSettings Is Nothing Then
                        ' first time around, no existing documents settings will exist, so
                        ' create one
                        objDocumentsSettings = New DocumentsSettingsInfo(MyBase.LocalResourceFile)
                        objDocumentsSettings.ModuleId = ModuleId
                        FillSettings(objDocumentsSettings)
                        With New DocumentController
                            .AddDocumentsSettings(objDocumentsSettings)
                        End With
                    Else
                        ' Documents settings found, update
                        FillSettings(objDocumentsSettings)

                        With New DocumentController
                            .UpdateDocumentsSettings(objDocumentsSettings)
                        End With
                    End If

                    SynchronizeModule()
                    DataCache.RemoveCache(Me.CacheKey & ";anon-doclist")
                End If
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Public Overloads ReadOnly Property CacheKey() As String
            Get
                Dim strCacheKey As String = "TabModule:"
                strCacheKey += TabModuleId.ToString & ":"
                strCacheKey += System.Threading.Thread.CurrentThread.CurrentCulture.ToString
                Return strCacheKey
            End Get
        End Property

        Public Function GetLocalizedText(ByVal Key As String) As String
            Return Services.Localization.Localization.GetString(Key, MyBase.LocalResourceFile)
        End Function

        Private Sub grdSortColumns_ItemCreated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles grdSortColumns.ItemCreated
            Select Case e.Item.ItemType
                Case System.Web.UI.WebControls.ListItemType.AlternatingItem, _
                  System.Web.UI.WebControls.ListItemType.Item, _
                  System.Web.UI.WebControls.ListItemType.SelectedItem
                    e.Item.CssClass = "Normal"

                    ' Localize the delete linkbutton/set css class
                    CType(e.Item.Cells(2).Controls(0), System.Web.UI.WebControls.LinkButton).Text = Services.Localization.Localization.GetString("cmdDelete.Text", MyBase.LocalResourceFile)
                    e.Item.Cells(2).CssClass = "CommandButton"
            End Select
        End Sub

        Private Sub grdDisplayColumns_ItemCreated(ByVal sender As System.Object, ByVal e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles grdDisplayColumns.ItemCreated
            Dim objUpImage As System.Web.UI.WebControls.ImageButton
            Dim objDownImage As System.Web.UI.WebControls.ImageButton

            Select Case e.Item.ItemType
                Case System.Web.UI.WebControls.ListItemType.AlternatingItem, _
                  System.Web.UI.WebControls.ListItemType.Item, _
                  System.Web.UI.WebControls.ListItemType.SelectedItem

                    ' Center the "visible" checkbox in its cell
                    e.Item.Cells(1).Style.Add("text-align", "center")

                    ' imgUp
                    objUpImage = CType(e.Item.Cells(2).FindControl("imgUp"), System.Web.UI.WebControls.ImageButton)
                    objUpImage.Visible = (e.Item.ItemIndex <> 0)
                    objUpImage.ImageUrl = ResolveUrl("~/images/up.gif")

                    ' imgDown
                    objDownImage = CType(e.Item.Cells(2).FindControl("imgDown"), System.Web.UI.WebControls.ImageButton)
                    objDownImage.ImageUrl = ResolveUrl("~/images/dn.gif")
                    If objUpImage.Visible = False Then
                        objDownImage.Style.Add("margin-left", "19px")
                    End If

                    e.Item.CssClass = "Normal"

                Case System.Web.UI.WebControls.ListItemType.Header
                    e.Item.CssClass = "SubHead"
            End Select
        End Sub

        Private Sub grdDisplayColumns_ItemCommand(ByVal source As System.Object, ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles grdDisplayColumns.ItemCommand
            Select Case e.CommandName
                Case "DisplayOrderDown"
                    ' swap e.CommandArgument and the one after it
                    SwapColumn(e.CommandArgument.ToString, System.ComponentModel.ListSortDirection.Descending)
                Case "DisplayOrderUp"
                    ' swap e.CommandArgument and the one before it
                    SwapColumn(e.CommandArgument.ToString, System.ComponentModel.ListSortDirection.Ascending)
            End Select
        End Sub

        Private Sub lnkAddSortColumn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lnkAddSortColumn.Click
            Dim objSortColumns As ArrayList
            Dim objNewSortColumn As New DocumentsSortColumnInfo

            objSortColumns = RetrieveSortColumnSettings()
            objNewSortColumn.ColumnName = lstSortFields.SelectedValue.ToString
            objNewSortColumn.LocalizedColumnName = Services.Localization.Localization.GetString(objNewSortColumn.ColumnName & ".Header", MyBase.LocalResourceFile)
            If cboSortOrderDirection.SelectedValue = "ASC" Then
                objNewSortColumn.Direction = DocumentsSortColumnInfo.SortDirection.Ascending
            Else
                objNewSortColumn.Direction = DocumentsSortColumnInfo.SortDirection.Descending
            End If

            objSortColumns.Add(objNewSortColumn)
            BindSortSettings(objSortColumns)
        End Sub

        Private Sub grdSortColumns_DeleteCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles grdSortColumns.DeleteCommand
            Dim objSortColumns As ArrayList
            Dim objSortColumnToDelete As New DocumentsSortColumnInfo

            objSortColumns = RetrieveSortColumnSettings()

            For Each objSortColumnToDelete In objSortColumns
                If objSortColumnToDelete.ColumnName = grdSortColumns.DataKeys(e.Item.ItemIndex).ToString Then
                    objSortColumns.Remove(objSortColumnToDelete)
                    Exit For
                End If
            Next

            BindSortSettings(objSortColumns)
        End Sub

#End Region

#Region "Control Handling/Utility Functions"
        Private Sub BindSortSettings(ByVal objSortColumns As ArrayList)
            SaveSortColumnSettings(objSortColumns)
            grdSortColumns.DataSource = objSortColumns
            grdSortColumns.DataKeyField = "ColumnName"
            Localization.LocalizeDataGrid(grdSortColumns, Me.LocalResourceFile)
            grdSortColumns.DataBind()
        End Sub

        Private Sub BindColumnSettings(ByVal objColumnSettings As ArrayList)
            objColumnSettings.Sort()
            SaveDisplayColumnSettings(objColumnSettings)
            grdDisplayColumns.DataSource = objColumnSettings
            grdDisplayColumns.DataKeyField = "ColumnName"

            If Not Me.IsPostBack Then
                Localization.LocalizeDataGrid(grdDisplayColumns, Me.LocalResourceFile)
            End If

            grdDisplayColumns.DataBind()

            With CType(grdDisplayColumns.Items(grdDisplayColumns.Items.Count - 1).Cells(2).FindControl("imgDown"), System.Web.UI.WebControls.ImageButton)
                ' Set down arrow invisible on the last item
                .Visible = False
            End With

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Read settings from the screen into the passed-in DocumentsSettings object
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub FillSettings(ByVal objDocumentsSettings As DocumentsSettingsInfo)
            Dim strDisplayColumns As String = ""
            Dim objColumnSettings As ArrayList
            Dim objColumnInfo As DocumentsDisplayColumnInfo
            Dim intIndex As Integer
            Dim objSortColumns As ArrayList
            Dim strSortColumnList As String = ""
            Dim objSortColumn As DocumentsSortColumnInfo

            'Ensure that if categories list is checked that we did have an available category
            If (chkUseCategoriesList.Checked AndAlso Not lstNoListsAvailable.Visible) Then
                'If so, set normally
                objDocumentsSettings.UseCategoriesList = chkUseCategoriesList.Checked
                objDocumentsSettings.CategoriesListName = cboCategoriesList.SelectedValue
            Else
                'Otherwise default values
                objDocumentsSettings.UseCategoriesList = False
                objDocumentsSettings.CategoriesListName = ""
            End If

            objDocumentsSettings.ShowTitleLink = chkShowTitleLink.Checked
            objDocumentsSettings.DefaultFolder = cboDefaultFolder.SelectedValue
            objDocumentsSettings.AllowUserSort = chkAllowUserSort.Checked

            objColumnSettings = RetrieveDisplayColumnSettings()
            intIndex = 0
            For Each objColumnInfo In objColumnSettings
                ' Figure out column visibility
                objColumnInfo.Visible = CType(grdDisplayColumns.Items(intIndex).Cells(1).FindControl("chkVisible"), System.Web.UI.WebControls.CheckBox).Checked

                If strDisplayColumns <> String.Empty Then
                    strDisplayColumns = strDisplayColumns & ","
                End If
                strDisplayColumns = strDisplayColumns & objColumnInfo.ColumnName & ";" & objColumnInfo.Visible.ToString

                intIndex = intIndex + 1
            Next

            objDocumentsSettings.DisplayColumns = strDisplayColumns

            objSortColumns = RetrieveSortColumnSettings()
            For Each objSortColumn In objSortColumns
                If strSortColumnList <> String.Empty Then
                    strSortColumnList = strSortColumnList & ","
                End If
                strSortColumnList = strSortColumnList & IIf(objSortColumn.Direction = DocumentsSortColumnInfo.SortDirection.Descending, "-", "").ToString & objSortColumn.ColumnName
            Next
            objDocumentsSettings.SortOrder = strSortColumnList
        End Sub

        Private Sub SwapColumn(ByVal ColumnName As String, ByVal Direction As System.ComponentModel.ListSortDirection)
            Dim objColumnSettings As ArrayList
            Dim intIndex As Integer
            Dim intDisplayOrderTemp As Integer

            ' First, find the column we want
            objColumnSettings = RetrieveDisplayColumnSettings()
            intIndex = DocumentsSettingsInfo.FindColumn(ColumnName, objColumnSettings, False)

            ' Swap display orders
            If intIndex >= 0 Then
                Select Case Direction
                    Case System.ComponentModel.ListSortDirection.Ascending
                        ' swap up
                        If intIndex > 0 Then
                            intDisplayOrderTemp = CType(objColumnSettings(intIndex), DocumentsDisplayColumnInfo).DisplayOrder
                            CType(objColumnSettings(intIndex), DocumentsDisplayColumnInfo).DisplayOrder = CType(objColumnSettings(intIndex - 1), DocumentsDisplayColumnInfo).DisplayOrder
                            CType(objColumnSettings(intIndex - 1), DocumentsDisplayColumnInfo).DisplayOrder = intDisplayOrderTemp
                        End If
                    Case System.ComponentModel.ListSortDirection.Descending
                        ' swap down
                        If intIndex < objColumnSettings.Count Then
                            intDisplayOrderTemp = CType(objColumnSettings(intIndex), DocumentsDisplayColumnInfo).DisplayOrder
                            CType(objColumnSettings(intIndex), DocumentsDisplayColumnInfo).DisplayOrder = CType(objColumnSettings(intIndex + 1), DocumentsDisplayColumnInfo).DisplayOrder
                            CType(objColumnSettings(intIndex + 1), DocumentsDisplayColumnInfo).DisplayOrder = intDisplayOrderTemp
                        End If
                End Select
            End If

            ' Re-bind the newly sorted collection to the datagrid
            BindColumnSettings(objColumnSettings)
        End Sub
#End Region

        Private Sub SaveSortColumnSettings(ByVal objSettings As ArrayList)
            ' Custom viewstate implementation to avoid reflection
            Dim objSortColumnInfo As DocumentsSortColumnInfo
            Dim strValues As String = ""

            For Each objSortColumnInfo In objSettings
                If strValues <> String.Empty Then
                    strValues = strValues & "#"
                End If

                strValues = strValues & objSortColumnInfo.ColumnName & "," & objSortColumnInfo.LocalizedColumnName & "," & objSortColumnInfo.Direction.ToString
            Next
            ViewState(VIEWSTATE_SORTCOLUMNSETTINGS) = strValues
        End Sub

        Private Function RetrieveSortColumnSettings() As ArrayList
            ' Custom viewstate implementation to avoid reflection
            Dim objSortColumnSettings As New ArrayList
            Dim objSortColumnInfo As DocumentsSortColumnInfo

            Dim strValues As String

            strValues = CType(ViewState(VIEWSTATE_SORTCOLUMNSETTINGS), String)
            If Not strValues Is Nothing AndAlso strValues <> String.Empty Then
                For Each strSortColumnSetting As String In strValues.Split(Char.Parse("#"))
                    objSortColumnInfo = New DocumentsSortColumnInfo
                    objSortColumnInfo.ColumnName = strSortColumnSetting.Split(Char.Parse(","))(0)
                    objSortColumnInfo.LocalizedColumnName = strSortColumnSetting.Split(Char.Parse(","))(1)
                    objSortColumnInfo.Direction = CType(System.Enum.Parse(GetType(DocumentsSortColumnInfo.SortDirection), strSortColumnSetting.Split(Char.Parse(","))(2)), DocumentsSortColumnInfo.SortDirection)

                    objSortColumnSettings.Add(objSortColumnInfo)
                Next
            End If

            Return objSortColumnSettings
        End Function

        Private Sub SaveDisplayColumnSettings(ByVal objSettings As ArrayList)
            ' Custom viewstate implementation to avoid reflection
            Dim objDisplayColumnInfo As DocumentsDisplayColumnInfo
            Dim strValues As String = ""

            For Each objDisplayColumnInfo In objSettings
                If strValues <> String.Empty Then
                    strValues = strValues & "#"
                End If
                strValues = strValues & objDisplayColumnInfo.ColumnName & "," & objDisplayColumnInfo.LocalizedColumnName & "," & objDisplayColumnInfo.DisplayOrder & "," & objDisplayColumnInfo.Visible
            Next
            ViewState(VIEWSTATE_DISPLAYCOLUMNSETTINGS) = strValues
        End Sub

        Private Function RetrieveDisplayColumnSettings() As ArrayList
            ' Custom viewstate implementation to avoid reflection
            Dim objDisplayColumnSettings As New ArrayList
            Dim objDisplayColumnInfo As DocumentsDisplayColumnInfo

            Dim strValues As String

            strValues = CType(ViewState(VIEWSTATE_DISPLAYCOLUMNSETTINGS), String)
            If Not strValues Is Nothing AndAlso strValues <> String.Empty Then
                For Each strDisplayColumnSetting As String In strValues.Split(Char.Parse("#"))
                    objDisplayColumnInfo = New DocumentsDisplayColumnInfo
                    objDisplayColumnInfo.ColumnName = strDisplayColumnSetting.Split(Char.Parse(","))(0)
                    objDisplayColumnInfo.LocalizedColumnName = strDisplayColumnSetting.Split(Char.Parse(","))(1)
                    objDisplayColumnInfo.DisplayOrder = CInt(strDisplayColumnSetting.Split(Char.Parse(","))(2))
                    objDisplayColumnInfo.Visible = CBool(strDisplayColumnSetting.Split(Char.Parse(","))(3))

                    objDisplayColumnSettings.Add(objDisplayColumnInfo)
                Next
            End If

            Return objDisplayColumnSettings
        End Function

        Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If UserInfo.IsSuperUser() Then
                lnkEditLists.Text = Services.Localization.Localization.GetString("lnkEditLists", MyBase.LocalResourceFile)

                'lnkEditLists.Target = "_blank"

                Try
                    With New DotNetNuke.Entities.Tabs.TabController
                        lnkEditLists.NavigateUrl = .GetTabByName("Lists", DotNetNuke.Common.Utilities.Null.NullInteger).FullUrl
                    End With
                Catch ex As Exception
                    'Unable to locate "Lists" tab
                    lblCannotEditLists.Text = Services.Localization.Localization.GetString("UnableToFindLists", MyBase.LocalResourceFile)
                    lblCannotEditLists.Visible = True
                    lnkEditLists.Visible = False
                End Try
            Else
                'Show error, then hide the "Edit" link
                lblCannotEditLists.Text = Services.Localization.Localization.GetString("NoListAccess", MyBase.LocalResourceFile)
                lblCannotEditLists.Visible = True
                lnkEditLists.Visible = False
            End If

            If Not IsPostBack Then
                cboSortOrderDirection.Items.Add(New System.Web.UI.WebControls.ListItem(Services.Localization.Localization.GetString("SortOrderAscending.Text", MyBase.LocalResourceFile), "ASC"))
                cboSortOrderDirection.Items.Add(New System.Web.UI.WebControls.ListItem(Services.Localization.Localization.GetString("SortOrderDescending.Text", MyBase.LocalResourceFile), "DESC"))
            End If
        End Sub

    End Class

End Namespace