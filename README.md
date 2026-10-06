# DestinoTrack

A web application for a cargo company, built with ASP.NET Core 8 MVC in a five-layer architecture, with role-based access, three languages and a full audit trail.

<img width="1806" height="902" alt="image" src="https://github.com/user-attachments/assets/757daf54-1146-42c0-9f72-032ffc8e9ee5" />

This project is based on a case study from the training I received at M&Y Yazılım Eğitim Akademi, under the guidance of Murat Yücedağ and Erhan Gündüz. The instructor's reference project was built in class; DestinoTrack is my own delivery, started from an empty solution and planned as epics and tasks the way a real team would track them.

## Features

### Accounts and roles

- Five roles: Admin, Manager, Personel (branch staff), Courier and Customer, created at startup
- Customer registration for individuals and companies, with tax and identity number rules per country; company accounts start as pending approval
- Login, logout and an account page; the session cookie expires after 30 minutes
- Account lockout after five failed attempts, for fifteen minutes
- Admin user management: create staff accounts, change roles and scopes (country for managers, branch for staff and couriers), activate or deactivate; the last active admin cannot be removed

### Company structure

- __Locations:__ Countries and cities. A country with cities, customers or managers cannot be deleted, and neither can a city with branches.
- __Branches and transfer centers:__ Managed on separate screens. A branch with cargo, staff, payments or a manager cannot be deleted.
- __Employees:__ Couriers carry vehicle, plate, region and rating information.
- __Branch summary:__ A summary card on top of the branch screens, built as a ViewComponent.

### Pricing

- __Tariffs:__ Set per origin country and route scope (same city, same country, international), in the origin country's currency.
- __Cargo types:__ Eight cargo types, each with a percentage multiplier and a delivery-day adjustment.
- __Price formula:__ `(base price + max(weight, volumetric weight) × unit price) × type multiplier`, where volumetric weight is width × length × height / 3000.
- __Estimated delivery:__ Calculated from the same tariff row.

### Platform

- __Languages:__ Turkish, English and Portuguese, with all texts, validation messages and Identity errors translated (323 resource keys per language).
- __Soft delete:__ Deleted records are hidden, not removed, and unique indexes apply only to records that are not deleted.
- __Audit log:__ Every create, update and delete is recorded with old and new values and the user who made the change.
- __Error handling:__ Global exception handling with error pages in every environment.
- __Lists:__ Ten rows per page, with filters kept in the session instead of the URL.

## Tech Stack

- __Framework:__ ASP.NET Core 8 MVC
- __Authentication:__ ASP.NET Core Identity with a `Guid` primary key
- __Database:__ SQL Server and Entity Framework Core 8 (Code First, 11 migrations, lazy loading proxies)
- __Validation:__ FluentValidation, on both the server and the browser
- __Mapping:__ Mapster
- __Localization:__ ASP.NET Core Localization with shared resource files
- __Styling:__ Tailwind CSS v4, built through the Tailwind CLI as part of `dotnet build`
- __Scripts:__ jQuery and jQuery Validation for client-side validation

## Architecture

```
DestinoTrack.Entity       entities, enums, BaseEntity (dates and soft delete)
DestinoTrack.DTO          request and response models
DestinoTrack.DataAccess   AppDbContext, repositories, interceptors, migrations
DestinoTrack.Business     services, validators, pricing, localization resources
DestinoTrack.WebUI        controllers, views, ViewComponents, Admin area
```

- __Thin controllers:__ Controllers only call services, and every rule lives in the Business layer.
- __Interceptors:__ Two EF Core interceptors run on every save. The first fills the dates and turns a delete into a soft delete. The second writes the audit log, so it sees a delete as `IsDeleted: false → true`.
- __Delete protection in services:__ A soft delete never triggers a foreign key error, so "cannot delete while in use" checks are written in the services, not left to the database.
- __Admin area:__ All admin controllers inherit from one base controller that requires the Admin role.

## Getting Started

### Prerequisites

- .NET 8 SDK
- SQL Server (Express or LocalDB)
- Node.js, required by the Tailwind CLI during the build

### Setup

```bash
git clone https://github.com/m3hmtA-k3r/DestinoTrack.git
cd DestinoTrack/DestinoTrack.WebUI
```

The connection string and the first admin account are read from User Secrets, so they never enter the repository:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=.\SQLEXPRESS;Database=DestinoTrackDb;Trusted_Connection=True;TrustServerCertificate=True"
dotnet user-secrets set "AdminSeed:Email" "admin@example.com"
dotnet user-secrets set "AdminSeed:Password" "<a strong password>"
```

Create the database and run:

```bash
dotnet ef database update --project ../DestinoTrack.DataAccess --startup-project .
dotnet run
```

`dotnet build` runs `npm install` and the Tailwind build automatically. Roles and the first admin are created on the first start.

## What I learned

- __A validation rule can work on the server and fail in the browser.__ My password rule used several `Matches("[A-Z]")`-style checks for "contains an uppercase letter". On the server this worked. In the browser, the jQuery adapter requires the pattern to match the whole value and keeps only one regex per input, so every valid password was rejected. HTTP tests did not catch it because they do not run JavaScript. I moved "contains" rules to `Must(...)` (server only) and kept browser patterns as full `^...$` matches.
- __Some "required" messages cannot be translated by translating messages.__ MVC treats value types such as `int`, `decimal` and `enum` as required, but adds no `RequiredAttribute` to the model metadata, so the browser message stayed in English in all three languages. I wrote an `IValidationMetadataProvider` that adds the attribute with a resource key, and excluded `Guid` and `bool` so FluentValidation's clearer messages still show.
- __Soft delete removes the database's safety net.__ With soft delete, deleting a country never reaches the foreign key, so nothing stopped a country with cities from being "deleted". The protection had to move into the services, with its own translated message for each case.
- __Visual Studio's resource editor can corrupt files.__ The table editor added placeholder keys and changed line endings in the `.resx` files. Since then I open them with View Code (F7) and add keys with a script.
- __Testing as a habit.__ I keep a separate verification project with Python scripts, one for each task, that check the database, the pages and the rules. A full regression run covers more than 700 checks and runs before every task is closed.

## Status

In progress. Accounts, company structure and pricing are complete. Cargo creation and tracking, and the staff, manager and customer panels, are the next epics.

## Acknowledgements

Thanks to Murat Yücedağ and Erhan Gündüz at M&Y Yazılım Eğitim Akademi for the training and the case study this project grew out of.

## About me

Mehmet Asker, a self-taught full stack developer with a background in operations management. More projects: [github.com/m3hmtA-k3r](https://github.com/m3hmtA-k3r)
