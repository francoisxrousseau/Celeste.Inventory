\# Catalog \& Inventory Microservice - PRD



\## 1. Overview



The Catalog \& Inventory Microservice manages product definitions and inventory levels for a single-store system.



\### Responsibilities

\- Product catalog management (manufacturers, products, variants)

\- Inventory tracking based on stock movements

\- Integration point for external systems (e.g., order service)



\### Design Principles

\- Simple

\- Extensible

\- Event-driven



\---



\## 2. Objectives



\- Provide a flexible catalog structure supporting generic items

\- Maintain accurate inventory through movement tracking

\- Ensure traceability and auditability of all inventory changes

\- Support integration with external systems via events

\- Remain scoped to a single store (no multi-location complexity)



\---



\## 3. Core Concepts



\### 3.1 Manufacturer

Represents the entity associated with product creation.



\- A manufacturer can have multiple products

\- Used for organizational and reference purposes



\### 3.2 Product

Represents a conceptual item.



Example: `T-Shirt`



\- A product can have multiple variants

\- Can be soft deleted while retaining history



\### 3.3 Variant

Represents the smallest unit that can be stocked and sold.



Example: `T-Shirt / Red / Medium`



\- Inventory is tracked at the variant level

\- Each variant belongs to a single product



\### 3.4 Inventory Movement

Represents a change in stock for a variant.



\- Movements are immutable

\- Inventory is derived from the sum of all movements



\#### Fields

\- Type (reason)

\- Quantity (positive or negative)

\- Timestamp

\- Origin (internal or external)



\### 3.5 Delivery

Represents a grouping of stock intake operations.



\- A delivery contains multiple inventory movements

\- Used for traceability of received stock

\- Does not include logistics or shipment tracking



\---



\## 4. Inventory Rules



\### Calculation

Inventory = sum of all inventory movements for a given variant



\### Constraints

\- Movements cannot be modified

\- Movements cannot be deleted



\### Corrections

\- Must be handled via adjustment movements



\### Scope

\- Inventory is always tracked at the variant level



\---



\## 5. Inventory Movement Types



\- DELIVERY → incoming stock

\- SALE → outgoing stock

\- RETURN → incoming stock

\- LOSS → outgoing stock (damage, shrinkage)

\- ADJUSTMENT → manual correction



\### Notes

\- Quantity determines increase or decrease

\- Type defines business meaning



\---



\## 6. Movement Origin



\- INTERNAL

&#x20; - Created by staff (deliveries, adjustments)

\- EXTERNAL

&#x20; - Received from external systems (e.g., order service)



\---



\## 7. Capabilities



\### Catalog Management

\- Create, update, deactivate manufacturers

\- Create, update, deactivate products

\- Create and manage product variants



\### Inventory Management

\- Record deliveries (multi-item intake)

\- Record inventory adjustments

\- Process external events (sales, returns)

\- View current inventory per variant

\- View inventory movement history



\---



\## 8. System Boundaries



\### In Scope

\- Catalog management

\- Inventory tracking via movements

\- Delivery grouping

\- External event consumption



\### Out of Scope

\- Order management

\- Payment processing

\- Payment system integrations

\- Shipment tracking

\- Inventory reservation

\- Multi-store / multi-warehouse support



\---



\## 9. External Interactions



\- The system does not manage orders or payments



\### Order Service Responsibilities

\- Handles order lifecycle

\- Emits events (sales, returns)



\### Integration

\- This service consumes events to create inventory movements



\---



\## 10. Data Initialization (Seeding)



\### Purpose

\- Provide usable data at startup

\- Enable testing and development



\### Seed Data

\- Manufacturers

\- Products

\- Variants



\### Delivery

\- Implemented as a dedicated milestone



\---



\## 11. Milestones



\- Manufacturer_Management

\- Product_Management

\- Variant_Management

\- Inventory_Movement_System

\- Delivery_Grouping

\- External_Event_Ingestion

\- Data_Seeding

\- Authentication_Authorization



\---



\## 12. Non-Goals



\- Full e-commerce platform

\- Financial transactions

\- Logistics or shipment tracking

\- Advanced inventory features (reservations, batching)



\---



\## 13. Engineering \& Technical Design



\### Tech Stack

\- .NET 10

\- Docker (containerization)

\- Kubernetes (deployment)

\- Docker Compose (local dependencies)



\### Authentication

\- Keycloak



\### Observability



\#### Local

\- Grafana

\- Prometheus



\#### Production

\- Azure Application Insights



\### Messaging \& Mediation



\- Kafka for event streaming

\- Emit for:

&#x20; - Mediation

&#x20; - Kafka consumers

&#x20; - Kafka producers



Reference:

https://emitdotnet.github.io/Emit/getting-started/installation/



\### Database

\- MongoDB

