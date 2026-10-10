# Part 5 — List, search & filter

## Goal
Make it quick to find a system once the list gets long. Also make the Systems list the app's home page.

## Prerequisites
- [Part 4](04-environments-pages.md) complete

## Steps

### 1. Home goes straight to the Systems list
`Pages/Index.cshtml.cs`:
```csharp
public IActionResult OnGet() => RedirectToPage("/Systems/Index");
```
The template's "Home" nav link was removed. The **App Registry** logo still links to `/`, which now lands on Systems.

### 2. Search box + environment-type filter (`Pages/Systems/Index`)
The form uses `method="get"`, so the search is in the URL:
```
/Systems?q=payroll
/Systems?type=UAT
/Systems?q=sam&type=Production
```
That means results can be **bookmarked or emailed** to a coworker.

The page model receives them with `SupportsGet`:
```csharp
[BindProperty(SupportsGet = true)] public string? Q { get; set; }
[BindProperty(SupportsGet = true)] public EnvironmentType? Type { get; set; }
```

### 3. What the search looks at
One box searches **all** of these (case-insensitive, partial matches):
- System **name** and **description**
- **Business Owner**, **System Admin**, **IT Developer**
- Every environment's **URL** and **Deployment Location**. Searching `SRV-APP-04` shows every system on that server

```csharp
var pattern = $"%{text}%";
query = query.Where(s =>
    EF.Functions.Like(s.Name, pattern, @"\") ||
    ...
    s.Environments.Any(e => EF.Functions.Like(e.DeploymentLocation, pattern, @"\")));
```
`EF.Functions.Like` becomes SQL `LIKE`, which ignores upper/lower case in both SQLite and Azure SQL. `%` and `_` typed by the user are escaped, so they're searched for literally instead of acting as wildcards.

### 4. "Has environment" filter
```csharp
if (Type is not null)
    query = query.Where(s => s.Environments.Any(e => e.Type == Type));
```
Shows only systems with at least one environment of that type, e.g. "every system that has a UAT".

### 5. Environment badges in the list
Each row shows one colored badge per environment type, with a count when there is more than one (e.g. **Production ×2**). Badge colors now come from one shared helper, `Models/EnvironmentTypeExtensions.cs` → `BadgeClass()`, used by both the list and the Details page.

### 6. Helpful states
- "Showing 2 of 14 system(s)" when filtered, with a **Clear** button
- "No systems match your search", with a link back to all systems
- The search box is hidden until at least one system exists

## How to verify
Add 3+ systems with different owners and environments, then:
- [ ] `http://localhost:5253/` redirects to `/Systems`
- [ ] Searching part of a system name finds it, regardless of upper/lower case
- [ ] Searching a person's name finds every system they're on (any of the three roles)
- [ ] Searching a server name or region from **Deployment Location** finds those systems
- [ ] **Has environment: UAT** shows only systems with a UAT environment
- [ ] Search + filter together narrow the results further
- [ ] The URL updates and can be bookmarked. **Clear** resets it
- [ ] A search with no results shows "No systems match your search."

## What you learned / why
- **GET vs POST forms:** searching changes nothing, so it uses GET. The criteria live in the URL and the page can be refreshed or shared. Forms that save data use POST.
- **Building queries step by step:** `IQueryable` lets you add `.Where(...)` conditions only when needed. EF Core turns the whole thing into **one SQL query** at `ToListAsync()`.
- **Searching related data:** `s.Environments.Any(...)` becomes a SQL `EXISTS` subquery, so systems can be found by their environments.
- **One source of truth for presentation:** the badge-color helper keeps the list and Details pages consistent.

**Next:** [Part 6 — UI polish & validation](06-ui-and-validation.md)
