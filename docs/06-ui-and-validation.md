# Part 6 — UI polish & validation

## Goal
Make the app pleasant and hard to get wrong, now that **many coworkers** will be entering data into the same database.

## Prerequisites
- [Part 5](05-list-and-search.md) complete

## What changed

### 1. Dark mode with a light/dark toggle
- `<html data-bs-theme="dark">` uses **Bootstrap 5.3's built-in color modes**. Dark is the default.
- The **☀ Light / 🌙 Dark** button in the nav bar switches modes. The choice is saved in the browser (`localStorage`) and applied before the page draws, so there's no white flash.
- Light-only colors were removed (`bg-white`, `text-dark`, `table-light`, hard-coded `#e5e5e5` borders), so everything follows the theme.

### 2. Template leftovers removed
- **Privacy** page and links deleted
- Footer now reads "App Registry · Agency application inventory"
- **Home** (`/`) redirects to the Systems list (Part 5)

### 3. Confirmation messages after every change
Each page sets a one-time message before redirecting:
```csharp
TempData["Success"] = $"System \"{AppSystem.Name}\" created.";
return RedirectToPage("Details", new { id = AppSystem.Id });
```
`_Layout.cshtml` shows it as a dismissible green alert. **TempData** survives exactly one redirect, so refreshing the page doesn't show it again.

### 4. Stronger validation (protecting shared data)

| Rule | Where | Why |
|---|---|---|
| Duplicate names are blocked **ignoring case** ("payroll portal" = "Payroll Portal") | `AppSystemQueries.NameExistsAsync` in `Data/AppDbContext.cs` | SQLite compares case-sensitively, Azure SQL doesn't, so the check makes it explicit |
| Two people saving the same name at the same moment get a clear message instead of a crash | `try/catch (DbUpdateException)` in Systems Create/Edit | The database's unique index is the final safety net |
| Extra spaces are trimmed from **every** text field. Blank optional fields are saved as `NULL` | `TrimText()` in `AppDbContext.SaveChangesAsync` | "Jane Smith " and "Jane Smith" are the same person, so search and reports stay clean |
| Required fields, max lengths, valid `http(s)` URLs | Model attributes (Parts 2 & 4) | Checked in the browser *and* on the server |

### 5. Friendly error pages
- `Pages/StatusCode` handles **404 Not Found** ("The system or environment may have been deleted…") and **400** ("The form may have expired…"). It's wired up in `Program.cs`:
  ```csharp
  app.UseStatusCodePagesWithReExecute("/status/{0}");
  ```
- `Pages/Error` (unexpected crashes in production) no longer shows developer text, just a friendly message, a link back, and a reference ID for troubleshooting.

### 6. Times shown in each viewer's time zone
Times are stored in **UTC**. On Azure App Service the server clock is also UTC, so converting on the server would show the wrong hour for staff. Instead:
```cshtml
Last updated @Html.LocalTime(s.UpdatedAt, includeTime: true)
```
renders `<time datetime="2026-10-10T20:35:38Z" data-local="datetime">`, and a few lines in `wwwroot/js/site.js` rewrite it in the viewer's local time (`Helpers/LocalTimeHtmlHelper.cs`).

## How to verify
- [ ] The app opens in **dark mode**. The toggle switches to light, and the choice survives a refresh
- [ ] Creating, editing or deleting anything shows a **green confirmation**, and refreshing makes it go away
- [ ] Creating "payroll portal" when "Payroll Portal" exists shows "A system with this name already exists."
- [ ] Typing `  Jane Smith  ` saves as `Jane Smith`
- [ ] `/Systems/Details/999` and `/no/such/page` show the friendly **404 Page not found** page
- [ ] "Last updated" matches your local clock, not UTC
- [ ] No "Privacy" link anywhere

## What you learned / why
- **Bootstrap color modes** handle theming with one attribute. Avoid hard-coded colors so both themes work.
- **TempData + Post-Redirect-Get** is the standard pattern for "Saved!" messages.
- **Validate in layers:** browser (fast feedback) → server (`ModelState`) → database (unique index). Each layer catches what the one before can miss.
- **Store UTC, display local.** Converting times in the browser keeps them correct whatever time zone the server or the users are in.

**Next:** [Part 7 — Deploy to Azure](07-deploy-to-azure.md)
