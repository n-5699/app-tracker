# Part 7 — Members

## Goal
Keep **one list of people** (Members) and pick each system's **Business Owner**, **System Admin** and **IT Developer** from it, instead of typing names. Each member's page shows their contact details and every system they're involved in.

## Prerequisites
- [Part 6](06-ui-and-validation.md) complete

## Steps

### 1. New model: `Member` (`Models/Member.cs`, `Models/MemberRole.cs`)

| Field | Rules |
|---|---|
| `Role` | `BusinessOwner`, `SystemAdmin`, or `ItDeveloper` (enum, stored as text) |
| `FullName` | Required, max 200 |
| `WorkEmail` | Required, valid email, **unique**, saved in lower case |
| `WorkPhone` | Required, valid phone number |
| `IsActive` | Defaults to true. Inactive members can't be picked for new systems |

`[Display(Name = "Business Owner")]` on each enum value gives readable labels in dropdowns. `MemberRole.DisplayName()` reads them in code.

### 2. Systems link to Members instead of storing names (`Models/AppSystem.cs`)
```csharp
[Required(ErrorMessage = "Choose a Business Owner.")]
public int? BusinessOwnerId { get; set; }   // foreign key
public Member? BusinessOwner { get; set; }  // navigation property

public int? SystemAdminId { get; set; }     // optional
public int? ItDeveloperId { get; set; }     // optional
```
`AppDbContext` sets up three relationships with **`DeleteBehavior.Restrict`**, so the database refuses to delete a member who is still assigned to a system.

### 3. Members pages (`Pages/Members/`)

| Page | URL | What it does |
|---|---|---|
| `Index` | `/Members` | Active members, with role filter tabs (`?role=SystemAdmin`), a "show inactive" link, and how many systems each person is on |
| `Create` | `/Members/Create` | Role, full name, work email, work phone. `?role=ItDeveloper` pre-selects the role |
| `Details` | `/Members/Details/{id}` | Contact info + **every system they're involved in** |
| `Edit` | `/Members/Edit/{id}` | Change details, or switch **Active** off |
| `Delete` | `/Members/Delete/{id}` | Only allowed if they're not on any system. Otherwise it explains why and suggests deactivating |

### 4. Role dropdowns on the System form (`Pages/Systems/_SystemFields.cshtml`)
Each dropdown lists **only active members with that role**:
```csharp
ViewData["BusinessOwnerIdOptions"] = await db.MemberOptionsAsync(MemberRole.BusinessOwner, AppSystem.BusinessOwnerId);
```
- When editing, the currently assigned person is kept in the list even if they've since been deactivated (marked "– inactive"), so saving doesn't silently drop them.
- An **Add a member** link under each dropdown opens the Add Member page, with that role pre-selected, in a new tab.

### 5. Server-side rules (`Data/MemberQueries.cs`)
The dropdowns limit choices in the browser, but the server re-checks everything, because forms can be tampered with:

| Rule | Message |
|---|---|
| Selected person must exist and have the matching role | "Choose a Business Owner from the list." |
| Newly assigned person must be active | "Raj Patel is inactive and can't be assigned to systems." |
| Work email must be unique (case-insensitive) | "A member with this work email already exists." |
| Role can't change while they fill that role on systems | "Raj Patel is the System Admin on 2 system(s). Assign someone else to those systems first." |
| Can't delete while assigned | Delete page explains and offers **deactivate** instead |

### 6. Search and display updates
- The Systems list and Details pages show names as links to the member's page. Details also shows email (clickable) and phone.
- Systems search now also matches the assigned people's **names and emails**.

### 7. Database schema
No Azure database existed yet, so the single migration was regenerated (`Migrations/..._InitialCreate.cs`) and [`docs/sql/create-schema.sql`](sql/create-schema.sql) re-exported. The local demo data was carried over: each typed name became a member with a placeholder `name@example.com` email and `(000) 000-0000` phone to edit.

> Once the app is live on Azure, schema changes must be **new** migrations (`dotnet ef migrations add <Name>`), never edits to an applied one.

## How to verify
- [ ] **Members** appears in the nav bar next to Systems
- [ ] **+ Add Member** with each role. A bad email or phone shows an error. A duplicate email is rejected
- [ ] Role filter tabs show only that role
- [ ] **New System**: each dropdown lists only members with that role. Leaving Business Owner empty shows "Choose a Business Owner."
- [ ] A member's page lists the systems they're on, and the Systems count on the Members list matches
- [ ] Deleting an assigned member is blocked. Deactivating them hides them from new-system dropdowns but keeps them on existing systems
- [ ] Searching Systems for a person's email finds their systems

## What you learned / why
- **Normalize shared data:** storing a person once and linking to them (a foreign key) means a phone-number change happens in one place, and "which systems is Jane on?" is a simple query.
- **`Restrict` vs `Cascade`:** environments belong to a system (cascade delete). People are shared across systems (restrict, so nothing gets deleted by accident).
- **Soft delete (deactivate):** keeps history intact while removing someone from future choices.
- **Never trust the dropdown:** the server re-validates IDs and roles, because the browser's form can be edited.

**Next:** [Part 8 — Deploy to Azure](08-deploy-to-azure.md)
