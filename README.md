# EduConnect - Smart Learning Management System

## How to Run in Visual Studio 2022
1. Open the **EduConnect.sln** file in Visual Studio 2022.
2. Ensure you have the `.NET desktop development` workload installed (specifically .NET Framework 4.8).
3. Build the solution (`Ctrl + Shift + B`).
4. To set up the initial database, run `Database/DatabaseSetup.sql` against `(localdb)\MSSQLLocalDB`. The script creates **Users** (for login/registration), **Courses**, **Students** (with optional UserId), **Enrollments**, **Payments**, and **Grades**. New users register on the login page; credentials are stored in **Users** and (for Student role) linked in **Students**.
5. Press `F5` to run the project.

**Login:** Use **Register** (new user?) on the login page to create an account; credentials are saved in the database and you are redirected to your dashboard. Demo logins (if no DB): Admin `admin` / `password` | Instructor `instructor` / `password` | Student `student` / `password`

## Implementation of 8 Core Requirements

**EX-1: Login System (Delegates & Events)**
- Handled in `Core/SessionManager.vb`.
- Uses `Delegate Sub LoginAttemptEventHandler` and `Event OnLoginAttempt`.
- `LoginForm.vb` catches the event via `Handles sessionManager.OnLoginAttempt`.
- Includes AES Salt + SHA256 simulation in `HashPassword`.

**EX-2: App Settings (Singleton Pattern)**
- Handled in `Core/AppSettings.vb`.
- Uses `Lazy(Of AppSettings)` with `LazyThreadSafetyMode.ExecutionAndPublication` to ensure thread-safe global access across all layered components.

**EX-3: Domain-Specific Calculator**
- Implemented in `Presentation/CalculatorForm.vb`.
- Acts as a dynamic "Course Fee & EMI Calculator" applying GST and discounts. Features combo boxes, checkboxes, and up/down numeric inputs.

**EX-4: Information Management System**
- Structured using the 3-Tier Layered architectural pattern.
- Domain layer objects in `BLL/Models.vb`.
- Services in `BLL/StudentService.vb`.
- Parameterized ADO.NET query executions in `DAL/StudentRepository.vb`.

**EX-5: Notes Module (Notepad)**
- Implemented in `Presentation/NotesForm.vb`.
- Embeds standard open/save/font Dialog utilities, outputting standard `.rtf` formats.

**EX-6: CRUD Operations (Database Controls)**
- Implemented in `Presentation/StudentMgmtForm.vb`.
- Uses a `DataGridView` linked to a `BindingSource` with full **Add**, **Edit**, and **Delete** via the BLL/DAL. Add/Edit panel with Name, Email, Course Id; validation and exception handling in the service and form.

**EX-7: Whiteboard Utility**
- Implemented in `Presentation/WhiteboardForm.vb`.
- Smooth `SmoothingMode.AntiAlias` applied mapping `Graphics.DrawLine()`. Embeds Mouse down/up/move hooks.

**EX-8: Encryption & Decryption**
- Located logically at `Core/SecurityHelper.vb` and executed through `Presentation/SecurityForm.vb`.
- Uses `.NET` `Aes.Create()` class using streams (`CryptoStream`) to dynamically read/write bits masking critical assignment files.

## Features
- **Student:** Enroll in courses (Enroll in Course), view and mark payments (My Payments), Study Materials, Notepad, Whiteboard.
- **Admin:** Course & Student CRUD, **Grades & GPA** (add grades per student/course, calculate GPA on 0–10 and 4.0 scale), Security Center, App Settings.

## Architecture
- Presentation Layer -> Forms and Event Hooks
- Business Logic Layer -> Domain validations and flow routing
- Data Access Layer -> SQL Connectivity & Security Repositories
