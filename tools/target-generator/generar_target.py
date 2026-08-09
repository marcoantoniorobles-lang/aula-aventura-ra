#CODIGO PYTHON PARA GENERAR LA IMAGEN TARGET
from PIL import Image, ImageDraw, ImageFont
import random

# ---------------------------------------------------------------------
# Generador del target AR para "Aula Aventura RA"
# Zonifica la imagen: 60% superior = zona plana (para las esferas del juego),
# 40% inferior = patron denso y asimetrico (para el tracking de Vuforia).
# ---------------------------------------------------------------------

random.seed(42)

W, H = 1200, 1200
img = Image.new("RGB", (W, H), (245, 244, 240))  # off-white
draw = ImageDraw.Draw(img)

split_y = int(H * 0.60)  # linea que separa zona plana (arriba) de patron (abajo)

# ---- ZONA SUPERIOR: fondo casi liso con textura minima ----
for _ in range(600):
    x = random.randint(10, W - 10)
    y = random.randint(10, split_y - 10)
    r = random.randint(1, 2)
    shade = random.randint(225, 238)
    draw.ellipse([x - r, y - r, x + r, y + r], fill=(shade, shade, shade - 2))

# marco + titulo
draw.rectangle([8, 8, W - 8, H - 8], outline=(60, 60, 60), width=4)
try:
    font_title = ImageFont.truetype(
        "/usr/share/fonts/truetype/dejavu/DejaVuSans-Bold.ttf", 26
    )
    font_small = ImageFont.truetype(
        "/usr/share/fonts/truetype/dejavu/DejaVuSans.ttf", 18
    )
except Exception:
    # En Colab, si la fuente DejaVu no esta en esa ruta exacta, PIL usa una por defecto
    font_title = ImageFont.load_default()
    font_small = ImageFont.load_default()

draw.text((30, 25), "AULA AVENTURA RA", font=font_title, fill=(50, 50, 50))
draw.text((30, 58), "TARGET 02 - ZONA DE JUEGO", font=font_small, fill=(90, 90, 90))

# ---- ZONA INFERIOR: patron asimetrico de alto contraste ----
# Paleta deliberadamente sin rojo/azul/verde/amarillo (colores usados por las
# esferas del juego), para que el patron no se confunda visualmente con ellas.
palette = [(20, 20, 20), (90, 90, 90), (150, 40, 130), (200, 130, 20), (60, 60, 40)]

draw.rectangle([8, split_y, W - 8, H - 8], fill=(230, 228, 222))
draw.line([8, split_y, W - 8, split_y], fill=(60, 60, 60), width=5)

random.seed(7)
nodes = []
for i in range(14):
    x = random.randint(40, W - 40)
    y = random.randint(split_y + 30, H - 40)
    nodes.append((x, y))

# lineas de conexion asimetricas (aportan riqueza de features para el tracking)
for i in range(len(nodes)):
    j = (i + random.choice([2, 3, 5])) % len(nodes)
    draw.line([nodes[i], nodes[j]], fill=(80, 80, 80), width=2)

shapes = ["circle", "triangle", "square", "diamond"]
for idx, (x, y) in enumerate(nodes):
    shape = shapes[idx % len(shapes)]
    color = palette[idx % len(palette)]
    s = random.randint(16, 30)
    if shape == "circle":
        draw.ellipse([x - s, y - s, x + s, y + s], fill=color, outline=(20, 20, 20), width=2)
    elif shape == "square":
        draw.rectangle([x - s, y - s, x + s, y + s], fill=color, outline=(20, 20, 20), width=2)
    elif shape == "diamond":
        draw.polygon([(x, y - s), (x + s, y), (x, y + s), (x - s, y)], fill=color, outline=(20, 20, 20))
    else:  # triangle
        draw.polygon([(x, y - s), (x + s, y + s), (x - s, y + s)], fill=color, outline=(20, 20, 20))

# marcas asimetricas en las esquinas (ayudan a orientacion / riqueza de features)
draw.polygon([(20, H - 20), (70, H - 20), (20, H - 70)], fill=(20, 20, 20))
draw.rectangle([W - 70, H - 70, W - 20, H - 20], outline=(20, 20, 20), width=4)
draw.text((30, H - 40), "AA-02", font=font_small, fill=(40, 40, 40))

OUTPUT_NAME = "target_aula_aventura_02.png"
img.save(OUTPUT_NAME)
print("Imagen generada:", OUTPUT_NAME, "-", img.size)

# ---------------------------------------------------------------------
# Si esto corre en Google Colab, descomenta las siguientes lineas para
# visualizarla directamente y descargarla a tu computadora:
# ---------------------------------------------------------------------
# from IPython.display import display
# display(img)
#
# from google.colab import files
# files.download(OUTPUT_NAME)
