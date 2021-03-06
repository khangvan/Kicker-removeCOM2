VERSION 5.00
Object = "{5003B5C3-1891-11D1-B6CF-0000C02DDDED}#1.0#0"; "cbotknob.ocx"
Object = "{86CF1D34-0C5F-11D2-A9FC-0000F8754DA1}#2.0#0"; "MSCOMCT2.OCX"
Begin VB.Form frmPS3645A 
   Caption         =   "3645A DC Power Supply"
   ClientHeight    =   4170
   ClientLeft      =   6840
   ClientTop       =   300
   ClientWidth     =   8325
   ControlBox      =   0   'False
   Icon            =   "PS3645A.frx":0000
   LinkTopic       =   "Form1"
   ScaleHeight     =   4170
   ScaleWidth      =   8325
   Begin VB.CommandButton cmdSet5V_1Amp_Max 
      Caption         =   "Set 5.2 V , 1 A Max"
      Height          =   375
      Left            =   6360
      TabIndex        =   32
      Top             =   3720
      Width           =   1695
   End
   Begin VB.TextBox txtStatus 
      Height          =   285
      Left            =   1200
      TabIndex        =   30
      Top             =   3720
      Width           =   4815
   End
   Begin VB.CommandButton cmdReadCurrentValue 
      Caption         =   "Read Current"
      Height          =   495
      Left            =   1200
      TabIndex        =   29
      Top             =   3000
      Width           =   1335
   End
   Begin VB.CommandButton cmdHideMe 
      Caption         =   "Quit"
      Height          =   495
      Left            =   120
      TabIndex        =   28
      Top             =   3000
      Width           =   975
   End
   Begin VB.CommandButton cmdOff 
      Caption         =   "Off"
      Height          =   495
      Left            =   2400
      TabIndex        =   27
      Top             =   2160
      Width           =   855
   End
   Begin VB.CommandButton cmdOn 
      Caption         =   "On"
      Height          =   495
      Left            =   2400
      TabIndex        =   26
      Top             =   1560
      Width           =   855
   End
   Begin VB.CommandButton cmd12V 
      Caption         =   "12 V"
      Height          =   495
      Left            =   2400
      TabIndex        =   25
      Top             =   960
      Width           =   855
   End
   Begin VB.CommandButton cmd5V 
      Caption         =   "5 V"
      Height          =   495
      Left            =   2400
      TabIndex        =   24
      Top             =   360
      Width           =   855
   End
   Begin VB.Timer tmrWaitMilliSecs 
      Left            =   240
      Top             =   360
   End
   Begin VB.Timer tmrUpdateScreen 
      Enabled         =   0   'False
      Left            =   0
      Top             =   2280
   End
   Begin VB.TextBox txtSetVolts 
      Alignment       =   1  'Right Justify
      BackColor       =   &H00000000&
      BeginProperty DataFormat 
         Type            =   1
         Format          =   "0.000"
         HaveTrueFalseNull=   0
         FirstDayOfWeek  =   0
         FirstWeekOfYear =   0
         LCID            =   1033
         SubFormatType   =   1
      EndProperty
      BeginProperty Font 
         Name            =   "Arial"
         Size            =   18
         Charset         =   0
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H0000FF00&
      Height          =   495
      Left            =   660
      TabIndex        =   21
      Text            =   "0.000"
      Top             =   240
      Width           =   1215
   End
   Begin VB.Timer tmrData 
      Enabled         =   0   'False
      Left            =   7440
      Top             =   3000
   End
   Begin VB.CheckBox chkPCcontrol 
      Caption         =   "PC Control"
      Height          =   255
      Left            =   6120
      TabIndex        =   20
      Top             =   3240
      Value           =   1  'Checked
      Width           =   1215
   End
   Begin VB.CheckBox chkPower 
      Caption         =   "Enable Output"
      Height          =   255
      Left            =   6120
      TabIndex        =   6
      Top             =   2880
      Width           =   1335
   End
   Begin VB.PictureBox ctrlComm 
      Height          =   480
      Left            =   4800
      ScaleHeight     =   420
      ScaleWidth      =   1140
      TabIndex        =   23
      Top             =   3000
      Width           =   1200
   End
   Begin VB.CommandButton cmdRead 
      Caption         =   "Read"
      Height          =   495
      Left            =   2760
      TabIndex        =   5
      Top             =   3000
      Width           =   855
   End
   Begin VB.Frame fraMax 
      Caption         =   "Maximums"
      BeginProperty Font 
         Name            =   "MS Sans Serif"
         Size            =   9.75
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   2535
      Left            =   6120
      TabIndex        =   4
      Top             =   240
      Width           =   2055
      Begin VB.TextBox txtMaxWatts 
         Alignment       =   1  'Right Justify
         BeginProperty DataFormat 
            Type            =   1
            Format          =   "0"
            HaveTrueFalseNull=   0
            FirstDayOfWeek  =   0
            FirstWeekOfYear =   0
            LCID            =   1033
            SubFormatType   =   1
         EndProperty
         BeginProperty Font 
            Name            =   "Arial"
            Size            =   12
            Charset         =   0
            Weight          =   400
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         Height          =   390
         Left            =   240
         TabIndex        =   17
         Text            =   "108"
         Top             =   1920
         Width           =   735
      End
      Begin VB.TextBox txtMaxAmps 
         Alignment       =   1  'Right Justify
         BeginProperty DataFormat 
            Type            =   1
            Format          =   "0"
            HaveTrueFalseNull=   0
            FirstDayOfWeek  =   0
            FirstWeekOfYear =   0
            LCID            =   1033
            SubFormatType   =   1
         EndProperty
         BeginProperty Font 
            Name            =   "Arial"
            Size            =   12
            Charset         =   0
            Weight          =   400
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         Height          =   390
         Left            =   240
         TabIndex        =   14
         Text            =   "3.00"
         Top             =   1200
         Width           =   735
      End
      Begin VB.TextBox txtMaxVolts 
         Alignment       =   1  'Right Justify
         BeginProperty DataFormat 
            Type            =   1
            Format          =   "0"
            HaveTrueFalseNull=   0
            FirstDayOfWeek  =   0
            FirstWeekOfYear =   0
            LCID            =   1033
            SubFormatType   =   1
         EndProperty
         BeginProperty Font 
            Name            =   "Arial"
            Size            =   12
            Charset         =   0
            Weight          =   400
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         Height          =   390
         Left            =   120
         TabIndex        =   12
         Text            =   "36.000"
         Top             =   480
         Width           =   855
      End
      Begin MSComCtl2.UpDown spnMaxVolts 
         Height          =   390
         Left            =   960
         TabIndex        =   11
         Top             =   480
         Width           =   240
         _ExtentX        =   423
         _ExtentY        =   688
         _Version        =   393216
         Value           =   36
         OrigLeft        =   960
         OrigTop         =   480
         OrigRight       =   1200
         OrigBottom      =   870
         Max             =   36
         Enabled         =   -1  'True
      End
      Begin MSComCtl2.UpDown spnMaxAmps 
         Height          =   390
         Left            =   960
         TabIndex        =   15
         Top             =   1200
         Width           =   240
         _ExtentX        =   423
         _ExtentY        =   688
         _Version        =   393216
         Value           =   30
         OrigLeft        =   960
         OrigTop         =   1200
         OrigRight       =   1200
         OrigBottom      =   1590
         Max             =   30
         Enabled         =   -1  'True
      End
      Begin MSComCtl2.UpDown spnMaxWatts 
         Height          =   390
         Left            =   960
         TabIndex        =   18
         Top             =   1920
         Width           =   240
         _ExtentX        =   423
         _ExtentY        =   688
         _Version        =   393216
         Value           =   108
         OrigLeft        =   960
         OrigTop         =   1920
         OrigRight       =   1200
         OrigBottom      =   2310
         Max             =   108
         Enabled         =   -1  'True
      End
      Begin VB.Label lblMaxWatts 
         Caption         =   "Watts"
         BeginProperty Font 
            Name            =   "Arial"
            Size            =   12
            Charset         =   0
            Weight          =   400
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         Height          =   255
         Left            =   1320
         TabIndex        =   19
         Top             =   2040
         Width           =   615
      End
      Begin VB.Label lblMaxAmps 
         Caption         =   "Amps"
         BeginProperty Font 
            Name            =   "Arial"
            Size            =   12
            Charset         =   0
            Weight          =   400
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         Height          =   255
         Left            =   1320
         TabIndex        =   16
         Top             =   1320
         Width           =   615
      End
      Begin VB.Label lblMaxVolts 
         Caption         =   "Volts"
         BeginProperty Font 
            Name            =   "Arial"
            Size            =   12
            Charset         =   0
            Weight          =   400
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         Height          =   255
         Left            =   1320
         TabIndex        =   13
         Top             =   600
         Width           =   615
      End
   End
   Begin VB.CommandButton cmdSettings 
      Caption         =   "Set Values"
      Height          =   495
      Left            =   3720
      TabIndex        =   3
      Top             =   3000
      Width           =   975
   End
   Begin VB.Frame fraCurrent 
      Caption         =   "Current Values"
      BeginProperty Font 
         Name            =   "MS Sans Serif"
         Size            =   9.75
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   2535
      Left            =   3480
      TabIndex        =   0
      Top             =   240
      Width           =   2415
      Begin VB.TextBox txtCurWatts 
         Alignment       =   1  'Right Justify
         BackColor       =   &H00000000&
         BeginProperty DataFormat 
            Type            =   1
            Format          =   "0.000"
            HaveTrueFalseNull=   0
            FirstDayOfWeek  =   0
            FirstWeekOfYear =   0
            LCID            =   1033
            SubFormatType   =   1
         EndProperty
         BeginProperty Font 
            Name            =   "Arial"
            Size            =   18
            Charset         =   0
            Weight          =   700
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         ForeColor       =   &H0000FF00&
         Height          =   495
         Left            =   240
         TabIndex        =   8
         Text            =   "0.00"
         Top             =   1920
         Width           =   1095
      End
      Begin VB.TextBox txtCurAmps 
         Alignment       =   1  'Right Justify
         BackColor       =   &H00000000&
         BeginProperty DataFormat 
            Type            =   1
            Format          =   "0.000"
            HaveTrueFalseNull=   0
            FirstDayOfWeek  =   0
            FirstWeekOfYear =   0
            LCID            =   1033
            SubFormatType   =   1
         EndProperty
         BeginProperty Font 
            Name            =   "Arial"
            Size            =   18
            Charset         =   0
            Weight          =   700
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         ForeColor       =   &H0000FF00&
         Height          =   495
         Left            =   240
         TabIndex        =   7
         Text            =   "0.00"
         Top             =   1200
         Width           =   1095
      End
      Begin VB.TextBox txtCurVolts 
         Alignment       =   1  'Right Justify
         BackColor       =   &H00000000&
         BeginProperty DataFormat 
            Type            =   1
            Format          =   "0.000"
            HaveTrueFalseNull=   0
            FirstDayOfWeek  =   0
            FirstWeekOfYear =   0
            LCID            =   1033
            SubFormatType   =   1
         EndProperty
         BeginProperty Font 
            Name            =   "Arial"
            Size            =   18
            Charset         =   0
            Weight          =   700
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         ForeColor       =   &H0000FF00&
         Height          =   495
         Left            =   120
         TabIndex        =   2
         Text            =   "0.000"
         Top             =   480
         Width           =   1215
      End
      Begin VB.Label lblCurWatts 
         Caption         =   "Watts"
         BeginProperty Font 
            Name            =   "Arial"
            Size            =   14.25
            Charset         =   0
            Weight          =   400
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         Height          =   375
         Left            =   1440
         TabIndex        =   10
         Top             =   2040
         Width           =   735
      End
      Begin VB.Label lblCurAmps 
         Caption         =   "Amps"
         BeginProperty Font 
            Name            =   "Arial"
            Size            =   14.25
            Charset         =   0
            Weight          =   400
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         Height          =   375
         Left            =   1440
         TabIndex        =   9
         Top             =   1320
         Width           =   735
      End
      Begin VB.Label lblCurVolts 
         Caption         =   "Volts"
         BeginProperty Font 
            Name            =   "Arial"
            Size            =   14.25
            Charset         =   0
            Weight          =   400
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         Height          =   375
         Left            =   1440
         TabIndex        =   1
         Top             =   600
         Width           =   735
      End
   End
   Begin CBOTKNOBLib.CbotKnob knobSet 
      Height          =   1815
      Left            =   360
      TabIndex        =   22
      Top             =   840
      Width           =   1815
      _Version        =   65536
      _ExtentX        =   3201
      _ExtentY        =   3201
      _StockProps     =   29
      Text            =   "Volts"
      BeginProperty Font {0BE35203-8F91-11CE-9DE3-00AA004BB851} 
         Name            =   "MS Sans Serif"
         Size            =   9.75
         Charset         =   0
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      KnobColor       =   12632064
      IndicatorColor  =   65280
      RimColor        =   8388608
      Max             =   360
      Wrap            =   0   'False
      IndicatorStyle  =   2
      IndicatorLineWidth=   3
      Appearence      =   1
      Appearence3DHeight=   4
      TickLength      =   0
      TickGap         =   0
      TickColor       =   4210752
      FineAdjust      =   -1  'True
      FineAdjustWidth =   20
      FineAdjustTicks =   100
      FineAdjustColor =   8421376
      FineAdjustLineLength=   6
   End
   Begin VB.Label Label1 
      Caption         =   "Status"
      BeginProperty Font 
         Name            =   "MS Sans Serif"
         Size            =   9.75
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   255
      Left            =   480
      TabIndex        =   31
      Top             =   3720
      Width           =   615
   End
End
Attribute VB_Name = "frmPS3645A"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False

Option Explicit


Dim strErr As String
Dim intPortNumber As Integer
Dim posFine As Integer
Dim bKnobChange As Boolean
Dim bSetControl As Boolean ' used to stop control event handler
Dim bWaitingForData As Boolean
Public WithEvents ctrlComm As MSCommLib.MSComm
Attribute ctrlComm.VB_VarHelpID = -1
'Private Declare Sub Sleep Lib "kernel32" (ByVal dwMilliseconds As Long)

' -------------------------------------------------------------
' some useful functions
' -------------------------------------------------------------

Private Sub TransmitPacket()
    If ctrlComm.PortOpen Then
        ctrlComm.Output = sendPacket
    Else
        MsgBox ("Serial port not opened")
    End If
End Sub

Private Sub cmdReadCurrentValue_Click()
    Dim dblTrash As Double
    ReadCurrent dblTrash
End Sub

Public Function ReadCurrent(ByRef dblCurrent As Double) As String
On Error Resume Next

    Dim i As Integer
    txtStatus = ""
    
    i = 0
    Do While (bWaitingForData = True) And (i <= 10) '5 seconds
        WaitMilliSecs 500
        DoEvents
        i = i + 1
    Loop
    
    If bWaitingForData Then
        ReadCurrent = "Error Power Supply is busy, try again later."
        txtStatus = "Error Power Supply is busy, try again later."
        Exit Function
    End If
    
    ReadCurrent = "OK"
    ReadParams
    
    i = 0
    Do While (tmrData.Enabled = True) And (i <= 25) '5 seconds
        WaitMilliSecs 200
        DoEvents
        i = i + 1
    Loop
    
    If i >= 25 Then
        ReadCurrent = "Error Power Supply reading power supply current"
        txtStatus = "Error Power Supply reading power supply current"
        Exit Function
    End If
    
    txtStatus = "OK"
    dblCurrent = curAmps

    
End Function

Private Sub ReadParams()
On Error Resume Next
    If bWaitingForData Then Exit Sub
    
    bWaitingForData = True
    
    SetReadPacket
    ' clear input data
    ctrlComm.InputLen = 0
    readPacket = ctrlComm.Input
    ctrlComm.RThreshold = 0
    TransmitPacket
    
    ' start waiting for data timer
    tmrData.Enabled = True
    tmrData.Interval = 500 ' wait before reading serial port data
    
End Sub



Private Sub cmdOn_Click()
On Error Resume Next
    PowerOn
End Sub

Public Sub PowerOn()
    Dim bPC As Boolean
    Dim i As Integer
        
    i = 0
    Do While (bWaitingForData = True) And (i <= 10) '5 seconds
        WaitMilliSecs 500
        DoEvents
        i = i + 1
    Loop
    
    chkPCcontrol.Value = vbChecked
    bPC = True
    
    SetControlPacket bPC, True
    TransmitPacket
    DoEvents
    
    intUpdateCycles = 6
    tmrUpdateScreen.Interval = 1000
    tmrUpdateScreen.Enabled = True

End Sub

Private Sub cmdOff_Click()
On Error Resume Next
    PowerOff
End Sub

Public Sub PowerOff()
    Dim bPC As Boolean
    Dim i As Integer
        
    i = 0
    Do While (bWaitingForData = True) And (i <= 10) '5 seconds
        WaitMilliSecs 500
        DoEvents
        i = i + 1
    Loop
            
    chkPCcontrol.Value = vbChecked
    bPC = True
    
    SetControlPacket bPC, False
    TransmitPacket
    DoEvents
    
    intUpdateCycles = 6
    tmrUpdateScreen.Interval = 1000
    tmrUpdateScreen.Enabled = True
End Sub


Private Sub SetPowerOn()
    Dim bPC As Boolean
        
    If chkPCcontrol.Value = vbChecked Then
        bPC = True
    Else
        bPC = False
    End If
    
    SetControlPacket bPC, True
    TransmitPacket
    DoEvents
    
    If bPC Then
        MousePointer = vbHourglass
        WaitMilliSecs (4000)
        ReadParams
    End If
End Sub

Private Sub SetPowerOff()
    Dim bPC As Boolean
            
    If chkPCcontrol.Value = vbChecked Then
        bPC = True
    Else
        bPC = False
    End If
    
    SetControlPacket bPC, False
    TransmitPacket
    DoEvents
    
    If bPC Then
        MousePointer = vbHourglass
        WaitMilliSecs (6000)
        ReadParams
    End If
End Sub

Private Sub cmd12V_Click()
On Error Resume Next
    SetVoltage 12#
End Sub

Private Sub cmd5V_Click()
On Error Resume Next
    SetVoltage 5#
End Sub

Private Sub cmdHideMe_Click()
    Unload Me
   'Hide.me
End Sub








' ----------------------------------------------------
' start of form event handlers
' ----------------------------------------------------

Private Sub Form_Load()
    
'    If (Trace) Then WriteTrace TrHIGH, "frmPS3645A", _
    "Form_Load"
    On Error GoTo ErrCondition

    
    curVolts = 0
    setVolts = 0
    maxVolts = 36
        
    curAmps = 0
    maxAmps = 3
    
    curWatts = 0
    maxWatts = 108
    
    posFine = 0
    bKnobChange = False
    bSetControl = False
    bWaitingForData = False
        
    Set ctrlComm = New MSCommLib.MSComm

    intPortNumber = 6 ' thay doi port 6
    ctrlComm.CommPort = intPortNumber
    ctrlComm.RThreshold = 0
    ctrlComm.InputMode = comInputModeBinary
    ctrlComm.PortOpen = True
    
    WaitMilliSecs (10)
    SetControlPacket True, False
    TransmitPacket
    WaitMilliSecs (100)
    
    Set_Voltage_and_Current 0#, 3#    ' amps
    
    'mode open com 6 and quit form
    On Error Resume Next
    SetVoltage 5#
   
    PowerOn
    Unload Me
   'Hide.me
    
    GoTo Bottom
ErrCondition:
    MsgBox "frmPS3645A Form_Load: " & Err.Description & "   Port# = " & _
    intPortNumber & "   Err# = " & Err.Number & _
    " " & Err.Source
       WaitMilliSecs (1000)
    
    Unload Me
Bottom:
End Sub

Private Sub Form_Unload(Cancel As Integer)
On Error Resume Next
    SetControlPacket True, False
    TransmitPacket
    WaitMilliSecs (100)
    SetControlPacket False, False
    TransmitPacket
    WaitMilliSecs (100)
    ctrlComm.PortOpen = False
End Sub

Private Sub chkPCcontrol_Click()
    Dim bPwrOn As Boolean
    Dim bPC As Boolean
    
    If bSetControl Then Exit Sub
    
    If chkPCcontrol.Value = vbChecked Then
        bPC = True
    Else
        bPC = False
    End If
    
    If chkPower.Value = vbChecked Then
        bPwrOn = True
    Else
        bPwrOn = False
    End If
    
    SetControlPacket bPC, bPwrOn
    TransmitPacket
End Sub

Private Sub chkPower_Click()
    If bSetControl Then Exit Sub
    
    If chkPower.Value = vbChecked Then
        SetPowerOn
    Else
        SetPowerOff
    End If
End Sub

Private Sub cmdRead_Click()
    ReadParams
End Sub

Private Sub cmdSettings_Click()
    MousePointer = vbHourglass
    commandValue = CmdSetParams
    SetPacketValues maxAmps, maxVolts, maxWatts, setVolts
    TransmitPacket
    WaitMilliSecs (500)
    ReadParams
End Sub

Private Sub cmdSet5V_1Amp_Max_Click()
    Set_Voltage_and_Current 5.2, 1#
End Sub

Public Sub Set_Voltage_and_Current(Set_Volts As Double, Max_Amps As Double)
    setVolts = Set_Volts 'Set Global Variables
    maxAmps = Max_Amps
    MousePointer = vbHourglass
    commandValue = CmdSetParams
    SetPacketValues maxAmps, maxVolts, maxWatts, setVolts
    TransmitPacket
    WaitMilliSecs (500)
    ReadParams
    WaitMilliSecs 500
End Sub

Public Function SetVoltage(dblVolts As Double) As String
On Error Resume Next
    Dim dVolts As Double
    
    SetVoltage = "OK"
    If bSetControl Then
        SetVoltage = "Error power supply is was already being adjusted."
        Exit Function
    End If
    
    dVolts = dblVolts
    ' Limit voltage -----------------
    If dVolts > 36 Then
        dVolts = 36
        txtSetVolts.Text = "36.000"
    End If
    
    If dVolts < 0 Then
        dVolts = 0
        txtSetVolts.Text = "0.000"
    End If
    
    ' Set Global Voltage variable
    setVolts = dVolts
    
    ' Set the power supply to the new value
    commandValue = CmdSetParams
    SetPacketValues maxAmps, maxVolts, maxWatts, setVolts
    TransmitPacket
    
    ' Update the Text box
    bKnobChange = True
    txtSetVolts.Text = Format(setVolts, "0.000")
    DoEvents
    bKnobChange = False
    
    ' --- Read back values from the power supply
    intUpdateCycles = 5
    tmrUpdateScreen.Interval = 1000
    tmrUpdateScreen.Enabled = True
    
    WaitMilliSecs 300 'In case more commands like power on come later
    
End Function

Private Sub knobSet_PositionChanged(ByVal delta As Integer)
    Dim nChange As Double
    Dim dDelta As Double
    
    If bSetControl Then Exit Sub
    
    bKnobChange = True
    dDelta = delta
    
    If knobSet.FineAdjustPosition <> posFine Then
        nChange = dDelta / 1000
        posFine = knobSet.FineAdjustPosition
    Else
        nChange = dDelta / 10
    End If
        
    setVolts = setVolts + nChange
    
    If setVolts < 0 Then
        setVolts = 0
    End If
    
    If setVolts > maxVolts Then
        setVolts = maxVolts
    End If
    
    txtSetVolts.Text = Format(setVolts, "0.000")
    
    DoEvents
    bKnobChange = False
    
End Sub

Private Sub spnMaxAmps_Change()
    Dim nVal As Double
    If bSetControl Then Exit Sub
    
    nVal = spnMaxAmps.Value / 10
    txtMaxAmps.Text = Format(nVal, "0.00")
End Sub


Private Sub tmrData_Timer()
    tmrData.Enabled = False
    tmrData.Interval = 0
    
    ' check for input data and process
    If ctrlComm.InBufferCount < packetSize Then
        bWaitingForData = False
        MousePointer = vbDefault
        Exit Sub
    End If
    
    If ctrlComm.InBufferCount > packetSize Then
        bWaitingForData = False
        MousePointer = vbDefault
        Exit Sub
    End If
    
    readPacket = ctrlComm.Input
    GetPacketValues ' process input packet
    bSetControl = True
        
    If gbPwrOn Then
        chkPower.Value = vbChecked
    Else
        chkPower.Value = vbUnchecked
    End If
    
    If gbPCcontrol Then
        txtCurAmps.Text = Format(curAmps, "0.00")
        txtCurWatts.Text = Format(curWatts, "0.0")
        txtCurVolts.Text = Format(curVolts, "0.000")
        
        txtMaxAmps.Text = Format(maxAmps, "0.00")
        txtMaxVolts.Text = Format(maxVolts, "0.000")
        txtMaxWatts.Text = Format(maxWatts, "0.0")
        
        If maxAmps * 10 < spnMaxAmps.Max Then
            spnMaxAmps.Value = maxAmps * 10
        Else
            spnMaxAmps.Value = 30
        End If
         
        txtSetVolts.Text = Format(setVolts, "0.000")
        knobSet.Position = (setVolts * 10) Mod 360
        knobSet.FineAdjustPosition = 0
    Else
        MsgBox "PC does not have control"
    End If
    
    If gbOverCurrent Or gbOverPower Then
        If gbOverCurrent And gbOverPower Then
            MsgBox "Over current and over power condition"
        ElseIf gbOverCurrent Then
            MsgBox "Over current condition"
        ElseIf gbOverPower Then
            MsgBox "Over power condition"
        End If
    End If
    
    DoEvents
    bSetControl = False
    bWaitingForData = False
    MousePointer = vbDefault
End Sub

Private Sub tmrUpdateScreen_Timer()
    intUpdateCycles = intUpdateCycles - 1
    If intUpdateCycles <= 0 Then
        tmrUpdateScreen.Enabled = False
    End If
    ReadParams
End Sub


Private Sub txtMaxAmps_Change()
    Dim dAmps As Double
       
    If bSetControl Then Exit Sub
       
    ' check for valid entry
    dAmps = Val(txtMaxAmps.Text)
    
    If dAmps > 3 Then
        dAmps = 3
        txtMaxAmps.Text = "3.00"
    End If
    
    If dAmps < 0 Then
        dAmps = 0
        txtMaxAmps.Text = "0.00"
    End If
    
    maxAmps = dAmps
    
    ' calculate correct power
    maxWatts = maxVolts * maxAmps
    bSetControl = True
    txtMaxWatts.Text = Format(maxWatts, "0.0")
    
    ' update spinner
    spnMaxAmps.Value = maxAmps * 10
    DoEvents
    bSetControl = False
End Sub

Private Sub txtMaxVolts_Change()
    Dim dVolts As Double
            
    If bSetControl Then Exit Sub
            
    ' check for valid entry
    dVolts = Val(txtMaxVolts.Text)
    
    If dVolts > 36 Then
        dVolts = 36
        txtMaxVolts.Text = "36.000"
    End If
    
    If dVolts < 0 Then
        dVolts = 0
        txtMaxVolts.Text = "0.000"
    End If
        
    maxVolts = dVolts
    
    ' calculate correct power
    maxWatts = maxVolts * maxAmps
    bSetControl = True
    txtMaxWatts.Text = Format(maxWatts, "0.0")
    DoEvents
    bSetControl = False
End Sub

Private Sub txtMaxWatts_Change()
    Dim dWatts As Double
    
    If bSetControl Then Exit Sub
    
    ' check for valid entry
    dWatts = Val(txtMaxWatts.Text)
    
    If dWatts > 108 Then
        dWatts = 108
        txtMaxWatts.Text = "108"
    End If
    
    If dWatts < 0 Then
        dWatts = 0
        txtMaxWatts.Text = "0.00"
    End If
    
    maxWatts = dWatts
    
    ' calculate correct current
    maxAmps = maxWatts / maxVolts
    bSetControl = True
    txtMaxAmps.Text = Format(maxAmps, "0.00")
    spnMaxAmps.Value = maxAmps * 10
    DoEvents
    bSetControl = False
End Sub

Private Sub txtSetVolts_Change()
    Dim dVolts As Double
    
    If bKnobChange Then Exit Sub
    If bSetControl Then Exit Sub
    
    dVolts = Val(txtSetVolts.Text)
    
    If dVolts > 36 Then
        dVolts = 36
        txtSetVolts.Text = "36.000"
    End If
    
    If dVolts < 0 Then
        dVolts = 0
        txtSetVolts.Text = "0.000"
    End If
    
    setVolts = dVolts
    
    knobSet.Position = (setVolts * 10) Mod 360
    knobSet.FineAdjustPosition = 0
End Sub


Public Sub WaitMilliSecs(intWaitTime As Integer)
'Delay Milliseconds
'
'    If (Trace) Then WriteTrace TrMED, strFormName, "Sub WaitMilliSecs"
On Error GoTo ErrCondition

    tmrWaitMilliSecs.Interval = intWaitTime
    tmrWaitMilliSecs.Enabled = True
    Do While (tmrWaitMilliSecs.Enabled = True)
        DoEvents
    Loop

    GoTo Bottom
ErrCondition:
'    ErrorHandler Me.Tag, True, strFormName, "WaitMilliSecs"
Bottom:
End Sub

Private Sub tmrWaitMilliSecs_Timer()
    tmrWaitMilliSecs.Enabled = False
End Sub

