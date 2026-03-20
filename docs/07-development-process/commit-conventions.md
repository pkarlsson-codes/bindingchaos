# Commit Conventions

This project follows [Conventional Commits](https://www.conventionalcommits.org/). Each commit message should be structured as:

```
<type>(<scope>): <description>
```

The scope is optional but recommended — use the bounded context name (e.g. `societies`, `ideation`, `signals`) or a layer name (e.g. `gateway`, `frontend`, `shared-kernel`).

## Types

| Type | When to use |
|---|---|
| `feat` | New functionality — new endpoint, UI feature, command, query |
| `fix` | Bug fix |
| `refactor` | Restructuring without behaviour change |
| `test` | Adding or fixing tests |
| `docs` | Documentation only |
| `chore` | Maintenance — dependencies, build scripts, config, tooling |
| `infra` | Docker, Keycloak, seeding, database migrations |
| `revert` | When reverting a previous commit |

## Examples

```
feat(societies): add leave society endpoint
fix(ideation): prevent duplicate amendment votes
refactor(shared-kernel): extract SocietyId value object
test(signals): add integration tests for signal capture
docs: update architecture overview in README
chore: bump .NET SDK to 10.0.200
infra: add Keycloak realm export for societies scope
```

For reverts, reference the commits reverted
```
revert: broke the build

Refs: <sha>, <sha>
```