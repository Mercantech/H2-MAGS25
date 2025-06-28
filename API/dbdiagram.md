## h2-Demo

```mermaid
erDiagram
  Bookings {
    Id text PK
    CheckInDate timestamp with time zone 
    CheckOutDate timestamp with time zone 
    TotalPrice numeric 
    CreatedAt timestamp with time zone 
    UpdatedAt timestamp with time zone 
  }
  BookingRooms {
    Id text PK
    BookingId text FK
    RoomId text FK
    CreatedAt timestamp with time zone 
    UpdatedAt timestamp with time zone 
  }
  BookingRooms }o--|| Bookings : FK_BookingRooms_Bookings_BookingId
  BookingRooms }o--|| Rooms : FK_BookingRooms_Rooms_RoomId
  BookingUsers {
    Id text PK
    BookingId text FK
    UserId text FK
    CreatedAt timestamp with time zone 
    UpdatedAt timestamp with time zone 
  }
  BookingUsers }o--|| Bookings : FK_BookingUsers_Bookings_BookingId
  BookingUsers }o--|| Users : FK_BookingUsers_Users_UserId
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
