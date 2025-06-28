## h2-Demo

```mermaid
erDiagram
  Bookings {
    Id text PK
    UserId text FK
    RoomId text FK
    CheckInDate timestamp with time zone 
    CheckOutDate timestamp with time zone 
    TotalPrice numeric 
    CreatedAt timestamp with time zone 
    UpdatedAt timestamp with time zone 
  }
  Bookings }o--|| Rooms : FK_Bookings_Rooms_RoomId
  Bookings }o--|| Users : FK_Bookings_Users_UserId
  Users {
    Id text PK
    Name text 
    CreatedAt timestamp with time zone 
    UpdatedAt timestamp with time zone 
  }
  Rooms {
    Id text PK
    Name text 
    Description text 
    Image text 
    Price numeric 
    Capacity integer 
    CreatedAt timestamp with time zone 
    UpdatedAt timestamp with time zone 
  }
```
