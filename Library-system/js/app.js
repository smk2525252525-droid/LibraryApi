// Ensure DOM is loaded so javascript waits till html is loaded so no exceptions are thrown
$(document).ready(function () {

    const API_URL = "https://localhost:7124"; //  port

    function getHeaders() {
        return {
            "Content-Type": "application/json",
            "Authorization": "Bearer " + localStorage.getItem("token")
        };
    }

    // login funtion
    $("#loginbtn").click(function () {
        let email = $("#email").val();
        let password = $("#password").val();

        if (email === "" || password === "") {
            alert("Please fill in all fields.");
            return;
        }

        $.ajax({
            url: `${API_URL}/api/auth/login`,
            method: "POST",
            contentType: "application/json",
            // Force the keys to match C# LoginDto.cs exactly
            data: JSON.stringify({
                Email: email,
                Password: password
            }),
            success: function (response) {
                localStorage.setItem("token", response.token);
                localStorage.setItem("loggedInUser", email);
                localStorage.setItem("userRole", response.role); // Save "Admin" or "Member"
                localStorage.setItem("userId", response.userId); // Save user ID
                alert("Login successful!");
                window.location.href = "dashboard.html";
            },
            error: function (xhr) {
                console.log("Login Error Status:", xhr.status);
                console.log("Login Error Body:", xhr.responseText);
                alert("Login failed! Check console for details.");
            }
        });
    });

    // register
    $("#registerbtn").click(function () {
        let name = $("#reg-email").val().split("@")[0];
        let email = $("#reg-email").val();
        let password = $("#reg-password").val();
        let confirmPassword = $("#confirmpassword").val();

        if (email === "" || password === "" || confirmPassword === "") {
            alert("Please fill in all fields.");
            return;
        }

        if (password !== confirmPassword) {
            alert("Passwords do not match.");
            return;
        }

        $.ajax({
            url: `${API_URL}/api/users`, // Registration uses the User POST endpoint
            method: "POST",
            contentType: "application/json",
            data: JSON.stringify({ name: name, email: email, password: password, roleId: 2 }),
            success: function () {
                alert("Registration successful! Please login.");
                window.location.href = "login.html";
            },
            error: function () {
                alert("Registration failed. Email might already exist.");
            }
        });
    });

    // --- EDIT MODE DETECTION: Pre-fill Member Form ---
    if ($("#Member-name").length && localStorage.getItem("editMemberId")) {
        let editUserId = localStorage.getItem("editMemberId");
        
        // Change UI to reflect Edit Mode
        $("h3.text-center").text("Edit Member");
        $("#addmemberBtn").text("Update Member");

        $.ajax({
            url: `${API_URL}/api/users/${editUserId}`,
            method: "GET",
            headers: getHeaders(),
            success: function (user) {
                $("#Member-name").val(user.name);
                $("#Member-email").val(user.email);
                $("#member-role").val(user.roleId == 1 ? "Admin" : "Member");
            }
        });
    }

    // add member function
    $("#addmemberBtn").click(function () {
        let name = $("#Member-name").val();
        let email = $("#Member-email").val();
        let roleId = $("#member-role").val() === "Admin" ? 1 : 2;

        if (name === "" || email === "" || roleId === "") {
            alert("Please fill in all required fields.");
            return;
        }

        let editUserId = localStorage.getItem("editMemberId");

        if (editUserId) {
            // update existing member
            $.ajax({
                url: `${API_URL}/api/users/${editUserId}`,
                method: "PUT",
                headers: getHeaders(),
                data: JSON.stringify({ name: name, email: email, roleId: roleId, password: "" }),
                success: function () {
                    localStorage.removeItem("editMemberId");
                    alert("Member updated successfully!");
                    window.location.href = "members.html";
                }
            });
        } else {
            // add new member
            $.ajax({
                url: `${API_URL}/api/users`,
                method: "POST",
                headers: getHeaders(),
                data: JSON.stringify({ name: name, email: email, password: "123456", roleId: roleId }),
                success: function () {
                    alert("User added successfully!");
                    window.location.href = "members.html";
                }
            });
        }
    });

    // load members table only if table exists
    if ($("#members-table-body").length) {
        $.ajax({
            url: `${API_URL}/api/users`,
            method: "GET",
            headers: getHeaders(),
            success: function (users) {
                $("#members-table-body").html("");
                users.forEach((user, index) => {
                    $("#members-table-body").append(`
                        <tr>
                            <td>${index + 1}</td>
                            <td>${user.name}</td>
                            <td>${user.email}</td>
                            <td>${user.role ? user.role.name : "Member"}</td>
                            <td>
                                <button class="btn btn-sm btn-primary me-2 edit-btn" data-id="${user.id}">Edit</button>
                                <button class="btn btn-sm btn-danger delete-btn" data-id="${user.id}">Delete</button>
                            </td>
                        </tr>
                    `);
                });
            }
        });
    }

    // edit member button click handler (redirects to form)
    $(document).on("click", ".edit-btn", function () {
        let id = $(this).data("id");
        localStorage.setItem("editMemberId", id);
        window.location.href = "add-member.html";
    });

    // delete member functionality
    $(document).on("click", ".delete-btn", function () {
        let id = $(this).data("id");
        if (confirm("Are you sure?")) {
            $.ajax({
                url: `${API_URL}/api/users/${id}`,
                method: "DELETE",
                headers: getHeaders(),
                success: function () { location.reload(); }
            });
        }
    });

    // logout functionality
    $("#logoutBtn").click(function () {
        localStorage.removeItem("token");
        localStorage.removeItem("loggedInUser");
        localStorage.removeItem("userRole");
        localStorage.removeItem("editMemberId");
        localStorage.removeItem("editBookId");
        window.location.href = "login.html";
    });

    //BOOKS

    // --- EDIT MODE DETECTION: Pre-fill Book Form ---
    if ($("#book-title").length && localStorage.getItem("editBookId")) {
        let editBookId = localStorage.getItem("editBookId");

        $("h3.text-center").text("Edit Book");
        $("#addBookBtn").text("Update Book");

        $.ajax({
            url: `${API_URL}/api/books/${editBookId}`,
            method: "GET",
            headers: getHeaders(),
            success: function (book) {
                $("#book-title").val(book.title);
                $("#book-author").val(book.author);
                $("#book-category").val(book.categoryId);
                $("#book-status").val(book.status);
            }
        });
    }

    // ADd/edit books
    $("#addBookBtn").click(function () {
        let title = $("#book-title").val();
        let author = $("#book-author").val();
        let categoryId = $("#book-category").val();
        let status = $("#book-status").val();

        if (title === "" || author === "" || categoryId === "") {
            alert("Please fill all fields.");
            return;
        }

        let editBookId = localStorage.getItem("editBookId");

        if (editBookId) {
            $.ajax({
                url: `${API_URL}/api/books/${editBookId}`,
                method: "PUT",
                headers: getHeaders(),
                data: JSON.stringify({ title, author, categoryId: parseInt(categoryId), status }),
                success: function () {
                    localStorage.removeItem("editBookId");
                    alert("Book updated successfully!");
                    window.location.href = "books.html";
                }
            });
        } else {
            $.ajax({
                url: `${API_URL}/api/books`,
                method: "POST",
                headers: getHeaders(),
                data: JSON.stringify({ title, author, categoryId: parseInt(categoryId), status }),
                success: function () {
                    alert("Book added successfully!");
                    window.location.href = "books.html";
                }
            });
        }
    });

    // BOOKS table
    if ($("#books-table-body").length) {
        // We fetch issuings ONLY to find the ID for the Return button logic
        $.ajax({
            url: `${API_URL}/api/issuings`,
            method: "GET",
            headers: getHeaders(),
            success: function (issuings) {
                $.ajax({
                    url: `${API_URL}/api/books`,
                    method: "GET",
                    headers: getHeaders(),
                    success: function (books) {
                        $("#books-table-body").html("");
                        const userRole = localStorage.getItem("userRole")?.toLowerCase();
                        const currentUserId = localStorage.getItem("userId");

                        books.forEach((book, index) => {
                            // Find the active issuing for this book so we can "Return" it later
                            let activeIssuing = issuings.find(i => i.bookId === book.id && i.returnedAt === null);
                            
                            let actionBtns = "";

                            if (userRole === "admin") {
                                actionBtns = `
                                    <button class="btn btn-sm btn-primary edit-book-btn" data-id="${book.id}">Edit</button>
                                    <button class="btn btn-sm btn-danger delete-book-btn" data-id="${book.id}">Delete</button>`;
                            } 
                            else if (userRole === "member") {
                                if (book.status === "Available") {
                                    actionBtns = `<button class="btn btn-sm btn-warning issue-book-btn" data-id="${book.id}">Issue</button>`;
                                } else if (book.status === "Issued" && activeIssuing && activeIssuing.userId == currentUserId) {
                                    // We store the issuingId in a data attribute for the database update
                                    actionBtns = `<button class="btn btn-sm btn-success return-book-btn" data-id="${book.id}" data-issuing-id="${activeIssuing.id}">Return</button>`;
                                }
                            }

                            // REVERTED TABLE: No "Return Date" column
                            $("#books-table-body").append(`
                                <tr>
                                    <td>${index + 1}</td>
                                    <td>${book.title}</td>
                                    <td>${book.author}</td>
                                    <td>${book.category ? book.category.name : "N/A"}</td>
                                    <td><span class="badge ${book.status === 'Available' ? 'bg-success' : 'bg-danger'}">${book.status}</span></td>
                                    <td>${actionBtns}</td>
                                </tr>
                            `);
                        });
                    }
                });
            }
        });
    }

    // edit book button click handler (redirects to form)
    $(document).on("click", ".edit-book-btn", function () {
        let id = $(this).data("id");
        localStorage.setItem("editBookId", id);
        window.location.href = "addbook.html";
    });

    $(document).on("click", ".delete-book-btn", function () {
        let id = $(this).data("id");
        if (confirm("Are you sure?")) {
            $.ajax({
                url: `${API_URL}/api/books/${id}`,
                method: "DELETE",
                headers: getHeaders(),
                success: function () { location.reload(); }
            });
        }
    });

    // Get data for dashboard
    if ($("#totalUsers").length) {
        $.ajax({
            url: `${API_URL}/api/users`,
            method: "GET",
            headers: getHeaders(),
            success: function (users) { $("#totalUsers").text(users.length); }
        });
        $.ajax({
            url: `${API_URL}/api/books`,
            method: "GET",
            headers: getHeaders(),
            success: function (books) { $("#totalBooks").text(books.length); }
        });
    }

    // --- GLOBAL UI UPDATES (Role Display & Permissions) ---
    function applyPermissions() {
        const loggedInUserEmail = localStorage.getItem("loggedInUser");
        const userRole = localStorage.getItem("userRole");

        if (loggedInUserEmail && $("#userEmail").length) {
            $("#userEmail").text(`${loggedInUserEmail} (${userRole})`);
        }

        // Hide admin-only features for Members
        if (userRole && userRole.toLowerCase() !== "admin") {
            $(".adminOnly").hide(); 
            // Hide the dashboard summary item for Total Users
            $("#totalUsers").closest(".summary-item").hide(); 
        }
    }
    applyPermissions(); // Run once on page load


    // --- DASHBOARD MODAL LOGIC ---
    const featuredBooks = [
        { title: "Death Note", desc: "A high-school student discovers a supernatural notebook" },
        { title: "Sapiens: a brief history", desc: "Explores the history of humankind." },
        { title: "the Martian", desc: "An astronaut stranded on Mars." },
        { title: "Angels & Demons", desc: "Robert Langdon stops a secret society." },
        { title: "Romeo and Juliet", desc: "Classic tragedy by Shakespeare." },
        { title: "A Tale of Two Cities", desc: "Set in London and Paris during the revolution." },
        { title: "Harry Potter", desc: "A young wizard's journey." },
        { title: "The Lord of the Rings", desc: "Epic journey to destroy the One Ring." }
    ];

    $(document).on("click", ".book-card", function () {
        let title = $(this).find(".book-title").text();
        let img = $(this).find("img").attr("src");
        let book = featuredBooks.find(b => b.title === title);

        $("#bookModalTitle").text(title);
        $("#bookModalImg").attr("src", img);
        $("#bookModalDesc").text(book ? book.desc : "No description available.");

        // Show the Bootstrap modal
        var myModal = new bootstrap.Modal(document.getElementById('bookModal'));
        myModal.show();
    });


    // --- DRAG TO SCROLL LOGIC ---
    const carousel = document.querySelector(".book-carousel");
    if (carousel) {
        let isDragging = false;
        let startX, scrollLeft;

        carousel.addEventListener("mousedown", (e) => {
            isDragging = true;
            carousel.classList.add("dragging");
            startX = e.pageX - carousel.offsetLeft;
            scrollLeft = carousel.scrollLeft;
        });

        carousel.addEventListener("mouseleave", () => {
            isDragging = false;
        });

        carousel.addEventListener("mouseup", () => {
            isDragging = false;
            carousel.classList.remove("dragging");
        });

        carousel.addEventListener("mousemove", (e) => {
            if (!isDragging) return;
            e.preventDefault();
            const x = e.pageX - carousel.offsetLeft;
            const walk = (x - startX) * 2; // Scroll speed
            carousel.scrollLeft = scrollLeft - walk;
        });
    }

    // ISSUE BOOK FUNCTIONALITY
    $(document).on("click", ".issue-book-btn", function () {
        let bookId = $(this).data("id");
        let userId = localStorage.getItem("userId"); // No longer hardcoded to 1

        if (confirm("Issue this book?")) {
            $.ajax({
                url: `${API_URL}/api/issuings`,
                method: "POST",
                headers: getHeaders(),
                data: JSON.stringify({ bookId: bookId, userId: parseInt(userId) }),
                success: function () {
                    updateBookStatus(bookId, "Issued");
                }
            });
        }
    });

    // updates the Issuings table ReturnedAt column
    $(document).on("click", ".return-book-btn", function () {
        let bookId = $(this).data("id");
        let issuingId = $(this).data("issuing-id"); // The ID of the specific loan record

        if (confirm("Return this book?")) {
            // Update the Issuing record first (sets the date in DB)
            $.ajax({
                url: `${API_URL}/api/issuings/return/${issuingId}`,
                method: "PUT",
                headers: getHeaders(),
                success: function () {
                    // Then update the book status
                    updateBookStatus(bookId, "Available");
                }
            });
        }
    });

    // Helper function to update status in Database
    function updateBookStatus(bookId, newStatus) {
        $.ajax({
            url: `${API_URL}/api/books/status/${bookId}`,
            method: "PATCH",
            headers: getHeaders(),
            contentType: "application/json",
            data: JSON.stringify(newStatus),
            success: function () {
                alert("Database updated successfully!");
                location.reload(); // Refresh table to show new status
            }
        });
    }

});