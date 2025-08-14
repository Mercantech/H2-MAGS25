---
name: Uge 3.4 - Udvidet EF Core og Hosting
about: Uge 3.4 - Udvidet EF Core og Hosting
title: Uge 3.4 - Udvidet EF Core og Hosting
labels: ''
assignees: ''

---

## Hosting på deploy.mercantec.tech og Eager vs Lazy Loading

- [ ] Host applikationen på deploy.mercantec.tech
  - [ ] Login: Findes på Notion
  - [ ] Brug jeres tildelte porte (se gruppe-oversigt)
- [ ] Implementér avanceret EFCore koncepter såsom Lazyload eller Eager Loading
- [ ] Sammenlign performance mellem begge tilgange

---

### Eksempel på Eager Loading
```csharp
var hotel = _context.Hotels
    .Include(h => h.Rooms)
    .ThenInclude(r => r.Bookings)
    .FirstOrDefault(h => h.Id == "abc");
```
