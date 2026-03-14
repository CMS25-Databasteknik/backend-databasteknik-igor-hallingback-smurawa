# Backlog — Portfolio Audit Fixes

All 850 tests pass. Result pattern migration complete.
These are the remaining inconsistencies identified during a portfolio code review.

---

## ✅ DONE

- **rename-paymentmethod-folder** — folder renamed, namespace updated to `PaymentMethods`, alias removed everywhere
- **rename-typename-rolename** — `CourseEventType.TypeName` → `Name`, `InstructorRole.RoleName` → `Name`
- **fix-id-validation** — `CourseEventType.Reconstitute` and `InstructorRole.Reconstitute` now validate `ThrowIfNegativeOrZero`; `PaymentMethod.Reconstitute` fixed from `ThrowIfNegative` → `ThrowIfNegativeOrZero`; `PaymentMethodService` id guards fixed from `< 0` to `<= 0`
- **fix-endpoint-redundancy** — all 13 GET/PUT/DELETE handlers simplified to `return response.ToHttpResult();`
- **fix-dead-getall** — dead `if (!x.Any())` branch removed from `CourseService` and `InstructorService`
- **fix-instructor-setvalues** — `Role` now assigned only after all validation passes
- **fix-sealed** — `CourseService` and `InstructorService` are now `sealed`

---

## 🔴 HIGH — Bugs

### fix-catch-swallow
**Fix `ArgumentException` catch swallowing `ex.Message` in 3 lookup services**
`VenueTypeService`, `PaymentMethodService`, `ParticipantContactTypeService` all do:
```csharp
catch (ArgumentException ex)
{
    return Result<...>.BadRequest("An error occurred."); // ex.Message is discarded
}
```
Fix: change to `return Result<...>.BadRequest(ex.Message);` — same as `CourseService`/`InstructorService`.

### fix-dead-getall
**Remove dead code in `GetAllCoursesAsync` + `GetAllInstructorsAsync`**
Both methods have two identical return branches:
```csharp
if (!x.Any())
    return Result<...>.Ok(x); // same as the line below
return Result<...>.Ok(x);
```
Fix: remove the `if` block entirely.

### fix-instructor-setvalues
**Fix `Instructor.SetValues` double `Role` assignment**
`Role` is assigned via null-coalescing before validation is complete, then assigned again:
```csharp
Role = role ?? throw new ArgumentNullException(...); // assigns before validation
if (role.Id <= 0) throw ...;                         // may throw after assignment
...
Role = role; // assigns again
```
Fix: `ArgumentNullException.ThrowIfNull(role)` → validate `role.Id > 0` → assign `Name`/`Role`/`InstructorRoleId` at the end.

### fix-sealed
**Add `sealed` to `CourseService` and `InstructorService`**
Every other service is `public sealed class`. These two are missing `sealed`.

---

## 🟡 MEDIUM — Inconsistencies

### rename-paymentmethod-folder
**Rename `Domain/Modules/PaymentMethod/` → `PaymentMethods/`**
It's the only singular module folder. Forces a `using` alias everywhere:
`using PaymentMethodModel = Backend.Domain.Modules.PaymentMethod.Models.PaymentMethod;`
Touches: domain model namespace, repository contract, EF entity, EF config, repository impl, application service + interface + inputs + outputs + cache, presentation endpoints, all tests.

### rename-typename-rolename
**Rename `CourseEventType.TypeName` → `Name`, `InstructorRole.RoleName` → `Name`**
All other lookup entities use a `Name` property. These two are inconsistent.
Touches: domain models, EF entities, EF configs, `CourseRepository.ToCourseEventModel`, `InstructorService.GetInstructorByIdAsync`, tests.

### fix-id-validation
**Standardize ID guard styles across domain models**
- `CourseEventType.Reconstitute` and `InstructorRole.Reconstitute` have no ID validation at all
- `PaymentMethodService` uses `id < 0` (allows 0); `VenueTypeService`/`ParticipantContactTypeService` use `id <= 0`
- Mix of `ThrowIfNegativeOrZero` / `ThrowIfNegative` / manual `if (id < 0)` across models

Fix: use `ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id)` consistently in all lookup entity `Reconstitute` methods; use `<= 0` guard in all service ID validations.

### fix-endpoint-redundancy
**Simplify non-create endpoint handlers to use `ToHttpResult()` directly**
Every GET/PUT/DELETE handler manually guards then returns `Results.Ok(response)`:
```csharp
if (!response.Success) return response.ToHttpResult();
return Results.Ok(response);
```
But `ToHttpResult()` already handles the success case internally.
Fix: replace with `return response.ToHttpResult();` for all non-create handlers. Keep `Results.Created(...)` in POST handlers.

---

## 🔵 LOW — Polish

### fix-courseevent-validation-style
**Standardize `CourseEvent.SetValues` validation style**
Mixes old-style `if (price < 0) throw new ArgumentOutOfRangeException(...)` with
new-style `ArgumentOutOfRangeException.ThrowIfNegativeOrZero(seats)`.
Fix: use `ThrowIfNegativeOrZero` / `ThrowIfNegative` consistently throughout.

---

## Baseline
- Tests: **850 passing** (645 unit, ~205 integration/E2E)
- Build: **0 errors**
