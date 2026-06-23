# Vehicle Catalog

A web application that lets a user pick a **car make** and a **manufacture year**, then shows the **vehicle types** and **models** available for that selection. Data comes from the public [NHTSA vPIC API](https://vpic.nhtsa.dot.gov/api/).

Built with **ASP.NET Core MVC (.NET 10)** following **Clean Architecture + CQRS**.

---

## Table of contents

1. [Features](#features)
2. [Tech stack](#tech-stack)
3. [Architecture](#architecture)
4. [Solution structure](#solution-structure)
5. [Upstream APIs](#upstream-apis)
6. [Run locally with .NET](#run-locally-with-net)
7. [Run locally with Docker](#run-locally-with-docker)
8. [Configuration](#configuration)
9. [Push to GitHub](#push-to-github)
10. [Deploy to AWS (free tier)](#deploy-to-aws-free-tier)
11. [Design notes](#design-notes)

---

## Features

- Searchable **make** dropdown (every make from NHTSA, filterable as you type).
- **Manufacture year** selector (1995 → next model year).
- Results show **vehicle types** for the make and **models** for the make + year.
- Click a vehicle type to **filter the models** by that type.
- Server-rendered MVC views — works without JavaScript (the type-to-filter box is progressive enhancement).
- Friendly message shown if the NHTSA service is unavailable.
- `/health` endpoint for container/orchestrator health checks.

---

## Tech stack

| Concern | Technology |
|---|---|
| Runtime | .NET 10 (`net10.0`), C# `latest` |
| Web | ASP.NET Core **MVC** (controllers + Razor views) |
| CQRS / mediator | **MediatR 12.x** (Apache-2.0, free) |
| HTTP | `IHttpClientFactory` typed client |
| Container | Docker (multi-stage, non-root, health-checked) |

> All dependencies are public packages from nuget.org — `nuget.config` references only the nuget.org feed.

---

## Architecture

Two projects, with a one-way dependency flow and CQRS inside the Handler project:

```
VehicleCatalog.Web (MVC)  ──►  VehicleCatalog.Handler (CQRS + mediator + NHTSA client)
```

- **Web** is the presentation + composition root: MVC controllers/views, and it wires the Handler services at startup.
- **Handler** holds everything else, organized by folder:
  - `Queries/` — CQRS queries (`IRequest<T>`) + their MediatR handlers.
  - `Models/`, `Services/` — domain models (`Make`, `VehicleType`, `VehicleModel`) and the `INhtsaVehicleClient` contract.
  - `Nhtsa/` — the typed HTTP client implementation and the JSON DTOs.

Request flow for a search:

```
Controller → IMediator.Send(Query) → QueryHandler → INhtsaVehicleClient → NHTSA API → View
```

---

## Solution structure

```
VehicleCatalog.slnx
├── VehicleCatalog.Web        → MVC controllers, Razor views, wwwroot, Program.cs
└── VehicleCatalog.Handler    → CQRS (MediatR queries/handlers), models, NHTSA client
    ├── Queries/               → GetAllMakes / GetVehicleTypesForMake / GetModelsForMakeYear
    ├── Models/ • Services/    → models + INhtsaVehicleClient contract
    ├── Nhtsa/                 → typed HttpClient + JSON DTOs
    └── DI/                    → AddHandlerServices(...)
```

---

## Upstream APIs

The three NHTSA vPIC endpoints used (all JSON):

| Purpose | Endpoint |
|---|---|
| All makes | `…/vehicles/getallmakes?format=json` |
| Vehicle types for a make | `…/vehicles/GetVehicleTypesForMakeId/{makeId}?format=json` |
| Models for make + year (+ optional type) | `…/vehicles/GetModelsForMakeIdYear/makeId/{makeId}/modelyear/{year}[/vehicletype/{type}]?format=json` |

Base URL is configurable (see [Configuration](#configuration)).

---

## Run locally with .NET

**Prerequisite:** [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0).

```bash
# from the repository root
dotnet restore
dotnet build
dotnet run --project VehicleCatalog.Web
```

Then open the URL printed in the console (e.g. `http://localhost:5080`).

> Outbound HTTPS access to `https://vpic.nhtsa.dot.gov` is required at runtime.

---

## Run locally with Docker

**Prerequisite:** Docker.

### Option A — docker compose (recommended)

```bash
docker compose up --build
```

Open **http://localhost:8080**.

### Option B — plain docker

```bash
docker build -t vehicle-catalog:latest .
docker run --rm -p 8080:8080 vehicle-catalog:latest
```

Health check: `curl http://localhost:8080/health` → `Healthy`.

---

## Configuration

Settings live in `VehicleCatalog.Web/appsettings.json` and can be overridden by environment variables (double-underscore syntax).

| Setting | Env var | Default |
|---|---|---|
| NHTSA base URL | `Nhtsa__BaseUrl` | `https://vpic.nhtsa.dot.gov/api/` |

Example:

```bash
docker run --rm -p 8080:8080 -e Nhtsa__BaseUrl=https://vpic.nhtsa.dot.gov/api/ vehicle-catalog:latest
```

---

## Push to GitHub

> This repository is not yet connected to a remote. Create the repo and push it yourself:

```bash
# from the repository root (already a local git repo if you ran `git init`)
git add .
git commit -m "Initial commit: Vehicle Catalog (ASP.NET Core MVC, Clean Architecture)"

# create an EMPTY repo on github.com first (no README), then:
git remote add origin https://github.com/<your-username>/vehicle-catalog.git
git branch -M main
git push -u origin main
```

---

## Deploy to AWS (free tier)

Pick **one** path. Both run the same Docker image; do the actual deploy with your own AWS account.

### Path 1 — EC2 free tier (`t2.micro` / `t3.micro`)

The most clearly "free-tier" option.

1. **Launch an instance**: Amazon Linux 2023, `t2.micro` (or `t3.micro`), in the free tier. Create/download a key pair.
2. **Security group**: allow inbound **TCP 80** (and 22 for SSH) from your IP / anywhere.
3. **SSH in** and install Docker:
   ```bash
   sudo dnf update -y
   sudo dnf install -y docker git
   sudo systemctl enable --now docker
   sudo usermod -aG docker ec2-user   # re-login after this
   ```
4. **Get the code** (clone your GitHub repo) and run it on port 80:
   ```bash
   git clone https://github.com/<your-username>/vehicle-catalog.git
   cd vehicle-catalog
   docker build -t vehicle-catalog:latest .
   docker run -d --restart unless-stopped -p 80:8080 --name vehicle-catalog vehicle-catalog:latest
   ```
5. Browse to `http://<EC2-public-IP>`.

> Tip: outbound HTTPS (to NHTSA) is allowed by default. Keep the instance in the free-tier hours budget.

### Path 2 — Container service (App Runner or ECS Fargate)

1. Build and push the image to **Amazon ECR**:
   ```bash
   aws ecr create-repository --repository-name vehicle-catalog
   aws ecr get-login-password --region <region> | docker login --username AWS --password-stdin <acct>.dkr.ecr.<region>.amazonaws.com
   docker build -t <acct>.dkr.ecr.<region>.amazonaws.com/vehicle-catalog:latest .
   docker push <acct>.dkr.ecr.<region>.amazonaws.com/vehicle-catalog:latest
   ```
2. **App Runner**: create a service from the ECR image, port **8080**, health-check path **`/health`**. App Runner provisions HTTPS and a public URL.
   *(ECS Fargate is an alternative; note Fargate is not in the always-free tier.)*

In all cases the container listens on **8080** and exposes **`/health`** for the load balancer / service health check.

---

## Design notes

- **Right-sized structure.** The solution is intentionally kept to **two projects** — `Web` (MVC) and `Handler` (CQRS + mediator + NHTSA client) — with no unnecessary ceremony: no database (so no migrations), no result-envelope, no validation pipeline. Handlers return their data directly.
- **CQRS via MediatR.** Queries and handlers run through **MediatR** (12.x, free Apache-2.0); controllers depend only on `IMediator`.
- **Analyzers.** .NET analyzers run at the "Recommended" level and the solution builds with **zero warnings**.
