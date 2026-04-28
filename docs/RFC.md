# 1. Título y Autor

Título de la Propuesta: Implementación del Microservicio de Proveedores (Suppliers_MS) mediante Arquitectura Hexagonal para la Gestión Desacoplada de Reabastecimiento.

Autores:  
Santiago Alzate        Diego Alejandro Rodriguez        Sergio Alvarez Hernandez  
Mateo Cespedes        Yehicol Andrés Hincapié.

Fecha: 28/04/2026


# 2. Resumen

Esta propuesta detalla la evolución del ecosistema StockPro hacia una arquitectura distribuida mediante la implementación de dos microservicios especializados, diseñados bajo el patrón de Arquitectura Hexagonal y desplegados en un entorno orquestado con Docker.

* Microservicio de Proveedores (Suppliers_MS): Centraliza la lógica de gestión de suministros, permitiendo el registro de proveedores y su vinculación con productos del inventario. Su funcionalidad principal radica en un motor de evaluación que, en tiempo real, recomienda al proveedor óptimo según tiempos de entrega, resolviendo automáticamente los estados de "REABASTECIMIENTO_PENDIENTE" generados por el núcleo de dominio de StockPro.

* Microservicio de Auditoría (Audit-Log): Implementa un sistema de observabilidad y trazabilidad inmutable para el ecosistema. A diferencia de los registros tradicionales, este microservicio opera de forma asíncrona, consumiendo eventos redirigidos por RabbitMQ cada vez que se ejecuta una acción crítica (creación, actualización, eliminación o salidas de stock) en el sistema principal. Utiliza MongoDB como motor de persistencia, lo que permite un almacenamiento de logs flexible, escalable y de alta velocidad, garantizando que el historial de operaciones sea independiente y consultable sin afectar el rendimiento de las transacciones de negocio en SQL Server.

Toda la infraestructura de comunicación entre estos componentes y el sistema principal está mediada por RabbitMQ, asegurando un desacoplamiento total y una alta disponibilidad del sistema ante picos de carga.


# 3. Motivación

* Justificación de Negocio (Suppliers_MS): El sistema principal de inventario detecta cuándo un producto alcanza su límite de stock mínimo, pero carecía de la capacidad para tomar una acción inteligente de resolución. Con Suppliers_MS, el sistema no sólo alerta sobre el desabastecimiento, sino que provee inmediatamente la mejor alternativa de reabastecimiento priorizando la rapidez de entrega, lo cual minimiza quiebres de stock y pérdidas de ventas.

* Justificación Técnica (Suppliers_MS): Acoplar el dominio de compras/proveedores dentro del dominio de inventario en StockPro habría violado el principio de responsabilidad única y alta cohesión. Al separar Suppliers_MS, garantizamos la autonomía de los datos y permitimos que la lógica de recomendación (que en el futuro podría incluir machine learning o cotizaciones dinámicas) escale independientemente sin afectar el rendimiento de las transacciones críticas de salida de inventario.

* Justificación Técnica (Audit-Log): El registro de actividades en la misma base de datos relacional del inventario genera contención de recursos y riesgos de integridad. Al delegar la auditoría a un microservicio con MongoDB, se logra un almacenamiento no relacional optimizado para grandes volúmenes de datos históricos que no requieren la rigidez de un esquema SQL.

* Justificación de Negocio (Audit-Log): Proporcionar una "fuente única de verdad" inmutable es vital para procesos de control interno y resolución de discrepancias en bodega, permitiendo saber exactamente qué usuario hizo qué acción y en qué momento.


# 4. Diseño Técnico

La solución se implementó utilizando Arquitectura Hexagonal para garantizar un alto grado de testabilidad y desacoplamiento tecnológico.

Suppliers_MS  
Integración entre Sistemas:  
Referencia Externa: No existen llaves foráneas duras entre bases de datos. Suppliers_MS almacena un ExternalProductId que hace referencia al UUID del producto en StockPro.  
Comunicación: Asincrónica vía RabbitQM, Cuando ocurre una alerta de stock, StockService pública el evento RestockRequestEvent en la cola, Posteriormente suppliersService consume este mensaje en segundo plano y ejecuta de manera autonoma el proceso de seleccion  

Desglose por Capas (Suppliers_MS):

* Capa de Dominio: Aislada y libre de frameworks. Contiene las entidades (Supplier, SupplierProduct), constructores bajo el patrón Builder (SupplierBuilder), y el SupplierService que encapsula la regla central de negocio: rankear a los proveedores priorizando el DeliveryDays más bajo.

* Capa de Aplicación: Contiene los puertos (ISupplierRepositoryPort, ISupplierUseCasePort) y el caso de uso SupplierUseCase, que orquesta el flujo de negocio entre la infraestructura y el dominio.

* Capa de Infraestructura: Implementa los adaptadores técnicos. Un SuppliersController para exponer la API REST, un SupplierAdapter que usa Entity Framework Core 8 para la persistencia, y mapeadores para aislar a la capa de dominio de la base de datos de SQL Server.


Audit_MS  
Integración entre Sistemas:  
Referencia Externa: No existen llaves foráneas duras entre bases de datos. AuditService almacena un identificador (ej. EntityId o ProductId) que hace referencia al UUID del producto modificado en StockPro, permitiendo la trazabilidad sin acoplamiento. Comunicación: Asincrónica vía RabbitMQ. Cuando ocurre cualquier alteración en el inventario (creación, actualización, eliminación o registro de salida), StockService publica un evento (ej. ProductEvent) en la cola. Posteriormente, AuditService consume este mensaje en segundo plano y registra el historial de la acción de manera autónoma sin penalizar el tiempo de respuesta del servicio principal.

Desglose por Capas (AuditService):

* Capa de Dominio: Aislada y libre de frameworks. Contiene la entidad principal (Audit), la cual modela la estructura inmutable del registro de auditoría (acción realizada, fecha, identificador del producto y detalles del cambio).

* Capa de - Aplicación: Contiene los puertos de entrada y salida, y el caso de uso AuditUseCase, encargado de recibir la orden de registrar la auditoría y orquestar el guardado a través de los puertos, manteniendo la lógica central protegida.

* Capa de Infraestructura: Implementa los adaptadores técnicos. Un RabbitMqConsumer (BackgroundService) para la escucha continua de eventos, y un adaptador de persistencia que utiliza el driver nativo de MongoDB, aprovechando las ventajas de una base de datos NoSQL documental orientada a la rápida escritura de logs masivos, aislando por completo al dominio de la tecnología de base de datos.


Nota: Los 3 diagramas están en un mismo archivo de draw.io, cada uno en una pagina independiente

Capa 1:  C1.png

Capa 2:  C2.png

Capa 3:  C3.png

DIAGRAMAS C4:  
https://drive.google.com/file/d/1WhzoUiJCEQrBwmTyf-9WtEwaXaE_VN7x/view?usp=sharing


# 5. Alternativas Consideradas

Base de Datos Compartida (Database Integration):  
Evaluación: Tener los dos microservicios, pero leyendo de la misma base de datos.  
Por qué se descartó: Considerado un anti-patrón de microservicios. Crea un acoplamiento temporal y estructural severo. Un cambio en una tabla de StockPro rompería el microservicio de Proveedores.

Comunicación Asíncrona (Event-Driven / RabbitMQ):  
Evaluación: Que StockPro emita un evento "ProductoAgotado" y Suppliers_MS reaccione en segundo plano.  
Por qué se descartó temporalmente: Requería mayor madurez de infraestructura y complejidad técnica (eventual consistency). Además, desde la perspectiva de UX (User Experience), el frontend requería obtener la recomendación de compra en tiempo real justo en la misma pantalla en la que se registra la venta que agota el producto. La comunicación HTTP síncrona resolvía mejor este requisito puntual.

Evaluación: Utilizar SQL Server o PostgreSQL para el almacenamiento de los registros de auditoría en lugar de MongoDB.  
Por qué se descartó: Los logs de auditoría son datos de "escritura intensiva" y estructura variable según la acción (Guardar, Eliminar, Salida). El uso de una base de datos relacional requeriría migraciones constantes de esquema y penalizará la velocidad de inserción. MongoDB fue seleccionado por su capacidad de almacenar documentos JSON flexibles y su alta escalabilidad horizontal.


# 6. Impacto

Escalabilidad y Rendimiento: La búsqueda y el cálculo para encontrar el proveedor óptimo son lecturas complejas. Al tener su propia API y BD, Suppliers_MS puede replicarse y absorber picos de procesamiento de datos sin penalizar la velocidad a la que StockPro registra las ventas (operaciones de escritura intensivas).

Despliegue y Operación: El impacto operacional es mínimo y controlado. El servicio ha sido completamente contenerizado con Docker. Además, incluye automatización de migraciones (context.Database.Migrate()) y resiliencia en la conexión a la base de datos (EnableRetryOnFailure), asegurando que los contenedores puedan ser destruidos y levantados sin fricción manual.


# 7. Enlaces Relacionados

Repositorio GitHub StockPro: https://github.com/Alzatiyo/Stock_Hexagonal