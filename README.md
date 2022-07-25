# TFG-mia6


Para jugar, se debe clonar el repositorio en local y ejecutar Robito.exe, que se encuentra dentro de la carpeta Build.

En la carpeta Proyecto se encuentra el proyecto de Unity en su totalidad, incluyendo los assets y el código fuente. Para inspeccionarlo será necesario descargar e instalar Unity Hub (https://unity.com/download). Después, desde Unity Hub, hacer click en Abrir y seleccionar la carpeta que contiene el proyecto, sin entrar en ella. Esto abrirá el proyecto con el editor de Unity.

La mayor parte del contenido del proyecto son carpetas y archivos que Unity administra automáticamente. La que nos interesa es Assets, que contiene todo lo definido por el usuario. Dentro de Assets tenemos:

 - Board: Contiene la clase BehBoard, que administra el tablero de juego y carga los niveles
 - Characters: Contiene las clases que gobiernan el comportamiento de los elementos del juego y las interacciones entre ellos. También están algunos de sus sprites.
 - Instruction: Contiene clases que gobiernan la lógica de la programación de los personajes: movimientos, condiciones, etc.
 - Resources: Contiene los iconos de la interfaz de usuario y algún sprite.
 - Scenes: Contiene la escena del juego, administrada por Unity, y una clase muy simple que nos permite mover la cámara con WASD
 - Squares: Contiene la clase BehSquare, que gobierna las casillas, además de los sprites de las propias casillas y de elementos como los muros, que originalmente iban a ser tipos de casillas
 - UI: Contiene las clases que gobiernan la lógica de la interfaz de usuario