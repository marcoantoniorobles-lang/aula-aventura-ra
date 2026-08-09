# Changelog

## [v2 -> Final] Fusion de Modulo 2 y flujo combinado
- Modulo 2 (Mundo Numerico) implementado completo: `GameManager_M2.cs`, `NumberBall.cs`, `SlotTarget.cs`, `Setup_M2.cs`.
- Persistencia de puntaje entre escenas mediante `ScoreManager.cs` (singleton `DontDestroyOnLoad`).
- Flujo fusionado: Modulo 1 -> transicion automatica -> Modulo 2 -> pantalla de resultado combinado (0-20 pts) con celebracion condicional.
- Corregido: arrastre de esferas deformado por inclinacion del marcador (fix: `Vector3.up` -> `transform.parent.up`).
- Corregido: snap incorrecto de esferas en casillas (fix: comparacion de coordenadas en espacio local del `ImageTarget` via `LocalXZ()`).
- Corregido: puntaje del Modulo 1 se perdia al llegar a resultados (fix: `ScoreManager` agregado explicitamente a la escena `Modulo1ok`).
- Removida pantalla intermedia de fin del Modulo 1 (innecesaria tras la fusion de flujo).
- Corregido solapamiento de texto en la pantalla de resultados del Modulo 2.

## [v1] Modulo 1 - Atencion Selectiva por Color
- Prototipo inicial operativo de extremo a extremo: `ColorTargetManager.cs`, `AnimalTarget.cs`, `TouchInputManager.cs`.
- Generador de target en Python (`generar_target.py`), con segunda iteracion zonificada para separar el patron de tracking de las esferas del juego.
- Resueltas incompatibilidades entre `OnMouseDown()` y el New Input System, escalado de interfaz entre resoluciones, y el modulo de entrada del `EventSystem` en Android.

Ver el informe tecnico completo (secciones 8 y 10) para el detalle de causa raiz y solucion de cada hallazgo.
