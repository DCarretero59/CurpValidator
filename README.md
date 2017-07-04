# CurpValidator
>Library for the creation and/or verification of the first 16 digits of the Mexican CURP (Clave Única de Registro de Población/Unique Population Registry Code)
## Information

### CURP Validation Rules

All of the rules applied on this library for the validation of CURP generation are regulated by RENAPO and further changes need to be notified to modify this library.

[RENAPO CURP Instructive](https://renapo.gob.mx/swb/swb/RENAPO/InstructivoCURP)

### Features

- Create first 16 digits of the Mexican CURP from a given full name, date of birth, sex and federal entity
- Validate if a given CURP matches a given fullname
- Validate a given CURP from a given full name, date of birth, sex and federal entity


### Public Attributes

- CURP Class: Static class that allows for the creation and validation of CURP.
- Genres enum: Enum containing the posible Genres for a given person.
- FederalEntities Class: Static class that allows for the seleciton of the Federal Entity for a given person.


### Usage

To create a CURP: 

```c#
string curp = CurpClass.CreateCURP("Concepción", "Salgado", "briseño", DateTime.ParseExact("26/06/1956", "dd/MM/yyyy", null), Genres.Female, FederalEntities.DistritoFederal);
```

Validate if full name corresponds to a given CURP (Returns boolean): 

```c#
bool curpValidated = CurpClass.FullNameMatchesCurp("Concepción", "Salgado", "briseño", "SABC560626MDFLRN");
```

Validate if given person information corresponds to a given CURP (Returns boolean): 

```c#
bool curpValidated = CurpClass.FullNameMatchesCurp("Concepción", "Salgado", "briseño", DateTime.ParseExact("26/06/1956", "dd/MM/yyyy", null), Genres.Female, FederalEntities.DistritoFederal, "SABC560626MDFLRN");
```
