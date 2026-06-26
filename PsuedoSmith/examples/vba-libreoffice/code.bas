' ============================================================
' TARGET_LANGUAGE    : VBA (LibreOffice Basic)
' TARGET_PLATFORM    : linux
' TARGET_UI_FRAMEWORK: LibreOffice Basic Dialogs
' NAMING_CONVENTIONS : PascalCase
' INDENTATION        : spaces:4
' DELIVERY_MODE      : inline
' ============================================================
Option Explicit

' Resolved (Step 3.3): ISO/IEC 7812 valid length range = 13..19 digits.
Const MIN_CARD_LEN As Integer = 13
Const MAX_CARD_LEN As Integer = 19

Function ValidateCardNumber(ByVal CardNumberString As String) As Boolean
    Dim CleanString As String
    Dim ReversedDigits As String
    Dim TotalSum As Integer
    Dim CurrentDigit As Integer
    Dim DoubledDigit As Integer
    Dim i As Integer
    Dim Pos As Integer
    Dim Ch As String

    CleanString = Replace(CardNumberString, " ", "")
    CleanString = Replace(CleanString, "-", "")

    ' Defensive guard: digits only
    If Len(CleanString) = 0 Then
        ValidateCardNumber = False
        Exit Function
    End If
    For i = 1 To Len(CleanString)
        Ch = Mid(CleanString, i, 1)
        If Ch < "0" Or Ch > "9" Then
            ValidateCardNumber = False
            Exit Function
        End If
    Next i

    ' Build reversed string
    ReversedDigits = ""
    For i = Len(CleanString) To 1 Step -1
        ReversedDigits = ReversedDigits & Mid(CleanString, i, 1)
    Next i

    TotalSum = 0
    For Pos = 0 To Len(ReversedDigits) - 1
        CurrentDigit = CInt(Mid(ReversedDigits, Pos + 1, 1))
        If (Pos Mod 2) = 1 Then
            DoubledDigit = CurrentDigit * 2
            If DoubledDigit > 9 Then DoubledDigit = DoubledDigit - 9
            TotalSum = TotalSum + DoubledDigit
        Else
            TotalSum = TotalSum + CurrentDigit
        End If
    Next Pos

    ValidateCardNumber = ((TotalSum Mod 10) = 0)
End Function

Function DigitsOnly(ByVal Source As String) As String
    Dim i As Integer
    Dim Ch As String
    Dim Result As String
    Result = ""
    For i = 1 To Len(Source)
        Ch = Mid(Source, i, 1)
        If Ch >= "0" And Ch <= "9" Then Result = Result & Ch
    Next i
    DigitsOnly = Result
End Function

Sub ShowMessage(ByVal MessageText As String, ByVal TitleText As String)
    MsgBox MessageText, 64, TitleText
End Sub

Sub ShowValidatorWindow()
    ' Loads dialog "ccDialog" from the Standard library (built in the Dialog Editor).
    ' Expected controls: CardInput (TextField), ValidateBtn, ExitBtn.
    Dim oDialog As Object
    DialogLibraries.LoadLibrary("Standard")
    oDialog = CreateUnoDialog(DialogLibraries.Standard.ccDialog)
    oDialog.Execute()
    oDialog.dispose()
End Sub

' Assign to ValidateBtn -> Events -> "Execute action" in the Dialog Editor.
Sub OnValidateClicked(oEvent As Object)
    Dim oDialog As Object
    Dim CardNumber As String
    oDialog = oEvent.Source.Context
    CardNumber = DigitsOnly(Trim(oDialog.getControl("CardInput").Text))

    If Len(CardNumber) = 0 Then
        ShowMessage "Please enter a credit card number.", "Input Error"
        Exit Sub
    End If
    If Len(CardNumber) < MIN_CARD_LEN Or Len(CardNumber) > MAX_CARD_LEN Then
        ShowMessage "Invalid credit card number", "Input Error"
        Exit Sub
    End If
    If ValidateCardNumber(CardNumber) Then
        ShowMessage "The credit card number is VALID.", "Success"
    Else
        ShowMessage "The credit card number is INVALID.", "Failure"
    End If
End Sub

' Assign to ExitBtn -> Events -> "Execute action" in the Dialog Editor.
Sub OnExitClicked(oEvent As Object)
    oEvent.Source.Context.endExecute()
End Sub

Sub Main
    ShowValidatorWindow()
End Sub
