# Documentation Implementation Guide

Use this when creating or updating project docs.

## Goal

Help someone take the correct action quickly.

## Decide If A New Doc Is Needed

1. Check if a canonical doc already exists.
2. If it exists, update that doc instead of creating a new one.
3. Create a new doc only when introducing new process, decisions, or operational steps.

## Required Structure

Write sections in this order unless there is a clear reason not to:

1. Purpose
2. Scope
3. Prerequisites
4. Steps
5. Validation
6. Troubleshooting
7. Related docs

## Writing Rules

- Be concise. Prefer bullets and short sections.
- Document implemented behavior, not aspirational design.
- Keep one source of truth. Link instead of duplicating.
- Use explicit language: `must` and `must not` for requirements.
- State assumptions and constraints directly.
- Keep examples aligned with the current codebase.

## Include

- Ownership and boundaries.
- Exact expected outcomes.
- Decision rules (`if X, do Y`).
- Definition of done.

## Avoid

- Long narrative without actionable steps.
- Repeated content across files.
- Vague wording (`might`, `usually`, `etc.`).
- Claims that are not implemented.

## Merge Checklist

- Purpose and quick path are visible at the top.
- A contributor can execute the process without guessing.
- Commands, paths, and names are current.
- Validation steps are testable.
- Related docs are linked.
- Duplicate content is removed.

## Minimal Template

```md
# Title

## Purpose

## Scope

## Prerequisites

## Steps

## Validation

## Troubleshooting

## Related docs
```
