# ASE Foundations — Day 9 CI/CD Exercise (.NET Core + React)

## Project: Task Manager — Full-Stack Deployment

A simple Task Manager application with a **React frontend** and **ASP.NET Core Web API backend**, containerized with Docker and ready for CI/CD deployment to Railway.

---

## Part 1: Local Setup & Verification

Before deploying anything, get the app running locally.

### Prerequisites

- **Docker Desktop** installed and running
- **Git** installed
- **GitHub account**
- **Docker Hub account** (free: https://hub.docker.com/signup)
- **Railway account** (free: https://railway.app — sign up with GitHub, no card required)

### Run Locally with Docker Compose

```bash
# 1. Clone this repo
git clone <your-repo-url>
cd <your-repo>

# 2. Build and start both containers
docker-compose up --build

# 3. Open in browser
#    Frontend: http://localhost:3000
#    Backend API: http://localhost:8000/api/tasks

# 4. Stop containers
docker-compose down
```

### Project Structure

```
├── backend/                 # ASP.NET Core Web API
│   ├── Controllers/         # TasksController with full CRUD
│   ├── Models/              # TaskItem, DTOs
│   ├── Data/                # EF Core DbContext (SQLite)
│   ├── Program.cs           # App startup: CORS, EF, auto-migrate
│   ├── Dockerfile           # Multi-stage: SDK build → runtime
│   └── TaskApi.csproj
│
├── frontend/                # React SPA
│   ├── src/                 # App.js with full CRUD UI
│   ├── Dockerfile           # Multi-stage: npm build → nginx serve
│   └── nginx.conf           # Proxies /api/ to backend container
│
├── docker-compose.yml       # Spins up both containers locally
├── .github/workflows/
│   └── deploy.yml           # CI/CD pipeline (INCOMPLETE — your task)
└── README.md
```

---

## Part 2: Your Task

You need to take this working local application and deploy it to the internet using GitHub Actions and Railway. This involves 4 stages:

### Stage 1: Push to GitHub (10 marks)

1. Create a new **public** GitHub repository
2. Initialize git, commit the code, and push to `main`
3. Verify the code is visible on GitHub

### Stage 2: Containerize and Verify (20 marks)

1. Run `docker-compose up --build` locally and confirm:
   - Backend responds at `http://localhost:8000/api/tasks`
   - Frontend loads at `http://localhost:3000`
   - You can create, read, update, and delete tasks through the UI
2. Build each image individually and verify:
   ```bash
   docker build -t taskmanager-backend ./backend
   docker build -t taskmanager-frontend --build-arg REACT_APP_API_URL=/api ./frontend
   ```

### Stage 3: Complete the CI/CD Pipeline (40 marks)

Open `.github/workflows/deploy.yml` — it has TODO placeholders. Complete all of them:

1. **Test job**: Run `dotnet test` in the backend directory
2. **Docker Hub login**: Use `docker/login-action@v3` with your secrets
3. **Build & push backend**: Build the backend Dockerfile and push to Docker Hub
4. **Build & push frontend**: Build the frontend Dockerfile (with `REACT_APP_API_URL` build arg) and push to Docker Hub
5. **Deploy**: Use the Railway CLI to redeploy both services from their latest Docker Hub images

You must also set up these **GitHub Repository Secrets** (Settings → Secrets and variables → Actions):
- `DOCKER_USERNAME` — your Docker Hub username
- `DOCKER_PASSWORD` — your Docker Hub access token (create a PAT in Docker Hub; never commit it)
- `RAILWAY_TOKEN` — your Railway API token (Dashboard → Account Settings → Tokens → Create Token)
- `RAILWAY_PROJECT_ID` — your Railway project UUID (Project → Settings, or from the project URL)
- `RAILWAY_SERVICE_ID_BACKEND` — backend service ID (service → Settings → Service ID)
- `RAILWAY_SERVICE_ID_FRONTEND` — frontend service ID (service → Settings → Service ID)
- `RAILWAY_BACKEND_API_URL` — browser-facing backend base URL, e.g. `https://your-backend.up.railway.app/api` (set this after the backend service has a public URL, then push again so the frontend image is built with the correct API URL)

**How to get your Railway Token:**
1. Go to https://railway.app → click your profile (bottom left) → Account Settings
2. Click "Tokens" → "Create Token" → name it "github-actions"
3. Copy the token and add it as a GitHub secret

### Stage 4: Deploy to Railway (30 marks)

1. Create a **Railway account** at https://railway.app (sign up with GitHub — no card required)
2. Create a **New Project** → **Empty Project**
3. Inside the project, create **two services**:
   - **Backend service**: Click "+ New" → "Docker Image" → enter `<dockerhub-username>/taskmanager-backend:latest`. Under Settings → Networking → generate a public domain. Set the port variable to `8000`. Copy **Settings → Service ID** into the `RAILWAY_SERVICE_ID_BACKEND` GitHub secret.
   - **Frontend service**: Click "+ New" → "Docker Image" → enter `<dockerhub-username>/taskmanager-frontend:latest`. Under Settings → Networking → generate a public domain. Set the port variable to `80`. Copy **Service ID** into `RAILWAY_SERVICE_ID_FRONTEND`.
4. After both services are deployed and have public URLs:
   - The frontend must call the backend from the browser. **Option A:** Update `nginx.conf` to proxy `/api/` to your Railway backend URL instead of `http://backend:8000`, then rebuild and push the frontend image.
   - **Option B (used by this repo’s GitHub Actions):** Set the `RAILWAY_BACKEND_API_URL` secret to your backend’s public API base (for example `https://<your-backend>.up.railway.app/api`). The workflow passes it as `REACT_APP_API_URL` when building the frontend image. After the backend URL exists, add the secret and push to `main` again so a new frontend image is built.
5. Push a commit to `main` and verify the full pipeline:
   - GitHub Actions: ✅ tests pass → ✅ images built and pushed → ✅ Railway redeploy triggered
   - The live app is accessible via your Railway frontend URL and full CRUD works

---

## Part 3: Deliverables

| # | Deliverable | Marks |
|---|-------------|-------|
| 1 | GitHub repository with all code committed and pushed | 10 |
| 2 | Docker Compose runs locally — both containers working, full CRUD functional | 20 |
| 3 | Completed `.github/workflows/deploy.yml` — all TODOs filled, pipeline passes on GitHub Actions | 40 |
| 4 | Live app deployed on Railway — accessible via URL, CRUD works end-to-end | 30 |
| | **Total** | **100** |

### Submission

Submit the following:
1. Link to your **GitHub repository**
2. Link to your **live app on Railway** (frontend URL)
3. Screenshot of a **successful GitHub Actions pipeline run** (green checkmarks)

---

## Part 4: Concepts You Should Understand

After completing this exercise, you should be able to answer:

1. What is the difference between a Docker image and a container?
2. Why do we use a multi-stage Dockerfile for both the frontend and the backend?
3. What does `docker-compose up` do vs `docker build` + `docker run`?
4. What are the three stages of a CI/CD pipeline (build, test, deploy)?
5. Why do we store secrets in GitHub Secrets instead of hardcoding them?
6. What happens when Railway pulls a new Docker image — what does it do with the old container?
7. Why does the frontend nginx need to proxy `/api/` requests to the backend?
8. What happens if the test job fails — does the deploy still run? Why not?

---

## Rules

- **AI can be used for reading documentation only — NOT for writing pipeline code**
- Google, Docker docs, GitHub Actions docs, and Railway docs are allowed
- If you get stuck on a specific step, ask your senior mentor — not AI
- Each commit should have a clear, descriptive message
# TaskApp_PinaCloids
