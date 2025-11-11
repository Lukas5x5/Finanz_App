# Deployment Guide für Finance App

Diese Anleitung erklärt, wie Sie die Finance App auf GitHub Pages deployen und mit Supabase verbinden.

## Voraussetzungen

- GitHub-Konto
- Supabase-Projekt (bereits erstellt und konfiguriert)
- Git installiert
- .NET 8 SDK installiert

## 1. GitHub Repository erstellen

1. Gehen Sie zu [GitHub](https://github.com) und erstellen Sie ein neues Repository
2. Nennen Sie es `Finanz_App` (oder einen anderen Namen)
3. Machen Sie es **öffentlich** (für GitHub Pages erforderlich)
4. Erstellen Sie **kein** README, .gitignore oder License (da wir bereits Dateien haben)

## 2. Lokales Repository initialisieren

```bash
cd c:\Users\LRAus\Desktop\Finanz_App
git init
git add .
git commit -m "Initial commit: Finance App mit Blazor WebAssembly und Supabase"
git branch -M main
git remote add origin https://github.com/IHR_USERNAME/Finanz_App.git
git push -u origin main
```

Ersetzen Sie `IHR_USERNAME` mit Ihrem GitHub-Benutzernamen.

## 3. GitHub Pages aktivieren

1. Gehen Sie zu Ihrem Repository auf GitHub
2. Klicken Sie auf **Settings** > **Pages**
3. Unter **Source** wählen Sie **GitHub Actions**
4. Die Workflows sind bereits konfiguriert und werden automatisch ausgeführt

## 4. GitHub Secrets konfigurieren

Für die Supabase-Integration müssen Sie folgende Secrets hinzufügen:

1. Gehen Sie zu **Settings** > **Secrets and variables** > **Actions**
2. Klicken Sie auf **New repository secret**
3. Fügen Sie folgende Secrets hinzu:

### Erforderliche Secrets:

- `SUPABASE_ACCESS_TOKEN`
  - Gehen Sie zu [Supabase Dashboard](https://supabase.com/dashboard)
  - Settings > API > Personal Tokens
  - Erstellen Sie einen neuen Token

- `SUPABASE_PROJECT_ID`
  - Wert: `nqbkogbynurtjtppajic` (Ihr Projekt-ID)

- `SUPABASE_DB_PASSWORD`
  - Ihr Supabase-Datenbank-Passwort
  - Zu finden unter: Settings > Database > Database password

- `SUPABASE_SERVICE_ROLE_KEY`
  - Zu finden unter: Settings > API > Service Role Key (secret)

## 5. Workflows überprüfen

Nach dem Push werden automatisch zwei Workflows ausgeführt:

### 1. Deploy Blazor WASM to GitHub Pages
- Kompiliert die Blazor WebAssembly App
- Deployed sie auf GitHub Pages
- URL: `https://IHR_USERNAME.github.io/Finanz_App/`

### 2. Deploy Supabase Migrations and Edge Functions
- Führt nur bei Änderungen in `supabase/` aus
- Deployed Datenbank-Migrationen
- Deployed Edge Functions (reminders, thumbnails)

## 6. Base URL konfigurieren (falls abweichend)

Falls Ihr Repository einen anderen Namen als `Finanz_App` hat:

1. Öffnen Sie `.github/workflows/pages.yml`
2. Ändern Sie Zeile 42:
   ```yaml
   sed -i 's/<base href="\/" \/>/<base href="\/IHR_REPO_NAME\/" \/>/g' release/wwwroot/index.html
   ```

## 7. Supabase Edge Functions testen

Die Edge Functions werden automatisch deployed. Um sie zu testen:

### Reminders Function
```bash
curl https://nqbkogbynurtjtppajic.supabase.co/functions/v1/reminders
```

### Thumbnails Function
```bash
curl -X POST https://nqbkogbynurtjtppajic.supabase.co/functions/v1/thumbnails \
  -H "Content-Type: application/json" \
  -d '{"attachmentId": "GUID_HIER"}'
```

## 8. Cron Schedule für Reminders einrichten

1. Gehen Sie zu [Supabase Dashboard](https://supabase.com/dashboard)
2. Edge Functions > reminders > Settings
3. Fügen Sie einen Cron Schedule hinzu:
   - Schedule: `0 6 * * *` (täglich um 6 Uhr morgens UTC)
   - HTTP Method: `POST`

## 9. MAUI Mobile App (optional)

Um die Mobile App zu erstellen, benötigen Sie:

```bash
# Android Workload installieren
dotnet workload install maui-android

# iOS Workload installieren (nur auf macOS)
dotnet workload install maui-ios

# Build für Android
cd src/FinanceApp.Mobile
dotnet build -f net8.0-android -c Release

# Build für iOS (nur auf macOS)
dotnet build -f net8.0-ios -c Release
```

### Android-Deployment

1. Erstellen Sie eine APK:
   ```bash
   dotnet publish -f net8.0-android -c Release
   ```

2. Die APK finden Sie unter:
   ```
   src/FinanceApp.Mobile/bin/Release/net8.0-android/publish/
   ```

3. Installieren Sie die APK auf Ihrem Android-Gerät

### iOS-Deployment

1. Benötigt einen Mac mit Xcode
2. Apple Developer Account erforderlich
3. Folgen Sie der [MAUI iOS Deployment Guide](https://learn.microsoft.com/en-us/dotnet/maui/ios/deployment/)

## 10. Troubleshooting

### Workflow schlägt fehl

**Problem:** GitHub Actions Workflow schlägt fehl
**Lösung:**
- Überprüfen Sie die Secrets in den Repository-Einstellungen
- Schauen Sie sich die Workflow-Logs an
- Stellen Sie sicher, dass alle Supabase-Migrationen lokal erfolgreich waren

### App lädt nicht

**Problem:** GitHub Pages zeigt 404 oder leere Seite
**Lösung:**
- Warten Sie 2-3 Minuten nach dem Deployment
- Überprüfen Sie die base href in der index.html
- Cache des Browsers leeren (Strg+F5)

### Supabase-Verbindung schlägt fehl

**Problem:** Login funktioniert nicht
**Lösung:**
- Überprüfen Sie die Supabase-URL und Anon-Key in `wwwroot/appsettings.json`
- Stellen Sie sicher, dass RLS-Policies aktiviert sind
- Überprüfen Sie die CORS-Einstellungen in Supabase

## 11. Lokale Entwicklung

Um die App lokal zu testen:

```bash
cd src/FinanceApp.Web
dotnet run
```

App ist verfügbar unter: https://localhost:5001

## 12. Weitere Schritte

Nach erfolgreichem Deployment können Sie:

1. **Custom Domain hinzufügen**
   - Settings > Pages > Custom domain
   - Folgen Sie den Anweisungen zur DNS-Konfiguration

2. **PWA Features aktivieren**
   - Die App ist bereits PWA-ready
   - Benutzer können sie auf dem Homescreen installieren

3. **Analytics hinzufügen**
   - Google Analytics
   - Plausible Analytics
   - Umami Analytics

4. **Monitoring einrichten**
   - Sentry für Error Tracking
   - Supabase Dashboard für Datenbank-Monitoring

## Dokumentation

- [Blazor WebAssembly Dokumentation](https://learn.microsoft.com/en-us/aspnet/core/blazor/)
- [Supabase Dokumentation](https://supabase.com/docs)
- [GitHub Pages Dokumentation](https://docs.github.com/en/pages)
- [.NET MAUI Dokumentation](https://learn.microsoft.com/en-us/dotnet/maui/)

## Support

Bei Fragen oder Problemen:
1. Überprüfen Sie die Workflow-Logs auf GitHub
2. Schauen Sie in die Supabase-Logs
3. Öffnen Sie ein Issue im Repository
