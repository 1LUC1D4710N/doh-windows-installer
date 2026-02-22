# DoH Windows Installer

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Platform](https://img.shields.io/badge/Platform-Windows%2010%2B-0078d4)](https://microsoft.com)
[![Built with](https://img.shields.io/badge/Built%20with-C%23%20%2F%20.NET%208-512BD4)](https://dotnet.microsoft.com)
[![Size](https://img.shields.io/badge/Size-78%20MB-green)](releases)

**Your favorite DNS. Automated. Encrypted.**

Install 223+ DNS servers into Windows. Then configure them in Settings. That's it.

---

## What It Does

**Creates Restore Point**  
System restore point created silently before installation. Rollback anytime via System Restore if needed.

**Installs 223+ DNS Servers**  
Cloudflare, Google, Quad9, OpenDNS, Mullvad, AdGuard, and more. Ready to use.

**Verifies Installation**  
Confirms all servers installed correctly before completion.

**One Reboot**  
Changes take effect after reboot. Then you're done.

---

## How It Works

**1. Download & Run**  
Download `doh-windows-installer.exe`. Right-click → Run as Administrator.

**2. Trust the Process**  
Creates a system restore point, installs 223+ DNS servers, verifies setup. Completes in seconds.

**3. Reboot**  
Restart your PC when prompted. Changes take effect.

**4. Configure in Settings**  
Open Settings → Network & Internet → DNS Settings. Choose your favorite provider. Windows auto-templates the DoH for you.

---

## After Installation

Once rebooted, configure DoH in Windows Settings (takes 2 minutes):

1. Open Settings (Win+I)
2. Go to Network & Internet → Advanced network settings → DNS Settings
3. Click Edit next to DNS servers
4. Select Manual
5. Type your preferred DNS provider's IPv4 and IPv6 addresses
6. Windows auto-templates the DoH endpoint for you
7. Click Save

That's it. Your DNS is now encrypted.

---

## Supported Providers

**Cloudflare** — 1.1.1.1 • 1.0.0.1 (12 configurations)

**Google** — 8.8.8.8 • 8.8.4.4 (6 configurations)

**Quad9** — 9.9.9.9 • 149.112.112.112 (13 configurations)

**OpenDNS** — 208.67.222.222 • 208.67.220.220 (12 configurations)

**Mullvad** — 194.242.2.2 • Adblock variants (10 configurations)

**AdGuard** — 94.140.14.14 • Family filtering (12 configurations)

**Control D** — 76.76.2.0 • Multiple options (26 configurations)

**Joindns** — 86.54.11.1 • Child protection (16 configurations)

**Clean Browsing** — 185.228.168.168 • Content filtering (12 configurations)

**LibreDNS** — 116.202.176.26 • Ad-blocking (2 configurations)

**Uncensored DNS** — 91.239.100.100 • Privacy (4 configurations)

All providers: IPv4 and IPv6 endpoints included

**→ [See complete DNS providers reference](DNS-PROVIDERS-REFERENCE.md) for all 223+ server configurations and details**

---

## License

MIT License – No email. No tracking. No accounts.
