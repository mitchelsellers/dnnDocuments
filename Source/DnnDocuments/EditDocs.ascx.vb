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

Namespace DotNetNuke.Modules.Documents

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The EditDocs Class provides the UI for managing the Documents
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	9/22/2004	Moved Documents to a separate Project
    '''   [ag]  11 March 2007 Migrated to VS2005
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public MustInherit Class EditDocs
        Inherits Entities.Modules.PortalModuleBase

#Region "Controls"

        Protected WithEvents plName As UI.UserControls.LabelControl
        Protected WithEvents txtName As System.Web.UI.WebControls.TextBox
        Protected WithEvents valName As System.Web.UI.WebControls.RequiredFieldValidator
        Protected WithEvents plUrl As UI.UserControls.LabelControl
        Protected WithEvents ctlUrl As UI.UserControls.UrlControl
        Protected WithEvents plCategory As UI.UserControls.LabelControl
        Protected WithEvents txtCategory As System.Web.UI.WebControls.TextBox
        Protected WithEvents txtDescription As System.Web.UI.WebControls.TextBox
        Protected WithEvents chkForceDownload As System.Web.UI.WebControls.CheckBox
        Protected WithEvents lstCategory As System.Web.UI.WebControls.DropDownList
        Protected WithEvents lstOwner As System.Web.UI.WebControls.DropDownList

        'tasks
        Protected WithEvents cmdUpdate As System.Web.UI.WebControls.LinkButton
        Protected WithEvents cmdCancel As System.Web.UI.WebControls.LinkButton
        Protected WithEvents cmdDelete As System.Web.UI.WebControls.LinkButton
        Protected WithEvents cmdUpdateOverride As System.Web.UI.WebControls.LinkButton

        'footer
        Protected WithEvents ctlAudit As DotNetNuke.UI.UserControls.ModuleAuditControl
        Protected WithEvents ctlTracking As UI.UserControls.URLTrackingControl
        Protected WithEvents txtSortIndex As System.Web.UI.WebControls.TextBox
        Protected WithEvents valSortIndex As System.Web.UI.WebControls.RangeValidator
        Protected WithEvents lblOwner As System.Web.UI.WebControls.Label
        Protected WithEvents lnkChange As System.Web.UI.WebControls.LinkButton
        Protected WithEvents lblAudit As System.Web.UI.WebControls.Label

#End Region

#Region "Private Members"

        Private mintItemId As Integer

#End Region

#Region "Event Handlers"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Page_Load runs when the control is loaded
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/22/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Dim objDocumentsSettings As DocumentsSettingsInfo

            Try
                ' Determine ItemId of Document to Update
                If Not (Request.QueryString("ItemId") Is Nothing) Then
                    ItemID = Int32.Parse(Request.QueryString("ItemId"))
                Else
                    ItemID = Convert.ToInt32(Common.Utilities.Null.NullInteger)
                End If

                ' Load module instance settings
                If Not IsPostBack Then
                    objDocumentsSettings = LoadSettings()

                    ' Configure categories entry as a list or textbox, based on user settings
                    If objDocumentsSettings.UseCategoriesList Then
                        ' Configure category entry as a list
                        lstCategory.Visible = True
                        txtCategory.Visible = False

                        ' Populate categories list
                        With New DotNetNuke.Common.Lists.ListController
                            lstCategory.DataSource = .GetListEntryInfoCollection(objDocumentsSettings.CategoriesListName)
                            lstCategory.DataTextField = "Text"
                            lstCategory.DataValueField = "Value"

                            lstCategory.DataBind()
                            lstCategory.Items.Insert(0, New System.Web.UI.WebControls.ListItem(Services.Localization.Localization.GetString("None_Specified"), "-1"))
                        End With
                    Else
                        ' Configure category entry as a free-text entry
                        lstCategory.Visible = False
                        txtCategory.Visible = True
                    End If

                    ' Add the "are you sure" message to the delete button click event
                    cmdDelete.Attributes.Add("onClick", "javascript:return confirm('" & Localization.GetString("DeleteItem") & "');")

                    ' If the page is being requested the first time, determine if an
                    ' document itemId value is specified, and if so populate page
                    ' contents with the document details
                    If Not Common.Utilities.Null.IsNull(ItemID) Then

                        ' Read document information
                        Dim objDocuments As New DocumentController
                        Dim objDocument As DocumentInfo = objDocuments.GetDocument(ItemID, ModuleId)

                        ' Read values from documentInfo object on to page
                        If Not objDocument Is Nothing Then
                            txtName.Text = objDocument.Title
                            txtDescription.Text = objDocument.Description
                            chkForceDownload.Checked = objDocument.ForceDownload

                            If objDocument.Url <> String.Empty Then
                                ctlUrl.Url = objDocument.Url
                            End If

                            ' Test to see if the document has been removed/deleted
                            If CheckFileExists(objDocument.Url) = False Then
                                ctlUrl.UrlType = "N"
                            End If

                            CheckFileSecurity(objDocument.Url)

                            txtSortIndex.Text = objDocument.SortOrderIndex.ToString

                            If objDocument.OwnedByUser = String.Empty Then
                                lblOwner.Text = Services.Localization.Localization.GetString("None_Specified")
                            Else
                                lblOwner.Text = objDocument.OwnedByUser
                            End If

                            If txtCategory.Visible Then
                                txtCategory.Text = objDocument.Category
                            Else
                                'Look for the category by name
                                Dim found As ListItem = lstCategory.Items.FindByText(objDocument.Category)
                                If found IsNot Nothing Then
                                    lstCategory.SelectedValue = found.Value
                                Else
                                    'Legacy support, do a fall-back
                                    found = lstCategory.Items.FindByValue(objDocument.Category)
                                    If found IsNot Nothing Then
                                        lstCategory.SelectedValue = found.Value
                                    End If
                                End If
                            End If

                            ' The audit control methods are mis-named.  The property called 
                            ' "CreatedByUser" actually means "last modified user", and the property
                            ' called "CreatedDate" actually means "ModifiedDate"
                            ctlAudit.CreatedByUser = objDocument.ModifiedByUser
                            ctlAudit.CreatedDate = objDocument.ModifiedDate.ToString

                            ctlTracking.URL = objDocument.Url
                            ctlTracking.ModuleID = ModuleId

                        Else       ' security violation attempt to access item not related to this Module
                            Response.Redirect(NavigateURL(), True)
                        End If
                    Else
                        Try
                            lstOwner.SelectedValue = DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo.UserID.ToString
                            lblOwner.Text = DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo.DisplayName
                        Catch exc As Exception
                            ' suppress error (defensive code only, would only happen if the owner
                            ' user has been deleted)
                        End Try

                        cmdDelete.Visible = False
                        ctlAudit.Visible = False
                        ctlTracking.Visible = False

                        ' Set default folder
                        ctlUrl.Url = objDocumentsSettings.DefaultFolder & "A"

                    End If
                End If
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Compare file's folder security with module security settings and display 
        ''' a warning message if they do not match.
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[ag]	11 March 2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function CheckFileSecurity(ByVal Url As String) As Boolean
            Dim intFileId As Integer
            Dim objFiles As New DotNetNuke.Services.FileSystem.FileController
            Dim objFile As New DotNetNuke.Services.FileSystem.FileInfo

            Select Case GetURLType(Url)
                Case Entities.Tabs.TabType.File
                    If Url.ToLower.StartsWith("fileid=") = False Then
                        ' to handle legacy scenarios before the introduction of the FileServerHandler
                        Url = "FileID=" & objFiles.ConvertFilePathToFileId(Url, PortalSettings.PortalId)
                    End If

                    intFileId = Integer.Parse(UrlUtils.GetParameterValue(Url))

                    objFile = objFiles.GetFileById(intFileId, PortalId)
                    If Not objFile Is Nothing Then
                        ' Get file's folder security
                        Return CheckRolesMatch(Me.ModuleConfiguration.AuthorizedViewRoles, FileSystemUtils.GetRoles(objFile.Folder, PortalId, "READ"))
                    End If
            End Select
            Return True
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Tests whether the roles that the module allows read access to are a subset of
        ''' the file's read-access roles.  If not, display a warning.
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[ag]	11 March 2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function CheckRolesMatch(ByVal ModuleRoles As String, ByVal FileRoles As String) As Boolean
            Dim objFileRoles As New Hashtable
            Dim blnNotMatching As Boolean = False
            Dim strRolesForMessage As String = ""

            For Each strFileRole As String In FileRoles.Split(";"c)
                objFileRoles.Add(strFileRole, strFileRole)
                If strFileRole = DotNetNuke.Common.Globals.glbRoleAllUsersName Then
                    ' If read access to the file is available for "all users", the file can
                    ' always be accessed
                    Return True
                End If
            Next

            For Each strModuleRole As String In ModuleRoles.Split(";"c)
                If Not objFileRoles.ContainsKey(strModuleRole) Then
                    ' A view role exists for the module that is not available for the file
                    blnNotMatching = True
                    If strRolesForMessage <> String.Empty Then
                        strRolesForMessage = strRolesForMessage & ", "
                    End If
                    strRolesForMessage = strRolesForMessage & strModuleRole
                End If
            Next

            If blnNotMatching Then
                ' Warn user that roles do not match
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, _
                  DotNetNuke.Services.Localization.Localization.GetString("msgFileSecurityWarning.Text", Me.LocalResourceFile).Replace("[$ROLELIST]", IIf(strRolesForMessage.IndexOf(",") >= 0, "s", "").ToString & "'" & strRolesForMessage & "'"), _
                  DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning)
                Return False
            Else
                Return True
            End If
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Tests whether the file exists.  If it does not, add a warning message.
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[ag]	11 March 2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function CheckFileExists(ByVal Url As String) As Boolean
            Dim intFileId As Integer
            Dim objFiles As New DotNetNuke.Services.FileSystem.FileController
            Dim objFile As New DotNetNuke.Services.FileSystem.FileInfo
            Dim blnAddWarning As Boolean

            If Url = String.Empty Then
                ' File not selected
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, _
                DotNetNuke.Services.Localization.Localization.GetString("msgNoFileSelected.Text", Me.LocalResourceFile), _
                  DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning)
                Return False
            Else
                Select Case GetURLType(Url)
                    Case Entities.Tabs.TabType.File
                        If Url.ToLower.StartsWith("fileid=") = False Then
                            ' to handle legacy scenarios before the introduction of the FileServerHandler
                            Url = "FileID=" & objFiles.ConvertFilePathToFileId(Url, PortalSettings.PortalId)
                        End If

                        intFileId = Integer.Parse(UrlUtils.GetParameterValue(Url))

                        objFile = objFiles.GetFileById(intFileId, PortalId)

                        blnAddWarning = False
                        If objFile Is Nothing Then
                            blnAddWarning = True
                        Else
                            Select Case objFile.StorageLocation
                                Case DotNetNuke.Services.FileSystem.FolderController.StorageLocationTypes.InsecureFileSystem
                                    blnAddWarning = Not IO.File.Exists(objFile.PhysicalPath)
                                Case DotNetNuke.Services.FileSystem.FolderController.StorageLocationTypes.SecureFileSystem
                                    blnAddWarning = Not IO.File.Exists(objFile.PhysicalPath & glbProtectedExtension)
                                Case DotNetNuke.Services.FileSystem.FolderController.StorageLocationTypes.DatabaseSecure
                                    blnAddWarning = False  ' Database-stored files cannot be deleted seperately
                            End Select
                        End If

                        If blnAddWarning Then
                            ' Display a "file not found" warning
                            DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, _
                              DotNetNuke.Services.Localization.Localization.GetString("msgFileDeleted.Text", Me.LocalResourceFile), _
                              DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning)
                            Return False
                        End If

                    Case Entities.Tabs.TabType.Url
                        ' Cannot validate "Url" link types 
                End Select
            End If
            Return True
        End Function



        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdCancel_Click runs when the cancel button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/22/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdCancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdCancel.Click
            Try
                ' Redirect back to the portal home page
                Response.Redirect(NavigateURL(), True)
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdDelete_Click runs when the delete button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/22/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdDelete_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdDelete.Click
            Try
                If Not Common.Utilities.Null.IsNull(ItemID) Then

                    Dim objdocuments As New DocumentController
                    objdocuments.DeleteDocument(Me.ModuleId, ItemID)
                End If

                SynchronizeModule()
                DataCache.RemoveCache(Me.CacheKey & ";anon-doclist")

                ' Redirect back to the portal home page
                Response.Redirect(NavigateURL(), True)
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdUpdate_Click runs when the update button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/22/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdUpdate_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdUpdate.Click
            Update(False)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdUpdate_Click runs when the update "override" button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[ag]	11 March 2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdUpdateOverride_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdUpdateOverride.Click
            Update(True)
        End Sub

        Private Sub Update(ByVal Override As Boolean)
            Try
                ' Only Update if Input Data is Valid
                If Page.IsValid = True Then

                    If Not Override Then
                        ' Test file exists, security
                        If Not CheckFileExists(ctlUrl.Url) OrElse Not CheckFileSecurity(ctlUrl.Url) Then
                            Me.cmdUpdateOverride.Visible = True
                            Me.cmdUpdate.Visible = False

                            ' '' Display page-level warning instructing users to click update again if they want to ignore the warning
                            DotNetNuke.UI.Skins.Skin.AddPageMessage( _
                               Me.Page, _
                               DotNetNuke.Services.Localization.Localization.GetString("msgFileWarningHeading.Text", Me.LocalResourceFile), _
                               DotNetNuke.Services.Localization.Localization.GetString("msgFileWarning.Text", Me.LocalResourceFile), _
                               Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning)
                            Exit Sub
                        End If
                    End If

                    Dim objDocument As DocumentInfo
                    Dim objDocuments As New DocumentController

                    ' Get existing document record
                    objDocument = objDocuments.GetDocument(ItemID, ModuleId)

                    If objDocument Is Nothing Then
                        ' New record
                        objDocument = New DocumentInfo
                        objDocument.ItemId = ItemID
                        objDocument.ModuleId = ModuleId

                        objDocument.CreatedByUserID = UserInfo.UserID

                        ' Default ownerid value for new documents is current user, may be changed
                        ' by the value of the dropdown list (below)
                        objDocument.OwnedByUserID = UserId
                    End If

                    objDocument.Title = txtName.Text
                    objDocument.Url = ctlUrl.Url
                    objDocument.Description = txtDescription.Text
                    objDocument.ForceDownload = chkForceDownload.Checked

                    If lstOwner.Visible Then
                        If lstOwner.SelectedValue <> String.Empty Then
                            objDocument.OwnedByUserID = Convert.ToInt32(lstOwner.SelectedValue)
                        Else
                            objDocument.OwnedByUserID = -1
                        End If
                    Else
                        ' User never clicked "change", leave ownedbyuserid as is
                    End If

                    If txtCategory.Visible Then
                        objDocument.Category = txtCategory.Text
                    Else
                        If lstCategory.SelectedItem.Text = Services.Localization.Localization.GetString("None_Specified") Then
                            objDocument.Category = ""
                        Else
                            objDocument.Category = lstCategory.SelectedItem.Value
                        End If
                    End If

                    If txtSortIndex.Text = String.Empty Then
                        objDocument.SortOrderIndex = 0
                    Else
                        objDocument.SortOrderIndex = Convert.ToInt32(txtSortIndex.Text)
                    End If
                    
                    ' Create an instance of the Document DB component

                    If Common.Utilities.Null.IsNull(ItemID) Then
                        objDocuments.AddDocument(objDocument)
                    Else
                        objDocuments.UpdateDocument(objDocument)
                    End If

                    ' url tracking
                    Dim objUrls As New UrlController
                    objUrls.UpdateUrl(PortalId, ctlUrl.Url, ctlUrl.UrlType, ctlUrl.Log, ctlUrl.Track, ModuleId, ctlUrl.NewWindow)

                    SynchronizeModule()
                    DataCache.RemoveCache(Me.CacheKey & ";anon-doclist")

                    ' Redirect back to the portal home page
                    Response.Redirect(NavigateURL(), True)

                End If
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

#End Region

#Region "Private methods"
        Private Property ItemID() As Integer
            Get
                Return mintItemId
            End Get
            Set(ByVal Value As Integer)
                mintItemId = Value
            End Set
        End Property

        Private Function LoadSettings() As DocumentsSettingsInfo
            Dim objDocumentsSettings As DocumentsSettingsInfo
            ' Load module instance settings
            With New DocumentController
                objDocumentsSettings = .GetDocumentsSettings(ModuleId)
            End With

            ' first time around, no existing documents settings will exist
            If objDocumentsSettings Is Nothing Then
                objDocumentsSettings = New DocumentsSettingsInfo(MyBase.LocalResourceFile)
            End If

            Return objDocumentsSettings
        End Function
#End Region

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

        Private Sub lnkChange_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lnkChange.Click
            lblOwner.Visible = False
            lnkChange.Visible = False
            lstOwner.Visible = True

            PopulateOwnerList()

            Try
                Dim objDocument As DocumentInfo
                Dim objDocuments As New DocumentController

                ' Get existing document record
                objDocument = objDocuments.GetDocument(ItemID, ModuleId)

                Try
                    If objDocument Is Nothing Then
                        lstOwner.SelectedValue = DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo.UserID.ToString
                    Else
                        lstOwner.SelectedValue = objDocument.OwnedByUserID.ToString
                    End If
                Catch exc As Exception
                    ' suppress error selecting owner user
                End Try

            Catch exc As Exception
                ' suppress error if the user no longer exists
            End Try
        End Sub

        Private Sub PopulateOwnerList()
            ' populate owner list
            ''With New DotNetNuke.Entities.Users.UserController
            lstOwner.DataSource = DotNetNuke.Entities.Users.UserController.GetUsers(PortalId, False)

            lstOwner.DataTextField = "FullName"
            lstOwner.DataValueField = "UserId"

            lstOwner.DataBind()

            ' .GetUsers doesn't return super-users, but they can own documents
            ' so add them to the list
            Dim objSuperUser As DotNetNuke.Entities.Users.UserInfo
            For Each objSuperUser In DotNetNuke.Entities.Users.UserController.GetUsers(Null.NullInteger, False)
                lstOwner.Items.Insert(0, New System.Web.UI.WebControls.ListItem(objSuperUser.DisplayName, objSuperUser.UserID.ToString))
            Next

            lstOwner.Items.Insert(0, New System.Web.UI.WebControls.ListItem(Services.Localization.Localization.GetString("None_Specified"), "-1"))
            '' End With
        End Sub

    End Class

End Namespace