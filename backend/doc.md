### Prueba
sqlcmd -S localhost -E

```shell
dotnet ef migrations add InitialCreate # Estructura las tablas
dotnet ef database update # Crea las tablas en la base de datos

dotnet ef database drop # borrar todo si cambio algo en el codigo y volver a crear todo con los dos comandos anteriores
```