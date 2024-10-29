using System;
using System.Collections.Generic;
using Tao.Sdl;

namespace MyGame
{
    class Program
    {
        // Estados del juego
        enum GameState { Titulo, Juego, FinJuego, Creditos, Pausa }
        enum Orientation { Vertical, Horizontal }
        static GameState _currentState = GameState.Titulo;

        //Control de teclas
        static Dictionary<int, (bool previous, bool current)> _keyStates = new Dictionary<int, (bool, bool)>();

        // Fuentes y Recursos
        static Font _font, _fontTitle;
        static Image _backgroundImage, _playerImageIcon, _playerImage, _layer1, _layer2_1, _layer2_2, _layer3_1, _layer3_2, _bulletImage, _enemyLinealImage
            , _enemyZigzagImage, _enemyMinaImage, _bulletEnemyImage;
        static Vector2 _playerPosition = new Vector2();

        //variables
        static int _optionTitle = 1, _optionFinJuego = 1, _speedMovement = 10, _screenWidth = 1024, _screenHeight = 768, _imageWidth = 64, _imageHeight = 64, _paralax = 0;
        static float _paralax2 = 0;
        //disparo
        static int _shootCooldown = 500; 
        static int _lastShootTime = 0; 
        static List<bullet> _bullets = new List<bullet>();
        static List<bullet> _enemyBullets = new List<bullet>();

        static Level _nivelActual;
        static int _nivel;
        static int _score;


        static void Main(string[] args)
        {
            Engine.Initialize();
            _font = Engine.LoadFont("assets/Arial.ttf", 24);
            _fontTitle = Engine.LoadFont("assets/Arial.ttf", 48);
            _backgroundImage = Engine.LoadImage("assets/fondo.png");
            _layer1 = Engine.LoadImage("assets/layer_1.png");
            _layer2_1 = Engine.LoadImage("assets/layer_2.png");
            _layer2_2 = Engine.LoadImage("assets/layer_2.png");
            _layer3_1 = Engine.LoadImage("assets/layer_3.png");
            _layer3_2 = Engine.LoadImage("assets/layer_3.png");
            _bulletImage = Engine.LoadImage("assets/bullet.png");
            _bulletEnemyImage = Engine.LoadImage("assets/enemy_bullet.png");
            _playerImageIcon = Engine.LoadImage("assets/playerIcon.png");
            _playerImage = Engine.LoadImage("assets/playerImage.png");
            _enemyLinealImage = Engine.LoadImage("assets/enemy_lineal.png");
            _enemyZigzagImage = Engine.LoadImage("assets/enemy_zigzag.png");
            _enemyMinaImage = Engine.LoadImage("assets/enemy_mina.png");
            _nivel = 1;
            _score = 0;
            _nivelActual = CargarNivel(_nivel);
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

            if (_currentState == GameState.Juego)
            {
                //mantener apretado disparo
                if (Engine.KeyPress(Engine.KEY_Z) || Engine.KeyPress(Engine.KEY_1))
                    HandleContinuousShoot(); 
                //disparopor pulsacion
                if (KeyReleased(Engine.KEY_Z) || KeyReleased(Engine.KEY_1))
                    PlayerShoot(); 

                PlayerMovement();
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
                if (!_keyStates.ContainsKey(key))
                    _keyStates[key] = (false, false);

                bool currentState = Engine.KeyPress(key);
                var previousState = _keyStates[key].current;
                _keyStates[key] = (previousState, currentState);
            }
        }

        static bool KeyReleased(int key)
        {
            if (_keyStates.ContainsKey(key))
            {
                var (previous, current) = _keyStates[key];
                return previous && !current;  
            }
            return false;
        }

        static Level CargarNivel(int level)
        {
            int offset = 100;
            int posicionInicialX = 1050;
            int enemySpeed = 5 * level;

            Level nivel = new Level();
            switch (level)
            {
                case 1:
                    nivel._waves.Add(GenerarOleada(_enemyLinealImage, "comun", 1, "lineal", posicionInicialX, 200, offset, enemySpeed, ""));
                    nivel._waves.Add(GenerarOleada(_enemyLinealImage, "comun", 5, "lineal", posicionInicialX, 500, offset, enemySpeed, ""));
                    nivel._waves.Add(GenerarOleada(_enemyLinealImage, "comun", 10, "lineal", posicionInicialX, 300, offset, enemySpeed, ""));
                    nivel._waves.Add(GenerarOleada(_enemyLinealImage, "comun", 10, "lineal", posicionInicialX, 100, offset, enemySpeed, ""));
                    nivel._waves.Add(GenerarOleada(_enemyLinealImage, "comun", 5, "lineal", posicionInicialX, 400, offset, enemySpeed, ""));
                    nivel._waves.Add(GenerarOleada(_enemyLinealImage, "comun", 10, "lineal", posicionInicialX, 600, offset, enemySpeed, ""));
                    nivel._waves.Add(GenerarOleada(_enemyLinealImage, "comun", 5, "lineal", posicionInicialX, 350, offset, enemySpeed, ""));
                    break;
                case 2:
                    nivel._waves.Add(GenerarOleada(_enemyLinealImage, "comun", 5, "lineal", posicionInicialX, 200, offset, enemySpeed, ""));
                    nivel._waves.Add(GenerarOleada(_enemyLinealImage, "comun", 5, "lineal", posicionInicialX, 500, offset, enemySpeed, ""));
                    nivel._waves.Add(GenerarOleada(_enemyZigzagImage, "comun", 10, "zigzag", posicionInicialX, 300, offset, enemySpeed, ""));
                    nivel._waves.Add(GenerarOleada(_enemyZigzagImage, "comun", 10, "zigzag", posicionInicialX, 100, offset, enemySpeed, ""));
                    nivel._waves.Add(GenerarOleada(_enemyLinealImage, "comun", 5, "lineal", posicionInicialX, 400, offset, enemySpeed, ""));
                    nivel._waves.Add(GenerarOleada(_enemyZigzagImage, "comun", 10, "zigzag", posicionInicialX, 600, offset, enemySpeed, ""));
                    nivel._waves.Add(GenerarOleada(_enemyLinealImage, "comun", 5, "lineal", posicionInicialX, 350, offset, enemySpeed, ""));
                    nivel._waves.Add(GenerarOleada(_enemyLinealImage, "comun", 5, "lineal", posicionInicialX, 200, offset, enemySpeed, ""));
                    nivel._waves.Add(GenerarOleada(_enemyLinealImage, "comun", 5, "lineal", posicionInicialX, 500, offset, enemySpeed, ""));
                    nivel._waves.Add(GenerarOleada(_enemyZigzagImage, "comun", 10, "zigzag", posicionInicialX, 300, offset, enemySpeed, ""));
                    nivel._waves.Add(GenerarOleada(_enemyZigzagImage, "comun", 10, "zigzag", posicionInicialX, 100, offset, enemySpeed, ""));
                    nivel._waves.Add(GenerarOleada(_enemyLinealImage, "comun", 5, "lineal", posicionInicialX, 400, offset, enemySpeed, ""));
                    nivel._waves.Add(GenerarOleada(_enemyZigzagImage, "comun", 10, "zigzag", posicionInicialX, 600, offset, enemySpeed, ""));
                    nivel._waves.Add(GenerarOleada(_enemyLinealImage, "comun", 5, "lineal", posicionInicialX, 350, offset, enemySpeed, ""));
                    break;
                case 3:
                    nivel._waves.Add(GenerarOleada(_enemyMinaImage, "comun", 1, "mina", posicionInicialX, 200, offset, 30, "explosion"));
                    nivel._waves.Add(GenerarOleada(_enemyLinealImage, "comun", 5, "lineal", posicionInicialX, 200, offset, 30, "explosion"));
                    nivel._waves.Add(GenerarOleada(_enemyMinaImage, "comun", 1, "mina", posicionInicialX, 200, offset, 30, "explosion"));
                    nivel._waves.Add(GenerarOleada(_enemyZigzagImage, "comun", 5, "zigzag", posicionInicialX, 100, offset, enemySpeed, "explosion"));
                    nivel._waves.Add(GenerarOleada(_enemyMinaImage, "comun", 1, "mina", posicionInicialX, 200, offset, 30, "explosion"));
                    nivel._waves.Add(GenerarOleada(_enemyLinealImage, "comun", 5, "lineal", posicionInicialX, 200, offset, 30, "explosion"));
                    nivel._waves.Add(GenerarOleada(_enemyMinaImage, "comun", 1, "mina", posicionInicialX, 200, offset, 30, "explosion"));
                    nivel._waves.Add(GenerarOleada(_enemyZigzagImage, "comun", 5, "zigzag", posicionInicialX, 100, offset, enemySpeed, "explosion"));
                    nivel._waves.Add(GenerarOleada(_enemyMinaImage, "comun", 1, "mina", posicionInicialX, 200, offset, 30, "explosion"));
                    nivel._waves.Add(GenerarOleada(_enemyLinealImage, "comun", 5, "lineal", posicionInicialX, 200, offset, 30, "explosion"));
                    nivel._waves.Add(GenerarOleada(_enemyMinaImage, "comun", 1, "mina", posicionInicialX, 200, offset, 30, "explosion"));
                    nivel._waves.Add(GenerarOleada(_enemyZigzagImage, "comun", 5, "zigzag", posicionInicialX, 100, offset, enemySpeed, "explosion"));
                    break;
                default:
                    _currentState = GameState.FinJuego;
                    break;
            }
            

            return nivel;
        }

        static Wave GenerarOleada(Image image, string tipo, int cantidadEnemigos, string patronMovimiento, int posicionInicialX, int posicionInicialY, int offset, int speed, string tipoAtaque)
        {
            Wave oleada = new Wave();
 
            for (int i = 0; i < cantidadEnemigos; i++)
            {
                Vector2 posicion = new Vector2(posicionInicialX + offset * i, posicionInicialY);
                oleada._enemies.Add(new Enemy(image, tipo, posicion, speed, patronMovimiento, tipoAtaque));
            }

            return oleada;
        }

        public static int GetRandomInt(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max + 1); 
        }

        static void HandleActionInput()
        {
            if (KeyReleased(Engine.KEY_Z) || KeyReleased(Engine.KEY_1))
                HandleMenuSelection();
            if (KeyReleased(Engine.KEY_X) || KeyReleased(Engine.KEY_2))
                SwitchState(GameState.Creditos, GameState.Titulo);
            if (KeyReleased(Engine.KEY_P) || KeyReleased(Engine.KEY_3))
                TogglePause();
            if (KeyReleased(Engine.KEY_ESC) && _currentState == GameState.Titulo)
                Environment.Exit(0);
        }

        static void HandleMovementInput()
        {
            if (KeyReleased(Engine.KEY_LEFT) || KeyReleased(Engine.KEY_A))
                UpdateOption(ref _optionFinJuego, -1, 1, 2);

            if (KeyReleased(Engine.KEY_RIGHT) || KeyReleased(Engine.KEY_D))
                UpdateOption(ref _optionFinJuego, 1, 1, 2);

            if (KeyReleased(Engine.KEY_UP) || KeyReleased(Engine.KEY_W))
                UpdateOption(ref _optionTitle, -1, 1, 3);

            if (KeyReleased(Engine.KEY_DOWN) || KeyReleased(Engine.KEY_S))
                UpdateOption(ref _optionTitle, 1, 1, 3);
        }

        static void HandleContinuousShoot()
        {
            int currentTime = Environment.TickCount;
            if (currentTime - _lastShootTime >= _shootCooldown)
            {
                PlayerShoot();
                _lastShootTime = currentTime;
            }
        }

        static void UpdateOption(ref int option, int change, int min, int max)
        {
            option = Clamp(option + change, min, max);
        }

        static void SwitchState(GameState fromState, GameState toState)
        {
            if (_currentState == fromState) _currentState = toState;

            if (_currentState == fromState)
            {
                _currentState = toState;

                if (toState == GameState.Juego)
                {
                    _nivelActual = CargarNivel(_nivel);
                    _playerPosition = new Vector2(200, 200);
                }
            }
        }

        static void TogglePause()
        {
            _currentState = _currentState == GameState.Juego ? GameState.Pausa : GameState.Juego;
        }

        static void HandleMenuSelection()
        {
            switch (_currentState)
            {
                case GameState.Titulo:
                    ProcessTitleSelection();
                    break;
                case GameState.FinJuego:
                    ProcessEndGameSelection();
                    break;
                case GameState.Creditos:
                    _currentState = GameState.Titulo;
                    break;
            }
        }

        static void ProcessTitleSelection()
        {
            switch (_optionTitle)
            {
                case 1: 
                    _currentState = GameState.Juego;
                    _playerPosition = new Vector2(200, 200);
                    break;
                case 2: _currentState = GameState.Creditos; break;
                case 3: Environment.Exit(0); break;
            }
            ResetOptions();
        }

        static void ProcessEndGameSelection()
        {
            _currentState = _optionFinJuego == 1 ? GameState.Juego : GameState.Titulo;
            ResetOptions();
        }

        static void ResetOptions()
        {
            _optionTitle = _optionFinJuego = _nivel = 1;
            _bullets.Clear();
            _enemyBullets.Clear();
            _playerPosition = new Vector2(200, 200);
            _nivelActual = CargarNivel(1);
        }

        static void Update()
        {
            CheckInputs();
            if (_currentState == GameState.Juego) InGame();
        }

        static void Render()
        {
            Engine.Clear();
            Engine.Draw(_backgroundImage, 0, 0);
            DrawCurrentState();
            Engine.Show();
        }

        static void DrawCurrentState()
        {
            switch (_currentState)
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
            Engine.DrawText("Titulo Juego", 300, 200, 255, 255, 255, _fontTitle);
            DrawTextWithOptions(" ", new string[] { "Jugar", "Creditos", "Salir" }, _optionTitle, 800, 600,Orientation.Vertical);
        }

        static void DrawEndGameScreen()
        {
            Engine.DrawText("Juego Terminado", 300, 200, 255, 255, 255, _fontTitle);
            Engine.DrawText($"SCORE: {_score}", 450, 350, 255, 255, 255, _font);
            DrawTextWithOptions(" ", new string[] { "REINICIAR", "PANTALLA TITULO" }, _optionFinJuego, 300, 650, Orientation.Horizontal);
        }

        static void DrawCreditsScreen()
        {
            Engine.DrawText("CREDITOS", 350, 100, 255, 255, 255, _fontTitle);
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
            Engine.DrawText("VOLVER", 800, 600, 255, 255, 255, _font);
            _playerPosition = new Vector2(760, 610);
            Engine.Draw(_playerImageIcon, _playerPosition._x, _playerPosition._y);
        }

        static void DrawPauseScreen()
        {
            DrawGameScreen();
            Engine.DrawText("- PAUSA -", 400, 350, 255, 255, 255, _fontTitle);
        }

        static void DrawGameScreen()
        {
            Engine.Draw(_layer1, 0, 0);
            Engine.Draw(_layer2_1, 0 + _paralax2, 0);
            Engine.Draw(_layer2_2, 1024 + _paralax2, 0);
            Engine.Draw(_layer3_1, 0 + _paralax, 0);
            Engine.Draw(_layer3_2, 1024 + _paralax, 0);

            foreach (bullet bullet in _bullets)
            {
                Engine.Draw(bullet._bulletImage, bullet._position._x, bullet._position._y);
            }

            foreach (bullet bulletEnemy in _enemyBullets)
            {
                Engine.Draw(bulletEnemy._bulletImage , bulletEnemy._position._x, bulletEnemy._position._y);
            }

            

            if (_nivelActual != null)
            {
                var oleada = _nivelActual.getCurrentWave();
                if (oleada != null)
                {
                    Engine.DrawText($"enemies: { oleada._enemies.Count}", 900, 50, 255, 255, 255, _font);
                    foreach (var enemigo in oleada._enemies)
                    {
                        if (!enemigo._isDestroyed)
                        {
                            Engine.Draw(enemigo._imagen, enemigo._posicion._x, enemigo._posicion._y);
                        }
                    }
                }
            }
            Engine.Draw(_playerImage, _playerPosition._x, _playerPosition._y);
            Engine.DrawText($"SCORE: {_score}", 50, 50, 255, 255, 255, _font);

        }

        static void DrawTextWithOptions(string title, string[] options, int selectedOption, int startX, int startY, Orientation orientation)
        {
            Engine.DrawText(title, startX, startY, 255, 255, 255, _fontTitle);

            for (int i = 0; i < options.Length; i++)
            {
                int offsetX = orientation == Orientation.Horizontal ? i * 300 : 0;
                int offsetY = orientation == Orientation.Vertical ? i * 50 : 0;

                Engine.DrawText(options[i], startX + offsetX, startY + offsetY, 255, 255, 255, _font);

                if (i + 1 == selectedOption)
                {
                    int iconX = startX + offsetX -40;
                    int iconY = startY + offsetY + 10;
                    _playerPosition = new Vector2(iconX, iconY);
                }
            }

            Engine.Draw(_playerImageIcon, _playerPosition._x, _playerPosition._y);
        }

        static void DrawTextList(string[] texts, int startX, int startY)
        {
            for (int i = 0; i < texts.Length; i++)
            {
                Engine.DrawText(texts[i], startX, startY + i * 50, 255, 255, 255, _font);
            }
        }

        static int Clamp(int value, int min, int max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        static void ClampPlayerPosition()
        {
            _playerPosition._x = Clamp(_playerPosition._x, 0, _screenWidth - _imageWidth);
            _playerPosition._y = Clamp(_playerPosition._y, 0, _screenHeight - _imageHeight);
        }

        static void PlayerMovement()
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

            _playerPosition += movement * _speedMovement;
            ClampPlayerPosition();
        }

        static void PlayerShoot()
        {
            int offsetY = 30;
            bullet instanciteBullet = new bullet(_bulletImage,_playerPosition._x, _playerPosition._y + offsetY);
            _bullets.Add(instanciteBullet);
        }

        static void InGame()
        {
            ParalaxController();
            BulletController();
            if (_nivelActual != null)
            {
                EnemyMovementLogic(_nivelActual); 
            }
            CollitionController();
        }

        static void BulletController()
        {
            if (_bullets.Count > 0)
            {
                foreach (bullet bullet in _bullets)
                {
                    bullet.move();
                }
            }

            if (_enemyBullets.Count > 0)
            {
                foreach (bullet bullet in _enemyBullets)
                {
                    bullet.move();
                }
            }
        }

        static void ParalaxController()
        {
            _paralax--;
            if (_paralax < -1024) _paralax = 0;
            _paralax2 -= 0.5f;
            if (_paralax2 < -1024) _paralax2 = 0;
        }

        static void EnemyMovementLogic(Level nivelActual)
        {
            Wave oleada = nivelActual.getCurrentWave();
            if (oleada != null)
            {
                foreach (Enemy enemigo in oleada._enemies)
                {
                    if(enemigo._posicion._x <= -200)
                    {
                        enemigo._posicion._x = 1050;
                    }
                    if (!enemigo._isDestroyed)
                    {
                        switch (enemigo._patronMovimiento)
                        {
                            case "zigzag":
                                MoverEnZigzag(enemigo);
                                break;
                            case "lineal":
                                MoverLineal(enemigo);
                                break;
                            case "mina":
                                MoverMina(enemigo);
                                break;
                        }
                    }
                }

                if (oleada.CompleteWave())
                {
                    bool hayMasOleadas = nivelActual.NextWave();
                    if (hayMasOleadas)
                    {
                        Console.WriteLine("Pasando a la siguiente oleada...");
                    }
                    else
                    { 
                        _nivel++;
                        _nivelActual = CargarNivel(_nivel);  
                    }
                }
            }
            else
            {
                _currentState = GameState.FinJuego;
            }
        }

        static void MoverEnZigzag(Enemy enemigo)
        {
            float tiempo = Sdl.SDL_GetTicks() / 1000.0f; 
            enemigo._posicion._x += Vector2.Left._x * enemigo._speed; 
            enemigo._posicion._y += (int)(Math.Sin(tiempo * 2) * 5);
        }

        static void MoverLineal(Enemy enemigo)
        {
            enemigo._posicion._x += Vector2.Left._x * enemigo._speed; 
        }

        static void MoverMina(Enemy enemigo)
        {
            enemigo._posicion._x += Vector2.Left._x * enemigo._speed;
            if(enemigo._posicion._x < 512)
            {
                DestruirEnemigo(enemigo);
            }
        }

        static void RealizarDisparo(Enemy enemigo)
        {
            int bulletSpeed = 2;

            List<Vector2> direcciones = new List<Vector2>
            {
                Vector2.Up,      
                Vector2.Down,        
                Vector2.Left,        
                Vector2.Right,       
                Vector2.UpLeft,      
                Vector2.UpRight,     
                Vector2.DownLeft,   
                Vector2.DownRight   
            };

            foreach (var direccion in direcciones)
            {
                bullet nuevaBala = new bullet(
                    _bulletEnemyImage,
                    enemigo._posicion._x + 32,
                    enemigo._posicion._y + 32
                );

                nuevaBala._direction = direccion;
                nuevaBala._speed = bulletSpeed;
                _enemyBullets.Add(nuevaBala);
            }

            Console.WriteLine("Enemigo disparó en 8 direcciones.");
        }

        static void CollitionController()
        {
            List<bullet> bulletsToRemove = new List<bullet>();

            foreach (var bullet in _bullets)
            {
                foreach (Wave oleada in _nivelActual._waves)
                {
                    foreach (Enemy enemigo in oleada._enemies)
                    {
                        if (bullet._position._x > 1024)
                        {
                            bulletsToRemove.Add(bullet);
                        }
                        else if (!enemigo._isDestroyed && DetectCollisionBulletEnemy(bullet, enemigo))
                        {
                            _score += 10;
                            DestruirEnemigo(enemigo);
                            bulletsToRemove.Add(bullet);
                            break;
                        }
                    }
                }
            }

            foreach (Wave oleada in _nivelActual._waves)
            {
                foreach (Enemy enemigo in oleada._enemies)
                {
                    if (!enemigo._isDestroyed && DetectCollisionPlayerEnemy(_playerPosition, enemigo._posicion))
                    {
                        DestruirEnemigo(enemigo);
                        DestruirJugador();
                        _currentState = GameState.FinJuego; 
                        Console.WriteLine("El jugador ha sido destruido. Fin del juego.");
                        return; 
                    }
                }
            }

            foreach (bullet bullet in _enemyBullets)
            {
                if (bullet._position._x < 0 || bullet._position._x > 1024 ||
                    bullet._position._y < 0 || bullet._position._y > 768)
                {
                    bulletsToRemove.Add(bullet);
                }

                else if (DetectCollisionBulletEnemyPlayer(bullet, _playerPosition))
                {
                    Console.WriteLine("Jugador impactado por bala enemiga. Fin del juego.");
                    DestruirJugador();
                    bulletsToRemove.Add(bullet);
                    _currentState = GameState.FinJuego;
                    return;
                }
            }

            foreach (var bullet in bulletsToRemove)
            {
                _bullets.Remove(bullet);
            }

        }

        static bool DetectCollisionPlayerEnemy(Vector2 playerPos, Vector2 enemyPos)
        {
            return playerPos._x < enemyPos._x + 64 &&
                   playerPos._x + 64 > enemyPos._x &&
                   playerPos._y < enemyPos._y + 64 &&
                   playerPos._y + 64 > enemyPos._y;
        }

        static void DestruirJugador()
        {
            Console.WriteLine("Jugador destruido.");
            _playerPosition = new Vector2(200, 200);
        }

        static bool DetectCollisionBulletEnemy(bullet bullet, Enemy enemigo)
        {
            return bullet._position._x < enemigo._posicion._x + 64 &&
                   bullet._position._x + 32 > enemigo._posicion._x &&
                   bullet._position._y < enemigo._posicion._y + 64 &&
                   bullet._position._y + 32 > enemigo._posicion._y;
        }

        static void DestruirEnemigo(Enemy enemigo)
        {
            if (enemigo._patronAtaque == "explosion")
            {
                RealizarDisparo(enemigo);
            }
            enemigo._isDestroyed = true;
            Console.WriteLine($"Enemigo {enemigo._tipo} ha sido destruido.");
        }

        static bool DetectCollisionBulletEnemyPlayer(bullet bullet, Vector2 playerPos)
        {
            return bullet._position._x < playerPos._x + 64 &&
                   bullet._position._x + 6 > playerPos._x &&
                   bullet._position._y < playerPos._y + 64 &&
                   bullet._position._y + 6 > playerPos._y;
        }

    }

    public class bullet
    {
        public Image _bulletImage;
        public Vector2 _position;
        public Vector2 _direction = Vector2.Right;
        public int _speed = 7;

        public bullet(Image bulletImage, int intialPositionX, int intialPositionY)
        {
            _position = new Vector2(intialPositionX, intialPositionY);
            _bulletImage = bulletImage;
        }

        public void move()
        {
            _position += _direction * _speed;
        }
    }


    public class Vector2
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

        public static Vector2 UpLeft => new Vector2(-1, -1);
        public static Vector2 UpRight => new Vector2(1, -1);
        public static Vector2 DownLeft => new Vector2(-1, 1);
        public static Vector2 DownRight => new Vector2(1, 1);

        public static Vector2 operator +(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1._x + v2._x, v1._y + v2._y);
        }

        public static Vector2 operator *(Vector2 v, int scalar)
        {
            return new Vector2(v._x * scalar, v._y * scalar);
        }
    }

    public class Level
    {
        public int _level;
        public List<Wave> _waves;
        public int _currentWave; 

        public Level()
        {
            _waves = new List<Wave>();
            _currentWave = 0;
        }

        public Wave getCurrentWave()
        {
            if (_waves != null && _waves.Count > _currentWave)
            {
                return _waves[_currentWave];
            }
            return null;
        }

        public bool NextWave()
        {
            if (_currentWave < _waves.Count - 1)
            {
                _currentWave++;
                return true;
            }
            return false; 
        }
    }

    public class Wave
    {
        public List<Enemy> _enemies;

        public Wave()
        {
            _enemies = new List<Enemy>();
        }

        public bool CompleteWave()
        {
            return _enemies.TrueForAll(e => e._isDestroyed);
        }
    }

    public class Enemy
    {
        public string _tipo;
        public Image _imagen;
        public Vector2 _posicion; 
        public string _patronMovimiento;
        public string _patronAtaque;
        public bool _isDestroyed;
        public int _speed;

        public Enemy(Image image, string tipo, Vector2 posicion, int speed, string movimiento, string ataque)
        {
            _tipo = tipo;
            _imagen = image;
           _posicion = posicion;
            _speed = speed;
            _patronMovimiento = movimiento;
            _patronAtaque = ataque;
            _isDestroyed = false;
        }
    }


}
