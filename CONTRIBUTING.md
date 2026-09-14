# Guía de Contribución 🇪🇸

¡Gracias por tu interés en contribuir a **LSPDFR Spanish Police Suite**!

## Pautas de Desarrollo

1. **Estándar C# 5 Estricto**:
   - Todo el código debe compilar con `csc.exe` v4.8 sin características de C# 6+.
   - Prohibido el uso de:
     - `$"..."` (interpolación de cadenas) $\rightarrow$ usar `string.Format()` o concatenación.
     - `?.` (operador null-conditional) $\rightarrow$ comprobación explícita `if (x != null)`.
     - `nameof()` $\rightarrow$ cadenas literales.
     - `async/await` $\rightarrow$ usar `GameFiber.StartNew()`.

2. **Localización y Realismo**:
   - Toda la terminología y protocolos deben reflejar fielmente la operativa del **Cuerpo Nacional de Policía (CNP)**, **Guardia Civil** y la legislación española vigente.
   - Textos 100% en español de España.

3. **Flujo de Trabajo**:
   - Crea un Fork del repositorio.
   - Desarrolla tus cambios en una rama descriptiva (`feature/nueva-mision`).
   - Envía tu Pull Request rellenando la plantilla.
