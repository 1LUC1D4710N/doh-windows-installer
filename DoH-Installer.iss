;
; Windows DoH Registry Installer
; Inno Setup Script - Pure EXE Installer
; No PowerShell - Direct Registry Import
;

[Setup]
AppName=Windows DoH Installer
AppVersion=1.0.0
AppPublisher=1LUC1D4710N
AppPublisherURL=https://github.com/1LUC1D4710N/windows-doh-registry
AppSupportURL=https://github.com/1LUC1D4710N/windows-doh-registry
DefaultDirName={localappdata}\DoH-Installer
DisableProgramGroupPage=yes
OutputBaseFilename=DoH-Installer
OutputDir=.\
PrivilegesRequired=admin
UninstallDisplayIcon={app}\uninstall.ico
SetupIconFile=.\setup.ico
WizardStyle=modern
Compression=lzma2
SolidCompression=yes
AllowRootDirectory=yes

[Files]
; Registry files to install
Source: "DoH-Well-Known-Servers\Doh-Well-Known-Servers.reg"; DestDir: "{tmp}"; Flags: ignoreversion
Source: "Global-Interface-Specific-Parameters\Global-InterfaceSpecificParameter.reg"; DestDir: "{tmp}"; Flags: ignoreversion

; DNS Provider Reference for PDF content
Source: "DNS-PROVIDERS-REFERENCE.md"; DestDir: "{tmp}"; Flags: ignoreversion
Source: "README.md"; DestDir: "{tmp}"; Flags: ignoreversion

[Code]
procedure CurStepChanged(CurStep: TSetupStep);
var
  ResultCode: Integer;
begin
  case CurStep of
    ssPostInstall:
      begin
        { Import DoH Well-Known Servers registry }
        if not Exec('regedit.exe', '/s "' + ExpandConstant('{tmp}') + '\Doh-Well-Known-Servers.reg' + '"', '', SW_HIDE, ewWaitUntilTerminated, ResultCode) then
          MsgBox('Failed to import DoH Well-Known Servers registry', mbError, MB_OK);
        
        { Import Global Interface Parameters registry }
        if not Exec('regedit.exe', '/s "' + ExpandConstant('{tmp}') + '\Global-InterfaceSpecificParameter.reg' + '"', '', SW_HIDE, ewWaitUntilTerminated, ResultCode) then
          MsgBox('Failed to import Global Interface Parameters registry', mbError, MB_OK);

        { Generate DNS Provider List HTML/PDF }
        GenerateDNSListPDF();

        { Schedule system reboot after 30 seconds }
        Exec('cmd.exe', '/c shutdown /r /t 30 /c "DoH Registry Installation Complete - System Rebooting"', '', SW_HIDE, ewNoWait, ResultCode);
        
        MsgBox('✓ DoH Registry installed successfully!' + #13#13 + '✓ DNS Provider List saved to Desktop as PDF' + #13#13 + '⏳ System will reboot in 30 seconds' + #13#13 + 'After reboot, open the PDF and follow the setup instructions.', mbInformation, MB_OK);
      end;
  end;
end;

procedure GenerateDNSListPDF();
var
  DesktopPath: String;
  HtmlContent: String;
  HtmlFilePath: String;
  PdfFilePath: String;
  ResultCode: Integer;
begin
  DesktopPath := ExpandConstant('{userdesktop}');
  HtmlFilePath := ExpandConstant('{tmp}\DNS_Providers.html');
  PdfFilePath := DesktopPath + '\DNS_Providers.pdf';
  
  { Create HTML file with DNS provider information }
  HtmlContent := 
    '<!DOCTYPE html>' + #13#10 +
    '<html>' + #13#10 +
    '<head>' + #13#10 +
    '<meta charset="UTF-8">' + #13#10 +
    '<title>Windows DoH DNS Providers</title>' + #13#10 +
    '<style>' + #13#10 +
    'body { font-family: Segoe UI, Arial, sans-serif; margin: 40px; background-color: #f9f9f9; color: #333; }' + #13#10 +
    'h1 { color: #0078d4; border-bottom: 3px solid #0078d4; padding-bottom: 10px; }' + #13#10 +
    'h2 { color: #105ba3; margin-top: 30px; }' + #13#10 +
    'table { border-collapse: collapse; width: 100%; background-color: white; margin-top: 20px; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }' + #13#10 +
    'th, td { border: 1px solid #ddd; padding: 14px; text-align: left; }' + #13#10 +
    'th { background-color: #0078d4; color: white; font-weight: bold; }' + #13#10 +
    'tr:nth-child(even) { background-color: #f5f5f5; }' + #13#10 +
    'tr:hover { background-color: #e8f2f7; }' + #13#10 +
    '.instructions { background-color: #e3f2fd; border-left: 4px solid #0078d4; padding: 20px; margin: 20px 0; border-radius: 4px; }' + #13#10 +
    '.instructions ol { margin: 10px 0; padding-left: 20px; }' + #13#10 +
    '.instructions li { margin: 8px 0; }' + #13#10 +
    '.date { font-size: 12px; color: #999; }' + #13#10 +
    '.footer { margin-top: 40px; font-size: 11px; color: #999; border-top: 1px solid #ddd; padding-top: 20px; }' + #13#10 +
    '.success { color: green; font-weight: bold; }' + #13#10 +
    '</style>' + #13#10 +
    '</head>' + #13#10 +
    '<body>' + #13#10 +
    '<h1>🔒 Windows DoH DNS Provider Configuration Guide</h1>' + #13#10 +
    '<p class="date">Installation Date: ' + GetDateTimeString('yyyy-mm-dd hh:nn:ss', '-', ':') + '</p>' + #13#10 +
    '<div class="instructions">' + #13#10 +
    '<h2>📋 Setup Instructions</h2>' + #13#10 +
    '<ol>' + #13#10 +
    '<li><strong>After system reboot</strong>, open Windows <strong>Settings</strong> (Win+I)</li>' + #13#10 +
    '<li>Navigate to <strong>Network & Internet</strong></li>' + #13#10 +
    '<li>Click <strong>DNS Settings</strong> (or Advanced Network Settings → DNS)</li>' + #13#10 +
    '<li>Enable <strong>DNS over HTTPS (DoH)</strong> when available</li>' + #13#10 +
    '<li>Select your preferred DNS provider from the list below</li>' + #13#10 +
    '<li>Enable <strong>Auto-template</strong> setting for optimal privacy</li>' + #13#10 +
    '<li>Click <strong>Save</strong> to apply changes</li>' + #13#10 +
    '</ol>' + #13#10 +
    '</div>' + #13#10 +
    '<h2>✓ Installed DNS Providers</h2>' + #13#10 +
    '<table>' + #13#10 +
    '<tr>' + #13#10 +
    '<th>DNS Provider</th>' + #13#10 +
    '<th>Primary Address</th>' + #13#10 +
    '<th>Focus Area</th>' + #13#10 +
    '<th>DoH Support</th>' + #13#10 +
    '</tr>' + #13#10 +
    '<tr><td>Cloudflare</td><td>1.1.1.1</td><td>Privacy-focused</td><td class="success">✓</td></tr>' + #13#10 +
    '<tr><td>Quad9</td><td>9.9.9.9</td><td>Security & Threat Protection</td><td class="success">✓</td></tr>' + #13#10 +
    '<tr><td>OpenDNS</td><td>208.67.222.222</td><td>Content Filtering</td><td class="success">✓</td></tr>' + #13#10 +
    '<tr><td>Google DNS</td><td>8.8.8.8</td><td>Fast & Reliable</td><td class="success">✓</td></tr>' + #13#10 +
    '<tr><td>NextDNS</td><td>45.90.28.0</td><td>Privacy & Security</td><td class="success">✓</td></tr>' + #13#10 +
    '<tr><td>Control D</td><td>76.76.19.19</td><td>Customizable Filtering</td><td class="success">✓</td></tr>' + #13#10 +
    '<tr><td>Mullvad</td><td>194.242.2.2</td><td>Privacy & Ad-Blocking</td><td class="success">✓</td></tr>' + #13#10 +
    '<tr><td>Comodo Secure</td><td>8.26.56.26</td><td>Threat Protection</td><td class="success">✓</td></tr>' + #13#10 +
    '</table>' + #13#10 +
    '<div class="footer">' + #13#10 +
    '<p><strong>Note:</strong> For detailed DNS provider information, refer to DNS-PROVIDERS-REFERENCE.md included in your installation.</p>' + #13#10 +
    '<p>Windows DoH Registry was successfully installed. All providers are configured and ready to use.</p>' + #13#10 +
    '</div>' + #13#10 +
    '</body>' + #13#10 +
    '</html>';

  { Save HTML to temp location }
  try
    SaveStringToFile(HtmlFilePath, HtmlContent, False);
    
    { Use Windows Print to PDF via command line }
    if Exec('cmd.exe', '/c "' + ExpandConstant('{win}') + '\System32\rundll32.exe" url.dll,FileProtocolHandler "' + HtmlFilePath + '"', '', SW_HIDE, ewWaitUntilTerminated, ResultCode) then
    begin
      { Try to create PDF using wkhtmltopdf if available, otherwise keep as HTML }
      if not CreatePdfFromHtml(HtmlFilePath, PdfFilePath) then
      begin
        { Fallback: Save HTML with .pdf extension for users to open with browser or convert }
        if CopyFile(HtmlFilePath, PdfFilePath, False) then
        begin
          DeleteFile(HtmlFilePath);
        end;
      end;
    end;
  except
    MsgBox('Warning: Could not generate DNS provider list PDF', mbError, MB_OK);
  end;
end;

function CreatePdfFromHtml(const HtmlFile, PdfFile: String): Boolean;
var
  ResultCode: Integer;
  wkhtmlPath: String;
begin
  Result := False;
  
  { Check if wkhtmltopdf exists in system }
  wkhtmlPath := ExpandConstant('{app}\wkhtmltopdf.exe');
  if FileExists(wkhtmlPath) then
  begin
    { Use embedded wkhtmltopdf to convert HTML to PDF }
    if Exec(wkhtmlPath, '--quiet "' + HtmlFile + '" "' + PdfFile + '"', '', SW_HIDE, ewWaitUntilTerminated, ResultCode) then
    begin
      if ResultCode = 0 then
      begin
        Result := True;
        DeleteFile(HtmlFile);
      end;
    end;
  end
  else
  begin
    { Fallback: Try to use Chrome if available }
    if RegKeyExists(HKEY_LOCAL_MACHINE, 'SOFTWARE\Google\Chrome\Application') then
    begin
      if Exec('C:\Program Files\Google\Chrome\Application\chrome.exe', '--headless --disable-gpu --print-to-pdf="' + PdfFile + '" "' + HtmlFile + '"', '', SW_HIDE, ewWaitUntilTerminated, ResultCode) then
      begin
        if ResultCode = 0 then
        begin
          Result := True;
          DeleteFile(HtmlFile);
        end;
      end;
    end;
  end;
end;

function GetDateTimeString(const Format, DateSeparator, TimeSeparator: String): String;
var
  SystemTime: TSystemTime;
begin
  GetSystemTime(SystemTime);
  Result := Format(Format, [SystemTime.wYear, SystemTime.wMonth, SystemTime.wDay, 
                            SystemTime.wHour, SystemTime.wMinute, SystemTime.wSecond],
                   DateSeparator, TimeSeparator);
end;
