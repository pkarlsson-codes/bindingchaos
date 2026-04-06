# Societies — Design Discussion

## The question we started with

The existing Societies BC felt disconnected from the rest of the system. Commons + UserGroups already covered the full governance flow (signal → commons → user group → project), so what role did Societies play? And what did the SocialContract actually govern?

---

## What Marsh's writing actually says

### Primary source

The clearest statement of the model is the diagram in the "Society vs dissociation" chapter (p. 39 of the PDF):

- A large green circle = **Society**
- Inside it, smaller overlapping coloured circles = **Systems/Commons** (Food, Shelter, Health, Transport, Communication, Assembly, Makerlabs, Games)
- Individual people scattered throughout, each participating in multiple systems

Marsh writes:

> "In it, every society includes the entire user group and no one but the user group. No one outside the user group can control the activity inside. Access is restricted for none. Information is available to all through transparency. Education is available to all from their peers, and through epistemic communities with knowledge bridges. Anyone can submit work through concentric user groups or stigmergy."

And from the end of the "Problems with democracy" chapter:

> "We can govern by user groups, respect individual rights and global commons, and collaborate using stigmergy. We can belong to overlapping societies voluntarily by acceptance of social contracts."

### Society vs User Group — are they the same?

No, but they are related. From the Glossary:

- **User group**: "The entire population which will be affected by an action, including no one not affected."
- **Society**: A tightly bound network of heavily interdependent relationships between people.

A user group is defined by *affectedness* — functional, emergent, not chosen. You are in a user group because you are affected by a system. You do not join it; you are in it by virtue of being affected.

A society is defined by *shared values and a social contract* — voluntary, by agreement. You join by accepting the contract.

In practice they converge: a society that governs a set of systems will have membership that is roughly coterminous with the people affected by those systems. But the distinction matters: affectedness is the raw condition; the social contract is the formalisation.

Our existing `AffectednessIndicated` event on Commons is the closest implementation of the raw user group concept. Society membership is the voluntary, formalised version of that same relationship.

### What the social contract actually governs

The social contract is **not** a procedural rulebook (no ratification thresholds, voting windows, or veto rules). It is a **statement of principles** — the rights and values all members agree to uphold.

Marsh's reference point is the 1948 UDHR: "shockingly complete and beautiful in its simplicity... in simple, pure and universal terms upon which law can be based."

From "Natural and negotiated rights":

> "The rights in these documents form the social contract between individuals and society. Each individual agrees that they will work for the greater good of the society and protect the individual rights of others in exchange for having their own rights protected."

If a governing body acts against the social contract, members can declare it void. This is the revocation mechanism — not a voting procedure, but a legitimacy withdrawal.

**Implication:** Our existing `SocialContract` aggregate with `DecisionProtocol` (ratificationThreshold, reviewWindowHours, allowVeto) models governance procedures, not a social contract in Marsh's sense. It is closer to what our own ubiquitous language doc calls a `Charter`. The naming is misleading.

### The concentric circles

Concentric circles exist in Marsh's model but describe **amplification of expertise within a user group**, not a two-tier governance structure. From the text:

> "Experts peer-promoted to the centre receive greatest amplification. Their findings are audited, amplified and explained to the general public by outer circles."

The circles are not hierarchical — the inner ring has no coercive authority over the outer. The user group (everyone affected) retains the power to shun any expert. The epistemic community is a knowledge resource, not a governing body.

Marsh names four concentric tiers: users (audit/feedback), contributors (periodic work submission), members (peer-accepted, contributing), epistemic core (recognised expertise). These are unmodelled in our current system and represent a significant gap.

### Competing societies around the same Commons

Yes — Marsh's model explicitly supports this. Multiple societies can wrap the same Commons. Their members are different people with different social contracts, both caring about the same system. The work that gets accepted and built upon by the broader user group wins (stigmergy). No authority adjudicates between competing approaches.

---

## How Societies fit into the existing model

**Society is the outer container. Commons are the inner circles.**

A Society wraps a set of Commons. It does not replace UserGroups or the stigmergic work process. It provides:

1. **A scoped feed** — members see signals and concerns related to their Society's Commons, not the whole world
2. **A social contract** — shared principles everyone has agreed to (a statement of values, not voting rules)
3. **Shunning** — the community's only enforcement tool; violating the social contract means losing access to the Society's Commons

The Society does **not**:
- Vote on which actions to take
- Tell UserGroups what to do
- Grant or withhold permission to contribute (stigmergy requires no permission)

### The missing relationship

The critical gap in the current implementation: **Society has no relationship to Commons**. Right now Society and Commons are completely disconnected. This is the structural problem.

The correct relationship is **many-to-many**: a Society wraps many Commons, and a Commons can be wrapped by many Societies (e.g., "Södermalm Residents" and "Stockholm Environmental Network" both care about the Water Quality Commons).

### How a Society forms

Not independently — a Society should form *around* Commons.

1. One or more existing Commons are nominated as the systems this society manages
2. A social contract is written — a short statement of principles
3. People who have already declared affectedness to those Commons are the natural membership pool
4. Anyone can join by accepting the social contract (open, stigmergic)
5. No approval required — anyone can propose a society

### Practical day-to-day for a member

- Default signal/concern feed scoped to the Society's Commons
- Can see what UserGroups exist within the Society and what they're working on
- Can declare affectedness to a specific Commons and join the work
- Can submit ideas or work via stigmergy — no permission required
- Subject to shunning by the Society's social contract if they violate its principles

---

## What needs to change in the implementation

### Keep
- `Society` aggregate with members and social contract
- `Membership` as the join mechanism
- Geographic bounds as optional (not definitional)

### Add
- `Society` → `Commons` relationship (many-to-many) — this is the load-bearing missing link
- Society-scoped feed filtering (signals/concerns surfaced by Commons membership)

### Rename / rethink
- `SocialContract.DecisionProtocol` — this models governance procedures, not a social contract. Consider replacing with a plain text `Principles` field, or renaming the aggregate to `Charter` if procedural rules are kept at all.

### Future (not now)
- Concentric tiers within UserGroup (users, contributors, members, epistemic core) — significant gap relative to Marsh's model, but not blocking anything current
- Shunning as an explicit domain action
- Social contract revocation mechanism
