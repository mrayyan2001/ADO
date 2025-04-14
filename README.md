---
theme: gaia
# class: invert
# paginate: true
author: "Mohammad Mrayyan"
marp: true
backgroundColor: #1e1e1e
color: #ffffff
style: |
  pre {
    background-color: transparent;
    border-left: 4px solid #007acc;
  }
  section {
    font-size: 150%
  }
---

# **Contact Us Form System (ADO.NET)**

---

## **Task 1: Create the Project and Configure ADO.NET**

**Problem Statement:**  
Set up a basic ASP.NET Core Web API project and configure it to use ADO.NET for database interactions.

**Solution Outline:**

1. Create the project using `dotnet new webapi`.
2. Configure the SQL Server connection string in `appsettings.json`.
3. Register configuration in `Program.cs`:
   ```csharp
   builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
   ```

---

## **Task 2: Define the `ContactMessage` Model**

**Problem Statement:**
Create a class that represents a contact message with fields such as `Name`, `Email`, `Subject`, and `Message`.

**Solution Outline:**

1. Create a `Models` folder and add `ContactMessage.cs` with:
   - `Id` (int)
   - `Name` (string)
   - `Email` (string)
   - `Subject` (string)
   - `Message` (string)
   - `CreatedAt` (DateTime)

---

## **Task 3: Create the SQL Table**

**Problem Statement:**
Create a `ContactMessages` table in SQL Server to store contact form submissions.

**Solution Outline:**

1. Create the table using SQL:
   ```sql
   CREATE TABLE ContactMessages (
     Id INT PRIMARY KEY IDENTITY,
     Name NVARCHAR(100) NOT NULL,
     Email NVARCHAR(100) NOT NULL,
     Subject NVARCHAR(200),
     Message NVARCHAR(MAX) NOT NULL,
     CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
   );
   ```

---

## **Task 4: Implement the `ContactController` with ADO.NET**

**Problem Statement:**
Use ADO.NET to perform CRUD operations for the `ContactMessage` entity.

**Solution Outline:**

1. Create a `Controllers` folder and add `ContactController.cs`.
2. Inject `IConfiguration` to access the connection string.
3. Implement `POST`, `GET`, `GET by ID`, and `DELETE` endpoints using ADO.NET:
   - Use `SqlConnection`, `SqlCommand`, and `SqlDataReader`.

---

## **Task 5: Add DTOs for Contact Message**

**Problem Statement:**
Use DTOs to separate database models from API request/response formats.

**Solution Outline:**

1. Create a `DTOs/ContactMessage` folder.
2. Add:
   - `CreateContactMessageDTO`
   - `ContactMessageDTO`
3. Map manually inside controller methods.

---

## **Task 6: Add Input Validation**

**Problem Statement:**
Ensure that input data for the contact form is validated before saving.

**Solution Outline:**

1. Use validation attributes:
   - `[Required]`, `[EmailAddress]`, `[StringLength]`.
2. Validate with `ModelState.IsValid`.

---

## **Task 7: Secure Against SQL Injection**

**Problem Statement:**
Ensure ADO.NET queries are secure against SQL injection attacks.

**Solution Outline:**

1. Always use parameterized queries:
   ```csharp
   cmd.Parameters.AddWithValue("@Email", dto.Email);
   ```

---

## **Task 8: Test the Contact API**

**Problem Statement:**
Test each API endpoint using Swagger or Postman.

**Solution Outline:**

1. Verify:
   - Creating a message.
   - Retrieving all messages.
   - Retrieving by ID.
   - Deleting a message.
2. Test both valid and invalid inputs.

---

## **Task 9: Add Logging and Error Handling**

**Problem Statement:**
Log errors and handle exceptions gracefully in the API.

**Solution Outline:**

1. Use try-catch blocks in each action method.
2. Log errors using `ILogger<ContactController>`.

---

## **Task 10: Prepare for Deployment**

**Problem Statement:**
Prepare the application for production deployment.

**Solution Outline:**

1. Enable CORS and HTTPS redirection.
2. Set up proper environment settings.
3. Publish using:
   ```bash
   dotnet publish -c Release
   ```

---

# ✅ Done! Contact Us System with ADO.NET
