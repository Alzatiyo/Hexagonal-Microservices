# 🔷 Hexagonal Microservices

Sistema de gestión de inventario construido con **Arquitectura Hexagonal (Ports & Adapters)** en **.NET 8**, compuesto por tres microservicios independientes que se comunican de forma asíncrona mediante **RabbitMQ**.

---

## ¿Qué es la Arquitectura Hexagonal?

La Arquitectura Hexagonal (también conocida como *Ports & Adapters*), propuesta por Alistair Cockburn, organiza el software en **tres capas concéntricas** con una regla de oro: **las dependencias siempre apuntan hacia adentro**, nunca hacia afuera.

```
┌─────────────────────────────────────────────┐
│            INFRAESTRUCTURA                  │
│  (HTTP, Base de datos, RabbitMQ, Mappers)   │
│  ┌───────────────────────────────────────┐  │
│  │           APLICACIÓN                  │  │
│  │   (Casos de uso, Puertos, DTOs)       │  │
│  │  ┌─────────────────────────────────┐  │  │
│  │  │          DOMINIO                │  │  │
│  │  │  (Modelos, Servicios, Reglas    │  │  │
│  │  │   de negocio, Excepciones)      │  │  │
│  │  └─────────────────────────────────┘  │  │
│  └───────────────────────────────────────┘  │
└─────────────────────────────────────────────┘
```

### Las tres capas del proyecto

**Dominio** — el núcleo. Contiene los modelos de negocio, servicios de dominio, enums, excepciones y builders. No tiene ninguna dependencia externa: no conoce Entity Framework, RabbitMQ ni ASP.NET. Es la capa más estable y la más fácil de testear.

**Aplicación** — la capa de orquestación. Define los **puertos** (interfaces) de entrada y salida, y contiene los casos de uso que coordinan el flujo entre el dominio y la infraestructura. Depende únicamente del Dominio.

**Infraestructura** — los **adaptadores**. Implementa los puertos definidos por la Aplicación: controladores REST, adaptadores de persistencia (EF Core), publishers/consumers de RabbitMQ y mappers. Es la única capa que conoce los detalles tecnológicos.

### Puertos y Adaptadores

| Concepto | ¿Qué es? | Ejemplo en este proyecto |
|---|---|---|
| **Puerto de Entrada** | Interfaz que define cómo el mundo exterior puede llamar al sistema | `IProductUseCasePort` |
| **Puerto de Salida** | Interfaz que define cómo el sistema llama al mundo exterior | `IProductRepositoryPort`, `IEventPublisherPort` |
| **Adaptador de Entrada** | Implementación que traduce solicitudes externas al puerto de entrada | `ProductController` (REST) |
| **Adaptador de Salida** | Implementación que conecta el puerto de salida a una tecnología | `ProductAdapter` (EF Core), `RabbitMqPublisher` |

> **El Dominio nunca sabe que existe HTTP, SQL Server, MongoDB ni RabbitMQ. Los puertos son la membrana protectora del hexágono.**

---

##  Arquitectura del Sistema

El sistema está compuesto por **tres microservicios** y **cuatro componentes de infraestructura**, todos orquestados con Docker Compose en una red interna compartida (`hexagonal-net`).

```
                        ┌─────────────────────────────────┐
                        │          RabbitMQ               │
                        │     (Message Broker)            │
                        │  :5672 AMQP | :15672 UI         │
                        └────────────┬────────────────────┘
                                     │
              ┌──────────────────────┼──────────────────────┐
              │                      │                      │
              ▼                      ▼                      ▼
   ┌──────────────────┐   ┌──────────────────┐   ┌──────────────────┐
   │  Stock Service   │   │Suppliers Service │   │  Audit Service   │
   │   :5001/swagger  │   │  :5002/swagger   │   │  :5003/swagger   │
   │                  │──►│                  │   │                  │
   │  .NET 8 + EF     │   │  .NET 8 + EF     │   │  .NET 8          │
   └────────┬─────────┘   └────────┬─────────┘   └────────┬─────────┘
            │                      │                       │
            ▼                      ▼                       ▼
   ┌──────────────────┐   ┌──────────────────┐   ┌──────────────────┐
   │  SQL Server      │   │  SQL Server      │   │    MongoDB       │
   │  (StockDb)       │   │  (SuppliersDb)   │   │  (AuditDb)       │
   │  :1433           │   │  :1434           │   │  :27017          │
   └──────────────────┘   └──────────────────┘   └──────────────────┘
```

### Flujo de eventos entre microservicios

```
[Usuario]
    │
    ▼ POST /api/product/{id}/exit/{qty}
[Stock Service]
    │
    ├──► Cola: "product-events" ──────────────────► [Audit Service]
    │         (cualquier operación CRUD)               Crea registro en MongoDB
    │
    └──► Cola: "stock-restock-requests" ──────────► [Suppliers Service]
              (solo cuando stock ≤ stock mínimo)       Busca y recomienda proveedor
```

**Colas de RabbitMQ:**
- `product-events` — publicada por StockService en cada operación CRUD. Consumida por AuditService para persistir la auditoría en MongoDB.
- `stock-restock-requests` — publicada por StockService cuando el stock de un producto cae al mínimo. Consumida por SuppliersService para identificar automáticamente el proveedor más rápido disponible.

---

##  Microservicios

### 1. Stock Service — Gestión de Inventario

Permite registrar y gestionar productos con su stock actual y umbrales mínimos de reabastecimiento.

**Base de datos:** SQL Server 2022 (`StockDb`) via Entity Framework Core  
**Puerto:** `5001`  
**Swagger:** http://localhost:5001/swagger

**Regla de negocio central:** Al registrar una salida de inventario, si el stock resultante queda igual o por debajo del mínimo definido (`StockMinimum`), el producto cambia automáticamente su estado a `ReabastecimientoPendiente` y se publica un evento de reabastecimiento.

```
stock_resultante = stock_actual - cantidad_salida

Si stock_resultante <= StockMinimum  →  Status = ReabastecimientoPendiente
Si stock_resultante >  StockMinimum  →  Status = Activo
```

| Endpoint | Descripción |
|---|---|
| `POST /api/product` | Crear un nuevo producto |
| `GET /api/product` | Listar todos los productos |
| `GET /api/product/{id}` | Obtener producto por ID |
| `PUT /api/product/{id}` | Actualizar un producto |
| `DELETE /api/product/{id}` | Eliminar un producto |
| `PATCH /api/product/{id}/exit/{quantity}` | Registrar salida de inventario (**regla de negocio**) |

**Estructura hexagonal:**

```
StockService/
├── Domain/               ← Núcleo (sin dependencias externas)
│   ├── Models/Product.cs           → Entidad + lógica RegisterExit()
│   ├── Services/ProductService.cs  → Valida cantidad y stock
│   ├── Enums/ProductStatus.cs      → Activo | ReabastecimientoPendiente
│   ├── Events/                     → ProductEvent, RestockRequestEvent
│   └── Builders/ProductBuilder.cs  → Patrón Builder
├── Aplication/           ← Orquestación
│   ├── Ports/In/IProductUseCasePort.cs     → Puerto de entrada
│   ├── Ports/Out/IProductRepositoryPort.cs → Puerto de salida (BD)
│   ├── Ports/Out/IEventPublisherPort.cs    → Puerto de salida (eventos)
│   ├── Ports/Out/IRestockPublisherPort.cs  → Puerto de salida (reabastecimiento)
│   └── UseCases/ProductUseCase.cs          → Caso de uso principal
├── Infrastructure/       ← Adaptadores
│   ├── Adapters/Rest/ProductController.cs       → Adaptador de entrada HTTP
│   ├── Adapters/Persistence/ProductAdapter.cs   → Adaptador de salida (EF Core)
│   ├── Messaging/RabbitMqPublisher.cs           → Publica en "product-events"
│   └── Messaging/RestockRabbitMqPublisher.cs    → Publica en "stock-restock-requests"
└── Api/Program.cs        ← Composition Root (inyección de dependencias)
```

---

### 2. Suppliers Service — Gestión de Proveedores

Microservicio especializado en proveedores y su asociación con productos del inventario. Permite registrar proveedores, vincularles productos externos y recomendar automáticamente el proveedor más adecuado cuando hay un evento de reabastecimiento.

**Base de datos:** SQL Server 2022 (`SuppliersDb`) via Entity Framework Core  
**Puerto:** `5002`  
**Swagger:** http://localhost:5002/swagger

**Regla de negocio central:** Al consultar proveedores disponibles para un producto, el sistema descarta los inactivos y ordena los activos por `DeliveryDays` (ascendente). El proveedor con menor tiempo de entrega queda marcado como `IsRecommended = true`.

**Integración con StockService:** Los microservicios se relacionan únicamente a través del `ExternalProductId` (un Guid), sin llaves foráneas entre bases de datos. Esto garantiza la autonomía de cada servicio.

| Endpoint | Descripción |
|---|---|
| `POST /api/suppliers` | Crear un nuevo proveedor |
| `GET /api/suppliers` | Listar todos los proveedores |
| `GET /api/suppliers/{id}` | Obtener proveedor por ID |
| `PUT /api/suppliers/{id}` | Actualizar un proveedor |
| `DELETE /api/suppliers/{id}` | Eliminar un proveedor |
| `POST /api/suppliers/{id}/products` | Asociar un producto a un proveedor |
| `GET /api/suppliers/by-product/{productId}` | Recomendar proveedor por producto |

**Estructura hexagonal:**

```
SuppliersService/
├── Domain/
│   ├── Models/Supplier.cs            → Entidad principal
│   ├── Models/SupplierProduct.cs     → Relación Proveedor ↔ Producto
│   ├── Services/SupplierService.cs   → GetAvailableForProduct()
│   └── Enums/SupplierStatus.cs       → Activo | Inactivo
├── Aplication/
│   ├── Ports/In/ISupplierUseCasePort.cs
│   ├── Ports/Out/ISupplierRepositoryPort.cs
│   └── UseCases/SupplierUseCase.cs
├── Infrastructure/
│   ├── Adapters/Rest/SuppliersController.cs
│   ├── Adapters/Persistence/SupplierAdapter.cs
│   └── Messaging/RestockRequestConsumer.cs  → Consume "stock-restock-requests"
└── Api/Program.cs
```

---

### 3. Audit Service — Auditoría de Eventos

Microservicio transversal que escucha todos los eventos del sistema y persiste un registro de auditoría en MongoDB. No expone lógica de negocio propia; actúa exclusivamente como consumidor de eventos.

**Base de datos:** MongoDB 7 (`AuditDb`)  
**Puerto:** `5003`  
**Swagger:** http://localhost:5003/swagger

**Funcionamiento:** Escucha la cola `product-events` de RabbitMQ en modo background (`BackgroundService`). Por cada mensaje recibido, crea un registro de auditoría con la acción, la entidad afectada, el usuario origen y los detalles del estado del producto en ese momento.

**Estructura hexagonal:**

```
AuditService/
├── Domain/
│   ├── Models/Audit.cs
│   └── Services/AuditDomainService.cs
├── Application/
│   ├── Ports/In/IAuditUseCasePort.cs
│   ├── Ports/Out/IAuditRepositoryPort.cs
│   └── UseCases/AuditUseCase.cs
├── Infrastructure/
│   ├── Adapters/Rest/AuditController.cs
│   ├── Adapters/Persistence/Mongo/AuditAdapter.cs  → MongoDB
│   └── Messaging/RabbitMqConsumer.cs               → Consume "product-events"
└── Api/Program.cs
```

---

##  Panel de Control Web

El repositorio incluye un **frontend de administración** (`index.html`) que permite interactuar con los tres microservicios directamente desde el navegador, sin necesidad de Swagger ni cURL.

### Cómo abrirlo

Con los servicios corriendo en Docker, abre el archivo `index.html` directamente en el navegador:

```
Doble clic en index.html   →   o arrastrarlo al navegador
```

> No requiere servidor web. Es un archivo HTML estático que se conecta directamente a las APIs corriendo en `localhost`.

### Funcionalidades del panel

El panel tiene cuatro secciones accesibles desde la barra lateral:

** Stock Service** — gestión completa de productos con pestañas para listar, crear, actualizar, registrar salidas y eliminar. La pestaña de listado muestra una tabla con accesos directos a "Editar" y "Salida" por fila. El encabezado muestra contadores en tiempo real: total de productos, cuántos están activos y cuántos en estado de reabastecimiento pendiente.

** Suppliers Service** — crear y listar proveedores, asociarles productos externos (usando el ID del producto de StockService), consultar la recomendación de proveedor por producto, y actualizar o eliminar.

** Audit Service** — visualizar todos los eventos de auditoría en tabla, buscar por ID o por entidad, y registrar eventos manualmente.

** Configuración** — permite cambiar las URLs base de cada servicio (útil si se cambian los puertos) y probar la conexión con los tres servicios de un clic. También incluye la tabla de referencia completa de endpoints.

### Indicadores de estado

El encabezado muestra tres puntos de colores (uno por servicio) que hacen ping automático cada 15 segundos:

- 🟢 Verde pulsante → servicio respondiendo
- ⚫ Gris → servicio sin respuesta o no iniciado

---

##  Instalación y Ejecución con Docker

### Prerrequisitos

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) instalado y en ejecución
- Git

### Pasos

**1. Clonar el repositorio:**

```bash
git clone https://github.com/Alzatiyo/Hexagonal-Microservices.git
cd Hexagonal-Microservices
```

**2. Levantar todos los servicios:**

```bash
docker compose up --build
```

Docker descargará las imágenes base, compilará los tres microservicios en C# y levantará todos los contenedores. Las migraciones de base de datos se aplican automáticamente al iniciar cada servicio.

> La primera vez puede tardar varios minutos dependiendo de la velocidad de la conexión y del hardware. Los servicios esperan que sus bases de datos y RabbitMQ estén sanos antes de arrancar (healthchecks configurados).

**3. Verificar que todo esté corriendo:**

Una vez que todos los contenedores estén healthy, los servicios estarán disponibles en:

| Servicio | URL |
|---|---|
| Stock Service — Swagger | http://localhost:5001/swagger |
| Suppliers Service — Swagger | http://localhost:5002/swagger |
| Audit Service — Swagger | http://localhost:5003/swagger |
| RabbitMQ Management UI | http://localhost:15672 (usuario: `guest` / contraseña: `guest`) |

### Detener los servicios

```bash
docker compose down
```

Para detener y también **eliminar todos los volúmenes** (bases de datos), útil para empezar desde cero:

```bash
docker compose down -v
```

---

##  Prueba rápida del flujo completo

Esta secuencia demuestra la comunicación entre los tres microservicios.

### Paso 1 — Crear un proveedor (Suppliers Service)

```bash
curl -X POST http://localhost:5002/api/suppliers \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Distribuidora Rápida S.A.",
    "contactName": "Ana Gómez",
    "email": "ventas@rapida.com",
    "phone": "555-9876",
    "deliveryDays": 2
  }'
```

Copia el `id` del proveedor devuelto en la respuesta.

### Paso 2 — Crear un producto (Stock Service)

```bash
curl -X POST http://localhost:5001/api/product \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Arroz Blanco 1kg",
    "descripcion": "Arroz de grano largo",
    "stock": 50,
    "stockminimum": 20,
    "price": 5500.00
  }'
```

Copia el `id` del producto devuelto.

### Paso 3 — Asociar el proveedor al producto

```bash
curl -X POST http://localhost:5002/api/suppliers/{SUPPLIER_ID}/products \
  -H "Content-Type: application/json" \
  -d '{
    "externalProductId": "{PRODUCT_ID}",
    "productName": "Arroz Blanco 1kg",
    "unitPrice": 4800.00,
    "isAvailable": true
  }'
```

### Paso 4 — Registrar una salida que activa el reabastecimiento

Esta operación baja el stock a 16 (≤ 20 mínimo), lo que desencadena:
1. El estado del producto cambia a `ReabastecimientoPendiente`
2. Stock Service publica en `product-events` → Audit Service registra la acción
3. Stock Service publica en `stock-restock-requests` → Suppliers Service identifica al proveedor recomendado

```bash
curl -X PATCH http://localhost:5001/api/product/{PRODUCT_ID}/exit/34
```

### Paso 5 — Verificar la auditoría (Audit Service)

```bash
curl -X GET http://localhost:5003/api/audit
```

### Paso 6 — Verificar proveedores recomendados (Suppliers Service)

```bash
curl -X GET http://localhost:5002/api/suppliers/by-product/{PRODUCT_ID}
```

---

##  Estructura del repositorio

```
Hexagonal-Microservices/
├── StockService/          ← Gestión de inventario (.NET 8 + SQL Server + EF Core)
│   ├── Domain/
│   ├── Aplication/
│   ├── Infrastructure/
│   ├── Api/
│   └── Dockerfile
├── SuppliersService/      ← Gestión de proveedores (.NET 8 + SQL Server + EF Core)
│   ├── Domain/
│   ├── Aplication/
│   ├── Infrastructure/
│   ├── Api/
│   └── Dockerfile
├── AuditService/          ← Auditoría de eventos (.NET 8 + MongoDB)
│   ├── Domain/
│   ├── Application/
│   ├── Infrastructure/
│   ├── Api/
│   └── Dockerfile
└── docker-compose.yml     ← Orquestación completa del sistema
```

---

##  Tecnologías utilizadas

| Tecnología | Uso |
|---|---|
| .NET 8 / C# | Framework base de los tres microservicios |
| ASP.NET Core | API REST y servidor HTTP (Kestrel) |
| Entity Framework Core | ORM para StockService y SuppliersService |
| SQL Server 2022 | Base de datos relacional (dos instancias aisladas) |
| MongoDB 7 | Base de datos documental para AuditService |
| RabbitMQ 3 | Message broker para comunicación asíncrona entre servicios |
| Docker / Docker Compose | Contenerización y orquestación del entorno completo |
| Swagger / OpenAPI | Documentación interactiva de cada API |

---

##  Patrones de diseño aplicados

**Builder (Creacional)** — `ProductBuilder`, `SupplierBuilder`. Construye objetos de dominio complejos paso a paso, asegurando que siempre inicien en un estado válido y evitando constructores con múltiples parámetros.

**Repository (Estructural)** — `IProductRepositoryPort` / `ProductAdapter`. Abstrae el acceso a datos detrás de una interfaz del dominio. El dominio no conoce Entity Framework ni SQL Server.

**Port & Adapter / Hexagonal (Arquitectural)** — Separa el núcleo de negocio de los detalles tecnológicos. Los puertos definen contratos; los adaptadores los implementan con tecnologías concretas.

**Mapper (Estructural)** — `ProductMapper`, `SupplierMapper`. Traduce entre el modelo de dominio puro y las entidades de persistencia, evitando que el dominio sea contaminado con anotaciones de Entity Framework.

**Domain Service (De Dominio)** — `ProductService`, `SupplierService`. Encapsula lógica de negocio que no pertenece a una sola entidad: validación de cantidades, evaluación de estado, selección del mejor proveedor.

**Background Service** — `RabbitMqConsumer` (AuditService), `RestockRequestConsumer` (SuppliersService). Servicios en segundo plano que procesan mensajes de las colas de forma continua e independiente del ciclo de vida de los requests HTTP.

---
