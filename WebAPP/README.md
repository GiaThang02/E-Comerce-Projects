# WebAPP - ASP.NET Core E-commerce Application

A modern e-commerce web application built with ASP.NET Core 6.0, featuring user authentication, product management, shopping cart functionality, and payment integration.

## Features

### User Features
- **User Registration & Authentication**: Secure user account creation and login system
- **Product Browsing**: Browse products by categories and brands
- **Shopping Cart**: Add/remove items, update quantities
- **Checkout Process**: Multiple payment options including COD and online payments
- **Order Management**: View order history and track orders
- **Profile Management**: Edit user profile and view account details

### Admin Features
- **Product Management**: Add, edit, and delete products with image upload
- **Category Management**: Organize products into categories
- **Brand Management**: Manage product brands
- **Order Management**: Process and track customer orders
- **User Management**: Manage user accounts and roles
- **Coupon System**: Create and manage discount coupons
- **Slider Management**: Manage homepage banners and promotions

### Technical Features
- **Responsive Design**: Mobile-friendly interface using Bootstrap
- **Payment Integration**: Support for multiple payment gateways (Momo, VNPay)
- **Image Upload**: Product image management with file validation
- **Session Management**: Shopping cart persistence across sessions
- **Database**: Entity Framework Core with SQL Server
- **Security**: Authentication and authorization with role-based access

## Technology Stack

- **Backend**: ASP.NET Core 6.0
- **Frontend**: HTML5, CSS3, JavaScript, Bootstrap
- **Database**: SQL Server with Entity Framework Core
- **Payment**: Momo, VNPay integration
- **Authentication**: ASP.NET Core Identity
- **File Storage**: Local file system for images

## Prerequisites

- .NET 6.0 SDK or later
- SQL Server (LocalDB, Express, or Full)
- Visual Studio 2022 or VS Code

## Installation & Setup

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd WebAPP
   ```

2. **Database Setup**
   - Update the connection string in `appsettings.json`
   - Run Entity Framework migrations:
   ```bash
   dotnet ef database update
   ```

3. **Install Dependencies**
   ```bash
   dotnet restore
   ```

4. **Run the Application**
   ```bash
   dotnet run
   ```

5. **Access the Application**
   - Main site: `https://localhost:7001`
   - Admin panel: `https://localhost:7001/Admin`

## Project Structure

```
WebAPP/
├── Areas/
│   └── Admin/           # Admin area with controllers and views
├── Controllers/         # Main application controllers
├── Models/             # Data models and view models
├── Views/              # Razor views
├── wwwroot/            # Static files (CSS, JS, images)
├── Migrations/         # Entity Framework migrations
└── Program.cs          # Application entry point
```

## Configuration

### Database Connection
Update the connection string in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=WebAPP;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

### Payment Configuration
Configure payment gateways in `appsettings.json`:
```json
{
  "Momo": {
    "PartnerCode": "your-partner-code",
    "AccessKey": "your-access-key",
    "SecretKey": "your-secret-key"
  },
  "VNPay": {
    "TmnCode": "your-tmn-code",
    "HashSecret": "your-hash-secret"
  }
}
```

## Admin Access

To access the admin panel:
1. Register a new user account
2. Update the user role to "Admin" in the database
3. Login and navigate to `/Admin`

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Support

For support and questions, please contact the development team or create an issue in the repository. 