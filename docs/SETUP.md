# Configuración inicial del proyecto

# Central Telefónica TG

Guía para preparar el entorno local después de clonar el repositorio.

---

# 1. Clonar repositorio

Ejecutar:

```bash
git clone https://github.com/JoseMoralesC/Central_TG.git
```

Entrar al proyecto:

```bash
cd Central_TG
```

---

# 2. Seleccionar rama de trabajo

Cada integrante debe trabajar únicamente en su rama asignada.

## Python - Identificador

```bash
git checkout feature/python
```

---

## Java - Proveedor

```bash
git checkout feature/java
```

---

## C# - Simulador

```bash
git checkout feature/csharp
```

---

# 3. Crear archivo de entorno

El proyecto incluye:

```txt
.env.example
```

Este archivo debe copiarse como:

```txt
.env
```

Comando:

Windows:

```cmd
copy .env.example .env
```

Linux:

```bash
cp .env.example .env
```

---

# 4. Configurar credenciales

Abrir:

```txt
.env
```

Cambiar los datos según el integrante.

Ejemplo MySQL:

```env
MYSQL_USER=usuario
MYSQL_PASSWORD=password
```

Ejemplo SQL Server:

```env
SQLSERVER_USER=usuario
SQLSERVER_PASSWORD=password
```

IMPORTANTE:

El archivo:

```txt
.env
```

NO debe subirse a GitHub.

---

# 5. Verificar conexión a bases de datos

El proyecto utiliza bases de datos remotas compartidas.

No es necesario ejecutar scripts SQL al clonar el repositorio.

Los scripts ubicados en:

database/mysql_identificador/

database/sqlserver_proveedor/

son únicamente para:

- documentación
- respaldo
- creación inicial
- reconstrucción del ambiente


Cada integrante debe configurar únicamente su archivo .env con las credenciales asignadas.


## MySQL Identificador

Host:

100.114.84.5

Puerto:

3306


## SQL Server Proveedor

Host:

100.88.25.17

Puerto:

49172


Si la conexión funciona, el ambiente está listo.

# 6. Verificar puertos

El proyecto utiliza:

|Servicio|Puerto|
|-|-|
|Identificador Python|5000|
|Proveedor Java|6000|

Verificar que estén disponibles.

---

# 7. Revisar contratos

Antes de programar revisar:

```txt
shared/contracts/
```

Estos archivos definen cómo se comunican los sistemas.

No modificar contratos sin avisar al equipo.

---

# 8. Revisar ejemplos

Casos disponibles:

```txt
shared/examples/
```

Incluye:

- llamada exitosa
- saldo insuficiente
- teléfono inactivo
- SIM inválida
- ubicación inválida
- consulta saldo
- finalizar llamada

---

# 9. Flujo diario recomendado

Antes de programar:

Actualizar main:

```bash
git checkout main

git pull origin main
```

Entrar a la rama personal:

```bash
git checkout feature/nombre
```

Actualizar rama:

```bash
git pull origin feature/nombre
```

Fusionar cambios recientes:

```bash
git merge main
```

---

# 10. Subir cambios

Ver cambios:

```bash
git status
```

Preparar:

```bash
git add .
```

Crear commit:

```bash
git commit -m "mensaje descriptivo"
```

Enviar:

```bash
git push origin feature/nombre
```

---

# Reglas del equipo

- No trabajar directamente en main.
- Avisar commits importantes.
- Mantener mensajes claros.
- No subir archivos `.env`.
- Respetar contratos JSON.
- Respetar estructura del proyecto.

---

# Estado esperado después de configuración

Cada integrante debe tener:

✔ Proyecto actualizado  
✔ Rama correcta  
✔ Archivo `.env` creado  
✔ Base correspondiente funcionando  
✔ Contratos revisados  

Después de esto puede iniciar desarrollo.