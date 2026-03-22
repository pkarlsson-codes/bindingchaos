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

Trust-degree queries are recursive graph traversals — "find all participants within N degrees of X." These are called frequently, gating visibility across SignalAwareness, Ideation, CommunityDiscourse, and the approval economy.

**Storage: Neo4j.** Trust relationships are stored in Neo4j rather than PostgreSQL. Recursive graph traversal is Neo4j's native strength; the equivalent PostgreSQL recursive CTE degrades non-linearly as the network grows. Shunning propagation (e.g. "filter anyone shunned by someone I trust") adds multi-hop queries that compound this.

Neo4j runs in Docker Compose alongside the existing infrastructure (port 7474 browser UI, 7687 Bolt). The .NET integration uses `Neo4j.Driver`. A repository interface abstracts the storage so the rest of the domain is unaware of Neo4j specifically.

A degree-scoped trust query in Cypher:

```cypher
MATCH (start:Participant {id: $participantId})-[:TRUSTS*1..{maxDegree}]->(trusted:Participant)
RETURN DISTINCT trusted.id
```

This set of participant IDs is then used to filter signals, amplifications, and content — showing only items touched by participants within the trust radius.

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
3. Trust graph query service (Neo4j traversal, cacheable per participant)
4. Filter integration into `GetSignals` query (degree-scoped amplification visibility)
5. Extend to Ideation and Discourse as the bounded context matures
6. Amplification weighting once epistemic communities exist

---

## Testing Strategy

- Unit tests for trust graph traversal logic
- Integration tests verifying that unconnected participants' content is not surfaced
- Integration tests verifying shunning propagation
- Performance tests for Neo4j traversal at realistic graph sizes
