# Sardine – Usage & Operating Instructions

This document explains how to correctly use Sardine (Parking Solver) and, more importantly, how **not** to use it.

Read this before assuming something is a bug.

---

## 1. What Sardine Is

Sardine is a **layout automation tool** for open parking plots.

It is designed to:
- Explore many layout variants quickly
- Provide consistent geometric logic
- Reduce repetitive drafting work

It is **not**:
- A regulatory checker
- A structural or vertical circulation solver
- A detailed construction design tool

---

## 2. Geometry Requirements (Strict)

- One closed planar polyline only
- No holes
- No inner exclusion zones
- No self-intersections
- No stacked or overlapping plots

If your plot violates any of the above, results are undefined.

---

## 3. Core Workflow

1. Define the plot boundary.
2. Define vehicular access points.
3. (Optional) Define axial road lines.
4. Define spot typologies.
5. Adjust road, pedestrian, and special spot settings.
6. Toggle visualization if needed.
7. Read results through Info components.

Do not jump ahead. The solver assumes this order.

---

## 4. Parking Solver Component Rules

- Use exactly one Parking Solver per definition.
- Do not feed transformed geometry back into the solver.
- Do not post-process solver output and expect stability.

The solver owns the geometry lifecycle.

---

## 5. Spot Logic Notes

- Outer and inner spots are solved separately.
- Percentages are applied after feasibility checks.
- Stacking only applies to angled spots.
- Handicap and elderly spots override standard spots.

Spot counts are a result, not a target.

---

## 6. Roads Logic Notes

- Axial roads subdivide the plot.
- Perimetral states control boundary usage.
- Access width is enforced globally.
- Island radius is a maximum, not a guarantee.

Roads are abstract circulation guides, not lane-accurate designs.

---

## 7. Pedestrian Logic Notes

- Pedestrian access is optional but recommended.
- Styles affect topology, not legality.
- Pedestrian paths do not currently resolve conflicts with vehicles.

Use with intent, not assumption.

---

## 8. Bike Spots Notes

- Bike spots are selected, not inferred.
- Percentages are relative to total spot count.
- Reset clears all bike assignments.

Bike logic is deliberately semi-manual.

---

## 9. Known Limitations

- No inner exclusions
- No vertical logic
- No multi-level parking
- No structural constraints
- No code-compliance validation
- Optimization is WIP

These are design decisions, not oversights.

---

## 10. Observations

- Small geometric changes can cause large layout shifts.
- Clean input geometry dramatically improves results.
- Over-constraining reduces solution diversity.

Treat Sardine as a generator, not a drafter.

---
