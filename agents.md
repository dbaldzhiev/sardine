# Sardine – Agent Architecture

This document defines the conceptual “agents” inside Sardine (Parking Solver for Rhino 8 / Grasshopper).  
Agents are not AI entities; they are logical responsibility holders that separate concerns, simplify debugging, and make future extensions possible.

The solver should be understood as a **coordinated system of deterministic agents operating on shared geometry**.

---

## 1. Solver Agent (Core Orchestrator)

**Responsibility**
- Owns the full solve cycle.
- Validates inputs.
- Coordinates all other agents.
- Produces final geometry and metadata.

**Inputs**
- Plot boundary (single closed polyline only)
- Access points
- Axial road lines
- Spot type definitions
- Optional overrides (roads, pedestrian, special spots)

**Outputs**
- Spot instances
- Road instances
- Pedestrian geometry
- Visualization geometry (optional)

**Rules**
- Only one Solver Agent instance per Grasshopper canvas.
- No geometry mutation after final solve phase.
- Fails fast on invalid plot geometry.

---

## 2. Boundary Agent

**Responsibility**
- Interprets and preprocesses the plot boundary.
- Handles edge indexing, culling, offsets, and tolerances.

**Key Behaviors**
- Identifies usable edges for outer spots.
- Applies skirt offset.
- Applies edge culling and side-out logic.

**Constraints**
- Accepts only a single closed polyline.
- No inner holes or exclusion zones.
- No self-intersections.

---

## 3. Access Agent (Vehicular)

**Responsibility**
- Processes vehicular access points.
- Aligns access to internal road logic.

**Key Behaviors**
- Snaps access to optimal locations (if enabled).
- Validates access widths.
- Ensures connectivity to axial or perimetral roads.

---

## 4. Road Agent

**Responsibility**
- Generates circulation geometry.
- Subdivides the plot into parking regions.

**Inputs**
- Axial lines
- Road settings
- Boundary interpretation

**Outputs**
- Road centerlines
- Road widths
- Island geometry

**Notes**
- No vertical logic.
- No structural logic.
- Roads are planar and abstract.

---

## 5. Spot Placement Agent

**Responsibility**
- Generates parking spots based on typology.
- Handles rotation, stacking, and distribution.

**Spot Types**
- Normal
- Handicap
- Elderly
- Bike (delegated selection)

**Rules**
- Outer and inner spots are treated separately.
- Spot dimensions are absolute.
- Replacement by percentage is stochastic but bounded.

---

## 6. Special Spot Agent

**Responsibility**
- Overrides default spot logic.
- Ensures compliance with accessibility constraints.

**Notes**
- This agent never increases total spot count.
- It replaces existing spots based on percentage rules.

---

## 7. Pedestrian Agent

**Responsibility**
- Generates pedestrian circulation.
- Ensures connectivity between access points and parking areas.

**Modes**
- Roadside lanes
- Sidewalk-style circulation

**Constraints**
- No conflict resolution with vehicle paths (yet).
- No level changes.

---

## 8. Bike Agent

**Responsibility**
- Converts selected islands/spots into bicycle parking.
- Operates partially interactively.

**Key Traits**
- User-guided picking.
- Percentage-based insertion.
- Geometry-aware spacing.

---

## 9. Visualization Agent

**Responsibility**
- Produces preview geometry only.
- Has zero influence on solving logic.

**Notes**
- Can be disabled for performance.
- Should never be relied on for data extraction.

---

## 10. Info Agents (Deconstructors)

**Responsibility**
- Expose internal data in a stable format.
- Provide read-only access to solver results.

**Examples**
- Spots Info
- Roads Info

---

## Design Philosophy

- Deterministic first, optimization later.
- Explicit constraints beat implicit assumptions.
- Geometry is data; visualization is optional.
- Failure states should be obvious, not silent.

