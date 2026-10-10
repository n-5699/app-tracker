# Part 4 — Environments pages

## Goal
Let users record **where each system is deployed**: add, edit, and delete environments (Dev, Test, UAT, Staging, Production) on a system's page. A system can have **several environments of the same type**.

## Prerequisites
- [Part 3](03-systems-pages.md) complete

## Steps

### 1. Pages created in `Pages/Environments/`

| Page | URL | What it does |
|---|---|---|
| `Create` | `/Environments/Create/{systemId}` | Adds an environment to that system. Type defaults to Production |
| `Edit` | `/Environments/Edit/{id}` | Changes type, label, URL, location, notes |
| `Delete` | `/Environments/Delete/{id}` | Confirms, then removes that one environment |
| `_EnvironmentFields` | (partial) | Form fields shared by Create and Edit |

Every page returns the user to the **system's Details page** afterwards.

### 2. The Type dropdown comes from the enum
```cshtml
<select asp-for="Type" asp-items="Html.GetEnumSelectList<EnvironmentType>()" class="form-select"></select>
```
Adding a new type later (e.g. `DR`) only means adding it to `Models/EnvironmentType.cs`.

### 3. The system ID comes from the URL, not the form
```csharp
public async Task<IActionResult> OnPostAsync(int systemId)
{
    var system = await db.Systems.FindAsync(systemId);
    if (system is null) return NotFound();
    ...
    AppEnvironment.AppSystemId = systemId;
    db.Environments.Add(AppEnvironment);
```
The Edit page copies only the editable fields, so an environment can't be moved to another system by tampering with the form.

### 4. System Details page updates (`Pages/Systems/Details.cshtml`)
- **+ Add Environment** button and an environment count in the heading
- Environments **grouped by type, Production first**, with color-coded badges:
  Production (green) · Staging (yellow) · UAT (cyan) · Test (blue) · Dev (grey)
- **Edit / Delete** buttons on every row
- URLs open in a **new tab** (`target="_blank" rel="noopener"`)

### 5. Environment changes update the system's "Last updated"
In `Data/AppDbContext.cs`, `SaveChangesAsync` now finds the system of any added, edited or deleted environment and bumps its `UpdatedAt`. This happens in one place, so no page can forget to do it.

### 6. Validation (from the model attributes in Part 2)
- **URL** is required and must be a full `http://`, `https://` or `ftp://` address. This also blocks unsafe links such as `javascript:...`
- **Deployment Location** is required
- Label max 100 characters, Notes max 1000

## How to verify
Run the app, open a system, then:
- [ ] With no environments, it shows "No environments recorded yet."
- [ ] **+ Add Environment**: add two **Production** environments (labels "Prod East" and "Prod West"), one **UAT**, one **Dev**
- [ ] The table shows them in the order Production, Production, UAT, Dev, with colored badges
- [ ] Clicking a URL opens it in a new tab
- [ ] Entering `not a url` as the URL shows "The URL field is not a valid fully-qualified http, https, or ftp URL."
- [ ] **Edit** one environment and change its type to Staging. It moves to the Staging group
- [ ] **Delete** one environment. The count drops by one
- [ ] The system's "Last updated" time changes after each of these
- [ ] Deleting the whole system removes all its environments

## What you learned / why
- **Nested resources:** environments always belong to a system, so their URLs carry the system ID (`/Environments/Create/5`), and every page leads back to that system.
- **Enums to dropdowns:** `GetEnumSelectList<T>()` keeps the list of choices in one place, the C# enum.
- **Cross-cutting rules belong in `DbContext`:** timestamp logic lives in `SaveChangesAsync`, so it applies everywhere automatically.
- **Validation as security:** the `[Url]` rule keeps the data consistent and also stops someone saving a link that runs script.

**Next:** [Part 5 — List, search & filter](05-list-and-search.md)
