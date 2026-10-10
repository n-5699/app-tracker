# Part 3 — Systems pages

## Goal
Build the pages to **list, create, view, edit, and delete** systems. This is the first part where data can be entered in the browser.

## Prerequisites
- [Part 2](02-data-model-and-database.md) complete (models + `AppDbContext` registered)

## How Razor Pages work (quick primer)
Every page is **two files**:

| File | Contains |
|---|---|
| `Something.cshtml` | HTML plus Razor syntax (`@Model.Name`, `@foreach`, tag helpers like `asp-for`) |
| `Something.cshtml.cs` | The **PageModel**: C# that loads data (`OnGetAsync`) and handles form posts (`OnPostAsync`) |

- The `AppDbContext` is passed into each PageModel automatically (**dependency injection**). It was registered in `Program.cs` in Part 2.
- `@page "{id:int}"` at the top of a page means the URL carries an id: `/Systems/Details/5`.
- `[BindProperty]` copies submitted form fields into a C# object.
- Forms include a hidden **anti-forgery token** automatically. ASP.NET Core rejects posts without it.

## Steps

### 1. Pages created in `Pages/Systems/`

| Page | URL | What it does |
|---|---|---|
| `Index` | `/Systems` | Table of all systems (name, people, environment count, last updated). Shows a "No systems yet" message when empty |
| `Create` | `/Systems/Create` | Form for a new system, then redirects to its Details page |
| `Details` | `/Systems/Details/{id}` | Everything about one system, plus its environments (URLs open in a new tab) |
| `Edit` | `/Systems/Edit/{id}` | Same form, pre-filled |
| `Delete` | `/Systems/Delete/{id}` | Confirmation that says how many environments will also be deleted |
| `_SystemFields` | (partial) | The form fields shared by Create and Edit, so the two forms never drift apart |

### 2. Key code patterns

**Loading data** (`Index.cshtml.cs`):
```csharp
Systems = await db.Systems
    .Include(s => s.Environments)   // also load each system's environments
    .OrderBy(s => s.Name)
    .AsNoTracking()                 // read-only query
    .ToListAsync();
```

**Saving a form** (`Create.cshtml.cs`):
```csharp
if (await db.Systems.AnyAsync(s => s.Name == AppSystem.Name))
    ModelState.AddModelError("AppSystem.Name", "A system with this name already exists.");

if (!ModelState.IsValid)
    return Page();                  // re-show the form with error messages

db.Systems.Add(AppSystem);
await db.SaveChangesAsync();
return RedirectToPage("Details", new { id = AppSystem.Id });
```

**Editing safely** (`Edit.cshtml.cs`): load the saved record and copy over **only** the editable fields. That way `Id` and `CreatedAt` can't be changed by a tampered form.

**Deleting**: `db.Systems.Remove(system)` removes the system. The **cascade delete** rule from Part 2 removes its environments too.

### 3. Navigation
`Pages/Shared/_Layout.cshtml` has a **Systems** link in the top nav bar, and the app name is now "App Registry".

## How to verify
Run the app (`dotnet run` in `src/AppRegistry.Web`) and open `/Systems`:
- [ ] The empty list shows **"No systems yet"**
- [ ] **+ New System**: fill in the form and save. You land on the Details page with your data
- [ ] Creating a second system with the **same name** shows "A system with this name already exists."
- [ ] Leaving **Business Owner** blank shows "The Business Owner field is required."
- [ ] **Edit** a field and save. The Details page shows the change and a new "Last updated" time
- [ ] **Delete** asks for confirmation, then the system is gone from the list
- [ ] `/Systems/Details/999` returns **404 Not Found**

## What you learned / why
- **PageModel + .cshtml** keeps the C# logic separate from the HTML.
- **Validation runs twice:** in the browser (instant feedback, via `_ValidationScriptsPartial`) and on the server (`ModelState.IsValid`). The server check is the one that protects the database.
- **Post-Redirect-Get:** after saving, the page redirects instead of returning HTML, so refreshing the browser doesn't submit the form twice.
- **A partial view** (`_SystemFields`) gives Create and Edit one shared set of fields.

**Next:** [Part 4 — Environments pages](04-environments-pages.md)
