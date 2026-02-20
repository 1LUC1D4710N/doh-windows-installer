# Windows DoH Installer

One-click Windows EXE installer that automatically installs DNS-over-HTTPS (DoH) registry configurations and generates a setup guide PDF. No PowerShell scripts, no user interaction required during installation.

## Features

✅ **One-Click Installation** - Just run the EXE  
✅ **Silent Registry Import** - Installs all DoH providers automatically  
✅ **PDF Guide Generation** - Creates setup instructions on Desktop  
✅ **Automatic Reboot** - System reboots with 30-second countdown  
✅ **8 DNS Providers Included** - Cloudflare, Quad9, OpenDNS, Google, NextDNS, Control D, Mullvad, Comodo  
✅ **No Dependencies** - No PowerShell, npm, or Node.js required  
✅ **Admin-Only Install** - Requires administrator privileges  

## Installation

### For Users

1. Download **DoH-Installer.exe** from [Releases](https://github.com/1LUC1D4710N/doh-windows-installer/releases)
2. Run `DoH-Installer.exe`
3. Click **Install**
4. Wait for registry imports to complete
5. Review the PDF generated on your Desktop
6. System reboots automatically in 30 seconds
7. After reboot, follow the PDF instructions to configure your DNS provider

### Configuration After Installation

1. Open **Settings** (Win+I)
2. Go to **Network & Internet** → **DNS Settings**
3. Enable **DNS over HTTPS (DoH)**
4. Select your preferred DNS provider:
   - **Cloudflare** (1.1.1.1) - Privacy-focused
   - **Quad9** (9.9.9.9) - Security & threat protection
   - **OpenDNS** (208.67.222.222) - Content filtering
   - **Google DNS** (8.8.8.8) - Fast & reliable
   - **NextDNS** (45.90.28.0) - Privacy & security
   - **Control D** (76.76.19.19) - Customizable filtering
   - **Mullvad** (194.242.2.2) - Privacy & ad-blocking
   - **Comodo** (8.26.56.26) - Threat protection

5. Enable **Auto-template** setting (when available)
6. Click **Save**

## Building from Source

### Requirements

- **Inno Setup 6.x** (free) - Download from [jrsoftware.org](https://jrsoftware.org/isdl.php)
- Windows 10/11
- Administrator privileges

### Steps

1. Install **Inno Setup**
2. Open `DoH-Installer.iss` in Inno Setup IDE
3. Click **Build → Compile** (Ctrl+F9)
4. Output: `DoH-Installer.exe` is generated in the project directory
5. Test the installer on a Windows VM before releasing

## File Structure

```
doh-windows-installer/
├── DoH-Installer.iss                          # Inno Setup script
├── Doh-Well-Known-Servers.reg                 # DoH registry configurations
├── Global-InterfaceSpecificParameter.reg      # Global DNS settings
├── DNS-PROVIDERS-REFERENCE.md                 # Provider documentation
├── README.md                                  # This file
└── LICENSE                                    # MIT License
```

## What Gets Installed

- **DoH Well-Known Servers Registry** - Adds 8 DNS providers with DoH support
- **Global Interface Parameters** - Configures system-wide DNS settings
- **Desktop PDF Guide** - Setup instructions and provider list

## Technical Details

### How It Works

1. **Pre-Installation**: Extracts registry files to temporary directory
2. **Installation Phase**:
   - Imports `Doh-Well-Known-Servers.reg` silently
   - Imports `Global-InterfaceSpecificParameter.reg` silently
   - Generates professional HTML/PDF guide
   - Saves PDF to Desktop
3. **Post-Installation**:
   - Schedules system reboot in 30 seconds
   - Shows completion message

### Registry Locations Modified

- `HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\Dnscache\Parameters\DohWellKnownServers`
- `HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\Dnscache\Parameters`

### Uninstallation

Use Windows **Settings** → **Apps** → **Installed apps** → find **Windows DoH Installer** → **Uninstall**

Uninstaller restores registry to pre-installation state.

## Requirements

- **Windows 10 21H1+** or **Windows 11**
- **Administrator privileges** (required for registry modification)
- **System reboot** (automatic)

## Notes

- Installation requires administrator rights
- System will reboot automatically after installation completes
- Back up system before installation (optional but recommended)
- Registry changes take effect immediately after reboot

## License

MIT License - See [LICENSE](LICENSE) file for details

## Support

For issues or questions, see [DNS-PROVIDERS-REFERENCE.md](DNS-PROVIDERS-REFERENCE.md) for detailed provider information.

---

**Created by**: [@1LUC1D4710N](https://github.com/1LUC1D4710N)  
**Repository**: [windows-doh-registry](https://github.com/1LUC1D4710N/windows-doh-registry)
