The application is a online store built with ASP.NET Core, utilizing the MVC architecture, Repository Pattern, and Service Layer. It allows users to browse products, add them to the cart, and place orders. 
Additionally, the system includes an admin panel for managing products, categories, and orders.
Technologies and Patterns

    Backend: ASP.NET Core MVC, Entity Framework Core (database)
    Frontend: JavaScript (AJAX), Razor Views, Bootstrap
    Database: SQL Server
    Architecture:
        MVC – separation of presentation layer from business logic
        Repository Pattern – abstraction for database access
        Service Layer – business logic separated from controllers

Features
1. User Module

    Registration and login (ASP.NET Identity)
    Browsing products and categories
    Adding products to the shopping cart
    Placing an order (saving to the database, order status management), payment with Stripe
    Order history

2. Admin Panel

    Managing products (CRUD operations)
    Managing categories
    Managing users and roles
    Viewing and updating order statuses
  
3. AJAX in the Frontend

    Dynamically adding products to the cart
    Asynchronous filtering and searching of products
    Real-time updates in the admin panel
