# ASP.NET Core Todo List (MVC + Web API)

Full-stack ASP.NET Core todo list application with a Razor MVC web UI and a RESTful Web API backend. It uses EF Core with SQLite for persistence, ASP.NET Core Identity for user authentication, and supports CRUD for lists and tasks, assignment by email, task search, and an “Assigned to me” virtual list.

## Key features

- **Authentication (WebApp)**: user registration/login/logout (ASP.NET Core Identity + cookies)
- **Todo lists**: create, edit, delete, view your lists
- **Tasks**: create, edit, delete, view details; due dates and status
- **Assignment by email**: creator can assign/reassign tasks by assignee email (mapped to user GUID in `users.db`)
- **Permissions model**:
  - **Creator** can edit all task fields and reassign
  - **Assignee** can update **status only**
- **Assigned tasks**:
  - “My Tasks” page shows tasks assigned to you
  - “My Lists” includes a **virtual list** **Assigned to me** (no DB record)
- **Search**: find tasks by title and date ranges (created/due)

## Tech stack

- **.NET**: ASP.NET Core (.NET 8)
- **UI**: Razor Views (MVC) + Bootstrap 5
- **API**: ASP.NET Core Web API
- **Data**: Entity Framework Core + SQLite
- **Auth**:
  - WebApp: ASP.NET Core Identity (cookies)
  - WebApi: API key (`Authorization: Bearer <key>`)

## Architecture overview

This solution contains two applications:

- **`TodoListApp.WebApp`**: MVC web application (browser UI) + Identity user store (`users.db`)
- **`TodoListApp.WebApi`**: REST API backend + domain data store for lists/tasks (`todolist.db`)

The WebApp communicates with the WebApi via typed `HttpClient` services and sends an API key in the `Authorization: Bearer <key>` header.

## Screenshots (add yours here)

Put screenshots into `docs/screenshots/` and link them here:

- **Login**: `docs/screenshots/login.png`
- **Register**: `docs/screenshots/register.png`
- **My Lists**: `docs/screenshots/lists.png`
- **Tasks in a List (status colors)**: `docs/screenshots/tasks.png`
- **Assign by email**: `docs/screenshots/assign-by-email.png`
- **Virtual list: Assigned to me**: `docs/screenshots/assigned-virtual-list.png`
- **My Tasks (assigned)**: `docs/screenshots/assigned-tasks.png`
- **Assignee can edit status only**: `docs/screenshots/assignee-status-only.png`
- **Task details**: `docs/screenshots/task-details.png`
- **Search**: `docs/screenshots/search.png`

## Demo scenario (recommended)

To generate a clean, recruiter-friendly dataset and screenshots, follow:

- `docs/demo-script.md`

## How to run locally

### Prerequisites

- .NET SDK 8

### 1) Run the Web API

Start **`TodoListApp.WebApi`** (default: `https://localhost:7001`).

### 2) Run the Web App

Start **`TodoListApp.WebApp`** and open the URL shown in the console (default: `https://localhost:7262`).

## Configuration

- **WebApp** (`TodoListApp.WebApp/appsettings.json`)
  - `WebApi:BaseUrl` (default: `https://localhost:7001/`)
  - `WebApi:ApiKey`
  - `ConnectionStrings:UsersDb` (default SQLite file: `users.db`)

- **WebApi** (`TodoListApp.WebApi/appsettings.json`)
  - `Authentication:ApiKey`
  - `ConnectionStrings:TodoListDb` (default SQLite file: `todolist.db`)

## Project structure

```
TodoListApp.WebApp/        # MVC UI + Identity (users.db)
TodoListApp.WebApi/        # REST API + EF Core (todolist.db)
docs/                      # documentation assets
docs/screenshots/          # screenshots referenced by README
```
