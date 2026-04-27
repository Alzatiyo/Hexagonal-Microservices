# Stock — Arquitectura Hexagonal

> **Arquitectura Hexagonal (Ports & Adapters)**  
> Tecnología: **.NET 8** | Base de datos: **Sql Server** | ORM: **Entity Framework Core**

---

##  Contexto del Negocio

Es un sistema para gestión de inventario de bodega. Permite registrar productos con su stock actual, definir umbrales mínimos de reabastecimiento y gestionar salidas de inventario con validación de estado.

---

##  Regla de Negocio Principal

> Al registrar una **salida de inventario**, si el stock resultante queda **igual o por debajo del mínimo** (`Stockminimum`) definido por el producto, el sistema cambia el estado del producto a **`ReabastecimientoPendiente`**.

Esta validación ocurre **exclusivamente en el dominio** (`ProductService.RegisterExit`), garantizando que ninguna capa externa pueda saltársela.

```
stock_resultante = stock_actual - cantidad_salida

Si stock_resultante <= stockminimum  →  Status = ReabastecimientoPendiente
Si stock_resultante >  stockminimum  →  Status = Activo
```

---

##  Requisitos Funcionales

| ID | Descripción | Endpoint |
|----|-------------|----------|
| RF-01 | Crear un nuevo producto | `POST /api/product` |
| RF-02 | Listar todos los productos | `GET /api/product` |
| RF-03 | Obtener producto por ID | `GET /api/product/{id}` |
| RF-04 | Actualizar un producto | `PUT /api/product/{id}` |
| RF-05 | Eliminar un producto | `DELETE /api/product/{id}` |
| RF-06 | Registrar salida de inventario | `PATCH /api/product/{id}/exit/{quantity}` |
| RF-07 | Cambio automático de estado al bajar del umbral mínimo | (lógica interna del dominio) |

---

##  Atributos de Calidad

| Atributo | Prioridad | Descripción |
|----------|-----------|-------------|
| **Mantenibilidad** | Alta | Separación estricta de capas. El dominio no depende de infraestructura. |
| **Testabilidad** | Alta | Los puertos permiten inyectar mocks. El dominio se puede probar sin BD. |
| **Extensibilidad** | Alta | Agregar un adaptador (ej. MongoDB) no modifica el dominio ni la aplicación. |
| **Cohesión** | Alta | Cada clase tiene una única responsabilidad (SRP de SOLID). |
| **Disponibilidad** | Media | La API responde correctamente con 201, 200, 204, 400 y 404 según el caso. |

---

##  Diseño Hexagonal — Mapping

| CAPA | COMPONENTE | ARCHIVO EN EL PROYECTO | DESCRIPCIÓN |
|------|------------|------------------------|-------------|
|DOMINIO|Entidad|`Domain/Models/Product.cs`|Modelo principal con lógica `RegisterExit()`|
| DOMINIO | Enum / VO | `Domain/Enums/ProductStatus.cs` | Estados: `Activo`, `ReabastecimientoPendiente` |
| DOMINIO | Builder | `Domain/Builders/ProductBuilder.cs` | Patrón Builder para construir `Product` |
| DOMINIO | Servicio de Dominio | `Domain/Services/ProductService.cs` | `EvaluateStockStatus()`, `RegisterExit()` |
| DOMINIO | Excepción | `Domain/Exceptions/DomainException.cs` | Excepción propia del núcleo |
| APLICACIÓN | Puerto Entrada | `Aplication/Ports/In/IProductUseCasePort.cs` | Contrato de casos de uso |
| APLICACIÓN | Puerto Salida | `Aplication/Ports/Out/IProductRepositoryPort.cs` | Contrato del repositorio |
| APLICACIÓN | Caso de Uso | `Aplication/UseCases/ProductUseCase.cs` | Orquestación CRUD y salidas |
| INFRAESTRUCTURA | Adaptador Entrada | `Infrastructure/Adapters/Rest/ProductController.cs` | REST API (HTTP) |
| INFRAESTRUCTURA | Adaptador Salida | `Infrastructure/Adapters/Persistence/ProductAdapter.cs` | Acceso a BD (EF Core) |
| INFRAESTRUCTURA | Mapper | `Infrastructure/Mappers/ProductMapper.cs` | Conversión Dominio ↔ Entidad BD |
| INFRAESTRUCTURA | DTOs | `Infrastructure/Dtos/` | `CreateProductRequest`, `UpdateProductRequest` |
| INFRAESTRUCTURA | Config BD | `Infrastructure/Config/AppDbContext.cs` | Contexto EF Core |

---

##  Estructura de Carpetas

```
Hexagonal.sln
├── Domain/                                  ← Núcleo (sin dependencias externas)
│   ├── Builders/
│   │   └── ProductBuilder.cs                ← Patrón Builder
│   ├── Enums/
│   │   └── ProductStatus.cs                 ← Activo / ReabastecimientoPendiente
│   ├── Exceptions/
│   │   └── DomainException.cs               ← Excepción de dominio
│   ├── Models/
│   │   └── Product.cs                       ← Entidad + lógica RegisterExit()
│   └── Services/
│       └── ProductService.cs                ← Servicio de dominio
│
├── Aplication/                              ← Casos de uso (orquestación)
│   ├── Ports/
│   │   ├── In/
│   │   │   └── IProductUseCasePort.cs       ← Puerto de ENTRADA
│   │   └── Out/
│   │       └── IProductRepositoryPort.cs    ← Puerto de SALIDA
│   └── UseCases/
│       └── ProductUseCase.cs                ← Implementa IProductUseCasePort
│
├── Infrastructure/                          ← Adaptadores externos
│   ├── Adapters/
│   │   ├── Persistence/
│   │   │   ├── ProductAdapter.cs            ← Adaptador de SALIDA (EF Core)
│   │   │   ├── ProductEntity.cs             ← Entidad de BD
│   │   │   └── ProductEntityBuilder.cs      ← Builder para entidad BD
│   │   └── Rest/
│   │       └── ProductController.cs         ← Adaptador de ENTRADA (REST)
│   ├── Config/
│   │   ├── AppDbContext.cs                  ← Contexto EF Core
│   │   └── InfrastructureServiceExtensions.cs
│   ├── Dtos/
│   │   ├── CreateProductRequest.cs
│   │   └── UpdateProductRequest.cs
│   └── Mappers/
│       ├── Interface/
│       │   └── IProductMapper.cs
│       └── ProductMapper.cs                 ← Dominio ↔ Entidad BD
│
└── Api/                                     ← Punto de entrada (Host)
    ├── Program.cs                           ← DI y configuración
    └── appsettings.json
```

---

##  Patrones de Diseño Aplicados

### 1. Builder (Creacional)
**Clases:** `ProductBuilder`, `ProductEntityBuilder`

Construye objetos `Product` complejos paso a paso, evitando constructores con múltiples parámetros. Permite crear instancias expresivas y legibles. Se usa dentro del propio dominio al ejecutar `RegisterExit()`:

```csharp
public Product RegisterExit(int quantity)
{
    return new ProductBuilder()
        .WithId(this.Id)
        .WithName(this.Name)
        .WithStock(this.Stock)
        .WithStockminimum(this.Stockminimum)
        // Regla de negocio aplicada en el dominio:
        .WithStatus((this.Stock - quantity) <= this.Stockminimum
            ? ProductStatus.ReabastecimientoPendiente
            : ProductStatus.Activo)
        .Build();
}
```

### 2. Repository (Estructural)
**Clases:** `IProductRepositoryPort`, `ProductAdapter`

Abstrae el acceso a datos detrás de una interfaz del dominio. El dominio no conoce EF Core ni SQLite; solo conoce el contrato del puerto `IProductRepositoryPort`.

### 3. Port & Adapter — Hexagonal (Arquitectural)
**Clases:** `IProductUseCasePort`, `IProductRepositoryPort`

Separa el núcleo de negocio de los detalles de entrada/salida. Los puertos definen los contratos; los adaptadores los implementan. El dominio nunca depende de la infraestructura.

### 4. Mapper (Estructural)
**Clases:** `ProductMapper`, `IProductMapper`

Traduce entre el modelo de dominio (`Product`) y la entidad de persistencia (`ProductEntity`), manteniendo la independencia de capas y evitando la "contaminación" del dominio con anotaciones de BD.

### 5. Domain Service (De Dominio)
**Clase:** `ProductService`

Encapsula lógica de negocio que no pertenece a una sola entidad: validación de cantidad, evaluación de stock y cambio automático de estado.

---

## API — Endpoints y cURL

> **Base URL:** `http://localhost:5000`  
> **Swagger UI:** `http://localhost:5000/swagger`

---

### `POST /api/product` — Crear producto

```bash
curl -X POST http://localhost:5000/api/product \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Arroz Blanco 1kg",
    "descripcion": "Arroz de grano largo premium",
    "stock": 100,
    "stockminimum": 20,
    "price": 5500.00
  }'
```
**Respuesta:** `201 Created` — El estado será `Activo` si `stock > stockminimum`.

---

### `GET /api/product` — Listar todos

```bash
curl -X GET http://localhost:5000/api/product
```
**Respuesta:** `200 OK` — Array JSON con todos los productos.

---

### `GET /api/product/{id}` — Obtener por ID

```bash
curl -X GET http://localhost:5000/api/product/550e8400-e29b-41d4-a716-446655440000
```
**Respuesta:** `200 OK` o `404 Not Found`.

---

### `PUT /api/product/{id}` — Actualizar producto

```bash
curl -X PUT http://localhost:5000/api/product/550e8400-e29b-41d4-a716-446655440000 \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Arroz Blanco 1kg - Actualizado",
    "descripcion": "Arroz de grano largo seleccionado",
    "stock": 80,
    "stockminimum": 15,
    "price": 5800.00
  }'
```
**Respuesta:** `200 OK` o `404 Not Found`.

---

### `DELETE /api/product/{id}` — Eliminar producto

```bash
curl -X DELETE http://localhost:5000/api/product/550e8400-e29b-41d4-a716-446655440000
```
**Respuesta:** `204 No Content` o `404 Not Found`.

---

### `PATCH /api/product/{id}/exit/{quantity}` — Registrar salida Regla de Negocio

Este es el endpoint que implementa la **regla de negocio central**:

```bash
# Ejemplo 1: Salida normal (stock resultante > stockminimum)
# stock=100, stockminimum=20, salida=10 → stock queda en 90 → Status: Activo
curl -X PATCH http://localhost:5000/api/product/550e8400-e29b-41d4-a716-446655440000/exit/10
```

```bash
# Ejemplo 2: Activa REABASTECIMIENTO_PENDIENTE
# stock=100, stockminimum=20, salida=85 → stock queda en 15 ≤ 20 → Status: ReabastecimientoPendiente
curl -X PATCH http://localhost:5000/api/product/550e8400-e29b-41d4-a716-446655440000/exit/85
```

```bash
# Ejemplo 3: Error — stock insuficiente (lanza DomainException)
curl -X PATCH http://localhost:5000/api/product/550e8400-e29b-41d4-a716-446655440000/exit/500
# Respuesta: 400 Bad Request
# { "error": "Stock insuficiente. Stock actual: 100, solicitado: 500." }
```

---

##  Instrucciones de Ejecución

### Prerrequisitos
- [.NET SDK 8.0+](https://dotnet.microsoft.com/download)
- Git
- Sql Server

### Pasos

```bash
# 1. Clonar el repositorio
git clone https://github.com/TU_USUARIO/Stock_Hexagonal.git
cd Stock_Hexagonal

# 2. Restaurar dependencias
dotnet restore

# 3. Ejecutar la aplicación
cd Api
dotnet run
```

La API estará disponible en `http://localhost:5000` y `https://localhost:7000`.  
Swagger UI en: `http://localhost:5000/swagger`

### Prueba rápida de la regla de negocio

```bash
# Paso 1: Crear producto (stock=30, stockminimum=20)
curl -X POST http://localhost:5000/api/product \
  -H "Content-Type: application/json" \
  -d '{"name":"Producto Test","descripcion":"Prueba regla negocio","stock":30,"stockminimum":20,"price":1000}'

# Paso 2: Copiar el ID de la respuesta y registrar salida de 15 unidades
# (30 - 15 = 15 <= 20) → debe cambiar a ReabastecimientoPendiente
curl -X PATCH http://localhost:5000/api/product/{ID_COPIADO}/exit/15
```

---

##  Guía de Exposición

### ¿Cómo se aplicó la Inversión de Dependencia?

Las dependencias apuntan **siempre hacia adentro** (hacia el dominio), nunca hacia afuera:

```
ProductController  →  IProductUseCasePort  ←  ProductUseCase
ProductUseCase     →  IProductRepositoryPort  ←  ProductAdapter  →  AppDbContext
```

El **composition root** es `Program.cs` en el proyecto `Api`. Es el único lugar que conoce todas las implementaciones concretas y registra las dependencias.

### ¿Dónde reside la lógica de negocio?

En la capa de **Dominio**, exclusivamente:

| Clase | Lógica |
|-------|--------|
| `Product.RegisterExit()` | Calcula nuevo stock y determina el estado (Builder + regla) |
| `ProductService.RegisterExit()` | Valida cantidad > 0 y stock suficiente. Lanza `DomainException` |
| `ProductService.EvaluateStockStatus()` | Evalúa estado al crear o actualizar un producto |

Las capas de Aplicación e Infraestructura **no contienen lógica de negocio**. Solo orquestan y adaptan.

### ¿Cómo se visualizan los puertos y adaptadores en ejecución?

Flujo completo de `PATCH /{id}/exit/{qty}`:

```
[HTTP Request]
     ↓
ProductController (Adaptador de Entrada)
     ↓ llama a →
IProductUseCasePort (Puerto de Entrada)
     ↓ implementado por →
ProductUseCase (Caso de Uso)
     ↓ llama a →
IProductRepositoryPort.GetByIdAsync() (Puerto de Salida)
     ↓ implementado por →
ProductAdapter → AppDbContext (SQLite)
     ↓ retorna Product al caso de uso
ProductUseCase
     ↓ invoca →
ProductService.RegisterExit() → Product.RegisterExit()  ← LÓGICA DE NEGOCIO
     ↓ resultado →
IProductRepositoryPort.UpdateAsync() → ProductAdapter → AppDbContext
     ↓
[HTTP Response 200 OK]
```

> **El Dominio nunca sabe que existe HTTP, EF Core, SQl Server ni JSON. Los puertos son la membrana protectora del hexágono.**

---

##  Modelo de Dominio

```
Product
├── Id          : Guid
├── Name        : string
├── Descripcion : string
├── Stock       : int
├── Stockminimum: int
├── Price       : decimal
├── Status      : ProductStatus (Activo | ReabastecimientoPendiente)
├── CreatedAt   : DateTime
├── UpdateAt    : DateTime
└── RegisterExit(quantity): Product   ← método con regla de negocio
```

---

*Desarrollado por Santiago Alzate — Arquitectura Hexagonal (Ports & Adapters)*
