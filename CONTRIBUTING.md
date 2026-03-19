# Contributing

Contributions are welcome — code, documentation, and domain modelling corrections alike.

## Getting started

See the [README](README.md) for prerequisites and how to run the project locally. Before submitting anything, make sure the build and tests pass:

```bash
npm run dotnet:build
npm run dotnet:test
npm run frontend:test
```

## Pull requests

Keep PRs focused on one thing. No need to open an issue first for small changes — just submit the PR. For larger changes or anything that touches the domain model, opening an issue to discuss the approach first saves everyone time.

## Code conventions

- **C#**: See [docs/07-development-process/coding-standards.md](docs/07-development-process/coding-standards.md). Warnings are treated as errors in production code — the build must be clean.
- **Architecture**: New functionality should follow the existing bounded context structure (domain → application → infrastructure). See [docs/07-development-process/adding-a-bounded-context.md](docs/07-development-process/adding-a-bounded-context.md) and [adding-a-functional-slice.md](docs/07-development-process/adding-a-functional-slice.md) for guidance.
- **Commit messages**: See [docs/07-development-process/commit-conventions.md](docs/07-development-process/commit-conventions.md).

## A note on domain modelling

This platform is one developer's interpretation of Heather Marsh's *Binding Chaos*. The domain model reflects a surface reading of the book and will contain misunderstandings. If you have deeper familiarity with the ideas, corrections to the modelling are especially welcome — not just code contributions.
