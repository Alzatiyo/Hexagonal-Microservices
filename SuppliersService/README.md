# Suppliers MS — Arquitectura Hexagonal

> **Arquitectura Hexagonal (Ports & Adapters)**  
> Tecnología: **.NET 8** | Base de datos: **SQL Server (Docker)** | ORM: **Entity Framework Core**

---

## 🏢 Contexto del Negocio

Es un microservicio especializado en la gestión de proveedores (`Suppliers`) y su asociación con los productos del sistema principal (`StockPro`). Permite registrar proveedores, vincularles productos externos, y consultar cuál es la mejor opción de compra cuando el inventario principal se agota.

---

## 🧠 Regla de Negocio Principal

> Al consultar los proveedores disponibles para un producto específico, el sistema evalúa a todos los proveedores activos que lo distribuyen, **ordenándolos y recomendando automáticamente al más rápido** (menor `DeliveryDays`).

Esta validación y selección ocurre **exclusivamente en el dominio** (`SupplierService.GetAvailableForProduct()`), garantizando que la recomendación se base estrictamente en la lógica de negocio y no en consultas SQL acopladas.

```
Para cada proveedor que vende el producto X:
Si Status == Inactivo → Descartar
Si Status == Activo   → Mantener

Ordenar lista resultante por DeliveryDays (Ascendente)
El primer elemento de la lista → IsRecommended = true
```

---

## 📋 Requisitos Funcionales

| ID | Descripción | Endpoint |
|----|-------------|----------|
| RF-01 | Crear un nuevo proveedor | `POST /api/suppliers` |
| RF-02 | Listar todos los proveedores | `GET /api/suppliers` |
| RF-03 | Obtener proveedor por ID | `GET /api/suppliers/{id}` |
| RF-04 | Actualizar un proveedor | `PUT /api/suppliers/{id}` |
| RF-05 | Eliminar un proveedor | `DELETE /api/suppliers/{id}` |
| RF-06 | Asociar producto a un proveedor | `POST /api/suppliers/{id}/products` |
| RF-07 | Recomendar proveedor por producto | `GET /api/suppliers/by-product/{productId}` |

---

## 🎯 Atributos de Calidad

| Atributo | Prioridad | Descripción |
|----------|-----------|-------------|
| **Mantenibilidad** | Alta | Separación estricta de capas. El dominio no depende de Entity Framework ni de ASP.NET. |
| **Portabilidad** | Alta | El proyecto entero, incluida la base de datos, está contenerizado con Docker Compose. |
| **Desacoplamiento** | Alta | Se relaciona con StockPro solo por un `ExternalProductId` (Guid), sin llaves foráneas entre bases de datos. |
| **Autonomía** | Alta | Crea y migra su propia base de datos automáticamente al encender el contenedor. |

---

## 🗺️ Diseño Hexagonal — Mapping

| CAPA | COMPONENTE | ARCHIVO EN EL PROYECTO | DESCRIPCIÓN |
|------|------------|------------------------|-------------|
| DOMINIO | Entidad | `Domain/Models/Supplier.cs` | Modelo principal con lógica de desactivación |
| DOMINIO | Entidad Asociativa | `Domain/Models/SupplierProduct.cs` | Relación Proveedor ↔ Producto (Externo) |
| DOMINIO | Enum / VO | `Domain/Enums/SupplierStatus.cs` | Estados: `Activo`, `Inactivo` |
| DOMINIO | Builder | `Domain/Builders/SupplierBuilder.cs` | Patrón Builder para construir `Supplier` |
| DOMINIO | Servicio de Dominio | `Domain/Services/SupplierService.cs` | `EvaluateSupplierStatus()`, `GetAvailableForProduct()` |
| APLICACIÓN | Puerto Entrada | `Aplication/Ports/In/ISupplierUseCasePort.cs` | Contrato de casos de uso |
| APLICACIÓN | Puerto Salida | `Aplication/Ports/Out/ISupplierRepositoryPort.cs` | Contrato del repositorio |
| APLICACIÓN | Caso de Uso | `Aplication/UseCases/SupplierUseCase.cs` | Orquestación CRUD y lógica de integración |
| INFRAESTRUCTURA | Adaptador Entrada | `Infrastructure/Adapters/Rest/SuppliersController.cs` | REST API (HTTP) |
| INFRAESTRUCTURA | Adaptador Salida | `Infrastructure/Adapters/Persistence/SupplierAdapter.cs` | Acceso a BD (EF Core) |
| INFRAESTRUCTURA | Mapper | `Infrastructure/Mappers/SupplierMapper.cs` | Conversión Dominio ↔ Entidad BD |
| INFRAESTRUCTURA | DTOs | `Infrastructure/Dtos/` | DTOs para requests HTTP |
| INFRAESTRUCTURA | Config BD | `Infrastructure/Config/AppDbContext.cs` | Contexto EF Core y Fluent API |

---

## 📂 Estructura de Carpetas

```
Suppliers_MS
├── Domain/                                  ← Núcleo (Reglas puras, sin librerías externas)
├── Aplication/                              ← Casos de uso (Orquesta Domain e Infra)
├── Infrastructure/                          ← Adaptadores externos (REST, EF Core, SQL Server)
├── Api/                                     ← Punto de entrada (Host, Kestrel, Swagger)
├── Dockerfile                               ← Build de la imagen de la API
└── docker-compose.yml                       ← Orquestador (API + SQL Server)
```

---

## 🏗️ Patrones de Diseño Aplicados

### 1. Builder (Creacional)
**Clases:** `SupplierBuilder`, `SupplierProductBuilder`
Construye objetos complejos paso a paso, garantizando que el dominio siempre inicie en un estado válido.

### 2. Repository (Estructural)
**Clases:** `ISupplierRepositoryPort`, `SupplierAdapter`
Abstrae el acceso a datos. El dominio no sabe que sus datos se guardan en SQL Server.

### 3. Port & Adapter — Hexagonal (Arquitectural)
**Clases:** `ISupplierUseCasePort`, `ISupplierRepositoryPort`
Aísla completamente el núcleo de negocio. La API HTTP entra por un puerto, y la base de datos sale por otro.

### 4. Mapper (Estructural)
**Clases:** `SupplierMapper`, `ISupplierMapper`
Traduce el dominio puro a las entidades `SupplierEntity` (con anotaciones `[Key]`, `[Required]`), evitando contaminar el negocio con atributos de Entity Framework.

---

## 🚀 Instrucciones de Ejecución (Docker)

El proyecto está diseñado para arrancar con "Cero Configuración" en cualquier máquina.

### Prerrequisitos
- Docker Desktop instalado y corriendo.

### Pasos

1. Clonar el repositorio y navegar a la carpeta raíz:
```bash
cd Suppliers_MS
```

2. Levantar toda la infraestructura:
```bash
docker-compose up -d --build
```

**¿Qué pasa internamente?**
- Descarga SQL Server 2022.
- Compila la API de C#.
- La API arranca, detecta que la base de datos está vacía y **aplica automáticamente las migraciones** creando las tablas necesarias.
- Queda escuchando peticiones en el puerto `5001`.

**Para borrar todo y empezar de cero:**
```bash
docker-compose down -v
```

---

## 📡 API — Endpoints y Pruebas Rápidas

> **Swagger UI:** `http://localhost:5001/swagger`

Puedes probar todo directamente desde la interfaz gráfica de Swagger, o usar cURL.

### 1. Crear un Proveedor (`POST /api/Suppliers`)
```bash
curl -X POST "http://localhost:5001/api/Suppliers" \
     -H "Content-Type: application/json" \
     -d '{
           "name": "Muebles Alfa",
           "contactName": "Juan Perez",
           "email": "ventas@alfa.com",
           "phone": "555-1234",
           "deliveryDays": 3
         }'
```

### 2. Asociar un Producto (`POST /api/Suppliers/{id}/products`)
*(Reemplaza el GUID de la URL por el ID generado en el paso anterior)*
```bash
curl -X POST "http://localhost:5001/api/Suppliers/11111111-2222-3333-4444-555555555555/products" \
     -H "Content-Type: application/json" \
     -d '{
           "externalProductId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
           "productName": "Silla Gamer",
           "unitPrice": 450.00,
           "isAvailable": true
         }'
```

### 3. Evaluar Proveedores por Producto (`GET /api/Suppliers/by-product/{productId}`)
Este es el endpoint que `StockPro` consultará internamente.
```bash
curl -X GET "http://localhost:5001/api/Suppliers/by-product/3fa85f64-5717-4562-b3fc-2c963f66afa6"
```

---

*Desarrollado como módulo desacoplado bajo Arquitectura Hexagonal*
