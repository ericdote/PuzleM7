using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace puzzle
{
    public partial class Form1 : Form
    {
        private int x = 0;
        private int y = 0;
        private int x2, y2;
        private int PosIniciaTop = 0;
        private int PosIniciaLeft = 0;
        private Image[] img; //Array de Imagenes para el puzzle
        private PictureBox[] pb; //Array de PictureBox donde se añaden las imagenes desordenadas
        private PictureBox imgSelect; //PictureBox de la imagen seleccionada ene se momento
        private Boolean[] imgRepe; //Array de booleanos que controla las posiciones que ya se han añadido del puzzle
        int cambioPosicionTop, cambioPosicionLeft, tagAux; //Variables auxiliares
        private Image aux; //Variable para imagen auxiliar


        public Form1()
        {
            InitializeComponent();
            //Inicializamos las imagenes
            img = new Image[9] { Properties.Resources._1, Properties.Resources._2, Properties.Resources._3, Properties.Resources._4, Properties.Resources._5, Properties.Resources._6, Properties.Resources._7, Properties.Resources._8, Properties.Resources._9 };
            //Inicializamos el array de imagenes para el puzle
            pb = new PictureBox[9];
            //Inicializamos el array de boleanos
            imgRepe = new Boolean[9];
            //Asignamos un tag a cada pieza del puzle, para mas adelante controlar si esta completo.
            for (int i = 0; i < img.Length; i++)
            {
                img[i].Tag = i;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }


        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            imgSelect = (PictureBox)sender;
            x = e.X;
            y = e.Y;

            PosIniciaTop = imgSelect.Top;
            PosIniciaLeft = imgSelect.Left;

            x2 = imgSelect.Top; //Le asignamos la nueva posicion a variables nuevas, ya que si van en las antiguas tenemos problemas de desplazamiento
            y2 = imgSelect.Left;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            imgSelect = (PictureBox)sender;
            imgSelect.BringToFront();
            label1.Text = Convert.ToString(x) + "-" + Convert.ToString(y);
            label2.Text = Convert.ToString(e.X) + "-" + Convert.ToString(e.Y);
            label3.Text = Convert.ToString(PosIniciaTop) + "-" + Convert.ToString(PosIniciaLeft);
            label4.Text = Convert.ToString(imgSelect.Top) + "-" + Convert.ToString(imgSelect.Left);

            cambioPosicionTop = imgSelect.Top; //Se actualiza la posicion por donde desplazamos la imagen verticalmente
            cambioPosicionLeft = imgSelect.Left; //Y horizontalmente


            if (e.Button == MouseButtons.Left)
            {
                imgSelect.Left = (imgSelect.Left + e.X) - x; 
                imgSelect.Top = (imgSelect.Top + e.Y) - y;
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            imgSelect = (PictureBox)sender;
            label1.Text = Convert.ToString(x) + "-" + Convert.ToString(x);
            label2.Text = Convert.ToString(e.X) + "-" + Convert.ToString(e.X);
            label3.Text = Convert.ToString(PosIniciaTop) + "-" + Convert.ToString(PosIniciaLeft);

            imgSelect.Left = PosIniciaLeft;
            imgSelect.Top = PosIniciaTop;
            Point pointImg1 = new Point(x2 / 100, y2 / 100);
            Point pointImg2 = new Point(cambioPosicionTop / 100, cambioPosicionLeft / 100);
            foreach (var i in pb)
            {                
                if(((i.Location.X / 100) == pointImg2.Y)&&((i.Location.Y /100) == pointImg2.X)) //Comprovamos la posicion donde esta la imagen que arrastramos
                {
                    foreach (var j in pb) 
                    {
                        if(((j.Location.X / 100) == pointImg1.Y) && (j.Location.Y /100) == pointImg1.X)
                        { //Comprovamos la posicion donde vamos a cambiar
                            aux = j.Image; //Cogemos la imagen secundaria y la ponemos en el auxiliar
                            j.Image = i.Image; //Cambiamos la secundaria por la que estamos arrastrando
                            i.Image = aux; //Cambiamos la posicion de donde hemos arrastrado la imagen arrastrada por la imagen que habia donde esta la nueva (osea que ponemos la auxiliar aqui)

                            //Lo mismo con el TAG
                            tagAux = (int)j.Tag;
                            j.Tag = i.Tag;
                            i.Tag = tagAux;
                        }
                    }
                    break;
                }
            }
            comprovacionGanar();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            //Con estas variables controlamos cuantas imagenes hay por columna y fila
            int imagenesDentroX = 0, imagenesDentroY = 0;
            //Rellenamos el array de PicturesBox con piezas de puzzle
            for (int i = 0; i < 9; i++)
            {
                //Declaramos un random para seleccionar piezas del puzzle
                Random rnd = new Random();
                int imgRnd = rnd.Next(0, 9);
                //Si la imagen no ha sido escogida antes(lo comprovamos con el array de boleanos)
                if (!imgRepe[imgRnd])
                {
                    //Inicializamos la casilla con un nuevo PictureBox
                    pb[i] = new PictureBox();
                    imgRepe[imgRnd] = true; //Asignamos la posicion en el array de booleanos a true
                    pb[i].Image = img[imgRnd]; //Asignamos la imagen
                    pb[i].Height = 100; //Le damos un tamaño
                    pb[i].Width = 100;
                    pb[i].SizeMode = PictureBoxSizeMode.StretchImage; //Le ajustamos su modo (en este caso que se ajuste a las proporciones)
                    //Si la imagen supera los 300 px 
                    if (imagenesDentroX >= 300)
                    {
                        //La X se reinicia y se baja una fila
                        imagenesDentroX = 0;
                        imagenesDentroY += 100;
                    }
                    //Si la Y se supera se reinicia la X a la posicion 0
                    if (imagenesDentroY >= 300)
                    {
                        imagenesDentroX = 0;
                    }
                    pb[i].Location = new Point(imagenesDentroX, imagenesDentroY); //Asignamos un puntero a la imagen para poder después cogerlo
                    imagenesDentroX += 100; //Asignamos +100 tras cada imagen añadida
                    this.panel1.Controls.Add(pb[i]); //Le añadimos a la imagen funciones
                    pb[i].MouseMove += pictureBox1_MouseMove;
                    pb[i].MouseUp += pictureBox1_MouseUp;
                    pb[i].MouseDown += pictureBox1_MouseDown;
                    pb[i].Tag = img[imgRnd].Tag; //Le asignamos un TAG a la imagen

                }
                else
                {
                    i--; //Con tal de no dejarnos ningun hueco vacio en el puzzle, restamos uno a i, para asi que hasta que el puzzle no este completo no deje de incluir imagenes, ya que si se repite pararia antes de estar lleno entero.
                }

            }
            btnStart.Enabled = false; //Desactivamos el boton de iniciar partida para que no pete al intentar inicar varias veces seguidas.
        }
        private void comprovacionGanar()
        {
            //Boolean iniciado a falso para comprovar si hemos ganado
            Boolean comprovacio = false;
            //Recorre el array de Picturebox
            for (int i = 0; i < pb.Length; i++)
            {
                int siguiente = i + 1;
                if((int)pb[i].Tag+1 != siguiente)
                {
                    comprovacio = false;
                    break;
                } else
                {
                    comprovacio = true;
                }
            }
            //Si todas las imagenes se han colocado correctamente salta el siguiente mensaje, si no, no sale.
            if (comprovacio)
                if (comprovacio)
            {
                MessageBox.Show("HAS GANADO!");
            }
        }
    }
}
