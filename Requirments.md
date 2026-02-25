# ATS System Requirements - Membership Tracking

This document outlines the requirements for the Membership Application Tracking System (ATS). 

## Estimation Baseline
* Story: User Authentication (Login)
* Baseline Points: 2
* Reason: Standard implementation of verification and JWT generation.

---

## 1. Membership Application Submission
**User Story:**
As a Potential Member, I want to submit a registration application so that the Admin can verify my identity before I gain library access.

**Estimation:** 3 Points

**Scenario: Valid application submission**
Given I am on the membership registration page
When I enter a unique email, name, and student ID
And I click the submit button
Then the database should create a new application record with status 'Pending'
And the system should return a 201 Created response.

---

## 2. Admin Application Review
**User Story:**
As an Admin, I want to approve pending applications so that the system automatically generates a library user account.

**Estimation:** 5 Points

**Scenario: Approving an applicant**
Given I am logged in as an Admin
When I access the 'Pending Applications' list
And I click 'Approve' on a specific record
Then the application status should update to 'Approved' 
And a new row should be created in the Users table with the 'Member' RoleId.

---

## 3. Status Tracking
**User Story:**
As an Applicant, I want to check the status of my application using my email address so that I know when I am authorized to borrow books.

**Estimation:** 2 Points

**Scenario: Checking status of an active application**
Given an application exists for my email address
When I enter my email into the tracker
Then the UI should display the current status and the date of submission.

---

## 4. Rejection Logic
**User Story:**
As an Admin, I want to reject incomplete applications with a specific reason so that the applicant knows why they were denied.

**Estimation:** 3 Points

**Scenario: Rejecting for invalid documentation**
Given I am reviewing an application as an Admin
When I click 'Reject' and enter 'Invalid ID format' as the reason
Then the application status should change to 'Rejected'
And the rejection reason must be saved to the database record.

---
