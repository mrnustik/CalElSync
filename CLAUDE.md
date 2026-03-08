# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Build
dotnet build

# Run all tests
dotnet test

# Run a single test
dotnet test --filter "FullyQualifiedName~SynchronizeCalendarEventsToTasksTests.RunSynchronization_WithEmptyTaskProject_CreatesThemAll"

# Run tests in a specific class
dotnet test --filter "ClassName~SynchronizeCalendarEventsToTasksTests"

# Build Docker image
docker build -t calelsync .

# Run with Docker Compose
docker compose up -d
```

## Architecture

CalElSync is an ASP.NET Core (.NET 10) worker service that syncs iCal calendar events into Todoist as tasks. The sync runs on a configurable cron schedule and can also be triggered via an HTTP POST to `/sync`. It is deployed via Docker.

### Project structure

| Project | Namespace prefix | Role |
|---|---|---|
| `CalElSync` | `CalElSync.Core` | Core domain: interfaces, models, and the `SynchronizeCalendarEventsToTasks` use case |
| `CalElSync.Events.iCal` | `CalElSync.Events.iCal` | iCal implementation of `IEventsImportService` — downloads `.ics` files and parses events |
| `CalElSync.Tasks.Todoist` | `CalElSync.Tasks.Todoist` | Todoist REST API client implementing `ITaskRepository` |
| `CalElSync.Host` | `CalElSync.Host` | ASP.NET Core host — wires DI together, runs the cron scheduler, and exposes the `/sync` HTTP endpoint |
| `CalElSync.Tests` | `CalElSync.Tests` | xUnit tests using FluentAssertions and in-memory mocks |

### Core abstractions (in `CalElSync.Core`)

- `IEventsImportService` — fetches events from a calendar URL within a `DateTimeInterval`
- `ITaskRepository` — reads/creates tasks in a project (identified by a string project ID)
- `ICalendarProjectMappingProvider` — returns the list of `CalendarProjectMapping` (calendar URL → project ID pairs)
- `ISynchronizeCalendarEventsToTasks` — the single use case; implemented by `SynchronizeCalendarEventsToTasks`

### Sync logic

For each calendar-to-project mapping, the use case:
1. Fetches iCal events in the configured interval
2. Fetches existing tasks for that project
3. Creates a task for every event that doesn't already have a matching task (matched by **title AND datetime**)

### Configuration

The host reads the following configuration sections:
- `Todoist` → `TodoistApiOptions` (API key)
- `CalendarsFile` → `JsonCalendarMappingOptions` (path to a JSON file with calendar-to-project mappings)
- `Sync` → `SyncScheduleOptions` (cron expression, default: `0 2 * * 1` — every Monday at 02:00)

### Testing approach

Tests use in-memory implementations of all three core interfaces. `InMemoryFileDownloader` serves a bundled `TestingCalendar.ics` embedded resource, enabling fully offline integration tests of the full sync use case.
