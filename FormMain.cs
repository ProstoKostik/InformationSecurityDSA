using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Numerics;

namespace DSA
{
    public partial class FormMain : Form
    {
        /// <summary>
        /// Параметр q
        /// </summary>
        private BigInteger _paramQ;
        /// <summary>
        /// Параметр p
        /// </summary>
        private BigInteger _paramP;
        /// <summary>
        /// Результат частного (p-1)/q
        /// </summary>
        private BigInteger _paramT;
        /// <summary>
        /// Заданный пользователем параметр q
        /// </summary>
        private BigInteger _tmpQ;
        /// <summary>
        /// Заданный пользователем параметр p
        /// </summary>
        private BigInteger _tmpP;
        /// <summary>
        /// Цифровая подпись
        /// </summary>
        private DigitalSignatureAlgorithm _DSA;
        /// <summary>
        /// Инициализация компонентов формы
        /// </summary>
        public FormMain()
        {
            InitializeComponent();
            _tmpP = BigInteger.Zero;
            _tmpQ = BigInteger.Zero;
            _paramQ = new BigInteger(142433);
            _paramT = new BigInteger(6);
            _paramP = _paramQ * _paramT + BigInteger.One;        
        }
        /// <summary>
        /// Выбор пользовательских настроек
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserInput_CheckedChanged(object sender, EventArgs e)
        {
            if (userInput.Checked)
            {
                userParams.Enabled = true;
            }
            else
            {
                userParams.Enabled = false;
            }
        }
        /// <summary>
        /// Изменение текста
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Text_TextChanged(object sender, EventArgs e)
        {
            if (text.Text.Length != 0)
            {
                Generate.Enabled = true;
                if (gen.Text.Length != 0)
                {
                    Check.Enabled = true;
                }
            }
            else
            {
                Generate.Enabled = false;
                Check.Enabled = false;
            }
        }
        /// <summary>
        /// Создание электронной подписи
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Generate_Click(object sender, EventArgs e)
        {
            if (_paramP <= _paramQ)
            {
                message.Text = "Неверно заданы параметры";
                message.ForeColor = System.Drawing.Color.DeepPink;
                message.Visible = true;
            }
            else
            {
                P.Text = _paramP.ToString();
                Q.Text = _paramQ.ToString();
                _DSA = new DigitalSignatureAlgorithm(_paramQ, _paramP);
                StringBuilder SB = _DSA.GenerateDigitalSignature(text.Text);
                gen.Clear();
                gen.AppendText(SB.ToString());
                Check.Enabled = true;
            }
        }
        /// <summary>
        /// Изменение параметра q
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Q_TextChanged(object sender, EventArgs e)
        {
            message.Visible = false;
            if (Q.Text.Length != 0)
            {
                if (!BigInteger.TryParse(Q.Text, out _tmpQ))
                {
                    wrongQ.Visible = true;
                }
                else
                {
                    _paramQ = _tmpQ;
                    wrongQ.Visible = false;
                }

            }
            else
            {
                _paramQ = int.MaxValue;
                wrongQ.Visible = false;
            }
        }
        /// <summary>
        /// Изменение параметра p
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void P_TextChanged(object sender, EventArgs e)
        {
            message.Visible = false;
            if (P.Text.Length != 0)
            {
                if (!BigInteger.TryParse(P.Text, out _tmpP))
                {
                    wrongP.Visible = true;
                }
                else
                {
                    _paramP = _tmpP;
                    wrongP.Visible = false;
                }

            }
            else
            {
                _paramP = _paramQ * _paramT + BigInteger.One;
                wrongP.Visible = false;
            }
        }
        /// <summary>
        /// Проверка электронной подписи
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Check_Click(object sender, EventArgs e)
        {
            StringBuilder SB = _DSA.CheckDigitalSignature(text.Text, gen.Lines, ref message);
            ch.Clear();
            ch.AppendText(SB.ToString());
        }
    }
}
