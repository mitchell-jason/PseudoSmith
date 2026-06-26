# Platform Reference: VBA and LibreOffice Basic

## Scope

This file covers Microsoft Office VBA and LibreOffice Basic/UNO-style targets.

Use `TARGET_LANGUAGE = VBA`. The blueprint must specify platform/dialect clearly enough to
choose Microsoft Office VBA vs LibreOffice Basic. If not clear and dialect-specific APIs are
needed, trigger Step 3.3.

## Defaults

Default VBA version: VBA 7.1 when targeting modern Office.

## Language Rules

- `DELEGATE` -> VBA has no native delegate/function type. Use `AddressOf` with `Application.Run`/`CallByName` dispatch, or an interface-based callback class (`Implements`). Record the substitution in the Implementation Report.

## Monetary Values

- `CURRENCY` -> use the native `Currency` type in MS Office VBA. In LibreOffice Basic, use
  `Currency` where supported, otherwise `CDec`/`Decimal`. Never map `CURRENCY` to
  `Double`/`Single` for monetary math.

## LibreOffice Basic / UNO

LibreOffice targets commonly require UNO APIs rather than MS Office object models.

MS VBA constructs that are not portable to LibreOffice include:

| MS VBA construct | LibreOffice substitute / rule |
|---|---|
| `UserForm`, `Form` | UNO dialog model (`com.sun.star.awt.UnoControlDialogModel`) |
| `CreateObject("ProgID")` | `CreateUnoService("com.sun.star...")` where equivalent exists |
| `UnoControlButtonModel.ActionCommand` | use `PushButtonType` and `execute()` return values |
| `Application.Run` | direct procedure call where possible |
| some string helpers such as `StrReverse` | manual loops when unavailable |

Always audit generated Basic code for dialect-specific APIs.

## LibreOffice Dialog Pattern

Use UNO dialog models and controls. For simple OK/Cancel flows, prefer dialog `execute()`
return values over complex listener classes unless the blueprint requires custom events.

Button `PushButtonType` values:

- `1` = OK;
- `2` = Cancel;
- `3` = Help.

## Microsoft Office VBA: Windows vs Mac

The Microsoft Scripting Runtime is Windows-only and absent on Mac VBA.

| Windows-only / risky API | Mac substitute / rule |
|---|---|
| `Scripting.Dictionary` | custom class, `Collection`-backed map, or parallel arrays |
| `FileSystemObject` | native VBA file I/O: `Dir`, `Open`, `Close`, `Print #`, `Kill`, `MkDir`, `RmDir` |
| `CreateObject` / `GetObject` COM | unavailable on Mac; use native/AppleScript equivalent or mark Not Implemented |
| `WScript.*` | unavailable on Mac |
| Win32 `Declare` APIs | wrap out on Mac and provide alternative if specified |

For cross-platform Office VBA, prefer native VBA file I/O over FSO.

## 32-bit vs 64-bit VBA

- VBA7 `Declare` statements require `PtrSafe`.
- Pointer-sized values should use `LongPtr`.
- Mac Office is 64-bit and does not support Windows DLL declares.

Use conditional compilation when Windows API declares are explicitly requested:

```text
#If Mac Then
    ' Mac path
#ElseIf Win64 Then
    ' Windows 64-bit path
#Else
    ' Windows 32-bit path
#End If
```

## GUI

GUI anchors require `TARGET_UI_FRAMEWORK`.

Common explicit values:

- `VBA.UserForm` for Microsoft Office VBA;
- `UNO` for LibreOffice Basic.

Do not use MS UserForms for LibreOffice targets.

## Database

VBA database behavior requires `DATABASE_PROVIDER`. Many database APIs are provider and host
specific.

Examples:

- Access/DAO: declare explicitly in blueprint/USES;
- ADO: Windows COM only; high risk or unavailable on Mac;
- ODBC: host/platform specific;
- LibreOffice Base/UNO: requires UNO APIs.

If provider/driver/host API is unclear, trigger Step 3.3.

## Comment and TODO Syntax

Use apostrophe comments:

```text
' TODO: description
```
