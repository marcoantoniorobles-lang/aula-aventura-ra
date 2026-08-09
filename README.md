# Aula Aventura RA

Videojuego educativo inmersivo de realidad aumentada (Unity 6 + Vuforia Engine), orientado a fortalecer la atencion sostenida y las funciones ejecutivas en ninos con Trastorno por Deficit de Atencion e Hiperactividad (TDAH).

Proyecto desarrollado para el curso **Topicos Avanzados en Computacion**, Doctorado en Ingenieria de Sistemas e Informatica, Universidad Nacional Mayor de San Marcos (UNMSM).

> El informe tecnico completo del proyecto (motivacion, arquitectura, pruebas, resultados, discusion, conclusiones y trabajos futuros) se entrega como documento separado (`Informe_AulaAventuraRA_FINAL.docx`). Este repositorio contiene unicamente el codigo fuente y los recursos de soporte.

## Estado del prototipo

Ambos modulos estan operativos de extremo a extremo y fusionados en un solo flujo:

```
Modulo 1 (Atencion Selectiva por Color) -> transicion automatica -> Modulo 2 (Mundo Numerico) -> pantalla de resultado combinado
```

- Puntaje combinado: 0-20 puntos (10 por modulo), con pantalla final de celebracion condicional.
- Pendiente: persistencia de datos (SQLite), assets 3D definitivos y validacion piloto con usuarios objetivo. Ver seccion 11.1 y 12 del informe.

## Estructura del repositorio

```
aula-aventura-ra/
├── README.md <- este archivo
├── LICENSE
├── .gitignore <- ignora carpetas generadas de Unity (Library/, Temp/, Build/, etc.)
├── Assets/
│   └── Scripts/
│       ├── ColorTargetManager.cs   Modulo 1 - gestor central del estado del juego
│       ├── AnimalTarget.cs         Modulo 1 - comportamiento por objeto interactivo
│       ├── TouchInputManager.cs    Modulo 1 - puente de entrada (mouse/touch)
│       ├── GameManager_M2.cs       Modulo 2 - maquina de estados del modulo
│       ├── NumberBall.cs           Modulo 2 - arrastre y colocacion de esferas numeradas
│       ├── SlotTarget.cs           Modulo 2 - casillas receptoras
│       ├── Setup_M2.cs             Modulo 2 - auto-ensamblado programatico de la escena
│       ├── ScoreManager.cs         Persistencia de puntaje entre escenas (singleton)
│       └── WebcamDiag.cs           Utilidad de diagnostico de camara (no forma parte del gameplay)
├── tools/
│   └── target-generator/
│       └── generar_target.py       Script Python (Pillow) que genera el marcador AR
├── docs/
│   └── graficos/
│       ├── target_aula_aventura_01.png   Primera version del target
│       ├── target_aula_aventura_02.png   Segunda version del target (zonificada)
│       └── consola_scoremanager_ok.jpg   Captura de consola verificando el fix del ScoreManager
└── CHANGELOG.md
```

> **Nota:** este repositorio contiene solo los archivos de codigo y assets de soporte generados por el equipo. No incluye la carpeta `Assets` completa de Unity (materiales, prefabs, escenas `.unity`, configuracion de Vuforia, `ProjectSettings/`, etc.), ya que esos artefactos binarios/de proyecto deben regenerarse abriendo el proyecto en el Editor de Unity 6 con el paquete de Vuforia Engine instalado. Para reconstruir el proyecto: crear un proyecto Unity 6000.3.17f1 (3D URP), instalar Vuforia Engine 11.4.4 desde el Package Manager, copiar los scripts de `Assets/Scripts/` al proyecto, y seguir la descripcion de jerarquia de escena de la seccion 6.2 del informe para recrear `Modulo1ok.unity` y `Modulo2ok.unity`.

## Scripts - que hace cada uno

| Script | Modulo | Rol |
|---|---|---|
| `ColorTargetManager.cs` | 1 | Maquina de estados del juego: color objetivo, verificacion, puntaje, fin de partida. Guarda el puntaje del modulo y dispara la transicion automatica al Modulo 2. |
| `AnimalTarget.cs` | 1 | Componente adjunto a cada esfera interactiva; compara su color contra el color objetivo vigente al recibir un toque. |
| `TouchInputManager.cs` | 1 | Traduce el clic/toque en un `Physics.Raycast` sobre la escena (sustituye a `OnMouseDown()`, incompatible con el New Input System, ver hallazgo 8.5 del informe). |
| `GameManager_M2.cs` | 2 | Gestiona el ciclo del Modulo 2: baraja la posicion inicial de las esferas (Fisher-Yates), detecta cada colocacion correcta, calcula el puntaje y dispara la secuencia de victoria. |
| `NumberBall.cs` | 2 | Arrastre y colocacion de cada esfera numerada, calculado en el espacio local del `ImageTarget` (ver hallazgos 8.12/8.13 del informe). |
| `SlotTarget.cs` | 2 | Casillas receptoras que validan la colocacion correcta de cada esfera. |
| `Setup_M2.cs` | 2 | Auto-ensamblado programatico de la escena `Modulo2ok`: crea las 5 esferas, los 5 slots, el confeti y la UI completa por codigo. |
| `ScoreManager.cs` | 1+2 | Singleton `DontDestroyOnLoad` que persiste `scoreModulo1` y `scoreModulo2` entre escenas (ver hallazgo 8.14 del informe). |
| `WebcamDiag.cs` | - | Utilidad de diagnostico de camara usada durante el desarrollo; no forma parte del flujo de juego. |

## Herramienta externa: generador de target

`tools/target-generator/generar_target.py` genera de forma programatica (Python 3 + Pillow) la imagen de marcador AR (`target_aula_aventura_02.png`) que usan ambos modulos, zonificando la imagen en una franja de patron denso (40% inferior, para el tracking de Vuforia) y una zona plana (60% superior, para que las esferas del juego no compitan visualmente con el marcador).

```bash
pip install Pillow
python tools/target-generator/generar_target.py
```

## Requisitos para abrir el proyecto en Unity

- Unity 6000.3.17f1 (Unity 6 LTS) o superior compatible.
- Vuforia Engine 11.4.4 (Package Manager o paquete `.unitypackage` de Vuforia Developer Portal).
- Licencia de Vuforia (App License Key) dada de alta en `Vuforia Configuration` - requisito obligatorio para que el motor inicialice (ver hallazgo 8.1 del informe).
- `Active Input Handling` configurado como **Input Manager (Old)** de forma exclusiva (Project Settings -> Player) - ver hallazgos 8.5, 8.8 y 8.10 del informe.

## Hallazgos tecnicos documentados

El informe tecnico documenta 16 hallazgos de depuracion con causa raiz y solucion (secciones 8.1 a 8.16), incluyendo problemas de espacios de coordenadas (mundo vs. local del marcador), incompatibilidades del sistema de entrada de Unity, y la persistencia de puntaje entre escenas. Se recomienda revisar esa seccion antes de modificar `NumberBall.cs`, `SlotTarget.cs` o `ScoreManager.cs`.

## Licencia

Uso academico - Doctorado en Ingenieria de Sistemas e Informatica, UNMSM.
