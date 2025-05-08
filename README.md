# 🖥️ TechFix Procurement System – SOA-Based Desktop Application

A robust, service-oriented desktop application tailored for **TechFix**, a local computer shop specializing in repairs, upgrades, and custom builds. This platform modernizes TechFix’s procurement process by streamlining supplier interaction, automating orders, and enabling data-driven decisions — all through Web APIs and SQL Server integration.

---

## 🚀 Key Features

- 📑 **Centralized Quotation Platform**: Compare quotes from multiple suppliers instantly.
- 📦 **Real-Time Inventory Visibility**: Live inventory levels from suppliers to prevent out-of-stock delays.
- 📬 **Automated Order Placement**: Direct order integration with supplier systems to eliminate manual entry.
- 📊 **Purchase History & Analytics**: Easily track past orders and generate procurement reports.
- 🔐 **Secure Login System**: Role-based access for TechFix staff.
- ✅ **Order Tracking & Status Updates**: Know exactly when and where your components are.
- ⚙️ **Error-Free Communication**: Reduce miscommunication with standardized API interactions.

---

## 🧰 Tech Stack

- **Frontend (Desktop App)**: C# (.NET WPF / WinForms)
- **Backend Services (SOA)**: ASP.NET Web API
- **Database**: SQL Server (SSMS)
- **Communication**: RESTful APIs (JSON)
- **Authentication**: Token-based / Session-based auth
- **Version Control**: Git & GitHub

---

## 🗂️ Solution Architecture (SOA)

[Client (Desktop App)]
|
| REST API Calls
↓
[Service Layer - Web API]
|
| SQL Queries / Procedures
↓
[SQL Server Database]

---


### 🔌 Services Implemented

- 🛒 `QuotationService`: Manage quote requests and responses
- 📦 `InventoryService`: Retrieve and sync supplier inventory
- 🧾 `OrderService`: Place, update, and track orders
- 🗂️ `ReportingService`: View analytics and past purchases
- 👤 `AuthService`: Handle login and role access

---

## 💻 How to Run Locally

1. **Clone this repo** 📁
2. **Set up SQL Server** and run the provided `TechFix_DB.sql` script
3. Open the solution in **Visual Studio**
4. Update connection strings in `appsettings.json` or config file
5. Build and run the **Web API** project
6. Build and run the **Desktop Client** (WinForms/WPF)
7. 🎉 Start quoting, ordering, and managing inventory smarter!

---

## 🖼️ Screenshots

![Screenshot (47)](https://github.com/user-attachments/assets/18ddce8c-9798-40f1-84da-40da83396442)
![Screenshot (50)](https://github.com/user-attachments/assets/6a44582e-941f-4e80-b422-bfb7271770a5)
![Screenshot (51)](https://github.com/user-attachments/assets/c7db7177-8e52-4495-a09b-2d2d149ce35d)
![Screenshot (54)](https://github.com/user-attachments/assets/f62115c6-63e0-4acb-be73-e2f683a5286e)
![Screenshot (62)](https://github.com/user-attachments/assets/d984ee24-9d63-4cd5-aba3-e0e690f9310b)
![Screenshot (63)](https://github.com/user-attachments/assets/6d76b4cf-2976-4619-8fc5-eecbaa10d9ef)
![Screenshot (64)](https://github.com/user-attachments/assets/86fc3377-8a0b-4187-aa61-4bfa4a5edade)


---

## 📁 Folder Structure

TechFix-Procurement-System/
├── ClientApp/ # Desktop app (WPF or WinForms)
├── WebAPI/ # ASP.NET Core Web API project
├── Database/ # SQL scripts & schema
├── README.md

---

## 🤝 Author

- 👨‍💻 **Mohana Dharshan**
- 🐙 GitHub: [@ITzDharshan](https://github.com/ITzDharshan)
- 💼 LinkedIn: [linkedin.com/in/mdharshan](https://www.linkedin.com/in/mdharshan)

---
