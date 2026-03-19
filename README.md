# BindingChaos

A software platform implementing the governance and coordination concepts from Heather Marsh's [*Binding Chaos*](https://georgiebc.wordpress.com/binding-chaos/) (2013).

The system enables locality-scoped, stigmergic participation: participants raise signals, propose ideas, amend them collectively, and coordinate real-world actions — all under pseudonymous identity with no personality-driven amplification.

## Status

Under active development. Core bounded contexts (signals, ideation, discourse, locality, tagging, pseudonymity) are functional. The reputation/epistemic community layer described in the book is not yet built.

## Architecture

```
Browser → Web (3000) → Web.Gateway/BFF (4000) → CorePlatform.API (5000)
```

- **Frontend**: React + TypeScript + Vite + Tailwind + Radix UI
- **Gateway**: ASP.NET Core BFF — handles OIDC auth (Keycloak), issues internal JWTs
- **API**: ASP.NET Core — CQRS via Wolverine, Marten (event store) + EF Core (relational)
- **Infrastructure**: PostgreSQL, Redis, RabbitMQ, Keycloak, MinIO

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js 20+](https://nodejs.org/)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)

## Getting Started

```bash
# Install dependencies
npm install

# Start infrastructure (PostgreSQL, Redis, RabbitMQ, Keycloak, MinIO)
npm run infra:start

# Seed all infrastructure with development data
npm run reset:all

# In separate terminals:
npm run api:start        # Core Platform API on :5000
npm run gateway:start    # Web Gateway on :4000
npm run frontend:start   # React dev server on :3000
```

Then open [http://localhost:3000](http://localhost:3000).

## Development

```bash
npm run dotnet:build     # Build entire solution
npm run dotnet:test      # Run all tests
npm run dotnet:format    # Auto-format C# code
npm run frontend:test    # Run frontend tests
npm run frontend:lint    # ESLint
npm run frontend:generate  # Regenerate TypeScript API client from Gateway OpenAPI spec
```

## Project Structure

```
src/
  BindingChaos.Web/                  # React frontend
  BindingChaos.Web.Gateway/          # ASP.NET Core BFF
  BindingChaos.CorePlatform.API/     # Main backend
  BindingChaos.SignalAwareness/      # Signals, amplification, suggested actions
  BindingChaos.Ideation/             # Ideas, amendments, amendment voting
  BindingChaos.CommunityDiscourse/   # Threaded discussion
  BindingChaos.LocalityManagement/   # Geographic locality hierarchy
  BindingChaos.Tagging/              # Tag management
  BindingChaos.Pseudonymity/         # Privacy-preserving participant pseudonyms
  BindingChaos.IdentityProfile/      # Identity storage
  BindingChaos.SharedKernel/         # DDD base types, CQRS primitives
docs/
  binding-chaos-concepts.md          # Concept reference grounded in the book
  design-notes.md                    # Recorded decisions and known gaps
```

## Philosophy

The platform is a direct implementation of Marsh's proposals: governance by user group rather than representative vote, stigmergic coordination where work is accepted or rejected on its merits, and pseudonymity as the default so participation is idea-driven rather than personality-driven.

## License

AGPL-3.0
