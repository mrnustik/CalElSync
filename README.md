# Cal El Sync

[![.NET](https://github.com/mrnustik/CalElSync/actions/workflows/build.yml/badge.svg)](https://github.com/mrnustik/CalElSync/actions/workflows/build.yml)
[![Docker](https://github.com/mrnustik/CalElSync/actions/workflows/docker.yml/badge.svg)](https://github.com/mrnustik/CalElSync/actions/workflows/docker.yml)

Calendar to task manager synchronization tool. Syncs iCal calendar events into Todoist as tasks on a configurable schedule.

## Docker Setup

### 1. Create a calendars mapping file

Create a `calendars.json` file that maps iCal URLs to Todoist project IDs:

```json
[
  {
    "calendarUrl": "https://example.com/calendar.ics",
    "projectId": "your-todoist-project-id"
  }
]
```

### 2. Run with Docker Compose

Copy `docker-compose.yml` and edit the environment variables:

```yaml
services:
  calelsync:
    image: ghcr.io/mrnustik/calelsync:main
    restart: unless-stopped
    volumes:
      - ./calendars.json:/app/calendars.json:ro
    environment:
      - Todoist__ApiKey=your-todoist-api-key
      - CalendarsFile__FilePath=/app/calendars.json
      - Sync__CronExpression=0 2 * * 1
```

Then start the container:

```bash
docker compose up -d
```

### Configuration

All settings can be configured via environment variables using `__` (double underscore) as the section separator:

| Environment variable | Description | Default |
|---|---|---|
| `Todoist__ApiKey` | Todoist API key | _(required)_ |
| `CalendarsFile__FilePath` | Path to the calendars JSON mapping file | _(required)_ |
| `Sync__CronExpression` | Cron expression for the sync schedule | `0 2 * * 1` (Mondays 02:00 UTC) |

### Manual trigger

The container exposes an HTTP endpoint to trigger a sync manually:

```bash
curl -X POST http://localhost:8080/sync
```
