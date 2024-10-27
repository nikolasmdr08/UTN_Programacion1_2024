using System;
using System.Collections.Generic;
using System.Media;
using System.Security.AccessControl;
using Tao.Sdl;

namespace MyGame
{
    class Program
    {
        // Estados del juego
        enum GameState { Titulo, Juego, FinJuego, Creditos, Pausa }
        enum Orientation { Vertical, Horizontal }
        static GameState currentState = GameState.Titulo;

        //Control de teclas
        static Dictionary<int, (bool previous, bool current)> keyStates = new Dictionary<int, (bool, bool)>();

        // Fuentes y Recursos
        static Font font, fontTitle;
        static Image backgroundImage, playerImageIcon, playerImage, layer1, layer2_1, layer2_2, layer3_1, layer3_2, bulletImage;
        static Vector2 playerPosition = new Vector2();

        //variables
        static int optionTitle = 1, optionFinJuego = 1, speedMovement = 10, screenWidth = 1024, screenHeight = 768, imageWidth = 64, imageHeight = 64, paralax = 0;
        static float paralax2 = 0;
        //disparo
        static int shootCooldown = 500; 
        static int lastShootTime = 0; 
        static List<bullet> bullets = new List<bullet>();

        static void Main(string[] args)
        {
            Engine.Initialize();
            font = Engine.LoadFont("assets/Arial.ttf", 24);
            fontTitle = Engine.LoadFont("assets/Arial.ttf", 48);
            backgroundImage = Engine.LoadImage("assets/fondo.png");
            layer1 = Engine.LoadImage("assets/layer_1.png");
            layer2_1 = Engine.LoadImage("assets/layer_2.png");
            layer2_2 = Engine.LoadImage("assets/layer_2.png");
            layer3_1 = Engine.LoadImage("assets/layer_3.png");
            layer3_2 = Engine.LoadImage("assets/layer_3.png");
            bulletImage = Engine.LoadImage("assets/bullet.png");
            playerImageIcon = Engine.LoadImage("assets/playerIcon.png");
            playerImage = Engine.LoadImage("assets/playerImage.png");
            GameLoop();
        }

        static void GameLoop()
        {
            while (true)
            {
                Update();
                Render();
                Sdl.SDL_Delay(20);
            }
        }

        static void CheckInputs()
        {
            UpdateKeyStates();

            if (currentState == GameState.Juego)
            {
                //mantener apretado disparo
                if (Engine.KeyPress(Engine.KEY_Z) || Engine.KeyPress(Engine.KEY_1))
                    HandleContinuousShoot(); 
                //disparopor pulsacion
                if (KeyReleased(Engine.KEY_Z) || KeyReleased(Engine.KEY_1))
                    playerShoot(); 

                playerMovement();
            }

            HandleMovementInput();
            HandleActionInput();
        }

        static void UpdateKeyStates() 
        {
            int[] keys = { Engine.KEY_LEFT, Engine.KEY_RIGHT, Engine.KEY_UP, Engine.KEY_DOWN,
                   Engine.KEY_Z, Engine.KEY_1, Engine.KEY_P, Engine.KEY_ESC };

            foreach (var key in keys)
            {
                if (!keyStates.ContainsKey(key))
                    keyStates[key] = (false, false);

                bool currentState = Engine.KeyPress(key);
                var previousState = keyStates[key].current;
                keyStates[key] = (previousState, currentState);
            }
        }

        static bool KeyReleased(int key)
        {
            if (keyStates.ContainsKey(key))
            {
                var (previous, current) = keyStates[key];
                return previous && !current;  
            }
            return false;
        }


        static void HandleActionInput()
        {
            if (KeyReleased(Engine.KEY_Z) || KeyReleased(Engine.KEY_1))
                HandleMenuSelection();
            if (KeyReleased(Engine.KEY_X) || KeyReleased(Engine.KEY_2))
                SwitchState(GameState.Creditos, GameState.Titulo);
            if (KeyReleased(Engine.KEY_P) || KeyReleased(Engine.KEY_3))
                TogglePause();
            if (KeyReleased(Engine.KEY_ESC) && currentState == GameState.Titulo)
                Environment.Exit(0);
        }

        static void HandleMovementInput()
        {
            if (KeyReleased(Engine.KEY_LEFT) || KeyReleased(Engine.KEY_A))
                UpdateOption(ref optionFinJuego, -1, 1, 2);

            if (KeyReleased(Engine.KEY_RIGHT) || KeyReleased(Engine.KEY_D))
                UpdateOption(ref optionFinJuego, 1, 1, 2);

            if (KeyReleased(Engine.KEY_UP) || KeyReleased(Engine.KEY_W))
                UpdateOption(ref optionTitle, -1, 1, 3);

            if (KeyReleased(Engine.KEY_DOWN) || KeyReleased(Engine.KEY_S))
                UpdateOption(ref optionTitle, 1, 1, 3);
        }

        static void HandleContinuousShoot()
        {
            int currentTime = Environment.TickCount;
            if (currentTime - lastShootTime >= shootCooldown)
            {
                playerShoot();
                lastShootTime = currentTime;
            }
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
                case 1: 
                    currentState = GameState.Juego;
                    playerPosition = new Vector2(200, 200);
                    break;
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
            Engine.Draw(playerImageIcon, playerPosition._x, playerPosition._y);
        }

        static void DrawPauseScreen()
        {
            DrawGameScreen();
            Engine.DrawText("- PAUSA -", 400, 350, 255, 255, 255, fontTitle);
        }

        static void DrawGameScreen()
        {
            Engine.Draw(layer1, 0, 0);
            Engine.Draw(layer2_1, 0 + paralax2, 0);
            Engine.Draw(layer2_2, 1024 + paralax2, 0);
            Engine.Draw(layer3_1, 0 + paralax, 0);
            Engine.Draw(layer3_2, 1024 + paralax,0);
            foreach (bullet bullet in bullets)
            {
                Engine.Draw(bullet._bulletImage, bullet._position._x, bullet._position._y);
            }
            Engine.Draw(playerImage, playerPosition._x, playerPosition._y);
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

            Engine.Draw(playerImageIcon, playerPosition._x, playerPosition._y);
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

        private static void ClampPlayerPosition()
        {
            playerPosition._x = Clamp(playerPosition._x, 0, screenWidth - imageWidth);
            playerPosition._y = Clamp(playerPosition._y, 0, screenHeight - imageHeight);
        }

        private static void playerMovement()
        {
            Vector2 movement = new Vector2();

            if (Engine.KeyPress(Engine.KEY_LEFT) || Engine.KeyPress(Engine.KEY_A))
                movement += Vector2.Left;
            if (Engine.KeyPress(Engine.KEY_RIGHT) || Engine.KeyPress(Engine.KEY_D))
                movement += Vector2.Right;
            if (Engine.KeyPress(Engine.KEY_UP) || Engine.KeyPress(Engine.KEY_W))
                movement += Vector2.Up;
            if (Engine.KeyPress(Engine.KEY_DOWN) || Engine.KeyPress(Engine.KEY_S))
                movement += Vector2.Down;

            playerPosition += movement * speedMovement;
            ClampPlayerPosition();
        }

        private static void playerShoot()
        {
            int offsetY = 30;
            bullet instanciteBullet = new bullet(bulletImage,playerPosition._x, playerPosition._y + offsetY);
            bullets.Add(instanciteBullet);
        }

        static void inGame()
        {
            paralaxController();
            bulletController();
            //enemyMovementLogic();
            //colitionController();
        }

        private static void bulletController()
        {
            if (bullets.Count > 0)
            {
                foreach (bullet bullet in bullets)
                {
                    bullet.move();
                }
            }
        }

        private static void paralaxController()
        {
            paralax--;
            if (paralax < -1024) paralax = 0;
            paralax2 -= 0.5f;
            if (paralax2 < -1024) paralax2 = 0;
        }

        private static void enemyMovementLogic()
        {
            
        }

        private static void colitionController()
        {
            
        }

    }

    class bullet
    {
        public Image _bulletImage;
        public Vector2 _position;
        public int _speed = 7;

        public bullet(Image bulletImage, int intialPositionX, int intialPositionY)
        {
            _position = new Vector2(intialPositionX, intialPositionY);
            _bulletImage = bulletImage;
        }

        public void move()
        {
            this._position += Vector2.Right * this._speed;
        }

        public bool collition()
        {
            return false;
        }


    }
    
    class Vector2
    {
        public int _x { get; set; }
        public int _y { get; set; }

        public Vector2(int x = 0, int y = 0)
        {
            _x = x;
            _y = y;
        }

        public static Vector2 Left => new Vector2(-1, 0);
        public static Vector2 Right => new Vector2(1, 0);
        public static Vector2 Up => new Vector2(0, -1);
        public static Vector2 Down => new Vector2(0, 1);

        public static Vector2 operator +(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1._x + v2._x, v1._y + v2._y);
        }

        public static Vector2 operator *(Vector2 v, int scalar)
        {
            return new Vector2(v._x * scalar, v._y * scalar);
        }
    }


}
