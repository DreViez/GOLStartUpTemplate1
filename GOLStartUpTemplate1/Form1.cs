using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace GOLStartUpTemplate1
{
    public partial class Form1 : Form
    {
        // The universe array
        bool[,] universe = new bool[20, 20];
        bool[,] scratchPad = new bool[20, 20];
        
        // Drawing colors
        Color gridColor = Color.Black;
        Color cellColor = Color.Aquamarine;

        // The Timer class
        Timer timer = new Timer();

        // Generation count
        int generations = 0;
        int LivingCells = 0;

        bool isHUDVisible = true;
        bool isToroidal = true;
        //Forms
        SeedDialog ranseed = new SeedDialog();
        Options options= new Options();
       
        public Form1()
        {
            InitializeComponent();

            // Reading  property
            graphicsPanel1.BackColor = Properties.Settings.Default.PanelColor;
            // Setup the timer
            timer.Interval = 100; // milliseconds
            timer.Tick += Timer_Tick;
            timer.Enabled = true; // start timer running
            //CellsAlive.Text = "Living Cells: " + LivingCells.ToString();
        }

        // Calculate the next generation of cells
        private void NextGeneration()
        {

            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    int count = CountNeighborsFinite(x, y);
                    //Apply rules (will it live or die)
                    if (scratchPad[x, y])
                    {
                        if (count == 2 || count == 3)
                            scratchPad[x, y] = true;
                        if (count < 2 || count > 3)
                            scratchPad[x, y] = false;

                    }
                    else
                    {
                        if (count == 3)
                            scratchPad[x, y] = true;
                    }
                    //Turn in on/off in scratchPad

                }
            }
            // copy from scratchPad to universe

            //swaping
            bool[,] temp = universe;
            universe = scratchPad;
            scratchPad = temp;

            // Increment generation count
            generations++;

            // Update status strip generations
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
            graphicsPanel1.Invalidate();
        }

        // The event called by the timer every Interval milliseconds.
        private void Timer_Tick(object sender, EventArgs e)
        {
            NextGeneration();
            countlivingcells();
        }

        
        private void graphicsPanel1_Paint(object sender, PaintEventArgs e)
        {
            //Font used to draw and sets where number will be
            Font font = new Font("Arial", 10f);
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;


            // Calculate the width and height of each cell in pixels
            // CELL WIDTH = WINDOW WIDTH / NUMBER OF CELLS IN X
            int cellWidth = graphicsPanel1.ClientSize.Width / universe.GetLength(0);
            // CELL HEIGHT = WINDOW HEIGHT / NUMBER OF CELLS IN Y
            int cellHeight = graphicsPanel1.ClientSize.Height / universe.GetLength(1);

            // A Pen for drawing the grid lines (color, width)
            Pen gridPen = new Pen(gridColor, 1);

            // A Brush for filling living cells interiors (color)
            Brush cellBrush = new SolidBrush(cellColor);

            // Iterate through the universe in the y, top to bottom
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    // A rectangle to represent each cell in pixels
                    Rectangle cellRect = Rectangle.Empty;
                    cellRect.X = x * cellWidth;
                    cellRect.Y = y * cellHeight;
                    cellRect.Width = cellWidth;
                    cellRect.Height = cellHeight;

                    // Fill the cell with a brush if alive
                    if (universe[x, y] == true)
                    {
                        e.Graphics.FillRectangle(cellBrush, cellRect);
                    }
                    //Draws number inside cell
                    e.Graphics.DrawString(CountNeighborsFinite(x, y).ToString(), font, Brushes.Black, cellRect, stringFormat);
                    // Outline the cell with a pen
                    e.Graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);
                }
            }
            if (isHUDVisible)
            {
                //DRAW HUD
            }
            // Cleaning up pens and brushes
            gridPen.Dispose();
            cellBrush.Dispose();
        }

        private void graphicsPanel1_MouseClick(object sender, MouseEventArgs e)
        {
            // If the left mouse button was clicked
            if (e.Button == MouseButtons.Left)
            {
                // Calculate the width and height of each cell in pixels
                int cellWidth = graphicsPanel1.ClientSize.Width / universe.GetLength(0);
                int cellHeight = graphicsPanel1.ClientSize.Height / universe.GetLength(1);

                // Calculate the cell that was clicked in
                // CELL X = MOUSE X / CELL WIDTH
                int x = e.X / cellWidth;
                // CELL Y = MOUSE Y / CELL HEIGHT
                int y = e.Y / cellHeight;

                // Toggle the cell's state
                universe[x, y] = !universe[x, y];

                // Tell Windows you need to repaint
                countlivingcells();
                graphicsPanel1.Invalidate();
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            timer.Enabled = true;
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            NextGeneration();

        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    universe[x, y] = false;
                }
            }

            //make generation and living cells 0
            generations = 0;
            LivingCells = 0;
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
            toolStripStatusLabel1.Text = "Living Cells = " + LivingCells.ToString();
            graphicsPanel1.Invalidate();
        }
        private int CountNeighborsFinite(int x, int y)
        {
            int count = 0;
            int xLen = universe.GetLength(0);
            int yLen = universe.GetLength(1);
            for (int yOffset = -1; yOffset <= 1; yOffset++)
            {
                for (int xOffset = -1; xOffset <= 1; xOffset++)
                {
                    int xCheck = x + xOffset;
                    int yCheck = y + yOffset;
                    if (xOffset == 0 || yOffset == 0)
                    {
                        continue;
                    }
                    if (xCheck < 0)
                    {
                        continue;
                    }
                    if (yCheck < 0)
                    {
                        continue;
                    }
                    if (xCheck >= xLen)
                    {
                        continue;
                    }
                    if (yCheck >= yLen)
                    {
                        continue;
                    }

                    if (universe[xCheck, yCheck] == true) count++;
                }
            }
            return count;
        }
        private int CountNeighborsToroidal(int x, int y)
        {
            int sum = 0;

            for (int i = -1; i < 2; i++)
            {
                // Iterate through the universe in the x, left to right
                for (int j = -1; j < 2; j++)
                {
                    //let col = (x + i + cols) % cols;
                    //let row = (y + j + rows) % rows;
                    //sum += grid[col][row];

                    int col = (y + i + universe.GetLength(1)) % universe.GetLength(1);

                    int row = (x + j + universe.GetLength(0)) % universe.GetLength(0);

                    if (universe[row, col] == true)
                    {
                        sum++;
                    }
                }

            }

            if (universe[x, y])
            {
                sum--;
            }
            return sum;
        }
        private void colorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();

            dlg.Color = graphicsPanel1.BackColor;

            if (DialogResult.OK == dlg.ShowDialog())
            {
                graphicsPanel1.BackColor = dlg.Color;
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Update propety
            Properties.Settings.Default.PanelColor = graphicsPanel1.BackColor;

            Properties.Settings.Default.Save();
        }

        private void restToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reset();
            // Reading the property
            graphicsPanel1.BackColor = Properties.Settings.Default.PanelColor;
        }

        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reload();
            // Reading the property
            graphicsPanel1.BackColor = Properties.Settings.Default.PanelColor;
        }

        private void randomizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int cell;
            Random rand = new Random();
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {

                    cell = rand.Next(0, 2);

                    if (cell == 0)
                    {
                        universe[x, y] = false;
                    }
                    else
                    {
                        universe[x, y] = true;
                    }
                }
            }
            countlivingcells();
            graphicsPanel1.Invalidate();
        }
        private void fromSeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ranseed.ShowDialog() == DialogResult.OK)
            {
                int cell;
                Random rand = new Random();
                for (int y = 0; y < universe.GetLength(1); y++)
                {
                    for (int x = 0; x < universe.GetLength(0); x++)
                    {

                        cell = rand.Next(0, 2);

                        if (cell == 0)
                        {
                            universe[x, y] = false;
                        }
                        else
                        {
                            universe[x, y] = true;
                        }
                    }
                }
                countlivingcells();
                graphicsPanel1.Invalidate();
            }
        }
        private void countlivingcells()
        {
            LivingCells = 0;

            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {

                    if (universe[x, y] == true)
                    {
                        LivingCells++;
                    }

                }
            }

            // Update status of living cells
            toolStripStatusLabel1.Text = "Living Cells = " + LivingCells.ToString();
        }
            //Save the file
        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            int x = 0;
            int y = 0;


            //For Saving The Universe
            SaveFileDialog dlg = new SaveFileDialog();

            dlg.Filter = "All Files|*.*|Cells|*.cells";

            dlg.FilterIndex = 2; dlg.DefaultExt = "cells";


            if (DialogResult.OK == dlg.ShowDialog())
            {

                StreamWriter writer = new StreamWriter(dlg.FileName);

                writer.WriteLine("!Name: " + Path.GetFileNameWithoutExtension(dlg.FileName));
                writer.WriteLine("!");

                //Pass Thr all the Universe and check for cells' status

                for (y = 0; y < universe.GetLength(1); y++)
                {
                    // Create a string to represent the current row.
                    String currentRow = string.Empty;
                    if (y > 0)
                    {
                        writer.WriteLine();
                    }



                    for (x = 0; x < universe.GetLength(0); x++)
                    {



                        if (universe[x, y] == false)
                        {
                            writer.Write(".");
                        }

                        else
                        {
                            writer.Write("O");
                        }
                    }

                }

                



                writer.Close();

            }
            countlivingcells();
            graphicsPanel1.Invalidate();
        }

        private void newToolStripButton_Click(object sender, EventArgs e)
        {
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    universe[x, y] = false;
                }
            }
            graphicsPanel1.Invalidate();
        }

        private void openToolStripButton_Click(object sender, EventArgs e)
        {  

            string line = "";
            int maxHeight = 0;
            int maxWidth = 0;

            //For Saving The Universe
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.DefaultExt = "cells";

            dlg.Title = "Opening the Universe.";

            dlg.Filter = "cells files (*.cells)|*.cells|Text Files (*.txt)|*.txt|All files (*.*)|*.*";

            dlg.CheckPathExists = true;


            if (dlg.ShowDialog() == DialogResult.OK)
            {

                Stream s = new MemoryStream();
                StreamReader raeder = new StreamReader(dlg.FileName);


                while ((line = raeder.ReadLine()) != null)
                {
                    if (line[0] == '!')
                    {
                        continue;
                    }

                    else if (line[1] == '!')
                    {
                        continue;
                    }

                    maxWidth = line.Length;


                    maxHeight++;
                }


                raeder.Close();

                //Resize The Universe 

                universe = new bool[maxWidth, maxHeight];
                scratchPad = new bool[maxWidth, maxHeight];
               


                maxWidth--;
                maxHeight = 0;


                StreamReader reader = new StreamReader(dlg.FileName);


                while ((line = reader.ReadLine()) != null)
                {
                    if (line[0] == '!')
                    {
                        continue;
                    }


                    for (int x = 0; x < maxWidth; x++)
                    {

                        if (line[x] == '.')
                        {
                            universe[x, maxHeight] = false;
                        }

                        else if (line[x] == 'O')
                        {
                            universe[x, maxHeight] = true;
                        }

                    }

                    maxHeight++;



                }
                reader.Close();

            }
            graphicsPanel1.Invalidate();
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(options.ShowDialog() == DialogResult.OK)
            {
                //Updating the Interval
                timer.Interval = options.interval;
                //updating the Universe Size
                universe = new bool[options.X, options.Y];
                scratchPad = new bool[options.X, options.Y];
            }
            graphicsPanel1.Invalidate();
        }
    }
}
