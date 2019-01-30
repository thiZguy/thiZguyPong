using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseMove);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseClick);
            this.KeyPress += new KeyPressEventHandler(this.Form1_keyPress);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Form1_keyUp);
        }
        Bitmap btm;
        Graphics g;
        Graphics SCG;
        Thread th;
        Rectangle ball = Rectangle.Empty;
        Rectangle leftSD = Rectangle.Empty;
        Rectangle rightSD = Rectangle.Empty;

        int ball_speed = 5;
        int ball_speedY = 5;
        int move_speed = 5;

        int player1Score = 0;
        int player2Score = 0;


        Point moveTo = Point.Empty;
        Point ballMove = Point.Empty;
        bool player1keyPress = false;
        bool player2keyPress = false;
        char player1keyPressValue;
        char player2keyPressvalue;
        bool drawing = true;
        bool paused = true;
        bool gameStarted = false;

        private void Form1_Load(object sender, EventArgs e)
        {
            btm = new Bitmap(this.Width, this.Height);
            g = Graphics.FromImage(btm);
            SCG = this.CreateGraphics();
            ball = new Rectangle(this.Width/2, this.Height/2,40,40);
            rightSD = new Rectangle(this.Width - 50, this.Height / 2, 30, 150);
            leftSD = new Rectangle(5, this.Height / 2, 30, 150);
            this.Text = "Pong Game";
            th = new Thread(draw);
            th.IsBackground = true;
            th.Start();
        }

        public void draw() {
            try
            {
                while (drawing)
                {
                    if (gameStarted) {
                        if (!paused)
                        {
                            g.Clear(Color.Black);

                            ball.X += ball_speed;

                            //jugadorIzquierdoLabel.Click = new MouseEventArgs();
                            if (ballMove.Y > ball.Y && (ball.Y <= this.Height || ball.Y >= 0))
                                ball.Y += ball_speedY;
                            if (ballMove.Y < ball.Y && (ball.Y <= this.Height || ball.Y >= 0))
                                ball.Y -= ball_speedY;

                            //cuando colisiona con las barras (jugadores)
                            Random rnd = new Random();
                            if ((leftSD.IntersectsWith(ball) || rightSD.IntersectsWith(ball)))
                            {
                                Console.WriteLine("cambiando direccion!");
                                ball_speed *= -1;
                                ball_speedY += (1 * rnd.Next(0, 5));
                            }
                            //colisiona con los bordes superior e inferior
                            //borde inferior
                            if (ball.Y + ball.Height >= (this.Height - (ball.Width * 2)))
                            {
                                Console.WriteLine("colisiona con el borde inferior");
                                ball_speedY *= -1;
                                //borde superior   
                            }
                            else if (ball.Y <= 0)
                            {
                                Console.WriteLine("colisiona con el borde superior");
                                ball.Y += 3;
                                ball_speedY *= ball_speedY <= 0 ? -1 : 1;
                                ballMove.Y += this.Height - rnd.Next(0, 20);
                            }

                            //control del jugador de la barra derecha
                            if (player2keyPress)
                            {

                                if ((leftSD.Y + leftSD.Height) < this.Height - 50 && player2keyPressvalue == 's')
                                {
                                    leftSD.Y += move_speed;
                                }

                                else if (leftSD.Y > 0 && player2keyPressvalue == 'w')
                                    leftSD.Y -= move_speed;
                            }
                            if (player1keyPress)
                            {
                                if ((rightSD.Y + rightSD.Height) < this.Height - 50 && player1keyPressValue == 'k')
                                    rightSD.Y += move_speed;
                                else if (rightSD.Y > 0 && player1keyPressValue == 'i')
                                    rightSD.Y -= move_speed;
                            }
                            //condicionales para un nuevo juego
                            //anota jugador 1     
                            if (ball.X <= 0)
                            {
                                player2Score++;
                                ThreaProcSafe();
                                restartGame();
                            }
                            //anota jugador 2
                            if((ball.X >= this.Width - (ball.Width * 2)))
                            {
                                player1Score++;
                                ThreaProcSafe();
                                restartGame();
                            }


                            setPlayerColor();
                            g.FillEllipse(Brushes.White, ball);

                            SCG.DrawImage(btm, 0, 0, this.Width, this.Height);
                        }        
                    }
                }
            }
            catch(Exception e) {
                Console.WriteLine("Exception \n "+e.Message);
            }
        }

        private void setPlayerColor()
        {   
            if(player1Score == player2Score){
                if (player1Score == 0 && player2Score == 0)
                {
                    g.FillRectangle(Brushes.White, rightSD);
                    g.FillRectangle(Brushes.White, leftSD);
                } 
                else {
                    g.FillRectangle(Brushes.Blue, rightSD);
                    g.FillRectangle(Brushes.Blue, leftSD);
                }
            }else if(player1Score>player2Score){
                g.FillRectangle(Brushes.Green, leftSD);
                if (player1Score == 4)
                {
                    g.FillRectangle(Brushes.Red, rightSD);
                }else {
                    g.FillRectangle(Brushes.Orange, rightSD);
                }
            }else if (player2Score > player1Score)
            {
                g.FillRectangle(Brushes.Green, rightSD);
                if (player2Score == 4)
                {
                    g.FillRectangle(Brushes.Red, leftSD);
                }
                else
                {
                    g.FillRectangle(Brushes.Orange, leftSD);
                }
            }
        }
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            Console.WriteLine("On mouse move");
            moveTo = e.Location;
        }
        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            Console.WriteLine("new game");
        }

        private void Form1_keyPress(object sender, KeyPressEventArgs e)
        {   //73 75, 87 y 83
            //(De arriba hacia abajo)
            Console.WriteLine("la tecla presionada fue: "+e.KeyChar);
           //moviendo el jugador de la derecha
            if (e.KeyChar == 'k' || e.KeyChar == 'i')
            {
                player1keyPress = true;
                player1keyPressValue = e.KeyChar;
            }
            if (e.KeyChar == 's'||e.KeyChar=='w')
            {
                player2keyPress = true;
                player2keyPressvalue = e.KeyChar;
            }
        }
        private void Form1_keyUp(object sender, KeyEventArgs e) 
        {
            Console.WriteLine("La tecla soltada fue: "+e.KeyValue);
            if (e.KeyValue == 75 || e.KeyValue == 73)
            {
                player1keyPress = false;
            }
            if (e.KeyValue == 87 || e.KeyValue == 83)
            {
                player2keyPress = false;
            }
            if (e.KeyValue == 32) {
                paused = !paused;
            }
            if (e.KeyValue == 13)
            {
                gameStarted = !gameStarted;
                if(gameStarted){
                    startGame();
                }
            }
        }

        private void restartGame()
        {
            ball.X = (this.Width / 2)- (ball.Width/2);
            ball_speedY = 0;
            ball.Y = (this.Height / 2) - (ball.Height / 2);
            rightSD.Location = new Point(this.Width - 50, (this.Height / 2) - (rightSD.Height/2));
            leftSD.Location = new Point(5, (this.Height / 2) - (leftSD.Height / 2));
            paused = true;
            if (player1Score == 5 || player2Score == 5)
            {
                player2Score = 0;
                player1Score = 0;
            }
            ThreaProcSafe();
        }

        private void startGame()
        {
            paused = !paused;
            this.label1.Hide();
            this.label2.Hide();
            this.label3.Hide();
            this.label3.Hide();
            this.label4.Hide();
            this.label5.Hide();
            this.label6.Hide();
            this.label7.Hide();
            this.label8.Hide();
            this.label9.Hide();
            this.label10.Hide();
            this.label11.Hide();
        }

        private void ThreaProcSafe() {
            ThreadHelperCallback.SetText(this,label13,player1Score.ToString());
            ThreadHelperCallback.SetText(this, label14, player2Score.ToString());
        }

    }

    public static class ThreadHelperCallback {
        delegate void SetTextCallback(Form f, Control ctrl, string text);
        public static void SetText(Form form, Control ctrl,string text) {
            if (ctrl.InvokeRequired) {
                SetTextCallback d = new SetTextCallback(SetText);
                form.Invoke(d, new Object[] { form, ctrl, text });
            }
            else {
                ctrl.Text = text;
            }
        }
    }

}
