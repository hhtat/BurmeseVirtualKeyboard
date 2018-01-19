!include "FileFunc.nsh"

!define PRNAME "Burmese Virtual Keyboard"

Name "${PRNAME}"

!define REGKEY "SOFTWARE\BurmeseVirtualKeyboard"
!define UNREGKEY "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\BurmeseVirtualKeyboard"
!define UNEXE "Uninstall.exe"
!define VERSION 1.0.4
!define PUBLISHER "Hein Htat"

OutFile "BurmeseVKSetup.exe"
InstallDir "$PROGRAMFILES\Burmese Virtual Keyboard"
InstallDirRegKey HKLM "${REGKEY}" "Install_Dir"
RequestExecutionLevel admin

VIProductVersion 1.0.4.0
VIAddVersionKey ProductName "${PRNAME}"
VIAddVersionKey CompanyName "${PUBLISHER}"
VIAddVersionKey LegalCopyright ""
VIAddVersionKey FileDescription "${PRNAME} ${VERSION} Installer"
VIAddVersionKey FileVersion ""
VIAddVersionKey ProductVersion "${VERSION}"

Page directory
Page instfiles

UninstPage uninstConfirm
UninstPage instfiles

Function .onInit

  ReadRegStr $R0 HKLM ${UNREGKEY} "UninstallString"
  StrCmp $R0 "" done
  ReadRegStr $R1 HKLM ${UNREGKEY} "Uninstaller"

  MessageBox MB_OKCANCEL|MB_ICONEXCLAMATION "$(^Name) is already installed.$\n$\nClick 'OK' to remove the previous version or 'Cancel' to cancel this upgrade." IDCANCEL abort

  ClearErrors
  ExecWait "$R0 _?=$INSTDIR"
  IfErrors abort

  StrCmp $R1 "" done
  Delete $R1
  Goto done

done:

  Return

abort:

  Abort

FunctionEnd

Function .onInstSuccess

  ShellExecAsUser::ShellExecAsUser "" "$SMPROGRAMS\$(^Name).lnk"

FunctionEnd

Section

  SetShellVarContext all
  SetOutPath "$INSTDIR"

  File "Files\BurmeseVirtualKeyboard.exe"
  File "Files\BurmeseVirtualKeyboard.pdb"
  File "Files\WindowsInput.dll"

  CreateShortcut "$SMPROGRAMS\$(^Name).lnk" "$INSTDIR\BurmeseVirtualKeyboard.exe" "" "$INSTDIR\BurmeseVirtualKeyboard.exe" 0

  WriteRegStr HKLM "${REGKEY}" "Install_Dir" "$INSTDIR"

  WriteRegStr HKLM "${UNREGKEY}" "DisplayName" "$(^Name)"
  WriteRegStr HKLM "${UNREGKEY}" "Publisher" "${PUBLISHER}"
  WriteRegStr HKLM "${UNREGKEY}" "DisplayVersion" "${VERSION}"
  WriteRegStr HKLM "${UNREGKEY}" "UninstallString" '"$INSTDIR\${UNEXE}"'
  WriteRegStr HKLM "${UNREGKEY}" "Uninstaller" "$INSTDIR\${UNEXE}"
  WriteRegDWORD HKLM "${UNREGKEY}" "NoModify" 1
  WriteRegDWORD HKLM "${UNREGKEY}" "NoRepair" 1

  ${GetSize} "$INSTDIR" "/S=0K" $0 $1 $2
  IntFmt $0 "0x%08X" $0
  WriteRegDWORD HKLM "${UNREGKEY}" "EstimatedSize" "$0"

  WriteUninstaller "Uninstall.exe"

SectionEnd

Section "Uninstall"

  SetShellVarContext all


  DeleteRegKey HKLM "${UNREGKEY}"
  DeleteRegKey HKLM "${REGKEY}"

  Delete "$INSTDIR\BurmeseVirtualKeyboard.exe"
  Delete "$INSTDIR\BurmeseVirtualKeyboard.pdb"
  Delete "$INSTDIR\WindowsInput.dll"

  Delete "$SMPROGRAMS\$(^Name).lnk"

  Delete "$INSTDIR\${UNEXE}"
  RMDir "$INSTDIR"

SectionEnd
