using System;
using System.Media;
using Tao.Sdl;

namespace MyGame
{
    
    class Program
    {
        // Posibles estados del juego
        enum GameState
        {
            Titulo,
            Juego,
            FinJuego,
            Creditos,
            Pausa
        }
        // Estado inicial del juego
        //static GameState currentState = GameState.Titulo;
        static GameState currentState = GameState.Creditos;
        //Fuentes
        static Font font;
        // Carga de imagenes
        static Image image = Engine.LoadImage("assets/fondo.png");
        static Image playerImage = Engine.LoadImage("assets/player.png");
        //personaje
        static Vector2 playerPosition = new Vector2();

        static int optionTitle = 1;

        static void Main(string[] args)
        {
            Engine.Initialize();
            font = Engine.LoadFont("assets/Arial.ttf", 24);

            while (true)
            {
                Update();
                Render();
                Sdl.SDL_Delay(60);  
            }
        }

        static void CheckInputs()
        {
            if (Engine.KeyPress(Engine.KEY_LEFT))
            {
                switch (currentState)
                {
                    case GameState.Titulo:
                        break;
                    case GameState.Juego:
                        break;
                    case GameState.FinJuego:
                        break;
                    case GameState.Creditos:
                        break;
                    case GameState.Pausa:
                        break;
                    default:
                        break;
                }
            }

            if (Engine.KeyPress(Engine.KEY_RIGHT))
            {
                switch (currentState)
                {
                    case GameState.Titulo:
                        break;
                    case GameState.Juego:
                        break;
                    case GameState.FinJuego:
                        break;
                    case GameState.Creditos:
                        break;
                    case GameState.Pausa:
                        break;
                    default:
                        break;
                }
            }

            if (Engine.KeyPress(Engine.KEY_UP))
            {
                switch (currentState)
                {
                    case GameState.Titulo:
                        optionTitle--;
                        if (optionTitle < 1)
                        {
                            optionTitle = 3;
                        }
                        break;
                    case GameState.Juego:
                        break;
                    case GameState.FinJuego:
                        break;
                    case GameState.Creditos:
                        break;
                    case GameState.Pausa:
                        break;
                    default:
                        break;
                }
            }

            if (Engine.KeyPress(Engine.KEY_DOWN))
            {
                switch (currentState)
                {
                    case GameState.Titulo:
                        optionTitle++;
                        if (optionTitle > 3)
                        {
                            optionTitle = 1;
                        }
                        break;
                    case GameState.Juego:
                        break;
                    case GameState.FinJuego:
                        break;
                    case GameState.Creditos:
                        break;
                    case GameState.Pausa:
                        break;
                    default:
                        break;
                }
            }

            if (Engine.KeyPress(Engine.KEY_Z))
            {
                switch (currentState)
                {
                    case GameState.Titulo:
                        switch (optionTitle)
                        {
                            case 1:
                                currentState = GameState.Juego;
                                break;
                            case 2:
                                currentState = GameState.Creditos;
                                break;
                            case 3:
                                Environment.Exit(0);
                                break;
                        }
                        break;
                    case GameState.Creditos:
                        break;
                    case GameState.Juego:
                        break;
                    default:
                        break;
                }
            }

        }

        static void Update()
        {
            CheckInputs();
        }

        static void Render()
        {
            Engine.Clear();
            Engine.Draw(image, 0, 0);
            switch (currentState)
            {
                case GameState.Titulo:
                    imprimirPantalla("titulo");
                    break;
                case GameState.Juego:
                    imprimirPantalla("juego");
                    break;
                case GameState.FinJuego:
                    imprimirPantalla("fin_juego");
                    break;
                case GameState.Creditos:
                    imprimirPantalla("credito");
                    break;
                case GameState.Pausa:
                    imprimirPantalla("juego");
                    imprimirPantalla("pausa");
                    break;
                default:
                    break;
            }
            Engine.Show();
        }

        private static void imprimirPantalla(string pantalla)
        {
            switch (pantalla)
            {
                case "titulo":
                    Engine.DrawText("Titulo Juego", 850, 550, 255, 255, 255, font);
                    Engine.DrawText("Jugar", 850, 600, 255, 255, 255, font);
                    Engine.DrawText("Creditos", 850, 650, 255, 255, 255, font);
                    Engine.DrawText("Salir", 850, 700, 255, 255, 255, font);
                    switch (optionTitle)
                    {
                        case 1:
                            playerPosition = new Vector2(810, 600);
                            break;
                        case 2:
                            playerPosition = new Vector2(810, 650);
                            break;
                        case 3:
                            playerPosition = new Vector2(810, 700);
                            break;
                    }
                    Engine.Draw(playerImage, playerPosition.X, playerPosition.Y);
                    break;
                case "juego":
                    break;
                case "fin_juego":
                    break;
                case "credito":
                    break;
                case "pausa":
                    break;
                default:
                    break;
            }
        }
    }

    class Vector2
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Vector2()
        {
            X = 0;
            Y = 0;
        }

        public Vector2(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}