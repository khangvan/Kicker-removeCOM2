Attribute VB_Name = "packet"
Option Explicit

' packet handling functions

Const deviceAddress As Byte = &H1 ' power supply address
Const SOP As Byte = &HAA ' start of packet byte

' commands
Public Const CmdSetParams As Byte = &H80
Public Const CmdReadParams As Byte = &H81
Public Const CmdONOFF As Byte = &H82
Public Const CmdSetProtection As Byte = &H83
Public Const CmdGetProtection As Byte = &H84
Public Const CmdDemarcatePwr As Byte = &H85
Public Const CmdGetVoltage As Byte = &H86
Public Const CmdDemarcateCurrent As Byte = &H87
Public Const CmdReadVoltage As Byte = &H88
Public Const CmdSetDemarcate As Byte = &H89
Public Const CmdGetDemarcate As Byte = &H8A
Public Const CmdSetSerialNum As Byte = &H8B
Public Const CmdGetSerialNum As Byte = &H8C
Public Const CmdResponseCheck As Byte = &H12 ' sometimes response to &H82 (104 bytes)

' these are 1 based indexes
' variant sourced array has 0 based indexing
Const ControlIndex As Integer = 4
Const AddressIndex As Integer = 16 ' new device address in setparams packet
Const StatusIndex As Integer = 24

' control byte bit masks
Const MaskONOFF As Byte = &H1
Const MaskPC As Byte = &H2

' status byte bit masks
Const MaskStatusONOFF As Byte = &H1
Const MaskStatusOverAmps As Byte = &H2
Const MaskStatusOverPower As Byte = &H4
Const MaskStatusPC As Byte = &H8

Public Const packetSize As Integer = 26

Public sendPacket(1 To 26) As Byte
Public readPacket As Variant ' variant input data byte array has 0 based indexing

Public commandValue As Byte
Public checkValue As Integer

Public gbPCcontrol As Boolean
Public gbPwrOn As Boolean
Public gbOverCurrent As Boolean
Public gbOverPower As Boolean

Public maxVolts As Double
Public curVolts As Double
Public setVolts As Double
Public maxAmps As Double
Public curAmps As Double
Public maxWatts As Double
Public curWatts As Double
Public intUpdateCycles As Integer
Public blnValidCurrentReading As Boolean

Public Sub SetControlPacket(bPCcontrol As Boolean, bPwrOn As Boolean)
    Dim ndx As Integer
    Dim nVal As Byte
    
    commandValue = CmdONOFF
       
    sendPacket(1) = SOP
    sendPacket(2) = deviceAddress
    sendPacket(3) = commandValue
    
    checkValue = SOP
    checkValue = checkValue + deviceAddress
    checkValue = checkValue + commandValue
            
    nVal = 0
    
    If bPCcontrol Then
        nVal = MaskPC
    End If
        
    If bPwrOn Then
        nVal = nVal Or MaskONOFF
    End If
        
    sendPacket(4) = nVal
    checkValue = checkValue + nVal
    
    For ndx = 5 To 25
        sendPacket(ndx) = 0
    Next ndx

    sendPacket(26) = CByte(checkValue And &HFF)
End Sub

Public Sub SetReadPacket()
    Dim ndx As Integer
       
    commandValue = CmdReadParams
       
    sendPacket(1) = SOP
    sendPacket(2) = deviceAddress
    sendPacket(3) = commandValue
    
    checkValue = SOP
    checkValue = checkValue + deviceAddress
    checkValue = checkValue + commandValue
                
    For ndx = 4 To 25
        sendPacket(ndx) = 0
    Next ndx

    sendPacket(26) = CByte(checkValue And &HFF)
End Sub

Public Sub SetPacketValues(maxAmps As Double, maxVolts As Double, maxWatts As Double, setVolts As Double)
    Dim ndx As Integer
    Dim nVal As Byte
    Dim lVal As Long
            
    sendPacket(1) = SOP
    sendPacket(2) = deviceAddress
    sendPacket(3) = commandValue
    
    checkValue = SOP
    checkValue = checkValue + deviceAddress
    checkValue = checkValue + commandValue
    ndx = 4
     
    ' make byte values from sub parameters
    ' max current
    lVal = maxAmps * 1000 ' convert to milliamps
    nVal = lVal And &HFF ' low byte
    sendPacket(ndx) = nVal
    ndx = ndx + 1
    checkValue = checkValue + nVal
    nVal = ((lVal And &HFF00) / &H100) And &HFF
    sendPacket(ndx) = nVal ' high byte
    ndx = ndx + 1
    checkValue = checkValue + nVal
    
    ' max voltage
    lVal = maxVolts * 1000 ' convert to millivolts
    nVal = lVal And &HFF ' low byte of low word
    sendPacket(ndx) = nVal
    ndx = ndx + 1
    checkValue = checkValue + nVal
    nVal = ((lVal And &HFF00) / &H100) And &HFF ' high byte of low word
    sendPacket(ndx) = nVal
    ndx = ndx + 1
    checkValue = checkValue + nVal
    nVal = ((lVal And &HFF0000) / &H10000) And &HFF ' low byte of high word
    sendPacket(ndx) = nVal
    ndx = ndx + 1
    checkValue = checkValue + nVal
    nVal = ((lVal And &HFF000000) / &H1000000) And &HFF ' high byte of high word
    sendPacket(ndx) = nVal
    ndx = ndx + 1
    checkValue = checkValue + nVal
    
    ' max power
    lVal = maxWatts * 100 ' adjust for packet
    nVal = lVal And &HFF ' low byte
    sendPacket(ndx) = nVal
    ndx = ndx + 1
    checkValue = checkValue + nVal
    nVal = ((lVal And &HFF00) / &H100) And &HFF
    sendPacket(ndx) = nVal ' high byte
    ndx = ndx + 1
    checkValue = checkValue + nVal
    
     ' set voltage
    lVal = setVolts * 1000 ' convert to millivolts
    nVal = lVal And &HFF ' low byte of low word
    sendPacket(ndx) = nVal
    ndx = ndx + 1
    checkValue = checkValue + nVal
    nVal = ((lVal And &HFF00) / &H100) And &HFF ' high byte of low word
    sendPacket(ndx) = nVal
    ndx = ndx + 1
    checkValue = checkValue + nVal
    nVal = ((lVal And &HFF0000) / &H10000) And &HFF ' low byte of high word
    sendPacket(ndx) = nVal
    ndx = ndx + 1
    checkValue = checkValue + nVal
    nVal = ((lVal And &HFF000000) / &H1000000) And &HFF ' high byte of high word
    sendPacket(ndx) = nVal
    ndx = ndx + 1
    checkValue = checkValue + nVal
    
    sendPacket(AddressIndex) = deviceAddress
    checkValue = checkValue + deviceAddress
    
    For ndx = AddressIndex + 1 To 25
        sendPacket(ndx) = 0
    Next ndx
    
    sendPacket(26) = CByte(checkValue And &HFF)
End Sub

' GetPacketValues processes response form read command &H81
Public Sub GetPacketValues()
    Dim nStatus As Byte
    Dim nRes As Byte
    Dim dVal As Double
    Dim dLoVal As Double
    Dim dHiVal As Double
    Dim strMsg As String
    Dim ndx As Integer
    
    nRes = readPacket(0)
    
    If nRes <> SOP Then
        'MsgBox "Wrong start of packet marker"
        Exit Sub
    End If
    
    nRes = readPacket(1)
    
    If nRes <> deviceAddress Then
        'MsgBox "Wrong device address"
        Exit Sub
    End If
        
    nRes = readPacket(2)
    
    If nRes <> CmdReadParams Then
        'MsgBox "Wrong command in return packet"
        Exit Sub
    End If
    
    ' TBD : check check digit
    
    ndx = 3
    ' get current amps
    dLoVal = readPacket(ndx) ' low byte
    ndx = ndx + 1
    dHiVal = readPacket(ndx) ' high byte
    ndx = ndx + 1
    dVal = dLoVal + (dHiVal * &H100)
    curAmps = dVal / 1000 ' convert to amps
    blnValidCurrentReading = True
    
    ' get current volts
    dLoVal = readPacket(ndx) ' low byte of low word
    ndx = ndx + 1
    dHiVal = readPacket(ndx)  ' high byte of low word
    ndx = ndx + 1
    dVal = dLoVal + (dHiVal * &H100)
    dLoVal = readPacket(ndx) ' low byte of high word
    ndx = ndx + 1
    dHiVal = readPacket(ndx)  ' high byte of high word
    ndx = ndx + 1
    dVal = dVal + (dLoVal * &H10000) + (dHiVal * &H1000000)
    curVolts = dVal / 1000 ' convert to volts
        
    ' get current power
    dLoVal = readPacket(ndx) ' low byte
    ndx = ndx + 1
    dHiVal = readPacket(ndx) ' high byte
    ndx = ndx + 1
    dVal = dLoVal + (dHiVal * &H100)
    curWatts = dVal / 100 ' convert to watts
        
    ' get max amps
    dLoVal = readPacket(ndx) ' low byte
    ndx = ndx + 1
    dHiVal = readPacket(ndx) ' high byte
    ndx = ndx + 1
    dVal = dLoVal + (dHiVal * &H100)
    maxAmps = dVal / 1000 ' convert to amps
        
    ' get max volts
    dLoVal = readPacket(ndx) ' low byte of low word
    ndx = ndx + 1
    dHiVal = readPacket(ndx)  ' high byte of low word
    ndx = ndx + 1
    dVal = dLoVal + (dHiVal * &H100)
    dLoVal = readPacket(ndx) ' low byte of high word
    ndx = ndx + 1
    dHiVal = readPacket(ndx)  ' high byte of high word
    ndx = ndx + 1
    dVal = dVal + (dLoVal * &H10000) + (dHiVal * &H1000000)
    maxVolts = dVal / 1000 ' convert to volts
    
    ' get max power
    dLoVal = readPacket(ndx) ' low byte
    ndx = ndx + 1
    dHiVal = readPacket(ndx) ' high byte
    ndx = ndx + 1
    dVal = dLoVal + (dHiVal * &H100)
    maxWatts = dVal / 100 ' convert to watts
    
    ' get voltage setting
    dLoVal = readPacket(ndx) ' low byte of low word
    ndx = ndx + 1
    dHiVal = readPacket(ndx)  ' high byte of low word
    ndx = ndx + 1
    dVal = dLoVal + (dHiVal * &H100)
    dLoVal = readPacket(ndx) ' low byte of high word
    ndx = ndx + 1
    dHiVal = readPacket(ndx)  ' high byte of high word
    ndx = ndx + 1
    dVal = dVal + (dLoVal * &H10000) + (dHiVal * &H1000000)
    setVolts = dVal / 1000 ' convert to volts
    
    ' use globals to set status information
    nStatus = readPacket(StatusIndex - 1)

    gbPCcontrol = False
    gbPwrOn = False
    gbOverCurrent = False
    gbOverPower = False
        
    nRes = nStatus And MaskStatusONOFF
    If nRes > 0 Then gbPwrOn = True
    
    nRes = nStatus And MaskStatusOverAmps
    If nRes > 0 Then
        gbOverCurrent = True
    End If
    
    nRes = nStatus And MaskStatusOverPower
    If nRes > 0 Then gbOverPower = True
    
    nRes = nStatus And MaskStatusPC
    If nRes > 0 Then gbPCcontrol = True
    
End Sub
