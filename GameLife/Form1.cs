using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameLife
{
    public partial class Form1 : Form
    {
        const int mapSize = 30;
        const int cellSize = 30;

        int[,] currentState = new int[mapSize,mapSize];
        int[,] nextState = new int[mapSize,mapSize];

        Button[,] cells = new Button[mapSize,mapSize];

        bool isPlaying = false;

        Timer mainTimer;

        int offset = 25;

        public Form1()
        {
            InitializeComponent();
            SetFormSize();
            BuildMenu();
            Init();
        }

        void SetFormSize()
        {
            this.Width = (mapSize + 1) * cellSize;
            this.Height = (mapSize + 1) * cellSize +40;
        }

        public void Init()
        {
            isPlaying = false;
            mainTimer = new Timer();
            mainTimer.Interval = 100;
            mainTimer.Tick += new EventHandler(UpdateStates);
            currentState = InitMap();
            nextState = InitMap();
            InitCells();
        }

        void ClearGame()
        {
            isPlaying = false;
            mainTimer = new Timer();
            mainTimer.Interval = 100;
            mainTimer.Tick += new EventHandler(UpdateStates);
            currentState = InitMap();
            nextState = InitMap();
            ResetCells();
        }

        void ResetCells()
        {
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    cells[i, j].BackColor = Color.White;
                }
            }
        }

        void BuildMenu()
        {
            var menu = new MenuStrip();

            var restart = new ToolStripMenuItem("Начать заного");
            restart.Click += new EventHandler(Restart);

            var play = new ToolStripMenuItem("Начать симуляцию");
            play.Click += new EventHandler(Play);

            menu.Items.Add(play);
            menu.Items.Add(restart);

            this.Controls.Add(menu);
        }

        private void Restart(object sender, EventArgs e)
        {
            mainTimer.Stop();
            ClearGame();
        }

        private void Play(object sender, EventArgs e)
        {
            if (!isPlaying)
            {
                isPlaying = true;
                mainTimer.Start();
            }
        }

        private void UpdateStates(object sender, EventArgs e)
        {            
            CalculateNextState();
            DisplayMap();
            if (CheckGenerationDead())
            {
                mainTimer.Stop();
                MessageBox.Show("Поколение себя изжило :(");
            }
        }

        bool CheckGenerationDead()
        {
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    if (currentState[i, j] == 1)
                        return false;
                }
            }
            return true;
        }

        void CalculateNextState()
        {
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    var countNeighboors = CountNeighboors(i, j);
                    if (currentState[i,j] == 0 && countNeighboors == 3)
                    {
                        nextState[i, j] = 1;
                    }else if (currentState[i, j] == 1 && (countNeighboors<2 &&countNeighboors>3))
                    {
                        nextState[i, j] = 0;
                    }
                    else if (currentState[i, j] == 1 && (countNeighboors >= 2 && countNeighboors <= 3))
                    {
                        nextState[i, j] = 1;
                    }
                    else
                    {
                        nextState[i, j] = 0;
                    }
                }
            }
            currentState = nextState;
            nextState = InitMap();
        }

        void DisplayMap()
        {
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    if (currentState[i, j] == 1)
                        cells[i, j].BackColor = Color.Black;
                    else cells[i, j].BackColor = Color.White;
                }
            }
        }

        int CountNeighboors(int i,int j)
        {
            var count = 0;
            for(int k = i - 1; k <= i + 1; k++)
            {
                for (int l = j - 1; l <= j + 1; l++)
                {
                    if (!IsInsideMap(k, l))
                        continue;
                    if (k == i && l==j)
                        continue;
                    if (currentState[k, l] == 1)
                        count++;
                }
            }
            return count;
        }

        bool IsInsideMap(int i,int j)
        {
            if(i<0 || i>=mapSize || j<0 || j >= mapSize)
            {
                return false;
            }
            return true;
        }

        int[,] InitMap()
        {
            int[,] arr = new int[mapSize, mapSize];
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    arr[i, j] = 0;
                }
            }
            return arr;
        }

        void InitCells()
        {
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    Button button = new Button();
                    button.Size = new Size(cellSize, cellSize);
                    button.BackColor = Color.White;
                    button.Location = new Point(j * cellSize, (i * cellSize) + offset);
                    button.Click += new EventHandler(OnCellClick);
                    this.Controls.Add(button);
                    cells[i, j] = button;
                }
            }
        }

        private void OnCellClick(object sender, EventArgs e)
        {
            var pressedButton = sender as Button;
            if (!isPlaying)
            {
                var i = (pressedButton.Location.Y - offset) / cellSize ;
                var j = pressedButton.Location.X / cellSize;

                if(currentState[i,j] == 0)
                {
                    currentState[i, j] = 1;
                    cells[i, j].BackColor = Color.Black;
                }else
                {
                    currentState[i, j] = 0;
                    cells[i, j].BackColor = Color.White;
                }
            }
        }

    }
}
