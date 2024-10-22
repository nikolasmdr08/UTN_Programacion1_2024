using System;
using System.Media;
using Tao.Sdl;

namespace MyGame
{
    class Program
    {
        // Estados del juego
        enum GameState { Titulo, Juego, FinJuego, Creditos, Pausa }
        enum Orientation { Vertical, Horizontal }
        static GameState currentState = GameState.FinJuego;

        // Fuentes y Recursos
        static Font font, fontTitle;
        static Image backgroundImage, playerImageIcon;
        static Vector2 playerPosition = new Vector2();

        static int optionTitle = 1, optionFinJuego = 1;

        static void Main(string[] args)
        {
            Engine.Initialize();
            font = Engine.LoadFont("assets/Arial.ttf", 24);
            fontTitle = Engine.LoadFont("assets/Arial.ttf", 48);
            backgroundImage = Engine.LoadImage("assets/fondo.png");
            playerImageIcon = Engine.LoadImage("assets/player.png");

            GameLoop();
        }

        static void GameLoop()
        {
            while (true)
            {
                Update();
                Render();
                Sdl.SDL_Delay(60);
            }
        }

        static void CheckInputs()
        {
            HandleMovementInput();
            HandleActionInput();
        }

        static void HandleMovementInput()
        {
            if (Engine.KeyPress(Engine.KEY_LEFT) || Engine.KeyPress(Engine.KEY_A))
                UpdateOption(ref optionFinJuego, -1, 1, 2);
            if (Engine.KeyPress(Engine.KEY_RIGHT) || Engine.KeyPress(Engine.KEY_D))
                UpdateOption(ref optionFinJuego, 1, 1, 2);
            if (Engine.KeyPress(Engine.KEY_UP) || Engine.KeyPress(Engine.KEY_W))
                UpdateOption(ref optionTitle, -1, 1, 3);
            if (Engine.KeyPress(Engine.KEY_DOWN) || Engine.KeyPress(Engine.KEY_S))
                UpdateOption(ref optionTitle, 1, 1, 3);
        }

        static void HandleActionInput()
        {
            if (Engine.KeyPress(Engine.KEY_Z) || Engine.KeyPress(Engine.KEY_1))
                HandleMenuSelection();
            if (Engine.KeyPress(Engine.KEY_X) || Engine.KeyPress(Engine.KEY_2))
                SwitchState(GameState.Creditos, GameState.Titulo);
            if (Engine.KeyPress(Engine.KEY_P) || Engine.KeyPress(Engine.KEY_3))
                TogglePause();
            if (Engine.KeyPress(Engine.KEY_ESC) && currentState == GameState.Titulo)
                Environment.Exit(0);
        }

        static void UpdateOption(ref int option, int change, int min, int max)
        {
            option = Clamp(option + change, min, max);
        }

        static void SwitchState(GameState fromState, GameState toState)
        {
            if (currentState == fromState) currentState = toState;
        }

        static void TogglePause()
        {
            currentState = currentState == GameState.Juego ? GameState.Pausa : GameState.Juego;
        }

        static void HandleMenuSelection()
        {
            switch (currentState)
            {
                case GameState.Titulo:
                    ProcessTitleSelection();
                    break;
                case GameState.FinJuego:
                    ProcessEndGameSelection();
                    break;
                case GameState.Creditos:
                    currentState = GameState.Titulo;
                    break;
            }
        }

        static void ProcessTitleSelection()
        {
            switch (optionTitle)
            {
                case 1: currentState = GameState.Juego; break;
                case 2: currentState = GameState.Creditos; break;
                case 3: Environment.Exit(0); break;
            }
            ResetOptions();
        }

        static void ProcessEndGameSelection()
        {
            currentState = optionFinJuego == 1 ? GameState.Juego : GameState.Titulo;
            ResetOptions();
        }

        static void ResetOptions()
        {
            optionTitle = optionFinJuego = 1;
        }

        static void Update()
        {
            CheckInputs();
            if (currentState == GameState.Juego) inGame();
        }

        static void Render()
        {
            Engine.Clear();
            Engine.Draw(backgroundImage, 0, 0);
            DrawCurrentState();
            Engine.Show();
        }

        static void DrawCurrentState()
        {
            switch (currentState)
            {
                case GameState.Titulo: DrawTitleScreen(); break;
                case GameState.Juego: DrawGameScreen(); break;
                case GameState.FinJuego: DrawEndGameScreen(); break;
                case GameState.Creditos: DrawCreditsScreen(); break;
                case GameState.Pausa: DrawPauseScreen(); break;
            }
        }

        static void DrawTitleScreen()
        {
            Engine.DrawText("Titulo Juego", 300, 200, 255, 255, 255, fontTitle);
            DrawTextWithOptions(" ", new string[] { "Jugar", "Creditos", "Salir" }, optionTitle, 800, 600,Orientation.Vertical);
        }

        static void DrawEndGameScreen()
        {
            Engine.DrawText("Juego Terminado", 300, 200, 255, 255, 255, fontTitle);
            Engine.DrawText($"SCORE: 0", 450, 350, 255, 255, 255, font);
            DrawTextWithOptions(" ", new string[] { "REINICIAR", "PANTALLA TITULO" }, optionFinJuego, 300, 650, Orientation.Horizontal);
        }

        static void DrawCreditsScreen()
        {
            Engine.DrawText("CREDITOS", 350, 100, 255, 255, 255, fontTitle);
            string[] controls = {
                "Controles:", "Arriba: W / Flecha Arriba", "Abajo: S / Flecha Abajo",
                "Izquierda: A / Flecha Izquierda", "Derecha: D / Flecha Derecha",
                "Disparar / Aceptar: Z / Num 1", "Volver: X / Num 2", "Pausa: P"
            };
            DrawTextList(controls, 200, 200);
            string[] gameInfo = {"Materia: Programación 1", 
                "Profesores: [Nombre del Profesor]", 
                "Alumno: Nicolas Madera"
            };
            DrawTextList(gameInfo, 600, 200);
            Engine.DrawText("VOLVER", 800, 600, 255, 255, 255, font);
            playerPosition = new Vector2(760, 610);
            Engine.Draw(playerImageIcon, playerPosition.X, playerPosition.Y);
        }

        static void DrawPauseScreen()
        {
            DrawGameScreen();
            Engine.DrawText("- PAUSA -", 400, 350, 255, 255, 255, fontTitle);
        }

        static void DrawGameScreen()
        {
            
        }

        static void DrawTextWithOptions(string title, string[] options, int selectedOption, int startX, int startY, Orientation orientation)
        {
            Engine.DrawText(title, startX, startY, 255, 255, 255, fontTitle);

            for (int i = 0; i < options.Length; i++)
            {
                int offsetX = orientation == Orientation.Horizontal ? i * 300 : 0;
                int offsetY = orientation == Orientation.Vertical ? i * 50 : 0;

                Engine.DrawText(options[i], startX + offsetX, startY + offsetY, 255, 255, 255, font);

                if (i + 1 == selectedOption)
                {
                    int iconX = startX + offsetX -40;
                    int iconY = startY + offsetY + 10;
                    playerPosition = new Vector2(iconX, iconY);
                }
            }

            Engine.Draw(playerImageIcon, playerPosition.X, playerPosition.Y);
        }

        static void DrawTextList(string[] texts, int startX, int startY)
        {
            for (int i = 0; i < texts.Length; i++)
            {
                Engine.DrawText(texts[i], startX, startY + i * 50, 255, 255, 255, font);
            }
        }

        public static int Clamp(int value, int min, int max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        static void inGame() { 
        
        }
    }

    class Vector2
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Vector2(int x = 0, int y = 0)
        {
            X = x;
            Y = y;
        }
    }
}
