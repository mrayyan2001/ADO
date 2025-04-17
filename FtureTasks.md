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

# **Contact Us Form System – Refactoring & Feature Upgrades**

---

## **Task 1: Make Repository Methods Async**

**Problem Statement:**  
Improve performance and scalability using async ADO.NET methods.

**Solution Outline:**

1. Replace:
   - `ExecuteReader()` → `ExecuteReaderAsync()`
   - `ExecuteNonQuery()` → `ExecuteNonQueryAsync()`
2. Update interface methods to be async:
   ```csharp
   Task<IEnumerable<ContactMessage>> GetAllAsync();
   ```

---

## **Task 2: Add Update (PUT) Support**

**Problem Statement:**
Allow updating existing contact messages via API.

**Solution Outline:**

1. Add `UpdateContactMessageDTO`.
2. Add `PUT /api/contact/{id}` in controller.
3. Add `UpdateAsync(int id, UpdateContactMessageDTO dto)` to:
   - Repository
   - Service

---

## **Task 3: Centralize Error Handling**

**Problem Statement:**
Avoid repeating try-catch blocks and standardize error responses.

**Solution Outline:**

1. Create custom `ExceptionMiddleware`.
2. Register middleware in `Program.cs`:
   ```csharp
   app.UseMiddleware<ExceptionMiddleware>();
   ```
3. Return standardized responses on error.

---

## **Task 4: Improve Logging with Categories**

**Problem Statement:**
Gain better visibility into repository/service activity.

**Solution Outline:**

1. Inject `ILogger<ContactRepository>` and `ILogger<ContactService>`.
2. Add logs:
   - On method entry/exit
   - On exceptions
   - For important operations

---

## **Task 5: Wrap Responses in a Standard Format**

**Problem Statement:**
Make API responses consistent and more informative.

**Solution Outline:**

1. Create `ApiResponse<T>` model:
   ```csharp
   public class ApiResponse<T> {
     public bool Success { get; set; }
     public T Data { get; set; }
     public string Message { get; set; }
   }
   ```
2. Wrap controller return values:
   ```csharp
   return Ok(new ApiResponse<ContactMessageDTO> { ... });
   ```

---

## **Task 6: Add Unit Tests for Services**

**Problem Statement:**
Ensure service logic is reliable and testable.

**Solution Outline:**

1. Use xUnit or NUnit.
2. Mock repository with Moq.
3. Test:
   - Create
   - Update
   - GetById
   - Validation and error handling

---

## **Task 7: Protect Against Spam (Optional)**

**Problem Statement:**
Prevent abuse of the contact form.

**Solution Outline:**

1. Add simple **rate limiting middleware** or
2. Integrate **Google reCAPTCHA** on the frontend and validate token in controller.

---

## ✅ Final Outcome

- Async and scalable
- Updatable entries
- Centralized error handling
- Clean logs and structured responses
- Ready for production-grade reliability
