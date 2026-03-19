# Sybil Resistance via Trust Networks

## Overview

Sybil resistance emerges from the trust graph: new accounts with no trust connections have no reach. Spam and astroturfing are ineffective without real trust relationships — not because entry is gated, but because unconnected nodes are invisible.

This approach is directly inspired by Heather Marsh's Getgee project and is consistent with the pseudonymity and approval economy principles in *Binding Chaos*.

---

## Core Model

A participant may explicitly trust another participant. Trust is directional: A trusting B does not imply B trusts A.

**Degrees of trust** determine visibility:

| Degree | Meaning |
|--------|---------|
| 0 | Only content you have explicitly trusted yourself |
| 1 | Also includes content trusted by participants you trust |
| 2–3 | Transitive expansion — practical default range |
| 4–5 | Near-complete coverage; filters only isolated/unconnected nodes |

A sybil cluster — a set of fake accounts — is only effective if it has real trust connections. Without them, its signals and amplifications are invisible to the wider network.

---

## Relationship to Shunning

Trust and shunning are two sides of the same mechanism:

- **Trusting** a participant causes their contributions to propagate through your trust graph
- **Shunning** a participant removes their contributions from your view and signals to your trust network that they should not be trusted

Shunning is the primary sanction in Marsh's model. It requires no central authority — the user group collectively removes amplification from bad actors by withdrawing trust.

---

## Storage

A single join table is sufficient for the trust graph:

```
TrustRelationship
  TrusterId   (ParticipantId)
  TrusteeId   (ParticipantId)
  CreatedAt
```

Shunning relationships use the same structure with a separate table or a discriminator flag, depending on whether shunning needs to propagate (e.g. "participants shunned by people I trust are also filtered").

---

## Graph Traversal

Trust-degree queries are recursive graph traversals. PostgreSQL supports these natively via recursive CTEs:

```sql
WITH RECURSIVE trust_graph AS (
  -- Degree 0: the participant themselves
  SELECT @participantId AS trusted_id, 0 AS degree
  UNION ALL
  -- Expand outward one degree at a time
  SELECT tr.trustee_id, tg.degree + 1
  FROM trust_relationships tr
  JOIN trust_graph tg ON tr.truster_id = tg.trusted_id
  WHERE tg.degree < @maxDegree
)
SELECT DISTINCT trusted_id FROM trust_graph;
```

This set of `trusted_id` values is then used to filter signals, amplifications, and content — showing only items that have been touched by participants within the trust radius.

---

## Integration Points

- **SignalAwareness**: amplification weight and signal visibility filtered by trust degree
- **Ideation**: amendment endorsement visibility
- **CommunityDiscourse**: post visibility
- **Amplification weighting**: once epistemic communities exist, amplifications from trusted experts carry more weight than amplifications from peripheral nodes (see [design-notes.md](../design-notes.md) — Amplification Is Unweighted)
- **Approval economy**: trust graph is the substrate for societal approval standing

---

## Bounded Context

Trust relationships belong in a dedicated `Reputation` (or `TrustGraph`) bounded context, not in `Pseudonymity` or `IdentityProfile`. The `Pseudonymity` context handles identity masking; the `Reputation` context handles standing within the network.

---

## Implementation Sequence

1. `TrustRelationship` aggregate and repository (simple write side)
2. `ShunRelationship` aggregate and repository
3. Trust graph query service (recursive CTE, cacheable per participant)
4. Filter integration into `GetSignals` query (degree-scoped amplification visibility)
5. Extend to Ideation and Discourse as the bounded context matures
6. Amplification weighting once epistemic communities exist

---

## Testing Strategy

- Unit tests for trust graph traversal logic
- Integration tests verifying that unconnected participants' content is not surfaced
- Integration tests verifying shunning propagation
- Performance tests for recursive CTE at realistic graph sizes
