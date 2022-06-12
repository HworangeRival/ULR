Public Class Form1
    '+---------------------------------------------------------------------------------------+
    '888    888                                                                          8888888b.  d8b                   888 '      
    '888    888                                                                          888   Y88b Y8P                   888 '      
    '888    888                                                                          888    888                       888 '      
    '8888888888 888  888  888  .d88b.  888d888  8888b.  88888b.   .d88b.   .d88b.        888   d88P 888 888  888  8888b.  888 '      
    '888    888 888  888  888 d88""88b 888P"       "88b 888 "88b d88P"88b d8P  Y8b       8888888P"  888 888  888     "88b 888 '      
    '888    888 888  888  888 888  888 888     .d888888 888  888 888  888 88888888       888 T88b   888 Y88  88P .d888888 888 '      
    '888    888 Y88b 888 d88P Y88..88P 888     888  888 888  888 Y88b 888 Y8b.           888  T88b  888  Y8bd8P  888  888 888 '      
    '888    888  "Y8888888P"   "Y88P"  888     "Y888888 888  888  "Y88888  "Y8888        888   T88b 888   Y88P   "Y888888 888 '      
    '                                                                 888                                                     '      
    '                                                             Y8b d88P                                                    '
    '                                                              "Y88P"                                                     '
    '+---------------------------------------------------------------------------------------+
    Sub ScanHCR()
        On Error Resume Next
        ListView1.Items.Clear() : Me.Text = "Scanning...." : ComboBox1.Enabled = False : Button1.Enabled = False
        Dim key As Microsoft.Win32.RegistryKey
        Select Case ComboBox1.Text
            Case "HKCU" : key = My.Computer.Registry.CurrentUser.OpenSubKey("SOFTWARE\Classes")
            Case "HKCR" : key = My.Computer.Registry.ClassesRoot
            Case "HKLM" : key = My.Computer.Registry.LocalMachine.OpenSubKey("SOFTWARE\Classes")
            Case "HKU" : key = My.Computer.Registry.Users.OpenSubKey(".DEFAULT\Software\Classes")
            Case "HKCC" : key = My.Computer.Registry.CurrentConfig
        End Select
        Dim Listachiavi() As String = key.GetSubKeyNames
        ProgressBar1.Maximum = Listachiavi.Length - 1
        For Each ValueName In Listachiavi
            ProgressBar1.Value += 1
            Dim key2 As Microsoft.Win32.RegistryKey = My.Computer.Registry.ClassesRoot.OpenSubKey(ValueName, Microsoft.Win32.RegistryKeyPermissionCheck.ReadSubTree, Security.AccessControl.RegistryRights.ReadKey)
            For Each Vm In key2.GetValueNames
                If CheckBox1.Checked Then If My.Resources.SystemProcess.ToString.ToLower.Contains(ValueName.ToLower) Then GoTo SystemEntryName
                If ComboBox1.Text = "HKCC" And ValueName = "System" Then GoTo SystemEntryName
                If Vm.ToUpper.Contains("URL") Then
                    Dim RegURL As String = My.Computer.Registry.ClassesRoot.OpenSubKey(ValueName).GetValue("")
                    If Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ValueName & "\shell\open\command") Is Nothing Then
                        ' Key doesn't exist
                        ListView1.Items.Add(ValueName).SubItems.Add("NO CMD PATH!!! ---> " & Vm & "  " & RegURL)
                    Else
                        Dim RegRead As String = My.Computer.Registry.ClassesRoot.OpenSubKey(ValueName & "\shell\open\command").GetValue("")
                        ListView1.Items.Add(ValueName).SubItems.Add("LoaderKey: ---> " & Vm & "  " & RegURL & " - Command: ---> " & RegRead) 'vvvvvvvvvvvvvvvvvvvvvvvvv
                    End If
                End If
SystemEntryName:
            Next
        Next : ProgressBar1.Value = ProgressBar1.Maximum : Me.Text = "Registry URL Protocol Explorer - Found " & ListView1.Items.Count & " entry's!!!" : ProgressBar1.Value = 0
        ComboBox1.Enabled = True : Button1.Enabled = True
    End Sub

    Private Sub ListView1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListView1.SelectedIndexChanged
        On Error Resume Next
        RichTextBox1.Text = "Reg Key: " & ListView1.Items(ListView1.FocusedItem.Index).Text &
             vbCrLf & ListView1.Items(ListView1.FocusedItem.Index).SubItems(1).Text
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Control.CheckForIllegalCrossThreadCalls = False
        Dim t As New Threading.Thread(AddressOf ScanHCR)
        t.Start()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Control.CheckForIllegalCrossThreadCalls = False
        Dim t As New Threading.Thread(AddressOf ScanHCR)
        t.Start()
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged

        Select Case ComboBox1.Text
            Case "HKCU"
                Label2.Text = "Scan of HKEY CURRENT USER"
            Case "HKCR"
                Label2.Text = "Scan of HKEY CLASSES ROOT"
            Case "HKLM"
                Label2.Text = "Scan of HKEY LOCAL MACHINE"
            Case "HKU"
                Label2.Text = "Scan of HKEY USERS"
            Case "HKCC"
                Label2.Text = "Scan of HKEY CURRENT CONFIG"
        End Select
    End Sub

    Private Sub ExploreToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExploreToolStripMenuItem.Click
        Dim id As Security.Principal.WindowsIdentity = Security.Principal.WindowsIdentity.GetCurrent()
        Dim p As Security.Principal.WindowsPrincipal = New Security.Principal.WindowsPrincipal(id)
        If p.IsInRole(Security.Principal.WindowsBuiltInRole.Administrator) Then

            Dim RegPath As String = "", FolderPath As String = "\"
            If ListView1.SelectedIndices.Count < 1 Then : MsgBox("Select Entry from List!") : Exit Sub
            Else : FolderPath = FolderPath & ListView1.Items(ListView1.FocusedItem.Index).Text : End If

            For Each pxc As Process In Process.GetProcesses 'close all instace of regedit
                If pxc.ProcessName().Contains("regedit") Then pxc.Kill()
            Next
            Select Case ComboBox1.Text
                Case "HKCU" : RegPath = My.Computer.Registry.CurrentUser.ToString & "\SOFTWARE\Classes" & FolderPath
                Case "HKCR" : RegPath = My.Computer.Registry.ClassesRoot.ToString & FolderPath
                Case "HKLM" : RegPath = My.Computer.Registry.LocalMachine.ToString & "\SOFTWARE\Classes" & FolderPath
                Case "HKU" : RegPath = My.Computer.Registry.Users.ToString & "\.DEFAULT\Software\Classes" & FolderPath
                Case "HKCC" : RegPath = My.Computer.Registry.CurrentConfig.ToString & FolderPath
            End Select
            Microsoft.Win32.Registry.SetValue("HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Applets\Regedit", "LastKey", RegPath, Microsoft.Win32.RegistryValueKind.String)
            Process.Start("regedit.exe")
        Else
            MsgBox("if you get an error, you have to run with Admin right!", MsgBoxStyle.Information, Application.ProductName)
            Dim RegPath As String = "", FolderPath As String = "\"
            If ListView1.SelectedIndices.Count < 1 Then : MsgBox("Select Entry from List!") : Exit Sub
            Else : FolderPath = FolderPath & ListView1.Items(ListView1.FocusedItem.Index).Text : End If
            For Each pxc As Process In Process.GetProcesses 'close all instace of regedit
                If pxc.ProcessName().Contains("regedit") Then pxc.Kill()
            Next
            Select Case ComboBox1.Text
                Case "HKCU" : RegPath = My.Computer.Registry.CurrentUser.ToString & "\SOFTWARE\Classes" & FolderPath
                Case "HKCR" : RegPath = My.Computer.Registry.ClassesRoot.ToString & FolderPath
                Case "HKLM" : RegPath = My.Computer.Registry.LocalMachine.ToString & "\SOFTWARE\Classes" & FolderPath
                Case "HKU" : RegPath = My.Computer.Registry.Users.ToString & "\.DEFAULT\Software\Classes" & FolderPath
                Case "HKCC" : RegPath = My.Computer.Registry.CurrentConfig.ToString & FolderPath
            End Select
            Microsoft.Win32.Registry.SetValue("HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Applets\Regedit", "LastKey", RegPath, Microsoft.Win32.RegistryValueKind.String)
            Process.Start("regedit.exe")
        End If
    End Sub

    Private Sub DeleteToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DeleteToolStripMenuItem.Click
        Dim id As Security.Principal.WindowsIdentity = Security.Principal.WindowsIdentity.GetCurrent()
        Dim p As Security.Principal.WindowsPrincipal = New Security.Principal.WindowsPrincipal(id)
        If p.IsInRole(Security.Principal.WindowsBuiltInRole.Administrator) Then
            Dim FolderPath As String = ""
            If ListView1.SelectedIndices.Count < 1 Then : MsgBox("Select Entry from List!") : Exit Sub
            Else : FolderPath = FolderPath & ListView1.Items(ListView1.FocusedItem.Index).Text : End If
            Dim result = MessageBox.Show("Are you sure???", "Delete Entry from List", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            If result = DialogResult.No Then : Exit Sub
            ElseIf result = DialogResult.Yes Then
                Try : Select Case ComboBox1.Text
                        Case "HKCU" : My.Computer.Registry.CurrentUser.DeleteSubKey("SOFTWARE\Classes\" & FolderPath)
                        Case "HKCR" : My.Computer.Registry.ClassesRoot.DeleteSubKey(FolderPath)
                        Case "HKLM" : My.Computer.Registry.LocalMachine.OpenSubKey("SOFTWARE\Classes").DeleteSubKey(FolderPath)
                        Case "HKU" : My.Computer.Registry.Users.DeleteSubKey(".DEFAULT\Software\Classes\" & FolderPath)
                        Case "HKCC" : My.Computer.Registry.CurrentConfig.DeleteSubKey(FolderPath)
                    End Select : Dim t As New Threading.Thread(AddressOf ScanHCR) : t.Start() 'rescann all
                    MsgBox("Key Deleted!!", MsgBoxStyle.Information, Application.ProductName)
                Catch ex As Exception : MsgBox(ex.Message, MsgBoxStyle.Critical, Application.ProductName)
                End Try : Else : Exit Sub : End If
        Else
            MsgBox("if you get an error, you have to run with Admin right!", MsgBoxStyle.Information, Application.ProductName)
            Dim FolderPath As String = ""
            If ListView1.SelectedIndices.Count < 1 Then : MsgBox("Select Entry from List!") : Exit Sub
            Else : FolderPath = FolderPath & ListView1.Items(ListView1.FocusedItem.Index).Text : End If
            Dim result = MessageBox.Show("Are you sure???", "Delete Entry from List", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            If result = DialogResult.No Then : Exit Sub
            ElseIf result = DialogResult.Yes Then
                Try : Select Case ComboBox1.Text
                        Case "HKCU" : My.Computer.Registry.CurrentUser.DeleteSubKey("SOFTWARE\Classes\" & FolderPath)
                        Case "HKCR" : My.Computer.Registry.ClassesRoot.DeleteSubKey(FolderPath)
                        Case "HKLM" : My.Computer.Registry.LocalMachine.OpenSubKey("SOFTWARE\Classes").DeleteSubKey(FolderPath)
                        Case "HKU" : My.Computer.Registry.Users.DeleteSubKey(".DEFAULT\Software\Classes\" & FolderPath)
                        Case "HKCC" : My.Computer.Registry.CurrentConfig.DeleteSubKey(FolderPath)
                    End Select : Dim t As New Threading.Thread(AddressOf ScanHCR) : t.Start() 'rescann all
                    MsgBox("Key Deleted!!", MsgBoxStyle.Information, Application.ProductName)
                Catch ex As Exception : MsgBox(ex.Message, MsgBoxStyle.Critical, Application.ProductName)
                End Try : Else : Exit Sub : End If
        End If
    End Sub

    Private Sub ExportItToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExportItToolStripMenuItem.Click
        MsgBox("work in progress...")
    End Sub
    Public Sub ExportKey(RegKey As String, SavePath As String)
        Dim path As String = (Convert.ToString("""") & SavePath) + """"
        Dim key As String = (Convert.ToString("""") & RegKey) + """"

        Dim proc = New Process()
        Try
            proc.StartInfo.FileName = "regedit.exe"
            proc.StartInfo.UseShellExecute = False
            proc = Process.Start("regedit.exe", (Convert.ToString((Convert.ToString("/e ") & path) + " ") & key) + "")

            If proc IsNot Nothing Then
                proc.WaitForExit()
            End If
        Finally
            If proc IsNot Nothing Then
                proc.Dispose()
            End If
        End Try

    End Sub

End Class
