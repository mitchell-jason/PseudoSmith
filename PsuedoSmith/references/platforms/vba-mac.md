# Platform Reference: VBA on macOS

## Scope

This file covers Microsoft Office VBA running on macOS.

Load this reference when:

- `TARGET_LANGUAGE = VBA`
- `TARGET_PLATFORM = mac_x64` or `mac_arm`
- the blueprint indicates Microsoft Office for Mac, Excel for Mac, Word for Mac, PowerPoint for Mac, Outlook for Mac, or another Office-on-macOS host.

## Defaults

Default dialect: Microsoft Office VBA for Mac.

Default VBA version: modern VBA 7.x for Office for Mac unless the blueprint specifies an older host/version.

Office host must be clear when host-specific APIs are required. Common hosts include:

- Excel
- Word
- PowerPoint
- Outlook

If the blueprint requires host-specific object models and the host is not specified, trigger the material ambiguity checkpoint.

## Language Rules

- `DELEGATE` -> VBA has no native delegate/function type. Use `AddressOf` with `Application.Run`/`CallByName` dispatch, or an interface-based callback class (`Implements`). Record the substitution in the Implementation Report.

## Monetary Values

- `CURRENCY` -> use the native VBA `Currency` type (a scaled 64-bit integer, 4 decimal
  places). Do not map `CURRENCY` to `Double`/`Single`, which corrupts monetary math. Use
  `Decimal` (via a `Variant`/`CDec`) only when more than 4 decimal places are required.

## Core Compatibility Rules

VBA on macOS is not equivalent to Windows VBA.

Do not generate Windows-only VBA APIs for macOS targets unless the blueprint explicitly asks for conditional Windows/Mac code.

Avoid or checkpoint before using:

- Win32 API `Declare` calls;
- Windows DLL calls;
- COM automation through `CreateObject` or `GetObject`;
- `Scripting.FileSystemObject`;
- `Scripting.Dictionary`;
- Windows Registry access;
- WScript objects;
- ActiveX controls;
- ADO, DAO, OLEDB providers that rely on Windows COM;
- Windows path separators or drive-letter paths.

When cross-platform Office VBA is requested, use conditional compilation and native VBA fallbacks where possible.

Example conditional structure:

```text
#If Mac Then
    ' macOS implementation
#Else
    ' Windows implementation
#End If
```

## File System

Prefer native VBA file and directory APIs:

- `Dir`
- `Open`
- `Close`
- `Input #`
- `Line Input #`
- `Print #`
- `Kill`
- `Name`
- `MkDir`
- `RmDir`

Use host-provided file pickers where appropriate, such as Excel's `Application.GetOpenFilename` or `Application.GetSaveAsFilename`, when the host supports them.

Use `Application.PathSeparator` when available.

Do not emit Windows drive-letter paths such as `C:\...` for Mac targets.

Use POSIX-style paths only when the blueprint or host API expects them. If the path format is unclear and materially affects file access, trigger the material ambiguity checkpoint.

## macOS Sandboxing and File Access

Modern Office for Mac may be sandboxed. File access outside allowed locations may require user selection or explicit permission.

For Excel for Mac, when multiple external files must be accessed, consider `GrantAccessToMultipleFiles` only when the blueprint or host context supports it.

Do not silently add broad filesystem access behavior. If the blueprint requires automated access to arbitrary folders and the permission model is unclear, trigger the material ambiguity checkpoint.

## Dictionaries and Collections

`Scripting.Dictionary` is Windows COM-based and should not be generated for Mac-only VBA unless a compatible replacement dependency is declared.

For key-value behavior on Mac VBA, use one of:

1. `Collection` with explicit key handling when sufficient;
2. parallel arrays for small/simple mappings;
3. a custom dictionary class generated within scope;
4. a declared cross-platform dictionary dependency if specified by `USES`.

Record non-trivial substitutions in the Implementation Report.

## GUI and Forms

GUI anchors require `TARGET_UI_FRAMEWORK`.

Common explicit values for Office VBA on Mac include:

- `VBA.UserForm`
- `OfficeRibbon`
- `HostDialog`
- `custom:<name>`

Microsoft Office UserForms are supported in many Mac Office contexts, but ActiveX controls and some Windows-specific form/control behavior are not portable.

Do not generate worksheet ActiveX controls for Mac targets. Prefer Forms controls or UserForms when explicitly requested and supported by the host.

If a requested GUI control or event is unavailable on Mac Office, provide the closest native alternative when unambiguous and report the substitution. If no safe substitute exists, emit a TODO and mark the item as partially implemented.

## AppleScript and Shell Integration

Do not use AppleScript, shell scripts, or `AppleScriptTask` unless the blueprint explicitly requests macOS automation or there is no other host-native way to implement the requested behavior.

`MacScript` is deprecated and should not be used for new code unless the blueprint explicitly requires legacy compatibility.

If the blueprint requires macOS automation, external process execution, shell commands, or AppleScript integration, and the exact mechanism is unspecified, trigger the material ambiguity checkpoint.

## Database

VBA on Mac has limited database connectivity compared with Windows VBA.

Database behavior requires `DATABASE_PROVIDER`.

Do not assume ADO, DAO, OLEDB, or Access database support on Mac.

Common options, only when declared or sufficiently specified:

- ODBC with installed macOS driver;
- host-specific workbook-backed storage;
- CSV/file-backed storage;
- HTTP API calls to an external database service;
- SQLite only if a compatible driver/library or integration method is declared.

If the database provider, driver, DSN, connection method, or host support is unclear, trigger the material ambiguity checkpoint.

## Network and HTTP

VBA on Mac does not have the same COM HTTP objects commonly used on Windows, such as `MSXML2.XMLHTTP` or `WinHttp.WinHttpRequest`.

Do not generate those COM objects for Mac-only targets.

If HTTP/network behavior is required, use a host-supported or declared mechanism. If none is specified, trigger the material ambiguity checkpoint.

## Security Notes

Do not invent:

- macro trust policy;
- code-signing setup;
- sandbox entitlements;
- password storage policy;
- encryption strategy;
- keychain integration;
- filesystem permission grants.

If the blueprint requests security-sensitive behavior and the mechanism is unspecified, trigger the material ambiguity checkpoint.

## Platform Compatibility Audit

Before presenting output, audit for Windows-only constructs, including:

- `CreateObject`;
- `GetObject`;
- `Scripting.Dictionary`;
- `FileSystemObject`;
- `WScript`;
- Win32 `Declare`;
- Windows Registry APIs;
- Windows-only path handling;
- ADO, OLEDB, DAO assumptions;
- ActiveX controls.

Replace with Mac-compatible equivalents when unambiguous. Otherwise emit TODO comments and report the item as partially implemented.

## TODO Comment Syntax

Use apostrophe comments:

```text
' TODO: description
```
