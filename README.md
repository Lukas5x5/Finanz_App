# Finance App

Eine moderne Finanz-Management-Anwendung für Privat und Unternehmen, entwickelt mit Blazor WebAssembly und Supabase.

## Features

- **Multi-Organisation Support**: Wechseln Sie einfach zwischen privaten und geschäftlichen Organisationen
- **Fixkosten-Verwaltung**: Verwalten Sie wiederkehrende Ausgaben
- **Rechnungs-Tracking**: Behalten Sie offene und bezahlte Rechnungen im Blick
- **Daten-Export**: Exportieren Sie Ihre Finanzdaten als CSV
- **Moderne UI**: Apple-inspiriertes Design mit Glassmorphismus-Effekten
- **Sichere Authentifizierung**: Powered by Supabase Auth
- **Row Level Security**: Ihre Daten sind durch PostgreSQL RLS geschützt

## Technologie-Stack

- **Frontend**: Blazor WebAssembly (.NET 8.0)
- **Backend**: Supabase (PostgreSQL, Auth, Storage)
- **Design**: CSS Custom Properties, Glassmorphismus
- **CI/CD**: GitHub Actions
- **Hosting**: GitHub Pages

## Lokale Entwicklung

### Voraussetzungen

- .NET 8.0 SDK
- Supabase CLI (optional, für lokale Entwicklung)
- Git

### Installation

1. Repository klonen:
```bash
git clone <your-repo-url>
cd Finanz_App
```

2. Abhängigkeiten wiederherstellen:
```bash
dotnet restore src/FinanceApp.Web/FinanceApp.Web.csproj
```

3. Supabase-Konfiguration:
   - Erstellen Sie ein Supabase-Projekt auf [supabase.com](https://supabase.com)
   - Führen Sie die Migrationen aus dem `supabase/migrations/` Ordner aus
   - Aktualisieren Sie die Supabase-URL und API-Key in Ihrer Konfiguration

4. Anwendung starten:
```bash
cd src/FinanceApp.Web
dotnet run
```

Die App läuft dann auf `https://localhost:5001`

## GitHub Pages Deployment

### Erstmalige Einrichtung

1. **Repository auf GitHub erstellen**:
   ```bash
   git add .
   git commit -m "Initial commit"
   git branch -M main
   git remote add origin <your-github-repo-url>
   git push -u origin main
   ```

2. **GitHub Pages aktivieren**:
   - Gehen Sie zu Ihrem GitHub Repository
   - Navigieren Sie zu **Settings** > **Pages**
   - Unter "Build and deployment":
     - Source: **GitHub Actions** auswählen
   - Speichern

3. **Supabase-Konfiguration für Production**:
   - Stellen Sie sicher, dass Ihre Supabase-Konfiguration die richtige Production-URL verwendet
   - Fügen Sie die GitHub Pages URL zu den Supabase Auth "Redirect URLs" hinzu:
     - Format: `https://<username>.github.io/<repository-name>/`

4. **Deployment auslösen**:
   - Der GitHub Actions Workflow wird automatisch bei jedem Push zum `main` Branch ausgeführt
   - Sie können den Build-Status unter **Actions** in Ihrem Repository überprüfen
   - Nach erfolgreichem Build ist die App verfügbar unter:
     `https://<username>.github.io/<repository-name>/`

### Manuelle Deployment-Trigger

Sie können das Deployment auch manuell auslösen:
1. Gehen Sie zu **Actions** in Ihrem Repository
2. Wählen Sie den "Deploy Blazor WASM to GitHub Pages" Workflow
3. Klicken Sie auf "Run workflow"

## Datenbank-Migrationen

Die Datenbank-Migrationen befinden sich im Ordner `supabase/migrations/`.

### Wichtige Migrationen:

1. **20250101000001_initial_schema.sql**: Initiale Datenbankstruktur
   - Organizations, Memberships
   - Fixed Costs, Invoices, Attachments
   - RLS Policies

2. **20250111000001_fix_membership_trigger.sql**: Fix für RLS Policy
   - Ermöglicht automatische Membership-Erstellung
   - Verwendet SECURITY DEFINER

### Migrationen ausführen:

**Mit Supabase CLI:**
```bash
supabase db push
```

**Manuell über Supabase Dashboard:**
1. Gehen Sie zu Ihrem Supabase-Projekt
2. SQL Editor öffnen
3. SQL-Inhalt der Migrationsdatei kopieren und ausführen

## Projekt-Struktur

```
Finanz_App/
├── .github/
│   └── workflows/
│       └── deploy.yml          # GitHub Actions Workflow
├── src/
│   ├── FinanceApp.Shared/      # Shared Models & DTOs
│   └── FinanceApp.Web/         # Blazor WASM App
│       ├── Pages/              # Razor Pages/Components
│       ├── Services/           # Service Layer
│       └── wwwroot/            # Static Files & CSS
├── supabase/
│   └── migrations/             # Datenbank-Migrationen
├── .gitignore
└── README.md
```

## Konfiguration

### Supabase

Aktualisieren Sie die Supabase-Konfiguration in Ihrer App (typischerweise in `Program.cs` oder einer Konfigurationsdatei):

```csharp
var supabaseUrl = "https://your-project.supabase.co";
var supabaseKey = "your-anon-key";
```

**WICHTIG**: Verwenden Sie für Production Environment-Variablen oder sichere Konfigurationsmethoden für sensible Daten.

### GitHub Pages Base Path

Der GitHub Actions Workflow konfiguriert automatisch den korrekten Base Path für GitHub Pages. Sie müssen nichts manuell ändern.

## Troubleshooting

### Build-Fehler nach Deployment

- Überprüfen Sie die GitHub Actions Logs unter **Actions** > Ihr Workflow-Run
- Stellen Sie sicher, dass alle NuGet-Pakete korrekt wiederhergestellt werden

### 404-Fehler auf GitHub Pages

- Warten Sie einige Minuten nach dem ersten Deployment
- Überprüfen Sie, ob GitHub Pages in den Repository-Settings aktiviert ist
- Stellen Sie sicher, dass die `.nojekyll`-Datei erstellt wurde

### Supabase-Verbindungsfehler

- Überprüfen Sie Ihre Supabase-URL und API-Key
- Stellen Sie sicher, dass die RLS-Policies korrekt konfiguriert sind
- Überprüfen Sie die Browser-Konsole auf detaillierte Fehlermeldungen

### Organisation kann nicht erstellt werden (403 Fehler)

- Führen Sie die Migration `20250111000001_fix_membership_trigger.sql` aus
- Diese behebt das RLS Policy Problem bei der Membership-Erstellung

## Lizenz

[Ihre Lizenz hier einfügen]

## Kontakt

[Ihre Kontaktinformationen hier einfügen]
