using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class Snake : MonoBehaviour
{
    private enum Direction
    {
        Left,
        Right,
        Down,
        Up
    }

    private enum State
    {
        Alive,
        Dead
    }

   

    
    private class SnakeBodyPart
    {
        private SnakeMovePosition snakeMovePosition; // Posición 2D de la SnakeBodyPart
        private Transform transform;

        public SnakeBodyPart(int bodyIndex)
        {
            GameObject snakeBodyPartGameObject = new GameObject("Snake Body",
                typeof(SpriteRenderer));
            SpriteRenderer snakeBodyPartSpriteRenderer = snakeBodyPartGameObject.GetComponent<SpriteRenderer>();
            snakeBodyPartSpriteRenderer.sprite =
                GameAssets.Instance.snakeBodySprite;
            snakeBodyPartSpriteRenderer.sortingOrder = -bodyIndex;
            transform = snakeBodyPartGameObject.transform;
        }

        public void SetMovePosition(SnakeMovePosition snakeMovePosition)
        {
            // Posición (gridPosition)
            this.snakeMovePosition = snakeMovePosition; // Posición 2D y la dirección de la SnakeBodyPart
            Vector2Int gridPosition = snakeMovePosition.GetGridPosition();
            transform.position = new Vector3(gridPosition.x,
                gridPosition.y, 0); // Posición 3D del G.O.

            // Dirección (direction)
            float angle;
            switch (snakeMovePosition.GetDirection())
            {
                default:
                case Direction.Left: // Currently Going Left
                    switch (snakeMovePosition.GetPreviousDirection())
                    {
                        default: // Previously Going Left
                            angle = 90;
                            break;
                        case Direction.Down: // Previously Going Down
                            angle = -45;
                            break;
                        case Direction.Up: // Previously Going Up
                            angle = 45;
                            break;
                    }
                    break;
                case Direction.Right: // Currently Going Right
                    switch (snakeMovePosition.GetPreviousDirection())
                    {
                        default: // Previously Going Right
                            angle = -90;
                            break;
                        case Direction.Down: // Previously Going Down
                            angle = 45;
                            break;
                        case Direction.Up: // Previously Going Up
                            angle = -45;
                            break;
                    }
                    break;
                case Direction.Up: // Currently Going Up
                    switch (snakeMovePosition.GetPreviousDirection())
                    {
                        default: // Previously Going Up
                            angle = 0;
                            break;
                        case Direction.Left: // Previously Going Left
                            angle = 45;
                            break;
                        case Direction.Right: // Previously Going Right
                            angle = -45;
                            break;
                    }
                    break;
                case Direction.Down: // Currently Going Down
                    switch (snakeMovePosition.GetPreviousDirection())
                    {
                        default: // Previously Going Down
                            angle = 180;
                            break;
                        case Direction.Left: // Previously Going Left
                            angle = -45;
                            break;
                        case Direction.Right: // Previously Going Right
                            angle = 45;
                            break;
                    }
                    break;
            }

            transform.eulerAngles = new Vector3(0, 0, angle);
        }
    }

    private class SnakeMovePosition
    {
        private SnakeMovePosition previousSnakeMovePosition;
        private Vector2Int gridPosition;
        private Direction direction;

        public SnakeMovePosition(SnakeMovePosition previousSnakeMovePosition, Vector2Int gridPosition, Direction direction)
        {
            this.previousSnakeMovePosition = previousSnakeMovePosition;
            this.gridPosition = gridPosition;
            this.direction = direction;
        }

        public Vector2Int GetGridPosition()
        {
            return gridPosition;
        }

        public Direction GetDirection()
        {
            return direction;
        }

        public Direction GetPreviousDirection()
        {
            if (previousSnakeMovePosition == null)
            {
                return Direction.Right;
            }
            return previousSnakeMovePosition.GetDirection();
        }

    }

    #region VARIABLES
    private Vector2Int gridPosition; // Posición 2D de la cabeza
    private Vector2Int startGridPosition;
    private Direction gridMoveDirection; // Dirección de la cabeza

    private float horizontalInput, verticalInput;

    private float gridMoveTimer;
    private float gridMoveTimerMax;
    public float[] speedGrid = { 0.5f, 0.3f, 0.1f};
    int index;

    public delegate void OnSpeedChangeDelegate (float speed);
    public static event OnSpeedChangeDelegate OnSpeedChange;

    private LevelGrid levelGrid;

    private int snakeBodySize; // Partes del cuerpo.
    private List<SnakeMovePosition> snakeMovePositionsList; // Posiciones y direcciones.
    private List<SnakeBodyPart> snakeBodyPartsList;

    private State state;

    #endregion

    private void Awake()
    {
        startGridPosition = new Vector2Int(0, 0);
        gridPosition = startGridPosition;

        gridMoveDirection = Direction.Up; // Dirección arriba 
        transform.eulerAngles = Vector3.zero; // Rotación arriba 

        snakeBodySize = 0;
        snakeMovePositionsList = new List<SnakeMovePosition>();
        snakeBodyPartsList = new List<SnakeBodyPart>();

        state = State.Alive;

        index = -1; //asi en la pantalla se muestra la velocidad con la que empezamos (y forzamos que index = 0, ya que en la función sumamos 1) 

        SnakeVelocity();
        
    }

    private void Update()
    {
        switch (state)
        {
            case State.Alive:
                HandleMoveDirection();
                HandleGridMovement();
                break;
            case State.Dead:
                break;
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            SnakeVelocity();
            Debug.Log(gridMoveTimerMax);
        }

     
       
        
    }

    public void Setup(LevelGrid levelGrid)
    {
       
        this.levelGrid = levelGrid;
    }

    private void HandleGridMovement() // Relativo al movimiento en 2D
    {
        gridMoveTimer += Time.deltaTime;
        if (gridMoveTimer >= gridMoveTimerMax)
        {
            gridMoveTimer -= gridMoveTimerMax; // Se reinicia el temporizador

            SnakeMovePosition previousSnakeMovePosition = null;
            if (snakeMovePositionsList.Count > 0)
            {
                previousSnakeMovePosition = snakeMovePositionsList[0];
            }

            SnakeMovePosition snakeMovePosition = new SnakeMovePosition(previousSnakeMovePosition, gridPosition, gridMoveDirection);
            snakeMovePositionsList.Insert(0, snakeMovePosition);

            // Relación entre enum Direction y vectores left, right, down y up
            Vector2Int gridMoveDirectionVector;
            switch (gridMoveDirection)
            {
                default:
                case Direction.Left:
                    gridMoveDirectionVector = new Vector2Int(-1, 0);
                    break;
                case Direction.Right:
                    gridMoveDirectionVector = new Vector2Int(1, 0);
                    break;
                case Direction.Down:
                    gridMoveDirectionVector = new Vector2Int(0, -1);
                    break;
                case Direction.Up:
                    gridMoveDirectionVector = new Vector2Int(0, 1);
                    break;
            }

            gridPosition += gridMoveDirectionVector; // Mueve la posición 2D de la cabeza de la serpiente
            gridPosition = levelGrid.ValidateGridPosition(gridPosition);

            // ¿He comido comida?
            bool snakeAteFood = levelGrid.TrySnakeEatFood(gridPosition);
            if (snakeAteFood)
            {
                // El cuerpo crece
                snakeBodySize++;
                CreateBodyPart();
            }

            if (snakeMovePositionsList.Count > snakeBodySize)
            {
                snakeMovePositionsList.
                    RemoveAt(snakeMovePositionsList.Count - 1);
            }

            // Comprobamos el Game Over aquí porque tenemos la posición de la cabeza y la lista snakeMovePositionsList actualizadas para poder comprobar la muerte
            foreach (SnakeMovePosition movePosition in snakeMovePositionsList)
            {
                if (gridPosition == movePosition.GetGridPosition()) // Posición de la cabeza coincide con alguna parte del cuerpo
                {
                    // GAME OVER
                    state = State.Dead;
                    GameManager.Instance.SnakeDied();
                 
                }
            }

            transform.position = new Vector3(gridPosition.x, gridPosition.y, 0);
            transform.eulerAngles = new Vector3(0, 0, GetAngleFromVector(gridMoveDirectionVector));
            UpdateBodyParts();
        }
    }

    private void HandleMoveDirection()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // Cambio dirección hacia arriba
        if (verticalInput > 0) // Si he pulsado hacia arriba (W o Flecha Arriba)
        {
            if (gridMoveDirection != Direction.Down) // Si iba en horizontal
            {
                // Cambio la dirección hacia arriba (0,1)
                gridMoveDirection = Direction.Up;
            }
        }

        // Cambio dirección hacia abajo
        // Input es abajo?
        if (verticalInput < 0)
        {
            // Mi dirección hasta ahora era horizontal
            if (gridMoveDirection != Direction.Up)
            {
                gridMoveDirection = Direction.Down;
            }
        }

        // Cambio dirección hacia derecha
        if (horizontalInput > 0)
        {
            if (gridMoveDirection != Direction.Left)
            {
                gridMoveDirection = Direction.Right;
            }
        }

        // Cambio dirección hacia izquierda
        if (horizontalInput < 0)
        {
            if (gridMoveDirection != Direction.Right)
            {
                gridMoveDirection = Direction.Left;
            }
        }
    }

    private float GetAngleFromVector(Vector2Int direction)
    {
        float degrees = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if (degrees < 0)
        {
            degrees += 360;
        }

        return degrees - 90;
    }

    public Vector2Int GetGridPosition()
    {
        return gridPosition;
    }

    public List<Vector2Int> GetFullSnakeBodyGridPosition()
    {
        List<Vector2Int> gridPositionList = new List<Vector2Int>() { gridPosition };
        foreach (SnakeMovePosition snakeMovePosition in snakeMovePositionsList)
        {
            gridPositionList.Add(snakeMovePosition.GetGridPosition());
        }
        return gridPositionList;
    }

    private void CreateBodyPart()
    {
        snakeBodyPartsList.Add(new SnakeBodyPart(snakeBodySize));
    }

    private void UpdateBodyParts()
    {
        for (int i = 0; i < snakeBodyPartsList.Count; i++)
        {
            snakeBodyPartsList[i].SetMovePosition(snakeMovePositionsList[i]);
        }

       
    }
    

   private void SnakeVelocity()
    {
        index += 1;

        if(index >= speedGrid.Length) { index = 0; }

        gridMoveTimerMax = speedGrid[index];

        if (OnSpeedChange != null)
        {
            OnSpeedChange(gridMoveTimerMax);
        }




    }
  
   

}




