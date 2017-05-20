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

Namespace DotNetNuke.Modules.Documents
  Public Class DownloadColumnTemplate

    Implements System.Web.UI.ITemplate

    'Shared itemcount As Integer = 0
    Private mobjTemplateType As System.Web.UI.webcontrols.ListItemType
    Private mstrID As String
    Private mstrCaption As String

    Sub New(ByVal ID As String, ByVal Caption As String, ByVal Type As System.Web.UI.webcontrols.ListItemType)
      mobjTemplateType = Type
      mstrID = ID
      mstrCaption = Caption
      If mstrCaption = String.Empty Then
        mstrCaption = "Download"
      End If
    End Sub

    Public Sub InstantiateIn(ByVal container As System.Web.UI.Control) Implements System.Web.UI.ITemplate.InstantiateIn
      Dim objButton As System.Web.UI.WebControls.HyperLink

      Select Case mobjTemplateType
        Case Web.UI.WebControls.ListItemType.Item, _
             Web.UI.WebControls.ListItemType.AlternatingItem, _
             Web.UI.WebControls.ListItemType.SelectedItem
          objButton = New System.Web.UI.WebControls.HyperLink
          objButton.Text = mstrCaption
          objButton.ID = mstrID

          container.Controls.Add(objButton)
      End Select
      'itemcount += 1
    End Sub
  End Class
End Namespace