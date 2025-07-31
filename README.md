# 🚗 Car Inventory Management System

A web-based application that allows you to manage car inventory records by uploading CSV files directly into a relational database. Designed for dealerships and inventory managers to handle large volumes of car data efficiently.

---

 ##  Features

-   Upload CSV files to import car data in bulk
-   Validate CSV content before inserting into DB
-   Store cars in a structured relational database
-   Search, filter, and view inventory records
-   Edit and update car information
-   Delete entries with confirmation
-   Optional: Generate reports data details

---

##  Tech Stack

| Layer        | Technology           |
|--------------|----------------------|
| Frontend     | ReactJS / HTML / CSS |
| Backend      | Java Spring Boot     |
| Database     | PostgreSQL           |
| CSV Parser   | OpenCSV (Java)       |
| API Format   | RESTful JSON         |

---

## CSV Format (Required Columns)

```csv
VIN,Make,Model,Year,Mileage,Price,Status
1HGCM82633A004352,Honda,Civic,2020,30000,18000,Available
2FMDK3GC9BBB12345,Ford,Edge,2018,45000,15500,Sold
