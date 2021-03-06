'
' DotNetNukeŽ - http://www.dotnetnuke.com
' Copyright (c) 2002-2013
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


    ''' <summary>
    ''' Holds the information about a single document
    ''' </summary>
  Public Class DocumentInfo

#Region "Private Members"
        Private _clicks As Integer
#End Region



#Region "Auto Implemented Properties"

        ' public properties
        Public Property ItemId As Integer
        Public Property ModuleId As Integer
        Public Property CreatedByUserId As Integer
        Public Property CreatedByUser As String
        Public Property CreatedDate As Date
        Public Property Url As String
        Public Property Title As String
        Public Property Category As String
        Public Property Size As Integer
        Public Property ContentType As String
        Public Property TrackClicks As Boolean
        Public Property NewWindow As Boolean
        Public Property OwnedByUserId As Integer
        Public Property OwnedByUser As String
        Public Property ModifiedByUserId As Integer
        Public Property ModifiedByUser As String
        Public Property ModifiedDate As DateTime
        Public Property SortOrderIndex As Integer
        Public Property Description As String
        Public Property ForceDownload As Boolean
#End Region





#Region "Custom Properties"

        ''' <summary>
        ''' Gets the size of the format.
        ''' </summary>
        ''' <value>The size of the format.</value>
        Public ReadOnly Property FormatSize() As String
            Get
                Try
                    If Not Common.Utilities.Null.IsNull(Size) Then
                        If Size > (1024 * 1024) Then
                            Return Format(Size / (1024 * 1024), "#,##0.00 MB")
                        Else
                            Return Format(Size / 1024, "#,##0.00 KB")
                        End If
                    Else
                        Return Localization.GetString("Unknown")
                    End If
                Catch exc As Exception
                    Return Localization.GetString("Unknown")
                End Try
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets the clicks.
        ''' </summary>
        ''' <value>The clicks.</value>
        Public Property Clicks() As Integer
            Get
                If _clicks < 0 Then
                    Return 0
                Else
                    Return _clicks
                End If

            End Get
            Set(ByVal Value As Integer)
                _clicks = Value
            End Set
        End Property
#End Region
        

  End Class

End Namespace
