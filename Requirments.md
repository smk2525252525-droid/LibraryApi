# Application Tracking System (ATS) - User Stories & Requirements

## Project: Library Membership Tracking
**Baseline Story:** User Login (Authentication)
**Baseline Estimate:** 2 Story Points (S)

---

## Feature: Membership Application Management

### Story 1: Submit Membership Application
**As a** Potential Member,  
**I want to** fill out a digital membership application,  
**So that** the librarian can review my request for access.

*   **Estimation:** 3 Story Points (M)
*   **Scenario:** Successful Submission of Application
    *   **Given** I am on the "Apply for Membership" page
    *   **When** I enter a unique email, full name, and student ID
    *   **And** I click "Submit"
    *   **Then** the system should create a record with "Pending" status
    *   **And** the API should return a `201 Created` status code.

---

### Story 2: Admin Review and Approval
**As an** Admin,  
**I want to** approve pending applications,  
**So that** applicants are automatically granted "Member" access.

*   **Estimation:** 5 Story Points (L)
*   **Scenario:** Converting a Pending Application to a User
    *   **Given** I am logged in with the "Admin" role
    *   **When** I select a "Pending" application from the dashboard
    *   **And** I click the "Approve" button
    *   **Then** the application status should update to "Approved"
    *   **And** a new entry should be created in the `Users` table with `RoleId = 2`.

---

### Story 3: Track Application Status
**As an** Applicant,  
**I want to** check the current status of my request using my email,  
**So that** I know when I am authorized to visit the library.

*   **Estimation:** 2 Story Points (S)
*   **Scenario:** Checking status of an "In-Review" application
    *   **Given** I have a submission associated with my email
    *   **When** I enter my email into the "Status Checker" field
    *   **Then** the system should display the current status (e.g., "Pending", "Approved", or "Rejected")
    *   **And** display the date the application was submitted.

---

### Story 4: Reject Application with Reason
**As an** Admin,  
**I want to** reject invalid applications and provide a reason,  
**So that** applicants know why they were not approved.

*   **Estimation:** 3 Story Points (M)
*   **Scenario:** Rejecting for missing documentation
    *   **Given** I am reviewing a pending application as an Admin
    *   **When** I click the "Reject" button
    *   **And** I enter the reason "Missing Student ID Photo"
    *   **Then** the application status should change to "Rejected"
    *   **And** the reason should be stored in the database for the applicant to view.

---
