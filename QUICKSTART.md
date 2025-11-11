# 🚀 Quickstart Guide - Finance App

## ⚠️ Wichtig: .NET 8 SDK erforderlich!

Falls Sie den Fehler `No .NET SDKs were found` erhalten, installieren Sie bitte:

**Download: https://dotnet.microsoft.com/download/dotnet/8.0**

Wählen Sie:
- **Windows**: `.NET 8.0 SDK (x64)` Installer
- Nach Installation Terminal/PowerShell neu öffnen
- Überprüfung: `dotnet --version` sollte `8.0.x` anzeigen

---

## 📋 Schritt-für-Schritt Anleitung

### 1. Supabase Datenbank vorbereiten

#### Option A: Supabase Dashboard (Empfohlen)
1. Gehe zu https://supabase.com/dashboard
2. Login mit deinem Account
3. Projekt auswählen: **`nqbkogbynurtjtppajic`**
4. **SQL Editor** öffnen (linke Sidebar)
5. Führe **nacheinander** folgende SQL-Dateien aus:

**a) Schema & Tabellen:**
```sql
-- Kopiere Inhalt von: supabase/migrations/20250101000001_initial_schema.sql
-- Einfügen im SQL Editor → "RUN" klicken
```

**b) RLS Policies:**
```sql
-- Kopiere Inhalt von: supabase/migrations/20250101000002_rls_policies.sql
-- Einfügen im SQL Editor → "RUN" klicken
```

**c) Views & RPC Funktionen:**
```sql
-- Kopiere Inhalt von: supabase/migrations/20250101000003_views_and_rpc.sql
-- Einfügen im SQL Editor → "RUN" klicken
```

**Überprüfung:**
- Gehe zu **Database** → **Tables**
- Du solltest sehen: `organizations`, `memberships`, `cost_items`, `invoices`, `attachments`

#### Option B: Supabase CLI (Fortgeschritten)
```bash
# Supabase CLI installieren
npm install -g supabase

# In Projektordner
cd C:\Users\LRAus\Desktop\Finanz_App

# Mit deinem Projekt verbinden
supabase link --project-ref nqbkogbynurtjtppajic

# Migrationen ausführen
supabase db push
```

### 2. TailwindCSS bauen

```powershell
# Im Projektroot
cd C:\Users\LRAus\Desktop\Finanz_App

# TailwindCSS bauen (einmalig)
npm run css:build
```

**Erwartetes Ergebnis:**
```
Done in 305ms.
```
→ Datei erstellt: `src/FinanceApp.Web/wwwroot/css/output.css`

### 3. Blazor Web-App starten

```powershell
cd src\FinanceApp.Web
dotnet restore
dotnet run
```

**Erwartete Ausgabe:**
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:5001
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
```

**Browser öffnen:**
- https://localhost:5001
- oder http://localhost:5000

### 4. App testen

1. **Registrierung:**
   - Klicke "Registrieren"
   - E-Mail: `test@example.com`
   - Passwort: `Test123!`
   - → Konto wird erstellt

2. **Anmeldung:**
   - Nach Registrierung automatisch auf "Anmelden" umgeleitet
   - Gleiche Credentials eingeben
   - → Weiterleitung zu `/dashboard`

3. **Datenbank prüfen:**
   - Supabase Dashboard → **Table Editor**
   - Öffne `organizations` → Du solltest einen Eintrag sehen
   - Öffne `memberships` → Dein User ist Owner

---

## 🛠 Häufige Probleme & Lösungen

### Problem 1: `.NET SDK not found`
**Lösung:**
```powershell
# .NET 8 SDK installieren von:
# https://dotnet.microsoft.com/download/dotnet/8.0

# Nach Installation PowerShell NEU öffnen
dotnet --version
# Sollte zeigen: 8.0.x
```

### Problem 2: `npm not found`
**Lösung:**
```powershell
# Node.js installieren von:
# https://nodejs.org/en/download/

# Nach Installation:
node --version
npm --version
```

### Problem 3: `TailwindCSS not building`
**Lösung:**
```powershell
# node_modules löschen und neu installieren
rm -r node_modules
npm install
npm run css:build
```

### Problem 4: `Supabase connection error`
**Lösung:**
1. Prüfe `appsettings.json`:
   ```json
   {
     "Supabase": {
       "Url": "https://nqbkogbynurtjtppajic.supabase.co",
       "AnonKey": "eyJhbGc..."
     }
   }
   ```

2. Supabase Dashboard → **Settings** → **API**
   - Prüfe ob URL & Key korrekt sind

### Problem 5: `Database migrations failed`
**Lösung:**
1. Supabase Dashboard → **SQL Editor**
2. Führe Migrationen **manuell** aus (siehe Schritt 1, Option A)
3. Prüfe Fehler im SQL Editor
4. Bei Syntax-Fehlern: Kopiere SQL nochmal vollständig

---

## 📱 Mobile-App bauen (Optional)

### Android (Windows/Mac/Linux)
```powershell
cd src\FinanceApp.Mobile
dotnet build -f net8.0-android

# Auf Gerät deployen (USB-Debugging aktiviert)
dotnet run -f net8.0-android
```

**Voraussetzungen:**
- Android SDK installiert
- Android-Emulator oder physisches Gerät

### iOS (nur macOS)
```bash
cd src/FinanceApp.Mobile
dotnet build -f net8.0-ios

# Auf Simulator deployen
dotnet run -f net8.0-ios
```

**Voraussetzungen:**
- macOS mit Xcode installiert
- iOS Simulator

---

## 🎯 Nächste Schritte

Nach erfolgreichem Start kannst du:

1. **Dashboard-Seite erstellen** (`Pages/Dashboard.razor`)
2. **Fixkosten-Verwaltung** implementieren (`Pages/Costs/`)
3. **Rechnungs-Verwaltung** implementieren (`Pages/Invoices/`)
4. **Edge Functions deployen** (siehe README.md)
5. **GitHub Pages deployen** (siehe README.md)

---

## 📚 Weitere Hilfe

- **Vollständige Dokumentation**: [README.md](README.md)
- **Supabase Docs**: https://supabase.com/docs
- **Blazor Docs**: https://learn.microsoft.com/aspnet/core/blazor
- **.NET MAUI Docs**: https://learn.microsoft.com/dotnet/maui

---

## ✅ Checkliste

- [ ] .NET 8 SDK installiert (`dotnet --version`)
- [ ] Node.js installiert (`node --version`)
- [ ] Supabase Migrationen ausgeführt (Tabellen sichtbar)
- [ ] TailwindCSS gebaut (`output.css` existiert)
- [ ] Blazor App läuft (`https://localhost:5001`)
- [ ] Registrierung erfolgreich
- [ ] Login erfolgreich

Viel Erfolg! 🚀
