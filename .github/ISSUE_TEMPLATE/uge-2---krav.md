---
name: Uge 2 - krav
about: Opretter hovedopgaver med underopgaver for uge 2
title: Opretter hovedopgaver med underopgaver for uge 2
labels: ''
assignees: ''

---

## 📦 Database & EFCore

        - [ ] Opret `User`, `Room` og `Booking` modeller i C#
        - [ ] Konfigurer relationer i `DbContext`
        - [ ] Lav og kør første EF Core migration
        - [ ] Bekræft at databasen oprettes korrekt i PostgreSQL

        ## 🔐 Brugerhåndtering (Registrering & Login)

        - [ ] Opret `UserController`
        - [ ] Implementer registrering (POST /register)
          - [ ] Hash adgangskode med BCrypt
        - [ ] Implementer login (POST /login)
          - [ ] Valider bruger og adgangskode
          - [ ] Udsted JWT ved succesfuldt login

        ## 🔒 JWT og beskyttet endpoint

        - [ ] Konfigurer JWT-token udstedelse og validering
        - [ ] Opret endpoint (GET /me)
          - [ ] Returner oplysninger om brugeren ud fra JWT

        ## 🧪 Test og dokumentation

        - [ ] Test registrering og login i Postman
        - [ ] Test JWT-beskyttet adgang til `/me`
        - [ ] Skriv kort API-beskrivelse i README
