# DestinoTrack

A web application for a cargo company, built with ASP.NET Core 8 MVC in a five-layer architecture, with role-based access, three languages and a full audit trail. It covers a shipment end to end: acceptance at the counter, pricing, the printed label, movements between branches, delivery with a code at the door, failed attempts and automatic return, and a public tracking page that needs no account.

<img width="1806" height="902" alt="image" src="https://github.com/user-attachments/assets/757daf54-1146-42c0-9f72-032ffc8e9ee5" />

This project is based on a case study from the training I received at M&Y Yazılım Eğitim Akademi, under the guidance of Murat Yücedağ and Erhan Gündüz. The instructor's reference project was built in class; DestinoTrack is my own delivery, started from an empty solution and planned as epics and tasks the way a real team would track them.

## Features

### Accounts and roles

- Five roles: Admin, Manager, Personel (branch staff), Courier and Customer, created at startup
- Customer registration for individuals and companies, with tax and identity number rules per country; company accounts start as pending approval
- Login, logout and an account page; the session cookie expires after 30 minutes
- Account lockout after five failed attempts, for fifteen minutes
- Admin user management: create staff accounts, change roles and scopes (country for managers, branch for staff and couriers), activate or deactivate; the last active admin cannot be removed
- __Login goes to the right place:__ staff and couriers land on the branch panel, managers on the country panel, admins on the management panel, everyone else on the public site

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
- __Quote before saving:__ The acceptance screen can price a shipment without creating it, so the clerk can tell the customer the amount first.

### Cargo operations

- __Acceptance:__ Staff open a shipment at their own branch; the origin branch is taken from the account, not chosen on the form. Tracking number, price, estimated delivery and barcode are produced on save.
- __Tracking number:__ `{prefix}-{year}-{six digits}`, with the prefix in configuration.
- __Status flow:__ Nine statuses with a state machine; an invalid transition cannot be selected on screen and is refused in the service.
- __Movement entry:__ A two-step screen — find by tracking number, then pick from the statuses that are actually open for that shipment. The movement is written to the staff member's own branch.
- __Distribution and delivery:__ One screen with two sections — ready for distribution (assign a courier) and out for delivery (deliver, or record a failed attempt). Delivery asks for the six-digit code and the name of the person who received the parcel.
- __Delivery code:__ Produced once and valid until the parcel is delivered, so the recipient is not given a new code on a second attempt. Only the courier the shipment is assigned to can deliver it.
- __Automatic return:__ After the attempt limit (three by default) the shipment moves to return in progress on its own, with a movement record.
- __Issue list:__ Delayed, failed and returning shipments in three groups; delivered and returned parcels drop out of all of them.
- __Printable label:__ A 10×15 cm thermal label on its own page, with the barcode and QR code embedded as SVG, the destination branch code in the sorting band and the amount due at the door. The print dialog opens with the page.

### Panels and tracking

- __Branch panel:__ Daily summary with six cards, latest movements, branch cargo list, acceptance, movement entry, distribution and the issue list. The courier menu narrows to the delivery screens.
- __Country panel:__ The same summary over every branch in the manager's country, with a read-only shipment detail. Printing a label belongs to the field, so that button is not drawn here.
- __Public tracking:__ `/Track/{tracking number}` shows status, route and the movement timeline with no account and no personal data: no names, phone numbers, addresses, delivery code or staff names. The price appears only when the receiver pays at the door.
- __Rate limit:__ The tracking endpoint allows 20 requests per minute per IP, and a malformed tracking number never reaches the database.

### Platform

- __Languages:__ Turkish, English and Portuguese, with all texts, validation messages and Identity errors translated (448 resource keys per language).
- __Data scope:__ Admin sees everything, a manager their country, staff their branch, a courier their own shipments and a customer their own parcels. The scope is resolved in the service and applied in the query, so paging and filters stay correct.
- __Soft delete:__ Deleted records are hidden, not removed, and unique indexes apply only to records that are not deleted.
- __Audit log:__ Every create, update and delete is recorded with old and new values and the user who made the change.
- __Error handling:__ Global exception handling with error pages in every environment.
- __Lists:__ Ten rows per page, with filters kept in the session instead of the URL.

## Tech Stack

- __Framework:__ ASP.NET Core 8 MVC
- __Authentication:__ ASP.NET Core Identity with a `Guid` primary key
- __Database:__ SQL Server and Entity Framework Core 8 (Code First, 12 migrations, lazy loading proxies)
- __Validation:__ FluentValidation, on both the server and the browser
- __Mapping:__ Mapster
- __Localization:__ ASP.NET Core Localization with shared resource files
- __Barcodes:__ ZXing.Net, producing Code 128 and QR codes as inline SVG
- __Styling:__ Tailwind CSS v4, built through the Tailwind CLI as part of `dotnet build`
- __Scripts:__ jQuery and jQuery Validation for client-side validation

## Architecture

```
DestinoTrack.Entity       18 entities, 15 enums, BaseEntity (dates and soft delete)
DestinoTrack.DTO          request and response models
DestinoTrack.DataAccess   AppDbContext, repositories, interceptors, migrations
DestinoTrack.Business     services, validators, pricing, state machine, resources
DestinoTrack.WebUI        controllers, views, ViewComponents, three areas
```

- __Thin controllers:__ Controllers only call services, and every rule lives in the Business layer.
- __Three areas:__ `Admin` for management screens, `Personel` for the branch panel (staff and couriers) and `Manager` for the country panel. Each area has a base controller that carries its role requirement.
- __Interceptors:__ Two EF Core interceptors run on every save. The first fills the dates and turns a delete into a soft delete. The second writes the audit log, so it sees a delete as `IsDeleted: false → true`.
- __Delete protection in services:__ A soft delete never triggers a foreign key error, so "cannot delete while in use" checks are written in the services, not left to the database.
- __State machine and gate rules, separately:__ The transition map answers "which status can follow this one". Three statuses with a flow of their own — out for delivery, delivered, delivery failed — are additionally refused at the service gate, so only `DispatchAsync`, `DeliverAsync` and `FailDeliveryAsync` can write them.
- __ViewComponents:__ Five classes cover six of the twelve components planned for the finished product. Two of them serve both the branch and the country panel: the shared view sits at the root and each area keeps only its own empty-state wording.

## How this project maps to the case

The case describes three roles; this project keeps the five it already had and maps the case onto them. Every permission and screen the case asks for is covered, under different names.

| Case role | What the case gives it | DestinoTrack role | Data filter |
|---|---|---|---|
| __Admin__ | All cargo, branches, transfer centers, staff and users; overall dashboard | __Admin__ | none |
| __Manager__ — branch manager | Only the cargo arriving at and leaving its branch; movement updates, distribution list, delivery code, issue list, daily operations; branch dashboard | __Personel__ (branch staff), scoped by `BranchId` | origin or destination branch is mine |
| *(not in the case)* | — | __Manager__ (regional operations), scoped by `CountryId` | the shipment's branch → city → country is mine |
| *(not a role in the case; "the staff member who did it")* | Delivering staff and the person who received the parcel | __Courier__, scoped by `BranchId` and an `Employee` record | the shipment is assigned to me |
| __User__ — customer | Own incoming and outgoing parcels only | __Customer__, scoped by `CustomerId` | sender, receiver or customer is me |

__Deliberate differences__

- __Project names.__ The case suggests `CargoTrack.*`; the solution is named `DestinoTrack.*`. The layer names and their responsibilities are the same.
- __Transfer centers.__ Modelled as a branch with `BranchType.TransferCenter` rather than a separate entity, because they share address, capacity and staff with a branch. They still have their own screen, with the type locked.
- __Mapster.__ Used in the services where a DTO really is a field-by-field copy of an entity — branches, cities, countries, employees and tariffs. Where a DTO is a projection or a total, it is built inside the query instead, so the shaping happens in the database rather than in memory; and `UserService` fills its models by hand, because Identity's operations are not a copy at all.
- __`UserService` goes through `UserManager`.__ The other services follow controller → service → repository → database. User operations go through Identity's `UserManager`, since password hashing, role assignment and lockout live there and should not be reimplemented over the context.

__Beyond the case__

A regional manager role and panel, a courier role with its own distribution list, a transfer center screen, three languages, a soft delete with an audit log, a printable thermal label, and a rate limit on public tracking.

__Not built yet__

The admin dashboard with its charts, the customer panel, performance reports and a screen for reading the audit log. The data behind them is already recorded; what is missing is the screens.

## Testing

Verification lives in a separate repository, as Python scripts — one per task — that check the database, the pages and the rules rather than the internals of a class. A full regression run builds a throwaway database from every migration, runs all scripts in order and drops it again: __41 scripts, 1604 checks, about five minutes__. It runs before a task is closed, and every epic also ends with a manual test table that has to pass.

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
- __A transition map is not a permission.__ The movement screen offered every status the map allowed, so a shipment could be set to "out for delivery" by hand — with no courier assigned and no delivery code produced. After that neither delivery nor a failed attempt would be accepted, and the parcel could never be delivered again. "Which transition is possible" and "who may write it" are two different questions, and the second one belongs in the service, not in the screen's query.
- __Lazy loading fills a field you did not ask for.__ The branch panel's movement list showed a branch chip that should only appear in the country panel: the proxy loaded the navigation property during projection, which also turned one query into one per row. Nothing was wrong on screen until a test compared the two panels. The service now decides explicitly which scope it is answering for.
- __A rule can be written, tested and still unreachable.__ Manual testing showed that a shipment that failed delivery could not be put back on a distribution list from any screen, so the "three attempts then return" rule could never fire in real use, although its unit-level checks passed. Reading the rule in the code is not the same as walking the operation end to end.
- __Visual Studio's resource editor can corrupt files.__ The table editor added placeholder keys and changed line endings in the `.resx` files. Since then I open them with View Code (F7) and add keys with a script.
- __Testing as a habit.__ I keep a separate verification project with Python scripts, one for each task, that check the database, the pages and the rules. A full regression run covers more than 1600 checks and runs before every task is closed.

## Status

In progress, and close to the end of the case scope. Accounts, company structure, pricing, the full cargo lifecycle, public tracking, the branch panel, the country panel and the printable label are complete. What remains are the admin dashboard, the customer panel, performance reports and the audit log screen.

## Acknowledgements

Thanks to Murat Yücedağ and Erhan Gündüz at M&Y Yazılım Eğitim Akademi for the training and the case study this project grew out of.

## About me

Mehmet Asker, a self-taught full stack developer with a background in operations management. More projects: [github.com/m3hmtA-k3r](https://github.com/m3hmtA-k3r)
